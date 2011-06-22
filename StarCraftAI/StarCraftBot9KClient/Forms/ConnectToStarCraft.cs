using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using StarCraftBot9K.StarCraft;

namespace StarCraftBot9KClient.Forms
{
    public partial class ConnectToStarCraft : Form
    {
        private Timer m_timer;
        public Communication.StarCraftConnector Connection { get; protected set; }

        public ConnectToStarCraft()
        {
            InitializeComponent();

            Connection = new Communication.StarCraftConnector();
        }

        private void ConnectToStarCraft_Load(object sender, EventArgs e)
        {
            Connection.BeginListening();
            Connection.HandshakeComplete += 
                new Communication.HandshakeCompleteDelegate(m_connection_HandshakeComplete);

            this.Connection.FlagGivePerfectInformation = cbGivePerfectInfo.Checked;
            this.Connection.FlagAllowUserInput = cbAllowUserInput.Checked;

            // Shameless marque
            m_timer = new Timer();
            m_timer.Interval = 300;
            m_timer.Tick += new EventHandler(t_Tick);
            m_timer.Start();
        }

        void t_Tick(object sender, EventArgs e)
        {
            pbMarquee.Value = (pbMarquee.Value + 2) % pbMarquee.Maximum;
        }

        void m_connection_HandshakeComplete(object sender, EventArgs args)
        {
            m_timer.Stop();
            this.DialogResult = DialogResult.OK;

            /*
            // Fantastic! Our work here is done. However first we need 
            // to hop on the UI thread so we can close this dialog.
            Action closeThisForm = () => this.Close();
            this.Invoke(closeThisForm);
            */
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            m_timer.Stop();
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void cbAllowUserInput_CheckedChanged(object sender, EventArgs e)
        {
            this.Connection.FlagAllowUserInput = cbAllowUserInput.Checked;
        }

        private void cbGivePerfectInfo_CheckedChanged(object sender, EventArgs e)
        {
            this.Connection.FlagGivePerfectInformation = cbGivePerfectInfo.Checked;
        }
    }
}
