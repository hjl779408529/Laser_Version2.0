using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Laser_Version2._0
{
    struct Laser_CC_Data
    {
        public string RW;//读写标志：Read和Write (01-read；00-write),Response for Read和Write (03-for read；02-for write)；
        public string DataSize;//数据大小 ASCII:0-255；度取命令时始终为0
        public string Address;//地址
        public string Com_Control;//控制命令
        public string Data;//数据
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
        static public RS232 Com_rs232 = new RS232();
        //构造函数
        public Laser_Operation()
        {
            
        }
        //读取数据
        public void Read(byte[] Address,byte[] CC)//读取数据没有D1-Dn
        {
            CC_Data.RW = System.Text.Encoding.Default.GetString(new byte[]{0x01});//读取标志
            CC_Data.DataSize = System.Text.Encoding.Default.GetString(new byte[] { 0x00 });//读取标志
            CC_Data.Address = System.Text.Encoding.Default.GetString(Address);//地址
            CC_Data.Com_Control = System.Text.Encoding.Default.GetString(CC);//控制指令
            //整合指令
            CC_Data.Sum = CC_Data.RW + CC_Data.DataSize + CC_Data.Address + CC_Data.Com_Control + CC_Data.Data;
        }
    }
}
