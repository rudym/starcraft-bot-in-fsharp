using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using StarCraftBot9K.StarCraft;
using StarCraftBot9K.AI;

using StarCraftBot9KClient.Forms;
using StarCraftBot9KClient.Controls;

namespace StarCraftBot9KClient
{
    public partial class MainForm : Form
    {
        private Communication.StarCraftConnector m_connection = null;
        private Communication.GameMediator m_mediator = null;

        public MainForm()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            statusLabel.Text = "Not Connected";

            ConnectToStarCraft connectionDlg = new ConnectToStarCraft();
            if (connectionDlg.ShowDialog(this) == DialogResult.Cancel)
            {
                this.Close();
                return;
            }

            m_connection = connectionDlg.Connection;
            statusLabel.Text = "Connected";

            if (m_connection.State != Communication.CommState.ListeningForUpdates)
            {
                MessageBox.Show(
                    "Error connecting to StarCraft. Application will now exit.",
                    "Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                this.Close();
                return;
            }

            m_connection.GameStateUpdated +=
                new Communication.PlayerStateRecievedDelegate(m_connection_GameStateUpdated);

            // Setup the mediator to enable AI modules
            m_mediator = new Communication.GameMediator(m_connection);

            // Update UI on the UI thread
            Action updateUI = () => gameStateControl.UpdateMap(m_connection.GameMetadata.Map);
            gameStateControl.Invoke(updateUI);
        }

        void m_connection_GameStateUpdated(object sender, BasicOM.PlayerState state)
        {
            // Update UI on the UI thread
            Action updateUI = () => gameStateControl.UpdateState(state);
            gameStateControl.Invoke(updateUI);

            Action updateUI2 = () => unitSummaryTree1.UpdateUnits(state);
            unitSummaryTree1.Invoke(updateUI2);

            // Dump data to a file
            /*
            StringBuilder unitDump = new StringBuilder();
            unitDump.AppendLine(DateTime.Now.ToLongDateString());
            foreach (var unit in state.Units)
            {
                unitDump.AppendLine("\t" + unit.ToString());
            }
            System.IO.File.AppendAllText("Log.txt", unitDump.ToString());
            */
        }

        private void btnIssueCommand_Click(object sender, EventArgs e)
        {
            SendCommandDialog scd = new SendCommandDialog();
            if (scd.ShowDialog() == DialogResult.OK)
            {
                m_connection.QueueCommand(scd.Command);
            }
        }

        private void cbEnableEconomyAI_CheckedChanged(object sender, EventArgs e)
        {
            // If it just got unchecked, stop the AI
            if (cbEnableEconomyAI.Checked == false)
                EconomyAI.stopEconomyAI();
            else
                EconomyAI.startEconomyAI(m_mediator);
        }
    }
}
