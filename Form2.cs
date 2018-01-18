using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace WindowsFormsApp1
{
    

    public partial class Form2 : Form
    {
        
        public Form2()
        {
            InitializeComponent();
            discover_();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            discover_();
        }
       


        SerialPort sp;
        private void button2_Click(object sender, EventArgs e)
        {
            sp = connect_(sp, textBox1, comboBox1);
            sp.DataReceived += Sp_DataReceived;
        }
        

        private void Sp_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            SetText(textBox1, textBox1.Text + "recieve << " + sp.ReadExisting() + "\r\n");            
        }
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            action_(sender, sp, textBox1,textBox3);
        }
        private void button3_Click(object sender, EventArgs e)
        {
            close_(sp);
        }



        SerialPort sp2;
        private void button5_Click(object sender, EventArgs e)
        {
            sp2 = connect_(sp2, textBox2, comboBox2);
            sp2.DataReceived += Sp_DataReceived2;
        }

        private void Sp_DataReceived2(object sender, SerialDataReceivedEventArgs e)
        {
            SetText(textBox2, textBox2.Text + "recieve << " + sp2.ReadExisting() + "\r\n");
        }

        private void radioButton_CheckedChanged2(object sender, EventArgs e)
        {
            action_(sender, sp2, textBox2,textBox4);
        }
        private void button4_Click(object sender, EventArgs e)
        {
            close_(sp2);
        }



        void discover_()
        {
            string[] s = SerialPort.GetPortNames(); comboBox1.Items.AddRange(s); comboBox2.Items.AddRange(s);
        }
        SerialPort connect_(SerialPort spx, TextBox tx, ComboBox cbx)
        {
            if (!String.IsNullOrEmpty(cbx.SelectedItem.ToString()))
            {
                try
                {
                    spx = new SerialPort(cbx.SelectedItem.ToString(), 9600);                   
                    spx.DtrEnable = true;
                    spx.Open();
                    tx.Text = "Connected to " + cbx.SelectedItem;
                }
                catch (Exception ex)
                {
                    tx.Text = "Connection failed for " + cbx.SelectedItem;
                }
            }
            return spx;
        }
        void action_(object sender,SerialPort spx,TextBox tx,TextBox tx2=null)
        {
            RadioButton rb = sender as RadioButton;
            if (rb.Checked)
            {
                if (spx != null && spx.IsOpen) { spx.Write(rb.Text + (tx2==null?"":tx2.Text)); tx.Text += "send >> " + rb.Text + "\r\n"; }
                else tx.Text += "please connect...\r\n";
            }
        }
        void close_(SerialPort spx)
        {
            if (spx != null && spx.IsOpen) spx.Close();

        }




        private void SetText(TextBox txt, string text)
        {
            if (txt.InvokeRequired)
            {
                Invoke((MethodInvoker)(() => txt.Text = text));
            }
            else
            {
                txt.Text = text;
            }
        }

    }
}
