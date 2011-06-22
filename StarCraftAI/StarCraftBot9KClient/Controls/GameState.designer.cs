namespace StarCraftBot9KClient.Controls
{
    partial class GameState
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.scMapCtrl = new StarCraftBot9KClient.Controls.StarCraftMap();
            this.playerStatusCtrl = new StarCraftBot9KClient.Controls.PlayerStatus();
            this.SuspendLayout();
            // 
            // scMapCtrl
            // 
            this.scMapCtrl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.scMapCtrl.Location = new System.Drawing.Point(0, 26);
            this.scMapCtrl.Map = null;
            this.scMapCtrl.Name = "scMapCtrl";
            this.scMapCtrl.Size = new System.Drawing.Size(254, 173);
            this.scMapCtrl.TabIndex = 0;
            // 
            // playerStatusCtrl
            // 
            this.playerStatusCtrl.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.playerStatusCtrl.Dock = System.Windows.Forms.DockStyle.Top;
            this.playerStatusCtrl.Location = new System.Drawing.Point(0, 0);
            this.playerStatusCtrl.Name = "playerStatusCtrl";
            this.playerStatusCtrl.Size = new System.Drawing.Size(254, 26);
            this.playerStatusCtrl.TabIndex = 0;
            // 
            // GameState
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Controls.Add(this.scMapCtrl);
            this.Controls.Add(this.playerStatusCtrl);
            this.Name = "GameState";
            this.Size = new System.Drawing.Size(254, 199);
            this.ResumeLayout(false);

        }

        #endregion

        private PlayerStatus playerStatusCtrl;
        private StarCraftMap scMapCtrl;
    }
}
