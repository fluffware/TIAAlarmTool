using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Siemens.Engineering;
using Siemens.Engineering.SW;
using Siemens.Engineering.HW;
using Siemens.Engineering.Hmi;
using Siemens.Engineering.Hmi.Communication;

namespace TIAAlarmTool
{
    public partial class MainForm : Form
    {
        AlarmDefinitionList defs;
        public MainForm()
        {
            InitializeComponent();
            alarmList.CellFormatting +=
            new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.cellFormatter);
            defs = new AlarmDefinitionList();
        }

        private void saveAlarmDefinitionsToolStripMenuItem_Click(object sender, EventArgs ev)
        {
            if (saveFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    defs.Save(saveFileDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to save alarm definitions: " + ex.Message);
                }

            }
        }

        private void cellFormatter(object sender,
        System.Windows.Forms.DataGridViewCellFormattingEventArgs e)
        {
            string colName = alarmList.Columns[e.ColumnIndex].Name;
            if (colName.Equals("ColumnID"))
            {
                if (e.Value is Int32)
                {
                    int id = (int)e.Value;
                    if (id < 0)
                    {
                        e.Value = "-";
                    }


                }
            }
            else if (colName.Equals("ColumnSilent"))
            {

                if (e.Value is AlarmDefinition.Option)
                {

                    e.Value = (((AlarmDefinition.Option)e.Value) & AlarmDefinition.Option.Silent) != 0;
                }
            }
            else if (colName.Equals("ColumnAutoAck"))
            {

                if (e.Value is AlarmDefinition.Option)
                {

                    e.Value = (((AlarmDefinition.Option)e.Value) & AlarmDefinition.Option.AutoAck) != 0;
                }
            }
        }
        private void loadAlarmDefinitionsToolStripMenuItem_Click(object sender, EventArgs ev)
        {
            if (loadFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                try
                {
                    alarmList.DataSource = null;
                    defs.Load(loadFileDialog.FileName);
                    defs.ValidateID();
                    alarmList.AutoGenerateColumns = false;
                    alarmList.DataSource = defs;

                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load alarm definitions: " + ex.Message);
                }

            }
        }

        TiaPortal tiaPortal = null;
        PortalSelect select_dialog = null;
        private void connectToolStripMenuItem_Click(object sender, EventArgs e)
        {



            if (select_dialog == null)
            {
                select_dialog = new PortalSelect();
            }
            if (select_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                TiaPortalProcess proc = select_dialog.selectedProcess();
                if (proc != null)
                {
                    WaitConnect wait = new WaitConnect();
                    wait.Show();
                    Application.DoEvents();
                    try
                    {
                        tiaPortal = proc.Attach();
                        connectToolStripMenuItem.Enabled = false;
                        disconnectToolStripMenuItem.Enabled = true;
                    }
                    catch (EngineeringException ex)
                    {
                        MessageBox.Show("Failed to connect to TIAPortal: " + ex.Message);
                    }
                    wait.Hide();
                    wait.Dispose();

                }
            }


        }

        private void disconnectToolStripMenuItem_Click(object sender, EventArgs e)
        {

            tiaPortal.Dispose();
            tiaPortal = null;
            folder_dialog = null;
            data_block_dialog = null;
            browse_dialog = null;
            connectToolStripMenuItem.Enabled = true;
            disconnectToolStripMenuItem.Enabled = false;


        }

        // Generate alarm definitions in project
        BrowseDialog folder_dialog;
        private void alarmDefsToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (tiaPortal != null)
            {
                if (folder_dialog == null)
                {
                    folder_dialog = new BrowseDialog(tiaPortal);
                    folder_dialog.Descend = TIATree.ControllerOnly;
                    folder_dialog.Leaf = (o => o is IBlockAggregation);
                    folder_dialog.AutoExpandMaxChildren = 1;
                    folder_dialog.AcceptText = "Generate";
                    folder_dialog.Text = "Select where to generate block";
                }
                if (folder_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (folder_dialog.SelectedObject is IBlockAggregation)
                    {
                        IBlockAggregation folder = (IBlockAggregation)folder_dialog.SelectedObject;
                        string filename = AlarmDB.buildFile(defs);

                        try
                        {
                            folder.Import(filename, ImportOptions.Override);
                        }
                        catch (Siemens.Engineering.EngineeringException ex)
                        {
                            MessageBox.Show(this, "Failed to import alarm definitions: " + ex.Message);
                            System.IO.File.Delete(filename);
                            return;
                        }

                        System.IO.File.Delete(filename);

                        // Move up the tree until we find a ControllerTarget
                        IEngineeringObject obj = folder.Parent;
                        while (!(obj is ControllerTarget))
                        {
                            obj = obj.Parent;
                            // Shouldn't happen, but just in case
                            if (obj == null)
                            {
                                MessageBox.Show(this, "No controller found as parent");
                                return;
                            }
                        }
                        ControllerTarget controller = (ControllerTarget)obj;
                        List<ConstTable.Constant> consts = new List<ConstTable.Constant>();
                        foreach (AlarmDefinition alarm in defs)
                        {
                            consts.Add(new ConstTable.Constant("Alarm" + alarm.Name, alarm.ID, alarm.Text));
                        }
                        filename = ConstTable.buildFile("Alarms", consts);
                        try
                        {
                            controller.ControllerTagFolder.TagTables.Import(filename, ImportOptions.Override);
                        }
                        catch (Siemens.Engineering.EngineeringException ex)
                        {
                            MessageBox.Show(this, "Failed to import constants: " + ex.Message);
                            System.IO.File.Delete(filename);
                            return;
                        }

                        System.IO.File.Delete(filename);
                    }
                }

            }
        }

        private void alarmList_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        SelectHMI hmi_dialog;
        private void HMITagsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (tiaPortal != null)
            {
                if (hmi_dialog == null)
                {
                    hmi_dialog = new SelectHMI(tiaPortal);

                }
                if (hmi_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string filename = HMIAlarmTags.buildFile(defs, "sDB_LarmDefs", "HMI_Connection");
                    Console.WriteLine("HMI tags file: " + filename);
                    HmiTarget hmi = hmi_dialog.SelectedHMI;
                    Console.WriteLine("HMI name: " + hmi.Name);
                    
                    if (hmi.Connections.Count != 1) {
                          MessageBox.Show(this, "Can only handle exacltly one HMI connection. This device has "+hmi.Connections.Count);
                            System.IO.File.Delete(filename);
                            return;
                    }
                    Connection c = hmi.Connections.First();
                    c.Export(filename, ExportOptions.WithDefaults | ExportOptions.WithReadOnly);
                }
            }


        }
          BrowseDialog data_block_dialog;
          private void extractAlarmDefsToolStripMenuItem_Click(object sender, EventArgs e)
          {
              if (tiaPortal != null)
              {
                  if (data_block_dialog == null)
                  {
                      data_block_dialog = new BrowseDialog(tiaPortal);
                      data_block_dialog.Descend = TIATree.ControllerOnly;
                      data_block_dialog.Leaf = TIATree.SharedDBOnly;
                      data_block_dialog.AutoExpandMaxChildren = 1;
                      data_block_dialog.AcceptText = "Extract";
                      data_block_dialog.Text = "Select alarm data block";
                  }
                  if (data_block_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                  {
                      if (data_block_dialog.SelectedObject is DataBlock)
                      {
                          DataBlock block = (DataBlock)data_block_dialog.SelectedObject;
                          try
                          {

                              // Extract from data base
                              string filename = TempFile.Name("AlarmDB", "xml");
                           
                              block.Export(filename, ExportOptions.WithDefaults | ExportOptions.WithReadOnly);
                              alarmList.DataSource = null;
                              AlarmDB.parseFile(defs, filename);
                              System.IO.File.Delete(filename);

                              // Extract from constant tags 
                              // Move up the tree until we find a ControllerTarget
                              IEngineeringObject obj = (block as IEngineeringObject).Parent;
                              while (!(obj is ControllerTarget))
                              {
                                  obj = obj.Parent;
                                  // Shouldn't happen, but just in case
                                  if (obj == null)
                                  {
                                      MessageBox.Show(this, "No controller found as parent");
                                      return;
                                  }
                              }
                              ControllerTarget controller = (ControllerTarget)obj;
                              List<ConstTable.Constant> consts = new List<ConstTable.Constant>();

                              ControllerTagTable table = controller.ControllerTagFolder.TagTables.Find("Alarms");
                              if (table == null)
                              {
                                  MessageBox.Show(this, "No tag table named Alarms was found");
                              }
                              else
                              {
                                  filename = TempFile.Name("ConstantTags", "xml");
                                  Console.WriteLine("Wrote to " + filename);
                                  table.Export(filename, ExportOptions.WithDefaults | ExportOptions.WithReadOnly);
                                  List<ConstTable.Constant> constants = ConstTable.getConstants(filename);
                                  foreach (ConstTable.Constant c in constants)
                                  {
                                      if (c.Name.StartsWith("Alarm") && c.Value is int)
                                      {
                                          AlarmDefinition a = defs.FindByID((int)c.Value);
                                          if (a != null)
                                          {
                                              a.Name = c.Name.Substring(5);
                                          }
                                      }
                                  }
                              }
                              defs.ValidateID();

                              alarmList.AutoGenerateColumns = false;
                              alarmList.DataSource = defs;
                          }
                          catch (Exception ex)
                          {
                              MessageBox.Show("Failed to extract alarm definitions: " + ex.Message);
                          }


                      }
                  }
              }
          }

          BrowseDialog browse_dialog;
          private void browseToolStripMenuItem_Click(object sender, EventArgs e)
          {
              if (tiaPortal != null)
              {
                  if (browse_dialog == null)
                  {
                      browse_dialog = new BrowseDialog(tiaPortal);
                      browse_dialog.Descend = TIATree.ControllerOnly;
                      browse_dialog.Leaf = TIATree.SharedDBOnly;
                      browse_dialog.AutoExpandMaxChildren = 1;
                      browse_dialog.AcceptText = "Select";
                      browse_dialog.Text = "Browse TIA portal";
                  }
                  if (browse_dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                  {
                  }
              }
          }
    }
}
