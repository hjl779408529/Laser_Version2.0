using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using GTS;
using RTC5Import;

namespace Initialization
{
    class Initial
    {
        //GTS初始化内容
        //定义GTS函数调用返回值
        short Com_Return;
        //定义日志输出函数
        readonly Prompt.Log Log = new Prompt.Log();
        //调用Factory
        GTS_Fun.Factory Gts_Factory = new GTS_Fun.Factory();
        RTC_Fun.Factory Rtc_Factory = new RTC_Fun.Factory();
        
        public void Gts_Initial()
        {
            //打开运动控制器 
            Com_Return = MC.GT_Open(0, 0);
            Log.Commandhandler("Gts_Initial---GT_Open", Com_Return);
            //复位GtsD:\Visualstudio_Project\Laser\Laser_Program\Laser_Version2.0\Laser_Version2.0\RTC_Fun.cs
            Gts_Factory.Reset();
            
        }


        //激光器初始化内容
        public void Rtc_Initial()
        {
            Rtc_Factory.Reset();
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
            Para_List.Serialize_Parameter Load_Parameter = new Para_List.Serialize_Parameter();
            Load_Parameter.Reserialize("Para.xml");

        }
    }


}