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
        private decimal stretch_x, distortion_x, delta_x;
        private decimal stretch_y, distortion_y, delta_y;
        //共有属性
        public decimal Stretch_X
        {
            get { return stretch_x; }
            set { stretch_x = value; }
        }
        public decimal Distortion_X
        {
            get { return distortion_x; }
            set { distortion_x = value; }
        }

        public decimal Delta_X
        {
            get { return delta_x; }
            set { delta_x = value; }
        }
        public decimal Stretch_Y
        {
            get { return stretch_y; }
            set { stretch_y = value; }
        }
        public decimal Distortion_Y
        {
            get { return distortion_y; }
            set { distortion_y = value; }
        }
        public decimal Delta_Y
        {
            get { return delta_y; }
            set { delta_y = value; }
        }
        //公开构造函数        
        //有参数
        public Affinity_Matrix(decimal stretch_x, decimal distortion_x, decimal delta_x, decimal stretch_y, decimal distortion_y, decimal delta_y)
        {
            this.stretch_x = stretch_x;
            this.distortion_x = distortion_x;
            this.stretch_y = stretch_y;
            this.distortion_y = distortion_y;
            this.delta_x = delta_x;
            this.delta_y = delta_y;
        }
        public Affinity_Matrix(Affinity_Matrix Ini)
        {
            this.stretch_x = Ini.Stretch_X;
            this.distortion_x = Ini.Distortion_X;
            this.delta_x = Ini.Delta_X;
            this.stretch_y = Ini.Stretch_Y;
            this.distortion_y = Ini.Distortion_Y;
            this.delta_y = Ini.Delta_Y;
        }
        //清空
        public void Empty()
        {
            this.stretch_x = 0;
            this.distortion_x = 0;
            this.delta_x = 0;
            this.stretch_y = 0;
            this.distortion_y = 0;
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
            Vector Cal_Actual_Point = new Vector();
            //建立直角坐标系
            GTS_Fun.Interpolation.Coordination(Para_List.Parameter.Work.X, Para_List.Parameter.Work.Y);
            //定位到加工坐标原点
            GTS_Fun.Interpolation.Clear_FIFO();
            GTS_Fun.Interpolation.Line_FIFO(0, 0);//将直线插补数据写入
            GTS_Fun.Interpolation.Interpolation_Start();
            //停止坐标系运动
            GTS_Fun.Interpolation.Interpolation_Stop();
            //2.5mm步距进行数据提取和整合，使用INC指令
            for (i = 0; i < Para_List.Parameter.Gts_Calibration_Row; i++)
            {
                //1轴-x轴，2轴-y轴，X轴归零，y轴归 步距*i
                //插补运动实现
                GTS_Fun.Interpolation.Clear_FIFO();
                GTS_Fun.Interpolation.Line_FIFO(0, i * Para_List.Parameter.Gts_Calibration_Cell);//将直线插补数据写入
                GTS_Fun.Interpolation.Interpolation_Start();
                GTS_Fun.Interpolation.Interpolation_Stop();
                for (j = 0; j < Para_List.Parameter.Gts_Calibration_Col; j++)
                {
                    //清空Temp_Correct_Data
                    Temp_Correct_Data.Empty();
                    //插补运动实现
                    GTS_Fun.Interpolation.Clear_FIFO();
                    GTS_Fun.Interpolation.Line_FIFO(j * Para_List.Parameter.Gts_Calibration_Cell, i * Para_List.Parameter.Gts_Calibration_Cell);//将直线插补数据写入
                    GTS_Fun.Interpolation.Interpolation_Start();
                    GTS_Fun.Interpolation.Interpolation_Stop();
                    //调用相机，获取对比的坐标信息
                    Thread.Sleep(50);//延时30ms
                    //Main.T_Client
                    Cam_New = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation(1));//触发拍照 
                    if ((Cam_New.X==999) || (Cam_New.Y == 999))
                    {
                        MessageBox.Show("相机通讯数据等待，请检查！！！");
                        return new List<Correct_Data>();
                    }
                    //数据处理
                    if (j == 0)//X轴坐标归零
                    {
                        //计算差值
                        Cam_Delta_X = Cam_New.X - 243 * Para_List.Parameter.Cam_Reference;
                        Cam_Delta_Y = Cam_New.Y - 324 * Para_List.Parameter.Cam_Reference;
                    }
                    else
                    {
                        //计算差值
                        Cam_Delta_X = Cam_New.X - Cam_Old.X;
                        Cam_Delta_Y = Cam_New.Y - Cam_Old.Y;                        
                    }
                    //仿射矫正坐标
                    Cal_Actual_Point = Get_Cal_Actual_Point(new Vector(j * Para_List.Parameter.Gts_Calibration_Cell, i * Para_List.Parameter.Gts_Calibration_Cell));//初始化坐标
                    //数据保存
                    Temp_Correct_Data.Xo = Cal_Actual_Point.X + Cam_Delta_X;//相机实际X坐标
                    Temp_Correct_Data.Yo = Cal_Actual_Point.Y + Cam_Delta_Y;//相机实际Y坐标
                    Temp_Correct_Data.Xm = j * Para_List.Parameter.Gts_Calibration_Cell;//平台电机实际X坐标
                    Temp_Correct_Data.Ym = i * Para_List.Parameter.Gts_Calibration_Cell;//平台电机实际Y坐标
                    //New变Old
                    Cam_Old =new Vector(Cam_New);
                    //添加进入List
                    Result.Add(Temp_Correct_Data);

                    //线程终止
                    if (Exit_Flag)
                    {
                        Exit_Flag = false;
                        Serialize_Data.Serialize_Correct_Data(Result, "Correct_Data_01.xml");
                        CSV_RW.SaveCSV(CSV_RW.Correct_Data_DataTable(Result), "Gts_Correct_Data_01");
                        return Result;
                    }

                }
            }
            //保存文件至Config
            Serialize_Data.Serialize_Correct_Data(Result, "Correct_Data_01.xml");
            CSV_RW.SaveCSV(CSV_RW.Correct_Data_DataTable(Result), "Gts_Correct_Data_01");
            MessageBox.Show("数据采集完成！！！");
            return Result;
        }
        public static List<Correct_Data> Get_Datas_Correct() 
        {
            //建立变量
            List<Correct_Data> Result = new List<Correct_Data>();
            Correct_Data Temp_Correct_Data = new Correct_Data();

            //建立变量
            Vector Cam_Old = new Vector();
            Vector Cam_New = new Vector();
            decimal Cam_Delta_X = 0, Cam_Delta_Y = 0;
            int i = 0, j = 0;
            Vector Cal_Actual_Point = new Vector();

            //建立直角坐标系
            GTS_Fun.Interpolation.Coordination(Para_List.Parameter.Work.X, Para_List.Parameter.Work.Y);
            //定位到加工坐标原点
            GTS_Fun.Interpolation.Clear_FIFO();
            GTS_Fun.Interpolation.Line_FIFO(0, 0);//将直线插补数据写入
            GTS_Fun.Interpolation.Interpolation_Start();
            //停止坐标系运动
            GTS_Fun.Interpolation.Interpolation_Stop();
            //2.5mm步距进行数据提取和整合，使用INC指令
            for (i = 135; i < Para_List.Parameter.Gts_Calibration_Row; i++)
            {
                //1轴-x轴，2轴-y轴，X轴归零，y轴归 步距*i
                //插补运动实现
                GTS_Fun.Interpolation.Clear_FIFO();
                GTS_Fun.Interpolation.Line_FIFO_Correct(0, i * Para_List.Parameter.Gts_Calibration_Cell);//将直线插补数据写入
                GTS_Fun.Interpolation.Interpolation_Start();
                GTS_Fun.Interpolation.Interpolation_Stop();
                for (j = 0; j < Para_List.Parameter.Gts_Calibration_Col; j++)
                {
                    //清空Temp_Correct_Data
                    Temp_Correct_Data.Empty();
                    //定位X轴
                    //插补运动实现
                    GTS_Fun.Interpolation.Clear_FIFO();
                    GTS_Fun.Interpolation.Line_FIFO_Correct(j * Para_List.Parameter.Gts_Calibration_Cell, i * Para_List.Parameter.Gts_Calibration_Cell);//将直线插补数据写入
                    GTS_Fun.Interpolation.Interpolation_Start();
                    GTS_Fun.Interpolation.Interpolation_Stop();
#if !DEBUG
                    //调用相机，获取对比的坐标信息
                    Thread.Sleep(50);//延时200ms
                    //Main.T_Client
                    Cam_New = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation(1));//触发拍照 
                    if ((Cam_New.X == 999) || (Cam_New.Y == 999))
                    {
                        MessageBox.Show("相机通讯数据等待，请检查！！！");
                        return new List<Correct_Data>();
                    }
                    //数据处理
                    if (j == 0)//X轴坐标归零
                    {
                        //计算差值
                        Cam_Delta_X = Cam_New.X - 243 * Para_List.Parameter.Cam_Reference;
                        Cam_Delta_Y = Cam_New.Y - 324 * Para_List.Parameter.Cam_Reference;
                    }
                    else
                    {
                        //计算差值
                        Cam_Delta_X = Cam_New.X - Cam_Old.X;
                        Cam_Delta_Y = Cam_New.Y - Cam_Old.Y;                        
                    }
                    //仿射矫正坐标
                    Cal_Actual_Point = Get_Cal_Actual_Point(new Vector(j * Para_List.Parameter.Gts_Calibration_Cell, i * Para_List.Parameter.Gts_Calibration_Cell));//初始化坐标
                    //数据保存
                    Temp_Correct_Data.Xo = Cal_Actual_Point.X + Cam_Delta_X;//相机实际X坐标
                    Temp_Correct_Data.Yo = Cal_Actual_Point.Y + Cam_Delta_Y;//相机实际Y坐标
                    //Temp_Correct_Data.Xo = j * Para_List.Parameter.Gts_Calibration_Cell + Cam_Delta_X;//相机实际X坐标
                    //Temp_Correct_Data.Yo = i * Para_List.Parameter.Gts_Calibration_Cell + Cam_Delta_Y;//相机实际Y坐标
                    Temp_Correct_Data.Xm = j * Para_List.Parameter.Gts_Calibration_Cell;//平台电机实际X坐标
                    Temp_Correct_Data.Ym = i * Para_List.Parameter.Gts_Calibration_Cell;//平台电机实际Y坐标
                    //New变Old
                    Cam_Old = new Vector(Cam_New);
                    //添加进入List
                    Result.Add(Temp_Correct_Data);
#endif
                    //线程终止
                    if (Exit_Flag)
                    {
                        Exit_Flag = false;
                        Serialize_Data.Serialize_Correct_Data(Result, "Correct_Data_02.xml");
                        CSV_RW.SaveCSV(CSV_RW.Correct_Data_DataTable(Result), "Gts_Correct_Data_02");
                        return Result;
                    }
                }
            }
            //保存文件至Config
            Serialize_Data.Serialize_Correct_Data(Result, "Correct_Data_02.xml");
            CSV_RW.SaveCSV(CSV_RW.Correct_Data_DataTable(Result), "Gts_Correct_Data_02");
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
            {
                if (Counting == 0)
                {
                    //定位到ORG原点
                    Mark(new Vector(0, 0));
                }
                else
                {
                    //定位到矫正坐标
                    Mark(Para_List.Parameter.Cal_Org);
                }
                //调用相机，获取对比的坐标信息
                Thread.Sleep(200);//延时200ms
                //Main.T_Client
                Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation(1));//触发拍照 
                if ((Cam.X == 999) || (Cam.Y == 999))
                {
                    MessageBox.Show("相机通讯数据等待，请检查！！！");
                    return;
                }
                Cam = new Vector(Cam.X - 243 * Para_List.Parameter.Cam_Reference, Cam.Y - 324 * Para_List.Parameter.Cam_Reference);
                //获取坐标系平台坐标
                Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate(0));
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
            } while (!Differ_Deviation(Cam,Para_List.Parameter.Pos_Tolerance));
            Para_List.Parameter.Work = new Vector(Para_List.Parameter.Work.X - Para_List.Parameter.Cal_Org.X, Para_List.Parameter.Work.Y - Para_List.Parameter.Cal_Org.Y);
            //数据矫正完成
        }
        //计算标定板仿射变换参数
        public static Affinity_Matrix Cal_Calibration_Affinity_Matrix()
        {
            //建立变量
            Affinity_Matrix Result = new Affinity_Matrix();
            //定义仿射变换数组 
            Mat mat = new Mat(new Size(3, 2), Emgu.CV.CvEnum.DepthType.Cv32F, 1); //2行 3列 的矩阵
            //定义点位数组
            PointF[] srcTri = new PointF[3];//标准数据
            PointF[] dstTri = new PointF[3];//差异化数据 
            double[] temp_array;
            //定位点位计算标定板偏差
            Vector[] Cali_Mark_Src = new Vector[3] { new Vector(0, 0), new Vector(350, 0), new Vector(350, 350) };
            Vector[] Cali_Mark_Dst = new Vector[3] { new Vector(0, 0), new Vector(350, 0), new Vector(350, 350) };
            //中间变量
            int Counting = 0;
            Vector Cam = new Vector();
            Vector Coodinate_Point = new Vector();
            Vector Tem_Mark = new Vector();
            //标定板数据计算
            //矫正 标定板数据实际点位1           
            Counting = 0;
            do
            {
                //定位到标定板数据实际点位1
                Mark(Cali_Mark_Dst[0]);
                //调用相机，获取对比的坐标信息
                Thread.Sleep(200);//延时200ms
                Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation(1));//触发拍照 
                if ((Cam.X == 999) || (Cam.Y == 999))
                {
                    MessageBox.Show("相机通讯数据等待，请检查！！！");
                    return Result;
                }
                //获取坐标系平台坐标
                Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate(0));
                //计算偏移
                Tem_Mark = new Vector(Coodinate_Point.X + Cam.X - 243 * Para_List.Parameter.Cam_Reference, Coodinate_Point.Y + Cam.Y - 324 * Para_List.Parameter.Cam_Reference);
                //反馈回标定板数据实际点位1
                Cali_Mark_Dst[0] = new Vector(Tem_Mark);
                //自增
                Counting++;
                //跳出
                if (Counting >= 20)
                {
                    MessageBox.Show("标定板数据实际点位1 寻找失败");
                    return Result;
                }
            } while (!Differ_Deviation(new Vector(Cam.X - 243 * Para_List.Parameter.Cam_Reference, Cam.Y - 324 * Para_List.Parameter.Cam_Reference), Para_List.Parameter.Pos_Tolerance));


            //矫正 标定板数据实际点位2           
            Counting = 0;
            do
            {
                //定位到标定板数据实际点位2
                Mark(Cali_Mark_Dst[1]);
                //调用相机，获取对比的坐标信息
                Thread.Sleep(200);//延时200ms
                Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation(1));//触发拍照 
                if ((Cam.X == 999) || (Cam.Y == 999))
                {
                    MessageBox.Show("相机通讯数据等待，请检查！！！");
                    return Result;
                }
                //获取坐标系平台坐标
                Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate(0));
                //计算偏移
                Tem_Mark = new Vector(Coodinate_Point.X + Cam.X - 243 * Para_List.Parameter.Cam_Reference, Coodinate_Point.Y + Cam.Y - 324 * Para_List.Parameter.Cam_Reference);
                //反馈回标定板数据实际点位2
                Cali_Mark_Dst[1] = new Vector(Tem_Mark);
                //自增
                Counting++;
                //跳出
                if (Counting >= 20)
                {
                    MessageBox.Show("标定板数据实际点位2 寻找失败");
                    return Result;
                }
            } while (!Differ_Deviation(new Vector(Cam.X - 243 * Para_List.Parameter.Cam_Reference, Cam.Y - 324 * Para_List.Parameter.Cam_Reference), Para_List.Parameter.Pos_Tolerance));

            //矫正 标定板数据实际点位3          
            Counting = 0;
            do
            {
                //定位到标定板数据实际点位3
                Mark(Cali_Mark_Dst[2]);
                //调用相机，获取对比的坐标信息
                Thread.Sleep(200);//延时200ms
                Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation(1));//触发拍照 
                if ((Cam.X == 999) || (Cam.Y == 999))
                {
                    MessageBox.Show("相机通讯数据等待，请检查！！！");
                    return Result;
                }
                //获取坐标系平台坐标
                Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate(0));
                //计算偏移
                Tem_Mark = new Vector(Coodinate_Point.X + Cam.X - 243 * Para_List.Parameter.Cam_Reference, Coodinate_Point.Y + Cam.Y - 324 * Para_List.Parameter.Cam_Reference);
                //反馈回标定板数据实际点位3
                Cali_Mark_Dst[2] = new Vector(Tem_Mark);
                //自增
                Counting++;
                //跳出
                if (Counting >= 20)
                {
                    MessageBox.Show("标定板数据实际点位3 寻找失败");
                    return Result;
                }
            } while (!Differ_Deviation(new Vector(Cam.X - 243 * Para_List.Parameter.Cam_Reference, Cam.Y - 324 * Para_List.Parameter.Cam_Reference), Para_List.Parameter.Pos_Tolerance));

            //数据提取
            //标准数据
            srcTri[0] = new PointF((float)(Cali_Mark_Src[0].X), (float)(Cali_Mark_Src[0].Y));
            srcTri[1] = new PointF((float)(Cali_Mark_Src[1].X), (float)(Cali_Mark_Src[1].Y));
            srcTri[2] = new PointF((float)(Cali_Mark_Src[2].X), (float)(Cali_Mark_Src[2].Y));
            //仿射数据
            dstTri[0] = new PointF((float)(Cali_Mark_Dst[0].X), (float)(Cali_Mark_Dst[0].Y));
            dstTri[1] = new PointF((float)(Cali_Mark_Dst[1].X), (float)(Cali_Mark_Dst[1].Y));
            dstTri[2] = new PointF((float)(Cali_Mark_Dst[2].X), (float)(Cali_Mark_Dst[2].Y));
            //计算仿射变换矩阵
            mat = CvInvoke.GetAffineTransform(srcTri, dstTri);
            //提取矩阵数据
            temp_array = mat.GetDoubleArray();
            //获取仿射变换参数
            Result = Gts_Cal_Data_Resolve.Array_To_Affinity(temp_array);
            Para_List.Parameter.Cal_Trans_Affinity = new Affinity_Matrix(Result);
            //追加进入仿射变换List
            return Result;
        }
        //计算标定板仿射变换后坐标值
        public static Vector Get_Cal_Actual_Point(Vector src)
        {
            return new Vector(src.X * Para_List.Parameter.Cal_Trans_Affinity.Stretch_X + src.Y * Para_List.Parameter.Cal_Trans_Affinity.Distortion_X + Para_List.Parameter.Cal_Trans_Affinity.Delta_X, src.Y * Para_List.Parameter.Cal_Trans_Affinity.Stretch_Y + src.X * Para_List.Parameter.Cal_Trans_Affinity.Distortion_Y + Para_List.Parameter.Cal_Trans_Affinity.Delta_Y);
        }
        /// <summary>
        /// 矫正Mark坐标
        ///
        /// </summary>
        /// <param name="type"></param>
        /// type - 0 初次校准
        /// type - 1 re_cal
        public static void Calibrate_Mark(int type)
        {
            //建立变量
            Vector Cam = new Vector();
            Vector Coodinate_Point;
            Vector Tem_Mark;
            Vector Mark4_dif;
            UInt16 Counting;
            List<Vector> Mark_Datas = new List<Vector>();
            //abstract Mark Point
            if (type == 0)
            {
                Mark_Datas.Add(new Vector(Para_List.Parameter.Mark1));
                Mark_Datas.Add(new Vector(Para_List.Parameter.Mark2));
                Mark_Datas.Add(new Vector(Para_List.Parameter.Mark3));
                Mark_Datas.Add(new Vector(Para_List.Parameter.Mark4));
            }
            else
            {
                Mark_Datas.Add(new Vector(Gts_Cal_Data_Resolve.Get_Aff_After(Para_List.Parameter.Mark_Dxf1, Para_List.Parameter.Trans_Affinity)));
                Mark_Datas.Add(new Vector(Gts_Cal_Data_Resolve.Get_Aff_After(Para_List.Parameter.Mark_Dxf2, Para_List.Parameter.Trans_Affinity)));
                Mark_Datas.Add(new Vector(Gts_Cal_Data_Resolve.Get_Aff_After(Para_List.Parameter.Mark_Dxf3, Para_List.Parameter.Trans_Affinity)));
                Mark_Datas.Add(new Vector(Gts_Cal_Data_Resolve.Get_Aff_After(Para_List.Parameter.Mark_Dxf4, Para_List.Parameter.Trans_Affinity)));
            }            
            //process the mark point
            for (int i=0;i< Mark_Datas.Count;i++)
            {
                //矫正Mark           
                Counting = 0;
                do
                {
                    //定位到Mark点
                    Mark(Mark_Datas[i]);
                    //调用相机，获取对比的坐标信息
                    Thread.Sleep(200);//延时200ms
                    Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation(2));//触发拍照 
                    if ((Cam.X == 999) || (Cam.Y == 999))
                    {
                        MessageBox.Show("相机通讯数据等待，请检查！！！");
                        return;
                    }
                    //获取坐标系平台坐标
                    Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate(0));
                    //计算偏移
                    Tem_Mark = new Vector(Coodinate_Point.X + Cam.X, Coodinate_Point.Y + Cam.Y);
                    //反馈回Mark点
                    Mark_Datas[i] = new Vector(Tem_Mark);
                    //自增
                    Counting++;
                    //跳出
                    if (Counting >= 20)
                    {
                        MessageBox.Show(string.Format("Mark{0} 寻找失败!!!",i+1));
                        return;
                    }
                } while (!Differ_Deviation(Cam, Para_List.Parameter.Pos_Tolerance));
            }
            //cal Affinity matrics data 
            Para_List.Parameter.Trans_Affinity =new Affinity_Matrix(Gts_Cal_Data_Resolve.Cal_Affinity(Mark_Datas));

            //difference mark4 
            Tem_Mark = Gts_Cal_Data_Resolve.Get_Aff_After(Para_List.Parameter.Mark_Dxf4, Para_List.Parameter.Trans_Affinity);
            //caluate difference between theory mark4 and actual mark4
            Mark4_dif = new Vector(Tem_Mark - Mark_Datas[3]);
            //output result
            if (Differ_Deviation(Mark4_dif, Para_List.Parameter.Mark_Reference))
            {
                Prompt.Log.Info(String.Format("Mark4 验证OK！！！，X坐标偏差：{0}，Y坐标偏差：{1}", Mark4_dif.X, Mark4_dif.Y));
                MessageBox.Show(String.Format("Mark4 验证OK！！！，X坐标偏差：{0}，Y坐标偏差：{1}", Mark4_dif.X, Mark4_dif.Y));
            }
            else
            {
                Prompt.Log.Info(String.Format("Mark4 验证NG！！！，X坐标偏差：{0}，Y坐标偏差：{1}", Mark4_dif.X, Mark4_dif.Y));
                MessageBox.Show(String.Format("Mark4 验证NG！！！，X坐标偏差：{0}，Y坐标偏差：{1}", Mark4_dif.X, Mark4_dif.Y));
            }

        }
        //判别误差范围之内
        public static bool Differ_Deviation(Vector Indata,decimal reference)
        {
            if ((Math.Abs(Indata.X)<= reference) && (Math.Abs(Indata.Y) <= reference))
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
                Thread.Sleep(200);//延时200ms
                //Main.T_Client
                Cam = new Vector(Initialization.Initial.T_Client.Get_Cam_Deviation(2));//触发拍照 
                if ((Cam.X == 999) || (Cam.Y == 999))
                {
                    MessageBox.Show("相机通讯数据等待，请检查！！！");
                    return;
                }
                //获取坐标系平台坐标
                Coodinate_Point = new Vector(GTS_Fun.Interpolation.Get_Coordinate(0));
                //计算偏移
                Tem_Mark = new Vector(Coodinate_Point.X + Cam.X - 100, Coodinate_Point.Y + Cam.Y - 100);
                //反馈回RTC_ORG数据
                Para_List.Parameter.Rtc_Org = new Vector(Tem_Mark);
                //自增
                Counting++;
            } while (!Differ_Deviation(Cam, Para_List.Parameter.Pos_Tolerance) && (Counting <= 10));
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
        //处理相机与轴的数据，生成仿射变换数组，并保存进入文件
        public static List<Affinity_Matrix> Resolve(List<Correct_Data> correct_Datas)
        {
            //建立变量
            List<Affinity_Matrix> Result = new List<Affinity_Matrix>();
            Affinity_Matrix Temp_Affinity_Matrix = new Affinity_Matrix();
            List<Double_Fit_Data> Line_Fit_Data_AM = new List<Double_Fit_Data>();
            Double_Fit_Data Temp_Fit_Data_AM = new Double_Fit_Data();
            List<Double_Fit_Data> Line_Fit_Data_MA = new List<Double_Fit_Data>();
            Double_Fit_Data Temp_Fit_Data_MA = new Double_Fit_Data();
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
                    //初始化数据 M_A
                    double[] src_x_MA = new double[Para_List.Parameter.Gts_Calibration_Col];
                    double[] dst_x_MA = new double[Para_List.Parameter.Gts_Calibration_Col];
                    Tuple<double, double> R_X_MA = new Tuple<double, double>(0, 0);
                    double[] src_y_MA = new double[Para_List.Parameter.Gts_Calibration_Row];
                    double[] dst_y_MA = new double[Para_List.Parameter.Gts_Calibration_Row];
                    Tuple<double, double> R_Y_MA = new Tuple<double, double>(0, 0);
                    //初始化数据 A_M
                    double[] src_x_AM = new double[Para_List.Parameter.Gts_Calibration_Col];
                    double[] dst_x_AM = new double[Para_List.Parameter.Gts_Calibration_Col];
                    Tuple<double, double> R_X_AM = new Tuple<double, double>(0, 0);
                    double[] src_y_AM = new double[Para_List.Parameter.Gts_Calibration_Row];
                    double[] dst_y_AM = new double[Para_List.Parameter.Gts_Calibration_Row];
                    Tuple<double, double> R_Y_AM = new Tuple<double, double>(0, 0);                    
                    //拟合数据
                    for (i = 0; i < Para_List.Parameter.Gts_Calibration_Col; i++)
                    {
                        for (j = 0; j < Para_List.Parameter.Gts_Calibration_Row; j++)
                        {
                            //提取X轴拟合数据 M_A
                            src_x_MA[j] = (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xm);
                            dst_x_MA[j] = (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xo);
                            //提取Y轴拟合数据
                            src_y_MA[j] = (float)(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col].Ym);
                            dst_y_MA[j] = (float)(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col].Yo);

                            //提取X轴拟合数据 A_M
                            src_x_AM[j] = (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xo);
                            dst_x_AM[j] = (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xm);
                            //提取Y轴拟合数据
                            src_y_AM[j] = (float)(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col].Yo);
                            dst_y_AM[j] = (float)(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col].Ym);
                        }
                        //高阶曲线拟合
                        if (Line_Re == 4)
                        {
                            double[] Res_x_MA = Fit.Polynomial(src_x_MA, dst_x_MA, Line_Re);
                            double[] Res_y_MA = Fit.Polynomial(src_y_MA, dst_y_MA, Line_Re);
                            double[] Res_x_AM = Fit.Polynomial(src_y_AM, dst_x_AM, Line_Re);
                            double[] Res_y_AM = Fit.Polynomial(src_x_AM, dst_y_AM, Line_Re);
                            //提取拟合直线数据 AM
                            Temp_Fit_Data_AM = new Double_Fit_Data
                            {
                                K_X4 = (decimal)Res_x_AM[4],
                                K_X3 = (decimal)Res_x_AM[3],
                                K_X2 = (decimal)Res_x_AM[2],
                                K_X1 = (decimal)Res_x_AM[1],
                                Delta_X = (decimal)Res_x_AM[0],
                                K_Y4 = (decimal)Res_y_AM[4],
                                K_Y3 = (decimal)Res_y_AM[3],
                                K_Y2 = (decimal)Res_y_AM[2],
                                K_Y1 = (decimal)Res_y_AM[1],
                                Delta_Y = (decimal)Res_y_AM[0]
                            };
                            //提取拟合直线数据 MA
                            Temp_Fit_Data_MA = new Double_Fit_Data
                            {
                                K_X4 = (decimal)Res_x_MA[4],
                                K_X3 = (decimal)Res_x_MA[3],
                                K_X2 = (decimal)Res_x_MA[2],
                                K_X1 = (decimal)Res_x_MA[1],
                                Delta_X = (decimal)Res_x_MA[0],
                                K_Y4 = (decimal)Res_y_MA[4],
                                K_Y3 = (decimal)Res_y_MA[3],
                                K_Y2 = (decimal)Res_y_MA[2],
                                K_Y1 = (decimal)Res_y_MA[1],
                                Delta_Y = (decimal)Res_y_MA[0]
                            };
                        }
                        else if (Line_Re == 3)
                        {
                            double[] Res_x_MA = Fit.Polynomial(src_x_MA, dst_x_MA, Line_Re);
                            double[] Res_y_MA = Fit.Polynomial(src_y_MA, dst_y_MA, Line_Re);
                            double[] Res_x_AM = Fit.Polynomial(src_y_AM, dst_x_AM, Line_Re);
                            double[] Res_y_AM = Fit.Polynomial(src_x_AM, dst_y_AM, Line_Re);
                            //提取拟合直线数据 AM
                            Temp_Fit_Data_AM = new Double_Fit_Data
                            {
                                K_X4 = 0,
                                K_X3 = (decimal)Res_x_AM[3],
                                K_X2 = (decimal)Res_x_AM[2],
                                K_X1 = (decimal)Res_x_AM[1],
                                Delta_X = (decimal)Res_x_AM[0],
                                K_Y4 = (decimal)Res_y_AM[4],
                                K_Y3 = (decimal)Res_y_AM[3],
                                K_Y2 = (decimal)Res_y_AM[2],
                                K_Y1 = (decimal)Res_y_AM[1],
                                Delta_Y = (decimal)Res_y_AM[0]
                            };
                            //提取拟合直线数据 MA
                            Temp_Fit_Data_MA = new Double_Fit_Data
                            {
                                K_X4 = 0,
                                K_X3 = (decimal)Res_x_MA[3],
                                K_X2 = (decimal)Res_x_MA[2],
                                K_X1 = (decimal)Res_x_MA[1],
                                Delta_X = (decimal)Res_x_MA[0],
                                K_Y4 = 0,
                                K_Y3 = (decimal)Res_y_MA[3],
                                K_Y2 = (decimal)Res_y_MA[2],
                                K_Y1 = (decimal)Res_y_MA[1],
                                Delta_Y = (decimal)Res_y_MA[0]
                            };
                        }
                        else if (Line_Re == 2)
                        {
                            double[] Res_x_MA = Fit.Polynomial(src_x_MA, dst_x_MA, Line_Re);
                            double[] Res_y_MA = Fit.Polynomial(src_y_MA, dst_y_MA, Line_Re);
                            double[] Res_x_AM = Fit.Polynomial(src_y_AM, dst_x_AM, Line_Re);
                            double[] Res_y_AM = Fit.Polynomial(src_x_AM, dst_y_AM, Line_Re);
                            //提取拟合直线数据 AM
                            Temp_Fit_Data_AM = new Double_Fit_Data
                            {
                                K_X4 = 0,
                                K_X3 = 0,
                                K_X2 = (decimal)Res_x_AM[2],
                                K_X1 = (decimal)Res_x_AM[1],
                                Delta_X = (decimal)Res_x_AM[0],
                                K_Y4 = 0,
                                K_Y3 = 0,
                                K_Y2 = (decimal)Res_y_AM[2],
                                K_Y1 = (decimal)Res_y_AM[1],
                                Delta_Y = (decimal)Res_y_AM[0]
                            };
                            //提取拟合直线数据 MA
                            Temp_Fit_Data_MA = new Double_Fit_Data
                            {
                                K_X4 = 0,
                                K_X3 = 0,
                                K_X2 = (decimal)Res_x_MA[2],
                                K_X1 = (decimal)Res_x_MA[1],
                                Delta_X = (decimal)Res_x_MA[0],
                                K_Y4 = 0,
                                K_Y3 = 0,
                                K_Y2 = (decimal)Res_y_MA[2],
                                K_Y1 = (decimal)Res_y_MA[1],
                                Delta_Y = (decimal)Res_y_MA[0]
                            };
                        }
                        else if (Line_Re == 1)
                        {
                            double[] Res_x_MA = Fit.Polynomial(src_x_MA, dst_x_MA, Line_Re);
                            double[] Res_y_MA = Fit.Polynomial(src_y_MA, dst_y_MA, Line_Re);
                            double[] Res_x_AM = Fit.Polynomial(src_y_AM, dst_x_AM, Line_Re);
                            double[] Res_y_AM = Fit.Polynomial(src_x_AM, dst_y_AM, Line_Re);
                            //提取拟合直线数据 AM
                            Temp_Fit_Data_AM = new Double_Fit_Data
                            {
                                K_X4 = 0,
                                K_X3 = 0,
                                K_X2 = 0,
                                K_X1 = (decimal)Res_x_AM[1],
                                Delta_X = (decimal)Res_x_AM[0],
                                K_Y4 = 0,
                                K_Y3 = 0,
                                K_Y2 = 0,
                                K_Y1 = (decimal)Res_y_AM[1],
                                Delta_Y = (decimal)Res_y_AM[0]
                            };
                            //提取拟合直线数据 MA
                            Temp_Fit_Data_MA = new Double_Fit_Data
                            {
                                K_X4 = 0,
                                K_X3 = 0,
                                K_X2 = 0,
                                K_X1 = (decimal)Res_x_MA[1],
                                Delta_X = (decimal)Res_x_MA[0],
                                K_Y4 = 0,
                                K_Y3 = 0,
                                K_Y2 = 0,
                                K_Y1 = (decimal)Res_y_MA[1],
                                Delta_Y = (decimal)Res_y_MA[0]
                            };
                        }
                        else
                        {
                            R_X_MA = Fit.Line(src_x_MA, dst_x_MA); 
                            R_Y_MA = Fit.Line(src_y_MA, dst_y_MA);
                            R_X_AM = Fit.Line(src_y_AM, dst_x_AM);
                            R_Y_AM = Fit.Line(src_x_AM, dst_y_AM);
                            //提取拟合直线数据
                            Temp_Fit_Data_AM = new Double_Fit_Data
                            {
                                K_X1 = (decimal)R_X_AM.Item2,
                                Delta_X = (decimal)R_X_AM.Item1,
                                K_Y1 = (decimal)R_Y_AM.Item2,
                                Delta_Y = (decimal)R_Y_AM.Item1
                            };
                            //提取拟合直线数据
                            Temp_Fit_Data_MA = new Double_Fit_Data
                            {
                                K_X1 = (decimal)R_X_MA.Item2,
                                Delta_X = (decimal)R_X_MA.Item1,
                                K_Y1 = (decimal)R_Y_MA.Item2,
                                Delta_Y = (decimal)R_Y_MA.Item1
                            };
                        }                        
                        //保存进入Line_Fit_Data
                        Line_Fit_Data_AM.Add(new Double_Fit_Data(Temp_Fit_Data_AM));
                        //清空数据
                        Temp_Fit_Data_AM.Empty();
                        //保存进入Line_Fit_Data
                        Line_Fit_Data_MA.Add(new Double_Fit_Data(Temp_Fit_Data_MA));
                        //清空数据
                        Temp_Fit_Data_MA.Empty();
                    }
                    //保存轴拟合数据
                    CSV_RW.SaveCSV(CSV_RW.Double_Fit_Data_DataTable(Line_Fit_Data_AM), "Gts_Line_Fit_Data_AM");
                    //保存轴拟合数据
                    CSV_RW.SaveCSV(CSV_RW.Double_Fit_Data_DataTable(Line_Fit_Data_MA), "Gts_Line_Fit_Data_MA");
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

                            ////标准数据  平台坐标
                            //srcTri[0] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Ym));
                            //srcTri[1] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Ym));
                            //srcTri[2] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row + 1].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row + 1].Ym));
                            ////srcTri[3] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + 1].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + 1].Ym));

                            ////仿射数据  测量坐标
                            //dstTri[0] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Yo));
                            //dstTri[1] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Yo));
                            //dstTri[2] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row + 1].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row + 1].Yo));
                            ////dstTri[3] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + 1].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + 1].Yo));

                            //标准数据  平台坐标
                            srcTri[0] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Yo));
                            srcTri[1] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Yo));
                            srcTri[2] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row + 1].Xo), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row + 1].Yo));
                            //srcTri[3] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + 1].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + 1].Ym));

                            //仿射数据  测量坐标
                            dstTri[0] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col].Ym));
                            dstTri[1] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Ym));
                            dstTri[2] = new PointF((float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row + 1].Xm), (float)(correct_Datas[j + i * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row + 1].Ym));
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
        //abstract affinity parameter from array
        public static Affinity_Matrix Array_To_Affinity(double[] temp_array)
        {
            Affinity_Matrix Result = new Affinity_Matrix
            {
                //获取仿射变换参数
                Stretch_X = Convert.ToDecimal(temp_array[0]),
                Distortion_X = Convert.ToDecimal(temp_array[1]),
                Delta_X = Convert.ToDecimal(temp_array[2]),//x方向偏移
                Stretch_Y = Convert.ToDecimal(temp_array[4]),
                Distortion_Y = Convert.ToDecimal(temp_array[3]),
                Delta_Y = Convert.ToDecimal(temp_array[5])//y方向偏移
            };
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
        public static Affinity_Matrix Cal_Affinity()
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
            strHeader += "Stretch_X,Distortion_X,Delat_X,Stretch_Y,Distortion_Y,Delta_Y";
            bool isExit = File.Exists(filePath);
            StreamWriter sw = new StreamWriter(filePath, true, Encoding.GetEncoding("gb2312"));
            if (!isExit)
            {
                sw.WriteLine(strHeader);
            }
            //output rows data
            string strRowValue = "";
            strRowValue += Result.Stretch_X + delimiter
                         + Result.Distortion_X + delimiter
                         + Result.Delta_X + delimiter
                         + Result.Stretch_Y + delimiter
                         + Result.Distortion_Y + delimiter
                         + Result.Delta_Y;
            sw.WriteLine(strRowValue);
            sw.Close();

            //追加进入仿射变换List
            return Result;
        }
        //dxf 仿射变换 求DX，DY，Dct(sin \cos)
        public static Affinity_Matrix Cal_Affinity(List<Vector> indata)
        {
            //建立变量
            Affinity_Matrix Result = new Affinity_Matrix();
            if (indata.Count>=3)
            {
                //定义仿射变换数组 
                Mat mat = new Mat(new Size(3, 2), Emgu.CV.CvEnum.DepthType.Cv32F, 1); //2行 3列 的矩阵
                ///定义点位数组
                PointF[] srcTri = new PointF[3];//标准数据
                PointF[] dstTri = new PointF[3];//差异化数据 
                double[] temp_array;
                //数据提取
                //标准数据
                srcTri[0] = new PointF((float)(Para_List.Parameter.Mark_Dxf1.X), (float)(Para_List.Parameter.Mark_Dxf1.Y));
                srcTri[1] = new PointF((float)(Para_List.Parameter.Mark_Dxf2.X), (float)(Para_List.Parameter.Mark_Dxf2.Y));
                srcTri[2] = new PointF((float)(Para_List.Parameter.Mark_Dxf3.X), (float)(Para_List.Parameter.Mark_Dxf3.Y));
                //仿射数据
                dstTri[0] = new PointF((float)(indata[0].X), (float)(indata[0].Y));
                dstTri[1] = new PointF((float)(indata[1].X), (float)(indata[1].Y));
                dstTri[2] = new PointF((float)(indata[2].X), (float)(indata[2].Y));
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
                strHeader += "Stretch_X,Distortion_X,Delat_X,Stretch_Y,Distortion_Y,Delta_Y";
                bool isExit = File.Exists(filePath);
                StreamWriter sw = new StreamWriter(filePath, true, Encoding.GetEncoding("gb2312"));
                if (!isExit)
                {
                    sw.WriteLine(strHeader);
                }
                //output rows data
                string strRowValue = "";
                strRowValue += Result.Stretch_X + delimiter
                             + Result.Distortion_X + delimiter
                             + Result.Delta_X + delimiter
                             + Result.Stretch_Y + delimiter
                             + Result.Distortion_Y + delimiter
                             + Result.Delta_Y;
                sw.WriteLine(strRowValue);
                sw.Close();

                //追加进入仿射变换List
                return Result;
            }
            else
            {
                return Result;
            }
            
        }
        //获取线性补偿坐标
        public static Vector Get_Line_Fit_Coordinate_AM(decimal x,decimal y,List<Double_Fit_Data> line_fit_data)  
        {
            Vector Result = new Vector();
            //临时定位变量
            Int16 m, n;
            decimal X_per, Y_per;
            decimal K_x1, K_x2, K_x3, K_x4, B_x;
            decimal K_y1, K_y2, K_y3, K_y4, B_y; 

            //获取落点
            m = Seek_X_Pos(y);
            n = Seek_Y_Pos(x);
            //计算比率
            X_per = Math.Abs(y - m * Para_List.Parameter.Gts_Calibration_Cell) / Para_List.Parameter.Gts_Calibration_Cell;
            Y_per = Math.Abs(x - n * Para_List.Parameter.Gts_Calibration_Cell) / Para_List.Parameter.Gts_Calibration_Cell;
            //计算拟合参数
            K_x1 = (line_fit_data[m + 1].K_X1 - line_fit_data[m].K_X1) * X_per + line_fit_data[m].K_X1;
            K_x2 = (line_fit_data[m + 1].K_X2 - line_fit_data[m].K_X2) * X_per + line_fit_data[m].K_X2;
            K_x3 = (line_fit_data[m + 1].K_X3 - line_fit_data[m].K_X3) * X_per + line_fit_data[m].K_X3;
            K_x4 = (line_fit_data[m + 1].K_X4 - line_fit_data[m].K_X4) * X_per + line_fit_data[m].K_X4;
            B_x = (line_fit_data[m + 1].Delta_X - line_fit_data[m].Delta_X) * X_per + line_fit_data[m].Delta_X;
            K_y1 = (line_fit_data[n + 1].K_Y1 - line_fit_data[n].K_Y1) * Y_per + line_fit_data[n].K_Y1;
            K_y2 = (line_fit_data[n + 1].K_Y2 - line_fit_data[n].K_Y2) * Y_per + line_fit_data[n].K_Y2;
            K_y3 = (line_fit_data[n + 1].K_Y3 - line_fit_data[n].K_Y3) * Y_per + line_fit_data[n].K_Y3;
            K_y4 = (line_fit_data[n + 1].K_Y4 - line_fit_data[n].K_Y4) * Y_per + line_fit_data[n].K_Y4;
            B_y = (line_fit_data[n + 1].Delta_Y - line_fit_data[n].Delta_Y) * Y_per + line_fit_data[n].Delta_Y;
            //计算结果
            Result = new Vector(K_x4 * x * x * x * x + K_x3 * x * x * x + K_x2 * x * x + K_x1 * x + B_x, K_y4 * y * y * y * y + K_y3 * y * y * y + K_y2 * y * y + K_y1 * y + B_y);
#if DEBUG
            string sdatetime = DateTime.Now.ToString("D");
            string delimiter = ",";
            string strHeader = "";
            //保存的位置和文件名称
            string File_Path = @"./\Config/" + "Gts_Correct_Line_Fit " + sdatetime + ".csv";
            strHeader += "原X坐标,原Y坐标,补偿后X坐标,补偿后Y坐标,补偿前后X差值,补偿前后Y差值,取值坐标X位置,取值坐标Y位置";
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
                            + Result.X+ delimiter
                            + Result.Y + delimiter
                            + (Result.X - x) + delimiter
                            + (Result.Y - y) + delimiter
                            + m + delimiter
                            + n + delimiter;
            sw.WriteLine(strRowValue);
            sw.Close();
#endif
            //返回实际坐标
            return Result;
        }
        //获取线性补偿坐标
        public static Vector Get_Line_Fit_Coordinate_MA(decimal x, decimal y, List<Double_Fit_Data> line_fit_data)
        {
            Vector Result = new Vector();
            //临时定位变量
            Int16 m, n;
            decimal X_per, Y_per;
            decimal K_x1, K_x2, K_x3, K_x4, B_x;
            decimal K_y1, K_y2, K_y3, K_y4, B_y;

            //获取落点
            m = Seek_X_Pos(y);
            n = Seek_Y_Pos(x);
            //计算比率
            X_per = Math.Abs(y - m * Para_List.Parameter.Gts_Calibration_Cell) / Para_List.Parameter.Gts_Calibration_Cell;
            Y_per = Math.Abs(x - n * Para_List.Parameter.Gts_Calibration_Cell) / Para_List.Parameter.Gts_Calibration_Cell;
            //计算拟合参数
            K_x1 = (line_fit_data[m + 1].K_X1 - line_fit_data[m].K_X1) * X_per + line_fit_data[m].K_X1;
            K_x2 = (line_fit_data[m + 1].K_X2 - line_fit_data[m].K_X2) * X_per + line_fit_data[m].K_X2;
            K_x3 = (line_fit_data[m + 1].K_X3 - line_fit_data[m].K_X3) * X_per + line_fit_data[m].K_X3;
            K_x4 = (line_fit_data[m + 1].K_X4 - line_fit_data[m].K_X4) * X_per + line_fit_data[m].K_X4;
            B_x = (line_fit_data[m + 1].Delta_X - line_fit_data[m].Delta_X) * X_per + line_fit_data[m].Delta_X;
            K_y1 = (line_fit_data[n + 1].K_Y1 - line_fit_data[n].K_Y1) * Y_per + line_fit_data[n].K_Y1;
            K_y2 = (line_fit_data[n + 1].K_Y2 - line_fit_data[n].K_Y2) * Y_per + line_fit_data[n].K_Y2;
            K_y3 = (line_fit_data[n + 1].K_Y3 - line_fit_data[n].K_Y3) * Y_per + line_fit_data[n].K_Y3;
            K_y4 = (line_fit_data[n + 1].K_Y4 - line_fit_data[n].K_Y4) * Y_per + line_fit_data[n].K_Y4;
            B_y = (line_fit_data[n + 1].Delta_Y - line_fit_data[n].Delta_Y) * Y_per + line_fit_data[n].Delta_Y;
            //计算结果
            Result = new Vector(K_x4 * x * x * x * x + K_x3 * x * x * x + K_x2 * x * x + K_x1 * x + B_x, K_y4 * y * y * y * y + K_y3 * y * y * y + K_y2 * y * y + K_y1 * y + B_y);
#if DEBUG
            string sdatetime = DateTime.Now.ToString("D");
            string delimiter = ",";
            string strHeader = "";
            //保存的位置和文件名称
            string File_Path = @"./\Config/" + "Gts_Correct_Line_Fit " + sdatetime + ".csv";
            strHeader += "原X坐标,原Y坐标,补偿后X坐标,补偿后Y坐标,补偿前后X差值,补偿前后Y差值,取值坐标X位置,取值坐标Y位置";
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
                            + Result.X+ delimiter
                            + Result.Y + delimiter
                            + (Result.X - x) + delimiter
                            + (Result.Y - y) + delimiter
                            + m + delimiter
                            + n + delimiter;
            sw.WriteLine(strRowValue);
            sw.Close();
#endif
            //返回实际坐标
            return Result;
        }
        ///get point affinity point'
        ///
        public static Vector Get_Aff_After(Vector src,Affinity_Matrix affinity_Matrices)
        { 
            return new Vector(src.X * affinity_Matrices.Stretch_X + src.Y * affinity_Matrices.Distortion_X + affinity_Matrices.Delta_X,src.Y * affinity_Matrices.Stretch_Y + src.X * affinity_Matrices.Distortion_Y + affinity_Matrices.Delta_Y);
        }
        //获取Affinity补偿坐标
        public static Vector Get_Affinity_Point(int type,decimal x, decimal y, List<Affinity_Matrix> affinity_Matrices)//0-A_M,1-M_A
        {
            Vector Result = new Vector();
            //临时定位变量
            Int16 m, n;
            //获取落点
            m = Seek_X_Pos(x);
            n = Seek_Y_Pos(y);
            if (type == 1) //Motor_Coordinate ---- Actual_Coordinate
            {
                //终点计算
                if (affinity_Matrices.Count > 1)
                {
                    Result = new Vector(x * affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Stretch_X - y * affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Distortion_X + affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Delta_X, y * affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Stretch_Y - x * affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Distortion_Y + affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Delta_Y);
                }
                else if ((affinity_Matrices.Count > 0) && (affinity_Matrices.Count == 1))
                {
                    Result = new Vector(x * affinity_Matrices[0].Stretch_X - y * affinity_Matrices[0].Distortion_X + affinity_Matrices[0].Delta_X, y * affinity_Matrices[0].Stretch_Y - x * affinity_Matrices[0].Distortion_Y + affinity_Matrices[0].Delta_Y);
                }
            }
            else
            {
                //终点计算
                if (affinity_Matrices.Count > 1)
                {
                    Result = new Vector(x * affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Stretch_X + y * affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Distortion_X + affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Delta_X, y * affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Stretch_Y + x * affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Distortion_Y + affinity_Matrices[n * Para_List.Parameter.Gts_Affinity_Col + m].Delta_Y);
                }
                else if ((affinity_Matrices.Count > 0) && (affinity_Matrices.Count == 1))
                {
                    Result = new Vector(x * affinity_Matrices[0].Stretch_X + y * affinity_Matrices[0].Distortion_X + affinity_Matrices[0].Delta_X, y * affinity_Matrices[0].Stretch_Y + x * affinity_Matrices[0].Distortion_Y + affinity_Matrices[0].Delta_Y);
                }
            }
            
#if DEBUG
            string sdatetime = DateTime.Now.ToString("D");
            string delimiter = ",";
            string strHeader = "";
            //保存的位置和文件名称
            string File_Path = @"./\Config/" + "Gts_Correct_Line_Fit " + sdatetime + ".csv";
            strHeader += "原X坐标,原Y坐标,补偿后X坐标,补偿后Y坐标,补偿前后X差值,补偿前后Y差值,取值坐标X位置,取值坐标Y位置";
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
                            + Result.X+ delimiter
                            + Result.Y + delimiter
                            + (Result.X - x) + delimiter
                            + (Result.Y - y) + delimiter
                            + m + delimiter
                            + n + delimiter;
            sw.WriteLine(strRowValue);
            sw.Close();
#endif
            //返回实际坐标
            return Result;
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
            short Line_Re = 1;
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
            m = Seek_X_Pos(y);
            n = Seek_Y_Pos(x);
            //计算比率
            X_per = Math.Abs(y - m * Para_List.Parameter.Rtc_Calibration_Cell) / Para_List.Parameter.Rtc_Calibration_Cell;
            Y_per = Math.Abs(x - m * Para_List.Parameter.Rtc_Calibration_Cell) / Para_List.Parameter.Rtc_Calibration_Cell;
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
        //获取Affinity补偿坐标
        public static Vector Get_Affinity_Point(int type, decimal x, decimal y, List<Affinity_Matrix> affinity_Matrices)//0-A_M,1-M_A
        {
            Vector Result = new Vector();
            //临时定位变量
            Int16 m, n;
            //获取落点
            m = Seek_X_Pos(x);
            n = Seek_Y_Pos(y);
            if (type == 1) //Motor_Coordinate ---- Actual_Coordinate
            {
                //终点计算
                if (affinity_Matrices.Count > 1)
                {
                    Result = new Vector(x * affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Stretch_X - y * affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Distortion_X + affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Delta_X, y * affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Stretch_Y - x * affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Distortion_Y + affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Delta_Y);
                }
                else if ((affinity_Matrices.Count > 0) && (affinity_Matrices.Count == 1))
                {
                    Result = new Vector(x * affinity_Matrices[0].Stretch_X - y * affinity_Matrices[0].Distortion_X + affinity_Matrices[0].Delta_X, y * affinity_Matrices[0].Stretch_Y - x * affinity_Matrices[0].Distortion_Y + affinity_Matrices[0].Delta_Y);
                }
            }
            else
            {
                //终点计算
                if (affinity_Matrices.Count > 1)
                {
                    Result = new Vector(x * affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Stretch_X + y * affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Distortion_X + affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Delta_X, y * affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Stretch_Y + x * affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Distortion_Y + affinity_Matrices[n * Para_List.Parameter.Rtc_Affinity_Col + m].Delta_Y);
                }
                else if ((affinity_Matrices.Count > 0) && (affinity_Matrices.Count == 1))
                {
                    Result = new Vector(x * affinity_Matrices[0].Stretch_X + y * affinity_Matrices[0].Distortion_X + affinity_Matrices[0].Delta_X, y * affinity_Matrices[0].Stretch_Y + x * affinity_Matrices[0].Distortion_Y + affinity_Matrices[0].Delta_Y);
                }
            }

#if DEBUG
            string sdatetime = DateTime.Now.ToString("D");
            string delimiter = ",";
            string strHeader = "";
            //保存的位置和文件名称
            string File_Path = @"./\Config/" + "Gts_Correct_Line_Fit " + sdatetime + ".csv";
            strHeader += "原X坐标,原Y坐标,补偿后X坐标,补偿后Y坐标,补偿前后X差值,补偿前后Y差值,取值坐标X位置,取值坐标Y位置";
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
                            + Result.X+ delimiter
                            + Result.Y + delimiter
                            + (Result.X - x) + delimiter
                            + (Result.Y - y) + delimiter
                            + m + delimiter
                            + n + delimiter;
            sw.WriteLine(strRowValue);
            sw.Close();
#endif
            //返回实际坐标
            return Result;
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
