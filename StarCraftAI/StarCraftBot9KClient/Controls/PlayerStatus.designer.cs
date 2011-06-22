namespace StarCraftBot9KClient.Controls
{
    partial class PlayerStatus
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
            System.Windows.Forms.PictureBox pictureBox2;
            System.Windows.Forms.PictureBox pictureBox1;
            System.Windows.Forms.PictureBox pictureBox3;
            this.lbMinerals = new System.Windows.Forms.Label();
            this.lbGas = new System.Windows.Forms.Label();
            this.lbSupply = new System.Windows.Forms.Label();
            pictureBox2 = new System.Windows.Forms.PictureBox();
            pictureBox1 = new System.Windows.Forms.PictureBox();
            pictureBox3 = new System.Windows.Forms.PictureBox();
            this.SuspendLayout();
            // 
            // pictureBox2
            // 
            pictureBox2.Image = global::StarCraftBot9KClient.Properties.Resources.Gas;
            pictureBox2.Location = new System.Drawing.Point(61, 4);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new System.Drawing.Size(16, 16);
            pictureBox2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox2.TabIndex = 1;
            pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = global::StarCraftBot9KClient.Properties.Resources.Minerals;
            pictureBox1.Location = new System.Drawing.Point(4, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new System.Drawing.Size(16, 16);
            pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox1.TabIndex = 0;
            pictureBox1.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = global::StarCraftBot9KClient.Properties.Resources.Supply;
            pictureBox3.Location = new System.Drawing.Point(117, 4);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new System.Drawing.Size(16, 16);
            pictureBox3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            pictureBox3.TabIndex = 2;
            pictureBox3.TabStop = false;
            // 
            // lbMinerals
            // 
            this.lbMinerals.AutoSize = true;
            this.lbMinerals.ForeColor = System.Drawing.Color.Lime;
            this.lbMinerals.Location = new System.Drawing.Point(26, 7);
            this.lbMinerals.Name = "lbMinerals";
            this.lbMinerals.Size = new System.Drawing.Size(31, 13);
            this.lbMinerals.TabIndex = 3;
            this.lbMinerals.Text = "9999";
            // 
            // lbGas
            // 
            this.lbGas.AutoSize = true;
            this.lbGas.ForeColor = System.Drawing.Color.Lime;
            this.lbGas.Location = new System.Drawing.Point(83, 7);
            this.lbGas.Name = "lbGas";
            this.lbGas.Size = new System.Drawing.Size(31, 13);
            this.lbGas.TabIndex = 4;
            this.lbGas.Text = "9999";
            // 
            // lbSupply
            // 
            this.lbSupply.AutoSize = true;
            this.lbSupply.ForeColor = System.Drawing.Color.Lime;
            this.lbSupply.Location = new System.Drawing.Point(139, 7);
            this.lbSupply.Name = "lbSupply";
            this.lbSupply.Size = new System.Drawing.Size(54, 13);
            this.lbSupply.TabIndex = 5;
            this.lbSupply.Text = "200 / 200";
            // 
            // PlayerStatus
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.Controls.Add(this.lbSupply);
            this.Controls.Add(this.lbGas);
            this.Controls.Add(this.lbMinerals);
            this.Controls.Add(pictureBox3);
            this.Controls.Add(pictureBox2);
            this.Controls.Add(pictureBox1);
            this.Name = "PlayerStatus";
            this.Size = new System.Drawing.Size(198, 26);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbMinerals;
        private System.Windows.Forms.Label lbGas;
        private System.Windows.Forms.Label lbSupply;

    }
}
