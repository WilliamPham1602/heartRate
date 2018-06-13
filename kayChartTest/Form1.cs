using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using rtChart;
using System.Diagnostics;
using System.IO.Ports;
using System.Data.SqlClient;
using System.Configuration;
using System.Data.Sql;

namespace kayChartTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        kayChart serialDataChart;
        private void button1_Click(object sender, EventArgs e)
        {
            SerialPort aSerialPort = new SerialPort("COM4");
            aSerialPort.BaudRate = 115200;
            aSerialPort.Parity = Parity.None;
            aSerialPort.StopBits = StopBits.One;
            aSerialPort.DataBits = 8;
            aSerialPort.DataReceived += new SerialDataReceivedEventHandler(serialDataReceivedEventHandler);

            if (!aSerialPort.IsOpen)
            {
                aSerialPort.Open();
            }
        }

        private void serialDataReceivedEventHandler(object sender, SerialDataReceivedEventArgs e)
        {
            SerialPort sData = sender as SerialPort;
            string recvData = sData.ReadLine();
            serialData.Invoke((MethodInvoker)delegate { serialData.AppendText("Received: " + recvData); });
            // initialization of chart update
            int data;
            bool result = int.TryParse(recvData, out data);
            if (result)
            {
                serialDataChart.TriggeredUpdate(data);
            }
            // send data to SQL
            SqlCommand cmnd;
            SqlConnection con;
            con = new SqlConnection(@"Data Source=william1602.database.windows.net;Initial Catalog=heart_rate sensor;Persist Security Info=True;User ID=williampham;Password=Will1602* ");
            con.Open();
            cmnd = new SqlCommand("Insert into Heart_rate (heartRate)values(@data)", con);
            cmnd.Parameters.AddWithValue("@data", data);
            cmnd.ExecuteNonQuery();
            con.Close();
        }
        
        private void Form1_Load(object sender, EventArgs e)
        {
            serialDataChart = new kayChart(chart1, 60);
            serialDataChart.serieName = "heartRate";

        }

        private void serialData_TextChanged(object sender, EventArgs e)
        {
            serialData.SelectionStart = serialData.Text.Length;
            serialData.ScrollToCaret();
        }
     }
    }

