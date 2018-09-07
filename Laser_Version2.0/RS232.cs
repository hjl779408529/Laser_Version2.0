using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Communication.IO.Tools;

namespace Laser_Version2._0
{
    class RS232
    {
        //串口端口
        private SerialPort ComDevice = new SerialPort();
        //设备串口名List
        public List<string> PortName = new List<string>();
        //波特率
        public List<Int32> BaudRate = new List<Int32>() {300,600,1200,2400,4800,9600,19200,38400,43000,56000,57600,115200};
        //校验位
        public List<string> Parity = new List<string>() { "None", "Odd", "Even", "Mark", "Space" };
        //数据位
        public List<int> DataBits = new List<int>() {8, 7, 6};
        //停止位
        public List<int> StopBits = new List<int>() {1, 2, 3};
        //接收数据数组
        public string ReceiveData = "";
        public Laser_CC_Data Resolve_Rec = new Laser_CC_Data();//接收数据解析
        public bool Rec_Flag;//数据接收完成标志
        // Crc Computation Class
        private static CRCTool compCRC = new CRCTool();
        //构造函数
        public RS232()
        {
            //获取设备串口名
            PortName.Clear();
            PortName =SerialPort.GetPortNames().ToList<string>();
            //绑定数据接收事件
            ComDevice.DataReceived += new SerialDataReceivedEventHandler(Com_DataReceived);//绑定事件
        }
        //串口打开
        public void Open_Com(Int32 No)
        {
            if (PortName.Count < 0)
            {
                MessageBox.Show("没有发现串口,请检查线路！");
                return;
            }

            if (ComDevice.IsOpen == false)
            {
                ComDevice.PortName = PortName[No];
                ComDevice.BaudRate = 115200;//波特率
                ComDevice.Parity = (Parity)Convert.ToInt32("0");//校验位 
                ComDevice.DataBits = 8;//数据位 8、7、6
                ComDevice.StopBits = (StopBits)Convert.ToInt32(1);
                try
                {
                    ComDevice.Open();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
            }
            else
            {
                try
                {
                    ComDevice.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }
        //数据发送
        public bool Send_Data(string sendData)
        {
            //清除接收标志
            Rec_Flag = false;
            //发送的字节数组
            byte[] data = null;
            //将发送的字符串转化为byte
            data = Encoding.ASCII.GetBytes(StrCRC(sendData).Trim());
            //数据发送
            if (ComDevice.IsOpen)
            {
                try
                {
                    ComDevice.Write(data, 0, data.Length);//发送数据
                    return true;
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("串口未打开", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return false;
        }
        //Hex字符串转换16进制字节数组 只支持为16进制数字的字符串
        public byte[] StrToHexByte(string hexString) 
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0) hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Replace(" ", ""), 16);
            return returnBytes;
        }
        //将数值10进制转16进制，再将16进制转换为字符串返回 中心是byte转化为ASCII
        public string Append_Num_Str(UInt32 Num)
        {
            string tempStr = null;
            if (Num <= 255)
            {
                tempStr = string.Format("{0:X2}", Num);
            }
            else
            {
                tempStr = string.Format("{0:X4}", Num);
            }
            byte[] tempByte = StrToHexByte(tempStr);
            string Result = System.Text.Encoding.Default.GetString(tempByte);
            MessageBox.Show(Result);
            return Result;
        }
        //CRC数据校验值添加
        public string StrCRC(string inStr) 
        {
            byte[] data = null;
            //将发送的字符串转化为byte
            data = Encoding.ASCII.GetBytes(inStr.Trim());
            //CRC校准方式
            compCRC.Init(CRCTool.CRCCode.CRC_CCITT);
            ushort usCrc16 = (ushort)compCRC.crctablefast(data);
            /*
            回车(Carriage Return)和换行(Line Feed)区别
            CR用符号\r表示, 十进制ASCII代码是13, 十六进制代码为0x0D
            LF使用\n符号表示, ASCII代码是10, 十六制为0x0A
            Dos / windows: 回车 + 换行CR / LF表示下一行,
            UNIX / linux: 换行符LF表示下一行，
            MAC OS: 回车符CR表示下一行.
            */
            //将校准值追加 至 传入值 {1:X4}---x表示16进制，4表示保留4位  
            string Result = inStr + Append_Num_Str(usCrc16) + "\r";
            return Result;
        }
        //数据接收
        private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] ReDatas = new byte[ComDevice.BytesToRead];
            ComDevice.Read(ReDatas, 0, ReDatas.Length);//读取数据
            //接收数据处理 将ReDatas 转化为 String
            ReceiveData = new ASCIIEncoding().GetString(ReDatas);
            byte[] Rec_Data = null;
            Rec_Data = Encoding.ASCII.GetBytes(ReceiveData.Trim());
            //string ASCIIstr2 = null;
            //for (int j=0;j< Rec_Data.Length; j++)
            //{
            //    int asciicode = (int)(Rec_Data[j]);
            //    ASCIIstr2 += Convert.ToString(asciicode);//字符串ASCIIstr2 为对应的ASCII字符串
            //}
            //MessageBox.Show(ASCIIstr2);
            Resolve_Rec.Empty();
            //数据解析
            Resolve_Rec.RW = System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[0] });
            Resolve_Rec.DataSize = System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[1] });
            Resolve_Rec.Address = System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[2] });
            Resolve_Rec.Com_Control = System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[3] });
            //D1-Dn解析 和 Crc校验
            if (Rec_Data[1] > 0)
            {
                for (int i = 0; i < Rec_Data[1]; i++)
                {
                    Resolve_Rec.Data = Resolve_Rec.Data + System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[4 + i] });
                }
                Resolve_Rec.Crc = BitConverter.ToUInt16(new byte[] { Rec_Data[4 + Rec_Data[1]], Rec_Data[4 + Rec_Data[1] + 1] }, 0);
            }
            else
            {
                Resolve_Rec.Data = "";
                Resolve_Rec.Crc = BitConverter.ToUInt16(new byte[] { Rec_Data[4], Rec_Data[5] }, 0);
            }
            //接收数据组合
            Resolve_Rec.Sum = Resolve_Rec.RW + Resolve_Rec.DataSize + Resolve_Rec.Address + Resolve_Rec.Com_Control + Resolve_Rec.Data;
            byte[] tmp_Data = null;
            tmp_Data = Encoding.ASCII.GetBytes(Resolve_Rec.Sum.Trim());
            string ASCIIstr2 = null;
            for (int j = 0; j < tmp_Data.Length; j++)
            {
                int asciicode = (int)(tmp_Data[j]);
                ASCIIstr2 += Convert.ToString(asciicode);//字符串ASCIIstr2 为对应的ASCII字符串
            }
            MessageBox.Show(ASCIIstr2);
            UInt16 Cal_Num = Cal_Crc(Resolve_Rec.Sum);
            //校验
            if (Cal_Num == Resolve_Rec.Crc)
            {
                MessageBox.Show(string.Format("解析值：{0}，校验值：{1}，校验成功！！！" ,Resolve_Rec.Crc , Cal_Num));
            }
            else
            {
                MessageBox.Show(string.Format("解析值：{0}，校验值：{1}，校验失败！！！" , Resolve_Rec.Crc , Cal_Num));
            }
            
            //置位接收标志
            Rec_Flag = true;
        }
        //只计算CRC数值
        public UInt16 Cal_Crc(string inStr)
        {
            byte[] data = null;
            //将发送的字符串转化为byte
            data = Encoding.ASCII.GetBytes(inStr.Trim());
            //CRC校准方式
            compCRC.Init(CRCTool.CRCCode.CRC_CCITT);
            ushort usCrc16 = (ushort)compCRC.crctablefast(data);
            return usCrc16;
        }

    }
}
