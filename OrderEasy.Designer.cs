namespace OrderEasy
{
    partial class OrderEasy
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(OrderEasy));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.setToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.comb_product = new System.Windows.Forms.ComboBox();
            this.comboBox_strat = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comb_account = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOrderQty = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.comb_Instrument = new System.Windows.Forms.ComboBox();
            this.grid1 = new SourceGrid.Grid();
            this.panel2 = new System.Windows.Forms.Panel();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.numProfitStop3 = new System.Windows.Forms.NumericUpDown();
            this.numStopLoss3 = new System.Windows.Forms.NumericUpDown();
            this.numericQty3 = new System.Windows.Forms.NumericUpDown();
            this.numProfitStop2 = new System.Windows.Forms.NumericUpDown();
            this.numStopLoss2 = new System.Windows.Forms.NumericUpDown();
            this.numericQty2 = new System.Windows.Forms.NumericUpDown();
            this.numProfitStop1 = new System.Windows.Forms.NumericUpDown();
            this.numStopLoss1 = new System.Windows.Forms.NumericUpDown();
            this.numericQty1 = new System.Windows.Forms.NumericUpDown();
            this.radBtn3 = new System.Windows.Forms.RadioButton();
            this.radBtn2 = new System.Windows.Forms.RadioButton();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.radBtn1 = new System.Windows.Forms.RadioButton();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrderQty)).BeginInit();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numProfitStop3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStopLoss3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericQty3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProfitStop2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStopLoss2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericQty2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProfitStop1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStopLoss1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericQty1)).BeginInit();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(373, 25);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // setToolStripMenuItem
            // 
            this.setToolStripMenuItem.Name = "setToolStripMenuItem";
            this.setToolStripMenuItem.Size = new System.Drawing.Size(37, 21);
            this.setToolStripMenuItem.Text = "set";
            this.setToolStripMenuItem.Click += new System.EventHandler(this.setToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.comb_product);
            this.panel1.Controls.Add(this.comboBox_strat);
            this.panel1.Controls.Add(this.label4);
            this.panel1.Controls.Add(this.comb_account);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtOrderQty);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.comb_Instrument);
            this.panel1.Location = new System.Drawing.Point(3, 493);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(369, 71);
            this.panel1.TabIndex = 8;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(0, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "品种：";
            this.label1.Visible = false;
            // 
            // comb_product
            // 
            this.comb_product.FormattingEnabled = true;
            this.comb_product.Location = new System.Drawing.Point(42, 1);
            this.comb_product.Name = "comb_product";
            this.comb_product.Size = new System.Drawing.Size(53, 20);
            this.comb_product.TabIndex = 9;
            this.comb_product.Visible = false;
            this.comb_product.SelectedIndexChanged += new System.EventHandler(this.comb_product_SelectedIndexChanged);
            // 
            // comboBox_strat
            // 
            this.comboBox_strat.FormattingEnabled = true;
            this.comboBox_strat.Items.AddRange(new object[] {
            "<none>",
            "<Custom>"});
            this.comboBox_strat.Location = new System.Drawing.Point(95, 48);
            this.comboBox_strat.Name = "comboBox_strat";
            this.comboBox_strat.Size = new System.Drawing.Size(266, 20);
            this.comboBox_strat.TabIndex = 8;
            this.comboBox_strat.Visible = false;
            this.comboBox_strat.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(0, 52);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(89, 12);
            this.label4.TabIndex = 7;
            this.label4.Text = "自动平仓策略：";
            this.label4.Visible = false;
            // 
            // comb_account
            // 
            this.comb_account.FormattingEnabled = true;
            this.comb_account.Location = new System.Drawing.Point(42, 25);
            this.comb_account.Name = "comb_account";
            this.comb_account.Size = new System.Drawing.Size(149, 20);
            this.comb_account.TabIndex = 5;
            this.comb_account.Visible = false;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(0, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "帐号：";
            this.label3.Visible = false;
            // 
            // txtOrderQty
            // 
            this.txtOrderQty.Location = new System.Drawing.Point(279, 2);
            this.txtOrderQty.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.txtOrderQty.Name = "txtOrderQty";
            this.txtOrderQty.Size = new System.Drawing.Size(83, 21);
            this.txtOrderQty.TabIndex = 3;
            this.txtOrderQty.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.txtOrderQty.ValueChanged += new System.EventHandler(this.txtOrderQty_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(225, 7);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "手数：";
            // 
            // comb_Instrument
            // 
            this.comb_Instrument.FormattingEnabled = true;
            this.comb_Instrument.Location = new System.Drawing.Point(95, 1);
            this.comb_Instrument.Name = "comb_Instrument";
            this.comb_Instrument.Size = new System.Drawing.Size(96, 20);
            this.comb_Instrument.TabIndex = 1;
            this.comb_Instrument.Visible = false;
            this.comb_Instrument.SelectedIndexChanged += new System.EventHandler(this.comb_Instrument_SelectedIndexChanged);
            // 
            // grid1
            // 
            this.grid1.AllowOverlappingCells = true;
            this.grid1.AutoStretchColumnsToFitWidth = true;
            this.grid1.EnableSort = false;
            this.grid1.Location = new System.Drawing.Point(0, 0);
            this.grid1.Name = "grid1";
            this.grid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid1.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grid1.Size = new System.Drawing.Size(375, 487);
            this.grid1.TabIndex = 0;
            this.grid1.TabStop = true;
            this.grid1.ToolTipText = "";
            this.grid1.MouseHover += new System.EventHandler(this.grid1_MouseHover);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.groupBox2);
            this.panel2.Controls.Add(this.grid1);
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Location = new System.Drawing.Point(-1, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(375, 676);
            this.panel2.TabIndex = 9;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.numProfitStop3);
            this.groupBox2.Controls.Add(this.numStopLoss3);
            this.groupBox2.Controls.Add(this.numericQty3);
            this.groupBox2.Controls.Add(this.numProfitStop2);
            this.groupBox2.Controls.Add(this.numStopLoss2);
            this.groupBox2.Controls.Add(this.numericQty2);
            this.groupBox2.Controls.Add(this.numProfitStop1);
            this.groupBox2.Controls.Add(this.numStopLoss1);
            this.groupBox2.Controls.Add(this.numericQty1);
            this.groupBox2.Controls.Add(this.radBtn3);
            this.groupBox2.Controls.Add(this.radBtn2);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.radBtn1);
            this.groupBox2.Enabled = false;
            this.groupBox2.Location = new System.Drawing.Point(3, 561);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(369, 129);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "策略 参数（ticks）";
            this.groupBox2.Visible = false;
            // 
            // numProfitStop3
            // 
            this.numProfitStop3.Location = new System.Drawing.Point(277, 93);
            this.numProfitStop3.Name = "numProfitStop3";
            this.numProfitStop3.Size = new System.Drawing.Size(83, 21);
            this.numProfitStop3.TabIndex = 14;
            // 
            // numStopLoss3
            // 
            this.numStopLoss3.Location = new System.Drawing.Point(277, 66);
            this.numStopLoss3.Name = "numStopLoss3";
            this.numStopLoss3.Size = new System.Drawing.Size(83, 21);
            this.numStopLoss3.TabIndex = 13;
            // 
            // numericQty3
            // 
            this.numericQty3.Location = new System.Drawing.Point(277, 41);
            this.numericQty3.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericQty3.Name = "numericQty3";
            this.numericQty3.Size = new System.Drawing.Size(83, 21);
            this.numericQty3.TabIndex = 12;
            // 
            // numProfitStop2
            // 
            this.numProfitStop2.Location = new System.Drawing.Point(177, 93);
            this.numProfitStop2.Name = "numProfitStop2";
            this.numProfitStop2.Size = new System.Drawing.Size(83, 21);
            this.numProfitStop2.TabIndex = 11;
            // 
            // numStopLoss2
            // 
            this.numStopLoss2.Location = new System.Drawing.Point(177, 66);
            this.numStopLoss2.Name = "numStopLoss2";
            this.numStopLoss2.Size = new System.Drawing.Size(83, 21);
            this.numStopLoss2.TabIndex = 10;
            // 
            // numericQty2
            // 
            this.numericQty2.Location = new System.Drawing.Point(177, 41);
            this.numericQty2.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericQty2.Name = "numericQty2";
            this.numericQty2.Size = new System.Drawing.Size(83, 21);
            this.numericQty2.TabIndex = 9;
            // 
            // numProfitStop1
            // 
            this.numProfitStop1.Location = new System.Drawing.Point(76, 95);
            this.numProfitStop1.Name = "numProfitStop1";
            this.numProfitStop1.Size = new System.Drawing.Size(81, 21);
            this.numProfitStop1.TabIndex = 8;
            // 
            // numStopLoss1
            // 
            this.numStopLoss1.Location = new System.Drawing.Point(76, 68);
            this.numStopLoss1.Name = "numStopLoss1";
            this.numStopLoss1.Size = new System.Drawing.Size(81, 21);
            this.numStopLoss1.TabIndex = 7;
            // 
            // numericQty1
            // 
            this.numericQty1.Location = new System.Drawing.Point(76, 43);
            this.numericQty1.Maximum = new decimal(new int[] {
            100000,
            0,
            0,
            0});
            this.numericQty1.Name = "numericQty1";
            this.numericQty1.Size = new System.Drawing.Size(81, 21);
            this.numericQty1.TabIndex = 6;
            this.numericQty1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // radBtn3
            // 
            this.radBtn3.AutoSize = true;
            this.radBtn3.Location = new System.Drawing.Point(277, 20);
            this.radBtn3.Name = "radBtn3";
            this.radBtn3.Size = new System.Drawing.Size(59, 16);
            this.radBtn3.TabIndex = 5;
            this.radBtn3.TabStop = true;
            this.radBtn3.Text = "目标 3";
            this.radBtn3.UseVisualStyleBackColor = true;
            this.radBtn3.CheckedChanged += new System.EventHandler(this.radBtn3_CheckedChanged);
            // 
            // radBtn2
            // 
            this.radBtn2.AutoSize = true;
            this.radBtn2.Location = new System.Drawing.Point(177, 20);
            this.radBtn2.Name = "radBtn2";
            this.radBtn2.Size = new System.Drawing.Size(59, 16);
            this.radBtn2.TabIndex = 4;
            this.radBtn2.TabStop = true;
            this.radBtn2.Text = "目标 2";
            this.radBtn2.UseVisualStyleBackColor = true;
            this.radBtn2.CheckedChanged += new System.EventHandler(this.radBtn2_CheckedChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 98);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 3;
            this.label7.Text = "多仓：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 70);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(41, 12);
            this.label6.TabIndex = 2;
            this.label6.Text = "空仓：";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(41, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "手数：";
            // 
            // radBtn1
            // 
            this.radBtn1.AutoSize = true;
            this.radBtn1.Location = new System.Drawing.Point(76, 20);
            this.radBtn1.Name = "radBtn1";
            this.radBtn1.Size = new System.Drawing.Size(59, 16);
            this.radBtn1.TabIndex = 0;
            this.radBtn1.TabStop = true;
            this.radBtn1.Text = "目标 1";
            this.radBtn1.UseVisualStyleBackColor = true;
            this.radBtn1.CheckedChanged += new System.EventHandler(this.radBtn1_CheckedChanged);
            // 
            // OrderEasy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSlateGray;
            this.ClientSize = new System.Drawing.Size(373, 729);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "OrderEasy";
            this.Text = "快速下单";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OrderEasy_FormClosed);
            this.Load += new System.EventHandler(this.Sim101_Load);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrderQty)).EndInit();
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numProfitStop3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStopLoss3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericQty3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProfitStop2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStopLoss2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericQty2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numProfitStop1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numStopLoss1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericQty1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem setToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comb_Instrument;
        private System.Windows.Forms.ComboBox comb_account;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown txtOrderQty;


        private SourceGrid.Grid grid1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox_strat;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.RadioButton radBtn1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton radBtn3;
        private System.Windows.Forms.RadioButton radBtn2;
        private System.Windows.Forms.NumericUpDown numericQty1;
        private System.Windows.Forms.NumericUpDown numProfitStop3;
        private System.Windows.Forms.NumericUpDown numStopLoss3;
        private System.Windows.Forms.NumericUpDown numericQty3;
        private System.Windows.Forms.NumericUpDown numProfitStop2;
        private System.Windows.Forms.NumericUpDown numStopLoss2;
        private System.Windows.Forms.NumericUpDown numericQty2;
        private System.Windows.Forms.NumericUpDown numProfitStop1;
        private System.Windows.Forms.NumericUpDown numStopLoss1;
        private System.Windows.Forms.ComboBox comb_product;
        private System.Windows.Forms.Label label1;
    }
}

