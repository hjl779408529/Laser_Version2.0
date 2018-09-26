using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using Laser_Build_1._0;

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

        //GTS坐标矫正变换参数
        private static decimal gts_calibration_x_len = 350.0m;//标定板X尺寸 mm
        private static decimal gts_calibration_y_len = 350.0m;//标定板Y尺寸 mm
        private static decimal gts_calibration_cell = 2.5m;//标定板尺寸 mm
        private static Int16 gts_calibration_row = Convert.ToInt16((gts_calibration_x_len / gts_calibration_cell) + 1), gts_calibration_col = Convert.ToInt16((gts_calibration_y_len / gts_calibration_cell) + 1);//标定板的点位gts_calibration横纵数
        private static Int16 gts_affinity_row = Convert.ToInt16(gts_calibration_x_len / gts_calibration_cell), gts_affinity_col = Convert.ToInt16(gts_calibration_y_len / gts_calibration_cell);//仿射变换的横纵数 
        private static Int16 gts_affinity_type = 1;//GTS坐标矫正变换类型 0-三对点 小区块数据 仿射变换、1-全部点对 仿射变换
        //RTC坐标矫正变换参数
        private static decimal rtc_calibration_x_len = 350.0m;//标定板X尺寸 mm
        private static decimal rtc_calibration_y_len = 350.0m;//标定板Y尺寸 mm
        private static decimal rtc_calibration_cell = 2.5m;//标定板尺寸 mm
        private static Int16 rtc_calibration_row = Convert.ToInt16((rtc_calibration_x_len / rtc_calibration_cell) + 1), rtc_calibration_col = Convert.ToInt16((rtc_calibration_y_len / rtc_calibration_cell) + 1);//标定板的点位rtc_calibration横纵数
        private static Int16 rtc_affinity_row = Convert.ToInt16(rtc_calibration_x_len / rtc_calibration_cell), rtc_affinity_col = Convert.ToInt16(rtc_calibration_y_len / rtc_calibration_cell);//仿射变换的横纵数 
        private static Int16 rtc_affinity_type = 1;//RTC坐标矫正变换类型 0-三对点 小区块数据 仿射变换、1-全部点对 仿射变换
        //加工坐标系
        private static Vector work = new Vector(0,0);//加工坐标系
        //刀具绝对坐标 激光光束相对于整个平台绝对坐标
        private static Vector laser =new Vector(0,0);//刀具绝对坐标X坐标 Vector mark1 = new Vector(0, 0);
        //相机单像素对应的实际比例
        private static decimal cam_reference = 0.0m;
        //相机中心与坐标系原点对正时，像素偏差值，每次开机时矫正（待确定）
        private static Vector cam_org_pixel = new Vector(0,0); 
        //Camera中心 相对于 直角坐标系原点 相对间距
        private static Vector cam_org= new Vector(0,0);
        //振镜激光原点  相对于 直角坐标系原点 相对间距
        private static Vector rtc_org = new Vector(0,0); 
        //Camera中心 相对于 振镜激光原点   相对间距
        private static Vector cam_rtc = new Vector(0,0);
        //偏差补偿值  Dxf中图形 与实际工件摆放的 偏差值，每次加工前进行处理调用获取该值
        private static Vector delta = new Vector(0,0);
        //定义图像中心点
        private static Vector pic_center = new Vector(0,0);
        //定义矫正坐标点
        private static Vector cal_org = new Vector(0, 0);

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
        private static Vector rtc_home = new Vector(0, 0);

        //定义串口号
        private static Int32 com_no = 0;

        //定义RTC校准尺寸
        private static decimal rtc_cal_radius = 1.0m; //圆半径
        private static decimal rtc_cal_interval = 4.0m; //间距

        //mark点参数
        private static Vector mark1 = new Vector(0, 0);
        private static Vector mark2 = new Vector(0, 0);
        private static Vector mark3 = new Vector(0, 0);
        //Dxf Mark点参数
        private static Vector mark_dxf1 = new Vector(0, 0);
        private static Vector mark_dxf2 = new Vector(0, 0);
        private static Vector mark_dxf3 = new Vector(0, 0);
        //整体的仿射变换参数
        private static Affinity_Matrix trans_affinity = new Affinity_Matrix();
        //刀具补偿
        private static decimal cutter_radius = 0.5m; //刀具半径
        private static Int16 cutter_type = 0; //刀具补偿类型 0-不补偿、1-钻孔、2-落料
        //振镜加工范围约束
        private static Vector rtc_limit = new Vector(50,50);
        //加工次数
        private static UInt16 gts_repeat;
        private static UInt16 rtc_repeat;

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
        public static decimal Gts_Calibration_X_Len { get => gts_calibration_x_len; set => gts_calibration_x_len = value; }
        public static decimal Gts_Calibration_Y_Len { get => gts_calibration_y_len; set => gts_calibration_y_len = value; }
        public static decimal Gts_Calibration_Cell { get => gts_calibration_cell; set => gts_calibration_cell = value; }
        public static Int16 Gts_Calibration_Col { get => gts_calibration_col; set => gts_calibration_col = value; }
        public static Int16 Gts_Calibration_Row { get => gts_calibration_row; set => gts_calibration_row = value; }
        public static Int16 Gts_Affinity_Col { get => gts_affinity_col; set => gts_affinity_col = value; }
        public static Int16 Gts_Affinity_Row { get => gts_affinity_row; set => gts_affinity_row = value; }
        public static Int16 Gts_Affinity_Type { get => gts_affinity_type; set => gts_affinity_type = value; }        
        public static decimal Rtc_Calibration_X_Len { get => rtc_calibration_x_len; set => rtc_calibration_x_len = value; }
        public static decimal Rtc_Calibration_Y_Len { get => rtc_calibration_y_len; set => rtc_calibration_y_len = value; }
        public static decimal Rtc_Calibration_Cell { get => rtc_calibration_cell; set => rtc_calibration_cell = value; }
        public static Int16 Rtc_Calibration_Col { get => rtc_calibration_col; set => rtc_calibration_col = value; }
        public static Int16 Rtc_Calibration_Row { get => rtc_calibration_row; set => rtc_calibration_row = value; }
        public static Int16 Rtc_Affinity_Col { get => rtc_affinity_col; set => rtc_affinity_col = value; }
        public static Int16 Rtc_Affinity_Row { get => rtc_affinity_row; set => rtc_affinity_row = value; }
        public static Int16 Rtc_Affinity_Type { get => rtc_affinity_type; set => rtc_affinity_type = value; }        
        public static Vector Work { get => work; set => work = value; }
        public static Vector Laser { get => laser; set => laser = value; }
        public static decimal Cam_Reference { get => cam_reference; set => cam_reference = value; }
        public static Vector Cam_Org_Pixel { get => cam_org_pixel; set => cam_org_pixel = value; }     
        public static Vector Cam_Org { get => cam_org; set => cam_org = value; }
        public static Vector Rtc_Org { get => rtc_org; set => rtc_org = value; }
        public static Vector Cam_Rtc { get => cam_rtc; set => cam_rtc = value; }
        public static Vector Delta { get => delta; set => delta = value; }
        public static Vector Pic_Center { get => pic_center; set => pic_center = value; }
        public static Vector Cal_Org { get => cal_org; set => cal_org = value; } 

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
        public static decimal Rtc_Cal_Radius { get => rtc_cal_radius; set => rtc_cal_radius = value; }
        public static decimal Rtc_Cal_Interval { get => rtc_cal_interval; set => rtc_cal_interval = value; }
        public static Vector Mark1 { get => mark1; set => mark1 = value; }
        public static Vector Mark2 { get => mark2; set => mark2 = value; }
        public static Vector Mark3 { get => mark3; set => mark3 = value; }
        public static Vector Mark_Dxf1 { get => mark_dxf1; set => mark_dxf1 = value; }
        public static Vector Mark_Dxf2 { get => mark_dxf2; set => mark_dxf2 = value; }
        public static Vector Mark_Dxf3 { get => mark_dxf3; set => mark_dxf3 = value; }
        public static Affinity_Matrix Trans_Affinity { get => trans_affinity; set => trans_affinity = value; }
        public static decimal Cutter_Radius { get => cutter_radius; set => cutter_radius = value; }
        public static Int16 Cutter_Type { get => cutter_type; set => cutter_type = value; }        
        public static Vector Rtc_Limit { get => rtc_limit; set => rtc_limit = value; }
        public static UInt16 Gts_Repeat { get => gts_repeat; set => gts_repeat = value; }
        public static UInt16 Rtc_Repeat { get => rtc_repeat; set => rtc_repeat = value; }
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

        //GTS坐标矫正变换参数
        private decimal gts_calibration_x_len;//标定板X尺寸 mm
        private decimal gts_calibration_y_len;//标定板Y尺寸 mm
        private decimal gts_calibration_cell;//标定板间隔尺寸 mm 
        private Int16 gts_calibration_row, gts_calibration_col;//标定板的点位gts_calibration横纵数 row-行-x  col-列-y
        private Int16 gts_affinity_row, gts_affinity_col;//仿射变换的横纵数 row-行-x  col-列-y
        private Int16 gts_affinity_type = 1;//GTS坐标矫正变换类型 0-三对点 小区块数据 仿射变换、1-全部点对 仿射变换

        //RTC坐标矫正变换参数
        private decimal rtc_calibration_x_len;//标定板X尺寸 mm
        private decimal rtc_calibration_y_len;//标定板Y尺寸 mm
        private decimal rtc_calibration_cell;//标定板间隔尺寸 mm 
        private Int16 rtc_calibration_row, rtc_calibration_col;//标定板的点位rtc_calibration横纵数 row-行-x  col-列-y
        private Int16 rtc_affinity_row, rtc_affinity_col;//仿射变换的横纵数 row-行-x  col-列-y
        private Int16 rtc_affinity_type = 1;//RTC坐标矫正变换类型 0-三对点 小区块数据 仿射变换、1-全部点对 仿射变换

        //加工坐标系
        private Vector work;//加工坐标系
        //刀具绝对坐标 激光光束相对于整个平台绝对坐标
        private Vector laser;//刀具绝对坐标X坐标 Vector mark1 = new Vector(0, 0);
        //相机单像素对应的实际比例
        private decimal cam_reference;
        //相机中心与坐标系原点对正时，像素偏差值，每次开机时矫正（待确定）
        private Vector cam_org_pixel;
        //Camera中心 相对于 直角坐标系原点 相对间距
        private Vector cam_org;
        //振镜激光原点  相对于 直角坐标系原点 相对间距
        private Vector rtc_org;
        //Camera中心 相对于 振镜激光原点   相对间距
        private Vector cam_rtc;
        //偏差补偿值  Dxf中图形 与实际工件摆放的 偏差值，每次加工前进行处理调用获取该值
        private Vector delta;
        //定义图像中心点
        private Vector pic_center;
        //定义矫正坐标点
        private Vector cal_org;

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
        //定义RTC校准尺寸
        private decimal rtc_cal_radius; //圆半径
        private decimal rtc_cal_interval; //间距
        //mark点参数
        private Vector mark1 = new Vector(0, 0);
        private Vector mark2 = new Vector(0, 0);
        private Vector mark3 = new Vector(0, 0);
        //Dxf Mark点参数
        private Vector mark_dxf1 = new Vector(0, 0);
        private Vector mark_dxf2 = new Vector(0, 0);
        private Vector mark_dxf3 = new Vector(0, 0);
        //整体的仿射变换参数
        private Affinity_Matrix trans_affinity;
        //刀具补偿
        private decimal cutter_radius = 0.5m; //刀具半径
        private Int16 cutter_type = 0; //刀具补偿类型 0-不补偿、1-钻孔、2-落料
        private Vector rtc_limit;
        //加工次数
        private UInt16 gts_repeat;
        private UInt16 rtc_repeat;

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
        public decimal Gts_Calibration_X_Len { get => gts_calibration_x_len; set => gts_calibration_x_len = value; }
        public decimal Gts_Calibration_Y_Len { get => gts_calibration_y_len; set => gts_calibration_y_len = value; }
        public decimal Gts_Calibration_Cell { get => gts_calibration_cell; set => gts_calibration_cell = value; }
        public Int16 Gts_Calibration_Col { get => gts_calibration_col; set => gts_calibration_col = value; }
        public Int16 Gts_Calibration_Row { get => gts_calibration_row; set => gts_calibration_row = value; }
        public Int16 Gts_Affinity_Col { get => gts_affinity_col; set => gts_affinity_col = value; }
        public Int16 Gts_Affinity_Row { get => gts_affinity_row; set => gts_affinity_row = value; }
        public Int16 Gts_Affinity_Type { get => gts_affinity_type; set => gts_affinity_type = value; }
        public decimal Rtc_Calibration_X_Len { get => rtc_calibration_x_len; set => rtc_calibration_x_len = value; }
        public decimal Rtc_Calibration_Y_Len { get => rtc_calibration_y_len; set => rtc_calibration_y_len = value; }
        public decimal Rtc_Calibration_Cell { get => rtc_calibration_cell; set => rtc_calibration_cell = value; }
        public Int16 Rtc_Calibration_Col { get => rtc_calibration_col; set => rtc_calibration_col = value; }
        public Int16 Rtc_Calibration_Row { get => rtc_calibration_row; set => rtc_calibration_row = value; }
        public Int16 Rtc_Affinity_Col { get => rtc_affinity_col; set => rtc_affinity_col = value; }
        public Int16 Rtc_Affinity_Row { get => rtc_affinity_row; set => rtc_affinity_row = value; }
        public Int16 Rtc_Affinity_Type { get => rtc_affinity_type; set => rtc_affinity_type = value; }
        public Vector Work { get => work; set => work = value; }
        public Vector Laser { get => laser; set => laser = value; }
        public decimal Cam_Reference { get => cam_reference; set => cam_reference = value; }
        public Vector Cam_Org_Pixel { get => cam_org_pixel; set => cam_org_pixel = value; }
        public Vector Cam_Org { get => cam_org; set => cam_org = value; }
        public Vector Rtc_Org { get => rtc_org; set => rtc_org = value; }
        public Vector Cam_Rtc { get => cam_rtc; set => cam_rtc = value; }
        public Vector Delta { get => delta; set => delta = value; }
        public Vector Pic_Center { get => pic_center; set => pic_center = value; }
        public Vector Cal_Org { get => cal_org; set => cal_org = value; }        
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
        public decimal Rtc_Cal_Radius { get => rtc_cal_radius; set => rtc_cal_radius = value; }
        public decimal Rtc_Cal_Interval { get => rtc_cal_interval; set => rtc_cal_interval = value; }
        public Vector Mark1 { get => mark1; set => mark1 = value; }
        public Vector Mark2 { get => mark2; set => mark2 = value; }
        public Vector Mark3 { get => mark3; set => mark3 = value; }
        public Vector Mark_Dxf1 { get => mark_dxf1; set => mark_dxf1 = value; }
        public Vector Mark_Dxf2 { get => mark_dxf2; set => mark_dxf2 = value; }
        public Vector Mark_Dxf3 { get => mark_dxf3; set => mark_dxf3 = value; }
        public Affinity_Matrix Trans_Affinity { get => trans_affinity; set => trans_affinity = value; }
        public decimal Cutter_Radius { get => cutter_radius; set => cutter_radius = value; }
        public Int16 Cutter_Type { get => cutter_type; set => cutter_type = value; }
        public Vector Rtc_Limit { get => rtc_limit; set => rtc_limit = value; }
        public UInt16 Gts_Repeat { get => gts_repeat; set => gts_repeat = value; }
        public UInt16 Rtc_Repeat { get => rtc_repeat; set => rtc_repeat = value; }
        //构造函数
        public Parameter_RW() { }
    } 
    //参数变量序列化
    public class Serialize_Parameter
    {

        //参数保存
        public static void Serialize(string txtFile)
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
                Gts_Calibration_X_Len = Para_List.Parameter.Gts_Calibration_X_Len,
                Gts_Calibration_Y_Len = Para_List.Parameter.Gts_Calibration_Y_Len,
                Gts_Calibration_Cell = Para_List.Parameter.Gts_Calibration_Cell,
                Gts_Calibration_Col = Para_List.Parameter.Gts_Calibration_Col,
                Gts_Calibration_Row = Para_List.Parameter.Gts_Calibration_Row,
                Gts_Affinity_Col = Para_List.Parameter.Gts_Affinity_Col,
                Gts_Affinity_Row = Para_List.Parameter.Gts_Affinity_Row,
                Gts_Affinity_Type = Para_List.Parameter.Gts_Affinity_Type,
                Rtc_Calibration_X_Len = Para_List.Parameter.Rtc_Calibration_X_Len,
                Rtc_Calibration_Y_Len = Para_List.Parameter.Rtc_Calibration_Y_Len,
                Rtc_Calibration_Cell = Para_List.Parameter.Rtc_Calibration_Cell,
                Rtc_Calibration_Col = Para_List.Parameter.Rtc_Calibration_Col,
                Rtc_Calibration_Row = Para_List.Parameter.Rtc_Calibration_Row,
                Rtc_Affinity_Col = Para_List.Parameter.Rtc_Affinity_Col,
                Rtc_Affinity_Row = Para_List.Parameter.Rtc_Affinity_Row,
                Rtc_Affinity_Type = Para_List.Parameter.Rtc_Affinity_Type,
                Work = Para_List.Parameter.Work,
                Laser = Para_List.Parameter.Laser,
                Cam_Reference = Para_List.Parameter.Cam_Reference,
                Cam_Org_Pixel = Para_List.Parameter.Cam_Org_Pixel,
                Cam_Org = Para_List.Parameter.Cam_Org,
                Rtc_Org = Para_List.Parameter.Rtc_Org,
                Cam_Rtc = Para_List.Parameter.Cam_Rtc,
                Delta = Para_List.Parameter.Delta,
                Pic_Center = Para_List.Parameter.Pic_Center,
                Cal_Org = Para_List.Parameter.Cal_Org,
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
                Com_No = Para_List.Parameter.Com_No,
                Rtc_Cal_Radius = Para_List.Parameter.Rtc_Cal_Radius,
                Rtc_Cal_Interval = Para_List.Parameter.Rtc_Cal_Interval,
                Mark1 = Para_List.Parameter.Mark1,
                Mark2 = Para_List.Parameter.Mark2,
                Mark3 = Para_List.Parameter.Mark3,
                Mark_Dxf1 = Para_List.Parameter.Mark_Dxf1,
                Mark_Dxf2 = Para_List.Parameter.Mark_Dxf2,
                Mark_Dxf3 = Para_List.Parameter.Mark_Dxf3,
                Trans_Affinity = Para_List.Parameter.Trans_Affinity,
                Cutter_Radius = Para_List.Parameter.Cutter_Radius,
                Cutter_Type = Para_List.Parameter.Cutter_Type,
                Rtc_Limit = Para_List.Parameter.Rtc_Limit,
                Gts_Repeat = Para_List.Parameter.Gts_Repeat,
                Rtc_Repeat = Para_List.Parameter.Rtc_Repeat
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
        public static void Reserialize(string fileName)
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
                    Para_List.Parameter.Gts_Calibration_X_Len = parameter.Gts_Calibration_X_Len;
                    Para_List.Parameter.Gts_Calibration_Y_Len = parameter.Gts_Calibration_Y_Len;
                    Para_List.Parameter.Gts_Calibration_Cell = parameter.Gts_Calibration_Cell;
                    Para_List.Parameter.Gts_Calibration_Col = parameter.Gts_Calibration_Col;
                    Para_List.Parameter.Gts_Calibration_Row = parameter.Gts_Calibration_Row;
                    Para_List.Parameter.Gts_Affinity_Col = parameter.Gts_Affinity_Col;
                    Para_List.Parameter.Gts_Affinity_Row = parameter.Gts_Affinity_Row;
                    Para_List.Parameter.Gts_Affinity_Type = parameter.Gts_Affinity_Type;
                    Para_List.Parameter.Rtc_Calibration_X_Len = parameter.Rtc_Calibration_X_Len;
                    Para_List.Parameter.Rtc_Calibration_Y_Len = parameter.Rtc_Calibration_Y_Len;
                    Para_List.Parameter.Rtc_Calibration_Cell = parameter.Rtc_Calibration_Cell;
                    Para_List.Parameter.Rtc_Calibration_Col = parameter.Rtc_Calibration_Col;
                    Para_List.Parameter.Rtc_Calibration_Row = parameter.Rtc_Calibration_Row;
                    Para_List.Parameter.Rtc_Affinity_Col = parameter.Rtc_Affinity_Col;
                    Para_List.Parameter.Rtc_Affinity_Row = parameter.Rtc_Affinity_Row;
                    Para_List.Parameter.Rtc_Affinity_Type = parameter.Rtc_Affinity_Type;
                    Para_List.Parameter.Work = parameter.Work;
                    Para_List.Parameter.Laser = parameter.Laser;
                    Para_List.Parameter.Cam_Reference = parameter.Cam_Reference;
                    Para_List.Parameter.Cam_Org_Pixel = parameter.Cam_Org_Pixel;
                    Para_List.Parameter.Cam_Org = parameter.Cam_Org;
                    Para_List.Parameter.Rtc_Org = parameter.Rtc_Org;
                    Para_List.Parameter.Cam_Rtc = parameter.Cam_Rtc;
                    Para_List.Parameter.Delta = parameter.Delta;
                    Para_List.Parameter.Pic_Center = parameter.Pic_Center;
                    Para_List.Parameter.Cal_Org = parameter.Cal_Org;
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
                    Para_List.Parameter.Rtc_Cal_Radius = parameter.Rtc_Cal_Radius;
                    Para_List.Parameter.Rtc_Cal_Interval = parameter.Rtc_Cal_Interval;
                    Para_List.Parameter.Mark1 = parameter.Mark1;
                    Para_List.Parameter.Mark2 = parameter.Mark2;
                    Para_List.Parameter.Mark3 = parameter.Mark3;
                    Para_List.Parameter.Mark_Dxf1 = parameter.Mark_Dxf1;
                    Para_List.Parameter.Mark_Dxf2 = parameter.Mark_Dxf2;
                    Para_List.Parameter.Mark_Dxf3 = parameter.Mark_Dxf3;
                    Para_List.Parameter.Trans_Affinity = parameter.Trans_Affinity;
                    Para_List.Parameter.Cutter_Radius = parameter.Cutter_Radius;
                    Para_List.Parameter.Cutter_Type = parameter.Cutter_Type;
                    Para_List.Parameter.Rtc_Limit = parameter.Rtc_Limit;
                    Para_List.Parameter.Gts_Repeat = parameter.Gts_Repeat;
                    Para_List.Parameter.Rtc_Repeat = parameter.Rtc_Repeat;
                }
            }
        }

    }
   
}