using SharpPcap;
using PacketDotNet;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PacketCapture
{
    public partial class frmCapture : Form
    {
        CaptureDeviceList devices; //list of devices for this computer
        public static ICaptureDevice device;
        public static string stringPackets = ""; //data
        public static int numPackets = 0;
        frmSend fSend;  //Our send form


        public frmCapture()
        {
            InitializeComponent();
            devices = CaptureDeviceList.Instance;

            if (devices.Count < 1)
            {
                MessageBox.Show("No Capture Devices Found!!");
                Application.Exit();

            }
            //Add devices to combo box
            foreach (ICaptureDevice dev in devices)
            {
                cmbDevices.Items.Add(dev.Name);
            }

            //get the second device and display in combo box
            device = devices[1];
            cmbDevices.Text = device.Description;

            //Register our handler textbox
            device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);

            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);

        }

        private static void device_OnPacketArrival(object sender, CaptureEventArgs packet)
        {
            //increment num of packets captured
            numPackets++;

            stringPackets += "Packet Number: " + Convert.ToString(numPackets);
            stringPackets += Environment.NewLine;

            //array to store data
            byte[] data = packet.Packet.Data;

            //Keep track of the number of bytes displayed per line
            int byteCounter = 0;

            stringPackets += "Destination MAC Address";

            foreach (byte b in data)
            {
                //add byte to string(in hexidecimal)
                if (byteCounter <= 13) stringPackets += b.ToString("X2") + " ";
                byteCounter++;
                
                switch (byteCounter)
                {
                    case 6: stringPackets += Environment.NewLine;
                        stringPackets += "Source MAC Address";
                        break;

                    case 12: stringPackets += Environment.NewLine;
                        stringPackets += "EitherType";
                        break;
                    case 14: if (data[12] == 8)
                        {
                            if (data[13] == 0) stringPackets += "(IP)";
                            if (data[13] == 6) stringPackets += "(ARP)";

                        }
                        stringPackets += "(IP)";
                        break;
                }

                if (byteCounter == 16)
                {
                    byteCounter = 0;
                    stringPackets += Environment.NewLine;
                }
            }

            stringPackets += Environment.NewLine + Environment.NewLine;

            byteCounter = 0;
            stringPackets += "Raw Data" + Environment.NewLine;

            foreach (byte b in data)
            {
                //add byte to string(in hexidecimal)
                stringPackets += b.ToString("x2") + " ";
                byteCounter++;

                if (byteCounter == 16)
                {
                    byteCounter = 0;
                    stringPackets += Environment.NewLine;
                }
            }
            stringPackets += Environment.NewLine;
            stringPackets += Environment.NewLine;
        }

        private void cmbDevices_SelectedIndexChanged(object sender, EventArgs e)
        {
            device = devices[cmbDevices.SelectedIndex];
            cmbDevices.Text = device.Description;
            //Register our handler textbox
            device.OnPacketArrival += new SharpPcap.PacketArrivalEventHandler(device_OnPacketArrival);

            int readTimeoutMilliseconds = 1000;
            device.Open(DeviceMode.Promiscuous, readTimeoutMilliseconds);
        }

        private void btnStartStop_Click(object sender, EventArgs e)
        {
            try
            {
                if (btnStartStop.Text == "Start")
                {
                    device.StartCapture();
                    timer1.Enabled = true;
                    btnStartStop.Text = "Stop";
                }
                else
                {
                    device.StopCapture();
                    timer1.Enabled = false;
                    btnStartStop.Text = "Start";
                }
            }
            catch (Exception exp)
            {

            }
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            txtCaptureData.AppendText(stringPackets);
            stringPackets = "";
            txtNumPackets.Text = Convert.ToString(numPackets);

        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text files|*.txt|All Files|*.*";
            saveFileDialog1.Title = "Save the Captured Packets";
            saveFileDialog1.ShowDialog();

            if(saveFileDialog1.FileName != "")
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, txtCaptureData.Text);
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text files|*.txt|All Files|*.*";
            openFileDialog1.Title = "Open Captured Packets";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                txtCaptureData.Text = System.IO.File.ReadAllText(openFileDialog1.FileName);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void txtNumPackets_TextChanged(object sender, EventArgs e)
        {

        }

        private void sendWindowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (frmSend.instantiations == 0)
            {
                fSend = new frmSend(); //Creates a new frmSend
                fSend.Show();
            }
        }
    }
    
}
