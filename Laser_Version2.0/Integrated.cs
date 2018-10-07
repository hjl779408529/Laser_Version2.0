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
        public static bool Exit_Flag = false;
        public bool Exit_Flag_00 = false; 
        public bool Exit_Flag_01 = false;
        public bool Exit_Flag_02 = false;
        //该函数，利用传入的数据，将RTC和GTS数据配合，进行裁切
        public static void Rts_Gts(List<List<Interpolation_Data>> List_Datas)
        {
            //初始振镜回Home
            RTC_Fun.Motion.Home();

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
            int i = 0, j = 0,repeat = 0;
            //数据处理
            for (i = 0; i < List_Datas.Count; i++)//外层数据
            {
                for (j = 0; j < List_Datas[i].Count; j++)//内层数据
                {
                    if (List_Datas[i][j].Work == 10)//Gts加工数据
                    {
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //启动Gts运动
                            GTS_Fun.Interpolation.Integrate(List_Datas[i]);
                        }
                        else
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //定位到零点
                            RTC_Fun.Motion.Home();
                            //重复加工
                            for (repeat = 0; repeat < Para_List.Parameter.Gts_Repeat; repeat++)
                            {
                                //Gts移动到启动位置 上一list数据的结尾数据或本次的结尾或本次序号0的start;待测试
                                GTS_Fun.Interpolation.Gts_Ready(List_Datas[i][0].Start_x, List_Datas[i][0].Start_y);
                                //打开激光
                                RTC_Fun.Motion.Open_Laser();
                                //启动Gts运动
                                GTS_Fun.Interpolation.Integrate(List_Datas[i]);
                                //关闭激光
                                RTC_Fun.Motion.Close_Laser();
                                //退出执行
                                if (Exit_Flag)
                                {
                                    Exit_Flag = false;
                                    return;
                                }
                            }                               

                        }
                    }
                    else if (List_Datas[i][j].Work == 20)//Rtc加工数据
                    {
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //启动RTC加工
                            RTC_Fun.Motion.Draw(List_Datas[i], 1); 
                        }
                        else
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //Gts移动到准备位置 本次开头
                            GTS_Fun.Interpolation.Gts_Ready(List_Datas[i][0].Gts_x, List_Datas[i][0].Gts_y);
                            //重复加工
                            for (repeat = 0; repeat < Para_List.Parameter.Rtc_Repeat; repeat++)
                            {                                
                                //启动加工
                                RTC_Fun.Motion.Draw(List_Datas[i], 1);
                                //退出执行
                                if (Exit_Flag)
                                {
                                    Exit_Flag = false;
                                    return;
                                }
                            }
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();

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
        //该函数，利用传入的数据，将RTC和GTS数据配合，进行裁切 带入RTC与Gts校准
        public static void Rts_Gts_Correct(List<List<Interpolation_Data>> List_Datas)
        {
            //初始振镜回Home
            //Rtc_Motion.Home();

            //临时变量
            int i = 0, j = 0;
            //数据处理
            for (i = 0; i < List_Datas.Count; i++)//外层数据
            {
                for (j = 0; j < List_Datas[i].Count; j++)//内层数据
                {

                    if (List_Datas[i][j].Work == 10)//Gts加工数据
                    {
                        
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
#endif

                            //启动Gts运动
                            GTS_Fun.Interpolation.Integrate_Correct(List_Datas[i]);

                        }
                        else
                        {
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //定位到零点
                            RTC_Fun.Motion.Home();
#endif
                            //Gts移动到启动位置 上一list数据的结尾数据或本次的结尾;待测试
                            GTS_Fun.Interpolation.Gts_Ready_Correct(List_Datas[i][0].Start_x, List_Datas[i][0].Start_y);
                            //打开激光
#if !DEBUG
                            RTC_Fun.Motion.Open_Laser();
#endif
                            //启动Gts运动
                            GTS_Fun.Interpolation.Integrate_Correct(List_Datas[i]);
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
#endif
                        }

                    }
                        else if (List_Datas[i][j].Work == 20)//Rtc加工数据
                    {
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //启动RTC加工
                            RTC_Fun.Motion.Draw_Correct(List_Datas[i], 1);
#endif
                        }
                        else
                        {
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
#endif
                            //Gts移动到准备位置 本次开头
                            GTS_Fun.Interpolation.Gts_Ready_Correct(List_Datas[i][0].Gts_x, List_Datas[i][0].Gts_y);
#if !DEBUG
                            //启动加工
                            RTC_Fun.Motion.Draw_Correct(List_Datas[i], 1);
#endif
#if !DEBUG
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
#endif

                        }
                    }
                    break;
                }
                //退出执行
                if (Exit_Flag)
                {
                    Exit_Flag = false;
                    RTC_Fun.Motion.Home();
                    return;
                }
            }
            RTC_Fun.Motion.Home();
        }

        //该函数，利用传入的数据，将RTC和GTS数据配合，进行裁切  主要生成RTC校准坐标 用于生RTC板卡校准文件
        public static void Rts_Gts_Cal_Rtc(List<List<Interpolation_Data>> List_Datas) 
        {
            //初始振镜回Home
            RTC_Fun.Motion.Home();

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
            for (i = 0; i < List_Datas.Count; i++)//外层数据
            {
                for (j = 0; j < List_Datas[i].Count; j++)//内层数据
                {
                    if (List_Datas[i][j].Work == 10)//Gts加工数据
                    {
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //启动Gts运动
                            GTS_Fun.Interpolation.Integrate(List_Datas[i]);
                        }
                        else
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //定位到零点
                            RTC_Fun.Motion.Home();
                            //Gts移动到启动位置 上一list数据的结尾数据或本次的结尾或本次序号0的start;待测试
                            GTS_Fun.Interpolation.Gts_Ready(List_Datas[i][0].Start_x, List_Datas[i][0].Start_y);
                            //打开激光
                            RTC_Fun.Motion.Open_Laser();

                            //启动Gts运动
                            GTS_Fun.Interpolation.Integrate(List_Datas[i]);

                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();

                        }
                    }
                    else if (List_Datas[i][j].Work == 20)//Rtc加工数据
                    {
                        if (List_Datas[i][j].Lift_flag == 1)//抬刀标志
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //启动RTC加工
                            RTC_Fun.Motion.Draw_Cal(List_Datas[i], 1);
                        }
                        else
                        {
                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();
                            //Gts移动到准备位置 本次开头
                            GTS_Fun.Interpolation.Gts_Ready(List_Datas[i][0].Gts_x, List_Datas[i][0].Gts_y);
                            //启动加工
                            RTC_Fun.Motion.Draw_Cal(List_Datas[i], 1);

                            //关闭激光
                            RTC_Fun.Motion.Close_Laser();

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
                GTS_Fun.Axis_Home.Home(1);
            }
        }

        private void Axis02_Home()
        {
            if (Prompt.Refresh.Axis02_EN)
            {
                GTS_Fun.Axis_Home.Home(2);
            }
        }
    }
}
