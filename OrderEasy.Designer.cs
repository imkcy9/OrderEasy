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
            this.labelCurrentInstrument = new System.Windows.Forms.Label();
            this.labelOrderHang = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.labelServerState = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtOrderQty = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.grid1 = new SourceGrid.Grid();
            this.panel2 = new System.Windows.Forms.Panel();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.menuStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrderQty)).BeginInit();
            this.panel2.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(639, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // setToolStripMenuItem
            // 
            this.setToolStripMenuItem.Name = "setToolStripMenuItem";
            this.setToolStripMenuItem.Size = new System.Drawing.Size(37, 21);
            this.setToolStripMenuItem.Text = "set";
            this.setToolStripMenuItem.Visible = false;
            this.setToolStripMenuItem.Click += new System.EventHandler(this.setToolStripMenuItem_Click);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.labelCurrentInstrument);
            this.panel1.Controls.Add(this.labelOrderHang);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.labelServerState);
            this.panel1.Controls.Add(this.label3);
            this.panel1.Controls.Add(this.txtOrderQty);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Location = new System.Drawing.Point(3, 493);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(369, 52);
            this.panel1.TabIndex = 8;
            // 
            // labelCurrentInstrument
            // 
            this.labelCurrentInstrument.AutoSize = true;
            this.labelCurrentInstrument.Location = new System.Drawing.Point(10, 29);
            this.labelCurrentInstrument.Name = "labelCurrentInstrument";
            this.labelCurrentInstrument.Size = new System.Drawing.Size(83, 12);
            this.labelCurrentInstrument.TabIndex = 8;
            this.labelCurrentInstrument.Text = "当 前 合 约：";
            // 
            // labelOrderHang
            // 
            this.labelOrderHang.AutoSize = true;
            this.labelOrderHang.Location = new System.Drawing.Point(105, 7);
            this.labelOrderHang.Name = "labelOrderHang";
            this.labelOrderHang.Size = new System.Drawing.Size(11, 12);
            this.labelOrderHang.TabIndex = 7;
            this.labelOrderHang.Text = "0";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "当前挂单总数：";
            // 
            // labelServerState
            // 
            this.labelServerState.AutoSize = true;
            this.labelServerState.Location = new System.Drawing.Point(294, 29);
            this.labelServerState.Name = "labelServerState";
            this.labelServerState.Size = new System.Drawing.Size(41, 12);
            this.labelServerState.TabIndex = 5;
            this.labelServerState.Text = "未登陆";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(225, 29);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(77, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "服务器状态：";
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
            // grid1
            // 
            this.grid1.AllowOverlappingCells = true;
            this.grid1.AutoStretchColumnsToFitWidth = true;
            this.grid1.EnableSort = false;
            this.grid1.Location = new System.Drawing.Point(0, 0);
            this.grid1.Name = "grid1";
            this.grid1.OptimizeMode = SourceGrid.CellOptimizeMode.ForRows;
            this.grid1.SelectionMode = SourceGrid.GridSelectionMode.Cell;
            this.grid1.Size = new System.Drawing.Size(372, 487);
            this.grid1.TabIndex = 0;
            this.grid1.TabStop = true;
            this.grid1.ToolTipText = "";
            this.grid1.MouseHover += new System.EventHandler(this.grid1_MouseHover);
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.panel1);
            this.panel2.Controls.Add(this.grid1);
            this.panel2.Location = new System.Drawing.Point(-1, 24);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(365, 555);
            this.panel2.TabIndex = 9;
            // 
            // richTextBox1
            // 
            this.richTextBox1.BackColor = System.Drawing.Color.AliceBlue;
            this.richTextBox1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richTextBox1.Location = new System.Drawing.Point(0, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ReadOnly = true;
            this.richTextBox1.Size = new System.Drawing.Size(255, 484);
            this.richTextBox1.TabIndex = 0;
            this.richTextBox1.Text = "";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.richTextBox1);
            this.groupBox2.Location = new System.Drawing.Point(380, 24);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(255, 487);
            this.groupBox2.TabIndex = 10;
            this.groupBox2.TabStop = false;
            this.groupBox2.Visible = false;
            // 
            // OrderEasy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BackColor = System.Drawing.Color.LightSlateGray;
            this.ClientSize = new System.Drawing.Size(639, 582);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "OrderEasy";
            this.Text = "快速下单";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OrderEasy_FormClosing);
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OrderEasy_FormClosed);
            this.Load += new System.EventHandler(this.Sim101_Load);
            this.KeyUp += new System.Windows.Forms.KeyEventHandler(this.OrderEasy_KeyUp);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.txtOrderQty)).EndInit();
            this.panel2.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem setToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown txtOrderQty;


        private SourceGrid.Grid grid1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label labelServerState;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelOrderHang;
        private System.Windows.Forms.Label labelCurrentInstrument;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.GroupBox groupBox2;
    }
}

