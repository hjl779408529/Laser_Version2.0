using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using Initialization;

namespace Laser_Version2._0
{
    public partial class Laser_Control_Panel : Form
    {
        public Laser_Control_Panel()
        {
            InitializeComponent();
        }
        //Laser_Control  通讯控制
        Laser_Operation Laser_Control = new Laser_Operation();
        private void Laser_Control_Panel_Load(object sender, EventArgs e)
        {
            richTextBox1.AppendText("Running"+"\r\n");
            richTextBox1.AppendText("PowerOn" + "\r\n");
            richTextBox1.AppendText("Shutter Enabled" + "\r\n");
            richTextBox1.AppendText("Key Switch On" + "\r\n");
            richTextBox1.AppendText("LDD On" + "\r\n");
            richTextBox1.AppendText("QSW On" + "\r\n");
            richTextBox1.AppendText("Shutter Interlock" + "\r\n");
            richTextBox1.AppendText("LDD Interlock" + "\r\n");
            ChangeKeyColor("LDD Interlock",Color.Blue);

            
            //初始化通讯端口列表
            Com_List.Items.AddRange(Initialization.Initial.Com_Comunication.PortName.ToArray());
            //初始化默认的Com端口
            Com_List.SelectedIndex = Para_List.Parameter.Com_No;

        }

        public void ChangeKeyColor(string key, Color color)
        {
            Regex regex = new Regex(key);
            //找出内容中所有的要替换的关键字
            MatchCollection collection = regex.Matches(richTextBox1.Text);
            //对所有的要替换颜色的关键字逐个替换颜色    
            foreach (Match match in collection)
            {
                //开始位置、长度、颜色缺一不可
                richTextBox1.SelectionStart = match.Index;
                richTextBox1.SelectionLength = key.Length;
                richTextBox1.SelectionColor = color;
            }
        }
        //更改串口端口号
        private void Com_List_SelectedIndexChanged(object sender, EventArgs e)
        {
            Para_List.Parameter.Com_No = Com_List.SelectedIndex;
        }
        //状态更新
        private void button3_Click(object sender, EventArgs e)
        {
            Laser_Control.Write(new byte[]{0x00},new byte[]{0x07 }, new byte[] { 0x07 ,0x10}); 
        }
    }
}
