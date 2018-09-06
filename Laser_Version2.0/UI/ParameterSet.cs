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
        // 像素 毫米 比
        private void Set_txt_valueK_TextChanged(object sender, EventArgs e)
        {
            Para_List.Parameter.Cam_Reference = Convert.ToDecimal(Set_txt_valueK.Text);//1像素=0.008806mm ()
        }

        /// <summary>
        /// 8.31 获取Mark1点坐标位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Set_Btn_Mark1_Click(object sender, EventArgs e)
        {
            PointF pointF = new PointF();
           // 计算平台实际坐标 - 1
            pointF =get_mark_point();
            //更新显示
            Set_txt_markX1.Text = pointF.X.ToString();
            Set_txt_markY1.Text = pointF.Y.ToString();
        }
        /// <summary>
        /// 获取Mark2点坐标位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Set_Btn_Mark2_Click(object sender, EventArgs e)
        {
            PointF pointF = new PointF();
            //计算平台实际坐标-2
            pointF = get_mark_point();
            //更新显示
            Set_txt_markX2.Text = pointF.X.ToString();
            Set_txt_markY2.Text = pointF.Y.ToString();
        }
        /// <summary>
        /// 获取Mark3点坐标位置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Set_Btn_Mark3_Click(object sender, EventArgs e)
        {
            PointF pointF = new PointF();
            //计算平台实际坐标-3
            pointF = get_mark_point();
            //更新显示
            Set_txt_markX3.Text = pointF.X.ToString();
            Set_txt_markY3.Text = pointF.Y.ToString();
        }
        /// <summary>
        /// 获取 MARK点 平台坐标 GTS—
        /// </summary>
        /// <returns></returns>
        private PointF get_mark_point()
        {
            Para_List.Parameter.Cam_Reference = 0.00806m;
            PointF pointF = new PointF();
            //T_Client.
            Main.T_Client.Senddata(2);//触发拍照 1：标定 2：Mark点
            do
            {

            } while (!Main.T_Client.Rec_Ok);
            decimal Cam_New_X = Main.T_Client.Receive_Cordinate.X * Para_List.Parameter.Cam_Reference;
            decimal Cam_New_Y = Main.T_Client.Receive_Cordinate.Y * Para_List.Parameter.Cam_Reference;
            GTS.MC.GT_GetCrdPos(0, out double temp_X);
            GTS.MC.GT_GetCrdPos(1, out double temp_Y);
            pointF.X = (float)((decimal)temp_X + Cam_New_X);
            pointF.Y = (float)((decimal)temp_Y + Cam_New_Y);

            return pointF;
        }

    }
}
