namespace Laser_Version2._0
{
    partial class ParameterSet
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
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.Disconnect_Tcp = new System.Windows.Forms.Button();
            this.Re_Connect = new System.Windows.Forms.Button();
            this.button5 = new System.Windows.Forms.Button();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.button4 = new System.Windows.Forms.Button();
            this.label20 = new System.Windows.Forms.Label();
            this.textBox18 = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.textBox19 = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button6 = new System.Windows.Forms.Button();
            this.Set_txt_markY4 = new System.Windows.Forms.TextBox();
            this.Set_txt_markX4 = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.Set_Btn_Mark1 = new System.Windows.Forms.Button();
            this.Set_txt_markX1 = new System.Windows.Forms.TextBox();
            this.Set_txt_markY3 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.Set_txt_markX3 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.Set_txt_markY2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.Set_txt_markX2 = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.Set_txt_markY1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.Set_txt_valueK = new System.Windows.Forms.TextBox();
            this.Re_Cali_Mark = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 52);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 18);
            this.label1.TabIndex = 0;
            this.label1.Text = "K 值:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.Disconnect_Tcp);
            this.groupBox1.Controls.Add(this.Re_Connect);
            this.groupBox1.Controls.Add(this.button5);
            this.groupBox1.Controls.Add(this.numericUpDown1);
            this.groupBox1.Controls.Add(this.button4);
            this.groupBox1.Controls.Add(this.label20);
            this.groupBox1.Controls.Add(this.textBox18);
            this.groupBox1.Controls.Add(this.label21);
            this.groupBox1.Controls.Add(this.textBox19);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.Set_txt_valueK);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(14, 14);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(650, 673);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "系统参数";
            // 
            // Disconnect_Tcp
            // 
            this.Disconnect_Tcp.Location = new System.Drawing.Point(385, 598);
            this.Disconnect_Tcp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Disconnect_Tcp.Name = "Disconnect_Tcp";
            this.Disconnect_Tcp.Size = new System.Drawing.Size(109, 52);
            this.Disconnect_Tcp.TabIndex = 100;
            this.Disconnect_Tcp.Text = "断开相机";
            this.Disconnect_Tcp.UseVisualStyleBackColor = true;
            this.Disconnect_Tcp.Click += new System.EventHandler(this.Disconnect_Tcp_Click);
            // 
            // Re_Connect
            // 
            this.Re_Connect.Location = new System.Drawing.Point(252, 598);
            this.Re_Connect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Re_Connect.Name = "Re_Connect";
            this.Re_Connect.Size = new System.Drawing.Size(109, 52);
            this.Re_Connect.TabIndex = 99;
            this.Re_Connect.Text = "重连相机";
            this.Re_Connect.UseVisualStyleBackColor = true;
            this.Re_Connect.Click += new System.EventHandler(this.Re_Connect_Click);
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(518, 598);
            this.button5.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(109, 52);
            this.button5.TabIndex = 8;
            this.button5.Text = "触发拍照";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(548, 542);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            2,
            0,
            0,
            0});
            this.numericUpDown1.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(48, 28);
            this.numericUpDown1.TabIndex = 98;
            this.numericUpDown1.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(276, 530);
            this.button4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(195, 52);
            this.button4.TabIndex = 8;
            this.button4.Text = "矫正振镜与ORG的偏差";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(25, 590);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(206, 18);
            this.label20.TabIndex = 97;
            this.label20.Text = "振镜与ORG 中心差值Y/mm";
            // 
            // textBox18
            // 
            this.textBox18.Location = new System.Drawing.Point(53, 624);
            this.textBox18.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox18.Name = "textBox18";
            this.textBox18.Size = new System.Drawing.Size(150, 28);
            this.textBox18.TabIndex = 96;
            this.textBox18.TextChanged += new System.EventHandler(this.textBox18_TextChanged);
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(25, 510);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(206, 18);
            this.label21.TabIndex = 95;
            this.label21.Text = "振镜与ORG 中心差值X/mm";
            // 
            // textBox19
            // 
            this.textBox19.Location = new System.Drawing.Point(53, 545);
            this.textBox19.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBox19.Name = "textBox19";
            this.textBox19.Size = new System.Drawing.Size(150, 28);
            this.textBox19.TabIndex = 94;
            this.textBox19.TextChanged += new System.EventHandler(this.textBox19_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(367, 50);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(107, 18);
            this.label7.TabIndex = 5;
            this.label7.Text = "(毫米/像素)";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.Re_Cali_Mark);
            this.groupBox2.Controls.Add(this.button6);
            this.groupBox2.Controls.Add(this.Set_txt_markY4);
            this.groupBox2.Controls.Add(this.Set_txt_markX4);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.button3);
            this.groupBox2.Controls.Add(this.button2);
            this.groupBox2.Controls.Add(this.button1);
            this.groupBox2.Controls.Add(this.Set_Btn_Mark1);
            this.groupBox2.Controls.Add(this.Set_txt_markX1);
            this.groupBox2.Controls.Add(this.Set_txt_markY3);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.Set_txt_markX3);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.Set_txt_markY2);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.Set_txt_markX2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.Set_txt_markY1);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Location = new System.Drawing.Point(0, 85);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox2.Size = new System.Drawing.Size(650, 373);
            this.groupBox2.TabIndex = 4;
            this.groupBox2.TabStop = false;
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(545, 229);
            this.button6.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(91, 28);
            this.button6.TabIndex = 11;
            this.button6.Text = "定位";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // Set_txt_markY4
            // 
            this.Set_txt_markY4.Location = new System.Drawing.Point(367, 229);
            this.Set_txt_markY4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_markY4.Name = "Set_txt_markY4";
            this.Set_txt_markY4.Size = new System.Drawing.Size(148, 28);
            this.Set_txt_markY4.TabIndex = 9;
            this.Set_txt_markY4.TextChanged += new System.EventHandler(this.Set_txt_markY4_TextChanged);
            // 
            // Set_txt_markX4
            // 
            this.Set_txt_markX4.Location = new System.Drawing.Point(169, 229);
            this.Set_txt_markX4.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_markX4.Name = "Set_txt_markX4";
            this.Set_txt_markX4.Size = new System.Drawing.Size(148, 28);
            this.Set_txt_markX4.TabIndex = 10;
            this.Set_txt_markX4.TextChanged += new System.EventHandler(this.Set_txt_markX4_TextChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 235);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(152, 18);
            this.label8.TabIndex = 8;
            this.label8.Text = "Mark 点4（右下）";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(544, 180);
            this.button3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(91, 28);
            this.button3.TabIndex = 7;
            this.button3.Text = "定位";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(544, 133);
            this.button2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(91, 28);
            this.button2.TabIndex = 6;
            this.button2.Text = "定位";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(544, 85);
            this.button1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(91, 28);
            this.button1.TabIndex = 5;
            this.button1.Text = "定位";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // Set_Btn_Mark1
            // 
            this.Set_Btn_Mark1.Location = new System.Drawing.Point(53, 301);
            this.Set_Btn_Mark1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_Btn_Mark1.Name = "Set_Btn_Mark1";
            this.Set_Btn_Mark1.Size = new System.Drawing.Size(195, 52);
            this.Set_Btn_Mark1.TabIndex = 4;
            this.Set_Btn_Mark1.Text = "矫正Mark坐标";
            this.Set_Btn_Mark1.UseVisualStyleBackColor = true;
            this.Set_Btn_Mark1.Click += new System.EventHandler(this.Set_Btn_Mark1_Click);
            // 
            // Set_txt_markX1
            // 
            this.Set_txt_markX1.Location = new System.Drawing.Point(168, 85);
            this.Set_txt_markX1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_markX1.Name = "Set_txt_markX1";
            this.Set_txt_markX1.Size = new System.Drawing.Size(148, 28);
            this.Set_txt_markX1.TabIndex = 3;
            this.Set_txt_markX1.TextChanged += new System.EventHandler(this.Set_txt_markX1_TextChanged);
            // 
            // Set_txt_markY3
            // 
            this.Set_txt_markY3.Location = new System.Drawing.Point(366, 180);
            this.Set_txt_markY3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_markY3.Name = "Set_txt_markY3";
            this.Set_txt_markY3.Size = new System.Drawing.Size(148, 28);
            this.Set_txt_markY3.TabIndex = 3;
            this.Set_txt_markY3.TextChanged += new System.EventHandler(this.Set_txt_markY3_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(152, 18);
            this.label2.TabIndex = 2;
            this.label2.Text = "Mark 点1（左下）";
            // 
            // Set_txt_markX3
            // 
            this.Set_txt_markX3.Location = new System.Drawing.Point(168, 180);
            this.Set_txt_markX3.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_markX3.Name = "Set_txt_markX3";
            this.Set_txt_markX3.Size = new System.Drawing.Size(148, 28);
            this.Set_txt_markX3.TabIndex = 3;
            this.Set_txt_markX3.TextChanged += new System.EventHandler(this.Set_txt_markX3_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(231, 43);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(26, 18);
            this.label5.TabIndex = 2;
            this.label5.Text = "X ";
            // 
            // Set_txt_markY2
            // 
            this.Set_txt_markY2.Location = new System.Drawing.Point(366, 133);
            this.Set_txt_markY2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_markY2.Name = "Set_txt_markY2";
            this.Set_txt_markY2.Size = new System.Drawing.Size(148, 28);
            this.Set_txt_markY2.TabIndex = 3;
            this.Set_txt_markY2.TextChanged += new System.EventHandler(this.Set_txt_markY2_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(433, 43);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(17, 18);
            this.label6.TabIndex = 2;
            this.label6.Text = "Y";
            // 
            // Set_txt_markX2
            // 
            this.Set_txt_markX2.Location = new System.Drawing.Point(168, 133);
            this.Set_txt_markX2.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_markX2.Name = "Set_txt_markX2";
            this.Set_txt_markX2.Size = new System.Drawing.Size(148, 28);
            this.Set_txt_markX2.TabIndex = 3;
            this.Set_txt_markX2.TextChanged += new System.EventHandler(this.Set_txt_markX2_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 139);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(152, 18);
            this.label3.TabIndex = 2;
            this.label3.Text = "Mark 点2（左上）";
            // 
            // Set_txt_markY1
            // 
            this.Set_txt_markY1.Location = new System.Drawing.Point(366, 85);
            this.Set_txt_markY1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_markY1.Name = "Set_txt_markY1";
            this.Set_txt_markY1.Size = new System.Drawing.Size(148, 28);
            this.Set_txt_markY1.TabIndex = 3;
            this.Set_txt_markY1.TextChanged += new System.EventHandler(this.Set_txt_markY1_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(14, 186);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(152, 18);
            this.label4.TabIndex = 2;
            this.label4.Text = "Mark 点3（右上）";
            // 
            // Set_txt_valueK
            // 
            this.Set_txt_valueK.Location = new System.Drawing.Point(93, 48);
            this.Set_txt_valueK.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Set_txt_valueK.Name = "Set_txt_valueK";
            this.Set_txt_valueK.Size = new System.Drawing.Size(247, 28);
            this.Set_txt_valueK.TabIndex = 1;
            this.Set_txt_valueK.TextChanged += new System.EventHandler(this.Set_txt_valueK_TextChanged);
            // 
            // Re_Cali_Mark
            // 
            this.Re_Cali_Mark.Location = new System.Drawing.Point(401, 301);
            this.Re_Cali_Mark.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Re_Cali_Mark.Name = "Re_Cali_Mark";
            this.Re_Cali_Mark.Size = new System.Drawing.Size(195, 52);
            this.Re_Cali_Mark.TabIndex = 12;
            this.Re_Cali_Mark.Text = "二次矫正Mark坐标";
            this.Re_Cali_Mark.UseVisualStyleBackColor = true;
            this.Re_Cali_Mark.Click += new System.EventHandler(this.Re_Cali_Mark_Click);
            // 
            // ParameterSet
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 723);
            this.Controls.Add(this.groupBox1);
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "ParameterSet";
            this.Text = "ParameterSet";
            this.Load += new System.EventHandler(this.ParameterSet_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox Set_txt_valueK;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox Set_txt_markY3;
        private System.Windows.Forms.TextBox Set_txt_markX3;
        private System.Windows.Forms.TextBox Set_txt_markY2;
        private System.Windows.Forms.TextBox Set_txt_markX2;
        private System.Windows.Forms.TextBox Set_txt_markY1;
        private System.Windows.Forms.TextBox Set_txt_markX1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button Set_Btn_Mark1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox textBox18;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox textBox19;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Button Re_Connect;
        private System.Windows.Forms.Button Disconnect_Tcp;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.TextBox Set_txt_markY4;
        private System.Windows.Forms.TextBox Set_txt_markX4;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button Re_Cali_Mark;
    }
}