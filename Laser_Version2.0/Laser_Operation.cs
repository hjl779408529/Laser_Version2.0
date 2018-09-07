using Communication.IO.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Laser_Version2._0
{
    struct Laser_CC_Data
    {
        public string RW;//读写标志：Read和Write (01-read；00-write),Response for Read和Write (03-for read；02-for write)；
        public string DataSize;//数据大小 ASCII:0-255；度取命令时始终为0
        public string Address;//地址
        public string Com_Control;//控制命令
        public string Data;//数据
        public UInt16 Crc;//CRC校验值
        public string Sum;//数据整合

        public void Empty()
        {
            RW = "";//读写标志：Read和Write (01-read；00-write),Response for Read和Write (03-for read；02-for write)；
            DataSize = "";//数据大小 ASCII:0-255；
            Address = "";//地址
            Com_Control = "";//控制命令
            Data = "";//数据
            Sum="";//数据整合
        }
    }
    
    class Laser_Operation
    {
        //命令数据
        private Laser_CC_Data CC_Data = new Laser_CC_Data();
        private static CRCTool compCRC = new CRCTool();
        //构造函数
        public Laser_Operation()
        {
            
        }
        //读取数据
        public void Read(byte[] Address,byte[] CC)//读取数据没有D1-Dn
        {
            CC_Data.RW = System.Text.Encoding.Default.GetString(new byte[]{0x01});//读取标志
            CC_Data.DataSize = System.Text.Encoding.Default.GetString(new byte[] { 0x00 });//读取数据，DataSize大小强制为0
            CC_Data.Address = System.Text.Encoding.Default.GetString(Address);//地址
            CC_Data.Com_Control = System.Text.Encoding.Default.GetString(CC);//控制指令
            //整合指令
            CC_Data.Sum = CC_Data.RW + CC_Data.DataSize + CC_Data.Address + CC_Data.Com_Control + CC_Data.Data;

            //发送数据
            Initialization.Initial.Com_Comunication.Send_Data(CC_Data.Sum);

            byte[] tmp_Data = null;
            tmp_Data = Encoding.ASCII.GetBytes(CC_Data.Sum.Trim());
            string ASCIIstr2 = null;
            for (int j = 0; j < tmp_Data.Length; j++)
            {
                int asciicode = (int)(tmp_Data[j]);
                ASCIIstr2 += Convert.ToString(asciicode);//字符串ASCIIstr2 为对应的ASCII字符串
            }
            MessageBox.Show(string.Format("{0}"+ ASCIIstr2,Cal_Crc(CC_Data.Sum)));
            //UInt16 Cal_Num = Cal_Crc(CC_Data.Sum);
        }
        //读取数据
        public void Write(byte[] Address, byte[] CC,byte[] Data)//写入数据，这就包含写入数据的参数：D1-Dn 
        {
            CC_Data.RW = System.Text.Encoding.Default.GetString(new byte[] { 0x00 });//写入标志
            CC_Data.DataSize = Append_Num_Str(Convert.ToUInt32(Data.Length));//写入数据，DataSize
            CC_Data.Address = System.Text.Encoding.Default.GetString(Address);//地址
            CC_Data.Com_Control = System.Text.Encoding.Default.GetString(CC);//控制指令
            CC_Data.Data = System.Text.Encoding.Default.GetString(Data);//数据
            //整合指令
            CC_Data.Sum = CC_Data.RW + CC_Data.DataSize + CC_Data.Address + CC_Data.Com_Control + CC_Data.Data;

            //发送数据
            Initialization.Initial.Com_Comunication.Send_Data(CC_Data.Sum);

            byte[] tmp_Data = null;
            tmp_Data = Encoding.ASCII.GetBytes(CC_Data.Sum.Trim());
            string ASCIIstr2 = null;
            for (int j = 0; j < tmp_Data.Length; j++)
            {
                int asciicode = (int)(tmp_Data[j]);
                ASCIIstr2 += Convert.ToString(asciicode);//字符串ASCIIstr2 为对应的ASCII字符串
            }
            MessageBox.Show(string.Format("{0}" + ASCIIstr2, Cal_Crc(CC_Data.Sum)));
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
        //Hex字符串转换16进制字节数组 只支持为数字的字符串
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
            string tempStr = "";
            if (Num <= 255)
            {
                tempStr = string.Format("{0:X2}", Num);
            }
            else
            {
                tempStr = string.Format("{0:X4}", Num);
            }
            byte[] tempByte = StrToHexByte(tempStr.Trim());
            string Result = System.Text.Encoding.Default.GetString(tempByte);
            return Result;
        }
    }
}
