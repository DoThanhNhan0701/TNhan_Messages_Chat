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
using System.IO;
using static System.Net.Mime.MediaTypeNames;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using Image = System.Drawing.Image;
using System.Runtime.Remoting.Messaging;

namespace DoThanhNhan_MessageChat
{
    public partial class Message_Server : Form
    {
        private Socket server, client;
        private IPEndPoint iPEndPoint;
        private int port = 8000;
        private IPAddress iPAddress;
        private List<Socket> listSockets;
        private string url = "";

        public Message_Server()
        {
            InitializeComponent();
            pictureBox1.Visible = false;

            Connect();
            listSockets = new List<Socket>();
           
            
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Send();
        }

        private void Connect()
        {
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            iPAddress = IPAddress.Any;
            iPEndPoint = new IPEndPoint(iPAddress, port);
            server.Bind(iPEndPoint);

            Thread thread1 = new Thread(() =>
            {
                while (true)
                {
                    try
                    {
                        server.Listen(50);
                        client = server.Accept();
                        listSockets.Add(client);

                        Thread threadReceive = new Thread(ReceiveMess);
                        threadReceive.IsBackground = true;
                        threadReceive.Start(client);
                    }
                    catch (Exception ex)
                    {
                        iPEndPoint = new IPEndPoint(iPAddress, port);
                        server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                    }
                }

            });
            thread1.Start();
        }

        private void Send()
        {
            foreach (Socket item in listSockets)
            {
                checkedListBox1.Items.Add(item.ToString());
                sendMessage(item, richTextBox1.Text.ToString());
                Console.WriteLine(item.ToString()); 
            }
            listView1.Items.Add(richTextBox1.Text.ToString());
            pictureBox1.Visible = false;
            richTextBox1.Text = "";
        }
  
        private void ReceiveMess(object obj)
        {
            Socket client = obj as Socket;
            try
            {
                while (true)
                {
                    byte[] data = new byte[1024 * 5000];
                    client.Receive(data);
                    string message = deCode(data);
                   /* Base64ToImage(message);*/

                    setMessagesForm(message);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                listSockets.Remove(client);
                client.Close();
            }
        }

        private void setMessagesForm(string messgae)
        {
            listView1.Items.Add(new ListViewItem() { Text = messgae });
        }
        private void setDataClient()
        {
            for(int i = 0; i < listSockets.Count; i++)
            {
                if(listSockets[i] != null)
                {
                    checkedListBox1.Items.Add(listSockets[i].ToString());
                }
                else
                {
                    MessageBox.Show("Không có Client");
                }
            }
        }

        private void sendMessage(Socket client, string message)
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
        private void button2_Click(object sender, EventArgs e)
        {
            sendImages();
        }

        private void sendImages()
        {
            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Title = "Open Image";
                dlg.Filter = "jpg files (*.jpg)|*.jpg";

                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                    pictureBox1.Image = new Bitmap(dlg.FileName);
                    url = dlg.FileName;
                }
            }
            richTextBox1.Text = ImageToBase64(url);
            pictureBox1.Visible = true;
        }

        private void setImagesList(string pathImages)
        {
            ImageList images = new ImageList();
            images.ImageSize = new Size(80, 80);
        }

        private byte[] enCode(string data)
        {
            return Encoding.Unicode.GetBytes(data);
        }

        private void Message_Server_Load(object sender, EventArgs e)
        {
            listView1.View = View.Details;
            setDataClient();
        }
        private string ImageToBase64(string path)
        {
            using (System.Drawing.Image image = System.Drawing.Image.FromFile(path))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }

        private Image Base64ToImage(string base64String)
        {
            byte[] imageBytes = Convert.FromBase64String(base64String);
            MemoryStream ms = new MemoryStream(imageBytes, 0, imageBytes.Length);
            ms.Write(imageBytes, 0, imageBytes.Length);
            System.Drawing.Image image = System.Drawing.Image.FromStream(ms, true);
            return image;
        }

        private void button4_Click(object sender, EventArgs e)
        {
        
        }

        private string deCode(byte[] data)
        {
            return Encoding.Unicode.GetString(data);
        }

    }
}
