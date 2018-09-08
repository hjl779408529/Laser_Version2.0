using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Laser_Build_1._0
{
    class Integrated
    {
        //退出执行
        public bool Exit_Flag = false;
        
        //定义Rtc函数
        RTC_Fun.Motion Rtc_Motion = new RTC_Fun.Motion(); 
        //定义Gts函数
        GTS_Fun.Interpolation Gts_Fun = new GTS_Fun.Interpolation();
        GTS_Fun.Axis_Home axis01_Home = new GTS_Fun.Axis_Home();
        GTS_Fun.Axis_Home axis02_Home = new GTS_Fun.Axis_Home();
        //该函数，利用传入的数据，将RTC和GTS数据配合，进行裁切
        public void Rts_Gts(List<List<Interpolation_Data>> List_Datas) 
        {
            //初始振镜回Home
            Rtc_Motion.Home();

            //XY平台回零
            //两轴回零
            //Thread Axis01_home_thread = new Thread(this.Axis01_Home);
            //Thread Axis02_home_thread = new Thread(this.Axis02_Home);
            //Axis01_home_thread.Start();
            //Axis02_home_thread.Start();
            ////等待线程结束
            //Axis01_home_thread.Join();
            //Axis02_home_thread.Join();

            ////建立直角坐标系
            //Gts_Fun.Coordination(Para_List.Parameter.Syn_MaxVel, Para_List.Parameter.Syn_MaxAcc, Para_List.Parameter.Syn_EvenTime, Para_List.Parameter.Work_X, Para_List.Parameter.Work_Y);
            ////定位到加工坐标原点
            //Gts_Fun.Clear_FIFO();
            //Gts_Fun.Line_FIFO(0, 0, Para_List.Parameter.Line_synVel, Para_List.Parameter.Line_synAcc, Para_List.Parameter.Line_endVel);//将直线插补数据写入
            //Gts_Fun.Interpolation_Start();
            ////停止坐标系运动
            //Gts_Fun.Interpolation_Stop();

            //临时变量
            int i = 0, j = 0;
            //数据处理
            for (i=0; i<List_Datas.Count; i++)//外层数据
            {
                for (j=0; j<List_Datas[i].Count; j++)//内层数据
                {
                    if (List_Datas[i][j].Work == 10)//Gts加工数据
                    {
                        if (List_Datas[i][j].Lift_flag==1)//抬刀标志
                        {
                            //关闭激光
                            Rtc_Motion.Close_Laser();
                            //启动Gts运动
                            Gts_Fun.Integrate(List_Datas[i]);
                        }
                        else
                        {
                            //关闭激光
                            Rtc_Motion.Close_Laser();
                            //定位到零点
                            Rtc_Motion.Home();
                            //Gts移动到启动位置 上一list数据的结尾数据或本次的结尾;待测试
                            Gts_Fun.Gts_Ready(List_Datas[i][List_Datas[i].Count-1].End_x, List_Datas[i][List_Datas[i].Count - 1].End_y);
                            //打开激光
                            Rtc_Motion.Open_Laser();

                            //启动Gts运动
                            Gts_Fun.Integrate(List_Datas[i]);

                            //关闭激光
                            Rtc_Motion.Close_Laser();

                        }
                    }
                    else if (List_Datas[i][j].Work == 20)//Rtc加工数据
                    {
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
                            //关闭激光
                            Rtc_Motion.Close_Laser();
                            //启动RTC加工
                            Rtc_Motion.Draw(List_Datas[i], 1);
                        }
                        else
                        {
                            //关闭激光
                            Rtc_Motion.Close_Laser();
                            //Gts移动到准备位置 本次开头
                            Gts_Fun.Gts_Ready(List_Datas[i][0].Gts_x, List_Datas[i][0].Gts_y); 
                            //启动加工
                            Rtc_Motion.Draw(List_Datas[i], 1);

                            //关闭激光
                            Rtc_Motion.Close_Laser();
                            
                        }
                    }
                    break;
                }
                //退出执行
                if (Exit_Flag)
                {
                    Exit_Flag = false;
                    return;
                }
            }
        }

        private void Axis01_Home()
        {
            if (Prompt.Refresh.Axis01_EN)
            {
                axis01_Home.Home(1);
            }
        }

        private void Axis02_Home()
        {
            if (Prompt.Refresh.Axis02_EN)
            {
                axis02_Home.Home(2);
            }
        }
    }
}
