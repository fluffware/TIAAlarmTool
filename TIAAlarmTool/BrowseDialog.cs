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
using Siemens.Engineering.HW;
using Siemens.Engineering.SW;

namespace TIAAlarmTool
{
    public partial class BrowseDialog : Form
    {
        TIATree.TreeNodeBuilder builder;
        public BrowseDialog(TiaPortal portal)
        {
            InitializeComponent();
            AutoExpandMaxChildren = -1;
            builder = new TIATree.TreeNodeBuilder(portal);
            VisibleChanged += updateList;
            blockTree.MouseDoubleClick += treeDoubleClick;
        }


        public TIATree.Filter Descend
        {
            get { return builder.Descend; }
            set { builder.Descend = value; }
        }

        public TIATree.Filter Leaf
        {
            get { return builder.Leaf; }
            set { builder.Leaf = value; }
        }

        public string AcceptText { get { return OKBtn.Text; }  set {OKBtn.Text = value;}}
        public int AutoExpandMaxChildren { get; set; }
       
        protected void updateList(object sender, EventArgs e)
        {
            if (Visible)
            {
                blockTree.Nodes.Clear();
                builder.Build(blockTree.Nodes);
                if (AutoExpandMaxChildren > 0)
                {
                    TIATree.TreeNodeBuilder.Expand(blockTree.Nodes, AutoExpandMaxChildren);
                }
                OKBtn.Enabled = false;
            }
        }

        private void treeDoubleClick(object sender, EventArgs e)
        {
            OKBtn.PerformClick();
        }

        public Object SelectedObject { get; protected set; }

        private void blockTree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            TreeNode node = blockTree.SelectedNode;
            if (Leaf(node.Tag))
            {
                OKBtn.Enabled = true;
                SelectedObject = node.Tag;
            }
            else
            {
                OKBtn.Enabled = false;
            }
        }
    }
}
