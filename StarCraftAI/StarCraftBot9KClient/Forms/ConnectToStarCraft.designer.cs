namespace StarCraftBot9KClient.Forms
{
    partial class ConnectToStarCraft
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.pbMarquee = new System.Windows.Forms.ProgressBar();
            this.label1 = new System.Windows.Forms.Label();
            this.cbAllowUserInput = new System.Windows.Forms.CheckBox();
            this.cbGivePerfectInfo = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // pbMarquee
            // 
            this.pbMarquee.Location = new System.Drawing.Point(12, 29);
            this.pbMarquee.Name = "pbMarquee";
            this.pbMarquee.Size = new System.Drawing.Size(305, 57);
            this.pbMarquee.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
            this.pbMarquee.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Waiting for StarCraft";
            // 
            // cbAllowUserInput
            // 
            this.cbAllowUserInput.AutoSize = true;
            this.cbAllowUserInput.Checked = true;
            this.cbAllowUserInput.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbAllowUserInput.Location = new System.Drawing.Point(15, 115);
            this.cbAllowUserInput.Name = "cbAllowUserInput";
            this.cbAllowUserInput.Size = new System.Drawing.Size(100, 17);
            this.cbAllowUserInput.TabIndex = 4;
            this.cbAllowUserInput.Text = "Allow user input";
            this.cbAllowUserInput.UseVisualStyleBackColor = true;
            this.cbAllowUserInput.CheckedChanged += new System.EventHandler(this.cbAllowUserInput_CheckedChanged);
            // 
            // cbGivePerfectInfo
            // 
            this.cbGivePerfectInfo.AutoSize = true;
            this.cbGivePerfectInfo.Checked = true;
            this.cbGivePerfectInfo.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbGivePerfectInfo.Location = new System.Drawing.Point(15, 138);
            this.cbGivePerfectInfo.Name = "cbGivePerfectInfo";
            this.cbGivePerfectInfo.Size = new System.Drawing.Size(138, 17);
            this.cbGivePerfectInfo.TabIndex = 5;
            this.cbGivePerfectInfo.Text = "Give perfect information";
            this.cbGivePerfectInfo.UseVisualStyleBackColor = true;
            this.cbGivePerfectInfo.CheckedChanged += new System.EventHandler(this.cbGivePerfectInfo_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(13, 96);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 13);
            this.label2.TabIndex = 6;
            this.label2.Text = "Flags";
            // 
            // btnCancel
            // 
            this.btnCancel.Location = new System.Drawing.Point(127, 173);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // ConnectToStarCraft
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(329, 208);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbGivePerfectInfo);
            this.Controls.Add(this.cbAllowUserInput);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pbMarquee);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ConnectToStarCraft";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Please launch StarCraft";
            this.Load += new System.EventHandler(this.ConnectToStarCraft_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ProgressBar pbMarquee;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox cbAllowUserInput;
        private System.Windows.Forms.CheckBox cbGivePerfectInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel;

    }
}