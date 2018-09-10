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
   
   public class Generate_Affinity_Matrix
    {
        //生成所需的函数
        GTS_Fun.Motion motion = new GTS_Fun.Motion();
        GTS_Fun.Axis_Home axis01_Home = new GTS_Fun.Axis_Home();
        GTS_Fun.Axis_Home axis02_Home = new GTS_Fun.Axis_Home();
        GTS_Fun.Interpolation interpolation = new GTS_Fun.Interpolation();

        //定义退出变量
        public static bool Exit_Flag = false;
        public List<Correct_Data> Get_Datas()
        {
            //序列化
            Serialize_Data Save_Data = new Serialize_Data();
            //建立变量
            List<Correct_Data> Result = new List<Correct_Data>();
            Correct_Data Temp_Correct_Data = new Correct_Data();

            //建立变量
            decimal Cam_Old_X = 0, Cam_Old_Y = 0;
            decimal Cam_New_X = 0, Cam_New_Y = 0;
            decimal Cam_Delta_X = 0, Cam_Delta_Y = 0;

            int i = 0, j = 0;
            //两轴回零
            Thread Axis01_home_thread = new Thread(this.Axis01_Home);
            Thread Axis02_home_thread = new Thread(this.Axis02_Home);
            Axis01_home_thread.Start();
            Axis02_home_thread.Start();
            //等待线程结束
            Axis01_home_thread.Join();
            Axis02_home_thread.Join();

            //建立直角坐标系
            interpolation.Coordination(Para_List.Parameter.Work_X, Para_List.Parameter.Work_Y);
            //定位到加工坐标原点
            interpolation.Clear_FIFO();
            interpolation.Line_FIFO(0, 0);//将直线插补数据写入
            interpolation.Interpolation_Start();
            //停止坐标系运动
            interpolation.Interpolation_Stop();
            
            //1轴-x轴，2轴-y轴，X轴归零，y轴归 步距*i
            //motion.Abs(1, Convert.ToDouble(500 / Para_List.Parameter.Acc_reference), Convert.ToDouble(500 / Para_List.Parameter.Acc_reference), 2, Convert.ToInt32(Para_List.Parameter.Work_X), Convert.ToDouble(100 / Para_List.Parameter.Vel_reference));//绝对定位至坐标系X为零
            //motion.Abs(2, Convert.ToDouble(500 / Para_List.Parameter.Acc_reference), Convert.ToDouble(500 / Para_List.Parameter.Acc_reference), 2, Convert.ToInt32(Para_List.Parameter.Work_Y), Convert.ToDouble(100 / Para_List.Parameter.Vel_reference));//绝对定位至坐标系Y为零

            //2.5mm步距进行数据提取和整合，使用INC指令
            for (i = 0; i < Para_List.Parameter.Calibration_Row; i++)
            {
                //1轴-x轴，2轴-y轴，X轴归零，y轴归 步距*i
                //定位实现
                //motion.Abs(1, 500, 500, 2, Para_List.Parameter.Work_X,100);//绝对定位至坐标系X为零
                //motion.Inc(2, 500, 500, 2, -Para_List.Parameter.Calibration_Cell, 100);//循序渐加
                //插补运动实现
                interpolation.Clear_FIFO();
                interpolation.Line_FIFO(0, i * Para_List.Parameter.Calibration_Cell);//将直线插补数据写入
                interpolation.Interpolation_Start();
                interpolation.Interpolation_Stop();
                for (j = 0; j < Para_List.Parameter.Calibration_Col; j++)
                {
                    //清空Temp_Correct_Data
                    Temp_Correct_Data.Empty();
                    //定位X轴
                    //定位实现
                    //motion.Inc(1, 500, 500, 2, -Para_List.Parameter.Calibration_Cell, 100);
                    //插补运动实现
                    interpolation.Clear_FIFO();
                    interpolation.Line_FIFO(j * Para_List.Parameter.Calibration_Cell, i * Para_List.Parameter.Calibration_Cell);//将直线插补数据写入
                    interpolation.Interpolation_Start();
                    interpolation.Interpolation_Stop();
                    //调用相机，获取对比的坐标信息
                    Common_Method.Delay_Time.Delay(200);//延时200ms
                    //Main.t
                    //T_Client.
                    Main.T_Client.Senddata(1);//触发拍照
                    do
                    {

                    } while (!Main.T_Client.Rec_Ok);
                    Cam_New_X = Main.T_Client.Receive_Cordinate.X;
                    Cam_New_Y = Main.T_Client.Receive_Cordinate.Y;
                    //计算差值
                    Para_List.Parameter.Cam_Reference = 0.008806m;//1像素=0.008806mm ()

                    Cam_Delta_X = (Cam_New_X - Cam_Old_X) * Para_List.Parameter.Cam_Reference;
                    Cam_Delta_Y = (Cam_New_Y - Cam_Old_Y) * Para_List.Parameter.Cam_Reference;


                    //数据保存
                    Temp_Correct_Data.Xo = j * Para_List.Parameter.Calibration_Cell + Cam_Delta_X;//相机实际X坐标
                    Temp_Correct_Data.Yo = i * Para_List.Parameter.Calibration_Cell + Cam_Delta_Y;//相机实际Y坐标
                    Temp_Correct_Data.Xm = j * Para_List.Parameter.Calibration_Cell;//平台电机实际X坐标
                    Temp_Correct_Data.Ym = i * Para_List.Parameter.Calibration_Cell;//平台电机实际Y坐标

                    //save_csv(Temp_Correct_Data, Convert.ToString(Cam_New_X), Convert.ToString(Cam_New_Y));

                    //New变Old
                    Cam_Old_X = Cam_New_X;
                    Cam_Old_Y = Cam_New_Y;
                    //添加进入List
                    Result.Add(Temp_Correct_Data);

                    //线程终止
                    if (Exit_Flag)
                    {
                        Exit_Flag = false;
                        return Result;
                    }

                }
            }
            //保存文件至Config
            Save_Data.Serialize_Correct_Data(Result, "Correct_Data.xml");
            return Result;
        }

        //0829保存数据到 .CSV
        private void save_csv(Correct_Data resultdata, String newdataX, String newdataY) //type:0:分组，1：qa
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
                         + newdataY ;
            sw.WriteLine(strRowValue);
            sw.Close();
            //MessageBox.Show("保存成功！");
        }
        /// <summary>
        /// 求Mark点集合对应平台坐标
        /// </summary>
        /// <param name="Mark_data_dxf"><dxf中Mark点坐标/param>
        /// <returns></returns>
        public PointF[] Get_Mark_Datas(List<PointF> Mark_data_dxf)
        {         
            List<PointF> mark_data_Minition = new List<PointF>();
            PointF[] point_mark = new PointF[3];
            
            //红点到相机中心的距离
            decimal const_distance_X = 0;
            decimal const_distance_Y = 0;

            //三个点之间X，Y方向 的距离
            decimal Mark_distance_X = (decimal)(Mark_data_dxf[2].X - Mark_data_dxf[0].X);
            decimal Mark_distance_Y = (decimal)(Mark_data_dxf[2].Y - Mark_data_dxf[0].Y);

            //1复位-建立坐标系 
            //2 mark点对位 手动对位
            //插补运动实现 走到第2个Mark点对应红点位置 （左上） 坐标：获取平台坐标+距离 

            //获取坐标
            GTS.MC.GT_GetCrdPos(0, out double temp_X);
            GTS.MC.GT_GetCrdPos(1, out double temp_Y);

            interpolation.Clear_FIFO();
            interpolation.Line_FIFO((decimal)temp_X+const_distance_X, (decimal)temp_Y+const_distance_Y);//将直线插补数据写入
            interpolation.Interpolation_Start();
            interpolation.Interpolation_Stop();
            //T_Client.
            Main.T_Client.Senddata(2);//触发拍照
            do
            {

            } while (!Main.T_Client.Rec_Ok);

            decimal Cam_New_X = Main.T_Client.Receive_Cordinate.X * Para_List.Parameter.Cam_Reference;
            decimal Cam_New_Y = Main.T_Client.Receive_Cordinate.Y * Para_List.Parameter.Cam_Reference;
            GTS.MC.GT_GetCrdPos(0, out  temp_X);
            GTS.MC.GT_GetCrdPos(1, out  temp_Y);
            point_mark[1].X = (float)((decimal)temp_X + Cam_New_X);
            point_mark[1].Y = (float)((decimal)temp_Y + Cam_New_Y);

            //右上
            interpolation.Clear_FIFO();
            interpolation.Line_FIFO((decimal)temp_X+Mark_distance_X,(decimal) temp_Y+0);//将直线插补数据写入
            interpolation.Interpolation_Start();
            interpolation.Interpolation_Stop();
            //T_Client.
            Main.T_Client.Senddata(2);//触发拍照
            do
            {

            } while (!Main.T_Client.Rec_Ok);

            Cam_New_X = Main.T_Client.Receive_Cordinate.X * Para_List.Parameter.Cam_Reference;
            Cam_New_Y = Main.T_Client.Receive_Cordinate.Y * Para_List.Parameter.Cam_Reference;
            GTS.MC.GT_GetCrdPos(0, out temp_X);
            GTS.MC.GT_GetCrdPos(1, out temp_Y);
            point_mark[2].X = (float)((decimal)temp_X + Cam_New_X);
            point_mark[2].Y = (float)((decimal)temp_Y + Cam_New_Y);

            //左下
            interpolation.Clear_FIFO();
            interpolation.Line_FIFO((decimal)temp_X - Mark_distance_X, (decimal)temp_Y + Mark_distance_Y);//将直线插补数据写入
            interpolation.Interpolation_Start();
            interpolation.Interpolation_Stop();
            //T_Client.
            Main.T_Client.Senddata(2);//触发拍照
            do
            {

            } while (!Main.T_Client.Rec_Ok);

            Cam_New_X = Main.T_Client.Receive_Cordinate.X * Para_List.Parameter.Cam_Reference;
            Cam_New_Y = Main.T_Client.Receive_Cordinate.Y * Para_List.Parameter.Cam_Reference;
            GTS.MC.GT_GetCrdPos(0, out temp_X);
            GTS.MC.GT_GetCrdPos(1, out temp_Y);
            point_mark[2].X = (float)((decimal)temp_X + Cam_New_X);
            point_mark[2].Y = (float)((decimal)temp_Y + Cam_New_Y);

            return point_mark;
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

        //处理相机与轴的数据，生成仿射变换数组，并保存进入文件
        public List<Affinity_Matrix> Resolve(List<Correct_Data> correct_Datas)
        {
            
            //建立变量
            List<Affinity_Matrix> Result = new List<Affinity_Matrix>();
            Affinity_Matrix Temp_Affinity_Matrix=new Affinity_Matrix();
            Int16 i, j; 
            //定义仿射变换数组 
            Mat mat = new Mat(new Size(3, 2), Emgu.CV.CvEnum.DepthType.Cv32F, 1); //2行 3列 的矩阵
            //定义点位数组
            PointF[] srcTri = new PointF[3];//标准数据
            PointF[] dstTri = new PointF[3];//差异化数据
            double[] temp_array;
            //序列化
            Serialize_Data Save_Data = new Serialize_Data();

            if (Para_List.Parameter.Calibration_Col* Para_List.Parameter.Calibration_Row == correct_Datas.Count)//矫正和差异数据完整
            {
                //数据处理
                for (i = 0; i < Para_List.Parameter.Calibration_Row - 1; i++)
                {
                    for (j = 0; j < Para_List.Parameter.Calibration_Col - 1; j++)
                    {
                        //标准数据
                        srcTri[0] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Calibration_Col].Xo), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Calibration_Col].Yo));
                        srcTri[1] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Calibration_Col + 1].Xo), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Calibration_Col + 1].Yo));
                        srcTri[2] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Calibration_Col + Para_List.Parameter.Calibration_Row].Xo), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Calibration_Col + Para_List.Parameter.Calibration_Row].Yo));
                        //仿射数据
                        dstTri[0] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Calibration_Col].Xm), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Calibration_Col].Ym));
                        dstTri[1] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Calibration_Col + 1].Xm), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Calibration_Col + 1].Ym));
                        dstTri[2] = new PointF(Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Calibration_Col + Para_List.Parameter.Calibration_Row].Xm), Convert.ToSingle(correct_Datas[i + j * Para_List.Parameter.Calibration_Col + Para_List.Parameter.Calibration_Row].Ym));
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
                Save_Data.Serialize_Affinity_Matrix(Result, "Affinity_Matrix.xml");
            }
            return Result;
        }
        //8.28
        //dxf 仿射变换 求DX，DY，Dct(sin \cos)
        public Affinity_Matrix Resolve_DxfToM(PointF[] srcTri, PointF[] dstTri)//标准数据 //差异化数据
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

        //矫正Mark坐标
        public void Calibrate_Mark()
        {
            //建立变量
            decimal Cam_X = 0, Cam_Y = 0;
            Vector Coodinate_Point;
            Vector Tem_Mark;
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

            //矫正Mark1
            //定位到Mark1点
            interpolation.Gts_Ready(Para_List.Parameter.Mark1.X, Para_List.Parameter.Mark1.Y);
            //调用相机，获取对比的坐标信息
            Common_Method.Delay_Time.Delay(200);//延时200ms
            //Main.t
            //T_Client.
            Main.T_Client.Senddata(2);//触发拍照 1：标定 2：Mark点
            do
            {

            } while (!Main.T_Client.Rec_Ok);
            Cam_Y = Main.T_Client.Receive_Cordinate.X;
            Cam_X = Main.T_Client.Receive_Cordinate.Y;

            //获取坐标系平台坐标
            Coodinate_Point = new Vector(interpolation.Get_Coordinate());
            //计算偏移
            Tem_Mark = new Vector(Coodinate_Point.X - Cam_X * Para_List.Parameter.Cam_Reference - Para_List.Parameter.Cam_Org_X, Coodinate_Point.Y - Cam_Y * Para_List.Parameter.Cam_Reference - Para_List.Parameter.Cam_Org_Y);
            //反馈回Mark点
            Para_List.Parameter.Mark1 = new Vector(Tem_Mark);

            //定位到Mark1点
            //interpolation.Gts_Ready(Para_List.Parameter.Mark1.X, Para_List.Parameter.Mark1.Y);          

            
            //矫正Mark2
            //定位到Mark2点
            interpolation.Gts_Ready(Para_List.Parameter.Mark2.X, Para_List.Parameter.Mark2.Y);
            //调用相机，获取对比的坐标信息
            Common_Method.Delay_Time.Delay(200);//延时200ms
            //Main.t
            //T_Client.
            Main.T_Client.Senddata(2);//触发拍照 1：标定 2：Mark点
            do
            {

            } while (!Main.T_Client.Rec_Ok);
            Cam_Y = Main.T_Client.Receive_Cordinate.X;
            Cam_X = Main.T_Client.Receive_Cordinate.Y;

            //获取坐标系平台坐标
            Coodinate_Point = new Vector(interpolation.Get_Coordinate());
            //计算偏移
            Tem_Mark = new Vector(Coodinate_Point.X - Cam_X * Para_List.Parameter.Cam_Reference - Para_List.Parameter.Cam_Org_X, Coodinate_Point.Y - Cam_Y * Para_List.Parameter.Cam_Reference - Para_List.Parameter.Cam_Org_Y);
            //反馈回Mark点
            Para_List.Parameter.Mark2 = new Vector(Tem_Mark);
            //MessageBox.Show(string.Format("坐标系 X：{0}，Y：{1}", Coodinate_Point.X, Coodinate_Point.Y));

            //矫正Mark3
            //定位到Mark3点
            interpolation.Gts_Ready(Para_List.Parameter.Mark3.X, Para_List.Parameter.Mark3.Y);
            //调用相机，获取对比的坐标信息
            Common_Method.Delay_Time.Delay(200);//延时200ms
            //Main.t
            //T_Client.
            Main.T_Client.Senddata(2);//触发拍照 1：标定 2：Mark点
            do
            {

            } while (!Main.T_Client.Rec_Ok);
            Cam_Y = Main.T_Client.Receive_Cordinate.X;
            Cam_X = Main.T_Client.Receive_Cordinate.Y;

            //获取坐标系平台坐标
            Coodinate_Point = new Vector(interpolation.Get_Coordinate());
            //计算偏移
            Tem_Mark = new Vector(Coodinate_Point.X - Cam_X * Para_List.Parameter.Cam_Reference - Para_List.Parameter.Cam_Org_X, Coodinate_Point.Y - Cam_Y * Para_List.Parameter.Cam_Reference - Para_List.Parameter.Cam_Org_Y);
            //反馈回Mark点
            Para_List.Parameter.Mark3 = new Vector(Tem_Mark);
            //MessageBox.Show(string.Format("坐标系 X：{0}，Y：{1}", Coodinate_Point.X, Coodinate_Point.Y))

            //计算仿射变换参数
            Para_List.Parameter.Trans_Affinity =new Affinity_Matrix(Cal_Affinity());
            

        }
        //dxf 仿射变换 求DX，DY，Dct(sin \cos)
        public Affinity_Matrix Cal_Affinity()//标准数据 //差异化数据 
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
                Start_m = Convert.ToInt16((O.Start_x + Para_List.Parameter.Cam_Rtc_X) / Para_List.Parameter.Calibration_Cell);
                Start_n = Convert.ToInt16((O.Start_y + Para_List.Parameter.Cam_Rtc_Y) / Para_List.Parameter.Calibration_Cell);
                End_m = Convert.ToInt16((O.End_x + Para_List.Parameter.Cam_Rtc_X) / Para_List.Parameter.Calibration_Cell);
                End_n = Convert.ToInt16((O.End_y + Para_List.Parameter.Cam_Rtc_Y) / Para_List.Parameter.Calibration_Cell); 
                Center_m = Convert.ToInt16((O.Center_x + Para_List.Parameter.Cam_Rtc_X) / Para_List.Parameter.Calibration_Cell);
                Center_n = Convert.ToInt16((O.Center_y + Para_List.Parameter.Cam_Rtc_Y) / Para_List.Parameter.Calibration_Cell);
                //起点计算
                Temp_Data.Start_x = O.Start_x * affinity_Matrices[Start_m * Para_List.Parameter.Affinity_Col + Start_n].Cos_Value + O.Start_y * affinity_Matrices[Start_m * Para_List.Parameter.Affinity_Col + Start_n].Sin_Value + affinity_Matrices[Start_m * Para_List.Parameter.Affinity_Col + Start_n].Delta_X;
                Temp_Data.Start_y = O.Start_y * affinity_Matrices[Start_m * Para_List.Parameter.Affinity_Col + Start_n].Cos_Value - O.Start_x * affinity_Matrices[Start_m * Para_List.Parameter.Affinity_Col + Start_n].Sin_Value + affinity_Matrices[Start_n * Para_List.Parameter.Affinity_Col + Start_n].Delta_Y;
                //终点计算
                Temp_Data.End_x = O.End_x * affinity_Matrices[End_m * Para_List.Parameter.Affinity_Col + End_n].Cos_Value + O.End_y * affinity_Matrices[End_m * Para_List.Parameter.Affinity_Col + End_n].Sin_Value + affinity_Matrices[End_m * Para_List.Parameter.Affinity_Col + End_n].Delta_X;
                Temp_Data.End_y = O.End_y * affinity_Matrices[End_m * Para_List.Parameter.Affinity_Col + End_n].Cos_Value - O.End_x * affinity_Matrices[End_m * Para_List.Parameter.Affinity_Col + End_n].Sin_Value + affinity_Matrices[End_n * Para_List.Parameter.Affinity_Col + End_n].Delta_Y;
                //圆心计算
                Temp_Data.Center_x = O.Center_x * affinity_Matrices[Center_m * Para_List.Parameter.Affinity_Col + Center_n].Cos_Value + O.Center_y * affinity_Matrices[Center_m * Para_List.Parameter.Affinity_Col + Center_n].Sin_Value + affinity_Matrices[Center_m * Para_List.Parameter.Affinity_Col + Center_n].Delta_X;
                Temp_Data.Center_y = O.Center_y * affinity_Matrices[Center_m * Para_List.Parameter.Affinity_Col + Center_n].Cos_Value - O.Center_x * affinity_Matrices[Center_m * Para_List.Parameter.Affinity_Col + Center_n].Sin_Value + affinity_Matrices[Center_n * Para_List.Parameter.Affinity_Col + Center_n].Delta_Y;

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

        private Serialize_Data Read_Data = new Serialize_Data();        
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
            List<Affinity_Matrix> Affinity_Mat = Read_Data.Reserialize_Affinity_Matrix("Affinity_Matrix.xml");
            //获取坐标坐落区域
            Int16 Pos_Row = Convert.ToInt16(x / Para_List.Parameter.Calibration_Cell);
            Int16 Pos_Col = Convert.ToInt16(y / Para_List.Parameter.Calibration_Cell);
            //数据计算  说明：因为是负的角度，故Sin值变为负值
            Result.X = x * Affinity_Mat[Pos_Row * Para_List.Parameter.Affinity_Col + Pos_Col].Cos_Value + y * Affinity_Mat[Pos_Row * Para_List.Parameter.Affinity_Col + Pos_Col].Sin_Value + Affinity_Mat[Pos_Row * Para_List.Parameter.Affinity_Col + Pos_Col].Delta_X;
            Result.Y = y * Affinity_Mat[Pos_Row * Para_List.Parameter.Affinity_Col + Pos_Col].Cos_Value - x * Affinity_Mat[Pos_Row * Para_List.Parameter.Affinity_Col + Pos_Col].Sin_Value + Affinity_Mat[Pos_Col * Para_List.Parameter.Affinity_Col + Pos_Col].Delta_Y;

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
            List<Affinity_Matrix> Affinity_Mat = Read_Data.Reserialize_Affinity_Matrix("Affinity_Matrix.xml");
            //获取坐标坐落区域
            Int16 Pos_Row = Convert.ToInt16(x / Para_List.Parameter.Calibration_Cell);
            Int16 Pos_Col = Convert.ToInt16(y / Para_List.Parameter.Calibration_Cell);
            //数据计算  
            Result.X = x * Affinity_Mat[Pos_Row * Para_List.Parameter.Affinity_Col + Pos_Col].Cos_Value - y * Affinity_Mat[Pos_Row * Para_List.Parameter.Affinity_Col + Pos_Col].Sin_Value + Affinity_Mat[Pos_Row * Para_List.Parameter.Affinity_Col + Pos_Col].Delta_X;
            Result.Y = y * Affinity_Mat[Pos_Row * Para_List.Parameter.Affinity_Col + Pos_Col].Cos_Value + x * Affinity_Mat[Pos_Row * Para_List.Parameter.Affinity_Col + Pos_Col].Sin_Value + Affinity_Mat[Pos_Col * Para_List.Parameter.Affinity_Col + Pos_Col].Delta_Y;

            return Result;
        }
    }



    public class Serialize_Data 
    {
       
        //矫正数据序列化操作
        public void Serialize_Correct_Data(List<Correct_Data> list,string txtFile)
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
        public List<Correct_Data> Reserialize_Correct_Data (string fileName)
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
        public void Save_To_Csv(List<Correct_Data> Data,string fileName)
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
        public void Serialize_Affinity_Matrix(List<Affinity_Matrix> list, string txtFile) 
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
        public List<Affinity_Matrix> Reserialize_Affinity_Matrix(string fileName)  
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
