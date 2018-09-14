using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Basler.Pylon;
using System.Threading;
using Emgu.CV;  
using Emgu.CV.Structure;
using Emgu.CV.CvEnum;
using Emgu.Util;
using Emgu.CV.Util;
using Laser_Version2._0;

namespace Laser_Build_1._0
{
    public partial class Emgu_Figures : Form
    {
        public Emgu_Figures()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.CenterScreen;
            this.pictureBox1.BorderStyle = BorderStyle.FixedSingle;
            this.pictureBox1.BackColor = Color.DarkGray;
            this.pictureBox1.MouseWheel += new MouseEventHandler(pictureBox1_MouseWheel);
        }

        Bitmap m_bmp;               //画布中的图像
        Point m_ptCanvas;           //画布原点在设备上的坐标
        Point m_ptCanvasBuf;        //重置画布坐标计算时用的临时变量
        Point m_ptBmp;              //图像位于画布坐标系中的坐标
        float m_nScale = 1.0F;      //缩放比例
        Point m_ptMouseDown;        //鼠标点下是在设备坐标上的坐标
        string m_strMousePt;        //鼠标当前位置对应的坐标
        //Basler_Camera New_Camera = new Basler_Camera();
        Basler_Net_Cam C_R = new Basler_Net_Cam();
        private void Emgu_Figures_Load(object sender, EventArgs e)
        {
            //m_bmp = GetScreen();
            Emgu.CV.Image<Bgr, byte> pic = new Image<Bgr, byte>(480, 320, new Bgr(0, 255, 0));
            m_bmp =pic.ToBitmap();

            //输出List
            if (C_R.Device_list.Count>0)
            {
                richTextBox1.AppendText("在线相机数目：" + C_R.Device_list.Count + "\r\n");

            }else
            {
                richTextBox1.AppendText("无可用相机！！！！" + "\r\n");
            }
        }
        //图像测试
        private void button1_Click(object sender, EventArgs e)
        {
            //m_bmp =new Bitmap(New_Camera.Return_Pic().ToBitmap());
            //pictureBox1.Refresh();
            C_R.Open_Cam();
        }
        //相机参数
        private void button2_Click(object sender, EventArgs e)
        {
            // New_Camera.Camera_Info();
            //New_Camera.Save_Config();
            //New_Camera.Load_Config();
            //New_Camera.Take_Picture();   
            C_R.OneShot();
            do
            {

            } while (!C_R.Finish);
            //Bitmap转Image<Bgr, byte>
            Image<Bgr, byte> image = new Image<Bgr, byte>(C_R.m_bitmap);
            //Image<Bgr, byte>转Mat
            Mat _mat = image.Mat;
            //Mat转Image<Bgr, byte>
            Image<Bgr, byte> _image = _mat.ToImage<Bgr, byte>();
            //Image<Bgr, byte>转Bitmap
            Bitmap _bitmap1 = _image.Bitmap;
            //picturebox1 图像刷新
            m_bmp = _bitmap1;
            pictureBox1.Refresh();
        }

        //获取屏幕图像
        public Bitmap GetScreen()
        {
            Bitmap bmp = new Bitmap(Screen.PrimaryScreen.Bounds.Width,
                Screen.PrimaryScreen.Bounds.Height);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.CopyFromScreen(0, 0, 0, 0, bmp.Size);
            }
            return bmp;
        }
        //重绘图像
        private void pictureBox1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.TranslateTransform(m_ptCanvas.X, m_ptCanvas.Y);       //设置坐标偏移
            g.ScaleTransform(m_nScale, m_nScale);                   //设置缩放比
            g.DrawImage(m_bmp, m_ptBmp);                            //绘制图像

            g.ResetTransform();                                     //重置坐标系
            Pen p = new Pen(Color.Cyan, 3);
            g.DrawLine(p, 0, m_ptCanvas.Y, pictureBox1.Width, m_ptCanvas.Y);
            g.DrawLine(p, m_ptCanvas.X, 0, m_ptCanvas.X, pictureBox1.Height);
            p.Dispose();
            //绘制网格线
            //float nIncrement = (50 * m_nScale);             //网格间的间隔 根据比例绘制
            //for (float x = m_ptCanvas.X; x > 0; x -= nIncrement)
            //    g.DrawLine(Pens.Cyan, x, 0, x, pictureBox1.Height);
            //for (float x = m_ptCanvas.X; x < pictureBox1.Width; x += nIncrement)
            //    g.DrawLine(Pens.Cyan, x, 0, x, pictureBox1.Height);
            //for (float y = m_ptCanvas.Y; y > 0; y -= nIncrement)
            //    g.DrawLine(Pens.Cyan, 0, y, pictureBox1.Width, y);
            //for (float y = m_ptCanvas.Y; y < pictureBox1.Width; y += nIncrement)
            //    g.DrawLine(Pens.Cyan, 0, y, pictureBox1.Width, y);
            //计算屏幕左上角 和 右下角 对应画布上的坐标
            Size szTemp = pictureBox1.Size - (Size)m_ptCanvas;
            PointF ptCanvasOnShowRectLT = new PointF(
                -m_ptCanvas.X / m_nScale, -m_ptCanvas.Y / m_nScale);
            PointF ptCanvasOnShowRectRB = new PointF(
                szTemp.Width / m_nScale, szTemp.Height / m_nScale);
            //显示文字信息
            string strDraw = "Scale: " + m_nScale.ToString("F1") +
                "\nOrigin: " + m_ptCanvas.ToString() +
                "\nLT: " + Point.Round(ptCanvasOnShowRectLT).ToString() +
                "\nRB: " + Point.Round(ptCanvasOnShowRectRB).ToString() +
                "\n" + ((Size)Point.Round(ptCanvasOnShowRectRB)
                - (Size)Point.Round(ptCanvasOnShowRectLT)).ToString();
            Size strSize = TextRenderer.MeasureText(strDraw, this.Font);
            //绘制文字信息
            SolidBrush sb = new SolidBrush(Color.FromArgb(125, 0, 0, 0));
            g.FillRectangle(sb, 0, 0, strSize.Width, strSize.Height);
            g.DrawString(strDraw, this.Font, Brushes.Yellow, 0, 0);
            strSize = TextRenderer.MeasureText(m_strMousePt, this.Font);
            g.FillRectangle(sb, pictureBox1.Width - strSize.Width, 0, strSize.Width, strSize.Height);
            g.DrawString(m_strMousePt, this.Font, Brushes.Yellow, pictureBox1.Width - strSize.Width, 0);
            sb.Dispose();
        }
        //中键
        private void pictureBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {      //如果中键点下    初始化计算要用的临时数据
                m_ptMouseDown = e.Location;
                m_ptCanvasBuf = m_ptCanvas;
            }
            pictureBox1.Focus();
        }
        //平移图像
        private void pictureBox1_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Middle)
            {    //移动过程中 中键点下 重置画布坐标系
                //我总感觉这样写不妥 但却是方便计算  如果多次这样搞的话 还是重载操作符吧
                m_ptCanvas = (Point)((Size)m_ptCanvasBuf + ((Size)e.Location - (Size)m_ptMouseDown));
                pictureBox1.Invalidate();
            }
            //计算 右上角显示的坐标信息
            SizeF szSub = (Size)e.Location - (Size)m_ptCanvas;  //计算鼠标当前点对应画布中的坐标
            szSub.Width /= m_nScale;
            szSub.Height /= m_nScale;
            Size sz = TextRenderer.MeasureText(m_strMousePt, this.Font);    //获取上一次的区域并重绘
            pictureBox1.Invalidate(new Rectangle(pictureBox1.Width - sz.Width, 0, sz.Width, sz.Height));
            m_strMousePt = e.Location.ToString() + "\n" + ((Point)(szSub.ToSize())).ToString();
            sz = TextRenderer.MeasureText(m_strMousePt, this.Font);         //绘制新的区域
            pictureBox1.Invalidate(new Rectangle(pictureBox1.Width - sz.Width, 0, sz.Width, sz.Height));
        }
        //缩放图像
        private void pictureBox1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (m_nScale <= 0.3 && e.Delta <= 0) return;        //缩小下线
            if (m_nScale >= 4.9 && e.Delta >= 0) return;        //放大上线
            //获取 当前点到画布坐标原点的距离
            SizeF szSub = (Size)m_ptCanvas - (Size)e.Location;
            //当前的距离差除以缩放比还原到未缩放长度
            float tempX = szSub.Width / m_nScale;           //这里
            float tempY = szSub.Height / m_nScale;          //将画布比例
            //还原上一次的偏移                               //按照当前缩放比还原到
            m_ptCanvas.X -= (int)(szSub.Width - tempX);     //没有缩放
            m_ptCanvas.Y -= (int)(szSub.Height - tempY);    //的状态
            //重置距离差为  未缩放状态                       
            szSub.Width = tempX;
            szSub.Height = tempY;
            m_nScale += e.Delta > 0 ? 0.2F : -0.2F;
            //重新计算 缩放并 重置画布原点坐标
            m_ptCanvas.X += (int)(szSub.Width * m_nScale - szSub.Width);
            m_ptCanvas.Y += (int)(szSub.Height * m_nScale - szSub.Height);
            pictureBox1.Invalidate();
        }
        //将Mat图像显示出来
        private void button3_Click(object sender, EventArgs e)
        {
            //CvInvoke.NamedWindow("img", NamedWindowType.AutoSize); //创建窗口
            //CvInvoke.Imshow("img", New_Camera.Return_Mat()); //显示图片cv
            //Mat src = New_Camera.Return_Mat();
            //Image<Bgr, byte> dest = src.ToImage<Bgr, byte>();
            //m_bmp = dest.ToBitmap();
            //pictureBox1.Refresh();
        }
        //保存为bmp图片
        private void button4_Click(object sender, EventArgs e)
        {
            //CvInvoke.Imwrite("Pic.png", New_Camera.Return_Mat());
        }
        //窗口关闭，释放相机
        private void Emgu_Figures_FormClosed(object sender, FormClosedEventArgs e)
        {
            //New_Camera.Dispose();
            C_R.Dispose();
        }
        //模板匹配
        private void button5_Click(object sender, EventArgs e)
        {
            //richTextBox1.Text = "";
            ////源图像
            //Mat srcImg = New_Camera.Return_Mat();
            ////CvInvoke.Imshow("src", srcImg);
            //Mat result = srcImg.Clone();
            ////模板图像
            //Mat tempImg = CvInvoke.Imread("Mark.png");

            //int dstImg_rows = srcImg.Rows - tempImg.Rows + 1;
            //int dstImg_cols = srcImg.Cols - tempImg.Cols + 1;
            //Mat dstImg = new Mat(dstImg_rows, dstImg_cols, DepthType.Cv32F, 1);
            //CvInvoke.MatchTemplate(srcImg, tempImg, dstImg, TemplateMatchingType.CcoeffNormed);
            ////CvInvoke.Imshow("match", dstImg);
            //CvInvoke.Normalize(dstImg, dstImg, 0, 1, NormType.MinMax, dstImg.Depth);
            ////获取匹配的数据集合
            //Image<Gray, Single> ImgMatch = dstImg.ToImage<Gray, Single>();
            ////矩形框 框图
            //int count = 0;
            //int tempW = 0, tempH = 0;
            //for (int i = 0; i < dstImg_rows; i++)
            //{
            //    for (int j = 0; j < dstImg_cols; j++)
            //    {
            //        float matchValue = ImgMatch.Data[i, j, 0];
            //        if (matchValue >= 0.90 && (Math.Abs(j - tempW) > 50) && (Math.Abs(i - tempH) > 50))
            //        {
            //            count++;
            //            CvInvoke.Rectangle(result, new Rectangle(j, i, tempImg.Width, tempImg.Height),
            //                                                   new MCvScalar(0, 255, 0), 2);
            //            tempW = j;
            //            tempH = i;
            //        }
            //    }
            //}
            //Image<Bgr, byte> Display = result.ToImage<Bgr, byte>();
            //m_bmp = Display.ToBitmap();
            //pictureBox1.Refresh();
        }
        //轮廓匹配
        private void button6_Click(object sender, EventArgs e)
        {
            //richTextBox1.Text = "";
                        
            ////图像转置
            ////CvInvoke.Transpose(New_Camera.Return_Mat(), temImg);
            ////图像截取
            ////Mat srcImg = new Mat(temImg, new Rectangle(((int)temImg.Height / 2 - 500), ((int)temImg.Width / 2 - 500), 1000, 1000));
            //Mat srcImg = New_Camera.Return_Mat().Clone();
            ////高斯滤波
            //CvInvoke.GaussianBlur(srcImg, srcImg, new Size(5, 5), 0, 0);
            ////随机颜色
            //Random RD = new Random();
            //Mat grayImg = new Mat();
            //CvInvoke.CvtColor(srcImg, grayImg, ColorConversion.Bgr2Gray);
            //CvInvoke.Threshold(grayImg, grayImg, threshhold, 255, (ThresholdType)Binary_Value);
            //CircleF[] circles = CvInvoke.HoughCircles(grayImg, HoughType.Gradient, 1, 300, 100, 10, 100);
            ////CvInvoke.Imshow("grayImage",grayImg);
            ////VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            ////CvInvoke.FindContours(grayImg, contours, null, RetrType.External, ChainApproxMethod.ChainApproxNone);
            ////int count = 0;
            ////for (int i = 0; i < contours.Size; i++)
            ////{
            ////    CircleF circle = CvInvoke.MinEnclosingCircle(contours[i]);
            ////    if ((circle.Radius > 200) && (circle.Radius < 250))
            ////    {
            ////        count++;
            ////        CvInvoke.Circle(srcImg, new Point((int)circle.Center.X, (int)circle.Center.Y), (int)circle.Radius, new MCvScalar(0, 255, 0), 2);
            ////        CvInvoke.PutText(srcImg, Convert.ToString(count), new Point((int)circle.Center.X, (int)circle.Center.Y + (int)circle.Radius + 40), FontFace.HersheyDuplex, 2, new MCvScalar(0, 255, 0), 5);
            ////        richTextBox1.AppendText("序号：" + count + " 圆心坐标 X：" + circle.Center.X + " 圆心坐标 Y：" + circle.Center.Y + " 半径：" + circle.Radius + "\r\n");
            ////    }
            ////}
            //for (int i = 0; i < circles.Length; i++)
            //{
            //    if ((circles[i].Radius > 200) && (circles[i].Radius < 250))
            //    {
            //        CvInvoke.Circle(srcImg, new Point((int)circles[i].Center.X, (int)circles[i].Center.Y), (int)circles[i].Radius, new MCvScalar(0, 255, 0), 2);
            //        CvInvoke.PutText(srcImg, Convert.ToString(i), new Point((int)circles[i].Center.X, (int)circles[i].Center.Y + (int)circles[i].Radius + 40), FontFace.HersheyDuplex, 2, new MCvScalar(0, 255, 0), 5);
            //    }
            //}
            //richTextBox1.AppendText("图像高度 Height：" + srcImg.Height + " 图像宽度 Width：" + srcImg.Width + "\r\n");
            ////刷新PictureBox
            //Image<Bgr, byte> Display = grayImg.ToImage<Bgr, byte>();
            //m_bmp = Display.ToBitmap();
            //pictureBox1.Refresh();
            ////释放资源
            //srcImg.Dispose();
            //grayImg.Dispose();
        }
        //threshhold 值
        int threshhold;
        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            this.Invoke((EventHandler)delegate
            {
                if (!int.TryParse(textBox1.Text, out int tmp))
                {
                    MessageBox.Show("请正确输入数字");
                    return;
                }
                threshhold = tmp;
                trackBar1.Value = threshhold;
            });
        }
        //滚动条
        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            threshhold = trackBar1.Value;
            textBox1.Text = threshhold.ToString();
        }
        //二值化取反
        int Binary_Value;
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox1.Checked)
            {
                Binary_Value = 0;
            }
            else
            {
                Binary_Value = 1;
            }
        }
    }
}
