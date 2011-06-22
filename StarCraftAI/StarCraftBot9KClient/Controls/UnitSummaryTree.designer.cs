namespace StarCraftBot9KClient.Controls
{
    partial class UnitSummaryTree
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
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Player");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Minerals");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Critters");
            this.tvRootTree = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // tvRootTree
            // 
            this.tvRootTree.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvRootTree.Location = new System.Drawing.Point(0, 0);
            this.tvRootTree.Name = "tvRootTree";
            treeNode1.Name = "tnPlayer0";
            treeNode1.Text = "Player";
            treeNode2.Name = "tnMinerals";
            treeNode2.Text = "Minerals";
            treeNode3.Name = "Critters";
            treeNode3.Text = "Critters";
            this.tvRootTree.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3});
            this.tvRootTree.Size = new System.Drawing.Size(201, 285);
            this.tvRootTree.TabIndex = 0;
            // 
            // UnitSummaryTree
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tvRootTree);
            this.Name = "UnitSummaryTree";
            this.Size = new System.Drawing.Size(201, 285);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView tvRootTree;
    }
}
