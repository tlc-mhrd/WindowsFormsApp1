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
using System.Net;
using System.Threading;

namespace WindowsFormsApp1
{
    

    public partial class Form3 : Form
    {
        
        public Form3()
        {
            InitializeComponent();
            discover_();
            asyncget(0);

            /*            
            for(int i=0;i<20;i++)
                asyncsend(i);
            */

        }
        private void br_Click(object sender, EventArgs e)
        {
            discover_();
            clear_();
            SetText("RESET!");
        }   
        void clear_()
        {
            close_('r');
            close_('g');
            close_('w');
            stop = true;
            if (textBox1.InvokeRequired)
            {
                Invoke((MethodInvoker)(() => textBox1.Text = ""));
            }
            else textBox1.Text = "";

        }
        void discover_()
        {
            string[] s = SerialPort.GetPortNames(); c1.Items.AddRange(s); c2.Items.AddRange(s); c3.Items.AddRange(s);
        }

        SerialPort spR,spG,spW;
        bool close_(char s)
        {
            try
            {
              
                if (s == 'r')
                {
                    close_(spR); label1.Text = "off"; label1.BackColor = Color.Red;
                    return true;
                }
                else if (s == 'g')
                {
                    close_(spG); label2.Text = "off"; label2.BackColor = Color.Red;
                    return true;
                }
                else if (s == 'w')
                {
                    close_(spW); label3.Text = "off"; label3.BackColor = Color.Red;
                    return true;
                }
            }
            catch (Exception ex)
            {
                SetText("Close failed for `" + s + "`");
            }
            return !true;
        }
        bool connect_(char s) 
        {
            try
            {
                string prt = "";
                if (s == 'r')
                {
                    spR = new SerialPort(c1.SelectedItem + "", 9600,Parity.Even,8,StopBits.One); spR.WriteTimeout = spR.ReadTimeout = 500; spR.Open();
                    SetText("Connected to RED");
                    return true;
                }
                else if (s == 'g')
                {
                    spG = new SerialPort(c2.SelectedItem + "", 9600, Parity.Even, 8, StopBits.One); spG.WriteTimeout = spG.ReadTimeout = 500; spG.Open();//spG.DtrEnable = true;                  
                    SetText("Connected to GREEN");
                    return true;
                }
                else if (s == 'w')
                {
                    spW = new SerialPort(c3.SelectedItem+"", 9600, Parity.Even, 8, StopBits.One); spW.WriteTimeout = spW.ReadTimeout = 500; spW.Open(); //spW.DtrEnable = true;
                    SetText("Connected to WHITE");
                    return true;
                }
            }
            catch (Exception ex)
            {
                SetText("Connection failed for `" + s + "`");
            }
            return !true;
        }

        private void brc_Click(object sender, EventArgs e)
        {
            if (connect_('r')) { label1.Text = "on"; label1.BackColor = Color.Green; } else {  }
        }

        private void bgc_Click(object sender, EventArgs e)
        {
            if (connect_('g')) { label2.Text = "on"; label2.BackColor = Color.Green; } else { label2.Text = "off"; label2.BackColor = Color.Red; }
        }

        private void bwc_Click(object sender, EventArgs e)
        {
            if (connect_('w')) { label3.Text = "on"; label3.BackColor = Color.Green; } else { label3.Text = "off"; label3.BackColor = Color.Red; }
        }


        void close_(SerialPort spx)
        {
            if (spx != null && spx.IsOpen) spx.Close(); spx = null;

        }
        private void brd_Click(object sender, EventArgs e)
        {
            close_('r');
        }

        private void bgd_Click(object sender, EventArgs e)
        {
            close_('g');
        }

        private void bwd_Click(object sender, EventArgs e)
        {
            close_('w');
        }
        void action_(SerialPort sp, string act)
        {
            if (sp != null && sp.IsOpen)
            {
                try
                {
                    //spx.Write(new char[] { act[0] },0,1);
                    byte[] bytestosend = { Convert.ToByte(act[0]) };

                    sp.Write(bytestosend, 0, bytestosend.Length);

                    SetText("send >> " + act);
                }
                catch
                {
                    SetText("<< timeout >> "+act);
                }
            }
        }

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            RadioButton act__ = sender as RadioButton;
            if (!act__.Checked) return;

            SerialPort sp_ = rbb1.Checked ? spR : (rbb2.Checked ? spG : (rbb3.Checked ? spW : null));

            action_(sp_, "S");
        }
        private void radioButton_CheckedChanged2(object sender, EventArgs e)
        {
            RadioButton act__ = sender as RadioButton;
            if (!act__.Checked) return;

            SerialPort sp_ = rbb1.Checked ? spR : (rbb2.Checked ? spG : (rbb3.Checked ? spW : null));
            
            action_(sp_, act__.Text);
        }

        string prev0, prev4, prev2;
        private void asyncget(int i)
        {
            int a = Environment.TickCount;
            try
            {
                Thread.Sleep(120);
                string test = GetResponseText("http://iiitdm.ac.in/test/getdata.php");
                if (test != "")
                {
                    SetText(test + "," + (Environment.TickCount - a));
                    string[] values = test.Split(',');

                    //red,blue,gray,yellow,green
                    //SerialPort sp_ = rbb1.Checked ? spR : (rbb2.Checked ? spG : (rbb3.Checked ? spW : null));

                    if (values[0] != "") { if (prev0 != values[0]) { prev0 = values[0]; action_(spR, i2s(prev0)); } }
                    if (values[4] != "") { if (prev4 != values[4]) { prev4 = values[4]; action_(spG, i2s(prev4)); } }
                    if (values[2] != "") { if (prev2 != values[2]) { prev2 = values[2]; action_(spW, i2s(prev2)); } }


                }
            }
            catch
            {
                SetText("Request failed!");
            }
          
        }
        private string i2s(string a)
        {
            if (a == "0") return "S";
            else if (a == "1") return "F";
            else if (a == "3") return "B";
            else if (a == "4") return "L";
            else if (a == "2") return "R";
            return "S";
        }

        bool stop = false;
        private void button1_Click(object sender, EventArgs e)
        {
            stop = false;
            asyncget(0);
            ThreadPool.QueueUserWorkItem(o =>
            {
                if (!stop) SetText("STARTED!");

                while (!stop)
                    asyncget(0);

                if (stop) SetText("STOPPED!");
            });
        }

        private void button2_Click(object sender, EventArgs e)
        {
            stop = true;
        }

        private void Form3_FormClosing(object sender, FormClosingEventArgs e)
        {
            clear_();
        }

        public string GetResponseText(string address)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(address);

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    var encoding = Encoding.GetEncoding(response.CharacterSet);

                    using (var responseStream = response.GetResponseStream())
                    using (var reader = new System.IO.StreamReader(responseStream, encoding))
                        return reader.ReadToEnd();
                }
            }catch
            {
                SetText("Request Response failed!");
               
            }
            return "";
        }
        private void SetText(string text)
        {
            if (textBox1.InvokeRequired)
            {
                Invoke((MethodInvoker)(() => textBox1.Text += text + "\r\n"));
            }
            else
            {
                textBox1.Text += text + "\r\n";
            }
        }
        
       
    }
}
