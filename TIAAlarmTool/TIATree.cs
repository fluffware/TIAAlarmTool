using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using Siemens.Engineering;
using Siemens.Engineering.HW;
using Siemens.Engineering.SW;

namespace TIAAlarmTool
{
    public class TIATree
    {
        public delegate bool Filter(Object item); // Predicate for item

        // Discards all device items that isn't a controller or a container
        public static bool ControllerOnly(Object obj)
        {
            if (!(obj is IDeviceItem)) return true;
            IDeviceItem item = (IDeviceItem)obj;
            return (item is ControllerTarget);
        }

        public static bool SharedDBOnly(Object obj)
        {
            return (obj is DataBlock) && ((obj as DataBlock).DatablockType == DatablockType.SharedDB);
        }
        public class TreeNodeBuilder
        {
            public TiaPortal TIA;

        
            public Filter Descend = alwaysTrue; // Examine child items
            public Filter Leaf = alwaysTrue; // Include this item even if no child item is included

          
            class Handler : NodeHandler
            {
                TreeNodeCollection parent = null;
                TreeNode node = null;
                TreeNodeBuilder builder;
                public Handler(TreeNodeBuilder builder, TreeNodeCollection nodes) {
                    parent = nodes;
                    this.builder = builder;
                }

                public override NodeHandler Enter(Object obj, string name)
                {
                    node = new TreeNode(name);
                    node.Tag = obj;
                    if (builder.Descend(obj))
                    {
                        return new Handler(builder, node.Nodes);
                    }
                    else
                    {
                        return null;
                    }
                }

                public override void Exit(Object obj)
                {
                    if (builder.Leaf(obj) || node.Nodes.Count > 0)
                    {
                        parent.Add(node);
                    }
                    node = null;
                }
            }

            protected static bool alwaysTrue(Object obj)
            {
                return true;
            }
            public TreeNodeBuilder(TiaPortal tia)
            {
                TIA = tia;
            }
            public void Build(TreeNodeCollection nodes)
            {
                Handler handler = new Handler(this, nodes);
                Handle(handler, TIA);
            }

            // Expand all nodes with max_children or fewer
            public static void Expand(TreeNodeCollection nodes, int max_children)
            {

                foreach (TreeNode node in nodes)
                {
                    if (node.Nodes.Count <= max_children) node.Expand();
                    Expand(node.Nodes, max_children);
                }
            }
        }

        public class NodeHandler
        {
            /* Called before children are handled. Returns a handler used for the children, if null the children are ignored */
            public virtual NodeHandler Enter(Object obj, string name)
            {
                return this; // Reuse this handler for the children
            }
            /* Called after the children have been handled */
            public virtual void Exit(Object obj)
            {

            }
        }

        private static void handleDataBlock(NodeHandler handler, IBlock block)
        {
            handler.Enter(block, block.Name);
            handler.Exit(block);
        }

        private static void iterDataBlock(NodeHandler handler, IBlockAggregation blocks)
        {
            foreach (IBlock block in blocks)
            {
                handleDataBlock(handler, block);
            }
        }
        private static void handleBlockFolder(NodeHandler handler, ProgramblockUserFolder folder)
        {
            NodeHandler child_handler = handler.Enter(folder.Blocks, folder.Name);
            if (child_handler != null) {       
                iterDataBlock(child_handler, folder.Blocks);
                iterBlockFolder(child_handler, folder.Folders); 
            }
            handler.Exit(folder.Blocks);
        }
        private static void iterBlockFolder(NodeHandler handler, ProgramblockUserFolderAggregation folders)
        {
            foreach (ProgramblockUserFolder folder in folders)
            {
         
                handleBlockFolder(handler, folder);
            }
        }
        private static void handleDeviceItem(NodeHandler handler, IDeviceItem item)
        {
            NodeHandler child_handler = handler.Enter(item, item.Name);
            if (child_handler != null)
            {
                ControllerTarget controller = item as ControllerTarget;
                if (controller != null)
                {
                    NodeHandler block_handler = child_handler.Enter(controller.ProgramblockFolder.Blocks, "Blocks");
                    if (block_handler != null)
                    {
                        iterDataBlock(block_handler, controller.ProgramblockFolder.Blocks);
                        iterBlockFolder(block_handler, controller.ProgramblockFolder.Folders);
                    }
                    child_handler.Exit(controller.ProgramblockFolder.Blocks);
                }
                iterDeviceItem(child_handler, item.DeviceItems);
            }
            handler.Exit(item);
        }
        private static void iterDeviceItem(NodeHandler handler, IDeviceItemAggregation items)
        {
            foreach (IDeviceItem item in items)
            {
                handleDeviceItem(handler, item);
            }
        }

        private static void handleDevice(NodeHandler handler, Device device)
        {

            NodeHandler child_handler = handler.Enter(device, device.Name);
            if (child_handler != null)
            {
                iterDeviceItem(child_handler, device.DeviceItems);
            }
            handler.Exit(device);
        }

        private static void iterDevice(NodeHandler handler, DeviceAggregation devices)
        {
            foreach (Device d in devices)
            {
                handleDevice(handler, d);
            }

        }
        private static void handleDeviceFolder(NodeHandler handler, DeviceUserFolder folder)
        {

            TreeNode node = new TreeNode(folder.Name);
            node.Tag = folder;
            NodeHandler child_handler = handler.Enter(folder, folder.Name);
            if (child_handler != null)
            {
                iterDeviceFolder(child_handler, folder.Folders);
                iterDevice(child_handler, folder.Devices);
            }
            handler.Exit(folder);
        }
        private static void iterDeviceFolder(NodeHandler handler, DeviceUserFolderAggregation folders)
        {
            foreach (DeviceUserFolder f in folders)
            {
                handleDeviceFolder(handler, f);
            }
        }
        private static void handleProject(NodeHandler handler, Project proj)
        {

            TreeNode node = new TreeNode(proj.Path);
            node.Tag = proj;
            NodeHandler child_handler = handler.Enter(proj, proj.Path);
            if (child_handler != null)
            {
                iterDeviceFolder(child_handler, proj.DeviceFolders);
                iterDevice(child_handler, proj.Devices);
            }

            handler.Exit(proj);


        }
        public static void Handle(NodeHandler handler, TiaPortal tia)
        {
            foreach (Project p in tia.Projects)
            {
                handleProject(handler, p);
            }

        }

    }
}
