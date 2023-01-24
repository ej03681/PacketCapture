using System.Security.Cryptography;
using System.Text;

namespace myRSA
{
    public partial class Form1 : Form
    {
        String publicKey, privateKey;
        UnicodeEncoding encoder = new UnicodeEncoding();

        public Form1()
        {
            RSACryptoServiceProvider myRSA = new RSACryptoServiceProvider();
            InitializeComponent();
            privateKey = myRSA.ToXmlString(true);
            publicKey = myRSA.ToXmlString(false);

        }

        private void btnDecrypt_Click(object sender, EventArgs e)
        {
            var myRSA = new RSACryptoServiceProvider();
            var dataArray = textCT.Text.Split(new char[] { ',' });

            byte[] dataByte = new byte[dataArray.Length];
            for (int i = 0; i < dataArray.Length; i++)
            {
                dataByte[i] = Convert.ToByte(dataArray[i]);
            }
            
            myRSA.FromXmlString(privateKey);
            var decryptedBytes = myRSA.Decrypt(dataByte, false);

            textPT.Text = encoder.GetString(decryptedBytes);
        }

        private void textBox2_TextChanged(object sender, EventArgs e)
        {

        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textPT.Text = "";
            textPT.Refresh();
        }

        private void btnClear2_Click(object sender, EventArgs e)
        {
            textCT.Text = "";
            textCT.Refresh();
        }

        private void btnEncrypt_Click(object sender, EventArgs e)
        {
            var myRSA = new RSACryptoServiceProvider();
            myRSA.FromXmlString(publicKey);

            byte[] dataToEncrypt = encoder.GetBytes(textPT.Text);

            byte[] encryptedByteArray = myRSA.Encrypt(dataToEncrypt, false).ToArray();

            var length = encryptedByteArray.Count();
            var item = 0;
            var sb = new StringBuilder();
            
            foreach(var x in encryptedByteArray)
            {
                item++;
                sb.Append(x);
                if (item < length) sb.Append(",");
                textCT.Text = sb.ToString(); ;

            }
        }
    }
}