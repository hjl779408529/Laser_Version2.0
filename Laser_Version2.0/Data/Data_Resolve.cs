﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
using System.Xml.Serialization;

namespace Laser_Build_1._0
{
    class Data_Resolve
    {
        //读取文件
        public DxfDocument Read_File()
        {
            //定义文件名
            string Dxf_filename = "Sample.dxf";
            DxfDocument Result = new DxfDocument();
            //定义Log
            Prompt.Log log = new Prompt.Log();
            //获取文件名
            OpenFileDialog openfile = new OpenFileDialog();
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                Dxf_filename = openfile.FileName;
            }

            //检查文件是否存在
            FileInfo fileInfo = new FileInfo(Dxf_filename);
            if (!fileInfo.Exists)
            {
                log.Commandhandler(Dxf_filename + "----文件不存在！！！" + "\r\n");
                return Result;
            }
            DxfVersion dxfVersion = DxfDocument.CheckDxfFileVersion(Dxf_filename, out bool isBinary);

            // 检查Dxf文件版本是否正确
            if (dxfVersion < DxfVersion.AutoCad2000)
            {
                log.Commandhandler(Dxf_filename + "---文件版本不支持" + "\r\n");
                return Result;
            }

            //读取Dxf文件
            Result = DxfDocument.Load(Dxf_filename);
            // check if there has been any problems loading the file,
            // this might be the case of a corrupt file or a problem in the library
            if (Result == null)
            {
                log.Commandhandler("Dxf文件读取失败" + "\r\n");
                return Result;
            }

            //返回读取结果
            return Result;
        }
        //处理Dxf得到圆弧和线段数据
        public List<Entity_Data> Resolve_Arc_Line(DxfDocument dxf)
        {
            List<Entity_Data> Result = new List<Entity_Data>();
            //建立临时Entity数据
            Entity_Data Temp_Entity_Data = new Entity_Data();
            //临时变量
            int i = 0;
            //圆弧数据读取
            for (i = 0; i < dxf.Arcs.Count; i++)
            {
                Temp_Entity_Data.Type = 2;//圆弧
                Temp_Entity_Data.Start_x = Convert.ToDecimal(dxf.Arcs[i].StartPoint.X);
                Temp_Entity_Data.Start_y = Convert.ToDecimal(dxf.Arcs[i].StartPoint.Y);
                Temp_Entity_Data.End_x = Convert.ToDecimal(dxf.Arcs[i].EndPoint.X);
                Temp_Entity_Data.End_y = Convert.ToDecimal(dxf.Arcs[i].EndPoint.Y);
                Temp_Entity_Data.Cir_Start_Angle = Convert.ToDecimal(dxf.Arcs[i].StartAngle);
                if (Temp_Entity_Data.Cir_Start_Angle >= 359.99m)
                {
                    Temp_Entity_Data.Cir_Start_Angle = 0.0m;
                }
                Temp_Entity_Data.Cir_End_Angle = Convert.ToDecimal(dxf.Arcs[i].EndAngle);
                if (Temp_Entity_Data.Cir_End_Angle <= 0.01m)
                {
                    Temp_Entity_Data.Cir_End_Angle = 360.0m;
                }
                Temp_Entity_Data.Center_x = Convert.ToDecimal(dxf.Arcs[i].Center.X);
                Temp_Entity_Data.Center_y = Convert.ToDecimal(dxf.Arcs[i].Center.Y);
                Temp_Entity_Data.Circle_radius = Convert.ToDecimal(dxf.Arcs[i].Radius);
                
                //提交进入Arc_Data
                Result.Add(Temp_Entity_Data);
                Temp_Entity_Data.Empty();
            }
            //直线数据读取
            for (i = 0; i < dxf.Lines.Count; i++)
            {
                Temp_Entity_Data.Type = 1;
                Temp_Entity_Data.Start_x = Convert.ToDecimal(dxf.Lines[i].StartPoint.X);
                Temp_Entity_Data.Start_y = Convert.ToDecimal(dxf.Lines[i].StartPoint.Y);
                Temp_Entity_Data.End_x = Convert.ToDecimal(dxf.Lines[i].EndPoint.X);
                Temp_Entity_Data.End_y = Convert.ToDecimal(dxf.Lines[i].EndPoint.Y);
                //提交进入Arc_Data
                Result.Add(Temp_Entity_Data);
                Temp_Entity_Data.Empty();
            }
            //返回结果
            return Result;
        }

        //处理Dxf得到多边形数据
        public List<Entity_Data> Resolve_LightWeightPolyline(DxfDocument dxf)
        {
            List<Entity_Data> Result = new List<Entity_Data>();
            //建立临时Entity数据
            Entity_Data Temp_Entity_Data = new Entity_Data();
            //临时变量
            int i = 0, j = 0;
            //LightWeightPolyline 多边形读取
            if (dxf.LwPolylines.Count > 0)
            {
                for (i = 0; i < dxf.LwPolylines.Count; i++)
                {
                    for (j = 0; j < dxf.LwPolylines[i].Vertexes.Count; j++)
                    {
                        Temp_Entity_Data.Type = 1;//直线插补
                        Temp_Entity_Data.Start_x = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j].Position.X);
                        Temp_Entity_Data.Start_y = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j].Position.Y);
                        if (j <= dxf.LwPolylines[i].Vertexes.Count - 2)
                        {
                            Temp_Entity_Data.End_x = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j + 1].Position.X);
                            Temp_Entity_Data.End_y = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[j + 1].Position.Y);
                        }
                        else if (j == (dxf.LwPolylines[i].Vertexes.Count - 1))
                        {
                            Temp_Entity_Data.End_x = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[0].Position.X);
                            Temp_Entity_Data.End_y = Convert.ToDecimal(dxf.LwPolylines[i].Vertexes[0].Position.Y);
                        }

                        //提交进入LwPolylines_Entity_Data
                        Result.Add(Temp_Entity_Data);
                        Temp_Entity_Data.Empty();
                    }
                }
            }
            //返回结果
            return Result;
        }

        //处理Dxf得到Circle数据
        public List<Entity_Data> Resolve_Circle(DxfDocument dxf)
        {
            List<Entity_Data> Result = new List<Entity_Data>();
            //建立临时Entity数据
            Entity_Data Temp_Entity_Data = new Entity_Data();
            //临时变量
            int i = 0;
            //LightWeightPolyline 多边形读取
            //圆数据读取
            if (dxf.Circles.Count > 0)
            {
                for (i = 0; i < dxf.Circles.Count; i++)
                {
                    Temp_Entity_Data.Type = 3;//圆                    
                    Temp_Entity_Data.Center_x = Convert.ToDecimal(dxf.Circles[i].Center.X);
                    Temp_Entity_Data.Center_y = Convert.ToDecimal(dxf.Circles[i].Center.Y);
                    Temp_Entity_Data.Circle_radius = Convert.ToDecimal(dxf.Circles[i].Radius);
                    Temp_Entity_Data.Start_x = Temp_Entity_Data.Center_x + Convert.ToInt32(Temp_Entity_Data.Circle_radius);
                    Temp_Entity_Data.Start_y = Temp_Entity_Data.Center_y;
                    Temp_Entity_Data.End_x = Temp_Entity_Data.Center_x;
                    Temp_Entity_Data.End_y = Temp_Entity_Data.Center_y + Convert.ToInt32(Temp_Entity_Data.Circle_radius);
                    //画圆方向
                    Temp_Entity_Data.Circle_dir = 0;//顺时针画圆
                    //提交进入Circle_Entity_Data
                    Result.Add(Temp_Entity_Data);
                    Temp_Entity_Data.Empty();
                }
            }
            //返回结果
            return Result;
        }
        //Entity数据提取完成后，使用mark点计算的仿射变换参数处理数据，获取Dxf在平台坐标系的位置、同时补偿振镜中心与坐标系原点的差值
        public List<Entity_Data> Calibration_Entity(List<Entity_Data> entity_Datas, Affinity_Matrix Mark_affinity_Matrices) 
        {
            //建立变量 
            List<Entity_Data> Result = new List<Entity_Data>();
            Entity_Data Temp_Data = new Entity_Data();
            
            foreach (var O in entity_Datas)
            {
                //先清空
                Temp_Data.Empty();
                //后赋值
                Temp_Data = O;
                //起点计算
                Temp_Data.Start_x = O.Start_x * Mark_affinity_Matrices.Cos_Value + O.Start_y * Mark_affinity_Matrices.Sin_Value + Mark_affinity_Matrices.Delta_X + Para_List.Parameter.Rtc_Org_X;
                Temp_Data.Start_y = O.Start_y * Mark_affinity_Matrices.Cos_Value - O.Start_x * Mark_affinity_Matrices.Sin_Value + Mark_affinity_Matrices.Delta_Y + Para_List.Parameter.Rtc_Org_Y;
                //终点计算
                Temp_Data.End_x = O.End_x * Mark_affinity_Matrices.Cos_Value + O.End_y * Mark_affinity_Matrices.Sin_Value + Mark_affinity_Matrices.Delta_X + Para_List.Parameter.Rtc_Org_X;
                Temp_Data.End_y = O.End_y * Mark_affinity_Matrices.Cos_Value - O.End_x * Mark_affinity_Matrices.Sin_Value + Mark_affinity_Matrices.Delta_Y + Para_List.Parameter.Rtc_Org_Y;
                //圆心计算
                Temp_Data.Center_x = O.Center_x * Mark_affinity_Matrices.Cos_Value + O.Center_y * Mark_affinity_Matrices.Sin_Value + Mark_affinity_Matrices.Delta_X + Para_List.Parameter.Rtc_Org_X;
                Temp_Data.Center_y = O.Center_y * Mark_affinity_Matrices.Cos_Value - O.Center_x * Mark_affinity_Matrices.Sin_Value + Mark_affinity_Matrices.Delta_Y + Para_List.Parameter.Rtc_Org_Y;

                //追加数据至Result
                Result.Add(Temp_Data);
                //清空Temp_Data
                Temp_Data.Empty();

            }
            return Result;
        } 
        //标定板标定的整体仿射变换参数，计算的是加工轨迹的矫正值，则在生成加工轨迹数据后，统一对该数据进行标定板仿射变换参数修正
        //整理生成的List<List<Interpolation_Data>>数据，直接处理entity_Datas数据，生成的是工控卡平台的数据
        public List<Entity_Data> Calibration_Trail(List<Entity_Data> In_Data)
        {
            //建立变量 
            List<Entity_Data> Result = new List<Entity_Data>();
            Entity_Data Temp_Data = new Entity_Data();
            //临时定位变量
            Int16 Start_m, Start_n, End_m, End_n, Center_m, Center_n;
            //获取标定板标定数据
            List<Affinity_Matrix> affinity_Matrices = Reserialize_Affinity_Matrix("Affinity_Matrix.xml");
            foreach (var O in In_Data)
            {
                //先清空
                Temp_Data.Empty();
                //后赋值

                Temp_Data = O;
                //获取坐标坐落区域
                Start_m = Convert.ToInt16(O.Start_x / Para_List.Parameter.Calibration_Cell);
                Start_n = Convert.ToInt16(O.Start_y / Para_List.Parameter.Calibration_Cell);
                End_m = Convert.ToInt16(O.End_x / Para_List.Parameter.Calibration_Cell);
                End_n = Convert.ToInt16(O.End_y / Para_List.Parameter.Calibration_Cell);
                Center_m = Convert.ToInt16(O.Center_x / Para_List.Parameter.Calibration_Cell);
                Center_n = Convert.ToInt16(O.Center_y / Para_List.Parameter.Calibration_Cell);
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
        //数据处理 生成Arc_Line整合数据  振镜和平台联合加工
        public List<List<Interpolation_Data>> Integrate_Arc_Line(List<Entity_Data> Arc_Line_Datas)
        {
            //结果变量
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();
            List<Interpolation_Data> Single_Data = new List<Interpolation_Data>(); //辅助运算 用途:提取顺序的衔接和处理
            //临时变量
            List<Interpolation_Data> Temp_List_Data = new List<Interpolation_Data>();
            Interpolation_Data Temp_Data = new Interpolation_Data();
            int i = 0;
            int Num = 0;
            //初始清除
            Single_Data.Clear();
            Temp_List_Data.Clear();
            Temp_Data.Empty();

            //处理Line_Arc生成加工数据 初始数据  属于切入加工起点，故强制使用
            //直线插补走刀
            //强制生成独立的 List<Interpolation_Data>，并将其写入独立运行数据块 List<List<Interpolation_Data>>
            if (Arc_Line_Datas.Count > 0)
            {
                //选择任意切入点
                Temp_Data.Type = 1;//直线插补
                Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                Temp_Data.Lift_flag = 1;//抬刀标志
                Temp_Data.Repeat = 0;//重复次数
                Temp_Data.End_x = Arc_Line_Datas[0].Start_x;
                Temp_Data.End_y = Arc_Line_Datas[0].Start_y;

                //提交进入Arc_Data
                Single_Data.Add(Temp_Data);
                //整合数据生成代码
                Temp_List_Data.Add(Temp_Data);//追加数据
                Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

                //清空数据
                Temp_Data.Empty();
                Temp_List_Data.Clear();
            }

            //整理数据
            do
            {
                Num = Arc_Line_Datas.Count;//记录当前Arc_Line_Datas.Count,用于判断数据是否处理完或封闭寻找结束
                for (i = 0; i < Arc_Line_Datas.Count; i++)
                {
                    if (Differ_Err(Single_Data[Single_Data.Count - 1].End_x, Single_Data[Single_Data.Count - 1].End_y, Arc_Line_Datas[i].End_x, Arc_Line_Datas[i].End_y))//当前插补终点是 数据处理终点 同CAD文件规定方向相反
                    {

                        if (Arc_Line_Datas[i].Type == 1)//直线
                        {
                            Temp_Data.Type = 1;//直线插补
                        }
                        else if (Arc_Line_Datas[i].Type == 2)//圆弧
                        {
                            Temp_Data.Type = 2;//圆弧插补
                            Temp_Data.Circle_radius = Arc_Line_Datas[i].Circle_radius;
                            //圆弧插补 圆心坐标 减去 插补起点坐标
                            Temp_Data.Center_Start_x = Arc_Line_Datas[i].Center_x - Arc_Line_Datas[i].End_x;
                            Temp_Data.Center_Start_y = Arc_Line_Datas[i].Center_y - Arc_Line_Datas[i].End_y;
                            //圆弧圆心
                            Temp_Data.Center_x = Arc_Line_Datas[i].Center_x;
                            Temp_Data.Center_y = Arc_Line_Datas[i].Center_y;
                            //计算圆弧角度
                            Temp_Data.Angle = Arc_Line_Datas[i].Cir_End_Angle - Arc_Line_Datas[i].Cir_Start_Angle;
                            //圆弧方向
                            Temp_Data.Circle_dir = 0;
                        }

                        Temp_Data.Lift_flag = 0;//抬刀标志
                        Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                        Temp_Data.Repeat = 0;//重复次数
                        //插补起点坐标
                        Temp_Data.Start_x = Arc_Line_Datas[i].End_x;
                        Temp_Data.Start_y = Arc_Line_Datas[i].End_y;
                        //插补终点坐标
                        Temp_Data.End_x = Arc_Line_Datas[i].Start_x;
                        Temp_Data.End_y = Arc_Line_Datas[i].Start_y;

                        //提交进入Arc_Data
                        Single_Data.Add(Temp_Data);

                        //整合数据生成代码
                        Temp_List_Data.Add(Temp_Data);//追加数据

                        //清空数据
                        Temp_Data.Empty();


                        //删除当前的Entity数据
                        Arc_Line_Datas.RemoveAt(i);
                        break;
                    }
                    else if (Differ_Err(Single_Data[Single_Data.Count - 1].End_x, Single_Data[Single_Data.Count - 1].End_y, Arc_Line_Datas[i].Start_x, Arc_Line_Datas[i].Start_y)) //当前插补终点是 数据处理起点 同CAD文件规定方向相同
                    {

                        if (Arc_Line_Datas[i].Type == 1)//直线
                        {
                            Temp_Data.Type = 1;//直线插补 
                        }
                        else if (Arc_Line_Datas[i].Type == 2)//圆弧
                        {
                            Temp_Data.Type = 2;//圆弧插补
                            Temp_Data.Circle_radius = Arc_Line_Datas[i].Circle_radius;
                            //圆弧插补 圆心坐标 减去 插补起点坐标
                            Temp_Data.Center_Start_x = Arc_Line_Datas[i].Center_x - Arc_Line_Datas[i].Start_x;
                            Temp_Data.Center_Start_y = Arc_Line_Datas[i].Center_y - Arc_Line_Datas[i].Start_y;
                            //圆弧圆心
                            Temp_Data.Center_x = Arc_Line_Datas[i].Center_x;
                            Temp_Data.Center_y = Arc_Line_Datas[i].Center_y;
                            //计算圆弧角度
                            Temp_Data.Angle = Arc_Line_Datas[i].Cir_Start_Angle - Arc_Line_Datas[i].Cir_End_Angle;

                            //圆弧方向
                            Temp_Data.Circle_dir = 1;
                        }

                        Temp_Data.Lift_flag = 0;//抬刀标志
                        Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                        Temp_Data.Repeat = 0;//重复次数
                        //插补起点坐标
                        Temp_Data.Start_x = Arc_Line_Datas[i].Start_x;
                        Temp_Data.Start_y = Arc_Line_Datas[i].Start_y;
                        //插补终点坐标
                        Temp_Data.End_x = Arc_Line_Datas[i].End_x;
                        Temp_Data.End_y = Arc_Line_Datas[i].End_y;

                        //提交进入Arc_Data
                        Single_Data.Add(Temp_Data);
                        //整合数据生成代码
                        Temp_List_Data.Add(Temp_Data);//追加数据

                        //清空数据
                        Temp_Data.Empty();

                        //删除当前的Entity数据
                        Arc_Line_Datas.RemoveAt(i);
                        break;
                    }
                }

                //寻找结束点失败，意味着重新开始新的 线段或圆弧
                if ((Arc_Line_Datas.Count != 0) && (Num != 0) && (Num == Arc_Line_Datas.Count))
                {
                    //整合数据生成代码 当前结束的封闭图形加工数据
                    Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据
                    //清空数据
                    Temp_List_Data.Clear();

                    //跳刀直接使用直线插补走刀
                    //插补进入新的目标起点
                    Temp_Data.Type = 1;//直线插补
                    Temp_Data.Lift_flag = 1;//抬刀标志
                    Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                    Temp_Data.Repeat = 0;//重复次数
                    Temp_Data.End_x = Arc_Line_Datas[0].Start_x;
                    Temp_Data.End_y = Arc_Line_Datas[0].Start_y;

                    //提交进入Arc_Data
                    Single_Data.Add(Temp_Data);

                    //整合数据生成代码
                    Temp_List_Data.Add(Temp_Data);//追加数据
                    Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

                    //清空数据
                    Temp_Data.Empty();
                    Temp_List_Data.Clear();
                }
                else if ((Arc_Line_Datas.Count == 0) && (Num == 1))
                {
                    //整合数据生成代码 当前结束的封闭图形加工数据
                    Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据
                    //清空数据
                    Temp_List_Data.Clear();
                }

            } while (Arc_Line_Datas.Count > 0);//实体Line_Arc数据未清空完
            //返回结果
            return Result;
        }
        //数据处理 生成LWPolyline整合数据  振镜和平台联合加工
        public List<List<Interpolation_Data>> Integrate_LWPolyline(List<Entity_Data> LWPolyline_Datas)
        {
            //结果变量
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();
            List<Interpolation_Data> Single_Data = new List<Interpolation_Data>(); //辅助运算 用途:提取顺序的衔接和处理
            //临时变量
            List<Interpolation_Data> Temp_List_Data = new List<Interpolation_Data>();
            Interpolation_Data Temp_Data = new Interpolation_Data();
            int i = 0;
            int Num = 0;
            //初始清除
            Single_Data.Clear();
            Temp_List_Data.Clear();
            Temp_Data.Empty();

            //处理LWPolyline生成加工数据 初始数据  属于切入加工起点，故强制使用
            //直线插补走刀
            //强制生成独立的 List<Interpolation_Data>，并将其写入独立运行数据块 List<List<Interpolation_Data>>
            if (LWPolyline_Datas.Count > 0)
            {
                //选择任意切入点
                Temp_Data.Type = 1;//直线插补
                Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                Temp_Data.Lift_flag = 1;//抬刀标志
                Temp_Data.Repeat = 0;//重复次数
                Temp_Data.End_x = LWPolyline_Datas[0].Start_x;
                Temp_Data.End_y = LWPolyline_Datas[0].Start_y;

                //提交进入Arc_Data
                Single_Data.Add(Temp_Data);
                //整合数据生成代码
                Temp_List_Data.Add(Temp_Data);//追加数据
                Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

                //清空数据
                Temp_Data.Empty();
                Temp_List_Data.Clear();
            }

            //整理数据
            do
            {
                Num = LWPolyline_Datas.Count;//记录当前LWPolyline_Datas.Count,用于判断数据是否处理完或封闭寻找结束
                for (i = 0; i < LWPolyline_Datas.Count; i++)
                {
                    if (Differ_Err(Single_Data[Single_Data.Count - 1].End_x, Single_Data[Single_Data.Count - 1].End_y, LWPolyline_Datas[i].End_x, LWPolyline_Datas[i].End_y))//当前插补终点是 数据处理终点 同CAD文件规定方向相反
                    {
                        Temp_Data.Type = 1;//直线插补

                        Temp_Data.Lift_flag = 0;//抬刀标志
                        Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                        Temp_Data.Repeat = 0;//重复次数
                        //插补起点
                        Temp_Data.Start_x = LWPolyline_Datas[i].End_x;
                        Temp_Data.Start_y = LWPolyline_Datas[i].End_y;
                        //插补终点坐标
                        Temp_Data.End_x = LWPolyline_Datas[i].Start_x;
                        Temp_Data.End_y = LWPolyline_Datas[i].Start_y;

                        //提交进入Arc_Data
                        Single_Data.Add(Temp_Data);

                        //整合数据生成代码
                        Temp_List_Data.Add(Temp_Data);//追加数据

                        //清空数据
                        Temp_Data.Empty();

                        //删除当前的Entity数据
                        LWPolyline_Datas.RemoveAt(i);
                        break;
                    }
                    else if (Differ_Err(Single_Data[Single_Data.Count - 1].End_x, Single_Data[Single_Data.Count - 1].End_y, LWPolyline_Datas[i].Start_x, LWPolyline_Datas[i].Start_y)) //当前插补终点是 数据处理起点 同CAD文件规定方向相同
                    {
                        Temp_Data.Type = 1;//直线插补 

                        Temp_Data.Lift_flag = 0;//抬刀标志
                        Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                        Temp_Data.Repeat = 0;//重复次数
                        //插补起点
                        Temp_Data.Start_x = LWPolyline_Datas[i].Start_x;
                        Temp_Data.Start_y = LWPolyline_Datas[i].Start_y;
                        //插补终点坐标
                        Temp_Data.End_x = LWPolyline_Datas[i].End_x;
                        Temp_Data.End_y = LWPolyline_Datas[i].End_y;

                        //提交进入Arc_Data
                        Single_Data.Add(Temp_Data);

                        //整合数据生成代码
                        Temp_List_Data.Add(Temp_Data);//追加数据

                        //清空数据
                        Temp_Data.Empty();

                        //删除当前的Entity数据
                        LWPolyline_Datas.RemoveAt(i);
                        break;
                    }
                }

                //寻找结束点失败，意味着重新开始新的 线段或圆弧
                if ((LWPolyline_Datas.Count != 0) && (Num != 0) && (Num == LWPolyline_Datas.Count))
                {
                    //整合数据生成代码 当前结束的封闭图形加工数据
                    Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据
                    //清空数据
                    Temp_List_Data.Clear();

                    //跳刀直接使用直线插补走刀
                    //插补进入新的目标起点
                    Temp_Data.Type = 1;//直线插补
                    Temp_Data.Lift_flag = 1;//抬刀标志
                    Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                    Temp_Data.Repeat = 0;//重复次数
                    Temp_Data.End_x = LWPolyline_Datas[0].Start_x;
                    Temp_Data.End_y = LWPolyline_Datas[0].Start_y;

                    //提交进入Arc_Data
                    Single_Data.Add(Temp_Data);

                    //整合数据生成代码
                    Temp_List_Data.Add(Temp_Data);//追加数据
                    Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

                    //清空数据
                    Temp_Data.Empty();
                    Temp_List_Data.Clear();
                }
                else if ((LWPolyline_Datas.Count == 0) && (Num == 1))
                {
                    //整合数据生成代码 当前结束的封闭图形加工数据
                    Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据
                    //清空数据
                    Temp_List_Data.Clear();
                }
            } while (LWPolyline_Datas.Count > 0);//实体LWPolyline数据未清空完
            //返回结果
            return Result;
        }
        //数据处理 生成Circle整合数据  振镜和平台联合加工
        public List<List<Interpolation_Data>> Integrate_Circle(List<Entity_Data> Circle_Datas)
        {
            //结果变量
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();
            List<Interpolation_Data> Single_Data = new List<Interpolation_Data>(); //辅助运算 用途:提取顺序的衔接和处理
            //临时变量
            List<Interpolation_Data> Temp_List_Data = new List<Interpolation_Data>();
            Interpolation_Data Temp_Data = new Interpolation_Data();
            int i = 0;
            int Num = 0;
            //初始清除
            Single_Data.Clear();
            Temp_List_Data.Clear();
            Temp_Data.Empty();

            //处理Circle生成加工数据 初始数据  属于切入加工起点，故强制使用
            //直线插补走刀
            //强制生成独立的 List<Interpolation_Data>，并将其写入独立运行数据块 List<List<Interpolation_Data>>
            if (Circle_Datas.Count > 0)
            {
                //选择任意切入点
                Temp_Data.Type = 1;//直线插补
                Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                Temp_Data.Lift_flag = 1;//抬刀标志
                Temp_Data.Repeat = 0;//重复次数
                Temp_Data.End_x = Circle_Datas[0].Start_x;
                Temp_Data.End_y = Circle_Datas[0].Start_y;;

                //提交进入Arc_Data
                Single_Data.Add(Temp_Data);
                //整合数据生成代码
                Temp_List_Data.Add(Temp_Data);//追加数据
                Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

                //清空数据
                Temp_Data.Empty();
                Temp_List_Data.Clear();
            }

            //整理数据
            do
            {
                Num = Circle_Datas.Count;//记录当前Circle_Datas.Count,用于判断数据是否处理完或封闭寻找结束
                for (i = 0; i < Circle_Datas.Count; i++)
                {
                    if (Differ_Err(Single_Data[Single_Data.Count - 1].End_x, Single_Data[Single_Data.Count - 1].End_y, Circle_Datas[i].End_x, Circle_Datas[i].End_y))//当前插补终点是 数据处理终点 同CAD文件规定方向相反
                    {
                        Temp_Data.Type = 3;//圆形插补               

                        Temp_Data.Lift_flag = 0;//抬刀标志
                        Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                        Temp_Data.Repeat = 0;//重复次数

                        //圆形半径
                        Temp_Data.Circle_radius = Circle_Datas[i].Circle_radius;
                        //圆形起点坐标
                        Temp_Data.Start_x = Circle_Datas[i].End_x;
                        Temp_Data.Start_y = Circle_Datas[i].End_y;
                        //插补终点坐标
                        Temp_Data.End_x = Circle_Datas[i].End_x;
                        Temp_Data.End_y = Circle_Datas[i].End_y;

                        //圆弧插补 圆心坐标 减去 插补起点坐标
                        Temp_Data.Center_Start_x = Circle_Datas[i].Center_x - Temp_Data.End_x;
                        Temp_Data.Center_Start_y = Circle_Datas[i].Center_y - Temp_Data.End_y;

                        //圆形方向
                        Temp_Data.Circle_dir = Circle_Datas[i].Circle_dir;

                        //提交进入Arc_Data
                        Single_Data.Add(Temp_Data);

                        //整合数据生成代码
                        Temp_List_Data.Add(Temp_Data);//追加数据

                        //清空数据
                        Temp_Data.Empty();

                        //删除当前的Entity数据
                        Circle_Datas.RemoveAt(i);
                        break;
                    }
                    else if (Differ_Err(Single_Data[Single_Data.Count - 1].End_x, Single_Data[Single_Data.Count - 1].End_y, Circle_Datas[i].Start_x, Circle_Datas[i].Start_y)) //当前插补终点是 数据处理起点 同CAD文件规定方向相同
                    {

                        Temp_Data.Type = 3;//圆形插补

                        Temp_Data.Lift_flag = 0;//抬刀标志
                        Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                        Temp_Data.Repeat = 0;//重复次数
                        //圆形半径
                        Temp_Data.Circle_radius = Circle_Datas[i].Circle_radius;
                        //圆形起点坐标
                        Temp_Data.Start_x = Circle_Datas[i].Start_x;
                        Temp_Data.Start_y = Circle_Datas[i].Start_y;
                        //插补终点坐标
                        Temp_Data.End_x = Circle_Datas[i].Start_x;
                        Temp_Data.End_y = Circle_Datas[i].Start_y;

                        //圆弧插补 圆心坐标 减去 插补起点坐标
                        Temp_Data.Center_Start_x = Circle_Datas[i].Center_x - Temp_Data.End_x;
                        Temp_Data.Center_Start_y = Circle_Datas[i].Center_y - Temp_Data.End_y;

                        //圆形方向
                        Temp_Data.Circle_dir = Circle_Datas[i].Circle_dir;

                        //提交进入Arc_Data
                        Single_Data.Add(Temp_Data);
                        //整合数据生成代码
                        Temp_List_Data.Add(Temp_Data);//追加数据

                        //清空数据
                        Temp_Data.Empty();

                        //删除当前的Entity数据
                        Circle_Datas.RemoveAt(i);
                        break;
                    }
                }

                //寻找结束点失败，意味着重新开始新的 线段或圆弧
                if ((Circle_Datas.Count != 0) && (Num != 0) && (Num == Circle_Datas.Count))
                {

                    //整合数据生成代码 当前结束的封闭图形加工数据
                    Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据
                    //清空数据
                    Temp_List_Data.Clear();

                    //跳刀直接使用直线插补走刀
                    //插补进入新的目标起点
                    Temp_Data.Type = 1;//直线插补
                    Temp_Data.Lift_flag = 1;//抬刀标志
                    Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                    Temp_Data.Repeat = 0;//重复次数
                    Temp_Data.End_x = Circle_Datas[0].Start_x;
                    Temp_Data.End_y = Circle_Datas[0].Start_y;

                    //提交进入Arc_Data
                    Single_Data.Add(Temp_Data);

                    //整合数据生成代码
                    Temp_List_Data.Add(Temp_Data);//追加数据
                    Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据

                    //清空数据
                    Temp_Data.Empty();
                    Temp_List_Data.Clear();
                }
                else if ((Circle_Datas.Count == 0) && (Num == 1))
                {
                    //整合数据生成代码 当前结束的封闭图形加工数据
                    Result.Add(new List<Interpolation_Data>(Temp_List_Data));//追加数据
                    //清空数据
                    Temp_List_Data.Clear();
                }

            } while (Circle_Datas.Count > 0);//实体Circle数据未清空完
            //返回结果
            return Result;
        }        
        //将加工数据切分为RTC和GTS加工  该函数的对象是：直线、圆弧和整圆
        public List<List<Interpolation_Data>> Separate_Rtc_Gts(List<List<Interpolation_Data>> In_Data)
        { 
            //结果变量
            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();//返回值
            List<Interpolation_Data> Temp_Interpolation_List_Data = new List<Interpolation_Data>();//二级层
            Interpolation_Data Temp_Data = new Interpolation_Data();//一级层  
            decimal Delta_X = 0, Delta_Y = 0;//X、Y坐标极值差值
            decimal Rtc_Cal_X=0,Rtc_Cal_Y=0;//RTC坐标计算基准  
            int i = 0;
            int j = 0;
            int m = 0;
            //初始清除
            Result.Clear();
            Temp_Interpolation_List_Data.Clear();
            Temp_Data.Empty();

            //数据处理部分
            for (i = 0; i < In_Data.Count; i++)
            {
                //清除数据
                Temp_Interpolation_List_Data.Clear();
                if ((In_Data[i].Count > 0) && (In_Data[i].Count < 2)) //二级层，整合元素数量小于2，说明只有一个跳刀
                {
                    for (j = 0; j < In_Data[i].Count; j++)
                    {
                        //直线、整圆拆分，整理成GTS和RTC加工数据
                        if (In_Data[i][j].Type == 1)//直线
                        {
                            Result.Add(new List<Interpolation_Data>(In_Data[i]));//直接复制进入返回结果数值
                        }
                        else if (In_Data[i][j].Type == 2)// 圆弧
                        {
                            if (Temp_Data.Circle_radius >= 20)//圆弧半径大于等于20mm
                            {
                                Result.Add(new List<Interpolation_Data>(In_Data[i]));//直接复制进入返回结果数值
                            }
                            else
                            {
                                //生成Rtc加工数据
                                Temp_Data = In_Data[i][j];
                                //RTC arc_abs圆弧
                                Temp_Data.Type = 11;
                                //强制抬刀标志：0
                                Temp_Data.Lift_flag = 0;
                                //强制加工类型为RTC
                                Temp_Data.Work = 20;
                                //RTC加工，GTS平台配合坐标
                                Temp_Data.Gts_x = In_Data[i][j].Center_x;
                                Temp_Data.Gts_y = In_Data[i][j].Center_y;
                                //Rtc定位 激光加工起点坐标
                                Temp_Data.Rtc_x = In_Data[i][j].Start_x - In_Data[i][j].Center_x;
                                Temp_Data.Rtc_y = In_Data[i][j].Start_y - In_Data[i][j].Center_y;
                                //RTC 圆弧加工圆心坐标转换
                                Temp_Data.Center_x = In_Data[i][j].Center_x - Temp_Data.Gts_x;
                                Temp_Data.Center_y = In_Data[i][j].Center_y - Temp_Data.Gts_y;
                                //坐标转换 将坐标转换为RTC坐标系坐标
                                Temp_Data.End_x = In_Data[i][j].End_x - In_Data[i][j].Center_x;
                                Temp_Data.End_y = In_Data[i][j].End_y - In_Data[i][j].Center_y;
                                //追加修改的数据
                                Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                //清空数据
                                Temp_Data.Empty();
                                Temp_Interpolation_List_Data.Clear();

                                //再追加一组直线插补，让GTS平台跳至该圆弧终点
                                Temp_Data.Type = 1;//直线插补
                                Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                                Temp_Data.Lift_flag = 1;//抬刀标志
                                Temp_Data.Repeat = 0;//重复次数
                                Temp_Data.End_x = In_Data[i][j].End_x;
                                Temp_Data.End_y = In_Data[i][j].End_y;

                                //追加修改的数据
                                Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                //清空数据
                                Temp_Data.Empty();
                                Temp_Interpolation_List_Data.Clear();
                            }
                            
                        }
                        else if (In_Data[i][j].Type == 3)//整圆
                        {
                            //判断整圆大小
                            if (In_Data[i][j].Circle_radius >= 24) //整圆半径大于24mm，GTS加工
                            {
                                Result.Add(new List<Interpolation_Data>(In_Data[i]));//直接复制进入返回结果数值
                            }
                            else //整圆半径小于24mm，RTC加工
                            {
                                //数据赋值
                                Temp_Data = In_Data[i][j];
                                //RTC arc_abs圆弧
                                Temp_Data.Type = 11;                                
                                //强制抬刀标志：0
                                Temp_Data.Lift_flag = 0;
                                //强制加工类型为RTC
                                Temp_Data.Work = 20;
                                //RTC加工，GTS平台配合坐标
                                Temp_Data.Gts_x = In_Data[i][j].Center_x;
                                Temp_Data.Gts_y = In_Data[i][j].Center_y;                                
                                //RTC 圆弧加工圆心坐标转换
                                Temp_Data.Center_x = 0;
                                Temp_Data.Center_y = 0;
                                //RTC加工切入点
                                Temp_Data.End_x = Temp_Data.Center_x;
                                Temp_Data.End_y = Temp_Data.Center_y + In_Data[i][j].Circle_radius;
                                //RTC加工整圆角度
                                // arc angle in ° as a 64 - bit IEEE floating point value
                                // (positive angle values correspond to clockwise angles);
                                // allowed range: [–3600.0° … +3600.0°] (±10 full circles);
                                // out-of-range values will be edge-clipped.
                                Temp_Data.Angle = 360;//这个参数得看RTC手册，整圆的旋转角度

                                //Rtc定位 激光加工起点坐标
                                Temp_Data.Rtc_x = Temp_Data.Center_x;
                                Temp_Data.Rtc_y = Temp_Data.Center_y + In_Data[i][j].Circle_radius;

                                //追加修改的数据
                                Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                            }
                            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                        }
                    }
                }
                else if (In_Data[i].Count >= 2) //二级层，整合元素数量大于等于2，说明封闭图形
                {
                    //整合数据加工范围判断 取Max Min,Delta_X,Delta_Y长度均在50mm以内
                    //数据计算
                    Delta_X = Convert.ToDecimal(Math.Abs(In_Data[i].Max(o => o.End_x) - In_Data[i].Min(o => o.End_x)));//X坐标极值范围
                    Delta_Y = Convert.ToDecimal(Math.Abs(In_Data[i].Max(o => o.End_y) - In_Data[i].Min(o => o.End_y)));//Y坐标极值范围
                    //获取封闭图形中心坐标
                    Rtc_Cal_X = (In_Data[i].Max(o => o.End_x) + In_Data[i].Min(o => o.End_x)) / 2m;//RTC坐标X基准
                    Rtc_Cal_Y = (In_Data[i].Max(o => o.End_y) + In_Data[i].Min(o => o.End_y)) / 2m;//RTC坐标Y基准
                    //范围判断
                    if ((Delta_X >= 48) || (Delta_Y >= 48))//X、Y坐标极值范围大于等于48mm，由GTS加工，否则由RTC加工
                    {
                        //不考虑圆弧半径大小，全部由Gts加工
                        //Result.Add(new List<Interpolation_Data>(In_Data[i]));//直接复制进入返回结果数值

                        //考虑圆弧半径大小，Radius >= 20mm由Gts加工，Radius < 20mm由Rtc加工
                        for (m=0;m< In_Data[i].Count; m++)
                        {
                            //数据清空
                            Temp_Data.Empty();
                            //数据赋值
                            Temp_Data = In_Data[i][m];
                            //当前数据类型判断
                            if (Temp_Data.Type ==1)//直线
                            {
                                //追加修改的数据
                                Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                            }
                            else if (Temp_Data.Type == 2)//圆弧
                            {
                                if (Temp_Data.Circle_radius >= 20)//圆弧半径大于等于20mm
                                {
                                    //追加修改的数据
                                    Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                }
                                else//圆弧半径小于20mm
                                {
                                    //从起始到当前
                                    if (Temp_Interpolation_List_Data.Count>0)
                                    {
                                        Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                    }
                                    //清空数据  为生成Rtc数据做准备
                                    Temp_Interpolation_List_Data.Clear();

                                    //先计算圆心与切点的直线
                                    if (m>0)
                                    {
                                        //圆弧起点 与 圆心 的直线参数
                                        decimal k1 = (Temp_Data.Center_y - In_Data[i][m - 1].End_y) / (Temp_Data.Center_x - In_Data[i][m - 1].End_x);
                                        decimal b1 = Temp_Data.Center_y - k1 * Temp_Data.Center_x;
                                        //计算起点偏移点
                                        decimal x1 = Temp_Data.Center_x + 2;
                                        decimal y1 = x1 * k1 + b1;
                                        //圆弧终点 与 圆心 的直线参数
                                        decimal k2 = (Temp_Data.Center_y - Temp_Data.End_y) / (Temp_Data.Center_x - Temp_Data.End_x);
                                        decimal b2 = Temp_Data.Center_y - k2 * Temp_Data.Center_x;
                                        //计算终点偏移点
                                        decimal x2 = Temp_Data.Center_x + 2;
                                        decimal y2 = x2 * k2 + b2;

                                        //生成Rtc加工数据
                                        //RTC arc_abs圆弧
                                        Temp_Data.Type = 11;
                                        //强制抬刀标志：0
                                        Temp_Data.Lift_flag = 0;
                                        //强制加工类型为RTC
                                        Temp_Data.Work = 20;
                                        //RTC加工，GTS平台配合坐标
                                        Temp_Data.Gts_x = In_Data[i][m - 1].End_x;
                                        Temp_Data.Gts_y = In_Data[i][m - 1].End_y;
                                        //Rtc定位 激光加工起点坐标
                                        Temp_Data.Rtc_x = 0;
                                        Temp_Data.Rtc_y = 0;
                                        //RTC 圆弧加工圆心坐标转换
                                        Temp_Data.Center_x = In_Data[i][m].Center_x - Temp_Data.Gts_x;
                                        Temp_Data.Center_y = In_Data[i][m].Center_y - Temp_Data.Gts_y;
                                        //坐标转换 将坐标转换为RTC坐标系坐标
                                        Temp_Data.End_x = In_Data[i][m].End_x - Temp_Data.Gts_x;
                                        Temp_Data.End_y = In_Data[i][m].End_y - Temp_Data.Gts_y;
                                        //追加修改的数据
                                        Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                        Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                        //清空数据
                                        Temp_Data.Empty();
                                        Temp_Interpolation_List_Data.Clear();

                                        //再追加一组直线插补，让GTS平台跳至该圆弧终点
                                        Temp_Data.Type = 1;//直线插补
                                        Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                                        Temp_Data.Lift_flag = 1;//抬刀标志
                                        Temp_Data.Repeat = 0;//重复次数
                                        Temp_Data.End_x = In_Data[i][m].End_x;
                                        Temp_Data.End_y = In_Data[i][m].End_y;

                                        //追加修改的数据
                                        Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                        Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                        //清空数据
                                        Temp_Data.Empty();
                                        Temp_Interpolation_List_Data.Clear();

                                    }
                                    else//肯定有切入点
                                    {
                                        if (i > 0)
                                        {
                                            //圆弧起点 与 圆心 的直线参数
                                            decimal k1 = (Temp_Data.Center_y - In_Data[i - 1][In_Data[i - 1].Count - 1].End_y) / (Temp_Data.Center_x - In_Data[i - 1][In_Data[i - 1].Count - 1].End_x);
                                            decimal b1 = Temp_Data.Center_y - k1 * Temp_Data.Center_x;
                                            //计算起点偏移点
                                            decimal x1 = Temp_Data.Center_x + 2;
                                            decimal y1 = x1 * k1 + b1;
                                            //圆弧终点 与 圆心 的直线参数
                                            decimal k2 = (Temp_Data.Center_y - Temp_Data.End_y) / (Temp_Data.Center_x - Temp_Data.End_x);
                                            decimal b2 = Temp_Data.Center_y - k2 * Temp_Data.Center_x;
                                            //计算终点偏移点
                                            decimal x2 = Temp_Data.Center_x + 2;
                                            decimal y2 = x2 * k2 + b2;

                                            //生成Rtc加工数据
                                            //RTC arc_abs圆弧
                                            Temp_Data.Type = 11;
                                            //强制抬刀标志：0
                                            Temp_Data.Lift_flag = 0;
                                            //强制加工类型为RTC
                                            Temp_Data.Work = 20;
                                            //RTC加工，GTS平台配合坐标
                                            Temp_Data.Gts_x = In_Data[i - 1][In_Data[i - 1].Count - 1].End_x;
                                            Temp_Data.Gts_y = In_Data[i - 1][In_Data[i - 1].Count - 1].End_y;
                                            //Rtc定位 激光加工起点坐标
                                            Temp_Data.Rtc_x = 0;
                                            Temp_Data.Rtc_y = 0;
                                            //RTC 圆弧加工圆心坐标转换
                                            Temp_Data.Center_x = In_Data[i][m].Center_x - Temp_Data.Gts_x;
                                            Temp_Data.Center_y = In_Data[i][m].Center_y - Temp_Data.Gts_y;
                                            //坐标转换 将坐标转换为RTC坐标系坐标
                                            Temp_Data.End_x = In_Data[i][m].End_x - Temp_Data.Gts_x;
                                            Temp_Data.End_y = In_Data[i][m].End_y - Temp_Data.Gts_y;
                                            //追加修改的数据
                                            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                            //清空数据
                                            Temp_Data.Empty();
                                            Temp_Interpolation_List_Data.Clear();

                                            //再追加一组直线插补，让GTS平台跳至该圆弧终点
                                            Temp_Data.Type = 1;//直线插补
                                            Temp_Data.Work = 10;//10-Gts加工，20-Rtc加工
                                            Temp_Data.Lift_flag = 1;//抬刀标志
                                            Temp_Data.Repeat = 0;//重复次数
                                            Temp_Data.End_x = In_Data[i][m].End_x;
                                            Temp_Data.End_y = In_Data[i][m].End_y;

                                            //追加修改的数据
                                            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                                            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                            //清空数据
                                            Temp_Data.Empty();
                                            Temp_Interpolation_List_Data.Clear();
                                        }
                                        else
                                        {
                                            MessageBox.Show("整合数据异常，终止！！！");
                                        }
                                        
                                    }
                                    
                                }
                            }


                            //遍历结束
                            if ((Temp_Interpolation_List_Data.Count > 0) && (m == In_Data[i].Count-1))
                            {
                                Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                                //清空数据  为生成Rtc数据做准备
                                Temp_Interpolation_List_Data.Clear();
                            }                            

                        }

                    }
                    else
                    {
                        for (j = 0; j < In_Data[i].Count; j++)
                        {
                            //数据清空
                            Temp_Data.Empty();
                            //数据赋值
                            Temp_Data = In_Data[i][j];
                            //强制抬刀标志：0
                            Temp_Data.Lift_flag = 0;
                            //强制加工类型为RTC
                            Temp_Data.Work = 20;
                            //RTC加工，GTS平台配合坐标
                            if (j == 0)
                            {
                                //GTS平台配合坐标
                                Temp_Data.Gts_x = Rtc_Cal_X;
                                Temp_Data.Gts_y = Rtc_Cal_Y;
                                //Rtc定位 激光加工起点坐标
                                Temp_Data.Rtc_x = In_Data[i][In_Data[i].Count - 1].End_x - Rtc_Cal_X;
                                Temp_Data.Rtc_y = In_Data[i][In_Data[i].Count - 1].End_y - Rtc_Cal_Y;
                            }
                            //直线、圆弧拆分，整理成RTC加工数据
                            if (Temp_Data.Type==1)//直线
                            {
                                //RTC mark_abs直线
                                Temp_Data.Type = 15;                                
                            }
                            else if (Temp_Data.Type == 2)//圆弧
                            {
                                //RTC arc_abs圆弧
                                Temp_Data.Type = 11;
                                //RTC 圆弧加工圆心坐标转换
                                Temp_Data.Center_x = In_Data[i][j].Center_x - Rtc_Cal_X;
                                Temp_Data.Center_y = In_Data[i][j].Center_y - Rtc_Cal_Y;
                                if (In_Data[i][j].Circle_dir == 1)
                                {
                                    Temp_Data.Angle = In_Data[i][j].Angle;
                                }
                                else if (In_Data[i][j].Circle_dir == 0)
                                {
                                    Temp_Data.Angle = -In_Data[i][j].Angle;
                                }
                            }
                            //坐标转换 将坐标转换为RTC坐标系坐标
                            Temp_Data.End_x = In_Data[i][j].End_x - Rtc_Cal_X;
                            Temp_Data.End_y = In_Data[i][j].End_y - Rtc_Cal_Y;
                            //追加修改的数据
                            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
                        }
                        Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
                    }
                }
            }
            //处理二次结果，合并走直线的Gts数据，下次为Rtc加工，则变动该GTS数据终点坐标为RTC加工的gts基准位置
            for (int cal = 0; cal < Result.Count; cal++)
            {
                //当前序号 数量为1、加工类型1 直线、加工方式10 GTS
                //当前+1序号 数量大于1、加工方式20 RTX
                if ((cal< Result.Count-1) && (Result[cal].Count==1) && (Result[cal][0].Type==1) && (Result[cal][0].Work == 10) && (Result[cal+1].Count >= 1) && (Result[cal+1][0].Work == 20))
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
        //标定板标定的整体仿射变换参数，计算的是加工轨迹的矫正值，则在生成加工轨迹数据后，统一对该数据进行标定板仿射变换参数修正
        //整理生成的List<List<Interpolation_Data>>数据，故只处理Gts的数据
        //public List<List<Interpolation_Data>> Calibration_Trail(List<List<Interpolation_Data>> In_Data)
        //{
        //    //结果变量
        //    List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();//返回值
        //    List<Interpolation_Data> Temp_Interpolation_List_Data = new List<Interpolation_Data>();//二级层
        //    Interpolation_Data Temp_Data = new Interpolation_Data();//一级层 
        //    //临时变量
        //    int i = 0;
        //    int j = 0;
        //    int m = 0;
        //    //初始清除
        //    Result.Clear();
        //    Temp_Interpolation_List_Data.Clear();
        //    Temp_Data.Empty();
        //    //获取标定板标定数据
        //    List<Affinity_Matrix> affinity_Matrices = Reserialize_Affinity_Matrix("Affinity_Matrix.xml");
        //    //临时定位变量
        //    Int16 Start_m, Start_n, End_m, End_n, Center_m, Center_n,Gts_m,Gts_n,Center_Start_m, Center_Start_n; 
        //    //数据处理

        //    //返回结果
        //    return Result;
        //}
        //反序列化 标定板标定数据
        private List<Affinity_Matrix> Reserialize_Affinity_Matrix(string fileName)
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
        //生成RTC校准数据
        public List<List<Interpolation_Data>> Generate_Calibration_Data(decimal Radius, decimal Interval)
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
            Temp_Interpolation_List_Data.Clear();

            //坐标原点 2.5半径的圆圈 X+ 2号圆
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
            Temp_Data.Rtc_x = (Interval + Radius);
            Temp_Data.Rtc_y = 0;
            //RTC arc_abs圆弧
            Temp_Data.Type = 11;
            //RTC 圆弧加工圆心坐标转换
            Temp_Data.Center_x = Interval;
            Temp_Data.Center_y = 0;
            //圆弧角度
            Temp_Data.Angle = 360;
            //追加修改的数据
            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
            Temp_Interpolation_List_Data.Clear();

            //坐标原点 2.5半径的圆圈 X- 3号圆
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
            Temp_Data.Rtc_x = -(Interval + Radius);
            Temp_Data.Rtc_y = 0;
            //RTC arc_abs圆弧
            Temp_Data.Type = 11;
            //RTC 圆弧加工圆心坐标转换
            Temp_Data.Center_x = -Interval;
            Temp_Data.Center_y = 0;
            //圆弧角度
            Temp_Data.Angle = 360;
            //追加修改的数据
            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
            Temp_Interpolation_List_Data.Clear();

            //坐标原点 2.5半径的圆圈 Y+ 4号圆
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
            Temp_Data.Rtc_x = 0;
            Temp_Data.Rtc_y = (Interval + Radius); ;
            //RTC arc_abs圆弧
            Temp_Data.Type = 11;
            //RTC 圆弧加工圆心坐标转换
            Temp_Data.Center_x = 0;
            Temp_Data.Center_y = Interval;
            //圆弧角度
            Temp_Data.Angle = 360;
            //追加修改的数据
            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
            Temp_Interpolation_List_Data.Clear();

            //坐标原点 2.5半径的圆圈 Y- 5号圆
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
            Temp_Data.Rtc_x = 0;
            Temp_Data.Rtc_y = -(Interval + Radius); ;
            //RTC arc_abs圆弧
            Temp_Data.Type = 11;
            //RTC 圆弧加工圆心坐标转换
            Temp_Data.Center_x = 0;
            Temp_Data.Center_y = -Interval;
            //圆弧角度
            Temp_Data.Angle = 360;
            //追加修改的数据
            Temp_Interpolation_List_Data.Add(new Interpolation_Data(Temp_Data));
            Result.Add(new List<Interpolation_Data>(Temp_Interpolation_List_Data));
            Temp_Interpolation_List_Data.Clear();

            //返回结果
            return Result;
        }
        //坐标误差容许判断
        private bool Differ_Err(decimal x1, decimal y1, decimal x2, decimal y2)
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
        //将取得的仿射变换系数转换为，全局参数变量
        public void Convert_Affinity_Rate(Affinity_Rate Rate)
        {
            Para_List.Parameter.Arc_Compensation_A = Rate.Angle;//角度
            Para_List.Parameter.Delta_X = Rate.Delta_X;//X坐标偏移
            Para_List.Parameter.Delta_Y = Rate.Delta_Y;//Y坐标偏移
        }
        //角度补偿 放置角度，偏移量　Entity数据处理　
        public List<Interpolation_Data> Compensation_Seperate(List<Interpolation_Data> In_Data)
        {
            Para_List.Parameter.Arc_Compensation_R = Convert.ToDecimal(Math.PI) * (Para_List.Parameter.Arc_Compensation_A / 180.0m);
            decimal Cos_Arc = Convert.ToDecimal(Math.Cos(Convert.ToDouble(Para_List.Parameter.Arc_Compensation_R)));
            decimal Sin_Arc = Convert.ToDecimal(Math.Sin(Convert.ToDouble(Para_List.Parameter.Arc_Compensation_R)));

            List<Interpolation_Data> Result = new List<Interpolation_Data>();
            Interpolation_Data Temp_interpolation_Data = new Interpolation_Data();
            for (int i = 0; i < In_Data.Count; i++)
            {

                //数据处理
                Temp_interpolation_Data.Empty();
                Temp_interpolation_Data = In_Data[i];
                Temp_interpolation_Data.End_x = In_Data[i].End_x * Cos_Arc - In_Data[i].End_y * Sin_Arc + Para_List.Parameter.Delta_X;//坐标原点的坐标X
                Temp_interpolation_Data.End_y = In_Data[i].End_y * Cos_Arc + In_Data[i].End_x * Sin_Arc + Para_List.Parameter.Delta_Y;//坐标原点的坐标Y
                Temp_interpolation_Data.Center_Start_x = In_Data[i].Center_Start_x * Cos_Arc - In_Data[i].Center_Start_y * Sin_Arc;
                Temp_interpolation_Data.Center_Start_y = In_Data[i].Center_Start_y * Cos_Arc + In_Data[i].Center_Start_x * Sin_Arc;

                Result.Add(Temp_interpolation_Data);
            }
            return Result;
        }
        //角度补偿 放置角度，偏移量　整合数据处理　
        public List<List<Interpolation_Data>> Compensation_Integrate(List<List<Interpolation_Data>> In_Data)
        {
            Para_List.Parameter.Arc_Compensation_R = Convert.ToDecimal(Math.PI) * (Para_List.Parameter.Arc_Compensation_A / 180.0m);
            decimal Cos_Arc = Convert.ToDecimal(Math.Cos(Convert.ToDouble(Para_List.Parameter.Arc_Compensation_R)));
            decimal Sin_Arc = Convert.ToDecimal(Math.Sin(Convert.ToDouble(Para_List.Parameter.Arc_Compensation_R)));

            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();//返回值
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
                    Temp_interpolation_Data.End_x = In_Data[i][j].End_x * Cos_Arc - In_Data[i][j].End_y * Sin_Arc + Para_List.Parameter.Delta_X;//相对于坐标原点的坐标X
                    Temp_interpolation_Data.End_y = In_Data[i][j].End_y * Cos_Arc + In_Data[i][j].End_x * Sin_Arc + Para_List.Parameter.Delta_Y;//相对于坐标原点的坐标Y
                    Temp_interpolation_Data.Center_Start_x = In_Data[i][j].Center_Start_x * Cos_Arc - In_Data[i][j].Center_Start_y * Sin_Arc;
                    Temp_interpolation_Data.Center_Start_y = In_Data[i][j].Center_Start_y * Cos_Arc + In_Data[i][j].Center_Start_x * Sin_Arc;
                    Temp_interpolation_List_Data.Add(Temp_interpolation_Data);
                }
                Result.Add(new List<Interpolation_Data>(Temp_interpolation_List_Data));
            }
            return Result;
        }
        //角度补偿 放置角度，偏移量　Entity数据处理　自定义仿射变换系数
        public List<Interpolation_Data> Compensation_Seperate(List<Interpolation_Data> In_Data, Affinity_Rate Rate)
        {
            decimal Arc = Convert.ToDecimal(Math.PI) * (Rate.Angle / 180.0m);
            decimal Cos_Arc = Convert.ToDecimal(Math.Cos(Convert.ToDouble(Arc)));
            decimal Sin_Arc = Convert.ToDecimal(Math.Sin(Convert.ToDouble(Arc)));

            List<Interpolation_Data> Result = new List<Interpolation_Data>();
            Interpolation_Data Temp_interpolation_Data = new Interpolation_Data();
            for (int i = 0; i < In_Data.Count; i++)
            {

                //数据处理
                Temp_interpolation_Data.Empty();
                Temp_interpolation_Data = In_Data[i];
                Temp_interpolation_Data.End_x = In_Data[i].End_x * Cos_Arc - In_Data[i].End_y * Sin_Arc + Rate.Delta_X;//坐标原点的坐标X
                Temp_interpolation_Data.End_y = In_Data[i].End_y * Cos_Arc + In_Data[i].End_x * Sin_Arc + Rate.Delta_Y;//坐标原点的坐标Y
                Temp_interpolation_Data.Center_Start_x = In_Data[i].Center_Start_x * Cos_Arc - In_Data[i].Center_Start_y * Sin_Arc;
                Temp_interpolation_Data.Center_Start_y = In_Data[i].Center_Start_y * Cos_Arc + In_Data[i].Center_Start_x * Sin_Arc;

                Result.Add(Temp_interpolation_Data);
            }
            return Result;
        }
        //角度补偿 放置角度，偏移量　整合数据处理　自定义仿射变换系数
        public List<List<Interpolation_Data>> Compensation_Integrate(List<List<Interpolation_Data>> In_Data, Affinity_Rate Rate)
        {
            decimal Arc = Convert.ToDecimal(Math.PI) * (Rate.Angle / 180.0m);
            decimal Cos_Arc = Convert.ToDecimal(Math.Cos(Convert.ToDouble(Arc)));
            decimal Sin_Arc = Convert.ToDecimal(Math.Sin(Convert.ToDouble(Arc)));

            List<List<Interpolation_Data>> Result = new List<List<Interpolation_Data>>();//返回值
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
                    Temp_interpolation_Data.End_x = In_Data[i][j].End_x * Cos_Arc - In_Data[i][j].End_y * Sin_Arc + Rate.Delta_X;//相对于坐标原点的坐标X
                    Temp_interpolation_Data.End_y = In_Data[i][j].End_y * Cos_Arc + In_Data[i][j].End_x * Sin_Arc + Rate.Delta_Y;//相对于坐标原点的坐标Y
                    Temp_interpolation_Data.Center_Start_x = In_Data[i][j].Center_Start_x * Cos_Arc - In_Data[i][j].Center_Start_y * Sin_Arc;
                    Temp_interpolation_Data.Center_Start_y = In_Data[i][j].Center_Start_y * Cos_Arc + In_Data[i][j].Center_Start_x * Sin_Arc;
                    Temp_interpolation_List_Data.Add(Temp_interpolation_Data);
                }
                Result.Add(new List<Interpolation_Data>(Temp_interpolation_List_Data));
            }
            return Result;
        }

    }
}
