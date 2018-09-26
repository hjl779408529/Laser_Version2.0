using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GTS;
using RTC5Import;
using Laser_Version2._0;
using System.Windows.Forms;
using Laser_Build_1._0;
using Prompt;

namespace Initialization
{
    class Initial
    {
        //GTS初始化内容
        //定义GTS函数调用返回值
        short Com_Return;        
        //强制定义RS232端口
        public static RS232 Com_Comunication = new RS232();
        //定义Tcp连接
        public static Tclient T_Client = new Tclient();
        public void Gts_Initial()
        {
            //打开运动控制器 
            Com_Return = MC.GT_Open(0, 0);
            Log.Commandhandler("Gts_Initial---GT_Open", Com_Return);
            //复位
            GTS_Fun.Factory.Reset();
            //Gts_Fun各功能初始化
            GTS_Fun.Axis_Home Gts_Fun_Axis_Home = new GTS_Fun.Axis_Home();
            GTS_Fun.Factory Gts_Fun_Factory = new GTS_Fun.Factory();
            GTS_Fun.Motion Gts_Fun_Motion = new GTS_Fun.Motion();
            GTS_Fun.Interpolation Gts_Fun_Interpolation = new GTS_Fun.Interpolation(); 

        }
        //激光器初始化内容
        public void Rtc_Initial()
        {
            //复位
            RTC_Fun.Factory.Reset();
            //Rtc_Fun各功能初始化
            RTC_Fun.Factory Rtc_Fun_Factory = new RTC_Fun.Factory();
            RTC_Fun.Motion Rtc_Fun_Motion = new RTC_Fun.Motion();
        }
        //公共初始化内容
        //文件目录指定  配置文件夹所在目录
        const string Dir = @"./\Config";//当前目录下的Config文件夹
        public void Common_Initial()
        {
            //建立配置文件存储目录
            if (!Directory.Exists(Dir))
            {
                Directory.CreateDirectory(Dir);
            }
            //读取参数
            //配方数据读取
            Para_List.Serialize_Parameter.Reserialize("Para.xml");

        }
        //232通讯初始化
        public void RS232_Initial() 
        {
            if (Para_List.Parameter.Com_No < Com_Comunication.PortName.Count)
            {
                Com_Comunication.Open_Com(Para_List.Parameter.Com_No);
            }
            else
            {
                MessageBox.Show("激光控制器通讯串口端口编号异常，请在激光控制面板选择正确的串口编号！！！");
            }            
        }
        //Tcp通讯初始化
        public void Tcp_Initial() 
        {
            T_Client.TCP_Start();
        }
        //laser 功率矫正初始化

    }


}