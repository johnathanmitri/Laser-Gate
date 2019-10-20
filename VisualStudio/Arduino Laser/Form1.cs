﻿using System;
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
        UInt32 BaseTime;
        bool Paused = false;
        decimal BlockageLength = 10;
       
        public Form1()
        {
            InitializeComponent();
            dataGridView.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(64, 70, 78);
            dataGridView.Controls[0].BackColor = Color.FromArgb(64, 70, 78);
            this.MaximizeBox = false;

            port = new SerialPort();
            //Form1.CheckForIllegalCrossThreadCalls = false;
            /*source.Columns.Add();
            source.Columns.Add();
            source.Columns.Add();
            source.Columns.Add();
            source.Columns.Add();
            dataGridView.DataSource = source;
            //source.TableNewRow += Source_TableNewRow;*/
            //dataGridView
            
            //tbPort.Text = "COM3"; // DELETE ME----------------------------------------------
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
                        port.NewLine = "z";
                        port.BaudRate = 57600;
                        port.PortName = portStr;
                        port.Open();
                        port.Write("yo");
                        port.ReadTimeout = 1500;
                        string lol = "";
                        try
                        {
                            lol = port.ReadLine();
                        }
                        catch { port.Close(); continue; }
                        if (lol == "sup")
                        {
                            port.DataReceived += new SerialDataReceivedEventHandler(dataRecieved);
                            port.ReadTimeout = -1;
                            return 1;
                        }
                        port.Close();
                    }
                }
                catch (Exception y) {port.Close();}
            }
            else
            {
                try
                {
                    port.NewLine = "z";
                    port.BaudRate = 57600;
                    port.PortName = tbPort.Text;
                    port.Open();
                    port.ReadTimeout = 1500;
                    port.Write("yo");
                    string lol = "";
                    try
                    {
                        lol = port.ReadLine();
                    }
                    catch { port.Close(); }
                    if (lol == "sup")
                    {
                        port.DataReceived += new SerialDataReceivedEventHandler(dataRecieved);
                        port.ReadTimeout = -1;
                        return 1;
                    }
                    port.Close();
                }
                catch (Exception y) {port.Close();}
            }
            return 0;
        }

        private void dataRecieved(object sender, SerialDataReceivedEventArgs e)
        {
            if (Paused)
            {
                byte[] dogshit = new byte [8];
                port.Read(dogshit, 0, 8);
                return;
            }
            byte[] startBuffer = new byte[4];
            byte[] endBuffer = new byte[4];  // { 1, 0, 0, 0};
                    
            port.Read(endBuffer, 0, 4); //run this one first, because end values come first
            port.Read(startBuffer, 0, 4);

            UInt32 startTime = BitConverter.ToUInt32(startBuffer, 0);
            UInt32 endTime = BitConverter.ToUInt32(endBuffer, 0);

            

            double time;
            if (Trials == 0)
            {
                time = 0;        //microseconds
                BaseTime = startTime;
            }
            else
            {
                time = startTime - BaseTime;
            }

            UInt32 blockageTime = endTime - startTime;
            double blockageTimeSeconds = (double)blockageTime / 1000000;
            double MS = (((double)BlockageLength) / 1000) / blockageTimeSeconds;
            double MPH = MS * 2.23694;


            dataGridView.Invoke(new MethodInvoker(() =>   //THIS IS REALLY REALY SLOW FIX IT !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
            {
                dataGridView.Rows.Add(Trials + 1, (time / 1000000).ToString("N3"), blockageTimeSeconds.ToString("N4"), MS.ToString("N3"), MPH.ToString("N3"));
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
            if (port.BytesToRead > 7)
            {
                int size = 8*(port.BytesToRead / 8); //this is to make sure not to offset the data, and only dump data in a multiple of 8. ie, if theres 17 bytes to be read, we dump 16
                byte[] dump = new byte[size];
                port.Read(dump, 0, size);
            }
            
        }

        private void lblAbout_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Made by Johnathan and Frederick Mitri", "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (port != null && port.IsOpen)   //to avoid any errors. without it we error.
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
