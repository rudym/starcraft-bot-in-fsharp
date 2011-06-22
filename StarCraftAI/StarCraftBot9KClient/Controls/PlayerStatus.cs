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
    public partial class PlayerStatus : UserControl
    {
        public PlayerStatus()
        {
            InitializeComponent();
            this.components = this.components ?? this.components; // Bug workaround?
        }

        public void UpdatePlayer(BasicOM.PlayerState state)
        {
            lbMinerals.Text = state.Minerals.ToString();
            lbGas.Text = state.Gas.ToString();
            lbSupply.Text = String.Format("{0} / {1}", state.SupplyUsed, state.SupplyTotal);
        }
    }
}
