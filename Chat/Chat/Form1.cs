using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;

namespace Chat
{
    public partial class Form1 : Form
    {
        Socket socket;
        EndPoint endPointLocal, endPointRemote;

        public Form1()
        {
            InitializeComponent();

            socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.ReuseAddress, true);

            txtIP_1.Text = GetLocal();
            txtIP_2.Text = GetLocal();
        }

        public string GetLocal()
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());

            foreach(IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }

            return "127.0.0.1";
        }

        public void MessageCallBack(IAsyncResult asyncResult)
        {
            try
            {
                int tam = socket.EndReceiveFrom(asyncResult, ref endPointRemote);


                if (tam > 0)
                {
                    byte[] receiveData = (byte[])asyncResult.AsyncState;

                    ASCIIEncoding ecg = new ASCIIEncoding();

                    string receiveMessage = ecg.GetString(receiveData);

                    listMessage.Items.Add(txtName_2.Text + " disse: " + receiveMessage);
                }

                byte[] buffer = new byte[1500];

                socket.BeginReceiveFrom(buffer, 0, buffer.Length, 
                                        SocketFlags.None, ref endPointRemote, 
                                        new AsyncCallback(MessageCallBack), buffer);
            }
            catch(Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }

        private void btnConnect_Click(object sender, EventArgs e)
        {
            txtIP_1.Enabled = false;
            txtIP_2.Enabled = false;
            txtName_1.Enabled = false;
            txtName_2.Enabled = false;
            txtPort_1.Enabled = false;
            txtPort_2.Enabled = false;

            try
            {
                endPointLocal = new IPEndPoint(IPAddress.Parse(txtIP_1.Text), Convert.ToInt32(txtPort_1.Text));
                socket.Bind(endPointLocal);

                endPointRemote = new IPEndPoint(IPAddress.Parse(txtIP_2.Text), Convert.ToInt32(txtPort_2.Text));
                socket.Connect(endPointRemote);

                byte[] buffer = new byte[1500];

                socket.BeginReceiveFrom(buffer, 0, buffer.Length,
                                        SocketFlags.None, ref endPointRemote,
                                        new AsyncCallback(MessageCallBack), buffer);

                btnConnect.Enabled = false;
                txtMessage.Enabled = true;
                txtMessage.Focus();

                btnSend.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            try
            {
                System.Text.ASCIIEncoding enc = new System.Text.ASCIIEncoding();

                byte[] msg = enc.GetBytes(txtMessage.Text);

                socket.Send(msg);

                listMessage.Items.Add(txtName_1.Text + " disse:" + txtMessage.Text);
                txtMessage.Clear();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }
    }
}
