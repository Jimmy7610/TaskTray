namespace TaskTray
{
    partial class ManagerForm
    {
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.TabControl tabMain;
        private System.Windows.Forms.TabPage tabLauncher;
        private System.Windows.Forms.TabPage tabSettings;
        
        // Launcher controls
        private System.Windows.Forms.ListBox lstCategories;
        private System.Windows.Forms.FlowLayoutPanel flowApps;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnAddProgram;
        private System.Windows.Forms.Button btnAddCategory;
        private System.Windows.Forms.Button btnRenameCategory;
        private System.Windows.Forms.Button btnDeleteCategory;
        private System.Windows.Forms.SplitContainer splitMain;
        private System.Windows.Forms.Panel pnlHeader;
        private System.Windows.Forms.Panel pnlCategoryActions;

        // Settings controls
        private System.Windows.Forms.CheckBox chkAutoStart;
        private System.Windows.Forms.ComboBox cmbLanguage;
        private System.Windows.Forms.Label lblLanguage;
        private System.Windows.Forms.Label lblSettingsHeader;

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
            this.tabMain = new System.Windows.Forms.TabControl();
            this.tabLauncher = new System.Windows.Forms.TabPage();
            this.tabSettings = new System.Windows.Forms.TabPage();
            
            this.lstCategories = new System.Windows.Forms.ListBox();
            this.flowApps = new System.Windows.Forms.FlowLayoutPanel();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnAddProgram = new System.Windows.Forms.Button();
            this.btnAddCategory = new System.Windows.Forms.Button();
            this.btnRenameCategory = new System.Windows.Forms.Button();
            this.btnDeleteCategory = new System.Windows.Forms.Button();
            this.splitMain = new System.Windows.Forms.SplitContainer();
            this.pnlHeader = new System.Windows.Forms.Panel();
            this.pnlCategoryActions = new System.Windows.Forms.Panel();

            this.chkAutoStart = new System.Windows.Forms.CheckBox();
            this.cmbLanguage = new System.Windows.Forms.ComboBox();
            this.lblLanguage = new System.Windows.Forms.Label();
            this.lblSettingsHeader = new System.Windows.Forms.Label();

            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).BeginInit();
            this.splitMain.Panel1.SuspendLayout();
            this.splitMain.Panel2.SuspendLayout();
            this.splitMain.SuspendLayout();
            this.pnlHeader.SuspendLayout();
            this.pnlCategoryActions.SuspendLayout();
            this.tabMain.SuspendLayout();
            this.tabLauncher.SuspendLayout();
            this.tabSettings.SuspendLayout();
            this.SuspendLayout();

            // tabMain
            this.tabMain.Controls.Add(this.tabLauncher);
            this.tabMain.Controls.Add(this.tabSettings);
            this.tabMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabMain.Location = new System.Drawing.Point(0, 0);

            // tabLauncher
            this.tabLauncher.Controls.Add(this.splitMain);
            this.tabLauncher.Controls.Add(this.pnlHeader);
            this.tabLauncher.Text = "Launcher";

            // pnlHeader
            this.pnlHeader.Controls.Add(this.txtSearch);
            this.pnlHeader.Controls.Add(this.btnAddProgram);
            this.pnlHeader.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlHeader.Height = 50;
            this.pnlHeader.Padding = new System.Windows.Forms.Padding(10);

            // txtSearch
            this.txtSearch.Location = new System.Drawing.Point(10, 12);
            this.txtSearch.Size = new System.Drawing.Size(200, 26);

            // btnAddProgram
            this.btnAddProgram.Location = new System.Drawing.Point(220, 10);
            this.btnAddProgram.Size = new System.Drawing.Size(120, 30);

            // splitMain
            this.splitMain.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitMain.Location = new System.Drawing.Point(0, 50);
            this.splitMain.SplitterDistance = 200;

            // lstCategories
            this.lstCategories.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lstCategories.ItemHeight = 25;

            // pnlCategoryActions
            this.pnlCategoryActions.Controls.Add(this.btnAddCategory);
            this.pnlCategoryActions.Controls.Add(this.btnRenameCategory);
            this.pnlCategoryActions.Controls.Add(this.btnDeleteCategory);
            this.pnlCategoryActions.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlCategoryActions.Height = 35;

            this.btnAddCategory.Size = new System.Drawing.Size(30, 30);
            this.btnAddCategory.Text = "+";
            this.btnAddCategory.Location = new Point(5, 2);

            this.btnRenameCategory.Size = new System.Drawing.Size(70, 30);
            this.btnRenameCategory.Text = "Rename";
            this.btnRenameCategory.Location = new Point(40, 2);

            this.btnDeleteCategory.Size = new System.Drawing.Size(30, 30);
            this.btnDeleteCategory.Text = "-";
            this.btnDeleteCategory.Location = new Point(115, 2);

            // flowApps
            this.flowApps.Dock = System.Windows.Forms.DockStyle.Fill;
            this.flowApps.AutoScroll = true;
            this.flowApps.Padding = new System.Windows.Forms.Padding(10);

            // tabSettings
            this.tabSettings.Controls.Add(this.lblSettingsHeader);
            this.tabSettings.Controls.Add(this.chkAutoStart);
            this.tabSettings.Controls.Add(this.lblLanguage);
            this.tabSettings.Controls.Add(this.cmbLanguage);
            this.tabSettings.Padding = new System.Windows.Forms.Padding(20);
            this.tabSettings.Text = "Settings";

            this.lblSettingsHeader.Text = "Settings";
            this.lblSettingsHeader.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            this.lblSettingsHeader.Location = new Point(20, 20);
            this.lblSettingsHeader.Size = new Size(200, 30);

            this.chkAutoStart.Location = new Point(25, 70);
            this.chkAutoStart.Size = new Size(300, 30);
            this.chkAutoStart.Text = "Run at Windows startup";

            this.lblLanguage.Location = new Point(25, 110);
            this.lblLanguage.Size = new Size(100, 30);
            this.lblLanguage.Text = "Language:";
            this.lblLanguage.TextAlign = ContentAlignment.MiddleLeft;

            this.cmbLanguage.Location = new Point(130, 115);
            this.cmbLanguage.Size = new Size(150, 30);
            this.cmbLanguage.DropDownStyle = ComboBoxStyle.DropDownList;

            // Add panels to split container
            this.splitMain.Panel1.Controls.Add(this.lstCategories);
            this.splitMain.Panel1.Controls.Add(this.pnlCategoryActions);
            this.splitMain.Panel2.Controls.Add(this.flowApps);

            // ManagerForm
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 500);
            this.Controls.Add(this.tabMain);
            this.MinimumSize = new System.Drawing.Size(600, 400);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;

            ((System.ComponentModel.ISupportInitialize)(this.splitMain)).EndInit();
            this.splitMain.Panel1.ResumeLayout(false);
            this.splitMain.Panel2.ResumeLayout(false);
            this.splitMain.ResumeLayout(false);
            this.pnlHeader.ResumeLayout(false);
            this.pnlHeader.PerformLayout();
            this.pnlCategoryActions.ResumeLayout(false);
            this.tabMain.ResumeLayout(false);
            this.tabLauncher.ResumeLayout(false);
            this.tabSettings.ResumeLayout(false);
            this.ResumeLayout(false);
        }
    }
}
