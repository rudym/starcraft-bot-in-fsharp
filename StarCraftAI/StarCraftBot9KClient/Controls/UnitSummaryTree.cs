using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using StarCraftBot9K.StarCraft;

namespace StarCraftBot9KClient.Controls
{
    public partial class UnitSummaryTree : UserControl
    {
        /// <summary>
        /// Serves as the child node index for tree view nodes
        /// </summary>
        const int UnitDataIndex_Order = 0;

        public UnitSummaryTree()
        {
            InitializeComponent();

            this.components = this.components ?? this.components; // Bug workaround?

            tvRootTree.Nodes.Clear();
            m_playertn = new TreeNode("Player");
            m_resourcestn = new TreeNode("Resources");
            m_critterstn = new TreeNode("Critters");
            tvRootTree.Nodes.Add(m_playertn);
            tvRootTree.Nodes.Add(m_resourcestn);

            m_unitCache = new Dictionary<int, TreeNode>();
            m_playerCache = new Dictionary<int, TreeNode>();
        }

        private TreeNode m_playertn;
        private TreeNode m_resourcestn;
        private TreeNode m_critterstn;

        private Dictionary<int, TreeNode> m_unitCache;
        private Dictionary<int, TreeNode> m_playerCache;

        public void UpdateUnits(BasicOM.PlayerState state)
        {
            if (!m_playerCache.ContainsKey(BasicOM.g_GameMetadata.PlayerID))
                m_playerCache.Add(BasicOM.g_GameMetadata.PlayerID, this.m_playertn);

            tvRootTree.BeginUpdate();
            // Keep track of units which are still around, so we can remove dead ones from our tree
            HashSet<int> unitsAlive = new HashSet<int>();

            foreach (var unit in state.Units)
            {
                unitsAlive.Add(unit.ID);

                TreeNode unitNode = null;
                if (!m_unitCache.ContainsKey(unit.ID))
                {
                    TreeNode newNode = new TreeNode(String.Format("{0} - {1}", BasicOM.getUnitName(unit), unit.ID));
                    TreeNode orderNode = new TreeNode(String.Format("Order - {0}", ((Constants.UnitOrder) unit.OrderID).ToString()));
                    orderNode.Tag = unit.OrderID;
    
                    // NOTE: These HAVE to be added in the same order as outlined in UnitDataIndex
                    newNode.Nodes.Add(orderNode);

                    // So where does this go?
                    if (unit.TypeID == (int) Constants.UnitID.MineralField ||
                        unit.TypeID == (int) Constants.UnitID.VespeneGeyser)
                    {
                        m_resourcestn.Nodes.Add(newNode);
                    }
                    else if (unit.TypeID == (int)Constants.UnitID.CritterKakaru)
                    {
                        m_critterstn.Nodes.Add(newNode);
                    }
                    else if (unit.Player == BasicOM.g_GameMetadata.PlayerID)
                    {
                        m_playertn.Nodes.Add(newNode);
                    }
                    else
                    {
                        if (!m_playerCache.ContainsKey(unit.Player))
                        {
                            TreeNode newPlayerNode = new TreeNode("Enemy " + unit.Player.ToString());
                            m_playerCache.Add(unit.Player, newPlayerNode);
                            tvRootTree.Nodes.Add(newPlayerNode);
                        }
                        m_playerCache[unit.Player].Nodes.Add(newNode);
                    }

                    m_unitCache.Add(unit.ID, newNode);
                }

                unitNode = m_unitCache[unit.ID];

                // Update the unit node's children (if needed)
                if ((int)unitNode.Nodes[UnitDataIndex_Order].Tag != unit.OrderID)
                {
                    unitNode.Nodes[UnitDataIndex_Order].Text = String.Format("Order - {0}", ((Constants.UnitOrder)unit.OrderID).ToString());
                    unitNode.Nodes[UnitDataIndex_Order].Tag = unit.OrderID;
                }
            }

            // Loop through one more time to remove any dead units
            foreach (var item in m_unitCache)
            {
                if (!unitsAlive.Contains(item.Key))
                {
                    item.Value.Remove();
                }
            }
            
            tvRootTree.EndUpdate();
        }
    }
}
