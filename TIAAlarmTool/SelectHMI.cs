using Siemens.Engineering;
using Siemens.Engineering.Hmi;
using Siemens.Engineering.HW;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace TIAAlarmTool
{
    public partial class SelectHMI : Form
    {
        TiaPortal tiaPortal = null;
        public SelectHMI(TiaPortal portal)
        {
            tiaPortal = portal;
            InitializeComponent();
            VisibleChanged += updateList;
            itemTree.MouseDoubleClick += treeDoubleClick;
        }

        protected void updateList(object sender, EventArgs e)
        {
            if (Visible)
            {
                itemTree.Nodes.Clear();
                buildPortalTree(itemTree.Nodes, tiaPortal.Projects);
                selectBtn.Enabled = false;
            }
        }

        class HmiTargetNode : TreeNode
        {
            public HmiTarget hmi;
            public HmiTargetNode(string name, HmiTarget hmi)
                : base(name)
            {
                this.hmi = hmi;
            }

        }
     
        private void buildDeviceItemTree(TreeNodeCollection nodes, IDeviceItemAggregation items)
        {
            foreach (IDeviceItem item in items)
            {
                HmiTarget hmi = item as HmiTarget;
                if (hmi != null)
                {
                    TreeNode itemNode = new HmiTargetNode(item.Name, hmi);
                    nodes.Add(itemNode);
                }
                else
                {
                    buildDeviceItemTree(nodes, item.DeviceItems);
                }
            }
        }
        private void buildDeviceTree(TreeNodeCollection nodes, DeviceAggregation devices)
        {
            foreach (Device d in devices)
            {
                TreeNode deviceNode = new TreeNode(d.Name);
                nodes.Add(deviceNode);
            
                buildDeviceItemTree(deviceNode.Nodes, d.DeviceItems);
            }

        }
        private void buildDeviceFolderTree(TreeNodeCollection nodes, DeviceUserFolderAggregation folders)
        {
            foreach (DeviceUserFolder f in folders)
            {
                TreeNode folderNode = new TreeNode(f.Name);
                nodes.Add(folderNode);
                buildDeviceFolderTree(folderNode.Nodes, f.Folders);
                buildDeviceTree(folderNode.Nodes, f.Devices);
            }
        }
        private void buildProjectTree(TreeNodeCollection nodes, Project proj)
        {
            TreeNode projectNode = new TreeNode(proj.Path);
            nodes.Add(projectNode);

            TreeNode deviceNode = new TreeNode("Devices");
            projectNode.Nodes.Add(deviceNode);
            buildDeviceFolderTree(deviceNode.Nodes, proj.DeviceFolders);
            buildDeviceTree(deviceNode.Nodes, proj.Devices);

        }

        private void buildPortalTree(TreeNodeCollection nodes, ProjectAggregation projs)
        {
            foreach (Project p in projs)
            {
                buildProjectTree(nodes, p);
            }
        }

        private void itemTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = itemTree.SelectedNode;
            if (node is HmiTargetNode)
            {
                selectBtn.Enabled = true;
                SelectedHMI = (node as HmiTargetNode).hmi;
            }
            else
            {
                selectBtn.Enabled = false;
            }
        }

        public HmiTarget SelectedHMI {get; private set; }

        private void treeDoubleClick(object sender, EventArgs e)
        {
            selectBtn.PerformClick();
        }
    }
}
