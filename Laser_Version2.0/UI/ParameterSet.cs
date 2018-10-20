﻿using GTS;
using Laser_Build_1._0;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laser_Version2._0
{
    public partial class ParameterSet : Form
    {
        public ParameterSet()
        {
            InitializeComponent();
        }
        //输入转换变量
        Vector Tmp_Mark = new Vector();
        //触发数据
        short Intrigue = 1;
        // 像素 毫米 比
        private void Set_txt_valueK_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(Set_txt_valueK.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Cam_Reference = tmp;//1像素=0.008806mm ()
            });
        }
        /// <summary>
        /// 矫正Mark点坐标实际位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Set_Btn_Mark1_Click(object sender, EventArgs e)
        {
            Calibration.Calibrate_Mark(0);
            //更新显示
            Set_txt_markX1.Text = Para_List.Parameter.Mark1.X.ToString();
            Set_txt_markY1.Text = Para_List.Parameter.Mark1.Y.ToString();
            Set_txt_markX2.Text = Para_List.Parameter.Mark2.X.ToString();
            Set_txt_markY2.Text = Para_List.Parameter.Mark2.Y.ToString();
            Set_txt_markX3.Text = Para_List.Parameter.Mark3.X.ToString();
            Set_txt_markY3.Text = Para_List.Parameter.Mark3.Y.ToString();
            Set_txt_markX4.Text = Para_List.Parameter.Mark4.X.ToString();
            Set_txt_markY4.Text = Para_List.Parameter.Mark4.Y.ToString();
        }
        ///Re_Calibration Mark
        private void Re_Cali_Mark_Click(object sender, EventArgs e)
        {
            Calibration.Calibrate_Mark(0);
        }
        //Mark1 X坐标
        private void Set_txt_markX1_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Set_txt_markX1.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if ((tmp >= 0) && (tmp <= 350))
            {
                Tmp_Mark.X = tmp;
                Tmp_Mark.Y = Para_List.Parameter.Mark1.Y;
                Para_List.Parameter.Mark1 =new Vector(Tmp_Mark);
            }
            else
            {
                MessageBox.Show("请确认数值在0-350范围内");
                return;
            }
            
        }
        //Mark1 Y坐标
        private void Set_txt_markY1_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Set_txt_markY1.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if ((tmp>=0) && (tmp <= 350))
            {
                Tmp_Mark.X = Para_List.Parameter.Mark1.X;
                Tmp_Mark.Y = tmp;
                Para_List.Parameter.Mark1 = new Vector(Tmp_Mark);
            }
            else
            {
                MessageBox.Show("请确认数值在0-350范围内");
                return;
            }
            
        }
        //Mark2 X坐标
        private void Set_txt_markX2_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Set_txt_markX2.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if ((tmp >= 0) && (tmp <= 350))
            {
                Tmp_Mark.X = tmp;
                Tmp_Mark.Y = Para_List.Parameter.Mark2.Y;
                Para_List.Parameter.Mark2 = new Vector(Tmp_Mark);
            }
            else
            {
                MessageBox.Show("请确认数值在0-350范围内");
                return;
            }
            
        }
        //Mark2 Y坐标
        private void Set_txt_markY2_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Set_txt_markY2.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if ((tmp >= 0) && (tmp <= 350))
            {
                Tmp_Mark.X = Para_List.Parameter.Mark2.X;
                Tmp_Mark.Y = tmp;
                Para_List.Parameter.Mark2 = new Vector(Tmp_Mark);
            }
            else
            {
                MessageBox.Show("请确认数值在0-350范围内");
                return;
            }
            
        }
        //Mark3 X坐标
        private void Set_txt_markX3_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Set_txt_markX3.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if ((tmp >= 0) && (tmp <= 350))
            {
                Tmp_Mark.X = tmp;
                Tmp_Mark.Y = Para_List.Parameter.Mark3.Y;
                Para_List.Parameter.Mark3 = new Vector(Tmp_Mark);
            }
            else
            {
                MessageBox.Show("请确认数值在0-350范围内");
                return;
            }
            
        }
        //Mark3 Y坐标
        private void Set_txt_markY3_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Set_txt_markY3.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if ((tmp >= 0) && (tmp <= 350))
            {
                Tmp_Mark.X = Para_List.Parameter.Mark3.X;
                Tmp_Mark.Y = tmp;
                Para_List.Parameter.Mark3 = new Vector(Tmp_Mark);
            }
            else
            {
                MessageBox.Show("请确认数值在0-350范围内");
                return;
            }            
        }
        //Mark4 X坐标
        private void Set_txt_markX4_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Set_txt_markX4.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if ((tmp >= 0) && (tmp <= 350))
            {
                Tmp_Mark.X = tmp;
                Tmp_Mark.Y = Para_List.Parameter.Mark4.Y;
                Para_List.Parameter.Mark4 = new Vector(Tmp_Mark);
            }
            else
            {
                MessageBox.Show("请确认数值在0-350范围内");
                return;
            }
        }
        //Mark4 Y坐标
        private void Set_txt_markY4_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(Set_txt_markY4.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            if ((tmp >= 0) && (tmp <= 350))
            {
                Tmp_Mark.X = Para_List.Parameter.Mark4.X;
                Tmp_Mark.Y = tmp;
                Para_List.Parameter.Mark4 = new Vector(Tmp_Mark);
            }
            else
            {
                MessageBox.Show("请确认数值在0-350范围内");
                return;
            }
        }
        //form load
        private void ParameterSet_Load(object sender, EventArgs e)
        {
            Set_txt_valueK.Text = Para_List.Parameter.Cam_Reference.ToString();
            Set_txt_markX1.Text = Para_List.Parameter.Mark1.X.ToString();
            Set_txt_markY1.Text = Para_List.Parameter.Mark1.Y.ToString();
            Set_txt_markX2.Text = Para_List.Parameter.Mark2.X.ToString();
            Set_txt_markY2.Text = Para_List.Parameter.Mark2.Y.ToString();
            Set_txt_markX3.Text = Para_List.Parameter.Mark3.X.ToString();
            Set_txt_markY3.Text = Para_List.Parameter.Mark3.Y.ToString();
            Set_txt_markX4.Text = Para_List.Parameter.Mark4.X.ToString();
            Set_txt_markY4.Text = Para_List.Parameter.Mark4.Y.ToString();
            textBox19.Text = Para_List.Parameter.Rtc_Org.X.ToString();
            textBox18.Text = Para_List.Parameter.Rtc_Org.Y.ToString();
            numericUpDown1.Value = (short)Intrigue;
        }       
        //定位mark点1
        private void button1_Click(object sender, EventArgs e)
        {
            Calibration.Mark(Para_List.Parameter.Mark1);
        }
        //定位mark点2
        private void button2_Click(object sender, EventArgs e)
        {
            Calibration.Mark(Para_List.Parameter.Mark2);
        }
        //定位mark点3
        private void button3_Click(object sender, EventArgs e)
        {
            Calibration.Mark(Para_List.Parameter.Mark3);
        }
        //定位mark点4
        private void button6_Click(object sender, EventArgs e)
        {
            Calibration.Mark(Para_List.Parameter.Mark4);
        }
        //振镜与ORG 中心差值X/mm
        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(textBox19.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Para_List.Parameter.Rtc_Org=new Vector(tmp, Para_List.Parameter.Rtc_Org.Y);
        }
        //振镜与ORG 中心差值Y/mm
        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            if (!decimal.TryParse(textBox18.Text, out decimal tmp))
            {
                MessageBox.Show("请正确输入数字");
                return;
            }
            Para_List.Parameter.Rtc_Org=new Vector(Para_List.Parameter.Rtc_Org.X,tmp);
        }
        //矫正振镜与ORG的偏差
        private void button4_Click(object sender, EventArgs e)
        {
            Calibration.Calibrate_RTC_ORG();
            textBox19.Text = Para_List.Parameter.Rtc_Org.X.ToString(4);
            textBox18.Text = Para_List.Parameter.Rtc_Org.Y.ToString(4);
        }
        //触发拍照
        private void button5_Click(object sender, EventArgs e)
        {
            Initialization.Initial.T_Client.Get_Cam_Deviation(Intrigue);//触发拍照 
        }
        //Cam_Intrigue_Num
        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            Intrigue = (short)numericUpDown1.Value;
        }
        //Re_Connect_Tcp
        private void Re_Connect_Click(object sender, EventArgs e)
        {
            Initialization.Initial.T_Client.TCP_Start("127.0.0.1",6230);
        }
        //Disconnect_Tcp
        private void Disconnect_Tcp_Click(object sender, EventArgs e)
        {
            Initialization.Initial.T_Client.Stop_Connect();
        }
        //矫正相机坐标系
        private void Correct_Cam_Cor_Click(object sender, EventArgs e)
        {
            if (Calibration.Cal_Cam_Affinity())
            {
                MessageBox.Show("相机坐标系矫正完成！！！");
            }
            else
            {
                MessageBox.Show("相机坐标系矫正失败！！！");
            }
        }
    }
}
