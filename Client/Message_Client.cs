using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using static System.Net.Mime.MediaTypeNames;
using System.IO;
using Image = System.Drawing.Image;

namespace Client
{
    public partial class Message_Client : Form
    {
        private Socket client;
        private IPEndPoint iPEndPoint;
        private int port = 8000;
        private IPAddress iPAddress;
        public Message_Client()
        {
            InitializeComponent();
            pictureBox1.Visible = false;
            MyConnect();
            CheckForIllegalCrossThreadCalls = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            sendMessage(richTextBox1.Text.ToString());
            listView1.Items.Add(richTextBox1.Text.ToString());
            richTextBox1.Text = "";
        }

        private void MyConnect()
        {
            client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            iPAddress = IPAddress.Parse("127.0.0.1");
            iPEndPoint = new IPEndPoint(iPAddress, port);
            try
            {
                client.Connect(iPEndPoint);
                Console.WriteLine("Connect success !!!");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Không thể kết nối được !!!", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            Thread listens = new Thread(ReceiveMess);
            listens.IsBackground = true;
            listens.Start();
        }

        private void sendMessage(String message)
        {
            if (message != String.Empty)
            {
                client.Send(enCode(message));
            }
            else
            {
                MessageBox.Show("Vui lòng nhập tin nhắn !");
            }
        }

        private void ReceiveMess()
        {
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);
                    string message = (string)deCode(data);
                    setMessagesForm(message);
                    richTextBox1.Text = message;   
                    
                    
                }
            }
            catch (Exception ecccc)
            {
                Close();
                Console.WriteLine(ecccc);
            }
        }

        private void setMessagesForm(string messgae)
        {
            if(messgae != null)
            {
                listView1.Items.Add(new ListViewItem() { Text = messgae });
            }
        }
        private byte[] enCode(string data)
        {
            return Encoding.Unicode.GetBytes(data);
        }
        private string deCode(byte[] data)
        {
            return Encoding.Unicode.GetString(data);
        }
        public Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            return image;
        }

        private void Message_Client_Load(object sender, EventArgs e)
        {
           
        }

        private void Message_Client_FormClosed(object sender, FormClosedEventArgs e)
        {
            client.Close();
        }

        private void setPictureBox(string pathImages)
        {
            pictureBox1.Visible = true;
            pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
            pictureBox1.Image = Base64ToImage(pathImages);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            setPictureBox(richTextBox1.Text.ToString());
        }
    }
}
