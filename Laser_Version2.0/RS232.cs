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
        public SerialPort ComDevice = new SerialPort();
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
        public bool Open_Com(Int32 No)
        {
            if (PortName.Count < 0)
            {
                MessageBox.Show("没有发现串口,请检查线路！");
                return false;
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
                    return false;
                }
                return true;
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
                    return false;
                }
                return false;
            }

        }
        //数据发送
        public bool Send_Data(string sendData)
        {
            //清除接收标志
            Rec_Flag = false;
            //发送的字节数组
            byte[] data = null;
            
            //将发送的字符串转化为byte,并追加终止符号
            data = StrCRC(sendData).Concat(new byte[] { 0x0D }).ToArray();
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
            if ((hexString.Length % 2) != 0) hexString = " " + hexString;
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length ; i++)
                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Replace(" ", ""), 16);
            return returnBytes;
        }
        //将数值10进制转16进制，再将16进制转换为字符串返回 中心是byte转化为ASCII
        public byte[] Append_Num_Str(UInt32 Num)
        {
            string tempStr = null;
            if (Num <= 255)
            {
                tempStr = string.Format("{0:X4}", Num);
            }
            else
            {
                tempStr = string.Format("{0:X4}", Num);
            }
            byte[] Result = StrToHexByte(tempStr);
            return Result;
        }
        //CRC数据校验值添加
        public byte[] StrCRC(string inStr) 
        {
            byte[] data = null;
            //Check_Sum
            ushort usCrc16 = (ushort)compCRC.Check_Sum(StrToHexByte(inStr));
            /*
            回车(Carriage Return)和换行(Line Feed)区别
            CR用符号\r表示, 十进制ASCII代码是13, 十六进制代码为0x0D
            LF使用\n符号表示, ASCII代码是10, 十六制为0x0A
            Dos / windows: 回车 + 换行CR / LF表示下一行,
            UNIX / linux: 换行符LF表示下一行，
            MAC OS: 回车符CR表示下一行.
            */
            //Prompt.Log.Info(inStr + Crc_Append_Str(usCrc16));
            //将字符串转换为Byte
            data = Encoding.ASCII.GetBytes((inStr + Crc_Append_Str(usCrc16)).Trim());
            return data;
        }
        public string Crc_Append_Str(UInt32 Num)
        {
            string Result = null;
            Result = string.Format("{0:X4}", Num);
            //MessageBox.Show(Result);
            return Result;
        }
        //数据接收
        private void Com_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            byte[] ReDatas = new byte[ComDevice.BytesToRead];
            ComDevice.Read(ReDatas, 0, ReDatas.Length);//读取数据
            //接收数据处理 将ReDatas 转化为 String
            //该方式回丢弃数据
            //ASCII编码只能包含0-127的数据，高出的数据将丢弃
            //ReceiveData = new ASCIIEncoding().GetString(ReDatas);  
            //byte[] Rec_Data = null;
            //Rec_Data = Encoding.ASCII.GetBytes(ReceiveData.Trim());

            //接收的Byte数据
            byte[] Rec_Data =ReDatas;
            //清空数据
            Resolve_Rec.Empty();
            /**************将int数据以Hex形式显示******************/
            /*
            string ASCIIstr2 = null;
            for (int j = 0; j < Rec_Data.Length; j++)
            {
                int asciicode = (int)(Rec_Data[j]);
                ASCIIstr2 += Convert.ToString(asciicode);//字符串ASCIIstr2 为对应的ASCII字符串
            }
            MessageBox.Show(ASCIIstr2);
            */
            /**************将int数据以Hex形式显示******************/
            //数据拆分
            if (Rec_Data.Length >= 13)
            {
                Resolve_Rec.RW = System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[0], Rec_Data[1] });
                Resolve_Rec.DataSize = System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[2], Rec_Data[3] });
                Resolve_Rec.Address = System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[4], Rec_Data[5] });
                Resolve_Rec.Com_Control = System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[6], Rec_Data[7] });
            }           
            //检查格式
            if ((Resolve_Rec.RW == "03") || (Resolve_Rec.RW == "02")) //03-Read,02-Write
            {
                if (uint.TryParse(Resolve_Rec.DataSize, out uint tmp))//接收数据大小
                {
                    if ((13+ tmp * 2) == Rec_Data.Length)//校验长度
                    {
                        if (tmp > 0)//DataSize>0 
                        {
                            for (int i = 0; i < tmp * 2; i++)
                            {
                                Resolve_Rec.Data = Resolve_Rec.Data + System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[8 + i] });//获取Data数据
                            }
                            Resolve_Rec.Rec = Str_To_Uint16(Resolve_Rec.Data);//获取D1...Dn Uinte16格式
                            Resolve_Rec.Rec_Byte = StrToHexByte(Resolve_Rec.Data);//获取D1...Dn Byte值
                            Array.Reverse(Resolve_Rec.Rec_Byte);
                            Resolve_Rec.Crc = System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[8 + tmp * 2], Rec_Data[8 + tmp * 2 + 1], Rec_Data[8 + tmp * 2 + 2], Rec_Data[8 + tmp * 2 + 3] });//获取Crc校验
                        }
                        else
                        {
                            Resolve_Rec.Data = null;
                            Resolve_Rec.Crc = System.Text.Encoding.Default.GetString(new byte[] { Rec_Data[8 + tmp * 2], Rec_Data[8 + tmp * 2 + 1], Rec_Data[8 + tmp * 2 + 2], Rec_Data[8 + tmp * 2 + 3] });//获取Crc校验
                        }
                    }     
                    else
                    {
                        return;
                    }
                }                
                //接收数据组合
                Resolve_Rec.Sum = Resolve_Rec.RW + Resolve_Rec.DataSize + Resolve_Rec.Address + Resolve_Rec.Com_Control + Resolve_Rec.Data;
                //校验数据完整性
                if (Crc_Append_Str((ushort)compCRC.Check_Sum(StrToHexByte(Resolve_Rec.Sum))) == Resolve_Rec.Crc)
                {
                    //MessageBox.Show("校验成功！！！");
                    //置位接收标志
                    Rec_Flag = true;
                }
               
            }
            else
            {
                //MessageBox.Show("通讯数据格式异常！！！");
                Prompt.Log.Info("Rs232 通讯数据格式异常！！！");
            }
        }
        //将string转换为Uint16
        public UInt16[] Str_To_Uint16(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            if ((hexString.Length % 2) != 0) hexString = " " + hexString;
            UInt16[] Result = new UInt16[hexString.Length / 2];
            for (int i = 0; i < Result.Length; i++)
            {
                if (UInt16.TryParse(hexString.Substring(i * 2, 2).Replace(" ", ""), out UInt16 tmp))//判断是否可以转换
                {
                    Result[i] = tmp;
                }
            }
            return Result;
        }
        //只计算CRC数值 只能校验ASCII 0-127覆盖的范围，后续可以覆盖0-255的byte校验
        public UInt16 Cal_Crc(string inStr)
        {
            byte[] data = null;
            //将发送的字符串转化为byte
            data = Encoding.ASCII.GetBytes(inStr.Trim());
            //CRC校准方式
            //compCRC.Init(CRCTool.CRCCode.CRC16);
            //ushort usCrc16 = (ushort)compCRC.crctablefast(data);
            //return usCrc16;
            //Check_Sum
            return (ushort)compCRC.Check_Sum(data);
        }       

    }
}
