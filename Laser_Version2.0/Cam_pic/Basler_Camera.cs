using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Basler.Pylon;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Util;
using Emgu.CV.Structure;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.InteropServices;

namespace Laser_Build_1._0
{
    class Basler_Camera:IDisposable
    {
        static Version Sfnc2_0_0 = new Version(2, 0, 0);
        // The name of the pylon feature stream file.
        const string filename = @"./\Config/" +"CameraParameters.pfs";
        public Camera camera;
        //定义日志输出函数
        readonly Prompt.Log Log = new Prompt.Log();
        //定义拍摄完成标志
        private bool Flag = false;
        //定义输出值
        public decimal Cam_X;//计算的图像输出X坐标
        public decimal Cam_Y;//计算的图像输出Y坐标
        Mat Img;        
        //构造函数
        public Basler_Camera()
        {
            camera = new Camera();            
        }
        //相机打开创建软件触发线程
        private void  Camera_Open()
        {
            camera.CameraOpened += Configuration.SoftwareTrigger;
            //Open the connection to the camera device.
            camera.Open();
            // Set a handler for processing the images. 
            camera.StreamGrabber.ImageGrabbed += OnImageGrabbed;
            camera.StreamGrabber.Start(GrabStrategy.OneByOne, GrabLoop.ProvidedByStreamGrabber);
            Flag = false;
        }
        //获取相机信息，并保存在Log中
        public void Camera_Info()
        {
            //Open the connection to the camera device.
            camera.Open();

            List<string> Camera_Info = new List<string>();
            Camera_Info.Add(string.Format("Camera Device Information"));
            Camera_Info.Add(string.Format("========================="));
            Camera_Info.Add(string.Format("Vendor           : {0}", camera.Parameters[PLCamera.DeviceVendorName].GetValue()));
            Camera_Info.Add(string.Format("Model            : {0}", camera.Parameters[PLCamera.DeviceModelName].GetValue()));
            Camera_Info.Add(string.Format("Firmware version : {0}", camera.Parameters[PLCamera.DeviceFirmwareVersion].GetValue()));
            Camera_Info.Add(string.Format(""));
            Camera_Info.Add(string.Format("Camera Device Settings"));
            Camera_Info.Add(string.Format("======================"));
            // Setting the AOI. OffsetX, OffsetY, Width, and Height are integer parameters.
            //设置AOI. OffsetX, OffsetY, Width, and Height 为整型参数
            // On some cameras, the offsets are read-only. If they are writable, set the offsets to min.
            //某些相机，Offset参数只读，如果可写，则设置为最小值
            camera.Parameters[PLCamera.OffsetX].TrySetToMinimum();
            camera.Parameters[PLCamera.OffsetY].TrySetToMinimum();
            // Some parameters have restrictions. You can use GetIncrement/GetMinimum/GetMaximum to make sure you set a valid value.
            //一些参数有限制，可以使用GetIncrement/GetMinimum/GetMaximum来确保设定值是有效的
            // Here, we let pylon correct the value if needed.
            camera.Parameters[PLCamera.Width].SetValue(202, IntegerValueCorrection.Nearest);
            camera.Parameters[PLCamera.Height].SetValue(101, IntegerValueCorrection.Nearest);
            Camera_Info.Add(string.Format("OffsetX          : {0}", camera.Parameters[PLCamera.OffsetX].GetValue()));
            Camera_Info.Add(string.Format("OffsetY          : {0}", camera.Parameters[PLCamera.OffsetY].GetValue()));
            Camera_Info.Add(string.Format("Width            : {0}", camera.Parameters[PLCamera.Width].GetValue()));
            Camera_Info.Add(string.Format("Height           : {0}", camera.Parameters[PLCamera.Height].GetValue()));
            // Set an enum parameter.
            string oldPixelFormat = camera.Parameters[PLCamera.PixelFormat].GetValue(); // Remember the current pixel format.
            Camera_Info.Add(string.Format("Old PixelFormat  : {0} ({1})", camera.Parameters[PLCamera.PixelFormat].GetValue(), oldPixelFormat));
            // Set pixel format to Mono8 if available.
            //设置像素模式：Mono8
            if (camera.Parameters[PLCamera.PixelFormat].TrySetValue(PLCamera.PixelFormat.Mono8))
            {
                Camera_Info.Add(string.Format("New PixelFormat  : {0} ({1})", camera.Parameters[PLCamera.PixelFormat].GetValue(), oldPixelFormat));
            }
            // Some camera models may have auto functions enabled. To set the gain value to a specific value,
            // the Gain Auto function must be disabled first (if gain auto is available).
            camera.Parameters[PLCamera.GainAuto].TrySetValue(PLCamera.GainAuto.Off); // Set GainAuto to Off if it is writable.

            // Features, e.g. 'Gain', are named according to the GenICam Standard Feature Naming Convention (SFNC).
            // The SFNC defines a common set of features, their behavior, and the related parameter names.
            // This ensures the interoperability of cameras from different camera vendors.
            // Cameras compliant with the USB3 Vision standard are based on the SFNC version 2.0.
            // Basler GigE and Firewire cameras are based on previous SFNC versions.
            // Accordingly, the behavior of these cameras and some parameters names will be different.
            // The SFNC version can be used to handle differences between camera device models.
            if (camera.GetSfncVersion() < Sfnc2_0_0)
            {
                // In previous SFNC versions, GainRaw is an integer parameter.
                camera.Parameters[PLCamera.GainRaw].SetValuePercentOfRange(50);
                // GammaEnable is a boolean parameter.
                camera.Parameters[PLCamera.GammaEnable].TrySetValue(true);
            }
            else // For SFNC 2.0 cameras, e.g. USB3 Vision cameras
            {
                // In SFNC 2.0, Gain is a float parameter.
                camera.Parameters[PLUsbCamera.Gain].SetValuePercentOfRange(50);
                // For USB cameras, Gamma is always enabled.
            }

            /******* Parameter access status **************/
            // Each parameter is either readable or writable or both.
            // Depending on the camera's state, a parameter may temporarily not be readable or writable.
            // For example, a parameter related to external triggering may not be available when the camera is in free run mode.
            // Additionally, parameters can be read-only by default.
            Camera_Info.Add(string.Format("OffsetX readable        : {0}", camera.Parameters[PLCamera.OffsetX].IsReadable));
            Camera_Info.Add(string.Format("TriggerSoftware writable: {0}", camera.Parameters[PLCamera.TriggerSoftware].IsWritable));

            /********** Empty parameters **********************/
            // Camera models have different parameter sets available. For example, GammaEnable is not part of USB camera device
            // parameters. If a requested parameter does not exist, an empty parameter object will be returned to simplify handling.
            // Therefore, an additional existence check is not necessary.
            // An empty parameter is never readable or writable.
            Camera_Info.Add(string.Format("GammaEnable writable    : {0}", camera.Parameters[PLCamera.GammaEnable].IsWritable));
            Camera_Info.Add(string.Format("GammaEnable readable    : {0}", camera.Parameters[PLCamera.GammaEnable].IsReadable));
            Camera_Info.Add(string.Format("GammaEnable empty       : {0}", camera.Parameters[PLCamera.GammaEnable].IsEmpty));


            /********** Try or GetValueOrDefault methods **************************/
            // Several parameters provide Try or GetValueOrDefault methods. These methods are provided because
            // a parameter may not always be available, either because the camera device model does not support the parameter
            // or because the parameter is temporarily disabled (due to other parameter settings).
            camera.Parameters[PLCamera.GammaEnable].TrySetValue(true); // If the GammaEnable parameter is writable, enable it.

            // Toggle CenterX to change the availability of OffsetX.
            // If CenterX is readable, get the value. Otherwise, return false.
            bool centerXValue = camera.Parameters[PLCamera.CenterX].GetValueOrDefault(false);
            Camera_Info.Add(string.Format("CenterX                 : {0}", centerXValue));
            Camera_Info.Add(string.Format("OffsetX writable        : {0}", camera.Parameters[PLCamera.OffsetX].IsWritable));
            camera.Parameters[PLCamera.CenterX].TrySetValue(!centerXValue); // Toggle CenterX if CenterX is writable.
            Camera_Info.Add(string.Format("CenterX                 : {0}", camera.Parameters[PLCamera.CenterX].GetValueOrDefault(false)));
            Camera_Info.Add(string.Format("OffsetX writable        : {0}", camera.Parameters[PLCamera.OffsetX].IsWritable));
            camera.Parameters[PLCamera.CenterX].TrySetValue(centerXValue); // Restore the value of CenterX if CenterX is writable.

            // Important: The Try and the GetValueOrDefault methods are usually related to the access status (IsWritable or IsReadable) of a parameter.
            // For more information, check the summary of the methods.

            // There are additional methods available that provide support for setting valid values.
            // Set the width and correct the value to the nearest valid increment.
            camera.Parameters[PLCamera.Width].SetValue(202, IntegerValueCorrection.Nearest);
            // Set the width and correct the value to the nearest valid increment if the width parameter is readable and writable.
            camera.Parameters[PLCamera.Width].TrySetValue(202, IntegerValueCorrection.Nearest);
            // One of the following pixel formats should be available:
            string[] pixelFormats = new string[]
               {
                    PLCamera.PixelFormat.BayerBG8,
                    PLCamera.PixelFormat.BayerRG8,
                    PLCamera.PixelFormat.BayerGR8,
                    PLCamera.PixelFormat.BayerGB8,
                    PLCamera.PixelFormat.Mono8
               };
            camera.Parameters[PLCamera.PixelFormat].SetValue(pixelFormats); //Set the first valid pixel format in the list.
            camera.Parameters[PLCamera.PixelFormat].TrySetValue(pixelFormats); //Set the first valid pixel format in the list if PixelFormat is writable.
            Camera_Info.Add(string.Format("New PixelFormat  : {0}", camera.Parameters[PLCamera.PixelFormat].GetValue()));

            /********* Optional: Accessing camera parameters without using a parameter list ****************/

            // Accessing parameters without using a parameter list can be necessary in rare cases,
            // e.g. if you want to set newly added camera parameters that are not added to a parameter list yet.
            // It is recommended to use parameter lists if possible to avoid using the wrong parameter type and
            // to avoid spelling errors.

            // When accessing parameters, the name and the type must usually be known beforehand.
            // The following syntax can be used to access any camera device parameter.
            // Adjust the parameter name ("BrandNewFeature") and the parameter type (IntegerName, EnumName, FloatName, etc.)
            // according to the parameter that you want to access.
            camera.Parameters[(IntegerName)"BrandNewFeature"].TrySetToMaximum(); // TrySetToMaximum is called for demonstration purposes only.

            // Enumeration values are plain strings.
            // Similar to the example above, the pixel format is set to Mono8, this time without using a parameter list.
            if (camera.Parameters[(EnumName)"PixelFormat"].TrySetValue("Mono8"))
            {
                Camera_Info.Add(string.Format("New PixelFormat  : {0}", camera.Parameters[(EnumName)"PixelFormat"].GetValue()));
            }

            // Restore the old pixel format.
            camera.Parameters[PLCamera.PixelFormat].SetValue(oldPixelFormat);

            camera.Close();

            //保存为XML文件
            //路径定义
            string File_Path = @"./\Config/" + "Camera.xml";
            using (FileStream fs = new FileStream(File_Path, FileMode.Create, FileAccess.ReadWrite))
            {
               
                XmlSerializer bf = new XmlSerializer(typeof(List<string>));
                bf.Serialize(fs, Camera_Info);
            }
        }

        //加载相机配置文件
        public void Load_Config()
        {
            //Open the connection to the camera device.
            camera.Open();
            if (File.Exists(filename))
            {
                Log.Info(string.Format("Reading file {0} back to camera device parameters ...", filename));
                // Just for demonstration, read the content of the file back to the camera device parameters.
                camera.Parameters.Load(filename, ParameterPath.CameraDevice);
            }else
            {
                Log.Info(string.Format(filename+"is not exist！！！"));
            }

            camera.Close();            
        }
        //保存当前相机参数
        public void Save_Config() 
        {
            //Open the connection to the camera device.
            camera.Open();
            if (File.Exists(filename))
            {
                Log.Info(string.Format("Delete file ...", filename));
                File.Delete(filename);       //删掉配置文件
                Log.Info(string.Format("Saving camera device parameters to file {0} ...", filename));
                // Save the content of the camera device parameters in the file.
                camera.Parameters.Save(filename, ParameterPath.CameraDevice);
            }
            else
            {
                Log.Info(string.Format("Saving camera device parameters to file {0} ...", filename));
                // Save the content of the camera device parameters in the file.
                camera.Parameters.Save(filename, ParameterPath.CameraDevice);
            }

            camera.Close();
        }
        // Example of an image event handler.
        private void OnImageGrabbed(Object sender, ImageGrabbedEventArgs e)
        {
            // The grab result is automatically disposed when the event call back returns.
            // The grab result can be cloned using IGrabResult.Clone if you want to keep a copy of it (not shown in this sample).
            IGrabResult grabResult = e.GrabResult;
            // Image grabbed successfully?
            if (grabResult.GrabSucceeded)
            {
                //将basler的相机数据流转换为intptr
                GCHandle hObject = GCHandle.Alloc(grabResult.PixelData, GCHandleType.Pinned);
                IntPtr pObject = hObject.AddrOfPinnedObject();
                //将Basler相机的数据转化为Mat
                Mat src = new Mat(new Size(grabResult.Width,grabResult.Height),DepthType.Cv8U,1,pObject, grabResult.Width);
                //Bgr图像
                Image<Bgr, byte> dest = src.ToImage<Bgr, byte>();
                //直接转换为灰度图像
                //Image<Gray, byte> dest = src.ToImage<Gray, byte>();
                Img = dest.Mat.Clone();
                //CvInvoke.Imshow("New",Img);
                //释放资源
                hObject.Free();
                src.Dispose(); 
                dest.Dispose();
                //捕捉完成标志
                Flag = true;
            }
            else
            {
                Log.Commandhandler(string.Format("Error: {0} {1}", grabResult.ErrorCode, grabResult.ErrorDescription));
            }
        }
        public void Take_Picture()
        {
            Start_Grabber();  
            Stop_Grabber();
        }
        public void Start_Grabber()
        {
            Flag = false;
            //Open the connection to the camera device.
            camera.Open();
            //触发拍照
            if (camera.WaitForFrameTriggerReady(100, TimeoutHandling.ThrowException))
            {
                camera.ExecuteSoftwareTrigger();
            }
            //等待拍照完成
            while (!Flag)
            {
                //MessageBox.Show("拍摄完成");
            }
        }
        public void Stop_Grabber()
        {
            camera.StreamGrabber.Stop();
            Flag = false;
        }
        public void Camera_Close() 
        {
            camera.CameraOpened += Configuration.AcquireSingleFrame;
            camera.Close();
        }
        public void Dispose()
        {
            camera.Dispose();
        }
        //返回bmp图片
        public Emgu.CV.Image<Bgr, byte> Return_Pic() 
        {
            Take_Picture();
            Emgu.CV.Image<Bgr, byte> pic= new Image<Bgr, byte>(480, 320, new Bgr(0, 255, 0));
            //如果要使用MAT类可以更好得到图片
            if (File.Exists("test.bmp"))
            {
                pic = new Emgu.CV.Image<Bgr, byte>("test.bmp");
                File.Delete("test.bmp");       //删掉图片
            }
            return pic;
        }
        //返回Mat数据
        public Mat Return_Mat()
        {
            Take_Picture();
            //Mat Result = new Mat();
            //图像旋转90 、0、180
            //CvInvoke.Flip(Img,Result,FlipType.Horizontal);
            //图像转置
            //CvInvoke.Transpose(Img, Img);
            return Img;
        }
        //返回计算差值
        public Vector Get_Deviation()
        {
            Vector Result = new Vector();
            //获取照片
            Take_Picture();
            Mat srcImg = Img.Clone();
            //高斯滤波
            CvInvoke.GaussianBlur(srcImg, srcImg, new Size(5, 5), 0, 0);
            //随机颜色
            Random RD = new Random();
            Mat grayImg = new Mat();
            //bgr转gray
            CvInvoke.CvtColor(srcImg, grayImg, ColorConversion.Bgr2Gray);
            //图像二值化
            CvInvoke.Threshold(grayImg, grayImg, 100, 255, ThresholdType.Binary);
            //定义轮廓数组
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            //寻找轮廓
            CvInvoke.FindContours(grayImg, contours, null, RetrType.External, ChainApproxMethod.ChainApproxNone);
            //释放资源
            srcImg.Dispose();
            grayImg.Dispose();
            contours.Dispose();
            //返回结果
            return Result;
        }
    }

}
