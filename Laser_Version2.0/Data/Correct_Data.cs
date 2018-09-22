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
                    
                    if ((i==0) && (j == 0))
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
            Int16 i, j;
            //定义仿射变换数组 
            Mat mat = new Mat(new Size(3, 2), Emgu.CV.CvEnum.DepthType.Cv32F, 1); //2行 3列 的矩阵
            //定义点位数组
            PointF[] srcTri = new PointF[3];//标准数据
            PointF[] dstTri = new PointF[3];//差异化数据
            double[] temp_array;

            if (Para_List.Parameter.Gts_Calibration_Col * Para_List.Parameter.Gts_Calibration_Row == correct_Datas.Count)//矫正和差异数据完整
            {
                //数据处理
                for (i = 0; i < Para_List.Parameter.Gts_Calibration_Row - 1; i++)
                {
                    for (j = 0; j < Para_List.Parameter.Gts_Calibration_Col - 1; j++)
                    {
                        //标准数据
                        srcTri[0] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col].Xo), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col].Yo));
                        srcTri[1] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col + 1].Xo), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col + 1].Yo));
                        srcTri[2] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Xo), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Yo));
                        //仿射数据
                        dstTri[0] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col].Xm), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col].Ym));
                        dstTri[1] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col + 1].Xm), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col + 1].Ym));
                        dstTri[2] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Xm), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Gts_Calibration_Col + Para_List.Parameter.Gts_Calibration_Row].Ym));
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
                        Result.Add(new Affinity_Matrix(Temp_Affinity_Matrix));
                        //清除变量
                        Temp_Affinity_Matrix.Empty();
                    }
                }
                //保存为文件
                Serialize_Data.Serialize_Affinity_Matrix(Result, "Gts_Affinity_Matrix.xml");
            }
            return Result;

        }
        //定位坐标 X
        public static Int16 Seek_X_Pos(decimal Pos)
        {
            Int16 Result = 0;
            Result = (Int16)(Pos % Para_List.Parameter.Gts_Calibration_Cell);
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
            Result = (Int16)(Pos % Para_List.Parameter.Gts_Calibration_Cell);
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
            srcTri[0] = new PointF(Convert.ToSingle(Para_List.Parameter.Mark_Dxf1.X), Convert.ToSingle(Para_List.Parameter.Mark_Dxf1.Y));
            srcTri[1] = new PointF(Convert.ToSingle(Para_List.Parameter.Mark_Dxf2.X), Convert.ToSingle(Para_List.Parameter.Mark_Dxf2.Y));
            srcTri[2] = new PointF(Convert.ToSingle(Para_List.Parameter.Mark_Dxf3.X), Convert.ToSingle(Para_List.Parameter.Mark_Dxf3.Y));
            //仿射数据
            dstTri[0] = new PointF(Convert.ToSingle(Para_List.Parameter.Mark1.X), Convert.ToSingle(Para_List.Parameter.Mark1.Y));
            dstTri[1] = new PointF(Convert.ToSingle(Para_List.Parameter.Mark2.X), Convert.ToSingle(Para_List.Parameter.Mark2.Y));
            dstTri[2] = new PointF(Convert.ToSingle(Para_List.Parameter.Mark3.X), Convert.ToSingle(Para_List.Parameter.Mark3.Y));
            //计算仿射变换矩阵
            mat = CvInvoke.GetAffineTransform(srcTri, dstTri);
            //提取矩阵数据
            temp_array = mat.GetDoubleArray();
            //获取仿射变换参数
            Result.Cos_Value = Convert.ToDecimal(temp_array[0]);//余弦值
            Result.Sin_Value = Convert.ToDecimal(temp_array[1]);//正弦值
            Result.Delta_X = Convert.ToDecimal(temp_array[2]);//x方向偏移
            Result.Delta_Y = Convert.ToDecimal(temp_array[5]);//y方向偏移

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
            Int16 i, j;
            //定义仿射变换数组 
            Mat mat = new Mat(new Size(3, 2), Emgu.CV.CvEnum.DepthType.Cv32F, 1); //2行 3列 的矩阵
            //定义点位数组
            PointF[] srcTri = new PointF[3];//标准数据
            PointF[] dstTri = new PointF[3];//差异化数据
            double[] temp_array;

            if (Para_List.Parameter.Rtc_Calibration_Col * Para_List.Parameter.Rtc_Calibration_Row == correct_Datas.Count)//矫正和差异数据完整
            {
                //数据处理
                for (i = 0; i < Para_List.Parameter.Rtc_Calibration_Row - 1; i++)
                {
                    for (j = 0; j < Para_List.Parameter.Rtc_Calibration_Col - 1; j++)
                    {
                        //标准数据
                        srcTri[0] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Rtc_Calibration_Col].Xo), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Rtc_Calibration_Col].Yo));
                        srcTri[1] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Rtc_Calibration_Col + 1].Xo), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Rtc_Calibration_Col + 1].Yo));
                        srcTri[2] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row].Xo), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row].Yo));
                        //仿射数据
                        dstTri[0] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Rtc_Calibration_Col].Xm), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Rtc_Calibration_Col].Ym));
                        dstTri[1] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Rtc_Calibration_Col + 1].Xm), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Rtc_Calibration_Col + 1].Ym));
                        dstTri[2] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row].Xm), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Rtc_Calibration_Col + Para_List.Parameter.Rtc_Calibration_Row].Ym));
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
                        Result.Add(new Affinity_Matrix(Temp_Affinity_Matrix));
                        //清除变量
                        Temp_Affinity_Matrix.Empty();
                    }
                }
                //保存为文件
                Serialize_Data.Serialize_Affinity_Matrix(Result, "Rtc_Affinity_Matrix.xml");
            }
            return Result;
        }
        //定位坐标 X
        public static Int16 Seek_X_Pos(decimal Pos)
        {
            Int16 Result = 0;
            Result = (Int16)((Pos + 25 ) % Para_List.Parameter.Rtc_Calibration_Cell);
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
            Result = (Int16)((Pos + 25) % Para_List.Parameter.Rtc_Calibration_Cell);
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
