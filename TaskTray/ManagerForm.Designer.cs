namespace TaskTray
{
    partial class ManagerForm
    {
        private System.ComponentModel.IContainer components = null;
        
        // Layout Panels
        private System.Windows.Forms.Panel pnlToolbar;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.Panel pnlActionBar;
        
        // Toolbar controls
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnAddProgram;
        private System.Windows.Forms.Button btnAddCategory;
        private System.Windows.Forms.Button btnSettings;
        
        // Main view controls (Left/Right)
        private System.Windows.Forms.ListBox lstCategories;
        private System.Windows.Forms.FlowLayoutPanel flowApps;
        private System.Windows.Forms.Panel pnlEmptyState;
        
        // Bottom Action Bar controls
        private System.Windows.Forms.Button btnRenameCategory;
        private System.Windows.Forms.Button btnDeleteCategory;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnExport;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            
            this.pnlToolbar = new System.Windows.Forms.Panel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnAddProgram = new System.Windows.Forms.Button();
            this.btnAddCategory = new System.Windows.Forms.Button();
            this.btnSettings = new System.Windows.Forms.Button();
            
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.lstCategories = new System.Windows.Forms.ListBox();
            this.flowApps = new System.Windows.Forms.FlowLayoutPanel();
            this.pnlEmptyState = new System.Windows.Forms.Panel();
            
            this.pnlActionBar = new System.Windows.Forms.Panel();
            this.btnRenameCategory = new System.Windows.Forms.Button();
            this.btnDeleteCategory = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnExport = new System.Windows.Forms.Button();

            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.SuspendLayout();

            // pnlToolbar
            this.pnlToolbar.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlToolbar.Height = 60;
            this.pnlToolbar.Padding = new System.Windows.Forms.Padding(15);
            this.pnlToolbar.Controls.Add(this.txtSearch);
            this.pnlToolbar.Controls.Add(this.btnAddProgram);
            this.pnlToolbar.Controls.Add(this.btnAddCategory);
            this.pnlToolbar.Controls.Add(this.btnSettings);

            // txtSearch
            this.txtSearch.Location = new System.Drawing.Point(15, 17);
            this.txtSearch.Size = new System.Drawing.Size(220, 26);
            this.txtSearch.PlaceholderText = "Search apps...";

            // btnAddProgram
            this.btnAddProgram.Location = new System.Drawing.Point(245, 15);
            this.btnAddProgram.Size = new System.Drawing.Size(130, 30);
            this.btnAddProgram.Text = "+ Add Program";

            // btnAddCategory
            this.btnAddCategory.Location = new System.Drawing.Point(385, 15);
            this.btnAddCategory.Size = new System.Drawing.Size(40, 30);
            this.btnAddCategory.Text = "+";

            // btnSettings
            this.btnSettings.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnSettings.Location = new System.Drawing.Point(745, 15);
            this.btnSettings.Size = new System.Drawing.Size(40, 30);
            this.btnSettings.Text = "⚙";

            // splitMain
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 60);
            this.splitMain.SplitterDistance = 220;
            this.splitMain.BackColor = System.Drawing.Color.Transparent;

            // lstCategories
            this.lstCategories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstCategories.ItemHeight = 40;
            this.lstCategories.DrawMode = System.Windows.Forms.DrawMode.OwnerDrawFixed;
            this.lstCategories.BackColor = System.Drawing.Color.FromArgb(18, 18, 20);

            // flowApps
            this.flowApps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowApps.AutoScroll = true;
            this.flowApps.Padding = new System.Windows.Forms.Padding(15);
            this.flowApps.BackColor = System.Drawing.Color.FromArgb(10, 10, 11);

            // pnlEmptyState (Hidden by default, shown by logic)
            this.pnlEmptyState.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlEmptyState.Visible = false;

            // pnlActionBar
            this.pnlActionBar.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlActionBar.Height = 50;
            this.pnlActionBar.Padding = new System.Windows.Forms.Padding(15, 10, 15, 10);
            this.pnlActionBar.Controls.Add(this.btnRenameCategory);
            this.pnlActionBar.Controls.Add(this.btnDeleteCategory);
            this.pnlActionBar.Controls.Add(this.btnImport);
            this.pnlActionBar.Controls.Add(this.btnExport);

            // btnRenameCategory
            this.btnRenameCategory.Location = new System.Drawing.Point(15, 10);
            this.btnRenameCategory.Size = new System.Drawing.Size(80, 30);
            this.btnRenameCategory.Text = "Rename";

            // btnDeleteCategory
            this.btnDeleteCategory.Location = new System.Drawing.Point(100, 10);
            this.btnDeleteCategory.Size = new System.Drawing.Size(80, 30);
            this.btnDeleteCategory.Text = "Remove";

            // btnExport
            this.btnExport.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnExport.Location = new System.Drawing.Point(705, 10);
            this.btnExport.Size = new System.Drawing.Size(80, 30);
            this.btnExport.Text = "Export";

            // btnImport
            this.btnImport.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.btnImport.Location = new System.Drawing.Point(620, 10);
            this.btnImport.Size = new System.Drawing.Size(80, 30);
            this.btnImport.Text = "Import";

            // Assembly split container
            this.splitMain.Panel1.Controls.Add(this.lstCategories);
            this.splitMain.Panel2.Controls.Add(this.flowApps);
            this.splitMain.Panel2.Controls.Add(this.pnlEmptyState);

            // ManagerForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 520);
            this.Controls.Add(this.splitMain);
            this.Controls.Add(this.pnlToolbar);
            this.Controls.Add(this.pnlActionBar);
            this.MinimumSize = new System.Drawing.Size(650, 450);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
