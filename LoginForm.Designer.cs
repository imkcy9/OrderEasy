namespace OrderEasy
{
    partial class LoginForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.comb_Instrument = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.comb_account = new System.Windows.Forms.ComboBox();
            this.comb_product = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button_login = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // comb_Instrument
            // 
            this.comb_Instrument.FormattingEnabled = true;
            this.comb_Instrument.Location = new System.Drawing.Point(131, 82);
            this.comb_Instrument.Name = "comb_Instrument";
            this.comb_Instrument.Size = new System.Drawing.Size(96, 20);
            this.comb_Instrument.TabIndex = 1;
            this.comb_Instrument.SelectedIndexChanged += new System.EventHandler(this.comb_Instrument_SelectedIndexChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(25, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "帐号：";
            // 
            // comb_account
            // 
            this.comb_account.FormattingEnabled = true;
            this.comb_account.Location = new System.Drawing.Point(72, 56);
            this.comb_account.Name = "comb_account";
            this.comb_account.Size = new System.Drawing.Size(155, 20);
            this.comb_account.TabIndex = 5;
            // 
            // comb_product
            // 
            this.comb_product.FormattingEnabled = true;
            this.comb_product.Location = new System.Drawing.Point(72, 82);
            this.comb_product.Name = "comb_product";
            this.comb_product.Size = new System.Drawing.Size(53, 20);
            this.comb_product.TabIndex = 9;
            this.comb_product.SelectedIndexChanged += new System.EventHandler(this.comb_product_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 85);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 10;
            this.label1.Text = "品种：";
            // 
            // button_login
            // 
            this.button_login.Location = new System.Drawing.Point(72, 108);
            this.button_login.Name = "button_login";
            this.button_login.Size = new System.Drawing.Size(75, 23);
            this.button_login.TabIndex = 11;
            this.button_login.Text = "登陆";
            this.button_login.UseVisualStyleBackColor = true;
            this.button_login.Click += new System.EventHandler(this.button_login_Click);
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(251, 163);
            this.Controls.Add(this.button_login);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comb_account);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comb_product);
            this.Controls.Add(this.comb_Instrument);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "LoginForm";
            this.Text = "快速下单登陆";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoginForm_FormClosing);
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comb_Instrument;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox comb_account;
        private System.Windows.Forms.ComboBox comb_product;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_login;

    }
}