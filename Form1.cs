using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using FireSharp.Config;
using FireSharp.Interfaces;
using FireSharp.Response;



namespace deparseTool
{
    public partial class Form1 : Form
    {
        IFirebaseConfig config = new FirebaseConfig
        {
            AuthSecret = "euXIwSf1qoXdcqULpMr2sGekOD3hC6UzPLUhUnbi",
            BasePath = "https://tsdv-oop-default-rtdb.firebaseio.com/"
        };

        IFirebaseClient client;

        string InputData;
        delegate void SetTextCallback(string text);
        public Form1()
        {
            InitializeComponent();
            serialPort1.DataReceived += new SerialDataReceivedEventHandler(DataReceive);
        }
        private void DataReceive(object obj, SerialDataReceivedEventArgs e)
        {
            try
            {
                Thread.Sleep(500);
                InputData = serialPort1.ReadExisting();
                if (InputData != null)
                {
                    this.BeginInvoke(new SetTextCallback(SetText), new object[] { InputData });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Close Serial Port!");
            }
        }
        private void SetText(string text)
        {
            textBoxSMS.Text = InputData.Trim();
            if (textBoxSMS.Text != null) {
                deparseSMS(textBoxSMS.Text);
            }
        }

        private void deparseSMS(string text) {
            int index = text.IndexOf("ND");
            int index2 = text.IndexOf("GD +");
            int index3 = text.IndexOf("DV");
            string ID = text.Substring(index, index+9);
            string fee = text.Substring(index2, index2 + 6);
            string service = text.Substring(index3, index3 + 3);
            switch (service)
            {
                case "DV1":
                    if (fee == "50000")
                    {
                        SetResponse res = client.Set(@"PointTable/" + ID + "/PayState", "OK");
                    }
                    else
                    {
                        SetResponse res = client.Set(@"PointTable/" + ID + "/PayState", fee);
                    }
                    break;

                case "DV2":
                    if (fee == "50000")
                    {
                        SetResponse res = client.Set(@"CheckExam/" + ID + "/PayState", "OK");
                    }
                    else
                    {
                        SetResponse res = client.Set(@"CheckExam/" + ID + "/PayState", fee);
                    }
                    break;

                case "DV3":
                    if (fee == "50000")
                    {
                        SetResponse res = client.Set(@"RegisExpClass/" + ID + "/PayState", "OK");
                    }
                    else
                    {
                        SetResponse res = client.Set(@"RegisExpClass/" + ID + "/PayState", fee);
                    }
                    break;
            }
           
        }

        private void button2_Click_1(object sender, EventArgs e)
        {
            try
            {
                if (!serialPort1.IsOpen)
                {
                    serialPort1.PortName = comboBox1.Text;
                    serialPort1.BaudRate = Convert.ToInt32(9600);
                    serialPort1.Open();
                }

            }
            catch (Exception exception)
            {
                if (!serialPort1.IsOpen)
                {
                    MessageBox.Show("Serial port is not connected");
                }
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            serialPort1.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            client = new FireSharp.FirebaseClient(config);
            comboBox1.DataSource = SerialPort.GetPortNames();
        }
        private void Form1Closed(object sender, FormClosedEventArgs e)
        {
            serialPort1.Close();
        }
    }
}
