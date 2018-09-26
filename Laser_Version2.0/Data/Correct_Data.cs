using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Management;
using System.IO;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.Util;
using System.Drawing;
using System.Threading;
using System.Xml.Serialization;
using System.Windows.Forms;
using MathNet.Numerics;
using Laser_Version2._0;
using System.Data;

namespace Laser_Build_1._0
{
    //矫正数据存储
    [Serializable]
    public struct Correct_Data
    {
        //私有属性
        private decimal xo, yo;//x0,y0--基准坐标
        private decimal xm, ym;//x1,y1--轴实际坐标 

        //公开访问的属性
        public decimal Xo
        {
            get { return xo; }
            set { xo = value; }
        }
        public decimal Yo
        {
            get { return yo; }
            set { yo = value; }
        }
        public decimal Xm
        {
            get { return xm; }
            set { xm = value; }
        }
        public decimal Ym
        {
            get { return ym; }
            set { ym = value; }
        }


        //公开访问的方法
        //构造函数
        public Correct_Data(Correct_Data Ini)
        {
            this.xo = Ini.Xo;
            this.yo = Ini.Yo;
            this.xm = Ini.Xm;
            this.ym = Ini.Ym;
        }
        public Correct_Data(decimal xo, decimal yo, decimal xm, decimal ym)
        {
            this.xo = xo;
            this.yo = yo;
            this.xm = xm;
            this.ym = ym;
        }
        //清空数据
        public void Empty()
        {
            this.xo = 0;
            this.yo = 0;
            this.xm = 0;
            this.ym = 0;
        }
    }
    [Serializable]
    public struct Affinity_Matrix
    {
        //私有属性 
        private decimal cos_value, sin_value;//cos_value, sin_value--仿射变换角度值 
        private decimal delta_x, delta_y;//delta_x, delta_y--x、y坐标偏移值

        //共有属性
        public decimal Cos_Value{
            get { return cos_value; }
            set { cos_value = value; }
        }
        public decimal Sin_Value
        {
            get { return sin_value; }
            set { sin_value = value; }
        }

        public decimal Delta_X
        {
            get { return delta_x; }
            set { delta_x = value; }
        }
        public decimal Delta_Y
        {
            get { return delta_y; }
            set { delta_y = value; }
        }
        //公开构造函数        
        //有参数
        public Affinity_Matrix(decimal cos_value,decimal sin_value, decimal delta_x,decimal delta_y)
        {
            this.cos_value = cos_value;
            this.sin_value = sin_value;
            this.delta_x = delta_x;
            this.delta_y = delta_y;
        }
        public Affinity_Matrix(Affinity_Matrix Ini)
        {
            this.cos_value = Ini.Cos_Value;
            this.sin_value = Ini.Sin_Value;
            this.delta_x = Ini.Delta_X;
            this.delta_y = Ini.Delta_Y;
        }
        //清空
        public void Empty()
        {
            this.cos_value = 0;
            this.sin_value = 0;
            this.delta_x = 0;
            this.delta_y = 0;
        }
    }
    public struct Fit_Data
    {
        //私有属性 
        private decimal k1, k2,k3,k4;//k1-1次系数，k2-2次系数，k3-3次系数，k4-4次系数
        private decimal delta;//坐标偏移值
        //共有属性
        public decimal K1
        {
            get { return k1; }
            set { k1 = value; }
        }
        public decimal K2
        {
            get { return k2; }
            set { k2 = value; }
        }
        public decimal K3
        {
            get { return k3; }
            set { k3 = value; }
        }
        public decimal K4
        {
            get { return k4; }
            set { k4 = value; }
        }
        public decimal Delta
        {
            get { return delta; }
            set { delta = value; }
        }
        //公开构造函数        
        //有参数
        public Fit_Data(decimal k1, decimal k2, decimal k3, decimal k4, decimal delta)
        {
            this.k1 = k1;
            this.k2 = k2;
            this.k3 = k3;
            this.k4 = k4;
            this.delta = delta;
        }
        public Fit_Data(Fit_Data Ini)
        {
            this.k1 = Ini.K1;
            this.k2 = Ini.K2;
            this.k3 = Ini.K3;
            this.k4 = Ini.K4;
            this.delta = Ini.Delta;
        }
        //清空
        public void Empty()
        {
            this.k1 = 0;
            this.k2 = 0;
            this.k3 = 0;
            this.k4 = 0;
            this.delta = 0;
        }
    }
    public struct Double_Fit_Data
    {
        //私有属性 
        private decimal k_x1, k_x2, k_x3, k_x4;//k_x1-1次系数，k_x2-2次系数，k_x3-3次系数，k_x4-4次系数
        private decimal delta_x;//坐标偏移值
        private decimal k_y1, k_y2, k_y3, k_y4;//k_y1-1次系数，k_y2-2次系数，k_y3-3次系数，k_y4-4次系数
        private decimal delta_y;//坐标偏移值
        //共有属性
        public decimal K_X1
        {
            get { return k_x1; }
            set { k_x1 = value; }
        }
        public decimal K_X2
        {
            get { return k_x2; }
            set { k_x2 = value; }
        }
        public decimal K_X3
        {
            get { return k_x3; }
            set { k_x3 = value; }
        }
        public decimal K_X4
        {
            get { return k_x4; }
            set { k_x4 = value; }
        }
        public decimal Delta_X
        {
            get { return delta_x; }
            set { delta_x = value; }
        }
        public decimal K_Y1
        {
            get { return k_y1; }
            set { k_y1 = value; }
        }
        public decimal K_Y2
        {
            get { return k_y2; }
            set { k_y2 = value; }
        }
        public decimal K_Y3
        {
            get { return k_y3; }
            set { k_y3 = value; }
        }
        public decimal K_Y4
        {
            get { return k_y4; }
            set { k_y4 = value; }
        }
        public decimal Delta_Y
        {
            get { return delta_y; }
            set { delta_y = value; }
        }
        //公开构造函数        
        //有参数
        public Double_Fit_Data(decimal k_x1, decimal k_x2, decimal k_x3, decimal k_x4, decimal delta_x, decimal k_y1, decimal k_y2, decimal k_y3, decimal k_y4, decimal delta_y)
        {
            this.k_x1 = k_x1;
            this.k_x2 = k_x2;
            this.k_x3 = k_x3;
            this.k_x4 = k_x4;
            this.delta_x = delta_x;
            this.k_y1 = k_y1;
            this.k_y2 = k_y2;
            this.k_y3 = k_y3;
            this.k_y4 = k_y4;
            this.delta_y = delta_y;
        }
        public Double_Fit_Data(Double_Fit_Data Ini)
        {
            this.k_x1 = Ini.K_X1;
            this.k_x2 = Ini.K_X2;
            this.k_x3 = Ini.K_X3;
            this.k_x4 = Ini.K_X4;
            this.delta_x = Ini.Delta_X;
            this.k_y1 = Ini.K_Y1;
            this.k_y2 = Ini.K_Y2;
            this.k_y3 = Ini.K_Y3;
            this.k_y4 = Ini.K_Y4;
            this.delta_y = Ini.Delta_Y;
        }
        //清空
        public void Empty()
        {
            this.k_x1 = 0;
            this.k_x2 = 0;
            this.k_x3 = 0;
            this.k_x4 = 0;
            this.delta_x = 0;
            this.k_y1 = 0;
            this.k_y2 = 0;
            this.k_y3 = 0;
            this.k_y4 = 0;
            this.delta_y = 0;
        }
    }
    public class Calibration 
    {
        //定义退出变量
        public static bool Exit_Flag = false;
        public static List<Correct_Data> Get_Datas()
        {
            //建立变量
            List<Correct_Data> Result = new List<Correct_Data>();
            Correct_Data Temp_Correct_Data = new Correct_Data();

            //建立变量
            Vector Cam_Old=new Vector();
            Vector Cam_New=new Vector();
            decimal Cam_Delta_X = 0, Cam_Delta_Y = 0;
            int i = 0, j = 0;

            ////两轴回零
            //Thread Axis01_home_thread = new Thread(this.Axis01_Home);
            //Thread Axis02_home_thread = new Thread(this.Axis02_Home);
            //Axis01_home_thread.Start();
            //Axis02_home_thread.Start();
            ////等待线程结束
            //Axis01_home_thread.Join();
            //Axis02_home_thread.Join();

            //建立直角坐标系
            GTS_Fun.Interpolation.Coordination(Para_List.Parameter.Work.X, Para_List.Parameter.Work.Y);
            //定位到加工坐标原点
            GTS_Fun.Interpolation.Clear_FIFO();
            GTS_Fun.Interpolation.Line_FIFO(0, 0);//将直线插补数据写入
            GTS_Fun.Interpolation.Interpolation_Start();
            //停止坐标系运动
            GTS_Fun.Interpolation.Interpolation_Stop();
            
            //1轴-x轴，2轴-y轴，X轴归零，y轴归 步距*i
            //motion.Abs(1, Convert.ToDouble(500 / Para_List.Parameter.Acc_reference), Convert.ToDouble(500 / Para_List.Parameter.Acc_reference), 2, Convert.ToInt32(Para_List.Parameter.Work_X), Convert.ToDouble(100 / Para_List.Parameter.Vel_reference));//绝对定位至坐标系X为零
            //motion.Abs(2, Convert.ToDouble(500 / Para_List.Parameter.Acc_reference), Convert.ToDouble(500 / Para_List.Parameter.Acc_reference), 2, Convert.ToInt32(Para_List.Parameter.Work_Y), Convert.ToDouble(100 / Para_List.Parameter.Vel_reference));//绝对定位至坐标系Y为零

            //2.5mm步距进行数据提取和整合，使用INC指令
            for (i = 0; i < Para_List.Parameter.Gts_Calibration_Row; i++)
            {
                //1轴-x轴，2轴-y轴，X轴归零，y轴归 步距*i
                //定位实现
                //motion.Abs(1, 500, 500, 2, Para_List.Parameter.Work_X,100);//绝对定位至坐标系X为零
                //motion.Inc(2, 500, 500, 2, -Para_List.Parameter.Gts_Calibration_Cell, 100);//循序渐加
                //插补运动实现
                GTS_Fun.Interpolation.Clear_FIFO();
                GTS_Fun.Interpolation.Line_FIFO(0, i * Para_List.Parameter.Gts_Calibration_Cell);//将直线插补数据写入
                GTS_Fun.Interpolation.Interpolation_Start();
                GTS_Fun.Interpolation.Interpolation_Stop();
                for (j = 0; j < Para_List.Parameter.Gts_Calibration_Col; j++)
                {
                    //清空Temp_Correct_Data
                    Temp_Correct_Data.Empty();
                    //定位X轴
                    //定位实现
                    //motion.Inc(1, 500, 500, 2, -Para_List.Parameter.Gts_Calibration_Cell, 100);
                    //插补运动实现
                    GTS_Fun.Interpolation.Clear_FIFO();
                    GTS_Fun.Interpolation.Line_FIFO(j * Para_List.Parameter.Gts_Calibration_Cell, i * Para_List.Parameter.Gts_Calibration_Cell);//将直线插补数据写入
                    GTS_Fun.Interpolation.Interpolation_Start();
                    GTS_Fun.Interpolation.Interpolation_Stop();
                    //调用相机，获取对比的坐标信息
                    Common_Method.Delay_Time.Delay(200);//延时200ms
                    //Main.T_Client
                    Cam_New = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation(1));//触发拍照    
                    
                    if (j == 0)//X轴坐标归零
                    {
                        //计算差值
                        Cam_Delta_X = Cam_New.X - 243 * Para_List.Parameter.Cam_Reference;
                        Cam_Delta_Y = Cam_New.Y - 324 * Para_List.Parameter.Cam_Reference;
                        //数据保存
                        Temp_Correct_Data.Xo = j * Para_List.Parameter.Gts_Calibration_Cell + Cam_Delta_X;//相机实际X坐标
                        Temp_Correct_Data.Yo = i * Para_List.Parameter.Gts_Calibration_Cell + Cam_Delta_Y;//相机实际Y坐标
                        Temp_Correct_Data.Xm = j * Para_List.Parameter.Gts_Calibration_Cell;//平台电机实际X坐标
                        Temp_Correct_Data.Ym = i * Para_List.Parameter.Gts_Calibration_Cell;//平台电机实际Y坐标
                    }
                    else
                    {
                        //计算差值
                        Cam_Delta_X = Cam_New.X - Cam_Old.X;
                        Cam_Delta_Y = Cam_New.Y - Cam_Old.Y;
                        //数据保存
                        Temp_Correct_Data.Xo = j * Para_List.Parameter.Gts_Calibration_Cell + Cam_Delta_X;//相机实际X坐标
                        Temp_Correct_Data.Yo = i * Para_List.Parameter.Gts_Calibration_Cell + Cam_Delta_Y;//相机实际Y坐标
                        Temp_Correct_Data.Xm = j * Para_List.Parameter.Gts_Calibration_Cell;//平台电机实际X坐标
                        Temp_Correct_Data.Ym = i * Para_List.Parameter.Gts_Calibration_Cell;//平台电机实际Y坐标
                    }

                    //New变Old
                    Cam_Old =new Vector(Cam_New);
                    //添加进入List
                    Result.Add(Temp_Correct_Data);

                    //线程终止
                    if (Exit_Flag)
                    {
                        Exit_Flag = false;
                        Serialize_Data.Serialize_Correct_Data(Result, "Correct_Data_01.xml");
                        return Result;
                    }

                }
            }
            //保存文件至Config
            Serialize_Data.Serialize_Correct_Data(Result, "Correct_Data_01.xml");
            MessageBox.Show("数据采集完成！！！");
            return Result;
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
        
        //矫正原点
        public static void Calibrate_Org()
        {
            //建立变量
            Vector Cam = new Vector();
            Vector Coodinate_Point;
            Vector Tem_Mark;
            UInt16 Counting = 0;           

            //矫正原点
            do
            {   //定位到ORG原点
                Mark(Para_List.Parameter.Cal_Org);
                //调用相机，获取对比的坐标信息
                Common_Method.Delay_Time.Delay(200);//延时200ms
                //Main.T_Client
                Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation(1));//触发拍照 
                Cam = new Vector(Cam.X - 243 * Para_List.Parameter.Cam_Reference, Cam.Y - 324 * Para_List.Parameter.Cam_Reference);
                //获取坐标系平台坐标
                Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate());
                //计算偏移
                Tem_Mark = new Vector(Coodinate_Point.X + Cam.X, Coodinate_Point.Y + Cam.Y);
                //反馈回RTC_ORG数据
                Para_List.Parameter.Cal_Org = new Vector(Tem_Mark);
                //自增
                Counting++;
                //跳出
                if (Counting >= 10)
                {
                    MessageBox.Show("校准失败");
                    return;
                }
            } while (!Differ_Deviation(Cam));
            Para_List.Parameter.Work = new Vector(Para_List.Parameter.Work.X - Para_List.Parameter.Cal_Org.X, Para_List.Parameter.Work.Y - Para_List.Parameter.Cal_Org.Y);
            //数据矫正完成
        }
        //矫正Mark坐标
        public static void Calibrate_Mark()
        {
            //建立变量
            Vector Cam=new Vector();
            Vector Coodinate_Point;
            Vector Tem_Mark;
            UInt16 Counting;
            ////两轴回零
            //Thread Axis01_home_thread = new Thread(this.Axis01_Home);
            //Thread Axis02_home_thread = new Thread(this.Axis02_Home);
            //Axis01_home_thread.Start();
            //Axis02_home_thread.Start();
            ////等待线程结束
            //Axis01_home_thread.Join();
            //Axis02_home_thread.Join();

            ////建立直角坐标系
            //interpolation.Coordination(Para_List.Parameter.Work_X, Para_List.Parameter.Work_Y);

            ////测试程序
            ////定位到Mark1点
            //Mark(Para_List.Parameter.Mark3);
            ////调用相机，获取对比的坐标信息
            //Common_Method.Delay_Time.Delay(200);//延时200ms
            ////Main.T_Client
            //Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation(2));//触发拍照 
            ////获取坐标系平台坐标
            //Coodinate_Point = new Vector(interpolation.Get_Coordinate());
            ////计算偏移
            //Tem_Mark = new Vector(Coodinate_Point.X + Cam.X, Coodinate_Point.Y + Cam.Y);
            ////反馈回Mark点
            //Para_List.Parameter.Mark3 = new Vector(Tem_Mark);
            ////定位到Mark1点
            //Mark(Para_List.Parameter.Mark3);

            
            
            //矫正Mark1           
            Counting = 0;
            do
            {
                //定位到Mark1点
                Mark(Para_List.Parameter.Mark1);
                //调用相机，获取对比的坐标信息
                Common_Method.Delay_Time.Delay(200);//延时200ms
                //Main.T_Client
                Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation(2));//触发拍照 
                //获取坐标系平台坐标
                Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate());
                //计算偏移
                Tem_Mark = new Vector(Coodinate_Point.X + Cam.X, Coodinate_Point.Y + Cam.Y);
                //反馈回Mark点
                Para_List.Parameter.Mark1 = new Vector(Tem_Mark);
                //自增
                Counting++;
                //跳出
                if (Counting >= 20)
                {
                    MessageBox.Show("Mark1 寻找失败");
                    return;
                }
            } while (!Differ_Deviation(Cam));


            //矫正Mark2           
            Counting = 0;
            do
            {
                //定位到Mark1点
                Mark(Para_List.Parameter.Mark2);
                //调用相机，获取对比的坐标信息
                Common_Method.Delay_Time.Delay(200);//延时200ms
                //Main.T_Client
                Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation(2));//触发拍照 
                //获取坐标系平台坐标
                Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate());
                //计算偏移
                Tem_Mark = new Vector(Coodinate_Point.X + Cam.X, Coodinate_Point.Y + Cam.Y);
                //反馈回Mark点
                Para_List.Parameter.Mark2 = new Vector(Tem_Mark);
                //自增
                Counting++;
                //跳出
                if (Counting >= 20)
                {
                    MessageBox.Show("Mark2 寻找失败");
                    return;
                }
            } while (!Differ_Deviation(Cam));

            //矫正Mark3          
            Counting = 0;
            do
            {
                //定位到Mark1点
                Mark(Para_List.Parameter.Mark3);
                //调用相机，获取对比的坐标信息
                Common_Method.Delay_Time.Delay(200);//延时200ms
                //Main.T_Client
                Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation(2));//触发拍照 
                //获取坐标系平台坐标
                Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate());
                //计算偏移
                Tem_Mark = new Vector(Coodinate_Point.X + Cam.X, Coodinate_Point.Y + Cam.Y);
                //反馈回Mark点
                Para_List.Parameter.Mark3 = new Vector(Tem_Mark);
                //自增
                Counting++;
                //跳出
                if (Counting >= 20)
                {
                    MessageBox.Show("Mark3 寻找失败");
                    return;
                }
            } while (!Differ_Deviation(Cam));

            //计算仿射变换参数
            Para_List.Parameter.Trans_Affinity =new Affinity_Matrix(Gts_Cal_Data_Resolve.Cal_Affinity()); 

        }
        //判别误差范围之内
        public static bool Differ_Deviation(Vector Indata)
        {
            if ((Math.Abs(Indata.X)<=Para_List.Parameter.Pos_Tolerance) && (Math.Abs(Indata.Y) <= Para_List.Parameter.Pos_Tolerance))
            {
                return true;
            }
            else
            {
                return false;
            }
         }
        //矫正 振镜与ORG的距离
        public static void Calibrate_RTC_ORG()
        {
            //生成RTC扫圆轨迹
            List<List<Interpolation_Data>> Calibrate_Data = Generate_Org_Rtc_Data(1.0m, 2.0m);
            //执行
            Integrated.Rts_Gts(Calibrate_Data);                   
            //建立变量
            Vector Cam = new Vector();
            Vector Coodinate_Point;
            Vector Tem_Mark;
            UInt16 Counting = 0;

            ////定位到ORG矫正点
            //Mark(new Vector(Para_List.Parameter.Rtc_Org.X + 100, Para_List.Parameter.Rtc_Org.Y + 100));
            ////调用相机，获取对比的坐标信息
            //Common_Method.Delay_Time.Delay(200);//延时200ms
            ////Main.T_Client
            //Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation(2));//触发拍照 

            //矫正数据
            do
            {
                //定位到ORG矫正点
                Mark(new Vector(Para_List.Parameter.Rtc_Org.X + 100, Para_List.Parameter.Rtc_Org.Y + 100));
                //调用相机，获取对比的坐标信息
                Common_Method.Delay_Time.Delay(200);//延时200ms
                //Main.T_Client
                Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation(2));//触发拍照 
                //获取坐标系平台坐标
                Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate());
                //计算偏移
                Tem_Mark = new Vector(Coodinate_Point.X + Cam.X - 100, Coodinate_Point.Y + Cam.Y - 100);
                //反馈回RTC_ORG数据
                Para_List.Parameter.Rtc_Org = new Vector(Tem_Mark);
                //自增
                Counting++;
            } while (!Differ_Deviation(Cam) && (Counting <= 10));
            //数据矫正完成
        }
        
        //定位mark点
        public static void Mark(Vector point)
        {
            GTS_Fun.Interpolation.Gts_Ready(point.X,point.Y);
        }
        //生成RTC 与 原点距离矫正
        public static List<List<Interpolation_Data>> Generate_Org_Rtc_Data(decimal Radius, decimal Interval)
        {
            //结果变量
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();//返回值
            List<Interpolation_Data> Temp_Interpolation_List_Data = new List<Interpolation_Data>();//二级层
            Interpolation_Data Temp_Data = new Interpolation_Data();//一级层  
            decimal Gts_X = 100, Gts_Y = 100;//X、Y坐标
            //decimal Radius = 1.0m;//半径
            //decimal Interval = 3.0m;//间距  
            //初始清除
            Result.Clear();
            Temp_Interpolation_List_Data.Clear();
            Temp_Data.Empty();

            //走刀至Gts 平台坐标

            //Gts 直线插补
            Temp_Data.Type = 1;
            //强制抬刀标志：1
            Temp_Data.Lift_flag = 1;
            //强制加工类型为Gts
            Temp_Data.Work = 10;
            //直线终点坐标
            Temp_Data.End_x = Gts_X;
            Temp_Data.End_y = Gts_Y;
            //追加修改的数据
            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
            Temp_Interpolation_List_Data.Clear();

            //坐标原点 1半径的圆圈 1号圆
            //追加RTC加工数据
            //数据清空
            Temp_Data.Empty();
            //强制抬刀标志：0
            Temp_Data.Lift_flag = 0;
            //强制加工类型为RTC
            Temp_Data.Work = 20;
            //GTS平台配合坐标
            Temp_Data.Gts_x = Gts_X;
            Temp_Data.Gts_y = Gts_Y;
            //Rtc定位 激光加工起点坐标
            Temp_Data.Rtc_x = Radius;
            Temp_Data.Rtc_y = 0;
            //RTC arc_abs圆弧
            Temp_Data.Type = 11;
            //RTC 圆弧加工圆心坐标转换
            Temp_Data.Center_x = 0;
            Temp_Data.Center_y = 0;
            //圆弧角度
            Temp_Data.Angle = 360;
            //追加修改的数据
            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
            ////Temp_Interpolation_List_Data.Clear();

            //处理二次结果，合并走直线的Gts数据，下次为Rtc加工，则变动该GTS数据终点坐标为RTC加工的gts基准位置
            for (int cal = 0; cal < Result.Count; cal++)
            {
                //当前序号 数量为1、加工类型1 直线、加工方式10 GTS
                //当前+1序号 数量大于1、加工方式20 RTX
                if ((cal < Result.Count - 1) && (Result[cal].Count == 1) && (Result[cal][0].Type == 1) && (Result[cal][0].Work == 10) && (Result[cal + 1].Count >= 1) && (Result[cal + 1][0].Work == 20))
                {
                    Temp_Data.Empty();
                    Temp_Data = Result[cal][0];
                    Temp_Data.End_x = Result[cal + 1][0].Gts_x;
                    Temp_Data.End_y = Result[cal + 1][0].Gts_y;
                    //重新赋值
                    Result[cal][0] = new Interpolation_Data(Temp_Data);
                }
            }
            //返回结果
            return Result;
        }
    }
    //GTS校准数据处理
    class Gts_Cal_Data_Resolve
    {
        //0829保存数据到 .CSV
        private static void Save_Csv(Correct_Data resultdata, String newdataX, String newdataY) //type:0:分组，1：qa
        {
            string sdatetime = DateTime.Now.ToString("D");
            string filePath = "";
            string delimiter = ",";
            string strHeader = "";

            filePath = @"E:\\laser_data\\标定数据\\" + sdatetime + ".csv";
            strHeader += "相机实际X坐标,相机实际Y坐标,平台电机实际X坐标,平台电机实际Y坐标,相机X坐标,相机Y坐标";

            bool isExit = File.Exists(filePath);
            StreamWriter sw = new StreamWriter(filePath, true, Encoding.GetEncoding("gb2312"));
            if (!isExit)
            {
                sw.WriteLine(strHeader);
            }
            //output rows data
            string strRowValue = "";
            strRowValue += resultdata.Xo + delimiter
                         + resultdata.Yo + delimiter
                         + resultdata.Xm + delimiter
                         + resultdata.Ym + delimiter
                         + newdataX + delimiter
                         + newdataY;
            sw.WriteLine(strRowValue);
            sw.Close();
        }
        //8.28
        //dxf 仿射变换 求DX，DY，Dct(sin \cos)
        public static Affinity_Matrix Resolve_DxfToM(PointF[] srcTri, PointF[] dstTri)//标准数据 //差异化数据
        {
            //建立变量
            Affinity_Matrix Temp_Affinity_Matrix = new Affinity_Matrix();
            //定义仿射变换数组 
            Mat mat = new Mat(new Size(3, 2), Emgu.CV.CvEnum.DepthType.Cv32F, 1); //2行 3列 的矩阵

            double[] temp_array;
            //序列化
            Serialize_Data Save_Data = new Serialize_Data();

            //计算仿射变换矩阵
            mat = CvInvoke.GetAffineTransform(srcTri, dstTri);
            //提取矩阵数据
            temp_array = mat.GetDoubleArray();
            //获取仿射变换参数
            Temp_Affinity_Matrix.Cos_Value = Convert.ToDecimal(temp_array[0]);//余弦值
            Temp_Affinity_Matrix.Sin_Value = Convert.ToDecimal(temp_array[1]);//正弦值
            Temp_Affinity_Matrix.Delta_X = Convert.ToDecimal(temp_array[2]);//x方向偏移
            Temp_Affinity_Matrix.Delta_Y = Convert.ToDecimal(temp_array[5]);//y方向偏移
            //追加进入仿射变换List
            //保存为文件
            Save_Data.Serialize_Affinity_Matrix_dxf(Temp_Affinity_Matrix, "Affinity_Matrix_DxfToM.txt");
            return Temp_Affinity_Matrix;
        }
        //处理相机与轴的数据，生成仿射变换数组，并保存进入文件
        public static List<Affinity_Matrix> Resolve(List<Correct_Data> correct_Datas)
        {

            //建立变量
            List<Affinity_Matrix> Result = new List<Affinity_Matrix>();
            Affinity_Matrix Temp_Affinity_Matrix = new Affinity_Matrix();
            List<Double_Fit_Data> Line_Fit_Data = new List<Double_Fit_Data>();
            Double_Fit_Data Temp_Fit_Data = new Double_Fit_Data();
            Int16 i, j;
            //定义仿射变换数组 
            Mat mat = new Mat(new Size(3, 2), Emgu.CV.CvEnum.DepthType.Cv32F, 1); //2行 3列 的矩阵
            //定义仿射变换矩阵转换数组
            double[] temp_array;
            //拟合高阶次数
            short Line_Re = 4;
            //数据处理
            if (Para_List.Parameter.Gts_Calibration_Col * Para_List.Parameter.Gts_Calibration_Row == correct_Datas.Count)//矫正和差异数据完整
            {
                if (Para_List.Parameter.Gts_Affinity_Type == 1)//全部点对
                {
                    //定义点位数组 
                    PointF[] srcTri = new PointF[Para_List.Parameter.Gts_Calibration_Col * Para_List.Parameter.Gts_Calibration_Row];//标准数据
                    PointF[] dstTri = new PointF[Para_List.Parameter.Gts_Calibration_Col * Para_List.Parameter.Gts_Calibration_Row];//差异化数据
                    //所有点对
                    for (i = 0; i < correct_Datas.Count; i++)
                    {
                        srcTri[i] = new PointF((float)correct_Datas[i].Xo, (float)correct_Datas[i].Yo);
                        dstTri[i] = new PointF((float)correct_Datas[i].Xm, (float)correct_Datas[i].Ym);
                    }
                    mat = CvInvoke.EstimateRigidTransform(srcTri, dstTri, false);
                    //提取矩阵数据
                    temp_array = mat.GetDoubleArray();
                    //获取仿射变换参数
                    Temp_Affinity_Matrix = Array_To_Affinity(temp_array);
                    //追加进入仿射变换List
                    Result.Add(new Affinity_Matrix(Temp_Affinity_Matrix));
                    //清除变量
                    Temp_Affinity_Matrix.Empty();
                    //保存为文件
                    Serialize_Data.Serialize_Affinity_Matrix(Result, "Gts_Affinity_Matrix_All.xml");
                }
                else if (Para_List.Parameter.Gts_Affinity_Type == 2)//线性拟合
                {
                    //初始化数据
                    double[] src_x = new double[Para_List.Parameter.Gts_Calibration_Col];
                    double[] dst_x = new double[Para_List.Parameter.Gts_Calibration_Col];
                    Tuple<double, double> R_X = new Tuple<double, double>(0, 0);
                    double[] src_y = new double[Para_List.Parameter.Gts_Calibration_Row];
                    double[] dst_y = new double[Para_List.Parameter.Gts_Calibration_Row];
                    Tuple<double, double> R_Y = new Tuple<double, double>(0, 0); 
                    //拟合数据
                    for (i = 0; i < Para_List.Parameter.Gts_Calibration_Col; i++)
                    {
                        for (j = 0; j < Para_List.Parameter.Gts_Calibration_Row; j++)
                        {
                            //提取X轴拟合数据
                            src_x[j] = (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xo);
                            dst_x[j] = (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xm);
                            //提取Y轴拟合数据
                            src_y[j] = (float)(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col].Yo);
                            dst_y[j] = (float)(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col].Ym);
                            
                        }
                        //高阶曲线拟合
                        if (Line_Re >= 2)
                        {
                            double[] Res_x = Fit.Polynomial(src_x, dst_x, Line_Re);
                            double[] Res_y = Fit.Polynomial(src_y, dst_y, Line_Re);
                            //提取拟合直线数据
                            Temp_Fit_Data = new Double_Fit_Data
                            {
                                K_X4 = (decimal)Res_x[4],
                                K_X3 = (decimal)Res_x[3],
                                K_X2 = (decimal)Res_x[2],
                                K_X1 = (decimal)Res_x[1],
                                Delta_X = (decimal)Res_x[0],
                                K_Y4 = (decimal)Res_y[4],
                                K_Y3 = (decimal)Res_y[3],
                                K_Y2 = (decimal)Res_y[2],
                                K_Y1 = (decimal)Res_y[1],
                                Delta_Y = (decimal)Res_y[0]
                            };
                        }
                        else
                        {
                            R_X = Fit.Line(src_x, dst_x);
                            R_Y = Fit.Line(src_y, dst_y);
                            //提取拟合直线数据
                            Temp_Fit_Data = new Double_Fit_Data
                            {
                                K_X1 = (decimal)R_X.Item2,
                                Delta_X = (decimal)R_X.Item1,
                                K_Y1 = (decimal)R_Y.Item2,
                                Delta_Y = (decimal)R_Y.Item1
                            };
                        }                        
                        //保存进入Line_Fit_Data
                        Line_Fit_Data.Add(new Double_Fit_Data(Temp_Fit_Data));
                        //清空数据
                        Temp_Fit_Data.Empty();
                    }
                    //保存轴拟合数据
                    CSV_RW.SaveCSV(CSV_RW.Double_Fit_Data_DataTable(Line_Fit_Data), "Gts_Line_Fit_Data");
                }
                else
                {
                    //定义点位数组 
                    PointF[] srcTri = new PointF[3];//标准数据
                    PointF[] dstTri = new PointF[3];//差异化数据
                    //数据处理
                    for (i = 0; i < Para_List.Parameter.Gts_Calibration_Col - 1; i++)
                    {
                        for (j = 0; j < Para_List.Parameter.Gts_Calibration_Row - 1; j++)
                        {

                            //标准数据  平台坐标
                            srcTri[0] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Ym));
                            srcTri[1] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Ym));
                            srcTri[2] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row + 1].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row + 1].Ym));
                            //srcTri[3] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + 1].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + 1].Ym));

                            //仿射数据  测量坐标
                            dstTri[0] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Yo));
                            dstTri[1] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Yo));
                            dstTri[2] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row + 1].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row + 1].Yo));
                            //dstTri[3] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + 1].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + 1].Yo));

                            //计算仿射变换矩阵
                            mat = CvInvoke.GetAffineTransform(srcTri, dstTri);
                            //mat = CvInvoke.EstimateRigidTransform(srcTri, dstTri, false);
                            //提取矩阵数据
                            temp_array = mat.GetDoubleArray();
                            //获取仿射变换参数
                            Temp_Affinity_Matrix = Array_To_Affinity(temp_array);
                            //追加进入仿射变换List
                            Result.Add(new Affinity_Matrix(Temp_Affinity_Matrix));
                            //清除变量
                            Temp_Affinity_Matrix.Empty();
                        }
                    }
                    //保存为文件
                    Serialize_Data.Serialize_Affinity_Matrix(Result, "Gts_Affinity_Matrix_Three.xml");
                }   
            }
            return Result;

        }
        public static Affinity_Matrix Array_To_Affinity(double[] temp_array)
        {
            Affinity_Matrix Result = new Affinity_Matrix();
            //获取仿射变换参数
            Result.Cos_Value = Convert.ToDecimal(temp_array[0]);//余弦值
            //范围限制
            if (Result.Cos_Value > 1.0m)
            {
                Result.Cos_Value = 1.0m;
            }
            else if (Result.Cos_Value < -1.0m)
            {
                Result.Cos_Value = -1.0m;
            }
            //else if (Math.Abs(Result.Cos_Value) < 0.002m)
            //{
            //    Result.Cos_Value = 0.0m;
            //}
            Result.Sin_Value = Convert.ToDecimal(temp_array[1]);//正弦值
            //范围限制
            if (Result.Sin_Value > 1.0m)
            {
                Result.Sin_Value = 1.0m;
            }
            else if (Result.Sin_Value < -1.0m)
            {
                Result.Sin_Value = -1.0m;
            }
            //else if (Math.Abs(Result.Sin_Value) < 0.002m)
            //{
            //    Result.Sin_Value = 0.0m;
            //}
            Result.Delta_X = Convert.ToDecimal(temp_array[2]);//x方向偏移
            Result.Delta_Y = Convert.ToDecimal(temp_array[5]);//y方向偏移
            //返回结果
            return Result;
        }
        //定位坐标 X 
        public static Int16 Seek_X_Pos(decimal Pos)
        {
            Int16 Result = 0;
            Result = (Int16)Math.Floor((Pos / Para_List.Parameter.Gts_Calibration_Cell) + 0.01m);
            if (Result >= Para_List.Parameter.Gts_Affinity_Row)
            {
                Result = (Int16)(Para_List.Parameter.Gts_Affinity_Row - 1);
            }
            else if (Result <= 0)
            {
                Result = 0;
            }
            return Result;
        }
        //定位坐标 Y
        public static Int16 Seek_Y_Pos(decimal Pos)
        {
            Int16 Result = 0;
            Result = (Int16)Math.Floor((Pos / Para_List.Parameter.Gts_Calibration_Cell) + 0.01m);
            if (Result >= Para_List.Parameter.Gts_Affinity_Col)
            {
                Result = (Int16)(Para_List.Parameter.Gts_Affinity_Col - 1);
            }
            else if (Result <= 0)
            {
                Result = 0;
            }
            return Result;
        }
        //dxf 仿射变换 求DX，DY，Dct(sin \cos)
        public static Affinity_Matrix Cal_Affinity()//标准数据 //差异化数据 
        {
            //建立变量
            Affinity_Matrix Result = new Affinity_Matrix();
            //定义仿射变换数组 
            Mat mat = new Mat(new Size(3, 2), Emgu.CV.CvEnum.DepthType.Cv32F, 1); //2行 3列 的矩阵
            //定义点位数组
            PointF[] srcTri = new PointF[3];//标准数据
            PointF[] dstTri = new PointF[3];//差异化数据 
            double[] temp_array;
            //数据提取
            //标准数据
            srcTri[0] = new PointF((float)(Para_List.Parameter.Mark_Dxf1.X), (float)(Para_List.Parameter.Mark_Dxf1.Y));
            srcTri[1] = new PointF((float)(Para_List.Parameter.Mark_Dxf2.X), (float)(Para_List.Parameter.Mark_Dxf2.Y));
            srcTri[2] = new PointF((float)(Para_List.Parameter.Mark_Dxf3.X), (float)(Para_List.Parameter.Mark_Dxf3.Y));
            //仿射数据
            dstTri[0] = new PointF((float)(Para_List.Parameter.Mark1.X), (float)(Para_List.Parameter.Mark1.Y));
            dstTri[1] = new PointF((float)(Para_List.Parameter.Mark2.X), (float)(Para_List.Parameter.Mark2.Y));
            dstTri[2] = new PointF((float)(Para_List.Parameter.Mark3.X), (float)(Para_List.Parameter.Mark3.Y));
            //计算仿射变换矩阵
            mat = CvInvoke.GetAffineTransform(srcTri, dstTri);
            //提取矩阵数据
            temp_array = mat.GetDoubleArray();
            //获取仿射变换参数
            Result = Array_To_Affinity(temp_array);
            //保存为文件
            string sdatetime = DateTime.Now.ToString("D");
            string filePath = "";
            string delimiter = ",";
            string strHeader = "";
            filePath = @"./\Config/" + "Cal_Affinity.csv";

            strHeader += "Cos,Sin,Delatx,Deltay";
            bool isExit = File.Exists(filePath);
            StreamWriter sw = new StreamWriter(filePath, true, Encoding.GetEncoding("gb2312"));
            if (!isExit)
            {
                sw.WriteLine(strHeader);
            }
            //output rows data
            string strRowValue = "";
            strRowValue += Result.Cos_Value + delimiter
                         + Result.Sin_Value + delimiter
                         + Result.Delta_X + delimiter
                         + Result.Delta_Y;
            sw.WriteLine(strRowValue);
            sw.Close();

            //追加进入仿射变换List
            return Result;
        }
        //获取线性补偿坐标
        public static Vector Get_Line_Fit_Coordinate(Decimal x,decimal y,List<Double_Fit_Data> line_fit_data)  
        {
            //临时定位变量
            Int16 m, n;
            decimal X_per, Y_per;
            decimal K_x1, K_x2, K_x3, K_x4, B_x;
            decimal K_y1, K_y2, K_y3, K_y4, B_y;
            //获取落点
            m = Seek_X_Pos(x);
            n = Seek_Y_Pos(y);
            //计算比率
            X_per = Math.Abs(x - m * Para_List.Parameter.Gts_Calibration_Cell) / Para_List.Parameter.Gts_Calibration_Cell;
            Y_per = Math.Abs(y - m * Para_List.Parameter.Gts_Calibration_Cell) / Para_List.Parameter.Gts_Calibration_Cell;
            //计算实际 线性拟合数据
            K_x1 = (line_fit_data[m + 1].K_X1 - line_fit_data[m].K_X1) * X_per + line_fit_data[m].K_X1;
            K_x2 = (line_fit_data[m + 1].K_X2 - line_fit_data[m].K_X2) * X_per + line_fit_data[m].K_X2;
            K_x3 = (line_fit_data[m + 1].K_X3 - line_fit_data[m].K_X3) * X_per + line_fit_data[m].K_X3;
            K_x4 = (line_fit_data[m + 1].K_X4 - line_fit_data[m].K_X4) * X_per + line_fit_data[m].K_X4;
            B_x = (line_fit_data[m + 1].Delta_X - line_fit_data[m].Delta_X) * X_per + line_fit_data[m].Delta_X;
            K_y1 = (line_fit_data[m + 1].K_Y1 - line_fit_data[m].K_Y1) * Y_per + line_fit_data[m].K_Y1;
            K_y2 = (line_fit_data[m + 1].K_Y2 - line_fit_data[m].K_Y2) * Y_per + line_fit_data[m].K_Y2;
            K_y3 = (line_fit_data[m + 1].K_Y3 - line_fit_data[m].K_Y3) * Y_per + line_fit_data[m].K_Y3;
            K_y4 = (line_fit_data[m + 1].K_Y4 - line_fit_data[m].K_Y4) * Y_per + line_fit_data[m].K_Y4;
            B_y = (line_fit_data[m + 1].Delta_Y - line_fit_data[m].Delta_Y) * Y_per + line_fit_data[m].Delta_Y;
            //返回实际坐标
            return new Vector(K_x4 * x * x * x * x + K_x3 * x * x * x + K_x2 * x * x + K_x1 * x + B_x, K_y4 * y * y * y * y + K_y3 * y * y * y + K_y2 * y * y + K_y1 * y + B_y);
        }
    }
    //RTC校准数据处理
    class Rtc_Cal_Data_Resolve
    {        
        //处理测量值与实际值 数据，生成仿射变换数组，并保存进入文件
        public static List<Affinity_Matrix> Resolve(List<Correct_Data> correct_Datas)
        {

            //建立变量
            List<Affinity_Matrix> Result = new List<Affinity_Matrix>();
            Affinity_Matrix Temp_Affinity_Matrix = new Affinity_Matrix();
            List<Double_Fit_Data> Line_Fit_Data = new List<Double_Fit_Data>();
            Double_Fit_Data Temp_Fit_Data = new Double_Fit_Data();
            Int16 i, j;
            //定义仿射变换数组 
            Mat mat = new Mat(new Size(3, 2), Emgu.CV.CvEnum.DepthType.Cv32F, 1); //2行 3列 的矩阵
            //定义仿射变换矩阵转换数组
            double[] temp_array;
            //拟合高阶次数
            short Line_Re = 4;
            //数据处理
            if (Para_List.Parameter.Rtc_Calibration_Col * Para_List.Parameter.Rtc_Calibration_Row == correct_Datas.Count)//矫正和差异数据完整
            {
                if (Para_List.Parameter.Rtc_Affinity_Type == 1)//全部点对
                {
                    //定义点位数组 
                    PointF[] srcTri = new PointF[Para_List.Parameter.Rtc_Calibration_Col * Para_List.Parameter.Rtc_Calibration_Row];//标准数据
                    PointF[] dstTri = new PointF[Para_List.Parameter.Rtc_Calibration_Col * Para_List.Parameter.Rtc_Calibration_Row];//差异化数据
                    //所有点对
                    for (i = 0; i < correct_Datas.Count; i++)
                    {
                        srcTri[i] = new PointF((float)correct_Datas[i].Xo, (float)correct_Datas[i].Yo);
                        dstTri[i] = new PointF((float)correct_Datas[i].Xm, (float)correct_Datas[i].Ym);
                    }
                    mat = CvInvoke.EstimateRigidTransform(srcTri, dstTri, false);
                    //提取矩阵数据
                    temp_array = mat.GetDoubleArray();
                    //获取仿射变换参数
                    Temp_Affinity_Matrix = Gts_Cal_Data_Resolve.Array_To_Affinity(temp_array);
                    //追加进入仿射变换List
                    Result.Add(new Affinity_Matrix(Temp_Affinity_Matrix));
                    //清除变量
                    Temp_Affinity_Matrix.Empty();
                    //保存为文件
                    Serialize_Data.Serialize_Affinity_Matrix(Result, "Rtc_Affinity_Matrix_All.xml");
                }
                else if (Para_List.Parameter.Rtc_Affinity_Type == 2)//线性拟合
                {
                    //初始化数据
                    double[] src_x = new double[Para_List.Parameter.Rtc_Calibration_Col];
                    double[] dst_x = new double[Para_List.Parameter.Rtc_Calibration_Col];
                    Tuple<double, double> R_X = new Tuple<double, double>(0, 0);
                    double[] src_y = new double[Para_List.Parameter.Rtc_Calibration_Row];
                    double[] dst_y = new double[Para_List.Parameter.Rtc_Calibration_Row];
                    Tuple<double, double> R_Y = new Tuple<double, double>(0, 0);
                    //拟合数据
                    for (i = 0; i < Para_List.Parameter.Rtc_Calibration_Col; i++)
                    {
                        for (j = 0; j < Para_List.Parameter.Rtc_Calibration_Row; j++)
                        {
                            //提取X轴拟合数据
                            src_x[j] = (float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col].Xo);
                            dst_x[j] = (float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col].Xm);
                            //提取Y轴拟合数据
                            src_y[j] = (float)(correct_Datas[i + j * Para_List.Parameter.Rtc_Calibration_Col].Yo);
                            dst_y[j] = (float)(correct_Datas[i + j * Para_List.Parameter.Rtc_Calibration_Col].Ym);
                            
                        }
                        //高阶曲线拟合
                        if (Line_Re >= 2)
                        {
                            double[] Res_x = Fit.Polynomial(src_x, dst_x, Line_Re);
                            double[] Res_y = Fit.Polynomial(src_y, dst_y, Line_Re);
                            //提取拟合直线数据
                            Temp_Fit_Data = new Double_Fit_Data
                            {
                                K_X4 = (decimal)Res_x[4],
                                K_X3 = (decimal)Res_x[3],
                                K_X2 = (decimal)Res_x[2],
                                K_X1 = (decimal)Res_x[1],
                                Delta_X = (decimal)Res_x[0],
                                K_Y4 = (decimal)Res_y[4],
                                K_Y3 = (decimal)Res_y[3],
                                K_Y2 = (decimal)Res_y[2],
                                K_Y1 = (decimal)Res_y[1],
                                Delta_Y = (decimal)Res_y[0]
                            };
                        }
                        else//1阶线性拟合
                        {
                            R_X = Fit.Line(src_x, dst_x);
                            R_Y = Fit.Line(src_y, dst_y);
                            //提取拟合直线数据
                            Temp_Fit_Data = new Double_Fit_Data
                            {
                                K_X1 = (decimal)R_X.Item2,
                                Delta_X = (decimal)R_X.Item1,
                                K_Y1 = (decimal)R_Y.Item2,
                                Delta_Y = (decimal)R_Y.Item1
                            };
                        }
                        //保存进入Line_Fit_Data
                        Line_Fit_Data.Add(new Double_Fit_Data(Temp_Fit_Data));
                        //清空数据
                        Temp_Fit_Data.Empty();
                    }
                    //保存轴拟合数据
                    CSV_RW.SaveCSV(CSV_RW.Double_Fit_Data_DataTable(Line_Fit_Data), "Rtc_Line_Fit_Data");
                }
                else
                {
                    //定义点位数组 
                    PointF[] srcTri = new PointF[4];//标准数据
                    PointF[] dstTri = new PointF[4];//差异化数据
                    //数据处理
                    for (i = 0; i < Para_List.Parameter.Rtc_Calibration_Col - 1; i++)
                    {
                        for (j = 0; j < Para_List.Parameter.Rtc_Calibration_Row - 1; j++)
                        {
                            //标准数据  定位坐标
                            srcTri[0] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col].Ym));
                            srcTri[1] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row].Ym));
                            srcTri[2] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row + 1].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row + 1].Ym));//计算仿射变换矩阵

                            //仿射数据  测量坐标
                            dstTri[0] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col].Yo));
                            dstTri[1] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row].Yo));
                            dstTri[2] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row + 1].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row + 1].Yo));

                            //计算仿射变换矩阵
                            //mat = CvInvoke.GetAffineTransform(srcTri, dstTri);
                            mat = CvInvoke.EstimateRigidTransform(srcTri, dstTri, false);
                            //提取矩阵数据
                            temp_array = mat.GetDoubleArray();
                            //获取仿射变换参数
                            Temp_Affinity_Matrix = Gts_Cal_Data_Resolve.Array_To_Affinity(temp_array);
                            //追加进入仿射变换List
                            Result.Add(new Affinity_Matrix(Temp_Affinity_Matrix));
                            //清除变量
                            Temp_Affinity_Matrix.Empty();
                        }
                    }
                    //保存为文件
                    Serialize_Data.Serialize_Affinity_Matrix(Result, "Rtc_Affinity_Matrix_Three.xml");
                }
            }
            return Result;
        }
        //定位坐标 X
        public static Int16 Seek_X_Pos(decimal Pos)
        {
            Int16 Result = 0;
            Result = (Int16)Math.Floor(((Pos + 25 ) / Para_List.Parameter.Rtc_Calibration_Cell) + 0.01m);
            if (Result >= Para_List.Parameter.Rtc_Affinity_Row)
            {
                Result = (Int16)(Para_List.Parameter.Rtc_Affinity_Row - 1);
            }
            else if (Result <= 0)
            {
                Result = 0;
            }
            return Result;
        }
        //定位坐标 Y
        public static Int16 Seek_Y_Pos(decimal Pos)
        {
            Int16 Result = 0;
            Result = (Int16)Math.Floor(((Pos + 25) / Para_List.Parameter.Rtc_Calibration_Cell) + 0.01m);
            if (Result >= Para_List.Parameter.Rtc_Affinity_Col)
            {
                Result = (Int16)(Para_List.Parameter.Rtc_Affinity_Col - 1);
            }
            else if (Result <= 0)
            {
                Result = 0;
            }
            return Result;
        }
        public static Vector Get_Line_Fit_Coordinate(Decimal x, decimal y, List<Double_Fit_Data> line_fit_data)
        {
            //临时定位变量
            Int16 m, n;
            decimal X_per, Y_per;
            decimal K_x1, K_x2, K_x3, K_x4, B_x;
            decimal K_y1, K_y2, K_y3, K_y4, B_y; 
            //获取落点
            m = Seek_X_Pos(x);
            n = Seek_Y_Pos(y);
            //计算比率
            X_per = Math.Abs(x - m * Para_List.Parameter.Rtc_Calibration_Cell) / Para_List.Parameter.Rtc_Calibration_Cell;
            Y_per = Math.Abs(y - m * Para_List.Parameter.Rtc_Calibration_Cell) / Para_List.Parameter.Rtc_Calibration_Cell;
            //计算实际 线性拟合数据
            K_x1 = (line_fit_data[m + 1].K_X1 - line_fit_data[m].K_X1) * X_per + line_fit_data[m].K_X1;
            K_x2 = (line_fit_data[m + 1].K_X2 - line_fit_data[m].K_X2) * X_per + line_fit_data[m].K_X2;
            K_x3 = (line_fit_data[m + 1].K_X3 - line_fit_data[m].K_X3) * X_per + line_fit_data[m].K_X3;
            K_x4 = (line_fit_data[m + 1].K_X4 - line_fit_data[m].K_X4) * X_per + line_fit_data[m].K_X4;
            B_x = (line_fit_data[m + 1].Delta_X - line_fit_data[m].Delta_X) * X_per + line_fit_data[m].Delta_X;
            K_y1 = (line_fit_data[m + 1].K_Y1 - line_fit_data[m].K_Y1) * Y_per + line_fit_data[m].K_Y1;
            K_y2 = (line_fit_data[m + 1].K_Y2 - line_fit_data[m].K_Y2) * Y_per + line_fit_data[m].K_Y2;
            K_y3 = (line_fit_data[m + 1].K_Y3 - line_fit_data[m].K_Y3) * Y_per + line_fit_data[m].K_Y3;
            K_y4 = (line_fit_data[m + 1].K_Y4 - line_fit_data[m].K_Y4) * Y_per + line_fit_data[m].K_Y4;
            B_y = (line_fit_data[m + 1].Delta_Y - line_fit_data[m].Delta_Y) * Y_per + line_fit_data[m].Delta_Y;
            //返回实际坐标
            return new Vector(K_x4 * x * x * x * x + K_x3 * x * x * x + K_x2 * x * x + K_x1 * x + B_x, K_y4 * y * y * y * y + K_y3 * y * y * y + K_y2 * y * y + K_y1 * y + B_y);
        }
    }
    class Laser_Watt_Cal
    {
        //生成激光 百分比 与 功率的对应关系
        public static List<Fit_Data> Resolve(DataTable New_Data)
        {

            //建立变量
            List<Fit_Data> Result = new List<Fit_Data>();
            Fit_Data Temp_Fit_Data = new Fit_Data();
            Int16 i;
            //拟合高阶次数
            short Line_Re = 4;
            //初始化数据
            double[] src = new double[New_Data.Rows.Count];
            double[] dst = new double[New_Data.Rows.Count];
            Tuple<double, double> R = new Tuple<double, double>(0, 0);
            //数据处理
            for (i = 0; i < New_Data.Rows.Count; i++)
            {                
                if ((decimal.TryParse(New_Data.Rows[i][0].ToString(), out decimal X0 )) && (decimal.TryParse(New_Data.Rows[i][1].ToString(), out decimal X1)))
                {
                    src[i] = (float)X0;
                    dst[i] = (float)X1;
                }                
            }
            //高阶曲线拟合
            if (Line_Re >= 2)
            {
                double[] Res= Fit.Polynomial(src, dst, Line_Re);
                //提取拟合直线数据
                Temp_Fit_Data = new Fit_Data
                {
                    K4 = (decimal)Res[4],
                    K3 = (decimal)Res[3],
                    K2 = (decimal)Res[2],
                    K1 = (decimal)Res[1],
                    Delta = (decimal)Res[0]
                };
            }
            else//1阶线性拟合
            {
                //拟合
                R = Fit.Line(src, dst);
                //提取拟合直线数据
                Temp_Fit_Data = new Fit_Data
                {
                    K1 = (decimal)R.Item2,
                    Delta = (decimal)R.Item1
                };
            }
            //结果追加
            Result.Add(new Fit_Data(Temp_Fit_Data));
            //清空数据
            Temp_Fit_Data.Empty();
            //保存功率矫正拟合数据
            CSV_RW.SaveCSV(CSV_RW.Fit_Data_DataTable(Result), "Laser_Watt_Fit_Data");
            //返回结果
            return Result;
        }
    }
    //坐标转换 直接对从DXf的Arc、Circle、Line的坐标信息进行处理，返回对应的坐标信息，后续直接使用
    public class Transform
    {
        //Entity数据提取完成后，先进行实际坐标转换处理
        public List<Entity_Data> Resolve(List<Entity_Data> entity_Datas,List<Affinity_Matrix> affinity_Matrices)
        {
            //建立变量 
            List<Entity_Data> Result = new List<Entity_Data>();
            Entity_Data Temp_Data = new Entity_Data();
            //临时定位变量
            Int16 Start_m, Start_n,End_m, End_n, Center_m, Center_n; 
            foreach (var O in entity_Datas)
            {
                //先清空
                Temp_Data.Empty();
                //后赋值                
                Temp_Data = O;
                //获取坐标坐落区域
                Start_m = Convert.ToInt16((O.Start_x + Para_List.Parameter.Cam_Rtc.X) / Para_List.Parameter.Gts_Calibration_Cell);
                Start_n = Convert.ToInt16((O.Start_y + Para_List.Parameter.Cam_Rtc.Y) / Para_List.Parameter.Gts_Calibration_Cell);
                End_m = Convert.ToInt16((O.End_x + Para_List.Parameter.Cam_Rtc.X) / Para_List.Parameter.Gts_Calibration_Cell);
                End_n = Convert.ToInt16((O.End_y + Para_List.Parameter.Cam_Rtc.Y) / Para_List.Parameter.Gts_Calibration_Cell); 
                Center_m = Convert.ToInt16((O.Center_x + Para_List.Parameter.Cam_Rtc.X) / Para_List.Parameter.Gts_Calibration_Cell);
                Center_n = Convert.ToInt16((O.Center_y + Para_List.Parameter.Cam_Rtc.Y) / Para_List.Parameter.Gts_Calibration_Cell);
                //起点计算
                Temp_Data.Start_x = O.Start_x * affinity_Matrices[Start_m * Para_List.Parameter.Gts_Affinity_Col + Start_n].Cos_Value + O.Start_y * affinity_Matrices[Start_m * Para_List.Parameter.Gts_Affinity_Col + Start_n].Sin_Value + affinity_Matrices[Start_m * Para_List.Parameter.Gts_Affinity_Col + Start_n].Delta_X;
                Temp_Data.Start_y = O.Start_y * affinity_Matrices[Start_m * Para_List.Parameter.Gts_Affinity_Col + Start_n].Cos_Value - O.Start_x * affinity_Matrices[Start_m * Para_List.Parameter.Gts_Affinity_Col + Start_n].Sin_Value + affinity_Matrices[Start_n * Para_List.Parameter.Gts_Affinity_Col + Start_n].Delta_Y;
                //终点计算
                Temp_Data.End_x = O.End_x * affinity_Matrices[End_m * Para_List.Parameter.Gts_Affinity_Col + End_n].Cos_Value + O.End_y * affinity_Matrices[End_m * Para_List.Parameter.Gts_Affinity_Col + End_n].Sin_Value + affinity_Matrices[End_m * Para_List.Parameter.Gts_Affinity_Col + End_n].Delta_X;
                Temp_Data.End_y = O.End_y * affinity_Matrices[End_m * Para_List.Parameter.Gts_Affinity_Col + End_n].Cos_Value - O.End_x * affinity_Matrices[End_m * Para_List.Parameter.Gts_Affinity_Col + End_n].Sin_Value + affinity_Matrices[End_n * Para_List.Parameter.Gts_Affinity_Col + End_n].Delta_Y;
                //圆心计算
                Temp_Data.Center_x = O.Center_x * affinity_Matrices[Center_m * Para_List.Parameter.Gts_Affinity_Col + Center_n].Cos_Value + O.Center_y * affinity_Matrices[Center_m * Para_List.Parameter.Gts_Affinity_Col + Center_n].Sin_Value + affinity_Matrices[Center_m * Para_List.Parameter.Gts_Affinity_Col + Center_n].Delta_X;
                Temp_Data.Center_y = O.Center_y * affinity_Matrices[Center_m * Para_List.Parameter.Gts_Affinity_Col + Center_n].Cos_Value - O.Center_x * affinity_Matrices[Center_m * Para_List.Parameter.Gts_Affinity_Col + Center_n].Sin_Value + affinity_Matrices[Center_n * Para_List.Parameter.Gts_Affinity_Col + Center_n].Delta_Y;

                //追加数据至Result
                Result.Add(Temp_Data);
                //清空Temp_Data
                Temp_Data.Empty();

            }
            return Result;
        }
    }    
    public class Point_Resolve
    {       
        //计算Gts坐标  对应的 实际坐标  逆仿射变换
        public Vector Gts_To_Platom(decimal x,decimal y, Affinity_Rate Rate)
        {
            Vector Result=new Vector();
            //系数计算
            decimal Arc = Convert.ToDecimal(Math.PI) * (Rate.Angle / 180.0m);
            decimal Cos_Arc = Convert.ToDecimal(Math.Cos(Convert.ToDouble(Arc)));
            decimal Sin_Arc = Convert.ToDecimal(Math.Sin(Convert.ToDouble(Arc)));
            //计算结果
            Result.X = x * Cos_Arc + y * Sin_Arc + Rate.Delta_X;
            Result.Y = y * Cos_Arc - x * Sin_Arc + Rate.Delta_Y;

            return Result;
        }
        public Vector Gts_To_Platom(decimal x, decimal y)
        {
            Vector Result = new Vector();
            //读取矫正表
            List<Affinity_Matrix> Affinity_Mat = Serialize_Data.Reserialize_Affinity_Matrix("Affinity_Matrix.xml");
            //获取坐标坐落区域
            Int16 Pos_Row = Convert.ToInt16(x / Para_List.Parameter.Gts_Calibration_Cell);
            Int16 Pos_Col = Convert.ToInt16(y / Para_List.Parameter.Gts_Calibration_Cell);
            //数据计算  说明：因为是负的角度，故Sin值变为负值
            Result.X = x * Affinity_Mat[Pos_Row * Para_List.Parameter.Gts_Affinity_Col + Pos_Col].Cos_Value + y * Affinity_Mat[Pos_Row * Para_List.Parameter.Gts_Affinity_Col + Pos_Col].Sin_Value + Affinity_Mat[Pos_Row * Para_List.Parameter.Gts_Affinity_Col + Pos_Col].Delta_X;
            Result.Y = y * Affinity_Mat[Pos_Row * Para_List.Parameter.Gts_Affinity_Col + Pos_Col].Cos_Value - x * Affinity_Mat[Pos_Row * Para_List.Parameter.Gts_Affinity_Col + Pos_Col].Sin_Value + Affinity_Mat[Pos_Col * Para_List.Parameter.Gts_Affinity_Col + Pos_Col].Delta_Y;

            return Result;
        }
        //计算实际坐标 对应的 Gts坐标  仿射变换
        public Vector Platom_To_Gts(decimal x, decimal y, Affinity_Rate Rate)
        {
            Vector Result = new Vector();
            //系数计算
            decimal Arc = Convert.ToDecimal(Math.PI) * (Rate.Angle / 180.0m);
            decimal Cos_Arc = Convert.ToDecimal(Math.Cos(Convert.ToDouble(Arc)));
            decimal Sin_Arc = Convert.ToDecimal(Math.Sin(Convert.ToDouble(Arc)));
            //计算结果
            Result.X = x * Cos_Arc - y * Sin_Arc + Rate.Delta_X;
            Result.Y = y * Cos_Arc + x * Sin_Arc + Rate.Delta_Y;

            return Result;
        }
        public Vector Platom_To_Gts(decimal x, decimal y) 
        {
            Vector Result = new Vector();
            //读取矫正表
            List<Affinity_Matrix> Affinity_Mat = Serialize_Data.Reserialize_Affinity_Matrix("Affinity_Matrix.xml");
            //获取坐标坐落区域
            Int16 Pos_Row = Convert.ToInt16(x / Para_List.Parameter.Gts_Calibration_Cell);
            Int16 Pos_Col = Convert.ToInt16(y / Para_List.Parameter.Gts_Calibration_Cell);
            //数据计算  
            Result.X = x * Affinity_Mat[Pos_Row * Para_List.Parameter.Gts_Affinity_Col + Pos_Col].Cos_Value - y * Affinity_Mat[Pos_Row * Para_List.Parameter.Gts_Affinity_Col + Pos_Col].Sin_Value + Affinity_Mat[Pos_Row * Para_List.Parameter.Gts_Affinity_Col + Pos_Col].Delta_X;
            Result.Y = y * Affinity_Mat[Pos_Row * Para_List.Parameter.Gts_Affinity_Col + Pos_Col].Cos_Value + x * Affinity_Mat[Pos_Row * Para_List.Parameter.Gts_Affinity_Col + Pos_Col].Sin_Value + Affinity_Mat[Pos_Col * Para_List.Parameter.Gts_Affinity_Col + Pos_Col].Delta_Y;

            return Result;
        }
    }
    public class Serialize_Data 
    {
       
        //矫正数据序列化操作
        public static void Serialize_Correct_Data(List<Correct_Data> list,string txtFile)
        {
            //写入文件
            string File_Path = @"./\Config/" + txtFile;
            using (FileStream fs = new FileStream(File_Path, FileMode.Create,FileAccess.ReadWrite))
            {
                //二进制 序列化
                //BinaryFormatter bf = new BinaryFormatter();
                //xml 序列化
                XmlSerializer bf = new XmlSerializer(typeof(List<Correct_Data>));
                bf.Serialize(fs, list);
            }
        }

        //矫正数据反序列化
        public static List<Correct_Data> Reserialize_Correct_Data (string fileName)
        {            

            //读取文件
            string File_Path = @"./\Config/" + fileName;
            using (FileStream fs = new FileStream(File_Path, FileMode.Open,FileAccess.Read))
            {
                //二进制 反序列化
                //BinaryFormatter bf = new BinaryFormatter();
                //xml 反序列化
                XmlSerializer bf = new XmlSerializer(typeof(List<Correct_Data>));
                List<Correct_Data> list = (List<Correct_Data>)bf.Deserialize(fs);
                return list;
            }
        }
        //将矫正数据保存为csv
        public static void Save_To_Csv(List<Correct_Data> Data,string fileName)
        {
            string sdatetime = DateTime.Now.ToString("D");
            string delimiter = ",";
            string strHeader = "";
            //保存的位置和文件名称
            string File_Path = @"./\Config/" + fileName;

            strHeader += "相机实际X坐标,相机实际Y坐标,平台电机实际X坐标,平台电机实际Y坐标";

            bool isExit = File.Exists(File_Path);
            StreamWriter sw = new StreamWriter(File_Path, true, Encoding.GetEncoding("gb2312"));
            if (!isExit)
            {
                sw.WriteLine(strHeader);
            }
            for (int i=0;i< Data.Count;i++)
            {
                //output rows data
                string strRowValue = "";
                strRowValue += Data[i].Xo + delimiter
                             + Data[i].Yo + delimiter
                             + Data[i].Xm + delimiter
                             + Data[i].Ym + delimiter;
                sw.WriteLine(strRowValue);
            }
            sw.Close();
            MessageBox.Show("保存成功！");
        }

        //仿射变换数据序列化操作
        public static void Serialize_Affinity_Matrix(List<Affinity_Matrix> list, string txtFile) 
        {
            //写入文件
            string File_Path = @"./\Config/" + txtFile;
            using (FileStream fs = new FileStream(File_Path, FileMode.Create, FileAccess.ReadWrite))
            {
                //保存参数至文件 二进制
                //BinaryFormatter bf = new BinaryFormatter();
                //保存为xml
                XmlSerializer bf = new XmlSerializer(typeof(List<Affinity_Matrix>));
                bf.Serialize(fs, list);
            }
        }

        //仿射变换数据反序列化
        public static List<Affinity_Matrix> Reserialize_Affinity_Matrix(string fileName)  
        {

            //读取文件
            string File_Path = @"./\Config/" + fileName;
            using (FileStream fs = new FileStream(File_Path, FileMode.Open, FileAccess.Read))
            {
                //二进制 反序列化
                //BinaryFormatter bf = new BinaryFormatter();
                //xml 反序列化
                XmlSerializer bf = new XmlSerializer(typeof(List<Affinity_Matrix>));
                List<Affinity_Matrix> list = (List<Affinity_Matrix>)bf.Deserialize(fs);
                return list;
            }
        }        

        //8.28
        //dxf-仿射变换数据序列化操作
        public void Serialize_Affinity_Matrix_dxf(Affinity_Matrix list, string txtFile)
        {
            //写入文件
            string File_Path = @"./\Config/" + txtFile;
            using (FileStream fs = new FileStream(File_Path, FileMode.Create, FileAccess.ReadWrite))
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(fs, list);
            }
        }
        //8.28
        //dxf-仿射变换数据反序列化
        public Affinity_Matrix Reserialize_Affinity_Matrix_dxf(string fileName)
        {

            //读取文件
            string File_Path = @"./\Config/" + fileName;
            using (FileStream fs = new FileStream(File_Path, FileMode.Open, FileAccess.Read))
            {
                BinaryFormatter bf = new BinaryFormatter();
                Affinity_Matrix list = (Affinity_Matrix)bf.Deserialize(fs);
                return list;
            }
        }
    }
}
