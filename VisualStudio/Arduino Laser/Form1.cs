using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.IO.Ports;
using System.Runtime.InteropServices;
using System.Management;
using System.Diagnostics;
using System.Threading.Tasks;

//using System.Management;

namespace Arduino_Laser
{

    public partial class Form1 : Form
    {
        SerialPort port;
        int Trials = 0;
        double BaseTime;
        bool Paused = false;
        decimal BlockageLength = 10;
        //DataTable source = new DataTable();
        //int MyCount;

        Stopwatch sw;


        public Form1()
        {
            InitializeComponent();
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(64, 70, 78);
            dataGridView.Controls[0].BackColor = Color.FromArgb(64, 70, 78);
            this.MaximizeBox = false;
            //Form1.CheckForIllegalCrossThreadCalls = false;
            /*source.Columns.Add();
            source.Columns.Add();
            source.Columns.Add();
            source.Columns.Add();
            source.Columns.Add();
            dataGridView.DataSource = source;
            //source.TableNewRow += Source_TableNewRow;*/
            //dataGridView
            /*-----------------DELETE ME LATER!!!--------------*/  tbPort.Text = "COM3"; // DELETE ME----------------------------------------------
        }

       

        int init()
        {
            if (tbPort.Text == "Auto")
            {
                try
                {
                    string[] ports = SerialPort.GetPortNames();
                    foreach (string portStr in ports)
                    {
                        port = new SerialPort();
                        port.NewLine = "z";
                        port.BaudRate = 57600;
                        port.PortName = portStr;
                        port.Open();
                        port.Write("yo");
                        port.ReadTimeout = 1500;
                        string lol = port.ReadLine();
                        if (lol == "sup")
                        {
                            port.DataReceived += new SerialDataReceivedEventHandler(dataRecieved);
                            port.ReadTimeout = -1;
                            return 1;
                        }
                    }
                }
                catch (Exception y) { }
            }
            else
            {
                try
                {
                    port = new SerialPort();
                    port.NewLine = "z";
                    port.BaudRate = 57600;
                    port.PortName = tbPort.Text;
                    port.Open();
                    port.ReadTimeout = 1500;
                    port.Write("yo");
                    string lol = port.ReadLine();
                    if (lol == "sup")
                    {
                        port.DataReceived += new SerialDataReceivedEventHandler(dataRecieved);
                        port.ReadTimeout = -1;
                        return 1;
                    }
                }
                catch (Exception y) { }
            }
            return 0;
        }

        private void dataRecieved(object sender, SerialDataReceivedEventArgs e)
        {
            
            if (Paused)
            {
                port.ReadExisting();
                return;
            }

            double time;
            if (Trials == 0)
            {
                sw = new Stopwatch();
                sw.Start();
                time = 0;
            }
            else
            {
                time = sw.ElapsedMilliseconds;
            }
            byte[] buffer = new byte[4];  // { 1, 0, 0, 0};
//UInt32 bl0ckageTime = BitConverter.ToUInt32(buffer, 0);
            port.Read(buffer, 0, 4);
            UInt32 blockageTime = BitConverter.ToUInt32(buffer, 0);
            double blockageTimeSeconds = (double)blockageTime / 1000000;
            double MS = (((double)BlockageLength) / 1000) / blockageTimeSeconds;
            double MPH = MS * 2.23694;

            /*MyCount++;
            if (MyCount == 200)
            {
                MyCount = 200;
            }*/
            //long test = sw.ElapsedMilliseconds;

            /*string a = (Trials + 1).ToString();
            string b = time.ToString();
            string c = blockageTimeSeconds.ToString("N3");
            string d = MS.ToString("N3");
            string f = MPH.ToString("N3");*/
            //Invoke((MethodInvoker)(() =>   //THIS IS REALLY REALY SLOW FIX IT !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            //{
            //    dataGridView.Rows.Add(Trials + 1, (time / 1000).ToString("N3"), blockageTimeSeconds/*.ToString("N3")*/, MS.ToString("N3"), MPH.ToString("N3"));
            //    dataGridView.FirstDisplayedScrollingRowIndex = Trials;
            //}));

            dataGridView.Invoke( new MethodInvoker(() =>   //THIS IS REALLY REALY SLOW FIX IT !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            {
                dataGridView.Rows.Add(Trials + 1, (time / 1000).ToString("N3"), blockageTimeSeconds/*.ToString("N3")*/, MS.ToString("N3"), MPH.ToString("N3"));
                dataGridView.FirstDisplayedScrollingRowIndex = Trials;
            }));

            //MessageBox.Show((sw.ElapsedMilliseconds - test).ToString());

            //source.Rows.Add(Trials + 1, time, blockageTimeSeconds.ToString("N3"), MS.ToString("N3"), MPH.ToString("N3"));
            Trials++;

        }

        

        private void btnInit_Click(object sender, EventArgs e)
        {
            btnInit.Enabled = false;
            lblComPort.Enabled = false;
            tbPort.Enabled = false;
            lblFailed.Visible = false;
            lblConnected.Visible = false;
            if (init() == 0)
            {
                btnInit.Enabled = true;
                lblComPort.Enabled = true;
                tbPort.Enabled = true;
                lblFailed.Visible = true;
            }
            else
            {
                lblConnected.Visible = true;
                tbPort.Text = port.PortName;
            }

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            dataGridView.Rows.Clear();
            Trials = 0;
            port.ReadExisting();
            if (sw != null)
                sw.Stop();
        }

        private void lblAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Made by Johnathan and Frederick Mitri", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (port != null && port.IsOpen)
                port.Write("bye");
        }

        private void btnPause_Click(object sender, EventArgs e)
        {
            Paused = !Paused;
            if (Paused)
            {
                btnPause.Text = "Resume";
                btnPause.ForeColor = Color.FromArgb(222, 65, 65);
            }
            else
            {
                btnPause.Text = "Pause";
                btnPause.ForeColor = Color.FromArgb(155, 201, 242);
            }
        }

        private void btnTest_Click(object sender, EventArgs e)
        {
            MessageBox.Show(port.BytesToRead.ToString());
        }

        private void numBlockageLength_ValueChanged(object sender, EventArgs e)
        {
            BlockageLength = numBlockageLength.Value;
        }
    }
}
