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

namespace TIAAlarmTool
{
    public partial class PortalSelect : Form
    {
        public PortalSelect()
        {
            InitializeComponent();
            listBox1.MouseDoubleClick += listDoubleClick;
        }


        // Item that has the processes path as string representation
        private class ProcItem
        {
            public ProcItem(TiaPortalProcess proc)
            {
                this.proc = proc;
            }
            public TiaPortalProcess proc;


            public override string ToString()
            {
                string path = proc.ProjectPath;
                if (path == null)
                {
                    path = "No project loaded";
                }
                return path;
            }
        }
        private void PortalSelect_Load(object sender, EventArgs e)
        {
            listBox1.Items.Clear();
            foreach (TiaPortalProcess proc in TiaPortal.GetProcesses())
            {
                ProcItem item = new ProcItem(proc);
                listBox1.Items.Add(item);
                listBox1.SetSelected(0, true);
            }
            connectBtn.Enabled = (listBox1.SelectedItem != null);
        }

        public TiaPortalProcess selectedProcess()
        {
            if (listBox1.SelectedItem == null) return null;
            return ((ProcItem)listBox1.SelectedItem).proc;
        }

        private void listDoubleClick(object sender, EventArgs e)
        {
            connectBtn.PerformClick();
        }
        
    }
}
