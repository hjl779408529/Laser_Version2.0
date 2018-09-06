using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Para_List
{
    public class Parameter
    {

        //GTS 相关定位参数
        //电子齿轮相关的参数 1pluse--10um
        private static decimal gts_vel_reference = 10m;//速度参数转换基准，脉冲数转换为mm
        private static decimal gts_acc_reference = gts_vel_reference * 1000m;//加速度参数转换基准 mm/s2
        private static decimal gts_pos_reference = 100m;//位置参数转换基准，脉冲数转换为mm
                                                    //运行参数
        private static decimal manual_Vel = 100m;//手动速度 mm/s
        private static decimal auto_Vel = 200m;//自动速度 mm/s
        private static decimal acc = 500m;//加速度 mm/s2
        private static decimal dcc = 500m;//减速度 mm/s2
        private static decimal syn_MaxVel = 1200;//最大合成速度 mm/s
        private static decimal syn_MaxAcc = 10000;//最大合成加速度 mm/s2 1G=10米/秒
        private static decimal syn_EvenTime = 10;//合成平滑时间 ms
        private static decimal line_synVel = 100m;//直线插补速度 mm/s
        private static decimal line_synAcc = 500m;//直线插补加速度 mm/s2
        private static decimal line_endVel = line_synVel / 2;//直线插补结束停止速度 mm/s
        private static decimal circle_synVel = 100m;//圆弧插补速度 mm/s
        private static decimal circle_synAcc = 500m;//圆弧插补加速度  mm/s2
        private static decimal circle_endVel = circle_synVel / 2;//圆弧插补结束停止速度 mm/s 
        private static decimal lookAhead_EvenTime = 1;//前瞻运动平滑时间  ms
        private static decimal lookAhead_MaxAcc = syn_MaxAcc / 100;//前瞻运动最大合成加速度 mm/s2 1G=10米/秒

        //回零参数
        private static decimal search_Home = -2000;//设定原点搜索范围 搜索范围-2000mm
        private static decimal home_OffSet = 0;//设定原点OFF偏移距离
        private static decimal home_High_Speed = 50m;//设定原点回归速度，脉冲/ms,即200HZ mm/s
        private static decimal home_acc = 500m;//回零加速度 mm/s2
        private static decimal home_dcc = 500m;//回零减速度 mm/s2
        private static short home_smoothTime = 5;//回零平滑时间 ms        

        private static decimal pos_Tolerance = 0.02m;//坐标误差范围判断
        private static decimal arc_Compensation_A = 0.0m;//坐标误差范围判断  度
        private static decimal arc_Compensation_R = Convert.ToDecimal(Math.PI) * (arc_Compensation_A / 180.0m);//坐标误差范围判断  弧度

        //坐标矫正变换参数
        private static decimal calibration_x_len = 350.0m;//标定板X尺寸 mm
        private static decimal calibration_y_len = 350.0m;//标定板Y尺寸 mm
        private static decimal calibration_cell = 2.5m;//标定板尺寸 mm
        private static Int16 calibration_row = Convert.ToInt16((calibration_x_len / calibration_cell) + 1), calibration_col = Convert.ToInt16((calibration_y_len / calibration_cell) + 1);//标定板的点位calibration横纵数
        private static Int16 affinity_row = Convert.ToInt16(calibration_x_len / calibration_cell), affinity_col = Convert.ToInt16(calibration_y_len / calibration_cell);//仿射变换的横纵数 


        //加工坐标系
        private static decimal work_x = 450.0m;//加工坐标系X坐标
        private static decimal work_y = 300.0m;//加工坐标系Y坐标 
        //刀具绝对坐标 激光光束相对于整个平台绝对坐标
        private static decimal laser_x = 0.0m;//刀具绝对坐标X坐标
        private static decimal laser_y = 0.0m;//刀具绝对坐标Y坐标
        //相机单像素对应的实际比例
        private static decimal cam_reference = 0.0m;
        //相机中心与坐标系原点对正时，像素偏差值，每次开机时矫正（待确定）
        private static decimal cam_org_pixel_x = 0.0m;
        private static decimal cam_org_pixel_y = 0.0m;
        //Camera中心 相对于 直角坐标系原点 相对间距
        private static decimal cam_org_x = 0.0m;
        private static decimal cam_org_y = 0.0m;
        //振镜激光原点  相对于 直角坐标系原点 相对间距
        private static decimal rtc_org_x = 0.0m; 
        private static decimal rtc_org_y = 0.0m;
        //Camera中心 相对于 振镜激光原点   相对间距
        private static decimal cam_rtc_x = 0.0m;
        private static decimal cam_rtc_y = 0.0m;
        //偏差补偿值  Dxf中图形 与实际工件摆放的 偏差值，每次加工前进行处理调用获取该值
        private static decimal delta_x = 0.0m;
        private static decimal delta_y = 0.0m;
        //定义图像中心点
        private static decimal pic_center_x = 0.0m;
        private static decimal pic_center_y = 0.0m;

        //振镜参数
        private static UInt32 reset_completely = UInt32.MaxValue; //复位范围
        private static UInt32 default_card = 2; //初始PCI卡号
        private static UInt32 laser_mode = 6; //激光类型 0-CO2/1-YAG MODE1/2-YAG MODE2/3-YAG MODE3/4-LASER MODE4/6-LASER MODE6
        private static UInt32 laser_control = 0x00; //激光控制 Laser signals LOW active  (Bit 3 and 4) 0x18 高电平 

        //基准
        private static decimal rtc_period_reference = 64; //分频基准 RTC4-1/8us RTC5-1/64us 用于set_standby( HalfPeriod, PulseLength )、set_laser_pulses( HalfPeriod, PulseLength )、set_firstpulse_killer( Length )
        private static decimal laser_delay_reference = 1/2; //延时基准 RTC4-1us RTC5-1/2us 用于set_laser_delays( LaserOnDelay, LaserOffDelay )
        private static decimal scanner_delay_reference = 10; //延时基准 10us  用于set_scanner_delays( Jump, Mark, Polygon )，eg.warmup_time/jump_delay/mark_delay/polygon_delay/arc_delay/line_delay
        //运动基准
        private static decimal rtc_xpos_reference = 10000; //x轴 mm与振镜变量转换比例 
        private static decimal rtc_ypos_reference = 10000; //y轴 mm与振镜变量转换比例
        //RTC通用参数
        private static UInt32 analog_out_ch = 1;//  输出通道 
        private static UInt32 analog_out_value = 640;//  Standard Pump Source Value
        private static UInt32 analog_out_standby = 0;//  Standby Pump Source Value

        //以rtc_period_reference为基准 *
        private static UInt32 standby_half_period = 100;//  100 us [1/8 us] 
        private static UInt32 standby_pulse_width = 1;//  1 us [1/8 us] 
        private static UInt32 laser_half_period = 100;//  100 us [1/8 us]
        private static UInt32 laser_pulse_width = 100;//  100 us [1/8 us]
        private static UInt32 first_pulse_killer = 100;//  100 us [1/8 us]
        //以laser_delay_reference为基准 *
        private static Int32 laser_on_delay = 100;//  100 us [1 us]
        private static UInt32 laser_off_delay = 100;//  100 us [1 us]
        //以scanner_delay_reference为基准 /
        private static UInt32 warmup_time = 2000000;//  2s [10 us]
        private static UInt32 jump_delay = 250;//  250 us [10 us]
        private static UInt32 mark_delay = 100;//  100 us [10 us]
        private static UInt32 polygon_delay = 50;//  50 us [10 us]
        private static UInt32 arc_delay = 50;//  50 us [10 us]
        private static UInt32 line_delay = 50;//  50 us [10 us]

        private static double mark_speed = 250.0;//  [16 Bits/ms]
        private static double jump_speed = 1000.0;//  [16 Bits/ms] 
        private static UInt32 list1_size = 4000;//  list1 容量
        private static UInt32 list2_size = 4000;//  list2 容量
        //定义Home_XY
        private static Vector rtc_home = new Vector(100, 100);

        //定义串口号
        private static Int32 com_no = 0;


        public static decimal Gts_Vel_reference { get => gts_vel_reference; set => gts_vel_reference = value; }
        public static decimal Gts_Acc_reference { get => gts_acc_reference; set => gts_acc_reference = value; }
        public static decimal Gts_Pos_reference { get => gts_pos_reference; set => gts_pos_reference = value; } 
        public static decimal Manual_Vel { get => manual_Vel; set => manual_Vel = value; }
        public static decimal Auto_Vel { get => auto_Vel; set => auto_Vel = value; }
        public static decimal Acc { get => acc; set => acc = value; }
        public static decimal Dcc { get => dcc; set => dcc = value; }
        public static decimal Syn_MaxVel { get => syn_MaxVel; set => syn_MaxVel = value; }
        public static decimal Syn_MaxAcc { get => syn_MaxAcc; set => syn_MaxAcc = value; }
        public static decimal Syn_EvenTime { get => syn_EvenTime; set => syn_EvenTime = value; }
        public static decimal Line_synVel { get => line_synVel; set => line_synVel = value; }
        public static decimal Line_synAcc { get => line_synAcc; set => line_synAcc = value; }
        public static decimal Line_endVel { get => line_endVel; set => line_endVel = value; }
        public static decimal Circle_synVel { get => circle_synVel; set => circle_synVel = value; }
        public static decimal Circle_synAcc { get => circle_synAcc; set => circle_synAcc = value; }
        public static decimal Circle_endVel { get => circle_endVel; set => circle_endVel = value; }
        public static decimal LookAhead_EvenTime { get => lookAhead_EvenTime; set => lookAhead_EvenTime = value; }
        public static decimal LookAhead_MaxAcc { get => lookAhead_MaxAcc; set => lookAhead_MaxAcc = value; }
        public static decimal Search_Home { get => search_Home; set => search_Home = value; }
        public static decimal Home_OffSet { get => home_OffSet; set => home_OffSet = value; }
        public static decimal Home_High_Speed { get => home_High_Speed; set => home_High_Speed = value; }
        public static decimal Home_acc { get => home_acc; set => home_acc = value; }
        public static decimal Home_dcc { get => home_dcc; set => home_dcc = value; }
        public static short Home_smoothTime { get => home_smoothTime; set => home_smoothTime = value; }
        public static decimal Pos_Tolerance { get => pos_Tolerance; set => pos_Tolerance = value; }
        public static decimal Arc_Compensation_A { get => arc_Compensation_A; set => arc_Compensation_A = value; }
        public static decimal Arc_Compensation_R { get => arc_Compensation_R; set => arc_Compensation_R = value; }
        public static decimal Calibration_X_Len { get => calibration_x_len; set => calibration_x_len = value; }
        public static decimal Calibration_Y_Len { get => calibration_y_len; set => calibration_y_len = value; }
        public static decimal Calibration_Cell { get => calibration_cell; set => calibration_cell = value; }
        public static Int16 Calibration_Col { get => calibration_col; set => calibration_col = value; }
        public static Int16 Calibration_Row { get => calibration_row; set => calibration_row = value; }
        public static Int16 Affinity_Col { get => affinity_col; set => affinity_col = value; }
        public static Int16 Affinity_Row { get => affinity_row; set => affinity_row = value; }
        public static decimal Work_X { get => work_x; set => work_x = value; }
        public static decimal Work_Y { get => work_y; set => work_y = value; }
        public static decimal Laser_X { get => laser_x; set => laser_x = value; }
        public static decimal Laser_Y { get => laser_y; set => laser_y = value; }
        public static decimal Cam_Reference { get => cam_reference; set => cam_reference = value; }
        public static decimal Cam_Org_Pixel_X { get => cam_org_pixel_x; set => cam_org_pixel_x = value; }
        public static decimal Cam_Org_Pixel_Y { get => cam_org_pixel_y; set => cam_org_pixel_y = value; }        
        public static decimal Cam_Org_X { get => cam_org_x; set => cam_org_x = value; }
        public static decimal Cam_Org_Y { get => cam_org_y; set => cam_org_y = value; }
        public static decimal Rtc_Org_X { get => rtc_org_x; set => rtc_org_x = value; }
        public static decimal Rtc_Org_Y { get => rtc_org_y; set => rtc_org_y = value; }
        public static decimal Cam_Rtc_X { get => cam_rtc_x; set => cam_rtc_x = value; }
        public static decimal Cam_Rtc_Y { get => cam_rtc_y; set => cam_rtc_y = value; }
        public static decimal Delta_X { get => delta_x; set => delta_x = value; }
        public static decimal Delta_Y { get => delta_y; set => delta_y = value; }
        public static decimal Pic_Center_X { get => pic_center_x; set => pic_center_x = value; }
        public static decimal Pic_Center_Y { get => pic_center_y; set => pic_center_y = value; }
        public static UInt32 Reset_Completely { get => reset_completely; set => reset_completely = value; }
        public static UInt32 Default_Card { get => default_card; set => default_card = value; }
        public static UInt32 Laser_Mode { get => laser_mode; set => laser_mode = value; }
        public static UInt32 Laser_Control { get => laser_control; set => laser_control = value; }
        public static decimal Rtc_Period_Reference { get => rtc_period_reference; set => rtc_period_reference = value; }
        public static decimal Laser_Delay_Reference { get => laser_delay_reference; set => laser_delay_reference = value; }
        public static decimal Scanner_Delay_Reference { get => scanner_delay_reference; set => scanner_delay_reference = value; }
        public static decimal Rtc_XPos_Reference { get => rtc_xpos_reference; set => rtc_xpos_reference = value; }
        public static decimal Rtc_YPos_Reference { get => rtc_ypos_reference; set => rtc_ypos_reference = value; }
        public static UInt32 Analog_Out_Ch { get => analog_out_ch; set => analog_out_ch = value; }
        public static UInt32 Analog_Out_Value { get => analog_out_value; set => analog_out_value = value; }
        public static UInt32 Analog_Out_Standby { get => analog_out_standby; set => analog_out_standby = value; }
        public static UInt32 Standby_Half_Period { get => standby_half_period; set => standby_half_period = value; }
        public static UInt32 Standby_Pulse_Width { get => standby_pulse_width; set => standby_pulse_width = value; }
        public static UInt32 Laser_Half_Period { get => laser_half_period; set => laser_half_period = value; }
        public static UInt32 Laser_Pulse_Width { get => laser_pulse_width; set => laser_pulse_width = value; }
        public static UInt32 First_Pulse_Killer { get => first_pulse_killer; set => first_pulse_killer = value; }
        public static Int32 Laser_On_Delay { get => laser_on_delay; set => laser_on_delay = value; }
        public static UInt32 Laser_Off_Delay { get => laser_off_delay; set => laser_off_delay = value; }
        public static UInt32 Warmup_Time { get => warmup_time; set => warmup_time = value; }
        public static UInt32 Jump_Delay { get => jump_delay; set => jump_delay = value; }
        public static UInt32 Mark_Delay { get => mark_delay; set => mark_delay = value; }
        public static UInt32 Polygon_Delay { get => polygon_delay; set => polygon_delay = value; }
        public static UInt32 Arc_Delay { get => arc_delay; set => arc_delay = value; }
        public static UInt32 Line_Delay { get => line_delay; set => line_delay = value; }
        public static double Mark_Speed { get => mark_speed; set => mark_speed = value; }
        public static double Jump_Speed { get => jump_speed; set => jump_speed = value; }
        public static UInt32 List1_Size { get => list1_size; set => list1_size = value; }
        public static UInt32 List2_Size { get => list2_size; set => list2_size = value; }
        public static Vector Rtc_Home { get => rtc_home; set => rtc_home = value; }
        public static Int32 Com_No { get => com_no; set => com_no = value; }
        //公开构造函数
        public Parameter() { }
    }


    [Serializable]
    public class Parameter_RW
    {

        //GTS 相关定位参数
        //电子齿轮相关的参数 1pluse--10um
        private decimal gts_vel_reference;//速度参数转换基准，脉冲数转换为mm
        private decimal gts_acc_reference;//加速度参数转换基准 mm/s2
        private decimal gts_pos_reference;//位置参数转换基准，脉冲数转换为mm
        //运行参数
        private decimal manual_Vel;//手动速度 mm/s
        private decimal auto_Vel;//自动速度 mm/s
        private decimal acc;//加速度 mm/s2
        private decimal dcc;//减速度 mm/s2
        private decimal syn_MaxVel;//最大合成速度 mm/s
        private decimal syn_MaxAcc;//最大合成加速度 mm/s2 1G=10米/秒
        private decimal syn_EvenTime;//合成平滑时间 ms
        private decimal line_synVel;//直线插补速度 mm/s
        private decimal line_synAcc;//直线插补加速度 mm/s2
        private decimal line_endVel;//直线插补结束停止速度 mm/s
        private decimal circle_synVel;//圆弧插补速度 mm/s
        private decimal circle_synAcc;//圆弧插补加速度  mm/s2
        private decimal circle_endVel;//圆弧插补结束停止速度 mm/s 
        private decimal lookAhead_EvenTime;//前瞻运动平滑时间  ms
        private decimal lookAhead_MaxAcc;//前瞻运动最大合成加速度 mm/s2 1G=10米/秒

        //回零参数
        private decimal search_Home;//设定原点搜索范围 1脉冲10um，搜索范围-2000mm
        private decimal home_OffSet;//设定原点OFF偏移距离
        private decimal home_High_Speed;//设定原点回归速度，脉冲/ms,即200HZ mm/s
        private decimal home_acc;//回零加速度 mm/s2
        private decimal home_dcc;//回零减速度 mm/s2
        private short home_smoothTime;//回零平滑时间 ms

        private decimal pos_Tolerance;//坐标误差范围判断
        private decimal arc_Compensation_A;//坐标误差范围判断  度
        private decimal arc_Compensation_R;//坐标误差范围判断  弧度

        //坐标矫正变换参数
        private decimal calibration_x_len;//标定板X尺寸 mm
        private decimal calibration_y_len;//标定板Y尺寸 mm
        private decimal calibration_cell;//标定板间隔尺寸 mm 
        private Int16 calibration_row, calibration_col;//标定板的点位calibration横纵数 row-行-x  col-列-y
        private Int16 affinity_row, affinity_col;//仿射变换的横纵数 row-行-x  col-列-y
        

        //加工坐标系
        private decimal work_x;//加工坐标系X坐标
        private decimal work_y;//加工坐标系Y坐标
        //刀具绝对坐标 激光光束相对于整个平台绝对坐标 读取振镜的XY坐标，进行换算，得到laser_x,laser_y
        private decimal laser_x;//刀具绝对坐标X坐标
        private decimal laser_y;//刀具绝对坐标Y坐标 
        //相机单像素对应的实际比例
        private decimal cam_reference = 0.0m;
        //相机中心与坐标系原点对正时，像素偏差值，每次开机时矫正（待确定）
        private decimal cam_org_pixel_x = 0.0m;
        private decimal cam_org_pixel_y = 0.0m;
        //Camera中心 相对于 直角坐标系原点 相对间距
        private decimal cam_org_x;
        private decimal cam_org_y;
        //振镜激光原点  相对于 直角坐标系原点 相对间距
        private decimal rtc_org_x;
        private decimal rtc_org_y;
        //Camera中心 相对于 振镜激光原点   相对间距
        private decimal cam_rtc_x;
        private decimal cam_rtc_y;
        //偏差补偿值  Dxf中图形 与实际工件摆放的 偏差值，每次加工前进行处理调用获取该值
        private decimal delta_x;
        private decimal delta_y;
        //定义图像中心点
        private decimal pic_center_x;
        private decimal pic_center_y;

        //振镜参数
        private UInt32 reset_completely; //复位范围
        private UInt32 default_card; //初始PCI卡号
        private UInt32 laser_mode; //激光类型
        private UInt32 laser_control; //激光控制 

        private decimal rtc_period_reference; //分频基准 1/8us
        private decimal laser_delay_reference; //延时基准 1us
        private decimal scanner_delay_reference; //延时基准 1us
        private decimal rtc_xpos_reference; //x轴 mm与振镜变量转换比例 
        private decimal rtc_ypos_reference; //y轴 mm与振镜变量转换比例 
        //RTC通用参数
        private UInt32 analog_out_ch;//  输出通道 
        private UInt32 analog_out_value;//  Standard Pump Source Value
        private UInt32 analog_out_standby;//  Standby Pump Source Value
        private UInt32 standby_half_period;
        private UInt32 standby_pulse_width;
        private UInt32 laser_half_period;
        private UInt32 laser_pulse_width;
        private UInt32 first_pulse_killer;
        private Int32 laser_on_delay;
        private UInt32 laser_off_delay;
        private UInt32 warmup_time;
        private UInt32 jump_delay;
        private UInt32 mark_delay;
        private UInt32 polygon_delay;
        private UInt32 arc_delay;
        private UInt32 line_delay;
        private double mark_speed;
        private double jump_speed;
        private UInt32 list1_size;//  list1 容量
        private UInt32 list2_size;//  list2 容量
        //定义Home_XY
        private Vector rtc_home;
        //定义串口号
        private Int32 com_no = 0;
        public decimal Gts_Vel_reference { get => gts_vel_reference; set => gts_vel_reference = value; }
        public decimal Gts_Acc_reference { get => gts_acc_reference; set => gts_acc_reference = value; }
        public decimal Gts_Pos_reference { get => gts_pos_reference; set => gts_pos_reference = value; }
        public decimal Manual_Vel { get => manual_Vel; set => manual_Vel = value; }
        public decimal Auto_Vel { get => auto_Vel; set => auto_Vel = value; }
        public decimal Acc { get => acc; set => acc = value; }
        public decimal Dcc { get => dcc; set => dcc = value; }
        public decimal Syn_MaxVel { get => syn_MaxVel; set => syn_MaxVel = value; }
        public decimal Syn_MaxAcc { get => syn_MaxAcc; set => syn_MaxAcc = value; }
        public decimal Syn_EvenTime { get => syn_EvenTime; set => syn_EvenTime = value; }
        public decimal Line_synVel { get => line_synVel; set => line_synVel = value; }
        public decimal Line_synAcc { get => line_synAcc; set => line_synAcc = value; }
        public decimal Line_endVel { get => line_endVel; set => line_endVel = value; }
        public decimal Circle_synVel { get => circle_synVel; set => circle_synVel = value; }
        public decimal Circle_synAcc { get => circle_synAcc; set => circle_synAcc = value; }
        public decimal Circle_endVel { get => circle_endVel; set => circle_endVel = value; }
        public decimal LookAhead_EvenTime { get => lookAhead_EvenTime; set => lookAhead_EvenTime = value; }
        public decimal LookAhead_MaxAcc { get => lookAhead_MaxAcc; set => lookAhead_MaxAcc = value; }
        public decimal Search_Home { get => search_Home; set => search_Home = value; }
        public decimal Home_OffSet { get => home_OffSet; set => home_OffSet = value; }
        public decimal Home_High_Speed { get => home_High_Speed; set => home_High_Speed = value; }
        public decimal Home_acc { get => home_acc; set => home_acc = value; }
        public decimal Home_dcc { get => home_dcc; set => home_dcc = value; }
        public short Home_smoothTime { get => home_smoothTime; set => home_smoothTime = value; }
        public decimal Pos_Tolerance { get => pos_Tolerance; set => pos_Tolerance = value; }
        public decimal Arc_Compensation_A { get => arc_Compensation_A; set => arc_Compensation_A = value; }
        public decimal Arc_Compensation_R { get => arc_Compensation_R; set => arc_Compensation_R = value; }
        public decimal Calibration_X_Len { get => calibration_x_len; set => calibration_x_len = value; }
        public decimal Calibration_Y_Len { get => calibration_y_len; set => calibration_y_len = value; }
        public decimal Calibration_Cell { get => calibration_cell; set => calibration_cell = value; }
        public Int16 Calibration_Col { get => calibration_col; set => calibration_col = value; }
        public Int16 Calibration_Row { get => calibration_row; set => calibration_row = value; }
        public Int16 Affinity_Col { get => affinity_col; set => affinity_col = value; }
        public Int16 Affinity_Row { get => affinity_row; set => affinity_row = value; }
        public decimal Work_X { get => work_x; set => work_x = value; }
        public decimal Work_Y { get => work_y; set => work_y = value; }
        public decimal Laser_X { get => laser_x; set => laser_x = value; }
        public decimal Laser_Y { get => laser_y; set => laser_y = value; }
        public decimal Cam_Reference { get => cam_reference; set => cam_reference = value; }
        public decimal Cam_Org_Pixel_X { get => cam_org_pixel_x; set => cam_org_pixel_x = value; }
        public decimal Cam_Org_Pixel_Y { get => cam_org_pixel_y; set => cam_org_pixel_y = value; }
        public decimal Cam_Org_X { get => cam_org_x; set => cam_org_x = value; }
        public decimal Cam_Org_Y { get => cam_org_y; set => cam_org_y = value; }
        public decimal Rtc_Org_X { get => rtc_org_x; set => rtc_org_x = value; }
        public decimal Rtc_Org_Y { get => rtc_org_y; set => rtc_org_y = value; }
        public decimal Cam_Rtc_X { get => cam_rtc_x; set => cam_rtc_x = value; }
        public decimal Cam_Rtc_Y { get => cam_rtc_y; set => cam_rtc_y = value; }
        public decimal Delta_X { get => delta_x; set => delta_x = value; }
        public decimal Delta_Y { get => delta_y; set => delta_y = value; }
        public decimal Pic_Center_X { get => pic_center_x; set => pic_center_x = value; }
        public decimal Pic_Center_Y { get => pic_center_y; set => pic_center_y = value; }
        public UInt32 Reset_Completely { get => reset_completely; set => reset_completely = value; }
        public UInt32 Default_Card { get => default_card; set => default_card = value; }
        public UInt32 Laser_Mode { get => laser_mode; set => laser_mode = value; }
        public UInt32 Laser_Control { get => laser_control; set => laser_control = value; }
        public decimal Rtc_Period_Reference { get => rtc_period_reference; set => rtc_period_reference = value; }
        public decimal Laser_Delay_Reference { get => laser_delay_reference; set => laser_delay_reference = value; }
        public decimal Scanner_Delay_Reference { get => scanner_delay_reference; set => scanner_delay_reference = value; }
        public decimal Rtc_XPos_Reference { get => rtc_xpos_reference; set => rtc_xpos_reference = value; }
        public decimal Rtc_YPos_Reference { get => rtc_ypos_reference; set => rtc_ypos_reference = value; }
        public UInt32 Analog_Out_Ch { get => analog_out_ch; set => analog_out_ch = value; }
        public UInt32 Analog_Out_Value { get => analog_out_value; set => analog_out_value = value; }
        public UInt32 Analog_Out_Standby { get => analog_out_standby; set => analog_out_standby = value; }
        public UInt32 Standby_Half_Period { get => standby_half_period; set => standby_half_period = value; }
        public UInt32 Standby_Pulse_Width { get => standby_pulse_width; set => standby_pulse_width = value; }
        public UInt32 Laser_Half_Period { get => laser_half_period; set => laser_half_period = value; }
        public UInt32 Laser_Pulse_Width { get => laser_pulse_width; set => laser_pulse_width = value; }
        public UInt32 First_Pulse_Killer { get => first_pulse_killer; set => first_pulse_killer = value; }
        public Int32 Laser_On_Delay { get => laser_on_delay; set => laser_on_delay = value; }
        public UInt32 Laser_Off_Delay { get => laser_off_delay; set => laser_off_delay = value; }
        public UInt32 Warmup_Time { get => warmup_time; set => warmup_time = value; }
        public UInt32 Jump_Delay { get => jump_delay; set => jump_delay = value; }
        public UInt32 Mark_Delay { get => mark_delay; set => mark_delay = value; }
        public UInt32 Polygon_Delay { get => polygon_delay; set => polygon_delay = value; }
        public UInt32 Arc_Delay { get => arc_delay; set => arc_delay = value; }
        public UInt32 Line_Delay { get => line_delay; set => line_delay = value; }
        public double Mark_Speed { get => mark_speed; set => mark_speed = value; }
        public double Jump_Speed { get => jump_speed; set => jump_speed = value; }
        public UInt32 List1_Size { get => list1_size; set => list1_size = value; }
        public UInt32 List2_Size { get => list2_size; set => list2_size = value; }
        public Vector Rtc_Home { get => rtc_home; set => rtc_home = value; }
        public Int32 Com_No { get => com_no; set => com_no = value; }
        //构造函数
        public Parameter_RW() { }
    }
    //参数变量序列化
    public class Serialize_Parameter
    {

        //参数保存
        public void Serialize(string txtFile)
        {
            //中转当前参数
            Parameter_RW parameter = new Parameter_RW
            {
                //当前参数写入临时变量
                Gts_Vel_reference = Para_List.Parameter.Gts_Vel_reference,
                Gts_Acc_reference = Para_List.Parameter.Gts_Vel_reference * 1000m,
                Gts_Pos_reference = Para_List.Parameter.Gts_Pos_reference,
                Manual_Vel = Para_List.Parameter.Manual_Vel,
                Auto_Vel = Para_List.Parameter.Auto_Vel,
                Acc = Para_List.Parameter.Acc,
                Dcc = Para_List.Parameter.Dcc,
                Syn_MaxVel = Para_List.Parameter.Syn_MaxVel,
                Syn_MaxAcc = Para_List.Parameter.Syn_MaxAcc,
                Syn_EvenTime = Para_List.Parameter.Syn_EvenTime,
                Line_synVel = Para_List.Parameter.Line_synVel,
                Line_synAcc = Para_List.Parameter.Line_synAcc,
                Line_endVel = Para_List.Parameter.Line_endVel,
                Circle_synVel = Para_List.Parameter.Circle_synVel,
                Circle_synAcc = Para_List.Parameter.Circle_synAcc,
                Circle_endVel = Para_List.Parameter.Circle_endVel,
                LookAhead_EvenTime = Para_List.Parameter.LookAhead_EvenTime,
                LookAhead_MaxAcc = Para_List.Parameter.LookAhead_MaxAcc,
                Search_Home = Para_List.Parameter.Search_Home,
                Home_OffSet = Para_List.Parameter.Home_OffSet,
                Home_High_Speed = Para_List.Parameter.Home_High_Speed,
                Home_acc = Para_List.Parameter.Home_acc,
                Home_dcc = Para_List.Parameter.Home_dcc,
                Home_smoothTime = Para_List.Parameter.Home_smoothTime,
                Pos_Tolerance = Para_List.Parameter.Pos_Tolerance,
                Arc_Compensation_A = Para_List.Parameter.Arc_Compensation_A,
                Arc_Compensation_R = Para_List.Parameter.Arc_Compensation_R,
                Calibration_X_Len = Para_List.Parameter.Calibration_X_Len,
                Calibration_Y_Len = Para_List.Parameter.Calibration_Y_Len,
                Calibration_Cell = Para_List.Parameter.Calibration_Cell,
                Calibration_Col = Para_List.Parameter.Calibration_Col,
                Calibration_Row = Para_List.Parameter.Calibration_Row,
                Affinity_Col = Para_List.Parameter.Affinity_Col,
                Affinity_Row = Para_List.Parameter.Affinity_Row,
                Work_X = Para_List.Parameter.Work_X,
                Work_Y = Para_List.Parameter.Work_Y,
                Laser_X = Para_List.Parameter.Laser_X,
                Laser_Y = Para_List.Parameter.Laser_Y,
                Cam_Reference = Para_List.Parameter.Cam_Reference,
                Cam_Org_Pixel_X = Para_List.Parameter.Cam_Org_Pixel_X,
                Cam_Org_Pixel_Y = Para_List.Parameter.Cam_Org_Pixel_Y,
                Cam_Org_X = Para_List.Parameter.Cam_Org_X,
                Cam_Org_Y = Para_List.Parameter.Cam_Org_Y,
                Rtc_Org_X = Para_List.Parameter.Rtc_Org_X,
                Rtc_Org_Y = Para_List.Parameter.Rtc_Org_Y,
                Cam_Rtc_X = Para_List.Parameter.Cam_Rtc_X,
                Cam_Rtc_Y = Para_List.Parameter.Cam_Rtc_Y,
                Delta_X = Para_List.Parameter.Delta_X,
                Delta_Y = Para_List.Parameter.Delta_Y,
                Pic_Center_X = Para_List.Parameter.Pic_Center_X,
                Pic_Center_Y = Para_List.Parameter.Pic_Center_Y,
                Reset_Completely = Para_List.Parameter.Reset_Completely,
                Default_Card = Para_List.Parameter.Default_Card,
                Laser_Mode = Para_List.Parameter.Laser_Mode,
                Laser_Control = Para_List.Parameter.Laser_Control,
                Rtc_Period_Reference = Para_List.Parameter.Rtc_Period_Reference,
                Laser_Delay_Reference = Para_List.Parameter.Laser_Delay_Reference,
                Scanner_Delay_Reference = Para_List.Parameter.Scanner_Delay_Reference,
                Rtc_XPos_Reference = Para_List.Parameter.Rtc_XPos_Reference,
                Rtc_YPos_Reference = Para_List.Parameter.Rtc_YPos_Reference,
                Analog_Out_Ch = Para_List.Parameter.Analog_Out_Ch,
                Analog_Out_Value = Para_List.Parameter.Analog_Out_Value,
                Analog_Out_Standby = Para_List.Parameter.Analog_Out_Standby,
                Standby_Half_Period = Para_List.Parameter.Standby_Half_Period,
                Standby_Pulse_Width = Para_List.Parameter.Standby_Pulse_Width,
                Laser_Half_Period = Para_List.Parameter.Laser_Half_Period,
                Laser_Pulse_Width = Para_List.Parameter.Laser_Pulse_Width,
                First_Pulse_Killer = Para_List.Parameter.First_Pulse_Killer,
                Laser_On_Delay = Para_List.Parameter.Laser_On_Delay,
                Laser_Off_Delay = Para_List.Parameter.Laser_Off_Delay,
                Warmup_Time = Para_List.Parameter.Warmup_Time,
                Jump_Delay = Para_List.Parameter.Jump_Delay,
                Mark_Delay = Para_List.Parameter.Mark_Delay,
                Polygon_Delay = Para_List.Parameter.Polygon_Delay,
                Arc_Delay = Para_List.Parameter.Arc_Delay,
                Line_Delay = Para_List.Parameter.Line_Delay,
                Mark_Speed = Para_List.Parameter.Mark_Speed,
                Jump_Speed = Para_List.Parameter.Jump_Speed,
                List1_Size = Para_List.Parameter.List1_Size,
                List2_Size = Para_List.Parameter.List2_Size,
                Rtc_Home = Para_List.Parameter.Rtc_Home,
                Com_No = Para_List.Parameter.Com_No
            };

            //二进制 序列化
            /*
            string File_Path = @"./\Config/" + txtFile;
            using (FileStream fs = new FileStream(File_Path, FileMode.Create, FileAccess.ReadWrite))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, parameter);
            }
            */
            //xml 序列化
            string File_Path = @"./\Config/" + txtFile;
            using (FileStream fs = new FileStream(File_Path, FileMode.Create, FileAccess.ReadWrite))
            {
                XmlSerializer bf = new XmlSerializer(typeof(Parameter_RW));
                bf.Serialize(fs, parameter);
            }

        }

        //反序列化
        public void Reserialize(string fileName)
        {
            //读取文件
            string File_Path = @"./\Config/" + fileName;
            if (File.Exists(File_Path))
            {
                using (FileStream fs = new FileStream(File_Path, FileMode.Open, FileAccess.Read))
                {
                    //二进制 反序列化
                    //BinaryFormatter bf = new BinaryFormatter();
                    //xml 反序列化
                    XmlSerializer bf = new XmlSerializer(typeof(Parameter_RW));
                    Parameter_RW parameter = (Parameter_RW)bf.Deserialize(fs);
                    //参数写入当前设备参数
                    Para_List.Parameter.Gts_Vel_reference = parameter.Gts_Vel_reference;
                    Para_List.Parameter.Gts_Acc_reference = parameter.Gts_Vel_reference * 1000m;
                    Para_List.Parameter.Gts_Pos_reference = parameter.Gts_Pos_reference;
                    Para_List.Parameter.Manual_Vel = parameter.Manual_Vel;
                    Para_List.Parameter.Auto_Vel = parameter.Auto_Vel;
                    Para_List.Parameter.Acc = parameter.Acc;
                    Para_List.Parameter.Dcc = parameter.Dcc;
                    Para_List.Parameter.Syn_MaxVel = parameter.Syn_MaxVel;
                    Para_List.Parameter.Syn_MaxAcc = parameter.Syn_MaxAcc;
                    Para_List.Parameter.Syn_EvenTime = parameter.Syn_EvenTime;
                    Para_List.Parameter.Line_synVel = parameter.Line_synVel;
                    Para_List.Parameter.Line_synAcc = parameter.Line_synAcc;
                    Para_List.Parameter.Line_endVel = parameter.Line_endVel;
                    Para_List.Parameter.Circle_synVel = parameter.Circle_synVel;
                    Para_List.Parameter.Circle_synAcc = parameter.Circle_synAcc;
                    Para_List.Parameter.Circle_endVel = parameter.Circle_endVel;
                    Para_List.Parameter.LookAhead_EvenTime = parameter.LookAhead_EvenTime;
                    Para_List.Parameter.LookAhead_MaxAcc = parameter.LookAhead_MaxAcc;
                    Para_List.Parameter.Search_Home = parameter.Search_Home;
                    Para_List.Parameter.Home_OffSet = parameter.Home_OffSet;
                    Para_List.Parameter.Home_High_Speed = parameter.Home_High_Speed;
                    Para_List.Parameter.Home_acc = parameter.Home_acc;
                    Para_List.Parameter.Home_dcc = parameter.Home_dcc;
                    Para_List.Parameter.Home_smoothTime = parameter.Home_smoothTime;
                    Para_List.Parameter.Pos_Tolerance = parameter.Pos_Tolerance;
                    Para_List.Parameter.Arc_Compensation_A = parameter.Arc_Compensation_A;
                    Para_List.Parameter.Arc_Compensation_R = parameter.Arc_Compensation_R;
                    Para_List.Parameter.Calibration_X_Len = parameter.Calibration_X_Len;
                    Para_List.Parameter.Calibration_Y_Len = parameter.Calibration_Y_Len;
                    Para_List.Parameter.Calibration_Cell = parameter.Calibration_Cell;
                    Para_List.Parameter.Calibration_Col = parameter.Calibration_Col;
                    Para_List.Parameter.Calibration_Row = parameter.Calibration_Row;
                    Para_List.Parameter.Affinity_Col = parameter.Affinity_Col;
                    Para_List.Parameter.Affinity_Row = parameter.Affinity_Row;
                    Para_List.Parameter.Work_X = parameter.Work_X;
                    Para_List.Parameter.Work_Y = parameter.Work_Y;
                    Para_List.Parameter.Laser_X = parameter.Laser_X;
                    Para_List.Parameter.Laser_Y = parameter.Laser_Y;
                    Para_List.Parameter.Cam_Reference = parameter.Cam_Reference;
                    Para_List.Parameter.Cam_Org_Pixel_X = parameter.Cam_Org_Pixel_X;
                    Para_List.Parameter.Cam_Org_Pixel_Y = parameter.Cam_Org_Pixel_Y;
                    Para_List.Parameter.Cam_Org_X = parameter.Cam_Org_X;
                    Para_List.Parameter.Cam_Org_Y = parameter.Cam_Org_Y;
                    Para_List.Parameter.Rtc_Org_X = parameter.Rtc_Org_X;
                    Para_List.Parameter.Rtc_Org_Y = parameter.Rtc_Org_Y;
                    Para_List.Parameter.Cam_Rtc_X = parameter.Cam_Rtc_X;
                    Para_List.Parameter.Cam_Rtc_Y = parameter.Cam_Rtc_Y;
                    Para_List.Parameter.Delta_X = parameter.Delta_X;
                    Para_List.Parameter.Delta_Y = parameter.Delta_Y;
                    Para_List.Parameter.Pic_Center_X = parameter.Pic_Center_X;
                    Para_List.Parameter.Pic_Center_Y = parameter.Pic_Center_Y;
                    Para_List.Parameter.Reset_Completely = parameter.Reset_Completely;
                    Para_List.Parameter.Default_Card = parameter.Default_Card;
                    Para_List.Parameter.Laser_Mode = parameter.Laser_Mode;
                    Para_List.Parameter.Laser_Control = parameter.Laser_Control;
                    Para_List.Parameter.Rtc_Period_Reference = parameter.Rtc_Period_Reference;
                    Para_List.Parameter.Laser_Delay_Reference = parameter.Laser_Delay_Reference;
                    Para_List.Parameter.Scanner_Delay_Reference = parameter.Scanner_Delay_Reference;
                    Para_List.Parameter.Rtc_XPos_Reference = parameter.Rtc_XPos_Reference;
                    Para_List.Parameter.Rtc_YPos_Reference = parameter.Rtc_YPos_Reference;
                    Para_List.Parameter.Analog_Out_Ch = parameter.Analog_Out_Ch;
                    Para_List.Parameter.Analog_Out_Value = parameter.Analog_Out_Value;
                    Para_List.Parameter.Analog_Out_Standby = parameter.Analog_Out_Standby;
                    Para_List.Parameter.Standby_Half_Period = parameter.Standby_Half_Period;
                    Para_List.Parameter.Standby_Pulse_Width = parameter.Standby_Pulse_Width;
                    Para_List.Parameter.Laser_Half_Period = parameter.Laser_Half_Period;
                    Para_List.Parameter.Laser_Pulse_Width = parameter.Laser_Pulse_Width;
                    Para_List.Parameter.First_Pulse_Killer = parameter.First_Pulse_Killer;
                    Para_List.Parameter.Laser_On_Delay = parameter.Laser_On_Delay;
                    Para_List.Parameter.Laser_Off_Delay = parameter.Laser_Off_Delay;
                    Para_List.Parameter.Warmup_Time = parameter.Warmup_Time;
                    Para_List.Parameter.Jump_Delay = parameter.Jump_Delay;
                    Para_List.Parameter.Mark_Delay = parameter.Mark_Delay;
                    Para_List.Parameter.Polygon_Delay = parameter.Polygon_Delay;
                    Para_List.Parameter.Arc_Delay = parameter.Arc_Delay;
                    Para_List.Parameter.Line_Delay = parameter.Line_Delay;
                    Para_List.Parameter.Mark_Speed = parameter.Mark_Speed;
                    Para_List.Parameter.Jump_Speed = parameter.Jump_Speed;
                    Para_List.Parameter.List1_Size = parameter.List1_Size;
                    Para_List.Parameter.List2_Size = parameter.List2_Size;
                    Para_List.Parameter.Rtc_Home = parameter.Rtc_Home;
                    Para_List.Parameter.Com_No = parameter.Com_No;
                }
            }
        }

    }
   
}