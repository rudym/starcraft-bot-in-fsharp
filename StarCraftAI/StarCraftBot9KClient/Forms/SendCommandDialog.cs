using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using StarCraftBot9K.AI;
using StarCraftBot9K.StarCraft;

namespace StarCraftBot9KClient.Forms
{
    public partial class SendCommandDialog : Form
    {
        public SendCommandDialog()
        {
            InitializeComponent();
        }

        public AIBase.SCCommand Command { get; private set; }

        private void SendCommand_Load(object sender, EventArgs e)
        {
            cbCommand.Items.Clear();
            cbCommand.Items.AddRange(Enum.GetNames(typeof(AIBase.SCCommandID)));

            cbCommand.SelectedIndex = 0;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            try
            {
                const int MagicNumber = 123456789;
                int unit = Int32.Parse(tbUnit.Text);
                int arg0 = MagicNumber;
                int arg1 = MagicNumber;
                int arg2 = MagicNumber;
                
                Int32.TryParse(tbArg0.Text, out arg0);
                Int32.TryParse(tbArg1.Text, out arg1);
                Int32.TryParse(tbArg2.Text, out arg2);

                AIBase.SCCommandID cid = 
                    (AIBase.SCCommandID) Enum.Parse(
                        typeof(AIBase.SCCommandID), 
                        cbCommand.SelectedItem.ToString());

                switch (cid)
                {
                    case AIBase.SCCommandID.HoldPosition :
                        this.Command =
                            AIBase.SCCommand.NewHoldPosition(unit);
                        break;

                    case AIBase.SCCommandID.RightClickPos :
                        if (arg1 == MagicNumber ||
                            arg2 == MagicNumber)
                            throw new InvalidOperationException("Arguments are invalid.");
                        this.Command =
                            AIBase.SCCommand.NewRightClickPos(unit, new BasicOM.Location(arg0, arg1));
                        break;

                    default :
                        throw new InvalidOperationException("Invalid command id.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    "Invalid input. " + ex.Message,
                    "Error",
                     MessageBoxButtons.OK,
                     MessageBoxIcon.Warning);
                return;
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
