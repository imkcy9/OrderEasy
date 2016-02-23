namespace OrderEasy
{
    partial class SetForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SetForm));
            this.grid_addr = new SourceGrid.Grid();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tab_addr_m = new System.Windows.Forms.TabPage();
            this.tab_account_m = new System.Windows.Forms.TabPage();
            this.grid_account = new SourceGrid.Grid();
            this.tab_category_m = new System.Windows.Forms.TabPage();
            this.grid_category = new SourceGrid.Grid();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnSub = new System.Windows.Forms.Button();
            this.btnAdd = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tab_addr_m.SuspendLayout();
            this.tab_account_m.SuspendLayout();
            this.tab_category_m.SuspendLayout();
            this.panel3.SuspendLayout();
            this.SuspendLayout();
            // 
            // grid_addr
            // 
            this.grid_addr.AllowOverlappingCells = true;
            this.grid_addr.AutoStretchColumnsToFitWidth = true;
            this.grid_addr.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_addr.EnableSort = false;
            this.grid_addr.Location = new System.Drawing.Point(3, 3);
            this.grid_addr.Name = "grid_addr";
            this.grid_addr.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid_addr.SelectionMode = SourceGrid.GridSelectionMode.Row;
            this.grid_addr.Size = new System.Drawing.Size(325, 253);
            this.grid_addr.TabIndex = 1;
            this.grid_addr.TabStop = true;
            this.grid_addr.ToolTipText = "";
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tab_addr_m);
            this.tabControl1.Controls.Add(this.tab_account_m);
            this.tabControl1.Controls.Add(this.tab_category_m);
            this.tabControl1.Location = new System.Drawing.Point(2, 1);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(339, 284);
            this.tabControl1.TabIndex = 2;
            // 
            // tab_addr_m
            // 
            this.tab_addr_m.Controls.Add(this.grid_addr);
            this.tab_addr_m.Location = new System.Drawing.Point(4, 21);
            this.tab_addr_m.Name = "tab_addr_m";
            this.tab_addr_m.Padding = new System.Windows.Forms.Padding(3);
            this.tab_addr_m.Size = new System.Drawing.Size(331, 259);
            this.tab_addr_m.TabIndex = 0;
            this.tab_addr_m.Text = "连接地址管理";
            this.tab_addr_m.UseVisualStyleBackColor = true;
            // 
            // tab_account_m
            // 
            this.tab_account_m.Controls.Add(this.grid_account);
            this.tab_account_m.Location = new System.Drawing.Point(4, 21);
            this.tab_account_m.Name = "tab_account_m";
            this.tab_account_m.Padding = new System.Windows.Forms.Padding(3);
            this.tab_account_m.Size = new System.Drawing.Size(331, 259);
            this.tab_account_m.TabIndex = 1;
            this.tab_account_m.Text = "期货品种管理";
            this.tab_account_m.UseVisualStyleBackColor = true;
            // 
            // grid_account
            // 
            this.grid_account.AllowOverlappingCells = true;
            this.grid_account.AutoStretchColumnsToFitWidth = true;
            this.grid_account.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_account.EnableSort = false;
            this.grid_account.Location = new System.Drawing.Point(3, 3);
            this.grid_account.Name = "grid_account";
            this.grid_account.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid_account.SelectionMode = SourceGrid.GridSelectionMode.Row;
            this.grid_account.Size = new System.Drawing.Size(325, 253);
            this.grid_account.TabIndex = 2;
            this.grid_account.TabStop = true;
            this.grid_account.ToolTipText = "";
            // 
            // tab_category_m
            // 
            this.tab_category_m.Controls.Add(this.grid_category);
            this.tab_category_m.Location = new System.Drawing.Point(4, 21);
            this.tab_category_m.Name = "tab_category_m";
            this.tab_category_m.Padding = new System.Windows.Forms.Padding(3);
            this.tab_category_m.Size = new System.Drawing.Size(331, 259);
            this.tab_category_m.TabIndex = 2;
            this.tab_category_m.Text = "股票品种管理";
            this.tab_category_m.UseVisualStyleBackColor = true;
            // 
            // grid_category
            // 
            this.grid_category.AllowOverlappingCells = true;
            this.grid_category.AutoStretchColumnsToFitWidth = true;
            this.grid_category.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grid_category.EnableSort = false;
            this.grid_category.Location = new System.Drawing.Point(3, 3);
            this.grid_category.Name = "grid_category";
            this.grid_category.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid_category.SelectionMode = SourceGrid.GridSelectionMode.Row;
            this.grid_category.Size = new System.Drawing.Size(325, 253);
            this.grid_category.TabIndex = 3;
            this.grid_category.TabStop = true;
            this.grid_category.ToolTipText = "";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnDown);
            this.panel3.Controls.Add(this.btnUp);
            this.panel3.Controls.Add(this.btnOk);
            this.panel3.Controls.Add(this.btnSub);
            this.panel3.Controls.Add(this.btnAdd);
            this.panel3.Location = new System.Drawing.Point(2, 284);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(339, 28);
            this.panel3.TabIndex = 211;
            // 
            // btnDown
            // 
            this.btnDown.Location = new System.Drawing.Point(116, 3);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(33, 23);
            this.btnDown.TabIndex = 210;
            this.btnDown.Text = "↓";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Location = new System.Drawing.Point(76, 3);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(41, 23);
            this.btnUp.TabIndex = 209;
            this.btnUp.Text = "↑";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(150, 3);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(38, 23);
            this.btnOk.TabIndex = 208;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnSub
            // 
            this.btnSub.Location = new System.Drawing.Point(43, 3);
            this.btnSub.Name = "btnSub";
            this.btnSub.Size = new System.Drawing.Size(33, 23);
            this.btnSub.TabIndex = 207;
            this.btnSub.Text = "-";
            this.btnSub.UseVisualStyleBackColor = true;
            this.btnSub.Click += new System.EventHandler(this.btnSub_Click);
            // 
            // btnAdd
            // 
            this.btnAdd.Location = new System.Drawing.Point(3, 3);
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(41, 23);
            this.btnAdd.TabIndex = 206;
            this.btnAdd.Text = "+";
            this.btnAdd.UseVisualStyleBackColor = true;
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // SetForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(339, 312);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.tabControl1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "SetForm";
            this.Text = "设置";
            this.Load += new System.EventHandler(this.SetForm_Load);
            this.tabControl1.ResumeLayout(false);
            this.tab_addr_m.ResumeLayout(false);
            this.tab_account_m.ResumeLayout(false);
            this.tab_category_m.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private SourceGrid.Grid grid_addr;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tab_addr_m;
        private System.Windows.Forms.TabPage tab_account_m;
        private SourceGrid.Grid grid_account;
        private System.Windows.Forms.TabPage tab_category_m;
        private SourceGrid.Grid grid_category;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnSub;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
    }
}

