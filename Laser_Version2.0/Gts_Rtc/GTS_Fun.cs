using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;
using GTS;
using Laser_Build_1._0;
using Prompt;

namespace GTS_Fun
{

    //复位
    class Factory
    {
        //定义GTS函数调用返回值
        private static short Com_Return;

        public static void Reset()
        {           
            //复位运动控制器
            Com_Return = MC.GT_Reset();
            Log.Commandhandler("Gts_Initial---GT_Reset", Com_Return);
            //配置运动控制器
            Com_Return = MC.GT_LoadConfig("Axis.cfg");
            Log.Commandhandler("Gts_Initial--GT_LoadConfig", Com_Return);
            //清除各轴的报警和限位
            Com_Return = MC.GT_ClrSts(1, 4);
            Log.Commandhandler("Gts_Initial--清除各轴的报警和限位", Com_Return);
            //轴使能
            Com_Return = MC.GT_AxisOn(1);
            Com_Return = MC.GT_AxisOn(2);
        }
       
        public static void Free()
        {
            //关闭运动控制器
            Com_Return = MC.GT_Close();
            Log.Commandhandler("Gts_Initial---GT_Close", Com_Return);
        }
    }        


    //回原点
    class Axis_Home
    {
        //命令返回值
        public static short Gts_Return;       

        //定义回零运动标志，防止多次触发
        private static bool Homing_Flag;
        public static int Home(short Axis)
        {
            if (!Homing_Flag)
            {
                //反转回零标志
                Homing_Flag = !Homing_Flag;

                short Capture;//捕获状态值
                MC.TTrapPrm Home_TrapPrm = new MC.TTrapPrm();

                int Axis_Sta;//轴状态
                uint Axis_Pclock;//轴时钟
                Int32 Axis_Pos;//回零是触发Home开关时的轴位置
                double prfPos;//回零运动过程中规划位置
                //double encPos;//回零运动过程中编码器位置

                //清除指定轴报警和限位
                Gts_Return = MC.GT_ClrSts(Axis, 1);
                Log.Commandhandler("Axis_Home----GT_ClrSts", Gts_Return);

                //回零准备，向正方向前进20mm，后触发回零
                //切换到点动模式
                Gts_Return = MC.GT_PrfTrap(Axis);
                Log.Commandhandler("Axis_Home----GT_PrfTrap", Gts_Return);

                //读取点动模式运动参数
                Gts_Return = MC.GT_GetTrapPrm(Axis, out Home_TrapPrm);
                Log.Commandhandler("Axis_Home----GT_GetTrapPrm", Gts_Return);

                //设置点动模式运动参数
                Home_TrapPrm.acc = Convert.ToDouble(Para_List.Parameter.Home_acc / Para_List.Parameter.Gts_Acc_reference);
                Home_TrapPrm.dec = Convert.ToDouble(Para_List.Parameter.Home_dcc / Para_List.Parameter.Gts_Acc_reference);
                Home_TrapPrm.smoothTime = Para_List.Parameter.Home_smoothTime;

                //设置点动模式运动参数
                Gts_Return = MC.GT_SetTrapPrm(Axis, ref Home_TrapPrm);
                Log.Commandhandler("Axis_Home----GT_SetTrapPrm", Gts_Return);

                //设置点动模式目标速度，即回原点速度
                Gts_Return = MC.GT_SetVel(Axis, Convert.ToDouble(Para_List.Parameter.Home_High_Speed / Para_List.Parameter.Gts_Vel_reference));
                Log.Commandhandler("Axis_Home----GT_SetVel", Gts_Return);

                //设置点动模式目标位置，即原点准备距离 20mm
                Gts_Return = MC.GT_SetPos(Axis, Convert.ToInt32(20 * Para_List.Parameter.Gts_Pos_reference));
                Log.Commandhandler("Axis_Home----GT_SetPos", Gts_Return);

                //启动运动
                Gts_Return = MC.GT_Update(1 << (Axis - 1));
                Log.Commandhandler("Axis_Home----GT_Update", Gts_Return);

                do
                {
                    //读取轴状态
                    Gts_Return = MC.GT_GetSts(Axis, out Axis_Sta, 1, out Axis_Pclock);

                } while ((Axis_Sta & 0x400) != 0);

                //停止轴运动
                Gts_Return = MC.GT_Stop(1 << (Axis - 1), 0); //平滑停止轴运动
                Log.Commandhandler("Motion--停止轴运动", Gts_Return);

                //延时一段时间，等待电机稳定
                Common_Method.Delay_Time.Delay(200);//200ms

                //触发回零
                //启动Home捕捉
                Gts_Return = MC.GT_SetCaptureMode(Axis, MC.CAPTURE_HOME);
                Log.Commandhandler("Axis_Home----GT_SetCaptureMode", Gts_Return);

                //切换到点动模式
                Gts_Return = MC.GT_PrfTrap(Axis);
                Log.Commandhandler("Axis_Home----GT_PrfTrap", Gts_Return);

                //读取点动模式运动参数
                Gts_Return = MC.GT_GetTrapPrm(Axis, out Home_TrapPrm);
                Log.Commandhandler("Axis_Home----GT_GetTrapPrm", Gts_Return);

                //设置点动模式运动参数
                Home_TrapPrm.acc = Convert.ToDouble(Para_List.Parameter.Home_acc / Para_List.Parameter.Gts_Acc_reference);
                Home_TrapPrm.dec = Convert.ToDouble(Para_List.Parameter.Home_dcc / Para_List.Parameter.Gts_Acc_reference);
                Home_TrapPrm.smoothTime = Para_List.Parameter.Home_smoothTime;

                //设置点动模式运动参数
                Gts_Return = MC.GT_SetTrapPrm(Axis, ref Home_TrapPrm);
                Log.Commandhandler("Axis_Home----GT_SetTrapPrm", Gts_Return);

                //设置点动模式目标速度，即回原点速度
                Gts_Return = MC.GT_SetVel(Axis, Convert.ToDouble(Para_List.Parameter.Home_High_Speed / Para_List.Parameter.Gts_Vel_reference));
                Log.Commandhandler("Axis_Home----GT_SetVel", Gts_Return);

                //设置点动模式目标位置，即原点搜索距离
                Gts_Return = MC.GT_SetPos(Axis, Convert.ToInt32(Para_List.Parameter.Search_Home * Para_List.Parameter.Gts_Pos_reference));
                Log.Commandhandler("Axis_Home----GT_SetPos", Gts_Return);

                //启动运动
                Gts_Return = MC.GT_Update(1 << (Axis - 1));
                Log.Commandhandler("Axis_Home----GT_Update", Gts_Return);


                do
                {
                    //读取轴状态
                    Gts_Return = MC.GT_GetSts(Axis, out Axis_Sta, 1, out Axis_Pclock);
                    //读取捕获状态
                    Gts_Return = MC.GT_GetCaptureStatus(Axis, out Capture, out Axis_Pos, 1, out Axis_Pclock);
                    //读取编码器位置
                    //Gts_Return = MC.GT_GetEncPos(Axis, out encPos, 1, out Axis_Pclock);
                    //如果运动停止，返回出错信息
                    if (0 == (Axis_Sta & 0x400))
                    {
                        Log.Commandhandler("Axis_Home----No Home found!!", 1);
                        //反转回零标志
                        Homing_Flag = !Homing_Flag;
                        return 1;//整个过程Home信号一直没有触发，返回值为1                    
                    }
                } while (Capture == 0);

                /********************************待评估***********************************/
                /*
                //清除捕捉状态
                //Gts_Return = MC.GT_ClearCaptureStatus(Axis);
                //Log.Commandhandler("Axis_Home----清除捕捉状态", Gts_Return);

                //设置捕捉Home 下降沿
                //Gts_Return = MC.GT_SetCaptureSense(Axis, MC.CAPTURE_HOME, 0);
                //Log.Commandhandler("Axis_Home----设置捕捉Home 下降沿", Gts_Return);

                //设定目标位置为捕获位置+偏移量
                Gts_Return = MC.GT_SetPos(Axis, Axis_Pos + Home_OffSet);
                Log.Commandhandler("Axis_Home----GT_SetPos", Gts_Return);

                //在运动状态下更新目标位置
                Gts_Return = MC.GT_Update(1 << (Axis - 1));
                Log.Commandhandler("Axis_Home----GT_Update", Gts_Return);              

                do
                {
                    //读取轴状态
                    Gts_Return = MC.GT_GetSts(Axis, out Axis_Sta, 1, out Axis_Pclock);
                    //读取捕获状态
                    Gts_Return = MC.GT_GetCaptureStatus(Axis, out Capture, out Axis_Pos, 1, out Axis_Pclock);
                    //读取编码器位置
                    //Gts_Return = MC.GT_GetEncPos(Axis, out encPos, 1, out Axis_Pclock);
                    //如果运动停止，返回出错信息
                    if (0 == (Axis_Sta & 0x400))
                    {
                        Log.Commandhandler("Axis_Home----No Home found!!", 1);
                        //反转回零标志
                        Homing_Flag = !Homing_Flag;
                        return 1;//整个过程Home信号一直没有触发，返回值为1                    
                    }
                } while (Capture ==0);
                */
                /********************************待评估***********************************/

                //停止轴运动
                Gts_Return = MC.GT_Stop(1 << (Axis - 1), 0); //平滑停止轴运动
                Log.Commandhandler("Motion--停止轴运动", Gts_Return);

                //延时一段时间，等待电机稳定
                Common_Method.Delay_Time.Delay(200);//200ms
                //位置清零            
                Gts_Return = MC.GT_ZeroPos(Axis, 1);
                Log.Commandhandler("Axis_Home----GT_ZeroPos", Gts_Return);

                /***************************Home_Offset偏置距离 开始********************************************/

                //切换到点动模式
                Gts_Return = MC.GT_PrfTrap(Axis);
                Log.Commandhandler("Axis_Home----GT_PrfTrap", Gts_Return);

                //读取点动模式运动参数
                Gts_Return = MC.GT_GetTrapPrm(Axis, out Home_TrapPrm);
                Log.Commandhandler("Axis_Home----GT_GetTrapPrm", Gts_Return);

                //设置点动模式运动参数
                Home_TrapPrm.acc = Convert.ToDouble(Para_List.Parameter.Home_acc / Para_List.Parameter.Gts_Acc_reference);
                Home_TrapPrm.dec = Convert.ToDouble(Para_List.Parameter.Home_dcc / Para_List.Parameter.Gts_Acc_reference);
                Home_TrapPrm.smoothTime = Para_List.Parameter.Home_smoothTime;

                //设置点动模式运动参数
                Gts_Return = MC.GT_SetTrapPrm(Axis, ref Home_TrapPrm);
                Log.Commandhandler("Axis_Home----GT_SetTrapPrm", Gts_Return);

                //设置点动模式目标速度，即回原点速度
                Gts_Return = MC.GT_SetVel(Axis, Convert.ToDouble(Para_List.Parameter.Home_High_Speed / Para_List.Parameter.Gts_Vel_reference));
                Log.Commandhandler("Axis_Home----GT_SetVel", Gts_Return);

                //设置点动模式目标位置，即原点搜索距离
                Gts_Return = MC.GT_SetPos(Axis, Convert.ToInt32(Para_List.Parameter.Home_OffSet * Para_List.Parameter.Gts_Pos_reference));
                Log.Commandhandler("Axis_Home----GT_SetPos", Gts_Return);

                //启动运动
                Gts_Return = MC.GT_Update(1 << (Axis - 1));
                Log.Commandhandler("Axis_Home----GT_Update", Gts_Return);

                do
                {
                    //读取轴状态
                    Gts_Return = MC.GT_GetSts(Axis, out Axis_Sta, 1, out Axis_Pclock);
                    //读取规划位置
                    Gts_Return = MC.GT_GetPrfPos(Axis, out prfPos, 1, out Axis_Pclock);
                    //读取编码器位置
                    //Gts_Return = MC.GT_GetEncPos(Axis, out encPos, 1, out Axis_Pclock);

                } while ((Axis_Sta & 0x400) != 0);

                //检查是否到达 Home_OffSet
                if (prfPos != Convert.ToInt32(Para_List.Parameter.Home_OffSet * Para_List.Parameter.Gts_Pos_reference))
                {
                    Log.Commandhandler("Axis_Home----Move to Home_OffSet err!!", 1);
                    //反转回零标志
                    Homing_Flag = !Homing_Flag;

                    //置位Gts_Home标志
                    Refresh.Gts_Home_Flag = Refresh.Axis01_Limit_Up && Refresh.Axis01_Limit_Down && Refresh.Axis01_Alarm && Refresh.Axis01_MC_Err && Refresh.Axis01_IO_EMG && Refresh.Axis02_Limit_Up && Refresh.Axis02_Limit_Down && Refresh.Axis02_Alarm && Refresh.Axis02_MC_Err && Refresh.Axis02_IO_EMG && Refresh.EXI1 ;//任意（轴限位、报警、使能关闭、急停），致使原点标志丢失
                    return 2;
                }
                /***************************Home_Offset偏置距离 结束********************************************/
            }
            //反转回零标志
            Homing_Flag = !Homing_Flag;
            return 0;
        }
    }


    class Motion
    {
        public static short Gts_Return;//指令返回变量 

        //绝对定位
        public static void Abs(short Axis, decimal acc, decimal dcc, short smoothTime, decimal pos, decimal vel)
        {
            //定义点位运动参数变量
            MC.TTrapPrm trapPrm = new MC.TTrapPrm();
            //定义当前位置变量
            double prfpos;
            //定义时钟
            uint pclock;
            //定义轴状态
            int sts;
            //将轴设置为点位运动模式
            Gts_Return = MC.GT_PrfTrap(Axis);
            Log.Commandhandler("Motion--将轴设置为点位运动模式", Gts_Return);
            //读取点位运动运动参数
            Gts_Return = MC.GT_GetTrapPrm(Axis, out trapPrm);
            Log.Commandhandler("Motion--读取轴点位运动运动参数", Gts_Return);
            //设置要修改的参数
            trapPrm.acc = Convert.ToDouble(acc / Para_List.Parameter.Gts_Acc_reference);
            trapPrm.dec = Convert.ToDouble(dcc / Para_List.Parameter.Gts_Acc_reference);
            trapPrm.smoothTime = smoothTime;
            //设置点位运动参数
            Gts_Return = MC.GT_SetTrapPrm(Axis, ref trapPrm);
            Log.Commandhandler("Motion--读取轴设置点位运动参数", Gts_Return);

            //读取当前规划位置
            Gts_Return = MC.GT_GetPrfPos(Axis, out prfpos, 1, out pclock);
            Log.Commandhandler("Motion--读取轴当前规划位置", Gts_Return);

            //设置目标位置
            Gts_Return = MC.GT_SetPos(Axis, Convert.ToInt32(pos * Para_List.Parameter.Gts_Pos_reference));
            Log.Commandhandler("Motion--设置目标位置", Gts_Return);

            //设置目标速度
            Gts_Return = MC.GT_SetVel(Axis, Convert.ToDouble(vel / Para_List.Parameter.Gts_Vel_reference));
            Log.Commandhandler("Motion--设置目标速度", Gts_Return);

            //启动轴运动
            Gts_Return = MC.GT_Update(1 << (Axis - 1));
            Log.Commandhandler("Motion--启动轴运动", Gts_Return);

            do
            {
                //读取轴状态
                Gts_Return = MC.GT_GetSts(Axis, out sts, 1, out pclock);
                Log.Commandhandler("Motion--读取轴状态", Gts_Return);
            } while ((sts & 0x400) != 0);//等待Axis规划停止
        }
        //相对定位
        public static void Inc(short Axis, decimal acc, decimal dcc, short smoothTime, decimal pos, decimal vel)
        {
            //定义点位运动参数变量
            MC.TTrapPrm trapPrm = new MC.TTrapPrm();
            //定义当前位置变量
            double prfpos;
            //定义时钟
            uint pclock;
            //定义轴状态
            int sts;
            //将轴设置为点位运动模式
            Gts_Return = MC.GT_PrfTrap(Axis);
            Log.Commandhandler("Motion--将轴设置为点位运动模式", Gts_Return);
            //读取点位运动运动参数
            Gts_Return = MC.GT_GetTrapPrm(Axis, out trapPrm);
            Log.Commandhandler("Motion--读取轴点位运动运动参数", Gts_Return);
            //设置要修改的参数
            trapPrm.acc = Convert.ToDouble(acc / Para_List.Parameter.Gts_Acc_reference);
            trapPrm.dec = Convert.ToDouble(dcc / Para_List.Parameter.Gts_Acc_reference);
            trapPrm.smoothTime = smoothTime;
            //设置点位运动参数
            Gts_Return = MC.GT_SetTrapPrm(Axis, ref trapPrm);
            Log.Commandhandler("Motion--读取轴设置点位运动参数", Gts_Return);

            //读取当前规划位置
            Gts_Return = MC.GT_GetPrfPos(Axis, out prfpos, 1, out pclock);
            Log.Commandhandler("Motion--读取轴当前规划位置", Gts_Return);

            //设置目标位置
            Gts_Return = MC.GT_SetPos(Axis, Convert.ToInt32(Convert.ToDouble(pos * Para_List.Parameter.Gts_Pos_reference) + prfpos));
            Log.Commandhandler("Motion--设置目标位置", Gts_Return);

            //设置目标速度
            Gts_Return = MC.GT_SetVel(Axis, Convert.ToDouble(vel / Para_List.Parameter.Gts_Vel_reference));
            Log.Commandhandler("Motion--设置目标速度", Gts_Return);

            //启动轴运动
            Gts_Return = MC.GT_Update(1 << (Axis - 1));
            Log.Commandhandler("Motion--启动轴运动", Gts_Return);

            do
            {
                //读取轴状态
                Gts_Return = MC.GT_GetSts(Axis, out sts, 1, out pclock);
                Log.Commandhandler("Motion--读取轴状态", Gts_Return);
            } while ((sts & 0x400) != 0);//等待Axis规划停止


        }
        //Jog
        public static void Jog(short Axis, short dir, decimal JogVel, decimal JogAcc, decimal JogDcc)
        {
            //定义Jog运动参数变量
            MC.TJogPrm prfJog = new MC.TJogPrm();
            //将轴设置为Jog模式
            Gts_Return = MC.GT_PrfJog(Axis);
            Log.Commandhandler("Motion--将轴设置为Jog模式", Gts_Return);
            //读取轴jog运动参数
            Gts_Return = MC.GT_GetJogPrm(Axis, out prfJog);
            Log.Commandhandler("Motion--读取轴jog运动参数", Gts_Return);

            //设置要修改的参数
            prfJog.acc = Convert.ToDouble(JogAcc / Para_List.Parameter.Gts_Acc_reference);//加速度
            prfJog.dec = Convert.ToDouble(JogDcc / Para_List.Parameter.Gts_Acc_reference);//减速度

            //设置jog运动参数
            Gts_Return = MC.GT_SetJogPrm(Axis, ref prfJog);
            Log.Commandhandler("Motion--设置jog运动参数", Gts_Return);

            //设置轴Jog运行速度
            if (dir == 0) //Jog+
            {
                Gts_Return = MC.GT_SetVel(Axis, Convert.ToDouble(JogVel / Para_List.Parameter.Gts_Vel_reference));
                Log.Commandhandler("Motion--设置轴Jog运行速度", Gts_Return);
            }
            else    // Jog-
            {
                Gts_Return = MC.GT_SetVel(Axis, Convert.ToDouble(-JogVel / Para_List.Parameter.Gts_Vel_reference));
                Log.Commandhandler("Motion--设置轴Jog运行速度", Gts_Return);
            }

            //启动轴运动
            Gts_Return = MC.GT_Update(1 << (Axis - 1));
            Log.Commandhandler("Motion--启动轴运动", Gts_Return);
        }

        public static  void Smooth_Stop(short Axis)
        {
            //停止轴运动
            Gts_Return = MC.GT_Stop(1 << (Axis - 1), 0); //平滑停止轴运动
            Log.Commandhandler("Motion--停止轴运动", Gts_Return);
        }
        public static  void Emg_Stop(short Axis)
        {
            //停止轴运动
            Gts_Return = MC.GT_Stop(1 << (Axis - 1), 1 << (Axis - 1)); //紧急停止轴运动
            Log.Commandhandler("Motion--停止轴运动", Gts_Return);
        }
    }


    class Interpolation
    {
        public static short Gts_Return;//指令返回变量 
        public static short run;//插补运行状态
        private static int segment;//插补剩余个数
        private static int Remain_Segment;//插补剩余个数 
        private static MC.TCrdData[] crdData = new MC.TCrdData[4096];
        static IntPtr Crd_IntPtr = new IntPtr();
        public static double[] Crd_Pos = new double[2];//坐标系位置
        public static List<Affinity_Matrix> affinity_Matrices;//校准数据集合
        public static bool Exit_Flag = false;
        //构造函数
        public Interpolation()
        {
            string File_Path = @"./\Config/" + "Gts_Affinity_Matrix.xml";
            if (File.Exists(File_Path))
            {
                //获取标定板标定数据
                affinity_Matrices =new List<Affinity_Matrix>(Serialize_Data.Reserialize_Affinity_Matrix ("Gts_Affinity_Matrix.xml"));
                Log.Info("Gts仿射矫正文件加载成功！！！");
            }
            else
            {
                affinity_Matrices = new List<Affinity_Matrix>();
                MessageBox.Show("Gts仿射矫正文件不存在，禁止加工，请检查！");
                Log.Info("Gts仿射矫正文件不存在，禁止加工，请检查！");
            }
            
        }
        public static void Coordination(decimal X_original, decimal Y_original)
        {
            //结构体变量，用于定义坐标系 
            //初始化结构体变量
            MC.TCrdPrm crdPrm = new MC.TCrdPrm
            {
                dimension = 2,                        // 建立三维的坐标系
                synVelMax = Convert.ToDouble(Para_List.Parameter.Syn_MaxVel / Para_List.Parameter.Gts_Vel_reference),                      // 坐标系的最大合成速度是: 500 pulse/ms   （0-32767）/ms
                synAccMax = Convert.ToDouble(Para_List.Parameter.Syn_MaxAcc / Para_List.Parameter.Gts_Acc_reference),                        // 坐标系的最大合成加速度是: 2 pulse/ms^2  （0-32767）/ms
                evenTime = Convert.ToInt16(Para_List.Parameter.Syn_EvenTime),                         // 坐标系的最小匀速时间为0
                profile1 = 1,                       // 规划器1对应到X轴                       
                profile2 = 2,                       // 规划器2对应到Y轴
                profile3 = 0,
                profile4 = 0,
                profile5 = 0,
                profile6 = 0,
                profile7 = 0,
                profile8 = 0,
                setOriginFlag = 1,                    // 需要设置加工坐标系原点位置
                originPos1 = Convert.ToInt32(X_original * Para_List.Parameter.Gts_Pos_reference),                     // 加工坐标系原点位置在(0,0)，即与机床坐标系原点重合
                originPos2 = Convert.ToInt32(Y_original * Para_List.Parameter.Gts_Pos_reference),
                originPos3 = 0,
                originPos4 = 0,
                originPos5 = 0,
                originPos6 = 0,
                originPos7 = 0,
                originPos8 = 0
            };

            //停止轴规划运动，停止坐标系运动
            Gts_Return = MC.GT_Stop(783, 0);//783--1-4轴全停止，坐标系1、2均停止；0-平滑停止运动，783-急停运动
            Log.Commandhandler("Establish_Coordinationg--GT_Stop", Gts_Return);

            //建立坐标系
            Gts_Return = MC.GT_SetCrdPrm(1, ref crdPrm);
            Log.Commandhandler("Establish_Coordinationg--GT_SetCrdPrm", Gts_Return);
        }

        public static void Clear_FIFO()
        {            
            
            //首先清除坐标系1、FIFO0中的数据
            Gts_Return = MC.GT_CrdClear(1, 0);
            Log.Commandhandler("Line_Interpolation--清除坐标系1、FIFO0中的数据", Gts_Return);
        }
        public static void Line_FIFO(decimal x, decimal y)
        {
            //向缓存区写入一段插补数据.in
            Gts_Return = MC.GT_LnXY(
                1,//坐标系--1
                Convert.ToInt32(-x * Para_List.Parameter.Gts_Pos_reference),//插补X终点 [-1073741823,1073741823]
                Convert.ToInt32(-y * Para_List.Parameter.Gts_Pos_reference),//插补Y终点 [-1073741823,1073741823]
                Convert.ToDouble(Para_List.Parameter.Line_synVel / Para_List.Parameter.Gts_Vel_reference),//插补合成速度  [0-32767]
                Convert.ToDouble(Para_List.Parameter.Line_synAcc / Para_List.Parameter.Gts_Acc_reference),//插补合成加速度
                Convert.ToDouble(Para_List.Parameter.Line_endVel / Para_List.Parameter.Gts_Vel_reference),//插补终点速度
                0
                );
            Log.Commandhandler("Line_Interpolation--向缓存区写入一段直线插补数据", Gts_Return);
        }

        //圆心描述法 
        public static void Circle_C_FIFO(decimal x, decimal y, decimal Center_Start_x, decimal Center_Start_y, short dir)
        {
            //向缓存区写入一段插补数据
            Gts_Return = MC.GT_ArcXYC(
                1,//坐标系--1
                Convert.ToInt32(-x * Para_List.Parameter.Gts_Pos_reference), Convert.ToInt32(-y * Para_List.Parameter.Gts_Pos_reference),//插补圆弧终点坐标 [-1073741823,1073741823]
                Convert.ToDouble(-Center_Start_x * Para_List.Parameter.Gts_Pos_reference), Convert.ToDouble(-Center_Start_y * Para_List.Parameter.Gts_Pos_reference),//插补圆弧圆心相对于 （刀具加工点）起点位置的偏移量
                dir,//圆弧方向0-顺时针，1-逆时针
                Convert.ToDouble(Para_List.Parameter.Circle_synVel / Para_List.Parameter.Gts_Vel_reference),//插补合成速度  [0-32767]
                Convert.ToDouble(Para_List.Parameter.Circle_synAcc / Para_List.Parameter.Gts_Acc_reference),//插补合成加速度
                Convert.ToDouble(Para_List.Parameter.Circle_endVel / Para_List.Parameter.Gts_Vel_reference),//插补终点速度
                0
                );
            Log.Commandhandler("Line_Interpolation--向缓存区写入一段圆心插补数据", Gts_Return);
        }
        //圆弧描述法 不能用于描述整圆
        public static void Circle_R_FIFO(decimal x, decimal y, decimal radius, short dir)
        {
            //向缓存区写入一段插补数据
            Gts_Return = MC.GT_ArcXYR(
                1,//坐标系--1
                Convert.ToInt32(-x * Para_List.Parameter.Gts_Pos_reference), Convert.ToInt32(-y * Para_List.Parameter.Gts_Pos_reference),//插补圆弧终点坐标 [-1073741823,1073741823]
                Convert.ToDouble(radius * Para_List.Parameter.Gts_Pos_reference),//圆弧半径
                dir,//圆弧方向0-顺时针，1-逆时针
                Convert.ToDouble(Para_List.Parameter.Circle_synVel / Para_List.Parameter.Gts_Vel_reference),//插补合成速度  [0-32767]
                Convert.ToDouble(Para_List.Parameter.Circle_synAcc / Para_List.Parameter.Gts_Acc_reference),//插补合成加速度
                Convert.ToDouble(Para_List.Parameter.Circle_endVel / Para_List.Parameter.Gts_Vel_reference),//插补终点速度
                0
                );
            Log.Commandhandler("Line_Interpolation--向缓存区写入一段圆心插补数据", Gts_Return);
        }
        //转换为加工数据，添加进入FIFO      
        public static void Tran_Data(List<Interpolation_Data> Concat_Datas)
        {
            //清除FIFO 0
            Clear_FIFO();

            //初始化FIFO 0前瞻模块
            Gts_Return = MC.GT_InitLookAhead(1, 0, Convert.ToDouble(Para_List.Parameter.LookAhead_EvenTime), Convert.ToDouble(Para_List.Parameter.LookAhead_MaxAcc), 4096, ref crdData[0]);
            Log.Commandhandler("Line_Interpolation--初始化FIFO 0前瞻模块", Gts_Return);
            
            foreach (var o in Concat_Datas) 
            {               
                //未矫正数据

                if (o.Type == 1)//直线
                {
                    Line_FIFO(o.End_x, o.End_y);//将直线插补数据写入
                }
                else if (o.Type == 2)//圆弧
                {
                    Circle_R_FIFO(o.End_x, o.End_y, o.Circle_radius, o.Circle_dir);//将圆弧插补写入
                }
                else if (o.Type == 3)//圆形
                {
                    Circle_C_FIFO(o.End_x, o.End_y, o.Center_Start_x, o.Center_Start_y, o.Circle_dir);//将圆形插补写入
                }

            }

            //将前瞻数据压入控制器
            Gts_Return = MC.GT_CrdData(1, Crd_IntPtr, 0);
            Log.Commandhandler("Line_Interpolation--将前瞻数据压入控制器", Gts_Return);

        }

        //转换为加工数据，添加进入FIFO  启用校准   
        public static void Tran_Data_Correct (List<Interpolation_Data> Concat_Datas) 
        {
            //清除FIFO 0
            Clear_FIFO();

            //初始化FIFO 0前瞻模块
            Gts_Return = MC.GT_InitLookAhead(1, 0, Convert.ToDouble(Para_List.Parameter.LookAhead_EvenTime), Convert.ToDouble(Para_List.Parameter.LookAhead_MaxAcc), 4096, ref crdData[0]);
            Log.Commandhandler("Line_Interpolation--初始化FIFO 0前瞻模块", Gts_Return);

            //临时定位变量
            Int16 End_m, End_n, Center_m, Center_n;
            //定义处理的变量
            decimal Tmp_End_X;
            decimal Tmp_End_Y;
            decimal Tmp_Center_X;
            decimal Tmp_Center_Y;
            decimal Tmp_Center_Start_X;
            decimal Tmp_Center_Start_Y;
            foreach (var o in Concat_Datas)
            {

                //数据矫正
                //获取落点
                End_m = Gts_Cal_Data_Resolve.Seek_X_Pos(o.End_x);
                End_n = Gts_Cal_Data_Resolve.Seek_Y_Pos(o.End_y);
                Center_m = Gts_Cal_Data_Resolve.Seek_X_Pos(o.Center_x);
                Center_n = Gts_Cal_Data_Resolve.Seek_Y_Pos(o.Center_y);
                /*
                string sdatetime = DateTime.Now.ToString("D");
                string delimiter = ",";
                string strHeader = "";
                保存的位置和文件名称
                string File_Path = @"./\Config/" + "Gts_List.csv";
                strHeader += "原X坐标,原Y坐标,补偿后X坐标,补偿后Y坐标,补偿前后X差值,补偿前后Y差值";
                bool isExit = File.Exists(File_Path);
                StreamWriter sw = new StreamWriter(File_Path, true, Encoding.GetEncoding("gb2312"));
                if (!isExit)
                {
                    sw.WriteLine(strHeader);
                }
                */
                //计算最终数据
                //终点计算
                Tmp_End_X = o.End_x * affinity_Matrices[End_m * Para_List.Parameter.Gts_Affinity_Col + End_n].Cos_Value + o.End_y * affinity_Matrices[End_m * Para_List.Parameter.Gts_Affinity_Col + End_n].Sin_Value + affinity_Matrices[End_m * Para_List.Parameter.Gts_Affinity_Col + End_n].Delta_X;
                Tmp_End_Y = o.End_y * affinity_Matrices[End_m * Para_List.Parameter.Gts_Affinity_Col + End_n].Cos_Value - o.End_x * affinity_Matrices[End_m * Para_List.Parameter.Gts_Affinity_Col + End_n].Sin_Value + affinity_Matrices[End_n * Para_List.Parameter.Gts_Affinity_Col + End_n].Delta_Y;
                /*
                output rows data
                string strRowValue = "";
                strRowValue += o.End_x + delimiter
                                + o.End_y + delimiter
                                + Tmp_End_X + delimiter
                                + Tmp_End_Y + delimiter
                                + (Tmp_End_X - o.End_x) + delimiter
                                + (Tmp_End_Y - o.End_y) + delimiter;
                sw.WriteLine(strRowValue);
                */

                //圆心计算
                Tmp_Center_X = o.Center_x * affinity_Matrices[Center_m * Para_List.Parameter.Gts_Affinity_Col + Center_n].Cos_Value + o.Center_y * affinity_Matrices[Center_m * Para_List.Parameter.Gts_Affinity_Col + Center_n].Sin_Value + affinity_Matrices[Center_m * Para_List.Parameter.Gts_Affinity_Col + Center_n].Delta_X;
                Tmp_Center_Y = o.Center_y * affinity_Matrices[Center_m * Para_List.Parameter.Gts_Affinity_Col + Center_n].Cos_Value - o.Center_x * affinity_Matrices[Center_m * Para_List.Parameter.Gts_Affinity_Col + Center_n].Sin_Value + affinity_Matrices[Center_n * Para_List.Parameter.Gts_Affinity_Col + Center_n].Delta_Y;
                /*
                output rows data
                strRowValue = "";
                strRowValue += o.Center_x + delimiter
                                + o.Center_y + delimiter
                                + Tmp_Center_X + delimiter
                                + Tmp_Center_Y + delimiter
                                + (Tmp_Center_X - o.Center_x) + delimiter
                                + (Tmp_Center_Y - o.Center_y) + delimiter;
                sw.WriteLine(strRowValue);
                */
                //圆心与差值计算
                Tmp_Center_Start_X = Tmp_Center_X - Tmp_End_X;
                Tmp_Center_Start_Y = Tmp_Center_X - Tmp_End_Y;

                //sw.Close();


                //替换数据
                if (o.Type == 1)//直线
                {
                    Line_FIFO(Tmp_End_X, Tmp_End_Y);//将直线插补数据写入
                }
                else if (o.Type == 2)//圆弧
                {
                    Circle_R_FIFO(Tmp_End_X, Tmp_End_Y, o.Circle_radius, o.Circle_dir);//将圆弧插补写入
                }
                else if (o.Type == 3)//圆形
                {
                    Circle_C_FIFO(Tmp_End_X, Tmp_End_Y, Tmp_Center_Start_X, Tmp_Center_Start_Y, o.Circle_dir);//将圆形插补写入
                }
                
            }

            //将前瞻数据压入控制器
            Gts_Return = MC.GT_CrdData(1, Crd_IntPtr, 0);
            Log.Commandhandler("Line_Interpolation--将前瞻数据压入控制器", Gts_Return);

        }

        //获取当前点的坐标系坐标
        public static Vector Get_Coordinate()
        {
            Vector Result;
            double[] Curent_Pos=new double[2];
            MC.GT_GetCrdPos(1,out Curent_Pos[0]);
            Result = new Vector(-(decimal)Curent_Pos[0]/Para_List.Parameter.Gts_Pos_reference,-(decimal)Curent_Pos[1]/ Para_List.Parameter.Gts_Pos_reference);
            return Result;
        }
        public static void Interpolation_Start()
        {

            //缓存区延时指令
            Gts_Return = MC.GT_BufDelay(1, 2, 0);//2ms
            Log.Commandhandler("Line_Interpolation--缓存区延时指令", Gts_Return);

            //启动坐标系1、FIFO0插补运动
            Gts_Return = MC.GT_CrdStart(1, 0);
            Log.Commandhandler("Line_Interpolation--启动坐标系1、FIFO0插补运动", Gts_Return);
            do
            {
                //查询坐标系1、FIFO0插补运动状态
                Gts_Return = MC.GT_CrdStatus(
                    1,//坐标系1
                    out run,//插补运动状态
                    out segment,//当前已完成的插补段数
                    0
                    );

                //查询剩余插补段数
                Gts_Return = MC.GT_GetRemainderSegNum(
                    1,//坐标系1
                    out Remain_Segment,//剩余插补段数
                    0
                    );
                //获取坐标系位置
                Gts_Return = MC.GT_GetCrdPos(1, out Crd_Pos[0]);

                //退出
                if (Exit_Flag)
                {
                    Interpolation_Stop();//终止运动
                    Exit_Flag = false;
                    return;
                }

            } while (run == 1);
        }
        //停止轴运动
        public static void Interpolation_Stop()
        {
            //停止轴规划运动，停止坐标系运动
            Gts_Return = MC.GT_Stop(15, 0);//783-1-4轴全停止，坐标系1、2均停止,15-1-4轴全停止；0-平滑停止运动，783-急停运动
            Log.Commandhandler("Establish_Coordinationg--GT_Stop", Gts_Return);
        }
        //XY平台运动到配合振镜切割准备点
        public static void Gts_Ready(decimal x,decimal y)
        {          

            //启动定位
            Interpolation_Start();

            //无数据矫正
            //清除FIFO 0
            Clear_FIFO();
            //初始化FIFO 0前瞻模块
            Gts_Return = MC.GT_InitLookAhead(1, 0, Convert.ToDouble(Para_List.Parameter.LookAhead_EvenTime), Convert.ToDouble(Para_List.Parameter.LookAhead_MaxAcc), 4096, ref crdData[0]);
            Log.Commandhandler("Line_Interpolation--初始化FIFO 0前瞻模块", Gts_Return);

            //直线插补定位
            Line_FIFO(x, y);//将直线插补数据写入

            //将前瞻数据压入控制器
            Gts_Return = MC.GT_CrdData(1, Crd_IntPtr, 0);
            Log.Commandhandler("Line_Interpolation--将前瞻数据压入控制器", Gts_Return);
            //启动定位
            Interpolation_Start();
        }
        //XY平台运动到配合振镜切割准备点
        public static void Gts_Ready_Correct(decimal x, decimal y) 
        {
            
            //数据矫正
            //临时定位变量
            Int16 Tmp_m, Tmp_n;
            //获取落点
            Tmp_m = Gts_Cal_Data_Resolve.Seek_X_Pos(x);
            Tmp_n = Gts_Cal_Data_Resolve.Seek_Y_Pos(y);
            //定义处理的变量
            decimal Tmp_X;
            decimal Tmp_Y;
            //终点计算
            Tmp_X = x * affinity_Matrices[Tmp_m * Para_List.Parameter.Gts_Affinity_Col + Tmp_n].Cos_Value + y * affinity_Matrices[Tmp_m * Para_List.Parameter.Gts_Affinity_Col + Tmp_n].Sin_Value + affinity_Matrices[Tmp_m * Para_List.Parameter.Gts_Affinity_Col + Tmp_n].Delta_X;
            Tmp_Y = y * affinity_Matrices[Tmp_m * Para_List.Parameter.Gts_Affinity_Col + Tmp_n].Cos_Value - x * affinity_Matrices[Tmp_m * Para_List.Parameter.Gts_Affinity_Col + Tmp_n].Sin_Value + affinity_Matrices[Tmp_n * Para_List.Parameter.Gts_Affinity_Col + Tmp_n].Delta_Y;
            
            //清除FIFO 0
            Clear_FIFO();
            //初始化FIFO 0前瞻模块
            Gts_Return = MC.GT_InitLookAhead(1, 0, Convert.ToDouble(Para_List.Parameter.LookAhead_EvenTime), Convert.ToDouble(Para_List.Parameter.LookAhead_MaxAcc), 4096, ref crdData[0]);
            Log.Commandhandler("Line_Interpolation--初始化FIFO 0前瞻模块", Gts_Return);
            //直线插补定位
            Line_FIFO(Tmp_X, Tmp_Y);//将直线插补数据写入
            //将前瞻数据压入控制器
            Gts_Return = MC.GT_CrdData(1, Crd_IntPtr, 0);
            Log.Commandhandler("Line_Interpolation--将前瞻数据压入控制器", Gts_Return);          

            /*
            string sdatetime = DateTime.Now.ToString("D");
            string delimiter = ",";
            string strHeader = "";
            //保存的位置和文件名称
            string File_Path = @"./\Config/" + "Gts_Ready.csv";
            strHeader += "原X坐标,原Y坐标,补偿后X坐标,补偿后Y坐标,补偿前后X差值,补偿前后Y差值";
            bool isExit = File.Exists(File_Path);
            StreamWriter sw = new StreamWriter(File_Path, true, Encoding.GetEncoding("gb2312"));
            if (!isExit)
            {
                sw.WriteLine(strHeader);
            }
            //output rows data
            string strRowValue = "";
            strRowValue += x + delimiter
                            + y + delimiter
                            + Tmp_X + delimiter
                            + Tmp_Y + delimiter
                            + (Tmp_X - x) + delimiter
                            + (Tmp_Y - y) + delimiter;
            sw.WriteLine(strRowValue);
            sw.Close();
            */

        }
        //XY平台运动到指定点位
        public static void Gts_Ready(Vector Point)
        {
            //清除FIFO 0
            Clear_FIFO();
            //初始化FIFO 0前瞻模块
            Gts_Return = MC.GT_InitLookAhead(1, 0, Convert.ToDouble(Para_List.Parameter.LookAhead_EvenTime), Convert.ToDouble(Para_List.Parameter.LookAhead_MaxAcc), 4096, ref crdData[0]);
            Log.Commandhandler("Line_Interpolation--初始化FIFO 0前瞻模块", Gts_Return);
            //直线插补定位
            Line_FIFO(Point.X, Point.Y);//将直线插补数据写入
            //将前瞻数据压入控制器
            Gts_Return = MC.GT_CrdData(1, Crd_IntPtr, 0);
            Log.Commandhandler("Line_Interpolation--将前瞻数据压入控制器", Gts_Return); 
            //启动定位
            Interpolation_Start();            
        }
        //XY平台运动到指定点位
        public static void Gts_Ready_Correct(Vector Point) 
        {
            //数据矫正
            //临时定位变量
            Int16 Tmp_m, Tmp_n;
            //获取落点
            Tmp_m = Gts_Cal_Data_Resolve.Seek_X_Pos(Point.X);
            Tmp_n = Gts_Cal_Data_Resolve.Seek_Y_Pos(Point.Y);
            //定义处理的变量
            decimal Tmp_X;
            decimal Tmp_Y;
            //终点计算
            Tmp_X = Point.X * affinity_Matrices[Tmp_m * Para_List.Parameter.Gts_Affinity_Col + Tmp_n].Cos_Value + Point.Y * affinity_Matrices[Tmp_m * Para_List.Parameter.Gts_Affinity_Col + Tmp_n].Sin_Value + affinity_Matrices[Tmp_m * Para_List.Parameter.Gts_Affinity_Col + Tmp_n].Delta_X;
            Tmp_Y = Point.Y * affinity_Matrices[Tmp_m * Para_List.Parameter.Gts_Affinity_Col + Tmp_n].Cos_Value - Point.X * affinity_Matrices[Tmp_m * Para_List.Parameter.Gts_Affinity_Col + Tmp_n].Sin_Value + affinity_Matrices[Tmp_n * Para_List.Parameter.Gts_Affinity_Col + Tmp_n].Delta_Y;
            //清除FIFO 0
            Clear_FIFO();
            //初始化FIFO 0前瞻模块
            Gts_Return = MC.GT_InitLookAhead(1, 0, Convert.ToDouble(Para_List.Parameter.LookAhead_EvenTime), Convert.ToDouble(Para_List.Parameter.LookAhead_MaxAcc), 4096, ref crdData[0]);
            Log.Commandhandler("Line_Interpolation--初始化FIFO 0前瞻模块", Gts_Return);
            //直线插补定位
            Line_FIFO(Tmp_X, Tmp_Y);//将直线插补数据写入
            //将前瞻数据压入控制器
            Gts_Return = MC.GT_CrdData(1, Crd_IntPtr, 0);
            Log.Commandhandler("Line_Interpolation--将前瞻数据压入控制器", Gts_Return);

            //启动定位
            Interpolation_Start();

        }
        //Gts插补 整合Rtc振镜 数据，执行
        public static void Integrate(List<List<Interpolation_Data>> Gts_Datas, UInt16 No) 
        {            
            //追加数据
            Tran_Data(Gts_Datas[No]);
            //启动定位
            Interpolation_Start();
        }
        //Gts插补 特定数据，执行
        public static void Integrate(List<Interpolation_Data> Gts_Datas)
        {
            //追加数据
            Tran_Data(Gts_Datas);
            //启动定位
            Interpolation_Start();
        }
    }

}
