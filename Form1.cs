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
using System.Diagnostics;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
       
        public Form1()
        {
            InitializeComponent();
            sp = new SerialPort[10];
        }

     
     

        private void Form1_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            //sp[0].ReadLine();
        }

        #region web
        void read()
        {
            ThreadPool.QueueUserWorkItem(o =>
            {
                WebClient client = new WebClient();
                Stopwatch sw;
                while (true)
                {
                    sw = Stopwatch.StartNew();
                    String s = client.DownloadString(@"http://tlc.iiitdm.ac.in/live/file2.txt");
                    sw.Stop();

                    SetText(textBox1, textBox1.Text + String.Format("Request took {0}", sw.Elapsed + "\r\n"));
                }
            });
        }
        #endregion

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

        #region loco
        void recieve()
        {
            open(0, 5);
            open(0, 8);
        }

        void move(int i, String cmd)
        {
            sp[i].Write(cmd); Thread.Sleep(100); sp[i].Write("S");
        }
        #endregion

        #region bluetooth
     
        int getCount(bool isOpenOnly = true)
        {
            int kc = 0, ko = 0;
            for (int i = 0; i < sp.Length; i++)
            {
                if (sp[i] != null)
                {
                    if (sp[i].IsOpen)
                        ko++;
                    kc++;
                }
            }
            return isOpenOnly ? ko : kc;
        }

        #endregion

        private void button1_Click(object sender, EventArgs e)
        {
            read();
        }








        private void getPorts_clk(object sender, EventArgs e)
        {
            string[] ArrayComPortsNames =  SerialPort.GetPortNames();
            comboBox1.Items.AddRange( ArrayComPortsNames );
        }




        SerialPort[] sp;
        void open(int i, int com)
        {
            if (sp[i] != null)
                if (sp[i].IsOpen)
                    close(i);

            sp[i] = new SerialPort("COM" + com, 9600);sp[i].DataReceived += Form1_DataReceived;sp[i].DtrEnable = true;sp[i].Open();
        }
        void close(int i)
        {
            if (sp[i] != null)if (sp[i].IsOpen)sp[i].Close();
        }
    }
}
