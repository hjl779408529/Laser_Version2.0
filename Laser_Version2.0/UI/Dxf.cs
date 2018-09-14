using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using netDxf;
using netDxf.Header;
using netDxf.Blocks;
using netDxf.Collections;
using netDxf.Entities;
using netDxf.Objects;
using netDxf.Tables;
using netDxf.Units;
using System.IO;
using System.Timers;
using CCWin;
using Emgu.CV;
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.Util;

//多段线、多边形、样条曲线---进行圆弧处理数据
public struct Lwpolyline_Arc
{
    public double Radius;
    public Vector3 Center;
    public Vector3 StartPoint;
    public Vector3 EndPoint;
    public double StartAngle;
    public double EndAAngle;
    
}
namespace Laser_Build_1._0
{
    public partial class Dxf : Form
    {
        public Dxf()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;//关闭CheckForIllegalCrossThreadCalls
        }

        Display_Dxf display_dxf;//dxf文件显示

        //定义图像中心点
        decimal Pic_Center_x, Pic_Center_y;
        //定义文件名 
        string Dxf_filename = "sample.dxf";
       

        //建立定时器
        System.Timers.Timer Refresh_Timer = new System.Timers.Timer(200);


        //建立插补数据组Para_List.Parameter.
        List<Interpolation_Data> Arc_Line_Data = new List<Interpolation_Data>();
        List<Interpolation_Data> Circle_Data = new List<Interpolation_Data>();
        List<Interpolation_Data> LwPolyline_Data = new List<Interpolation_Data>();
        List<Interpolation_Data> Concat_Data = new List<Interpolation_Data>();
        List<Interpolation_Data> Compensation_Concat_Data = new List<Interpolation_Data>();
        //建立临时插补Data
        Interpolation_Data temp_Data = new Interpolation_Data();

        //建立整合数据组Para_List.Parameter.
        List<List<Interpolation_Data>> Arc_Line_List_Data = new List<List<Interpolation_Data>>();
        List<List<Interpolation_Data>> Circle_List_Data = new List<List<Interpolation_Data>>();
        List<List<Interpolation_Data>> LwPolyline_List_Data = new List<List<Interpolation_Data>>();
        List<List<Interpolation_Data>> Concat_List_Data = new List<List<Interpolation_Data>>();
        List<List<Interpolation_Data>> Compensation_Concat_List_Data = new List<List<Interpolation_Data>>();
        //建立临时插补Data
        List<Interpolation_Data> temp_List_Data = new List<Interpolation_Data>();

        //rtc和gts解析数据
        List<List<Interpolation_Data>> Rtc_List_Data = new List<List<Interpolation_Data>>();

        //定义整合加工函数
        Integrated integrated = new Integrated();
        //定义补偿函数 
        Data_Resolve Data_Cal = new Data_Resolve();

        //8.31-sta
        List<Entity_Data> Mark_Circle_Entity_Data = new List<Entity_Data>();//mark 点数据收集
        //8.31-end

        //生成所需的函数
        GTS_Fun.Motion motion = new GTS_Fun.Motion();
        GTS_Fun.Axis_Home axis01_Home = new GTS_Fun.Axis_Home();
        GTS_Fun.Axis_Home axis02_Home = new GTS_Fun.Axis_Home();
        GTS_Fun.Interpolation interpolation = new GTS_Fun.Interpolation();
        private void Dxf_Load(object sender, EventArgs e)
        {
            //启用定时器
            Refresh_Timer.Elapsed += Refresh_Timer_Elapsed_Thread;
            Refresh_Timer.AutoReset = true;
            Refresh_Timer.Enabled = true;
            Refresh_Timer.Start();

            //初始数据刷新
            //工件坐标系偏移
            textBox4.Text = Convert.ToString(Para_List.Parameter.Work.X);
            textBox3.Text = Convert.ToString(Para_List.Parameter.Work.Y);
            //直线插补
            textBox2.Text = Convert.ToString(Para_List.Parameter.Line_synVel);
            textBox1.Text = Convert.ToString(Para_List.Parameter.Line_synAcc);
            //圆弧插补
            textBox6.Text = Convert.ToString(Para_List.Parameter.Circle_synVel);
            textBox5.Text = Convert.ToString(Para_List.Parameter.Circle_synAcc);
            //角度补偿值
            textBox7.Text = Convert.ToString(Para_List.Parameter.Arc_Compensation_A);

            //刀具绝对坐标
            textBox9.Text = Convert.ToString(Para_List.Parameter.Laser.X);
            textBox8.Text = Convert.ToString(Para_List.Parameter.Laser.Y);

            //坐标运动平滑系数
            textBox12.Text = Convert.ToString(Para_List.Parameter.Syn_EvenTime);

            //偏移量
            textBox11.Text = Convert.ToString(Para_List.Parameter.Delta.X);
            textBox10.Text = Convert.ToString(Para_List.Parameter.Delta.Y);

            //插补终止速度
            textBox14.Text = Convert.ToString(Para_List.Parameter.Line_endVel); 
            textBox13.Text = Convert.ToString(Para_List.Parameter.Circle_endVel);

            //校准步距
            textBox15.Text = Convert.ToString(Para_List.Parameter.Calibration_Cell);

            //标定板尺寸
            textBox17.Text = Convert.ToString(Para_List.Parameter.Calibration_X_Len);
            textBox16.Text = Convert.ToString(Para_List.Parameter.Calibration_Y_Len);

            //相机与振镜 中心差值
            textBox19.Text = Convert.ToString(Para_List.Parameter.Cam_Rtc.X);
            textBox18.Text = Convert.ToString(Para_List.Parameter.Cam_Rtc.Y);

            //RTCjiaozhun
            textBox21.Text = Convert.ToString(Para_List.Parameter.Rtc_Cal_Radius);
            textBox20.Text = Convert.ToString(Para_List.Parameter.Rtc_Cal_Interval);
        }
        //线程函数
        private void Refresh_Timer_Elapsed_Thread(object sender, ElapsedEventArgs e)
        {
            Refresh_Timer_Elapsed();
        }
        //定时器线程函数
        private void Refresh_Timer_Elapsed()
        {
            /*
            this.Invoke((EventHandler)delegate {
               
                //工件坐标系偏移
                textBox4.Text = Convert.ToString(Para_List.Parameter.Work_X);
                textBox3.Text = Convert.ToString(Para_List.Parameter.Work_Y);
                //直线插补
                textBox2.Text = Convert.ToString(Para_List.Parameter.Line_synVel);
                textBox1.Text = Convert.ToString(Para_List.Parameter.Line_synAcc);
                //圆弧插补
                textBox6.Text = Convert.ToString(Para_List.Parameter.Circle_synVel);
                textBox5.Text = Convert.ToString(Para_List.Parameter.Circle_synAcc);
                //角度补偿值
                textBox7.Text = Convert.ToString(Para_List.Parameter.Arc_Compensation_A);

                //刀具绝对坐标
                textBox9.Text = Convert.ToString(Para_List.Parameter.Laser_X);
                textBox8.Text = Convert.ToString(Para_List.Parameter.Laser_Y);

                
            });
            */
             
        }

        
        //获取文件名
        private void button6_Click(object sender, EventArgs e)
        {
            //获取文件名
            OpenFileDialog openfile = new OpenFileDialog
            {
                //openfile.Filter = "*.dxf";//设置文件后缀名
                Filter = "dxf 文件(*.dxf)|*.dxf"
            };
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                Dxf_filename = openfile.FileName;
                label2.Text = Dxf_filename;
                //richTextBox1.Text =  richTextBox1.Text + Dxf_filename+ "\r\n";
                if (display_dxf == null)
                {
                    display_dxf = new Display_Dxf();
                    display_dxf.Show();
                }
                else
                {
                    if (display_dxf.IsDisposed)//若子窗体关闭 则打开新子窗体 并显示
                    {
                        display_dxf = new Display_Dxf();
                        display_dxf.Show();
                    }
                    else
                    {
                        display_dxf.Activate();//使子窗体获得焦点
                    }
                }
                display_dxf.axMxDrawX1.OpenDwgFile(openfile.FileName);
            }
        }
        //DXF文件数据处理        
        private void button8_Click(object sender, EventArgs e)
        {
            Thread Dxf_Open_thread = new Thread(Dxf_Open);
            Dxf_Open_thread.Start();
        }
        private void Dxf_Open()
        {
            int i = 0, j = 0;//循环数据使用
            int Num = 0;//用于数据记录

            //检查文件是否存在
            FileInfo fileInfo = new FileInfo(Dxf_filename);
            if (!fileInfo.Exists)
            {
                richTextBox1.AppendText(Dxf_filename + "----文件不存在！！！" + "\r\n");
                return;
            }
            DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(Dxf_filename, out bool isBinary);

            // 检查是否是Dxf文件
            if (dxfVersion == DxfVersion.Unknown)
            {
                richTextBox1.AppendText(Dxf_filename + "---文件格式错误" + "\r\n");
                return;
            }

            // 检查Dxf文件版本是否正确
            if (dxfVersion < DxfVersion.AutoCad2000)
            {
                richTextBox1.AppendText(Dxf_filename + "---文件版本不支持" + "\r\n");
                return;
            }

            //读取Dxf文件
            DxfDocument dxf = DxfDocument.Load(Dxf_filename);
            // check if there has been any problems loading the file,
            // this might be the case of a corrupt file or a problem in the library
            if (dxf == null)
            {
                richTextBox1.AppendText("Dxf文件读取失败" + "\r\n");
                return;
            }

            // the dxf has been properly loaded, let's show some information about it
            richTextBox1.AppendText("FILE NAME:" + Dxf_filename + "\r\n");
            richTextBox1.AppendText("FILE VERSION: " + dxf.DrawingVariables.AcadVer + "\r\n");
            richTextBox1.AppendText("FILE COMMENTS: " + dxf.Comments.Count + "\r\n");
            foreach (var o in dxf.Comments)//遍历DXF中的Comments
            {
                richTextBox1.AppendText(o + "\r\n");
            }

            // the entities lists contain the geometry that has a graphical representation in the drawing across all layouts,
            // to get the entities that belongs to a specific layout you can get the references through the Layouts.GetReferences(name)
            // or check the EntityObject.Owner.Record.Layout property
            richTextBox1.AppendText(EntityType.Arc + "--圆弧--count: " + dxf.Arcs.Count + "\r\n");//圆弧
            richTextBox1.AppendText(EntityType.AttributeDefinition + "--count: " + dxf.AttributeDefinitions.Count + "\r\n");
            richTextBox1.AppendText(EntityType.Circle + "--圆--count: " + dxf.Circles.Count + "\r\n");//圆
            richTextBox1.AppendText(EntityType.Ellipse + "--count: " + dxf.Ellipses.Count + "\r\n");
            richTextBox1.AppendText(EntityType.Face3D + "--count: " + dxf.Faces3d.Count + "\r\n");
            richTextBox1.AppendText(EntityType.Hatch + "--count: " + dxf.Hatches.Count + "\r\n");
            richTextBox1.AppendText(EntityType.Image + "--count: " + dxf.Images.Count + "\r\n");
            richTextBox1.AppendText(EntityType.Insert + "--count: " + dxf.Inserts.Count + "\r\n");
            richTextBox1.AppendText(EntityType.Leader + "--count: " + dxf.Leaders.Count + "\r\n");
            richTextBox1.AppendText(EntityType.LightWeightPolyline + "--多边形--count: " + dxf.LwPolylines.Count + "\r\n");//多边形
            richTextBox1.AppendText(EntityType.Line + "--直线--count: " + dxf.Lines.Count + "\r\n");//直线
            richTextBox1.AppendText(EntityType.Mesh + "--count: " + dxf.Meshes.Count + "\r\n");
            richTextBox1.AppendText(EntityType.MLine + "--count: " + dxf.MLines.Count + "\r\n");
            richTextBox1.AppendText(EntityType.MText + "--count: " + dxf.MTexts.Count + "\r\n");
            richTextBox1.AppendText(EntityType.Point + "--count: " + dxf.Points.Count + "\r\n");
            richTextBox1.AppendText(EntityType.PolyfaceMesh + "--count: " + dxf.PolyfaceMeshes.Count + "\r\n");
            richTextBox1.AppendText(EntityType.Polyline + "--count: " + dxf.Polylines.Count + "\r\n");
            richTextBox1.AppendText(EntityType.Solid + "--count: " + dxf.Solids.Count + "\r\n");
            richTextBox1.AppendText(EntityType.Spline + "--count: " + dxf.Splines.Count + "\r\n");
            richTextBox1.AppendText(EntityType.Text + "--count: " + dxf.Texts.Count + "\r\n");
            richTextBox1.AppendText(EntityType.Ray + "--count: " + dxf.Rays.Count + "\r\n");
            richTextBox1.AppendText(EntityType.Underlay + "--count: " + dxf.Underlays.Count + "\r\n");
            richTextBox1.AppendText(EntityType.Viewport + "--count: " + dxf.Viewports.Count + "\r\n");
            richTextBox1.AppendText(EntityType.Wipeout + "--count: " + dxf.Wipeouts.Count + "\r\n");
            richTextBox1.AppendText(EntityType.XLine + "--count: " + dxf.XLines.Count + "\r\n");


            //图层
            richTextBox1.AppendText( "图层--count: " + dxf.Layers.Count + "\r\n");//图层
            foreach (var o in dxf.Layers)
            {
                richTextBox1.AppendText("图层Name: " + o.Name + "\r\n");//图层名称       
            };            

            //建立临时数据存储组
            List<Entity_Data> Arc_Line_Entity_Data = new List<Entity_Data>();//圆弧直线数据
            List<Entity_Data> Circle_Entity_Data = new List<Entity_Data>();//圆数据
            List<Entity_Data> LwPolylines_Entity_Data = new List<Entity_Data>(); //多边形数据

            //建立临时Entity数据
            Entity_Data temp_Entity_Data = new Entity_Data();

            //建立临时整合数据
            List<List<Entity_Data>> Arc_Line_Entity_List_Data = new List<List<Entity_Data>>(); //圆弧直线数据
            List<List<Entity_Data>> Circle_Entity_List_Data = new List<List<Entity_Data>>(); //圆数据
            List<List<Entity_Data>> LwPolylines_Entity_List_Data = new List<List<Entity_Data>>();  //多边形数据
            //建立临时
            List<Interpolation_Data> temp_intepolation_Dat = new List<Interpolation_Data>();

            //清除数据
            Arc_Line_Entity_Data.Clear();
            Arc_Line_Entity_List_Data.Clear();
            temp_Entity_Data.Empty();

            //数据处理，规划路径

            //Arc_Line 直线圆弧数据处理
            //圆弧数据读取
            for (i = 0; i < dxf.Arcs.Count; i++)
            {
                if (dxf.Arcs[i].Layer.Name == "Laser")
                {
                    temp_Entity_Data.Type = 2;//圆弧
                    //使用Gts Pos_reference处理过
                    //temp_Entity_Data.Start_x = Convert.ToDecimal(dxf.Arcs[i].StartPoint.X) * Para_List.Parameter.Pos_reference - Pic_Center_x;
                    //temp_Entity_Data.Start_y = Convert.ToDecimal(dxf.Arcs[i].StartPoint.Y) * Para_List.Parameter.Pos_reference - Pic_Center_y;
                    //temp_Entity_Data.End_x = Convert.ToDecimal(dxf.Arcs[i].EndPoint.X) * Para_List.Parameter.Pos_reference - Pic_Center_x;
                    //temp_Entity_Data.End_y = Convert.ToDecimal(dxf.Arcs[i].EndPoint.Y) * Para_List.Parameter.Pos_reference - Pic_Center_y;
                    //temp_Entity_Data.Cir_Start_Angle = Convert.ToDecimal(dxf.Arcs[i].StartAngle);
                    //temp_Entity_Data.Cir_End_Angle = Convert.ToDecimal(dxf.Arcs[i].EndAngle);
                    //temp_Entity_Data.Center_x = Convert.ToDecimal(dxf.Arcs[i].Center.X) * Para_List.Parameter.Pos_reference - Pic_Center_x;
                    //temp_Entity_Data.Center_y = Convert.ToDecimal(dxf.Arcs[i].Center.Y) * Para_List.Parameter.Pos_reference - Pic_Center_y;
                    //temp_Entity_Data.Circle_radius =Convert.ToDecimal(dxf.Arcs[i].Radius) * Para_List.Parameter.Pos_reference;
                    //原始数据
                    temp_Entity_Data.Start_x = Convert.ToDecimal(dxf.Arcs[i].StartPoint.X);
                    temp_Entity_Data.Start_y = Convert.ToDecimal(dxf.Arcs[i].StartPoint.Y);
                    temp_Entity_Data.End_x = Convert.ToDecimal(dxf.Arcs[i].EndPoint.X);
                    temp_Entity_Data.End_y = Convert.ToDecimal(dxf.Arcs[i].EndPoint.Y);
                    temp_Entity_Data.Cir_Start_Angle = Convert.ToDecimal(dxf.Arcs[i].StartAngle);
                    if (temp_Entity_Data.Cir_Start_Angle >= 359.99m)
                    {
                        temp_Entity_Data.Cir_Start_Angle = 0.0m;
                    }
                    temp_Entity_Data.Cir_End_Angle = Convert.ToDecimal(dxf.Arcs[i].EndAngle);
                    if (temp_Entity_Data.Cir_End_Angle <= 0.01m)
                    {
                        temp_Entity_Data.Cir_End_Angle = 360.0m; 
                    }
                    temp_Entity_Data.Center_x = Convert.ToDecimal(dxf.Arcs[i].Center.X);
                    temp_Entity_Data.Center_y = Convert.ToDecimal(dxf.Arcs[i].Center.Y);
                    temp_Entity_Data.Circle_radius = Convert.ToDecimal(dxf.Arcs[i].Radius);

                    //提交进入Arc_Data
                    Arc_Line_Entity_Data.Add(temp_Entity_Data);
                    temp_Entity_Data.Empty();
                }
            }
                        
            //直线数据读取
            for (i = 0; i < dxf.Lines.Count; i++)
            {
                if (dxf.Lines[i].Layer.Name == "Laser")
                {
                    temp_Entity_Data.Type = 1;
                    //使用Gts Pos_reference处理过
                    //temp_Entity_Data.Start_x = Convert.ToDecimal(dxf.Lines[i].StartPoint.X) * Para_List.Parameter.Pos_reference - Pic_Center_x;
                    //temp_Entity_Data.Start_y = Convert.ToDecimal(dxf.Lines[i].StartPoint.Y) * Para_List.Parameter.Pos_reference - Pic_Center_y;
                    //temp_Entity_Data.End_x = Convert.ToDecimal(dxf.Lines[i].EndPoint.X) * Para_List.Parameter.Pos_reference - Pic_Center_x;
                    //temp_Entity_Data.End_y = Convert.ToDecimal(dxf.Lines[i].EndPoint.Y) * Para_List.Parameter.Pos_reference - Pic_Center_y;
                    //原始数据
                    temp_Entity_Data.Start_x = Convert.ToDecimal(dxf.Lines[i].StartPoint.X);
                    temp_Entity_Data.Start_y = Convert.ToDecimal(dxf.Lines[i].StartPoint.Y);
                    temp_Entity_Data.End_x = Convert.ToDecimal(dxf.Lines[i].EndPoint.X);
                    temp_Entity_Data.End_y = Convert.ToDecimal(dxf.Lines[i].EndPoint.Y);
                    //提交进入Arc_Data
                    Arc_Line_Entity_Data.Add(temp_Entity_Data);
                    temp_Entity_Data.Empty();
                }
            }
            //输出调试信息
            richTextBox1.AppendText("Arc_Data 数量： " + Arc_Line_Entity_Data.Count + "\r\n");

            //圆弧直线数据矫正
            //Arc_Line_Entity_Data =new List<Entity_Data>(Data_Cal.Calibration_Trail(Arc_Line_Entity_Data));//直接矫正路径

            //圆弧直线数据矫正
            Arc_Line_Entity_Data =new List<Entity_Data>(Data_Cal.Calibration_Entity(Arc_Line_Entity_Data,Para_List.Parameter.Trans_Affinity));//直接矫正路径

            int Current_Pos = 0;//用于记录比对插补数据的位置
            temp_intepolation_Dat.Clear();
            Arc_Line_Data.Clear();//清除直线圆弧插补数据
            Arc_Line_List_Data.Clear();//清除直线圆弧插补整合数据

            //处理Line_Arc生成加工数据 初始数据
            if (Arc_Line_Entity_Data.Count > 0)
            {
                //选择任意切入点
                temp_Data.Type = 1;//直线插补
                temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                temp_Data.Lift_flag = 1;//抬刀标志
                temp_Data.Repeat = 0;//重复次数
                temp_Data.End_x = Arc_Line_Entity_Data[0].Start_x;
                temp_Data.End_y = Arc_Line_Entity_Data[0].Start_y;

                Arc_Line_Data.Add(temp_Data);//追加数据

                //整合数据生成代码
                //追加数据 
                temp_intepolation_Dat.Add(temp_Data);
                Arc_Line_List_Data.Add(new List<Interpolation_Data>(temp_intepolation_Dat));//追加数据 

                //清空数据
                temp_Data.Empty();
                temp_intepolation_Dat.Clear();

                i = 0;
                Current_Pos = 0;//记录比对位置Index

                //整理数据
                do
                {

                    Num = Arc_Line_Entity_Data.Count;//记录当前Arc_Line_Entity_Data.Count
                    for (i = 0; i < Arc_Line_Entity_Data.Count; i++)
                    {
                        if (Differ_Err(Arc_Line_Data[Arc_Line_Data.Count - 1].End_x, Arc_Line_Data[Arc_Line_Data.Count - 1].End_y, Arc_Line_Entity_Data[i].End_x, Arc_Line_Entity_Data[i].End_y))//当前插补终点是 数据处理终点 同CAD文件规定方向相反
                        {
                            if (Arc_Line_Entity_Data[i].Type == 1)//直线
                            {
                                temp_Data.Type = 1;//直线插补
                            }
                            else if (Arc_Line_Entity_Data[i].Type == 2)//圆弧
                            {
                                temp_Data.Type = 2;//圆弧插补
                                temp_Data.Circle_radius = Arc_Line_Entity_Data[i].Circle_radius;
                                //圆弧插补 圆心坐标 减去 插补起点坐标
                                temp_Data.Center_Start_x = Arc_Line_Entity_Data[i].Center_x - Arc_Line_Entity_Data[i].End_x;
                                temp_Data.Center_Start_y = Arc_Line_Entity_Data[i].Center_y - Arc_Line_Entity_Data[i].End_y;
                                //圆弧圆心
                                temp_Data.Center_x = Arc_Line_Entity_Data[i].Center_x;
                                temp_Data.Center_y = Arc_Line_Entity_Data[i].Center_y;
                                //计算圆弧角度
                                temp_Data.Angle = Arc_Line_Entity_Data[i].Cir_End_Angle - Arc_Line_Entity_Data[i].Cir_Start_Angle;
                                //圆弧方向
                                temp_Data.Circle_dir = 0;
                            }

                            temp_Data.Lift_flag = 0;//抬刀标志
                            temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                            temp_Data.Repeat = 0;//重复次数
                            //插补起点坐标
                            temp_Data.Start_x = Arc_Line_Entity_Data[i].End_x;
                            temp_Data.Start_y = Arc_Line_Entity_Data[i].End_y;
                            //插补终点坐标
                            temp_Data.End_x = Arc_Line_Entity_Data[i].Start_x;
                            temp_Data.End_y = Arc_Line_Entity_Data[i].Start_y;

                            //提交进入Arc_Data
                            Arc_Line_Data.Add(temp_Data);

                            //整合数据生成代码
                            temp_intepolation_Dat.Add(temp_Data);//追加数据 

                            //清空数据
                            temp_Data.Empty();


                            //删除当前的Entity数据
                            Arc_Line_Entity_Data.RemoveAt(i);
                            break;
                        }
                        else if (Differ_Err(Arc_Line_Data[Arc_Line_Data.Count - 1].End_x, Arc_Line_Data[Arc_Line_Data.Count - 1].End_y, Arc_Line_Entity_Data[i].Start_x, Arc_Line_Entity_Data[i].Start_y)) //当前插补终点是 数据处理起点 同CAD文件规定方向相同
                        {

                            if (Arc_Line_Entity_Data[i].Type == 1)//直线
                            {
                                temp_Data.Type = 1;//直线插补 
                            }
                            else if (Arc_Line_Entity_Data[i].Type == 2)//圆弧
                            {
                                temp_Data.Type = 2;//圆弧插补
                                temp_Data.Circle_radius = Arc_Line_Entity_Data[i].Circle_radius;
                                //圆弧插补 圆心坐标 减去 插补起点坐标
                                temp_Data.Center_Start_x = Arc_Line_Entity_Data[i].Center_x - Arc_Line_Entity_Data[i].Start_x;
                                temp_Data.Center_Start_y = Arc_Line_Entity_Data[i].Center_y - Arc_Line_Entity_Data[i].Start_y;
                                //圆弧圆心
                                temp_Data.Center_x = Arc_Line_Entity_Data[i].Center_x;
                                temp_Data.Center_y = Arc_Line_Entity_Data[i].Center_y;
                                //计算圆弧角度
                                temp_Data.Angle = Arc_Line_Entity_Data[i].Cir_Start_Angle - Arc_Line_Entity_Data[i].Cir_End_Angle;
                                //圆弧方向
                                temp_Data.Circle_dir = 1;

                            }

                            temp_Data.Lift_flag = 0;//抬刀标志
                            temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                            temp_Data.Repeat = 0;//重复次数
                            //插补起点坐标
                            temp_Data.Start_x = Arc_Line_Entity_Data[i].Start_x;
                            temp_Data.Start_y = Arc_Line_Entity_Data[i].Start_y;
                            //插补终点坐标
                            temp_Data.End_x = Arc_Line_Entity_Data[i].End_x;
                            temp_Data.End_y = Arc_Line_Entity_Data[i].End_y;

                            //提交进入Arc_Data
                            Arc_Line_Data.Add(temp_Data);
                            //整合数据生成代码
                            temp_intepolation_Dat.Add(temp_Data);//追加数据

                            //清空数据
                            temp_Data.Empty();

                            //删除当前的Entity数据
                            Arc_Line_Entity_Data.RemoveAt(i);
                            break;
                        }
                    }

                    //寻找结束点失败，意味着重新开始新的 线段或圆弧
                    if ((Arc_Line_Entity_Data.Count != 0) && (Num != 0) && (Num == Arc_Line_Entity_Data.Count))
                    {
                        //整合数据生成代码 当前结束的封闭图形加工数据
                        Arc_Line_List_Data.Add(new List<Interpolation_Data>(temp_intepolation_Dat));//追加数据 
                                                                                                    //清空数据
                        temp_intepolation_Dat.Clear();

                        //跳刀直接使用直线插补走刀
                        //插补进入新的目标起点
                        temp_Data.Type = 1;//直线插补
                        temp_Data.Lift_flag = 1;//抬刀标志
                        temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                        temp_Data.Repeat = 0;//重复次数
                        temp_Data.End_x = Arc_Line_Entity_Data[0].Start_x;
                        temp_Data.End_y = Arc_Line_Entity_Data[0].Start_y;

                        //提交进入Arc_Data
                        Arc_Line_Data.Add(temp_Data);

                        //整合数据生成代码
                        temp_intepolation_Dat.Add(temp_Data);//追加数据
                        Arc_Line_List_Data.Add(new List<Interpolation_Data>(temp_intepolation_Dat));//追加数据 

                        //清空数据
                        temp_Data.Empty();
                        temp_intepolation_Dat.Clear();

                        //记录当前位置比对信息
                        Current_Pos = Arc_Line_Data.Count - 1; //记录比对位置Index

                    }
                    else if ((Arc_Line_Entity_Data.Count == 0) && (Num == 1))//数据整理完成
                    {
                        //整合数据生成代码 当前结束的封闭图形加工数据
                        Arc_Line_List_Data.Add(new List<Interpolation_Data>(temp_intepolation_Dat));//追加数据 
                                                                                                    //清空数据
                        temp_intepolation_Dat.Clear();
                    }
                } while (Arc_Line_Entity_Data.Count > 0);//实体Line_Arc数据未清空完
            }

            //输出调试信息
            richTextBox1.AppendText("处理完 Arc_Line_Entity_Data 数量： " + Arc_Line_Entity_Data.Count + "\r\n");
            richTextBox1.AppendText("处理完 Arc_Line_Data 数量： " + Arc_Line_Data.Count + "\r\n");
            

            //LightWeightPolyline 多边形读取
            if (dxf.LwPolylines.Count > 0)
            {
                
                for (i = 0; i < dxf.LwPolylines.Count; i++)
                {
                    if (dxf.LwPolylines[i].Layer.Name == "Laser")
                    {
                        for (j = 0; j < dxf.LwPolylines[i].Vertexes.Count; j++)
                        {
                            temp_Entity_Data.Type = 1;//直线插补
                            //使用Gts Pos_reference处理过
                            //temp_Entity_Data.Start_x = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j].Position.X) * Para_List.Parameter.Pos_reference - Pic_Center_x;
                            //temp_Entity_Data.Start_y = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j].Position.Y) * Para_List.Parameter.Pos_reference - Pic_Center_y;
                            //if (j <= dxf.LwPolylines[i].Vertexes.Count - 2)
                            //{
                            //    temp_Entity_Data.End_x = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j + 1].Position.X) * Para_List.Parameter.Pos_reference - Pic_Center_x;
                            //    temp_Entity_Data.End_y = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j + 1].Position.Y) * Para_List.Parameter.Pos_reference - Pic_Center_y;
                            //}
                            //else if (j == (dxf.LwPolylines[i].Vertexes.Count - 1))
                            //{
                            //    temp_Entity_Data.End_x = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[0].Position.X) * Para_List.Parameter.Pos_reference - Pic_Center_x;
                            //    temp_Entity_Data.End_y = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[0].Position.Y) * Para_List.Parameter.Pos_reference - Pic_Center_y;
                            //}
                            //原始数据
                            temp_Entity_Data.Start_x = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j].Position.X);
                            temp_Entity_Data.Start_y = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j].Position.Y);
                            if (j <= dxf.LwPolylines[i].Vertexes.Count - 2)
                            {
                                temp_Entity_Data.End_x = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j + 1].Position.X);
                                temp_Entity_Data.End_y = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j + 1].Position.Y);
                            }
                            else if (j == (dxf.LwPolylines[i].Vertexes.Count - 1))
                            {
                                temp_Entity_Data.End_x = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[0].Position.X);
                                temp_Entity_Data.End_y = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[0].Position.Y);
                            }
                            //提交进入LwPolylines_Entity_Data
                            LwPolylines_Entity_Data.Add(temp_Entity_Data);
                            temp_Entity_Data.Empty();
                        }
                    }
                }
                
            }
            //LightWeightPolyline调试信息输出
            richTextBox1.AppendText("多边形线段数： " + LwPolylines_Entity_Data.Count + "\r\n");

            //多边形数据矫正
            //LwPolylines_Entity_Data = new List<Entity_Data>(Data_Cal.Calibration_Trail(LwPolylines_Entity_Data));//直接矫正路径

            //多边形数据矫正
            LwPolylines_Entity_Data = new List<Entity_Data>(Data_Cal.Calibration_Entity(LwPolylines_Entity_Data, Para_List.Parameter.Trans_Affinity));//直接矫正路径

            //生成多边形插补数据
            temp_intepolation_Dat.Clear();
            LwPolyline_Data.Clear();//清除多边形插补数据 
            LwPolyline_List_Data.Clear();//清除多边形插补数据


            //处理LwPolyline_Data加工数据 初始数据
            if (LwPolylines_Entity_Data.Count > 0)
            {
                //选择任意切入点
                temp_Data.Type = 1;//直线插补
                temp_Data.Lift_flag = 1;//抬刀标志
                temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                temp_Data.Repeat = 0;//重复次数
                temp_Data.End_x = LwPolylines_Entity_Data[0].Start_x;
                temp_Data.End_y = LwPolylines_Entity_Data[0].Start_y;

                LwPolyline_Data.Add(temp_Data);//追加数据

                //整合数据生成代码
                temp_intepolation_Dat.Add(temp_Data);//追加数据
                LwPolyline_List_Data.Add(new List<Interpolation_Data>(temp_intepolation_Dat));//追加数据

                //清空数据
                temp_Data.Empty();
                temp_intepolation_Dat.Clear();

                i = 0;
                Current_Pos = 0;//记录比对位置Index

                //多边形数据提取
                do
                {
                    Num = LwPolylines_Entity_Data.Count;//记录当前LwPolylines_Entity_Data.Count
                    for (i = 0; i < LwPolylines_Entity_Data.Count; i++)
                    {
                        if (Differ_Err(LwPolyline_Data[LwPolyline_Data.Count - 1].End_x, LwPolyline_Data[LwPolyline_Data.Count - 1].End_y, LwPolylines_Entity_Data[i].End_x, LwPolylines_Entity_Data[i].End_y))//当前插补终点是 数据处理终点 同CAD文件规定方向相反
                        {
                            temp_Data.Type = 1;//直线插补;

                            temp_Data.Lift_flag = 0;//抬刀标志
                            temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                            temp_Data.Repeat = 0;//重复次数
                            //插补起点
                            temp_Data.Start_x = LwPolylines_Entity_Data[i].End_x;
                            temp_Data.Start_y = LwPolylines_Entity_Data[i].End_y;
                            //插补终点坐标
                            temp_Data.End_x = LwPolylines_Entity_Data[i].Start_x;
                            temp_Data.End_y = LwPolylines_Entity_Data[i].Start_y;

                            //提交进入Arc_Data
                            LwPolyline_Data.Add(temp_Data);

                            //整合数据生成代码
                            temp_intepolation_Dat.Add(temp_Data);//追加数据

                            //清空数据
                            temp_Data.Empty();

                            //删除当前的Entity数据
                            LwPolylines_Entity_Data.RemoveAt(i);

                            //跳出本层循环
                            break;
                        }
                        else if (Differ_Err(LwPolyline_Data[LwPolyline_Data.Count - 1].End_x, LwPolyline_Data[LwPolyline_Data.Count - 1].End_y, LwPolylines_Entity_Data[i].Start_x, LwPolylines_Entity_Data[i].Start_y)) //当前插补终点是 数据处理起点 同CAD文件规定方向相同
                        {

                            temp_Data.Type = 1;//直线插补 

                            temp_Data.Lift_flag = 0;//抬刀标志
                            temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                            temp_Data.Repeat = 0;//重复次数
                            //插补起点
                            temp_Data.Start_x = LwPolylines_Entity_Data[i].Start_x;
                            temp_Data.Start_y = LwPolylines_Entity_Data[i].Start_y;
                            //插补终点坐标
                            temp_Data.End_x = LwPolylines_Entity_Data[i].End_x;
                            temp_Data.End_y = LwPolylines_Entity_Data[i].End_y;

                            //提交进入Arc_Data
                            LwPolyline_Data.Add(temp_Data);

                            //整合数据生成代码
                            temp_intepolation_Dat.Add(temp_Data);//追加数据

                            //清空数据
                            temp_Data.Empty();

                            //删除当前的Entity数据
                            LwPolylines_Entity_Data.RemoveAt(i);

                            //跳出本层循环
                            break;
                        }
                    }

                    //寻找结束点失败，意味着重新开始新的 线段或圆弧
                    if ((LwPolylines_Entity_Data.Count != 0) && (Num != 0) && (Num == LwPolylines_Entity_Data.Count))
                    {
                        //整合数据生成代码 当前结束的封闭图形加工数据
                        LwPolyline_List_Data.Add(new List<Interpolation_Data>(temp_intepolation_Dat));//追加数据
                        //清空数据
                        temp_intepolation_Dat.Clear();

                        //跳刀直接使用直线插补走刀

                        //插补进入新的目标起点
                        temp_Data.Type = 1;//直线插补
                        temp_Data.Lift_flag = 1;//抬刀标志
                        temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                        temp_Data.Repeat = 0;//重复次数
                        temp_Data.End_x = LwPolylines_Entity_Data[0].Start_x;
                        temp_Data.End_y = LwPolylines_Entity_Data[0].Start_y;

                        //提交进入Arc_Data
                        LwPolyline_Data.Add(temp_Data);
                        //整合数据生成代码
                        temp_intepolation_Dat.Add(temp_Data);//追加数据
                        LwPolyline_List_Data.Add(new List<Interpolation_Data>(temp_intepolation_Dat));//追加数据

                        //清空数据
                        temp_Data.Empty();
                        temp_intepolation_Dat.Clear();

                        //记录当前位置比对信息
                        Current_Pos = LwPolyline_Data.Count - 1; //记录比对位置Index

                    }
                    else if ((LwPolylines_Entity_Data.Count == 0) && (Num == 1))//数据整理完成
                    {
                        //整合数据生成代码 当前结束的封闭图形加工数据
                        LwPolyline_List_Data.Add(new List<Interpolation_Data>(temp_intepolation_Dat));//追加数据
                        //清空数据
                        temp_intepolation_Dat.Clear();
                    }
                } while (LwPolylines_Entity_Data.Count > 0);
            }

            //输出多边形插补数据 调试信息
            richTextBox1.AppendText("多边形直线计数：" + LwPolyline_Data.Count + "\r\n");            

            //dxf.Circles.Count 圆读取
            if (dxf.Circles.Count > 0)
            {
                for (i = 0; i < dxf.Circles.Count; i++)
                {
                    if (dxf.Circles[i].Layer.Name == "Laser")                    
                    {
                        temp_Entity_Data.Type = 3;//圆  
                        //使用Gts Pos_reference处理过
                        //temp_Entity_Data.Center_x = Convert.ToDecimal(dxf.Circles[i].Center.X) * Para_List.Parameter.Pos_reference - Pic_Center_x;
                        //temp_Entity_Data.Center_y = Convert.ToDecimal(dxf.Circles[i].Center.Y) * Para_List.Parameter.Pos_reference - Pic_Center_y;
                        //temp_Entity_Data.Circle_radius =Convert.ToDecimal(dxf.Circles[i].Radius) * Para_List.Parameter.Pos_reference;
                        //temp_Entity_Data.Start_x = temp_Entity_Data.Center_x + temp_Entity_Data.Circle_radius;
                        //temp_Entity_Data.Start_y = temp_Entity_Data.Center_y;
                        //temp_Entity_Data.End_x = temp_Entity_Data.Center_x;
                        //temp_Entity_Data.End_y = temp_Entity_Data.Center_y + temp_Entity_Data.Circle_radius;
                        //原始数据
                        temp_Entity_Data.Center_x = Convert.ToDecimal(dxf.Circles[i].Center.X);
                        temp_Entity_Data.Center_y = Convert.ToDecimal(dxf.Circles[i].Center.Y);
                        temp_Entity_Data.Circle_radius = Convert.ToDecimal(dxf.Circles[i].Radius);
                        temp_Entity_Data.Start_x = temp_Entity_Data.Center_x + temp_Entity_Data.Circle_radius;
                        temp_Entity_Data.Start_y = temp_Entity_Data.Center_y;
                        temp_Entity_Data.End_x = temp_Entity_Data.Center_x;
                        temp_Entity_Data.End_y = temp_Entity_Data.Center_y + temp_Entity_Data.Circle_radius;
                        //画圆方向
                        temp_Entity_Data.Circle_dir = 0;//顺时针画圆
                        temp_Entity_Data.Cir_End_Angle = 360.0m;
                        temp_Entity_Data.Cir_Start_Angle = 360.0m;
                        //提交进入Circle_Entity_Data
                        Circle_Entity_Data.Add(new Entity_Data(temp_Entity_Data));
                        temp_Entity_Data.Empty();
                    }
                    else if (dxf.Circles[i].Layer.Name == "Mark")  //Mark点 数据收集
                    {
                        //原始数据
                        temp_Entity_Data.Center_x = Convert.ToDecimal(dxf.Circles[i].Center.X);
                        temp_Entity_Data.Center_y = Convert.ToDecimal(dxf.Circles[i].Center.Y);
                        temp_Entity_Data.Circle_radius = Convert.ToDecimal(dxf.Circles[i].Radius);
                        temp_Entity_Data.Start_x = temp_Entity_Data.Center_x + temp_Entity_Data.Circle_radius;
                        temp_Entity_Data.Start_y = temp_Entity_Data.Center_y;
                        temp_Entity_Data.End_x = temp_Entity_Data.Center_x;
                        temp_Entity_Data.End_y = temp_Entity_Data.Center_y + temp_Entity_Data.Circle_radius;
                        //画圆方向
                        temp_Entity_Data.Circle_dir = 0;//顺时针画圆
                        temp_Entity_Data.Cir_End_Angle = 360.0m;
                        temp_Entity_Data.Cir_Start_Angle = 360.0m;
                        //提交进入Circle_Entity_Data
                        Mark_Circle_Entity_Data.Add(new Entity_Data(temp_Entity_Data)); 
                        temp_Entity_Data.Empty();
                    }
                }
            }

            //Circle_Entity_Data调试信息输出
            richTextBox1.AppendText("圆形数据统计： " + Circle_Entity_Data.Count + "\r\n");
            richTextBox1.AppendText("Mark标记数据统计： " + Mark_Circle_Entity_Data.Count + "\r\n");
            //圆形数据矫正
            //Circle_Entity_Data = new List<Entity_Data>(Data_Cal.Calibration_Trail(Circle_Entity_Data));//直接矫正路径

            //圆形数据矫正
            Circle_Entity_Data = new List<Entity_Data>(Data_Cal.Calibration_Entity(Circle_Entity_Data, Para_List.Parameter.Trans_Affinity));//直接矫正路径

            //生成圆形插补数据
            temp_intepolation_Dat.Clear();
            Circle_Data.Clear();//清除圆形插补数据 
            Circle_List_Data.Clear();//清除圆形插补数据

            //处理Circle_Data加工数据 初始数据
            if (Circle_Entity_Data.Count > 0)
            {
                //选择任意切入点
                temp_Data.Type = 1;//直线插补
                temp_Data.Lift_flag = 1;//抬刀标志
                temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                temp_Data.Repeat = 0;//重复次数
                temp_Data.End_x = Circle_Entity_Data[0].Start_x;//X方向进入
                temp_Data.End_y = Circle_Entity_Data[0].Start_y;//Y方向进入

                Circle_Data.Add(temp_Data);//追加数据

                //整合数据生成代码
                temp_intepolation_Dat.Add(temp_Data);//追加数据
                Circle_List_Data.Add(new List<Interpolation_Data>(temp_intepolation_Dat));//追加数据

                //清空数据
                temp_Data.Empty();
                temp_intepolation_Dat.Clear();

                i = 0;
                Current_Pos = 0;//记录比对位置Index

                //圆形数据提取
                do
                {
                    Num = Circle_Entity_Data.Count;//记录当前Circle_Entity_Data.Count
                    for (i = 0; i < Circle_Entity_Data.Count; i++)
                    {
                        if (Differ_Err(Circle_Data[Circle_Data.Count - 1].End_x, Circle_Data[Circle_Data.Count - 1].End_y, Circle_Entity_Data[i].End_x, Circle_Entity_Data[i].End_y))//当前插补终点是 数据处理终点 同CAD文件规定方向相反
                        {
                            temp_Data.Type = 3;//圆形插补;

                            temp_Data.Lift_flag = 0;//抬刀标志
                            temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                            temp_Data.Repeat = 0;//重复次数
                            //圆形半径
                            temp_Data.Circle_radius = Circle_Entity_Data[i].Circle_radius;
                            //圆形起点坐标
                            temp_Data.Start_x = Circle_Entity_Data[i].End_x;
                            temp_Data.Start_y = Circle_Entity_Data[i].End_y;
                            //圆形终点坐标
                            temp_Data.End_x = Circle_Entity_Data[i].End_x;
                            temp_Data.End_y = Circle_Entity_Data[i].End_y;

                            //圆形插补圆心坐标-起点坐标
                            temp_Data.Center_Start_x = Circle_Entity_Data[i].Center_x - temp_Data.End_x;
                            temp_Data.Center_Start_y = Circle_Entity_Data[i].Center_y - temp_Data.End_y;
                            //圆形方向
                            temp_Data.Circle_dir = Circle_Entity_Data[i].Circle_dir;

                            //圆心坐标
                            temp_Data.Center_x = Circle_Entity_Data[i].Center_x;
                            temp_Data.Center_y = Circle_Entity_Data[i].Center_y;

                            //提交进入Arc_Data
                            Circle_Data.Add(temp_Data);

                            //整合数据生成代码
                            temp_intepolation_Dat.Add(temp_Data);//追加数据

                            //清空数据
                            temp_Data.Empty();
                            //删除当前的Entity数据
                            Circle_Entity_Data.RemoveAt(i);
                            //跳出本层循环
                            break;
                        }
                        else if (Differ_Err(Circle_Data[Circle_Data.Count - 1].End_x, Circle_Data[Circle_Data.Count - 1].End_y, Circle_Entity_Data[i].Start_x, Circle_Entity_Data[i].Start_y))//当前插补终点是 数据处理终点 同CAD文件规定方向相反
                        {
                            temp_Data.Type = 3;//圆形插补

                            temp_Data.Lift_flag = 0;//抬刀标志
                            temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                            temp_Data.Repeat = 0;//重复次数

                            //圆形半径
                            temp_Data.Circle_radius = Circle_Entity_Data[i].Circle_radius;
                            //圆形起点坐标
                            temp_Data.Start_x = Circle_Entity_Data[i].Start_x;
                            temp_Data.Start_y = Circle_Entity_Data[i].Start_y;
                            //圆形终点坐标
                            temp_Data.End_x = Circle_Entity_Data[i].Start_x;
                            temp_Data.End_y = Circle_Entity_Data[i].Start_y;

                            //圆形插补圆心坐标-起点坐标
                            temp_Data.Center_Start_x = Circle_Entity_Data[i].Center_x - temp_Data.End_x;
                            temp_Data.Center_Start_y = Circle_Entity_Data[i].Center_y - temp_Data.End_y;
                            //圆形方向
                            temp_Data.Circle_dir = Circle_Entity_Data[i].Circle_dir;

                            //圆心坐标
                            temp_Data.Center_x = Circle_Entity_Data[i].Center_x;
                            temp_Data.Center_y = Circle_Entity_Data[i].Center_y;

                            //提交进入Arc_Data
                            Circle_Data.Add(temp_Data);

                            //整合数据生成代码
                            temp_intepolation_Dat.Add(temp_Data);//追加数据

                            //清空数据
                            temp_Data.Empty();
                            //删除当前的Entity数据
                            Circle_Entity_Data.RemoveAt(i);
                            //清除当前的Entity数据
                            //Circle_Entity_Data[i].Empty();
                            //跳出本层循环
                            break;
                        }
                    }

                    //寻找结束点失败，意味着重新开始新的 线段或圆弧
                    if ((Circle_Entity_Data.Count != 0) && (Num != 0) && (Num == Circle_Entity_Data.Count))
                    {
                        //整合数据生成代码 当前结束的封闭图形加工数据
                        Circle_List_Data.Add(new List<Interpolation_Data>(temp_intepolation_Dat));//追加数据
                                                                                                  //清空数据
                        temp_intepolation_Dat.Clear();

                        //跳刀直接使用直线插补走刀

                        //插补进入新的目标起点
                        temp_Data.Type = 1;//直线插补
                        temp_Data.Lift_flag = 1;//抬刀标志
                        temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                        temp_Data.Repeat = 0;//重复次数
                        temp_Data.End_x = Circle_Entity_Data[0].Start_x;
                        temp_Data.End_y = Circle_Entity_Data[0].Start_y;

                        //提交进入Arc_Data
                        Circle_Data.Add(temp_Data);

                        //整合数据生成代码
                        temp_intepolation_Dat.Add(temp_Data);//追加数据jj
                        Circle_List_Data.Add(new List<Interpolation_Data>(temp_intepolation_Dat));//追加数据

                        //清空数据
                        temp_Data.Empty();
                        temp_intepolation_Dat.Clear();

                        //记录当前位置比对信息
                        Current_Pos = Circle_Data.Count - 1; //记录比对位置Index

                    }
                    else if ((Circle_Entity_Data.Count == 0) && (Num == 1))//数据整理完成
                    {
                        //整合数据生成代码 当前结束的封闭图形加工数据
                        Circle_List_Data.Add(new List<Interpolation_Data>(temp_intepolation_Dat));//追加数据
                                                                                                  //清空数据
                        temp_intepolation_Dat.Clear();
                    }

                } while (Circle_Entity_Data.Count > 0);
            }

            //输出圆形插补数据 调试信息
            richTextBox1.AppendText("圆形插补数据计数：" + Circle_Data.Count + "\r\n");

            //圆形、直线和圆弧、多边形数据整合
            Concat_Data.Clear();//清空数据
            Concat_Data.AddRange(Arc_Line_Data);
            Concat_Data.AddRange(LwPolyline_Data);
            Concat_Data.AddRange(Circle_Data);


            //圆形、直线和圆弧、多边形Rtc+Gts整合
            Concat_List_Data.Clear();//清空数据
            Concat_List_Data.AddRange(Arc_Line_List_Data);
            Concat_List_Data.AddRange(LwPolyline_List_Data);
            Concat_List_Data.AddRange(Circle_List_Data);

            //调试信息
            richTextBox1.AppendText("直线和圆弧整合数据计数：" + Arc_Line_List_Data.Count + "\r\n");
            richTextBox1.AppendText("多边形插补整合数据计数：" + LwPolyline_List_Data.Count + "\r\n");
            richTextBox1.AppendText("圆形插补整合数据计数：" + Circle_List_Data.Count + "\r\n");
            richTextBox1.AppendText("整合插补整合数据计数：" + Concat_List_Data.Count + "\r\n");

            //矫正数据
            Compensation_Concat_Data.Clear();
            Compensation_Axis(Concat_Data).ForEach(m => Compensation_Concat_Data.Add(m));

            //矫正数据 整合加工的数据
            Compensation_Concat_List_Data.Clear();
            Compensation_Axis(Concat_List_Data).ForEach(m => Compensation_Concat_List_Data.Add(m));

            richTextBox1.AppendText("整合后数据数量：" + Compensation_Concat_List_Data.Count + "\r\n");
            richTextBox1.AppendText("整合前数据数量：" + Compensation_Concat_Data.Count + "\r\n");
            //输出调试信息
            int Count_CN = 0;
            richTextBox1.AppendText("华丽的分割线------------------------" + "\r\n");
            for (i = 0; i < Compensation_Concat_List_Data.Count; i++)
            {
                Count_CN = Count_CN + Compensation_Concat_List_Data[i].Count;
            }
            richTextBox1.AppendText("整合处理数据数量：" + Count_CN + "\r\n");

        }
        /// <summary>
        /// 9.9
        /// 对MarK圆圈进行排序，依次：左下、左上、右上三个点 
        /// </summary>
        /// <param name="entity_Datas"><Mark点集合/param>
        /// <returns></returns>
        private List<Vector> Mark_Calculate(List<Entity_Data> Mark_Datas) 
        {
            //定义返回值
            List<Vector> Result = new List<Vector>();
            //定义点
            Vector Tmp_Point=new Vector();
            //左下点
            Tmp_Point.X = (Mark_Datas.Min(o => o.Center_x));
            Tmp_Point.Y = (Mark_Datas.Min(o => o.Center_y));
            Para_List.Parameter.Mark_Dxf1 = new Vector(Tmp_Point);
            Result.Add(new Vector(Tmp_Point));
            //左上点
            Tmp_Point.X = (Mark_Datas.Min(o => o.Center_x));
            Tmp_Point.Y = (Mark_Datas.Max(o => o.Center_y));
            Para_List.Parameter.Mark_Dxf2 = new Vector(Tmp_Point);
            Result.Add(new Vector(Tmp_Point));
            //右上点
            Tmp_Point.X = (Mark_Datas.Max(o => o.Center_x));
            Tmp_Point.Y = (Mark_Datas.Max(o => o.Center_y));
            Para_List.Parameter.Mark_Dxf3 = new Vector(Tmp_Point);
            Result.Add(new Vector(Tmp_Point));            
            //返回结果
            return Result;
        }

        //角度补偿坐标系
        private List<Interpolation_Data> Compensation_Axis(List<Interpolation_Data> In_Data) 
        {
            Para_List.Parameter.Arc_Compensation_R = Convert.ToDecimal(Math.PI) * (Para_List.Parameter.Arc_Compensation_A / 180.0m);
            decimal Cos_Arc = Convert.ToDecimal(Math.Cos(Convert.ToDouble(Para_List.Parameter.Arc_Compensation_R)));
            decimal Sin_Arc = Convert.ToDecimal(Math.Sin(Convert.ToDouble(Para_List.Parameter.Arc_Compensation_R)));   

            List<Interpolation_Data> Temp_Data=new List<Interpolation_Data>();
            Interpolation_Data Temp_interpolation_Data = new Interpolation_Data();
            for (int i=0;i< In_Data.Count; i++)
            {                
                
                //数据处理
                Temp_interpolation_Data.Empty();
                Temp_interpolation_Data = In_Data[i];
                //Temp_interpolation_Data.End_x =-( (In_Data[i].End_x - Pic_Center_x) * Cos_Arc - (In_Data[i].End_y - Pic_Center_y) * Sin_Arc + Pic_Center_x + Para_List.Parameter.Delta_X);//相对于Pic_Center_x的坐标-
                //Temp_interpolation_Data.End_y =-( (In_Data[i].End_y - Pic_Center_y) * Cos_Arc + (In_Data[i].End_x - Pic_Center_x) * Sin_Arc + Pic_Center_x + Para_List.Parameter.Delta_Y);//相对于Pic_Center_y的坐标-
                //Temp_interpolation_Data.Center_Start_x =- (In_Data[i].Center_Start_x * Cos_Arc - In_Data[i].Center_Start_y * Sin_Arc);
                //Temp_interpolation_Data.Center_Start_y =- (In_Data[i].Center_Start_y * Cos_Arc + In_Data[i].Center_Start_x * Sin_Arc);
                Temp_interpolation_Data.End_x = ((In_Data[i].End_x - Pic_Center_x) * Cos_Arc - (In_Data[i].End_y - Pic_Center_y) * Sin_Arc + Pic_Center_x + Para_List.Parameter.Delta.X);//相对于Pic_Center_x的坐标-
                Temp_interpolation_Data.End_y = ((In_Data[i].End_y - Pic_Center_y) * Cos_Arc + (In_Data[i].End_x - Pic_Center_x) * Sin_Arc + Pic_Center_x + Para_List.Parameter.Delta.Y);//相对于Pic_Center_y的坐标-
                Temp_interpolation_Data.Center_Start_x = (In_Data[i].Center_Start_x * Cos_Arc - In_Data[i].Center_Start_y * Sin_Arc);
                Temp_interpolation_Data.Center_Start_y = (In_Data[i].Center_Start_y * Cos_Arc + In_Data[i].Center_Start_x * Sin_Arc);

                Temp_Data.Add(Temp_interpolation_Data);
            }
            return Temp_Data;
        }

        private List<List<Interpolation_Data>> Compensation_Axis(List<List<Interpolation_Data>> In_Data)
        {
            Para_List.Parameter.Arc_Compensation_R = Convert.ToDecimal(Math.PI) * (Para_List.Parameter.Arc_Compensation_A / 180.0m);
            decimal Cos_Arc = Convert.ToDecimal(Math.Cos(Convert.ToDouble(Para_List.Parameter.Arc_Compensation_R)));
            decimal Sin_Arc = Convert.ToDecimal(Math.Sin(Convert.ToDouble(Para_List.Parameter.Arc_Compensation_R)));

            List<List<Interpolation_Data>> Temp_Data = new List<List<Interpolation_Data>>();//返回值
            List<Interpolation_Data> Temp_interpolation_List_Data = new List<Interpolation_Data>();//二级层
            Interpolation_Data Temp_interpolation_Data = new Interpolation_Data();//一级层            
            
            for (int i = 0; i < In_Data.Count; i++)
            {
                Temp_interpolation_List_Data.Clear();
                for (int j = 0; j < In_Data[i].Count; j++)
                {
                    //数据处理
                    Temp_interpolation_Data.Empty();                    
                    Temp_interpolation_Data = In_Data[i][j];
                    //Temp_interpolation_Data.End_x = -((In_Data[i][j].End_x - Pic_Center_x) * Cos_Arc - (In_Data[i][j].End_y - Pic_Center_y) * Sin_Arc + Pic_Center_x + Para_List.Parameter.Delta_X);//相对于Pic_Center_x的坐标-
                    //Temp_interpolation_Data.End_y = -((In_Data[i][j].End_y - Pic_Center_y) * Cos_Arc + (In_Data[i][j].End_x - Pic_Center_x) * Sin_Arc + Pic_Center_x + Para_List.Parameter.Delta_Y);//相对于Pic_Center_y的坐标-
                    //Temp_interpolation_Data.Center_Start_x = -(In_Data[i][j].Center_Start_x * Cos_Arc - In_Data[i][j].Center_Start_y * Sin_Arc);
                    //Temp_interpolation_Data.Center_Start_y = -(In_Data[i][j].Center_Start_y * Cos_Arc + In_Data[i][j].Center_Start_x * Sin_Arc);
                    Temp_interpolation_Data.End_x = ((In_Data[i][j].End_x - Pic_Center_x) * Cos_Arc - (In_Data[i][j].End_y - Pic_Center_y) * Sin_Arc + Pic_Center_x + Para_List.Parameter.Delta.X);//相对于Pic_Center_x的坐标-
                    Temp_interpolation_Data.End_y = ((In_Data[i][j].End_y - Pic_Center_y) * Cos_Arc + (In_Data[i][j].End_x - Pic_Center_x) * Sin_Arc + Pic_Center_x + Para_List.Parameter.Delta.Y);//相对于Pic_Center_y的坐标-
                    Temp_interpolation_Data.Center_Start_x = (In_Data[i][j].Center_Start_x * Cos_Arc - In_Data[i][j].Center_Start_y * Sin_Arc);
                    Temp_interpolation_Data.Center_Start_y = (In_Data[i][j].Center_Start_y * Cos_Arc + In_Data[i][j].Center_Start_x * Sin_Arc);
                    Temp_interpolation_List_Data.Add(Temp_interpolation_Data);
                }
                Temp_Data.Add(new List<Interpolation_Data>(Temp_interpolation_List_Data));
            }
            return Temp_Data;
        }

        //数据对比
        private bool Compare() 
        {
            return true;
        }

        //坐标误差容许判断
        private bool Differ_Err(decimal x1, decimal  y1, decimal x2, decimal y2)
        {
            if ((Convert.ToDecimal(Math.Abs(x1 - x2)) <= Para_List.Parameter.Pos_Tolerance) && (Convert.ToDecimal(Math.Abs(y1 - y2)) <= Para_List.Parameter.Pos_Tolerance))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //多段线、多边形、样条曲线---进行圆弧处理,搁浅暂停
        private Lwpolyline_Arc LwpolylineToArc(short Lw_No, short Ver_No, DxfDocument dxf)
        {
            /*AutoCAD中约定：凸度为0是直线顶点，它与下一个顶点连接为一直线；凸度不为0是圆弧顶点，它与下一个顶点连接为一圆弧；*/
            /*凸度值为负表示顺时针圆弧，凸度值为正表示逆时针圆弧；凸度绝对值小于1表示圆弧包角小于180°，凸度*/
            /*绝对值大于1表示圆弧包角大于180°。凸度与圆弧包角的关系是：圆弧包角= 4×arctan|凸度值|。*/
            Lwpolyline_Arc Arc_Data = new Lwpolyline_Arc();

            double centerAngle;//包角
            centerAngle = 4 * Math.Atan(Math.Abs(dxf.LwPolylines[Lw_No].Vertexes[Ver_No].Bulge));

            //double x1, x2, y1, y2;//圆弧起始点和终止点
            return Arc_Data;
        }


        //计算图像中心点
        /*
        private Vector3 Seek_Center_Point(DxfDocument dxf)
        {
            Vector3 CenterPoint=new Vector3();
            UInt16 amount = 0;
            int i = 0;
            int j = 0;
            //arc圆弧
            if (dxf.Arcs.Count > 0)
            {
                for (i= 0;i< dxf.Arcs.Count; i++)
                {
                    amount++;
                    CenterPoint = CenterPoint + dxf.Arcs[i].Center;
                }
            }
            //圆
            if (dxf.Circles.Count > 0)
            {
                for (i = 0; i < dxf.Circles.Count; i++)
                {
                    amount++;
                    CenterPoint = CenterPoint + dxf.Circles[i].Center;
                }
            }

            //多边形
            if (dxf.LwPolylines.Count > 0)
            {
                for (i = 0; i < dxf.LwPolylines.Count; i++)
                {
                    for (j = 0; j < dxf.LwPolylines.Count; j++)
                    {
                        amount++;
                        CenterPoint.X = CenterPoint.X + dxf.LwPolylines[i].Vertexes[j].Position.X;
                        CenterPoint.Y = CenterPoint.Y + dxf.LwPolylines[i].Vertexes[j].Position.Y;
                    }
                    
                }
            }
            //直线
            if (dxf.Lines.Count > 0)
            {
                for (i = 0; i < dxf.Lines.Count; i++)
                {
                    amount++;
                    CenterPoint = CenterPoint + dxf.Lines[i].StartPoint;
                }
            }

            //返回中心点
            if (0 == amount)
            {
                return CenterPoint;
            }
            else
            {
                CenterPoint.X = CenterPoint.X / amount;
                CenterPoint.Y = CenterPoint.Y / amount;
                return CenterPoint;
            }   
        }*/

        //暂未用到
        private Vector Seek_Center_Point(DxfDocument dxf)
        {
            Vector Center=new Vector(); 
            UInt16 amount = 0;
            int i = 0;
            int j = 0;
            //arc圆弧
            if (dxf.Arcs.Count > 0)
            {
                for (i = 0; i < dxf.Arcs.Count; i++)
                {
                    amount++;
                    Center.X = Center.X +Convert.ToDecimal( dxf.Arcs[i].Center.X);
                    Center.Y=Center.Y + Convert.ToDecimal(dxf.Arcs[i].Center.Y);
                }
            }
            //圆
            if (dxf.Circles.Count > 0)
            {
                for (i = 0; i < dxf.Circles.Count; i++)
                {
                    amount++;
                    Center.X = Center.X + Convert.ToDecimal(dxf.Circles[i].Center.X);
                    Center.Y=Center.Y + Convert.ToDecimal(dxf.Circles[i].Center.Y);
                }
            }

            //多边形
            if (dxf.LwPolylines.Count > 0)
            {
                for (i = 0; i < dxf.LwPolylines.Count; i++)
                {
                    for (j = 0; j < dxf.LwPolylines.Count; j++)
                    {
                        amount++;
                        Center.X=Center.X + Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j].Position.X);
                        Center.Y=Center.Y + Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j].Position.Y);
                    }

                }
            }
            //直线
            if (dxf.Lines.Count > 0)
            {
                for (i = 0; i < dxf.Lines.Count; i++)
                {
                    amount++;
                    Center.X=Center.X + Convert.ToDecimal(dxf.Lines[i].StartPoint.X);
                    Center.Y=Center.Y + Convert.ToDecimal(dxf.Lines[i].StartPoint.Y);
                }
            }

            //返回中心点
            if (0 == amount)
            {
                return Center;
            }
            else
            {
                Center.X=Center.X / amount;
                Center.Y=Center.Y / amount;
                return Center;
            }
        }
        //两轴回零
        private void button1_Click(object sender, EventArgs e)
        {
            Thread Axis01_home_thread = new Thread(this.Axis01_Home);
            Thread Axis02_home_thread = new Thread(this.Axis02_Home);
            Axis01_home_thread.Start();
            Axis02_home_thread.Start();
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

        //建立直角坐标系
        private void button2_Click(object sender, EventArgs e)
        {
            interpolation.Coordination(Para_List.Parameter.Work.X, Para_List.Parameter.Work.Y);
        }

        //生成加工数据
        private void button3_Click(object sender, EventArgs e)
        {
            //interpolation.Tran_Data(Concat_Data);//补正前数据
            interpolation.Tran_Data(Compensation_Concat_Data);//补正后数据
        }

        //加工启动
        private void button4_Click(object sender, EventArgs e)
        {     
            Thread Interpolation_start_thread = new Thread(Interpolation_Start);
            Interpolation_start_thread.Start();
        }
        private void Interpolation_Start()
        {            
            interpolation.Interpolation_Start();
        }       

        private void Dxf_FormClosed(object sender, FormClosedEventArgs e)
        {
            Refresh_Timer.Close();
        }
        //工件坐标系偏移X
        private void textBox4_TextChanged(object sender, EventArgs e)
        {            
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox4.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Work =new Vector(tmp, Para_List.Parameter.Work.Y);
            });
        }
        //工件坐标系偏移Y
        private void textBox3_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox3.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Work = new Vector(Para_List.Parameter.Work.X,tmp);
            });
        }                
        //刀具绝对坐标X
        private void textBox9_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox9.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Laser =new Vector(tmp, Para_List.Parameter.Laser.Y);
            });
        }
        //刀具绝对坐标Y
        private void textBox8_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox8.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Laser = new Vector( Para_List.Parameter.Laser.X, tmp);
            });
        }
        //直线插补速度
        private void textBox2_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox2.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Line_synVel = tmp;
            });
        }
        //直线插补加速度
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox1.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Line_synAcc = tmp;
            });
        }
        //圆弧插补速度
        private void textBox6_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox6.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Circle_synVel = tmp;
            });
        }
        //圆弧插补加速度
        private void textBox5_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox5.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Circle_synAcc = tmp;
            });
        }
        //相对偏移△X
        private void textBox11_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox11.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Delta =new Vector(tmp, Para_List.Parameter.Delta.Y);
            });
        }
        //相对偏移△Y
        private void textBox10_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox10.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Delta=new Vector(Para_List.Parameter.Delta.X,tmp);
            });
        }
        //角度补偿值
        private void textBox7_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox7.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Arc_Compensation_A = tmp;
            });

        }
        //仿射变换
        private void button9_Click(object sender, EventArgs e)
        {
            //定义仿射变换数组
            Mat mat = new Mat(new Size(3, 2),Emgu.CV.CvEnum.DepthType.Cv32F , 1); //2行 3列 的矩阵 
            //定义点位数组
            PointF[] srcTri = new PointF[3];
            PointF[] dstTri = new PointF[3];
            //点位数据赋值
            ////标准数据
            //srcTri[0] = new PointF(0, 0);
            //srcTri[1] = new PointF(5, 0);
            //srcTri[2] = new PointF(0, 5);
            ////仿射数据
            //dstTri[0] = new PointF(0+10, 0+11);
            //dstTri[1] = new PointF(Convert.ToSingle(5.0 * Math.Cos(Math.PI / 9)+10), Convert.ToSingle(-5.0 * Math.Sin(Math.PI / 9)+11));
            //dstTri[2] = new PointF(Convert.ToSingle(5.0 * Math.Sin(Math.PI / 9)+10), Convert.ToSingle(5.0 * Math.Cos(Math.PI / 9)+11));

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

            //遍历数组
            richTextBox1.AppendText("矩阵维度 Dims：" + mat.Dims + "\r\n");
            richTextBox1.AppendText("矩阵行 Rows：" + mat.Rows + "\r\n");
            richTextBox1.AppendText("矩阵列 Cols：" + mat.Cols + "\r\n");
            richTextBox1.AppendText("通道数 Channels：" + mat.NumberOfChannels + "\r\n");
            richTextBox1.AppendText("类型 Type：" + mat.GetType() + "\r\n");
            richTextBox1.AppendText("通道数据类型 DepthType：" + mat.Depth + "\r\n");

            double[] temp_array = mat.GetDoubleArray();
            for (int i=0;i< mat.Rows; i++)
            {
                for (int j = 0; j < mat.Cols; j++)
                {
                    richTextBox1.AppendText("映射数组元素:" +temp_array[i*mat.Rows+j]  + "\r\n");
                }
            }
            //数组输出
            for (int i = 0; i < temp_array.Length; i++)
            {
                richTextBox1.AppendText("按顺序数组元素:" + temp_array[i] + "\r\n");
            }
            //获取仿射变换参数
            decimal Cos_Arc = Convert.ToDecimal(temp_array[0]);//余弦值
            decimal Sin_Arc =-Convert.ToDecimal(temp_array[1]);//正弦值
            decimal Delta_x = Convert.ToDecimal(temp_array[2]);//x方向偏移
            decimal Delta_y = Convert.ToDecimal(temp_array[5]);//y方向偏移

            richTextBox1.AppendText("Mark1 X：" + Para_List.Parameter.Mark1.X + "  Y：" + Para_List.Parameter.Mark1.Y + "\r\n");
            richTextBox1.AppendText("Mark2 X：" + Para_List.Parameter.Mark2.X + "  Y：" + Para_List.Parameter.Mark2.Y + "\r\n");
            richTextBox1.AppendText("Mark3 X：" + Para_List.Parameter.Mark3.X + "  Y：" + Para_List.Parameter.Mark3.Y + "\r\n");
            richTextBox1.AppendText("Mark_Dxf1 X：" + Para_List.Parameter.Mark_Dxf1.X + "  Y：" + Para_List.Parameter.Mark_Dxf1.Y + "\r\n");
            richTextBox1.AppendText("Mark_Dxf2 X：" + Para_List.Parameter.Mark_Dxf2.X + "  Y：" + Para_List.Parameter.Mark_Dxf2.Y + "\r\n");
            richTextBox1.AppendText("Mark_Dxf3 X：" + Para_List.Parameter.Mark_Dxf3.X + "  Y：" + Para_List.Parameter.Mark_Dxf3.Y + "\r\n");

            //坐标转换值

        }
        //坐标运动平滑系数
        private void textBox12_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox12.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Syn_EvenTime = tmp;
            });
        }
        //直线终止速度
        private void textBox14_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox14.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Line_endVel = tmp;
            });
        }
        //圆弧终止速度
        private void textBox13_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox13.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Circle_endVel = tmp;
            });
        }
        //校准步距
        private void textBox15_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox15.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Calibration_Cell = tmp;
            });
        }

        //标定板横纵计算
        private void button18_Click(object sender, EventArgs e)
        {
            Para_List.Parameter.Calibration_Row = Convert.ToInt16((Para_List.Parameter.Calibration_X_Len / Para_List.Parameter.Calibration_Cell) + 1);
            Para_List.Parameter.Calibration_Col = Convert.ToInt16((Para_List.Parameter.Calibration_Y_Len / Para_List.Parameter.Calibration_Cell) + 1);
            Para_List.Parameter.Affinity_Row = Convert.ToInt16(Para_List.Parameter.Calibration_X_Len / Para_List.Parameter.Calibration_Cell);
            Para_List.Parameter.Affinity_Col = Convert.ToInt16(Para_List.Parameter.Calibration_Y_Len / Para_List.Parameter.Calibration_Cell);
            richTextBox1.AppendText("Calibration_Row:" + Para_List.Parameter.Calibration_Row + "\r\n");
            richTextBox1.AppendText("Calibration_Col:" + Para_List.Parameter.Calibration_Col + "\r\n");
            richTextBox1.AppendText("Affinity_Row:" + Para_List.Parameter.Affinity_Row + "\r\n");
            richTextBox1.AppendText("Affinity_Col:" + Para_List.Parameter.Affinity_Col + "\r\n");
        }
        //标定板X长度
        private void textBox17_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox17.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Calibration_X_Len = tmp;
            });
        }
        //标定板Y长度
        private void textBox16_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox16.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Calibration_Y_Len = tmp;
            });
        }            
        
        //参数保存
        private void button15_Click(object sender, EventArgs e)
        {
            Para_List.Serialize_Parameter serialize_Parameter = new Para_List.Serialize_Parameter();
            serialize_Parameter.Serialize("Para.txt");
            richTextBox1.AppendText("校验数据" + Para_List.Parameter.Line_synVel + "\r\n");
        }
        //参数读取
        private void button14_Click(object sender, EventArgs e)
        {
            Para_List.Serialize_Parameter serialize_Parameter = new Para_List.Serialize_Parameter();
            serialize_Parameter.Reserialize("Para.txt");
            richTextBox1.AppendText("读取的数据" +Para_List.Parameter.Affinity_Row + "\r\n");
        }       
        

        //清除调试信息窗口
        private void button7_Click(object sender, EventArgs e)
        {
            richTextBox1.Text = "";
            
        }
        //相机校准
        private void button16_Click(object sender, EventArgs e)
        {
            Calibration.Exit_Flag = false;
            Thread Correct_Data_thread = new Thread(Correct_Data);
            Correct_Data_thread.Start();
            Calibration.Exit_Flag = false;
        }

        private void Correct_Data()
        {
            Calibration generate_Affinity_Matrix = new Calibration();
            generate_Affinity_Matrix.Get_Datas();

        }
        //相机校准退出
        private void button17_Click(object sender, EventArgs e)
        {
            Calibration.Exit_Flag = true;
        }
        //整合加工启动
        private void button20_Click(object sender, EventArgs e)
        {
            Thread Integrate_thread = new Thread(Integrated_Start);
            Integrate_thread.Start();
            //Integrate_thread.Join();     

        }
        private void Integrated_Start()
        {
            integrated.Rts_Gts(Rtc_List_Data);
        }
        
        //整合加工终止 
        private void button19_Click(object sender, EventArgs e)
        {
            integrated.Exit_Flag = true;
        }
        
        //取list极值值 或 数据拆分
        private void button25_Click(object sender, EventArgs e)
        {
            Thread Resolve_thread = new Thread(Resolve_Start);
            Resolve_thread.Start();
            Resolve_thread.Join();
        }
        private void Resolve_Start()
        {
            //list极值计算
            //for (int i = 0; i < Concat_List_Data.Count; i++)
            //{
            //    for (int j = 0; j < Concat_List_Data[i].Count; j++)
            //    {
            //        richTextBox1.AppendText("list序号：" + i + "Max X：" + Concat_List_Data[i].Max(o=>o.End_x) + "Min X：" + Concat_List_Data[i].Min(o => o.End_x) + "\r\n");
            //        richTextBox1.AppendText("list序号：" + i + "Max Y：" + Concat_List_Data[i].Max(o => o.End_y) + "Min Y：" + Concat_List_Data[i].Min(o => o.End_y) + "\r\n");
            //        break;
            //    }
            //}
            //测试RTC和GTS数据拆分
            Rtc_List_Data.Clear();
            Data_Resolve Test = new Data_Resolve();
            Test.Separate_Rtc_Gts(Compensation_Concat_List_Data).ForEach(m => Rtc_List_Data.Add(m));
            //Rtc_List_Data = new List<List<Interpolation_Data>>(Compensation_Integrate(Concat_List_Data));
            //输出
            //for (int i = 0; i < Compensation_Concat_List_Data.Count; i++)
            //{
            //    for (int j = 0; j < Compensation_Concat_List_Data[i].Count; j++)
            //    {
            //        richTextBox1.AppendText("Type：" + Compensation_Concat_List_Data[i][j].Type + "  加工类型Work：" + Compensation_Concat_List_Data[i][j].Work + "  加工起点 Start_x：" + Compensation_Concat_List_Data[i][j].Start_x + "  加工起点 Start_y：" + Compensation_Concat_List_Data[i][j].Start_y + "  加工终点 End_x：" + Compensation_Concat_List_Data[i][j].End_x + "  加工终点 End_y：" + Compensation_Concat_List_Data[i][j].End_y + "  圆心X：" + Compensation_Concat_List_Data[i][j].Center_x + "  圆心Y：" + Compensation_Concat_List_Data[i][j].Center_y + "\r\n");
            //    }
            //}
            richTextBox1.AppendText("RTC和GTS数据拆分后数据数量：" + Rtc_List_Data.Count + "\r\n");
            for (int i = 0; i < Rtc_List_Data.Count; i++)
            {
                richTextBox1.AppendText("No：" + i + "  加工类型Work：" + Rtc_List_Data[i][0].Work + "  Gts起点x：" + Rtc_List_Data[i][0].Gts_x + "  Gts起点y：" + Rtc_List_Data[i][0].Gts_y + "\r\n");

                for (int j = 0; j < Rtc_List_Data[i].Count; j++)
                {
                    richTextBox1.AppendText("      Type：" + Rtc_List_Data[i][j].Type + "  加工类型Work：" + Rtc_List_Data[i][j].Work + "  Gts起点x：" + Rtc_List_Data[i][j].Gts_x + "  Gts起点y：" + Rtc_List_Data[i][j].Gts_y + "  Rtc起点x：" + Rtc_List_Data[i][j].Rtc_x + "  Rtc起点y：" + Rtc_List_Data[i][j].Rtc_y + "  加工起点 Start_x：" + Rtc_List_Data[i][j].Start_x + "  加工起点 Start_y：" + Rtc_List_Data[i][j].Start_y + "  加工终点 End_x：" + Rtc_List_Data[i][j].End_x + "  加工终点 End_y：" + Rtc_List_Data[i][j].End_y + "  圆心X：" + Rtc_List_Data[i][j].Center_x + "  圆心Y：" + Rtc_List_Data[i][j].Center_y + "  角度：" + Rtc_List_Data[i][j].Angle + "  圆弧方向：" + Rtc_List_Data[i][j].Circle_dir + "\r\n");
                }
            }

        }
        //定位坐标原点
        private void button10_Click(object sender, EventArgs e)
        {
            interpolation.Gts_Ready(0,0);
        }
        //相机与振镜 中心差值X/mm
        private void textBox19_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox19.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Cam_Rtc =new Vector(tmp, Para_List.Parameter.Cam_Rtc.Y);
            });
        }
        //相机与振镜 中心差值Y/mm
        private void textBox18_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox18.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Cam_Rtc =new Vector(Para_List.Parameter.Cam_Rtc.X,tmp);
            });
        }
        
        /// <summary>
        /// 8.31 Mark点 位置定位  自动定位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Mark_Position_Click(object sender, EventArgs e)
        {
            
            //dxf中Mark点集合（三个：左下 左上 右上）
            Mark_Calculate(Mark_Circle_Entity_Data);
            //点位结果输出对比
            for (int j = 0; j < Mark_Circle_Entity_Data.Count; j++)
            {
                richTextBox1.AppendText("Mark_Circle 序号：" + j +"  X：" + Mark_Circle_Entity_Data[j].Center_x+ "  Y：" + Mark_Circle_Entity_Data[j].Center_y + "\r\n");
            }
            richTextBox1.AppendText("Mark_Dxf1 X：" + Para_List.Parameter.Mark_Dxf1.X + "  Y：" + Para_List.Parameter.Mark_Dxf1.Y + "\r\n");
            richTextBox1.AppendText("Mark_Dxf2 X：" + Para_List.Parameter.Mark_Dxf2.X + "  Y：" + Para_List.Parameter.Mark_Dxf2.Y + "\r\n");
            richTextBox1.AppendText("Mark_Dxf3 X：" + Para_List.Parameter.Mark_Dxf3.X + "  Y：" + Para_List.Parameter.Mark_Dxf3.Y + "\r\n");

            //points_Dxf = Mark_points_DXF.ToArray();
            ////开启实际平台中坐标
            //Generate_Affinity_Matrix.Exit_Flag = false;
            //Thread Mark_Data_thread = new Thread(Mark_Data);
            //Mark_Data_thread.Start();
            //Generate_Affinity_Matrix.Exit_Flag = false;

        }
        //预留Test按钮
        private void button12_Click(object sender, EventArgs e)
        {
            //读取矫正原始数据
            //Serialize_Data Read_Data = new Serialize_Data();
            //Read_Data.Reserialize_Correct_Data("Correct_Data.xml");
            //获取标定板标定数据
            //Generate_Affinity_Matrix Get_Data = new Generate_Affinity_Matrix();
            //List<Affinity_Matrix> affinity_Matrices = Get_Data.Resolve(Read_Data.Reserialize_Correct_Data("Correct_Data.xml"));
            //richTextBox1.AppendText("理论数据量:" + 140 * 140 + "\r\n");
            //richTextBox1.AppendText("实际数据量:" + affinity_Matrices.Count + "\r\n");

            //将数据保存为Csv 方便Matlab处理
            //Read_Data.Save_To_Csv(Read_Data.Reserialize_Correct_Data("Correct_Data.xml"), "Data.csv");

            //获取DxfMark点

            //dxf中Mark点集合（三个：左下 左上 右上）
            Mark_Calculate(Mark_Circle_Entity_Data);
            //点位结果输出对比
            for (int j = 0; j < Mark_Circle_Entity_Data.Count; j++)
            {
                richTextBox1.AppendText("Mark_Circle 序号：" + j + "  X：" + Mark_Circle_Entity_Data[j].Center_x + "  Y：" + Mark_Circle_Entity_Data[j].Center_y + "\r\n");
            }
            richTextBox1.AppendText("Mark_Dxf1 X：" + Para_List.Parameter.Mark_Dxf1.X + "  Y：" + Para_List.Parameter.Mark_Dxf1.Y + "\r\n");
            richTextBox1.AppendText("Mark_Dxf2 X：" + Para_List.Parameter.Mark_Dxf2.X + "  Y：" + Para_List.Parameter.Mark_Dxf2.Y + "\r\n");
            richTextBox1.AppendText("Mark_Dxf3 X：" + Para_List.Parameter.Mark_Dxf3.X + "  Y：" + Para_List.Parameter.Mark_Dxf3.Y + "\r\n");
            //搜寻Mark
            Calibration Cal_Mark = new Calibration();
            Para_List.Parameter.Trans_Affinity = Cal_Mark.Cal_Affinity();

        }
        //生成Rtc标定数据
        private void button13_Click(object sender, EventArgs e)
        {
            Data_Resolve get_rtc = new Data_Resolve();
            Rtc_List_Data.Clear();
            //RTC切割距离矫正数据
            //Rtc_List_Data =get_rtc.Generate_Calibration_Data(Para_List.Parameter.Rtc_Cal_Radius, Para_List.Parameter.Rtc_Cal_Interval);
            //RTC与ORG 距离数据
            //Rtc_List_Data =get_rtc.Generate_Org_Rtc_Data(Para_List.Parameter.Rtc_Cal_Radius, Para_List.Parameter.Rtc_Cal_Interval);
            for (int i = 0; i < Rtc_List_Data.Count; i++)
            {
                for (int j = 0; j < Rtc_List_Data[i].Count; j++)
                {
                    richTextBox1.AppendText("Type：" + Rtc_List_Data[i][j].Type + "  加工类型Work：" + Rtc_List_Data[i][j].Work + "  Gts起点x：" + Rtc_List_Data[i][j].Gts_x + "  Gts起点y：" + Rtc_List_Data[i][j].Gts_y + "  Rtc起点x：" + Rtc_List_Data[i][j].Rtc_x + "  Rtc起点y：" + Rtc_List_Data[i][j].Rtc_y + "  加工起点 Start_x：" + Rtc_List_Data[i][j].Start_x + "  加工起点 Start_y：" + Rtc_List_Data[i][j].Start_y + "  加工终点 End_x：" + Rtc_List_Data[i][j].End_x + "  加工终点 End_y：" + Rtc_List_Data[i][j].End_y + "  圆心X：" + Rtc_List_Data[i][j].Center_x + "  圆心Y：" + Rtc_List_Data[i][j].Center_y + "  角度：" + Rtc_List_Data[i][j].Angle + "  圆弧方向：" + Rtc_List_Data[i][j].Circle_dir + "\r\n");
                }
            }
        }
        //RTC校准 间距
        private void textBox20_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox20.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Rtc_Cal_Interval = tmp;
            });
        }
        //RTC校准 半径
        private void textBox21_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox21.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Para_List.Parameter.Rtc_Cal_Radius = tmp;
            });
        }
        //提取坐标
        private void button11_Click(object sender, EventArgs e)
        {
            Vector Tem =new Vector(interpolation.Get_Coordinate());
            MessageBox.Show("X坐标："+ Convert.ToString(Tem.X) + "  Y坐标：" + Convert.ToString(Tem.Y));
        }
        decimal Cor_x, Cor_y;
        //X坐标
        private void textBox23_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox23.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Cor_x = tmp;
            });
        }
        //Y坐标
        private void textBox22_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!decimal.TryParse(textBox22.Text, out decimal tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                Cor_y = tmp;
            });
        }

        //定位坐标点
        private void button21_Click(object sender, EventArgs e)
        {
            interpolation.Gts_Ready(Cor_x,Cor_y);
        }

        //加工停止 
        private void button5_Click(object sender, EventArgs e)
        {
            interpolation.Interpolation_Stop();
        }

        
    }
}
