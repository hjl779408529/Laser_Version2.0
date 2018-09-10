using GTS;
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
        //搜寻Mark
        Generate_Affinity_Matrix Cal_Mark = new Generate_Affinity_Matrix();
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
            Cal_Mark.Calibrate_Mark();
            //更新显示
            Set_txt_markX1.Text = Para_List.Parameter.Mark1.X.ToString();
            Set_txt_markY1.Text = Para_List.Parameter.Mark1.Y.ToString();
            Set_txt_markX2.Text = Para_List.Parameter.Mark2.X.ToString();
            Set_txt_markY2.Text = Para_List.Parameter.Mark2.Y.ToString();
            Set_txt_markX3.Text = Para_List.Parameter.Mark3.X.ToString();
            Set_txt_markY3.Text = Para_List.Parameter.Mark3.Y.ToString();
            
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

        private void ParameterSet_Load(object sender, EventArgs e)
        {
            Set_txt_valueK.Text = Para_List.Parameter.Cam_Reference.ToString();
            Set_txt_markX1.Text = Para_List.Parameter.Mark1.X.ToString();
            Set_txt_markY1.Text = Para_List.Parameter.Mark1.Y.ToString();
            Set_txt_markX2.Text = Para_List.Parameter.Mark2.X.ToString();
            Set_txt_markY2.Text = Para_List.Parameter.Mark2.Y.ToString();
            Set_txt_markX3.Text = Para_List.Parameter.Mark3.X.ToString();
            Set_txt_markY3.Text = Para_List.Parameter.Mark3.Y.ToString();
        }
    }
}
