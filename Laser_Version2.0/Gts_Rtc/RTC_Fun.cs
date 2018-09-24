using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Laser_Build_1._0;
using Prompt;
using RTC5Import;

namespace RTC_Fun
{
    class Factory
    {
        //Rtc执行返回值
        static uint Rtc_Return;
        public static void Reset()
        {
            //初始化Dll
            Rtc_Return = RTC5Wrap.init_rtc5_dll();
            if (Rtc_Return != 0u && Rtc_Return != 2u)
            {
                Log.Commandhandler("振镜初始化出错", Rtc_Return);
            }
            //设置兼容RTC4
            //RTC5Wrap.set_rtc4_mode();
            RTC5Wrap.set_rtc5_mode();
            //停止正在执行的Rtc
            //  If the DefaultCard has been used previously by another application 
            //  a list might still be running. This would prevent load_program_file
            //  and load_correction_file from being executed.
            RTC5Wrap.stop_execution();

            //加载Correct文件
            Rtc_Return = RTC5Wrap.load_correction_file(
                "./config/Cor_1to1.ct5",
                1u,
                2u
                );
            if (Rtc_Return != 0u)
            {
                Log.Commandhandler("加载D2_1to1.ct5出错", Rtc_Return);
            }
            //加载Program_File文件
            Rtc_Return = RTC5Wrap.load_program_file(null);
            if (Rtc_Return != 0u)
            {
                Log.Commandhandler("加载Program_File出错", Rtc_Return);
            }

            //assigns the previously loaded correction tables #1 or #2 to the scan head control ports
            //and activates image field correction.
            //  table #1 at primary connector (default)
            RTC5Wrap.select_cor_table(1, 1);

            //stop_execution might have created an RTC5_TIMEOUT error
            //复位Rtc
            RTC5Wrap.reset_error(Para_List.Parameter.Reset_Completely);
            //defines the length of the FirstPulseKiller signal for a YAG laser
            RTC5Wrap.set_firstpulse_killer(Convert.ToUInt32(Para_List.Parameter.First_Pulse_Killer * Para_List.Parameter.Rtc_Period_Reference));

            //配置list大小
            //Configure list memory, default: config_list(4000,4000)
            RTC5Wrap.config_list(Para_List.Parameter.List1_Size, Para_List.Parameter.List2_Size);
            //配置激光模式
            RTC5Wrap.set_laser_mode(Para_List.Parameter.Laser_Mode);
            //配置激光控制
            //  This function must be called at least once to activate laser 
            //  signals. Later on enable/disable_laser would be sufficient.
            //Bit #0 (LSB) Pulse Switch Setting (doesn’t apply neither to laser mode 4 nor to laser mode 6):
            //    The setting only affects those laser control signals(more precisely: those LASER1 or LASER2
            //    “laser active” modulation pulses in CO2 mode or LASER1 Q - Switch pulses in the YAG modes) that are
            //    not yet fully processed at completion of the LASERON signal(see figure 46 and figure 47).
            //    = 0: The signals are cut off at the end of the LASERON signal.
            //    = 1: The final pulse will fully execute despite completion of the LASERON signal.
            //Bit #1 Phase shift of the laser control signals (doesn’t apply neither to laser mode 4 nor to laser mode 6)
            //    = 0: no phase shift
            //    = 1: CO2 mode: The LASER1 signal is exchanged with the LASER2 signal.
            //    YAG modes: The LASER1 is shifted back 180° (half a signal period).
            //Bit #2 Enabling or disabling of laser control signals for “Laser active” operation
            //    = 0: The “Laser active” laser control signals will be enabled.
            //    = 1: The “Laser active” laser control signals will be disabled.
            //Bit #3 LASERON signal level
            //    = 0: The signal at the LASERON port will be set to active - high.
            //    = 1: The signal at the LASERON port will be set to active - low.
            //Bit #4 LASER1/LASER2 signal level
            //    = 0: The signals at the LASER1 and LASER2 output ports will be set to active-high.
            //    = 1: The signals at the LASER1 and LASER2 output ports will be set to active-low.
            RTC5Wrap.set_laser_control(Para_List.Parameter.Laser_Control);//0x18 
            RTC5Wrap.set_firstpulse_killer(Convert.ToUInt32(Para_List.Parameter.First_Pulse_Killer * Para_List.Parameter.Rtc_Period_Reference));

            //activates the home jump mode (for the X and Y axes) and defines the home position
            RTC5Wrap.home_position(Convert.ToInt32(Para_List.Parameter.Rtc_Home.Y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(Para_List.Parameter.Rtc_Home.X * Para_List.Parameter.Rtc_XPos_Reference));

            // Turn on the optical pump source and wait for 2 seconds.
            // (The following assumes that signal ANALOG OUT1 of the
            // laser connector controls the pump source.)
            RTC5Wrap.write_da_x(Para_List.Parameter.Analog_Out_Ch, Para_List.Parameter.Analog_Out_Value);

            //定义待机 激光周期和脉冲长度
            //defines the output period and the pulse length of the standby pulses for “laser standby”
            //operation or – in laser mode 4 and laser mode 6 – the continuously-running laser signals
            //for “laser active” and “laser standby” operation.
            RTC5Wrap.set_standby(Convert.ToUInt32(Para_List.Parameter.Standby_Half_Period * Para_List.Parameter.Rtc_Period_Reference), Convert.ToUInt32(Para_List.Parameter.Standby_Pulse_Width * Para_List.Parameter.Rtc_Period_Reference));

            // Timing, delay and speed preset.
            // Transmit the following list commands to the list buffer.
            RTC5Wrap.set_start_list(1U);
            // Wait for Para_List.Parameter.Warmup_Time seconds
            RTC5Wrap.long_delay(Convert.ToUInt32(Para_List.Parameter.Warmup_Time / Para_List.Parameter.Scanner_Delay_Reference));
            RTC5Wrap.set_laser_pulses(
                Convert.ToUInt32(Para_List.Parameter.Laser_Half_Period * Para_List.Parameter.Rtc_Period_Reference),    // half of the laser signal period.
                Convert.ToUInt32(Para_List.Parameter.Laser_Pulse_Width * Para_List.Parameter.Rtc_Period_Reference));  // pulse widths of signal LASER1.
            RTC5Wrap.set_scanner_delays(
                Convert.ToUInt32(Para_List.Parameter.Jump_Delay / Para_List.Parameter.Scanner_Delay_Reference),    // jump delay, in 10 microseconds.
                Convert.ToUInt32(Para_List.Parameter.Mark_Delay / Para_List.Parameter.Scanner_Delay_Reference),    // mark delay, in 10 microseconds.
                Convert.ToUInt32(Para_List.Parameter.Polygon_Delay / Para_List.Parameter.Scanner_Delay_Reference));    // polygon delay, in 10 microseconds.
            RTC5Wrap.set_laser_delays(
                Convert.ToInt32(Para_List.Parameter.Laser_On_Delay * Para_List.Parameter.Laser_Delay_Reference),    // laser on delay, in microseconds.
                Convert.ToUInt32(Para_List.Parameter.Laser_Off_Delay * Para_List.Parameter.Laser_Delay_Reference));   // laser off delay, in microseconds.
            // jump speed in bits per milliseconds.
            RTC5Wrap.set_jump_speed(Para_List.Parameter.Jump_Speed);
            // marking speed in bits per milliseconds.
            RTC5Wrap.set_mark_speed(Para_List.Parameter.Mark_Speed);
            RTC5Wrap.set_end_of_list();
            RTC5Wrap.execute_list(1U);

            //Pump source warming up ,wait!!!
            uint Busy;
            do
            {
                RTC5Wrap.get_status(out Busy, out uint Position);
            } while (Busy != 0U);

        }
        public static void Free()  
        {
            //释放Rtc5_dll
            RTC5Wrap.free_rtc5_dll();
        }
    }

    class Motion 
    {
        public static List<Affinity_Matrix> affinity_Matrices;//校准数据集合
        public static bool Exit_Flag = false;
        //构造函数
        public Motion()
        {
            string File_Name = "";
            if (Para_List.Parameter.Rtc_Affinity_Type == 1)
            {
                File_Name = "Rtc_Affinity_Matrix_All.xml";
            }
            else
            {
                File_Name = "Rtc_Affinity_Matrix_Three.xml";
            }
            string File_Path = @"./\Config/" + File_Name;
            if (File.Exists(File_Path))
            {
                //获取标定板标定数据
                affinity_Matrices = new List<Affinity_Matrix>(Serialize_Data.Reserialize_Affinity_Matrix(File_Name));
            }
            else
            {
                affinity_Matrices = new List<Affinity_Matrix>();
                MessageBox.Show("Rtc仿射矫正文件不存在，禁止加工，请检查！");
                Log.Info("Rtc仿射矫正文件不存在，禁止加工，请检查！");
            }
            MessageBox.Show("Rtc校准数据 数量："+ affinity_Matrices.Count);

        }
        //关闭激光
        public static void Close_Laser()
        {

            RTC5Wrap.disable_laser();
            //强制低电平
            RTC5Wrap.set_laser_control(0);
        }
        //打开激光
        public static void Open_Laser()
        {
            RTC5Wrap.enable_laser();
            //强制高电平
            RTC5Wrap.set_laser_control(0x08);
        }
        //回到Home点
        public static void Home()
        {
            RTC5Wrap.goto_xy(-Convert.ToInt32(Para_List.Parameter.Rtc_Home.Y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(Para_List.Parameter.Rtc_Home.X * Para_List.Parameter.Rtc_XPos_Reference));
        }
        //关闭激光，移动激光聚焦点至加工起始位置
        public static void Rtc_Ready(decimal x, decimal y)
        {
            //goto 指定点
            RTC5Wrap.goto_xy(-Convert.ToInt32(y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(x * Para_List.Parameter.Rtc_XPos_Reference));
        }
        //x方向相对位移
        public static void Inc_X(decimal Distance, UInt32 Control, UInt32 List_No)//距离、控制方式、list区域        
        {
            //  wait list List_No to be not busy
            //  load_list( List_No, 0) returns 1 if successful, otherwise 0
            //  执行到POS 0
            do
            {

            }
            while (RTC5Wrap.load_list(List_No, 0u) == 0);
            // Transmit the following list commands to the list buffer.
            RTC5Wrap.set_start_list(List_No);

            //生成数据            
            if (Control == 4)//jump_rel
            {
                RTC5Wrap.jump_rel(0, Convert.ToInt32(Distance * Para_List.Parameter.Rtc_XPos_Reference));
            }
            else if (Control == 6)//mark_rel
            {
                RTC5Wrap.mark_rel(0, Convert.ToInt32(Distance * Para_List.Parameter.Rtc_XPos_Reference));
            }
            //设置List结束位置
            RTC5Wrap.set_end_of_list();

            //启动执行
            RTC5Wrap.execute_list(List_No);

            //Busy 运行等待结束
            uint Busy;
            do
            {
                RTC5Wrap.get_status(out Busy, out uint Position);
            } while (Busy != 0U);
        }
        //y方向相对位移
        public static void Inc_Y(decimal Distance, UInt32 Control, UInt32 List_No)//距离、控制方式、list区域        
        {
            //  wait list List_No to be not busy
            //  load_list( List_No, 0) returns 1 if successful, otherwise 0
            //  执行到POS 0
            do
            {

            }
            while (RTC5Wrap.load_list(List_No, 0u) == 0);
            // Transmit the following list commands to the list buffer.
            RTC5Wrap.set_start_list(List_No);

            //生成数据            
            if (Control == 4)//jump_rel
            {
                RTC5Wrap.jump_rel(-Convert.ToInt32(Distance * Para_List.Parameter.Rtc_YPos_Reference), 0);
            }
            else if (Control == 6)//mark_rel
            {
                RTC5Wrap.mark_rel(-Convert.ToInt32(Distance * Para_List.Parameter.Rtc_YPos_Reference), 0);
            }
            //设置List结束位置
            RTC5Wrap.set_end_of_list();

            //启动执行
            RTC5Wrap.execute_list(List_No);

            //Busy 运行等待结束
            uint Busy;
            do
            {
                RTC5Wrap.get_status(out Busy, out uint Position);
            } while (Busy != 0U);
        }
        //y方向绝对位移
        public static void Abs_XY(decimal x,decimal y, UInt32 Control, UInt32 List_No)//距离、控制方式、list区域         
        {
            //  wait list List_No to be not busy
            //  load_list( List_No, 0) returns 1 if successful, otherwise 0
            //  执行到POS 0
            do
            {

            }
            while (RTC5Wrap.load_list(List_No, 0u) == 0);
            // Transmit the following list commands to the list buffer.
            RTC5Wrap.set_start_list(List_No);

            //生成数据            
            if (Control == 4)//jump_rel
            {
                RTC5Wrap.jump_rel(-Convert.ToInt32(y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(x * Para_List.Parameter.Rtc_XPos_Reference));
            }
            else if (Control == 6)//mark_rel
            {
                RTC5Wrap.mark_rel(-Convert.ToInt32(y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(x * Para_List.Parameter.Rtc_XPos_Reference));
            }
            //设置List结束位置
            RTC5Wrap.set_end_of_list();

            //启动执行
            RTC5Wrap.execute_list(List_No);

            //Busy 运行等待结束
            uint Busy;
            do
            {
                RTC5Wrap.get_status(out Busy, out uint Position);
            } while (Busy != 0U);
        }
        //执行No指定的Rtc_Data封闭图形数据
        public static void Draw(List<List<Rtc_Data>> Rtc_Datas,UInt16 No,UInt32 List_No)
        {
            //  wait list List_No to be not busy
            //  load_list( List_No, 0) returns 1 if successful, otherwise 0
            //  执行到POS 0
            do
            {

            }
            while (RTC5Wrap.load_list(List_No, 0u) == 0);
            // Transmit the following list commands to the list buffer.
            RTC5Wrap.set_start_list(List_No);
           
            //生成数据
            foreach (var o in Rtc_Datas[No])
            {
                if (o.Type==1)//arc_abs 绝对圆弧
                {
                    RTC5Wrap.arc_abs(Convert.ToInt32(-o.Center_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.Center_x * Para_List.Parameter.Rtc_XPos_Reference), Convert.ToDouble(o.Angle));
                    //RTC5Wrap.set_ellipse(1,1, 0,Convert.ToDouble(o.Angle));
                    //RTC5Wrap.mark_ellipse_abs(Convert.ToInt32(-o.Center_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.Center_x * Para_List.Parameter.Rtc_XPos_Reference), 0);//圆心坐标，扁率:a-b/a
                }
                else if (o.Type == 2)//arc_rel
                {
                    RTC5Wrap.arc_rel(Convert.ToInt32(-o.Center_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.Center_x * Para_List.Parameter.Rtc_XPos_Reference), Convert.ToDouble(o.Angle));
                    //RTC5Wrap.set_ellipse(1, 1, 0, Convert.ToDouble(o.Angle));
                    //RTC5Wrap.mark_ellipse_rel(Convert.ToInt32(-o.Center_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.Center_x * Para_List.Parameter.Rtc_XPos_Reference), 0);//圆心坐标，扁率:a-b/a
                }
                else if (o.Type == 3)//jump_abs
                {
                    RTC5Wrap.jump_abs(Convert.ToInt32(-o.End_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.End_x * Para_List.Parameter.Rtc_XPos_Reference));
                }
                else if (o.Type == 4)//jump_rel
                {
                    RTC5Wrap.jump_rel(Convert.ToInt32(-o.End_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.End_x * Para_List.Parameter.Rtc_XPos_Reference));
                }
                else if (o.Type == 5)//mark_abs
                {
                    RTC5Wrap.mark_abs(Convert.ToInt32(-o.End_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.End_x * Para_List.Parameter.Rtc_XPos_Reference));
                }
                else if (o.Type == 6)//mark_rel
                {
                    RTC5Wrap.mark_rel(Convert.ToInt32(-o.End_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.End_x * Para_List.Parameter.Rtc_XPos_Reference));
                }
            }

            //设置List结束位置
            RTC5Wrap.set_end_of_list();

            //启动执行
            RTC5Wrap.execute_list(List_No);

            //Busy 运行等待结束
            uint Busy;
            do
            {
                RTC5Wrap.get_status(out Busy, out uint Position);
            } while (Busy != 0U);
        }
        //执行指定的Interpolation_Data 图形数据
        public static void Draw(List<Interpolation_Data> Rtc_Datas,UInt32 List_No)
        {
            //  wait list List_No to be not busy
            //  load_list( List_No, 0) returns 1 if successful, otherwise 0
            //  执行到POS 0
            do
            {

            }
            while (RTC5Wrap.load_list(List_No, 0u) == 0);
            // Transmit the following list commands to the list buffer.
            //RTC5Wrap.set_start_list(List_No);

            //初始Jump到启动点位
            RTC5Wrap.jump_abs(Convert.ToInt32(-Rtc_Datas[0].Rtc_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(Rtc_Datas[0].Rtc_x * Para_List.Parameter.Rtc_XPos_Reference));

            //生成数据
            foreach (var o in Rtc_Datas)
            {
                if (o.Type == 11)//arc_abs 绝对圆弧
                {
                    RTC5Wrap.arc_abs(Convert.ToInt32(-o.Center_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.Center_x * Para_List.Parameter.Rtc_XPos_Reference), Convert.ToDouble(o.Angle));
                }
                else if (o.Type == 12)//arc_rel
                {
                    RTC5Wrap.arc_rel(Convert.ToInt32(-o.Center_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.Center_x * Para_List.Parameter.Rtc_XPos_Reference), Convert.ToDouble(o.Angle));
                }
                else if (o.Type == 13)//jump_abs
                {
                    RTC5Wrap.jump_abs(Convert.ToInt32(-o.End_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.End_x * Para_List.Parameter.Rtc_XPos_Reference));
                }
                else if (o.Type == 14)//jump_rel
                {
                    RTC5Wrap.jump_rel(Convert.ToInt32(-o.End_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.End_x * Para_List.Parameter.Rtc_XPos_Reference));
                }
                else if (o.Type == 15)//mark_abs
                {
                    RTC5Wrap.mark_abs(Convert.ToInt32(-o.End_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.End_x * Para_List.Parameter.Rtc_XPos_Reference));
                }
                else if (o.Type == 16)//mark_rel
                {
                    RTC5Wrap.mark_rel(Convert.ToInt32(-o.End_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.End_x * Para_List.Parameter.Rtc_XPos_Reference));
                }
            }
            //结束Jump到启动点位
            RTC5Wrap.jump_abs(0,0);

            //设置List结束位置
            RTC5Wrap.set_end_of_list();

            //启动执行
            RTC5Wrap.execute_list(1u);

            //Busy 运行等待结束
            uint Busy;
            do
            {
                RTC5Wrap.get_status(out Busy, out uint Position);
                //退出
                if (Exit_Flag)
                {
                    Draw_Stop();
                    Exit_Flag = false;
                    return;
                }
            } while (Busy != 0U);
        }
        public static void Draw_Stop()
        {
            //终止运行
            RTC5Wrap.stop_execution();
        }
        //执行指定的Interpolation_Data 使用校准数据进行坐标校准 图形数据
        public static void Draw_Correct(List<Interpolation_Data> Rtc_Datas, UInt32 List_No) 
        {

            //临时定位变量
            Int16 R0_m, R0_n, End_m, End_n, Center_m, Center_n; 
            //定义处理的变量
            decimal Tmp_R0_X;
            decimal Tmp_R0_Y;
            decimal Tmp_End_X;
            decimal Tmp_End_Y;
            decimal Tmp_Center_X;
            decimal Tmp_Center_Y;
#if !DEBUG
            //  wait list List_No to be not busy
            //  load_list( List_No, 0) returns 1 if successful, otherwise 0
            //执行到POS 0
            do
            {

            }
            while (RTC5Wrap.load_list(List_No, 0u) == 0);
            // Transmit the following list commands to the list buffer.
            RTC5Wrap.set_start_list(List_No);
#endif

            //获取数据落点
            R0_m = Rtc_Cal_Data_Resolve.Seek_X_Pos(Rtc_Datas[0].Rtc_x);
            R0_n = Rtc_Cal_Data_Resolve.Seek_Y_Pos(Rtc_Datas[0].Rtc_y);

            //终点计算
            if (affinity_Matrices.Count>1)
            {
                Tmp_R0_X = Rtc_Datas[0].Rtc_x * affinity_Matrices[R0_n * Para_List.Parameter.Rtc_Affinity_Col + R0_m].Cos_Value + Rtc_Datas[0].Rtc_y * affinity_Matrices[R0_n * Para_List.Parameter.Rtc_Affinity_Col + R0_m].Sin_Value + affinity_Matrices[R0_n * Para_List.Parameter.Rtc_Affinity_Col + R0_m].Delta_X;
                Tmp_R0_Y = Rtc_Datas[0].Rtc_y * affinity_Matrices[R0_n * Para_List.Parameter.Rtc_Affinity_Col + R0_m].Cos_Value - Rtc_Datas[0].Rtc_x * affinity_Matrices[R0_n * Para_List.Parameter.Rtc_Affinity_Col + R0_m].Sin_Value + affinity_Matrices[R0_n * Para_List.Parameter.Rtc_Affinity_Col + R0_m].Delta_Y;
            }
            else if ((affinity_Matrices.Count > 0) && (affinity_Matrices.Count == 1))
            {
                Tmp_R0_X = Rtc_Datas[0].Rtc_x * affinity_Matrices[0].Cos_Value + Rtc_Datas[0].Rtc_y * affinity_Matrices[0].Sin_Value + affinity_Matrices[0].Delta_X;
                Tmp_R0_Y = Rtc_Datas[0].Rtc_y * affinity_Matrices[0].Cos_Value - Rtc_Datas[0].Rtc_x * affinity_Matrices[0].Sin_Value + affinity_Matrices[0].Delta_Y;
            }
            
#if !DEBUG
            //初始Jump到启动点位
            RTC5Wrap.jump_abs(Convert.ToInt32(-Tmp_R0_Y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(Tmp_R0_X * Para_List.Parameter.Rtc_XPos_Reference));
#endif
            //生成数据
            foreach (var o in Rtc_Datas)
            {

                //获取数据落点
                End_m = Rtc_Cal_Data_Resolve.Seek_X_Pos(o.End_x);                
                End_n = Rtc_Cal_Data_Resolve.Seek_Y_Pos(o.End_y);
                Center_m = Rtc_Cal_Data_Resolve.Seek_X_Pos(o.Center_x);
                Center_n = Rtc_Cal_Data_Resolve.Seek_Y_Pos(o.Center_y);

                //终点计算
                if (affinity_Matrices.Count > 1)
                {
                    Tmp_End_X = o.End_x * affinity_Matrices[End_n * Para_List.Parameter.Rtc_Affinity_Col + End_m].Cos_Value + o.End_y * affinity_Matrices[End_n * Para_List.Parameter.Rtc_Affinity_Col + End_m].Sin_Value + affinity_Matrices[End_n * Para_List.Parameter.Rtc_Affinity_Col + End_m].Delta_X;
                    Tmp_End_Y = o.End_y * affinity_Matrices[End_n * Para_List.Parameter.Rtc_Affinity_Col + End_m].Cos_Value - o.End_x * affinity_Matrices[End_n * Para_List.Parameter.Rtc_Affinity_Col + End_m].Sin_Value + affinity_Matrices[End_n * Para_List.Parameter.Rtc_Affinity_Col + End_m].Delta_Y;
                    Tmp_Center_X = o.Center_x * affinity_Matrices[Center_n * Para_List.Parameter.Rtc_Affinity_Col + Center_m].Cos_Value + o.Center_y * affinity_Matrices[Center_n * Para_List.Parameter.Rtc_Affinity_Col + Center_m].Sin_Value + affinity_Matrices[Center_n * Para_List.Parameter.Rtc_Affinity_Col + Center_m].Delta_X;
                    Tmp_Center_Y = o.Center_y * affinity_Matrices[Center_n * Para_List.Parameter.Rtc_Affinity_Col + Center_m].Cos_Value - o.Center_x * affinity_Matrices[Center_n * Para_List.Parameter.Rtc_Affinity_Col + Center_m].Sin_Value + affinity_Matrices[Center_n * Para_List.Parameter.Rtc_Affinity_Col + Center_m].Delta_Y;
                }
                else if ((affinity_Matrices.Count > 0) && (affinity_Matrices.Count == 1))
                {
                    Tmp_End_X = o.End_x * affinity_Matrices[0].Cos_Value + o.End_y * affinity_Matrices[0].Sin_Value + affinity_Matrices[0].Delta_X;
                    Tmp_End_Y = o.End_y * affinity_Matrices[0].Cos_Value - o.End_x * affinity_Matrices[0].Sin_Value + affinity_Matrices[0].Delta_Y;
                    Tmp_Center_X = o.Center_x * affinity_Matrices[0].Cos_Value + o.Center_y * affinity_Matrices[0].Sin_Value + affinity_Matrices[0].Delta_X;
                    Tmp_Center_Y = o.Center_y * affinity_Matrices[0].Cos_Value - o.Center_x * affinity_Matrices[0].Sin_Value + affinity_Matrices[0].Delta_Y;
                }
                
#if !DEBUG
                if (o.Type == 11)//arc_abs 绝对圆弧
                {
                    RTC5Wrap.arc_abs(Convert.ToInt32(-Tmp_Center_Y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(Tmp_Center_X * Para_List.Parameter.Rtc_XPos_Reference), Convert.ToDouble(o.Angle));
                }
                else if (o.Type == 12)//arc_rel
                {
                    RTC5Wrap.arc_rel(Convert.ToInt32(-Tmp_Center_Y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(Tmp_Center_X * Para_List.Parameter.Rtc_XPos_Reference), Convert.ToDouble(o.Angle));
                }
                else if (o.Type == 13)//jump_abs
                {
                    RTC5Wrap.jump_abs(Convert.ToInt32(-Tmp_End_Y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(Tmp_End_X * Para_List.Parameter.Rtc_XPos_Reference));
                }
                else if (o.Type == 14)//jump_rel
                {
                    RTC5Wrap.jump_rel(Convert.ToInt32(-Tmp_End_Y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(Tmp_End_X * Para_List.Parameter.Rtc_XPos_Reference));
                }
                else if (o.Type == 15)//mark_abs
                {
                    RTC5Wrap.mark_abs(Convert.ToInt32(-Tmp_End_Y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(Tmp_End_X * Para_List.Parameter.Rtc_XPos_Reference));
                }
                else if (o.Type == 16)//mark_rel
                {
                    RTC5Wrap.mark_rel(Convert.ToInt32(-Tmp_End_Y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(Tmp_End_X * Para_List.Parameter.Rtc_XPos_Reference));
                }
#endif
            }
#if !DEBUG
            //结束Jump到启动点位
            RTC5Wrap.jump_abs(0, 0);

            //设置List结束位置
            RTC5Wrap.set_end_of_list();

            //启动执行
            RTC5Wrap.execute_list(1u);

            //Busy 运行等待结束
            uint Busy;
            do
            {
                RTC5Wrap.get_status(out Busy, out uint Position);
                //退出
                if (Exit_Flag)
                {
                    Draw_Stop();
                    Exit_Flag = false;
                    return;
                }
            } while (Busy != 0U);
#endif
        }
        //执行list Interpolation_Data 数据
        public static void Draw(List<List<Interpolation_Data>> Rtc_Datas, UInt32 List_No)
        {
            //  wait list List_No to be not busy
            //  load_list( List_No, 0) returns 1 if successful, otherwise 0
            //  执行到POS 0
            do
            {

            }
            while (RTC5Wrap.load_list(List_No, 0u) == 0);
            // Transmit the following list commands to the list buffer.
            RTC5Wrap.set_start_list(List_No);

            //生成数据
            foreach (var n in Rtc_Datas)
            {
                foreach (var o in n)
                {
                    //初始Jump到启动点位
                    for (int i = 0; i < n.Count; i++)
                    {
                        if (i == 0)
                        {
                            RTC5Wrap.jump_abs(Convert.ToInt32(-n[i].Rtc_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(n[i].Rtc_x * Para_List.Parameter.Rtc_XPos_Reference));
                        }
                    } 
                    if (o.Type == 11)//arc_abs 绝对圆弧
                    {
                        RTC5Wrap.arc_abs(Convert.ToInt32(-o.Center_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.Center_x * Para_List.Parameter.Rtc_XPos_Reference), Convert.ToDouble(o.Angle));
                    }
                    else if (o.Type == 12)//arc_rel
                    {
                        RTC5Wrap.arc_rel(Convert.ToInt32(-o.Center_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.Center_x * Para_List.Parameter.Rtc_XPos_Reference), Convert.ToDouble(o.Angle));
                    }
                    else if (o.Type == 13)//jump_abs
                    {
                        RTC5Wrap.jump_abs(Convert.ToInt32(-o.End_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.End_x * Para_List.Parameter.Rtc_XPos_Reference));
                    }
                    else if (o.Type == 14)//jump_rel
                    {
                        RTC5Wrap.jump_rel(Convert.ToInt32(-o.End_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.End_x * Para_List.Parameter.Rtc_XPos_Reference));
                    }
                    else if (o.Type == 15)//mark_abs
                    {
                        RTC5Wrap.mark_abs(Convert.ToInt32(-o.End_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.End_x * Para_List.Parameter.Rtc_XPos_Reference));
                    }
                    else if (o.Type == 16)//mark_rel
                    {
                        RTC5Wrap.mark_rel(Convert.ToInt32(-o.End_y * Para_List.Parameter.Rtc_YPos_Reference), Convert.ToInt32(o.End_x * Para_List.Parameter.Rtc_XPos_Reference));
                    }
                }
            }            

            //设置List结束位置
            RTC5Wrap.set_end_of_list();

            //启动执行
            RTC5Wrap.execute_list(List_No);

            //Busy 运行等待结束
            uint Busy;
            do
            {
                RTC5Wrap.get_status(out Busy, out uint Position);
            } while (Busy != 0U);
        }
    }
}
