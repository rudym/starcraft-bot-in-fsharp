using StarCraftBot9KClient.Controls;
namespace StarCraftBot9KClient
{
    partial class MainForm
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
            System.Windows.Forms.FlowLayoutPanel flowLayoutPanel1;
            this.unitSummaryTree1 = new StarCraftBot9KClient.Controls.UnitSummaryTree();
            this.ssBotStatus = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.gameStateControl = new StarCraftBot9KClient.Controls.GameState();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.btnIssueCommand = new System.Windows.Forms.Button();
            this.cbEnableEconomyAI = new System.Windows.Forms.CheckBox();
            flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.ssBotStatus.SuspendLayout();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // unitSummaryTree1
            // 
            this.unitSummaryTree1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.unitSummaryTree1.Location = new System.Drawing.Point(3, 3);
            this.unitSummaryTree1.Name = "unitSummaryTree1";
            this.unitSummaryTree1.Size = new System.Drawing.Size(232, 353);
            this.unitSummaryTree1.TabIndex = 0;
            // 
            // ssBotStatus
            // 
            this.ssBotStatus.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel});
            this.ssBotStatus.Location = new System.Drawing.Point(0, 439);
            this.ssBotStatus.Name = "ssBotStatus";
            this.ssBotStatus.Padding = new System.Windows.Forms.Padding(1, 0, 10, 0);
            this.ssBotStatus.Size = new System.Drawing.Size(714, 22);
            this.ssBotStatus.TabIndex = 0;
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(135, 17);
            this.statusLabel.Text = "<StarCraftBot9K Status>";
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tableLayoutPanel1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.gameStateControl);
            this.splitContainer1.Size = new System.Drawing.Size(714, 439);
            this.splitContainer1.SplitterDistance = 238;
            this.splitContainer1.TabIndex = 7;
            // 
            // gameStateControl
            // 
            this.gameStateControl.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.gameStateControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.gameStateControl.Location = new System.Drawing.Point(0, 0);
            this.gameStateControl.Name = "gameStateControl";
            this.gameStateControl.Size = new System.Drawing.Size(472, 439);
            this.gameStateControl.TabIndex = 0;
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tableLayoutPanel1.Controls.Add(this.unitSummaryTree1, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.cbEnableEconomyAI, 0, 2);
            this.tableLayoutPanel1.Controls.Add(flowLayoutPanel1, 0, 1);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 55F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(238, 439);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // btnIssueCommand
            // 
            this.btnIssueCommand.Location = new System.Drawing.Point(3, 3);
            this.btnIssueCommand.Name = "btnIssueCommand";
            this.btnIssueCommand.Size = new System.Drawing.Size(75, 23);
            this.btnIssueCommand.TabIndex = 0;
            this.btnIssueCommand.Text = "Send Cmd";
            this.btnIssueCommand.UseVisualStyleBackColor = true;
            this.btnIssueCommand.Click += new System.EventHandler(this.btnIssueCommand_Click);
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(this.btnIssueCommand);
            flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            flowLayoutPanel1.Location = new System.Drawing.Point(3, 362);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new System.Drawing.Size(232, 49);
            flowLayoutPanel1.TabIndex = 0;
            // 
            // cbEnableEconomyAI
            // 
            this.cbEnableEconomyAI.AutoSize = true;
            this.cbEnableEconomyAI.Location = new System.Drawing.Point(3, 417);
            this.cbEnableEconomyAI.Name = "cbEnableEconomyAI";
            this.cbEnableEconomyAI.Size = new System.Drawing.Size(121, 17);
            this.cbEnableEconomyAI.TabIndex = 1;
            this.cbEnableEconomyAI.Text = "Economy AI Module";
            this.cbEnableEconomyAI.UseVisualStyleBackColor = true;
            this.cbEnableEconomyAI.CheckedChanged += new System.EventHandler(this.cbEnableEconomyAI_CheckedChanged);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(714, 461);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.ssBotStatus);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "MainForm";
            this.Text = "StarCraft Bot 9K";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ssBotStatus.ResumeLayout(false);
            this.ssBotStatus.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.StatusStrip ssBotStatus;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private GameState gameStateControl;
        private UnitSummaryTree unitSummaryTree1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.CheckBox cbEnableEconomyAI;
        private System.Windows.Forms.Button btnIssueCommand;
    }
}

