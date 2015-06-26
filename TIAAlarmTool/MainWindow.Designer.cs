namespace TIAAlarmTool
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadAlarmDefinitionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAlarmDefinitionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tIAPortalToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.connectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.disconnectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.browseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.generateToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alarmDefsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.hMITagsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.extractAlarmDefsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.loadFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.alarmList = new System.Windows.Forms.DataGridView();
            this.ColumnID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ColumnSilent = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ColumnAutoAck = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.saveFileDialog = new System.Windows.Forms.SaveFileDialog();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alarmList)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.tIAPortalToolStripMenuItem,
            this.generateToolStripMenuItem,
            this.extractToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(484, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.loadAlarmDefinitionsToolStripMenuItem,
            this.saveAlarmDefinitionsToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "File";
            // 
            // loadAlarmDefinitionsToolStripMenuItem
            // 
            this.loadAlarmDefinitionsToolStripMenuItem.Name = "loadAlarmDefinitionsToolStripMenuItem";
            this.loadAlarmDefinitionsToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.loadAlarmDefinitionsToolStripMenuItem.Text = "Load alarm definitions";
            this.loadAlarmDefinitionsToolStripMenuItem.Click += new System.EventHandler(this.loadAlarmDefinitionsToolStripMenuItem_Click);
            // 
            // saveAlarmDefinitionsToolStripMenuItem
            // 
            this.saveAlarmDefinitionsToolStripMenuItem.Name = "saveAlarmDefinitionsToolStripMenuItem";
            this.saveAlarmDefinitionsToolStripMenuItem.Size = new System.Drawing.Size(192, 22);
            this.saveAlarmDefinitionsToolStripMenuItem.Text = "Save alarm definitions";
            this.saveAlarmDefinitionsToolStripMenuItem.Click += new System.EventHandler(this.saveAlarmDefinitionsToolStripMenuItem_Click);
            // 
            // tIAPortalToolStripMenuItem
            // 
            this.tIAPortalToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectToolStripMenuItem,
            this.disconnectToolStripMenuItem,
            this.browseToolStripMenuItem});
            this.tIAPortalToolStripMenuItem.Name = "tIAPortalToolStripMenuItem";
            this.tIAPortalToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
            this.tIAPortalToolStripMenuItem.Text = "TIAPortal";
            // 
            // connectToolStripMenuItem
            // 
            this.connectToolStripMenuItem.Name = "connectToolStripMenuItem";
            this.connectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.connectToolStripMenuItem.Text = "Connect";
            this.connectToolStripMenuItem.Click += new System.EventHandler(this.connectToolStripMenuItem_Click);
            // 
            // disconnectToolStripMenuItem
            // 
            this.disconnectToolStripMenuItem.Name = "disconnectToolStripMenuItem";
            this.disconnectToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.disconnectToolStripMenuItem.Text = "Disconnect";
            this.disconnectToolStripMenuItem.Click += new System.EventHandler(this.disconnectToolStripMenuItem_Click);
            // 
            // browseToolStripMenuItem
            // 
            this.browseToolStripMenuItem.Name = "browseToolStripMenuItem";
            this.browseToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.browseToolStripMenuItem.Text = "Browse";
            this.browseToolStripMenuItem.Click += new System.EventHandler(this.browseToolStripMenuItem_Click);
            // 
            // generateToolStripMenuItem
            // 
            this.generateToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alarmDefsToolStripMenuItem,
            this.hMITagsToolStripMenuItem});
            this.generateToolStripMenuItem.Name = "generateToolStripMenuItem";
            this.generateToolStripMenuItem.Size = new System.Drawing.Size(66, 20);
            this.generateToolStripMenuItem.Text = "Generate";
            // 
            // alarmDefsToolStripMenuItem
            // 
            this.alarmDefsToolStripMenuItem.Name = "alarmDefsToolStripMenuItem";
            this.alarmDefsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.alarmDefsToolStripMenuItem.Text = "Alarm defs";
            this.alarmDefsToolStripMenuItem.Click += new System.EventHandler(this.alarmDefsToolStripMenuItem_Click);
            // 
            // hMITagsToolStripMenuItem
            // 
            this.hMITagsToolStripMenuItem.Name = "hMITagsToolStripMenuItem";
            this.hMITagsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.hMITagsToolStripMenuItem.Text = "HMI tags";
            this.hMITagsToolStripMenuItem.Click += new System.EventHandler(this.HMITagsToolStripMenuItem_Click);
            // 
            // extractToolStripMenuItem
            // 
            this.extractToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.extractAlarmDefsToolStripMenuItem});
            this.extractToolStripMenuItem.Name = "extractToolStripMenuItem";
            this.extractToolStripMenuItem.Size = new System.Drawing.Size(54, 20);
            this.extractToolStripMenuItem.Text = "Extract";
            // 
            // extractAlarmDefsToolStripMenuItem
            // 
            this.extractAlarmDefsToolStripMenuItem.Name = "extractAlarmDefsToolStripMenuItem";
            this.extractAlarmDefsToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.extractAlarmDefsToolStripMenuItem.Text = "Alarm defs";
            this.extractAlarmDefsToolStripMenuItem.Click += new System.EventHandler(this.extractAlarmDefsToolStripMenuItem_Click);
            // 
            // loadFileDialog
            // 
            this.loadFileDialog.DefaultExt = "xlsx";
            this.loadFileDialog.FileName = "AlarmDefs";
            this.loadFileDialog.Filter = "EXCEL files (*.xlsx)|*.xlsx";
            this.loadFileDialog.ValidateNames = false;
            // 
            // alarmList
            // 
            this.alarmList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.alarmList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColumnID,
            this.ColumnName,
            this.ColumnText,
            this.ColumnSilent,
            this.ColumnAutoAck});
            this.alarmList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.alarmList.Location = new System.Drawing.Point(0, 24);
            this.alarmList.Name = "alarmList";
            this.alarmList.Size = new System.Drawing.Size(484, 238);
            this.alarmList.TabIndex = 2;
            this.alarmList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.alarmList_CellContentClick);
            // 
            // ColumnID
            // 
            this.ColumnID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ColumnID.DataPropertyName = "ID";
            this.ColumnID.HeaderText = "ID";
            this.ColumnID.MinimumWidth = 30;
            this.ColumnID.Name = "ColumnID";
            this.ColumnID.ReadOnly = true;
            this.ColumnID.Width = 50;
            // 
            // ColumnName
            // 
            this.ColumnName.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnName.DataPropertyName = "Name";
            this.ColumnName.HeaderText = "Name";
            this.ColumnName.MinimumWidth = 50;
            this.ColumnName.Name = "ColumnName";
            this.ColumnName.ReadOnly = true;
            // 
            // ColumnText
            // 
            this.ColumnText.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.ColumnText.DataPropertyName = "Text";
            this.ColumnText.FillWeight = 200F;
            this.ColumnText.HeaderText = "Text";
            this.ColumnText.MinimumWidth = 50;
            this.ColumnText.Name = "ColumnText";
            this.ColumnText.ReadOnly = true;
            // 
            // ColumnSilent
            // 
            this.ColumnSilent.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ColumnSilent.DataPropertyName = "Options";
            this.ColumnSilent.HeaderText = "Silent";
            this.ColumnSilent.Name = "ColumnSilent";
            this.ColumnSilent.ReadOnly = true;
            this.ColumnSilent.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnSilent.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ColumnSilent.Width = 70;
            // 
            // ColumnAutoAck
            // 
            this.ColumnAutoAck.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.ColumnAutoAck.DataPropertyName = "Options";
            this.ColumnAutoAck.HeaderText = "Auto Ack";
            this.ColumnAutoAck.Name = "ColumnAutoAck";
            this.ColumnAutoAck.ReadOnly = true;
            this.ColumnAutoAck.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ColumnAutoAck.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.ColumnAutoAck.Width = 70;
            // 
            // saveFileDialog
            // 
            this.saveFileDialog.DefaultExt = "xlsx";
            this.saveFileDialog.FileName = "AlarmDefs";
            this.saveFileDialog.Filter = "EXCEL files (*.xlsx)|*.xlsx";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 262);
            this.Controls.Add(this.alarmList);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "MainForm";
            this.Text = "Alarm tool";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.alarmList)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem loadAlarmDefinitionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAlarmDefinitionsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem tIAPortalToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem connectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem disconnectToolStripMenuItem;
        private System.Windows.Forms.OpenFileDialog loadFileDialog;
        private System.Windows.Forms.DataGridView alarmList;
        private System.Windows.Forms.ToolStripMenuItem generateToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alarmDefsToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnID;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ColumnText;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnSilent;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColumnAutoAck;
        private System.Windows.Forms.ToolStripMenuItem hMITagsToolStripMenuItem;
        private System.Windows.Forms.SaveFileDialog saveFileDialog;
        private System.Windows.Forms.ToolStripMenuItem extractToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem extractAlarmDefsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem browseToolStripMenuItem;

    }
}

