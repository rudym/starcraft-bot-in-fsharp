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
    public partial class GameState : UserControl
    {
        public GameState()
        {
            InitializeComponent();
            this.components = this.components ?? this.components; // Bug workaround?
        }

        internal void UpdateMap(BasicOM.GameMap map)
        {
            scMapCtrl.Map = map;
        }

        internal void UpdateState(BasicOM.PlayerState state)
        {
            this.scMapCtrl.Units = state.Units;
            this.scMapCtrl.RepaintMap();

            this.playerStatusCtrl.UpdatePlayer(state);
        }
    }
}
