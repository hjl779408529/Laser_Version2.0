using Communication.IO.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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
        public string Crc;//CRC校验值
        public string Sum;//数据整合
        public UInt16[] Rec;//接收数据，指向数组的头部的指针
        public byte[] Rec_Byte;//接收数据byte，指向数组的头部的指针

        public void Empty()
        {
            RW = null;//读写标志：Read和Write (01-read；00-write),Response for Read和Write (03-for read；02-for write)；
            DataSize = null;//数据大小 ASCII:0-255；
            Address = null;//地址
            Com_Control = null;//控制命令
            Data = null;//数据
            Crc = null;//CRC校验值
            Sum = null;//数据整合
            Rec = null;
            Rec_Byte = null;
        }
    }
    
    class Laser_Operation
    {        
        //构造函数
        public Laser_Operation()
        {
            
        }
        //读取数据
        public static void Read(string Address,string CC)//读取数据没有D1-Dn
        {
            Laser_CC_Data CC_Data = new Laser_CC_Data();
            CC_Data.RW = "01";//读取标志
            CC_Data.DataSize = "00";//读取数据，DataSize大小强制为0
            CC_Data.Address = Address;//地址
            CC_Data.Com_Control = CC;//控制指令
            //整合指令
            CC_Data.Sum = CC_Data.RW + CC_Data.DataSize + CC_Data.Address + CC_Data.Com_Control + CC_Data.Data;
            //MessageBox.Show(CC_Data.Sum);
            //发送数据
            Initialization.Initial.Com_Comunication.Send_Data(CC_Data.Sum);
            //等待数据读取完成
            Thread.Sleep(200);
        }
        //写入数据
        public static void Write(string Address, string CC,string Data)//写入数据，这就包含写入数据的参数：D1-Dn 
        {
            Laser_CC_Data CC_Data = new Laser_CC_Data();
            CC_Data.RW = "00";//写入标志
            CC_Data.DataSize = Cal_Data_Size(Convert.ToUInt32(Data.Length/2));//写入数据，DataSize
            CC_Data.Address = Address;//地址
            CC_Data.Com_Control = CC;//控制指令
            CC_Data.Data = Data;//数据
            //整合指令
            CC_Data.Sum = CC_Data.RW + CC_Data.DataSize + CC_Data.Address + CC_Data.Com_Control + CC_Data.Data;
            //MessageBox.Show(CC_Data.Sum);
            //发送数据
            Initialization.Initial.Com_Comunication.Send_Data(CC_Data.Sum);
            //等待数据读取完成
            Thread.Sleep(200);
        }
        //将数值10进制转16进制，再将16进制转换为字符串返回 中心是byte转化为ASCII
        public static string Cal_Data_Size(UInt32 Num) 
        {
            string tempStr = null;
            if (Num <= 255)
            {
                tempStr = string.Format("{0:X2}", Num);
            }
            else
            {
                MessageBox.Show("发送数据长度异常！！！");
                return tempStr;
            }
            return tempStr;
        }
        //将数值10进制转16进制，再将16进制转换为字符串返回 中心是byte转化为ASCII  5000 - 1388
        public static string Uint_To_Str(UInt32 Num) 
        {
            string tempStr = null;
            tempStr = string.Format("{0:X4}", Num);
            if (tempStr.Length == 3)
            {
                tempStr = "0" + tempStr;
            }
            else if (tempStr.Length == 2)
            {
                tempStr = "00" + tempStr;
            }
            else if (tempStr.Length == 1)
            {
                tempStr = "000" + tempStr;
            }
            else if (tempStr.Length == 0)
            {
                tempStr = "0000";
            }
            return tempStr;
        }
        //将数值10进制转16进制，再将16进制转换为字符串返回 中心是byte转化为ASCII  5000 - 1388
        public static string PRF_To_Str(UInt32 Num) 
        {
            string tempStr = null;
            tempStr = string.Format("{0:X4}", Num);
            //MessageBox.Show(tempStr);
            if (tempStr.Length==8)
            {
                tempStr = tempStr.Substring(1);
                tempStr = tempStr.Substring(1);
            }
            else if (tempStr.Length == 7)
            {
                tempStr = tempStr.Substring(1);
            }
            else if (tempStr.Length == 5)
            {
                tempStr = "0" + tempStr;
            }
            else if (tempStr.Length == 4)
            {
                tempStr = "00" + tempStr;
            }
            else if (tempStr.Length == 3)
            {
                tempStr = "000" + tempStr;
            }
            //补位
            if ((tempStr.Length % 2) != 0) tempStr = "0" + tempStr;
            //MessageBox.Show(tempStr);
            return tempStr;
        }

    }
}
