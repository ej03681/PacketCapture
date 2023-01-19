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
    public partial class frmSend : Form
    {
        public static int instantiations = 0;
        public int doodle;


        public frmSend()
        {
            InitializeComponent();
            instantiations++;

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openFileDialog1.Filter = "Text files|*.txt|All Files|*.*";
            openFileDialog1.Title = "Open Captured Packets";
            openFileDialog1.ShowDialog();

            if (openFileDialog1.FileName != "")
            {
                txtPacket.Text = System.IO.File.ReadAllText(openFileDialog1.FileName);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            saveFileDialog1.Filter = "Text files|*.txt|All Files|*.*";
            saveFileDialog1.Title = "Save the Captured Packets";
            saveFileDialog1.ShowDialog();

            if (saveFileDialog1.FileName != "")
            {
                System.IO.File.WriteAllText(saveFileDialog1.FileName, txtPacket.Text);
            }
        }

        private void frmSend_FormClosed(object sender, FormClosedEventArgs e)
        {
            instantiations--;
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            string stringBytes = "";
            // Get the hex values
            foreach (string s in txtPacket.Lines)
            {
                string[] noComments = s.Split('#');
                string s1 = noComments[0];
                stringBytes += s1 + Environment.NewLine;


            }
            //Extract the hex values into a byte array
            string[] sBytes = stringBytes.Split(new string[] { "\n", "\r\n", " ", "\t" }, StringSplitOptions.RemoveEmptyEntries);

            //Change the strings int bytes
            byte[] packet = new byte[sBytes.Length];
            int i = 0;
            foreach (string s in sBytes)
            {
                packet[i] = Convert.ToByte(s, 16);
                i++;

            }
            //Send packets
            try
            {
                frmCapture.device.SendPacket(packet);
            }
            catch(Exception ex)
            {

            }

        }
    }
}
