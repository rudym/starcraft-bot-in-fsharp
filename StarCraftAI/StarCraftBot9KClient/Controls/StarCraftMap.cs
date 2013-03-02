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
    public partial class StarCraftMap : UserControl
    {
        const int ScaleFactor = 2;

        public StarCraftMap()
        {
            InitializeComponent();

            // Cache some solid brushes for printing grayscale
            m_grayscaleBrushes = new Brush[8];
            for (int i = 0; i < 8; i++)
            {
                int d = 256 / 9 * (i + 1);
                m_grayscaleBrushes[i] = new SolidBrush(Color.FromArgb(d, d, d));
            }

            // Cache unit dimensions
            m_unitDimensionCache = new Dictionary<int, Tuple<int, int>>(128);
        }

        private Dictionary<int, Tuple<int, int>> m_unitDimensionCache;
        private Brush[] m_grayscaleBrushes;

        public IEnumerable<BasicOM.BasicUnitInfo> Units { get; set; }

        private BasicOM.GameMap m_map;
        public BasicOM.GameMap Map
        {
            get
            {
                return m_map;
            }
            set
            {
                if (value == null)
                {
                    // Ignore, just rely on defaults
                }
                else
                {
                    m_map = value;
                    lbMapName.Text = Map.Name;
                    RepaintMap();
                }

                this.Refresh();
            }
        }

        internal void RepaintMap()
        {
            if (Map == null)
                return;

            // Render to a bitmap and display it in the center of the control
            Bitmap mapDisplay = new Bitmap(Map.Width * ScaleFactor, Map.Height * ScaleFactor);
            Graphics g = Graphics.FromImage(mapDisplay);

            for (int y = 0; y < Map.Height; y++)
            {
                for (int x = 0; x < Map.Width; x++)
                {
                    Brush cellColor = null;
                    // Default just color based on height
                    cellColor = m_grayscaleBrushes[Map.HeightAt[x, y]];

                    // Specially mark obsticals
                    if (!Map.Walkable[x, y])
                        cellColor = Brushes.Black;

                    // Each map cell is a ScaleFactor x ScaleFactor pixel grid
                    g.FillRectangle(
                        cellColor,
                        x * ScaleFactor, y * ScaleFactor,
                        ScaleFactor, ScaleFactor);
                }
            }

            // Draw resources
            Brush mineralBrush = Brushes.Teal;
            Brush gasBrush = Brushes.LimeGreen;
            Brush[] playerColors = new Brush[] {
                    Brushes.Pink,
                    Brushes.Blue,
                    Brushes.Red,
                    Brushes.Orange,
                    Brushes.Cyan,
                    Brushes.Brown,
                    Brushes.Violet,
                    Brushes.Tan };

            if (this.Units != null)
            {
                foreach (var unit in this.Units)
                {
                    Brush unitColor = null;
                    
                    // Look up unit width and height
                    if (!m_unitDimensionCache.ContainsKey(unit.TypeID))
                    {
                        foreach (var uti in BasicOM.g_GameMetadata.UnitTypes)
                        {
                            if (uti.ID == unit.TypeID)
                            {
                                m_unitDimensionCache.Add(uti.ID, Tuple.Create(uti.Width, uti.Height));
                                break;
                            }
                        }
                    }

                    int unitWidth = m_unitDimensionCache[unit.TypeID].Item1;
                    int unitHeight = m_unitDimensionCache[unit.TypeID].Item1;
                    switch ((Constants.UnitID)unit.TypeID)
                    {
                        case Constants.UnitID.MineralField: unitColor = mineralBrush; break;
                        case Constants.UnitID.VespeneGeyser: unitColor = gasBrush; break;
                        default: break;
                    }

                    // If it is owned by a player, color the unit there
                    if (unit.Player < 9 && unit.Player > 0)  //&& unit.Player > 0 added after for some reason unit.Player started to return -1
                        unitColor = playerColors[unit.Player];

                    if (unitColor != null)
                    {
                        g.FillRectangle(
                            unitColor,
                            unit.XPos * ScaleFactor, unit.YPos * ScaleFactor,
                            ScaleFactor * unitWidth, ScaleFactor * unitHeight);
                    }
                }
            }

            this.pictureBox1.Image = mapDisplay;
        }
    }
}
