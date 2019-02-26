using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace ClientServerModulesTest
{

    public partial class Control : Form
    {
        public Control()
        {
            InitializeComponent();
        }

        Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        Connection connectedClient = null;
        
        Module module = null;

        public void start()
        {
            listener.Bind(new IPEndPoint(IPAddress.Any, 6225));
            listener.Listen(10);
            Console.WriteLine("started listening...");
            listener.BeginAccept(new AsyncCallback(Accepted), null);
            buttonListen.Enabled = false;
        }

        private void Accepted(IAsyncResult ar)
        {
            Socket skt = listener.EndAccept(ar);
            if(skt.Connected)
            {
                Console.WriteLine("connection accepted.");
                Connection connection = new Connection(skt);
                connection.connectionLost = OnDisconnect;
                connectedClient = connection;
                module = connectedClient.GetModule();
                module.DataTransferred = new StreamUpdate(Sent); 
            }
        }

        private void Sent(DataStreamContext cxt)
        {
            if (cxt.Identifier == 1)
            {
                cxt.Cancelled = true;
            }
            cxt.Identifier = module.NextId();
            Console.WriteLine("Sent " + cxt.Identifier);
        }

        private void OnDisconnect(Connection c)
        {
            Console.WriteLine("connection closed");
            connectedClient = null;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            start();
            labelState.Text = "Listening...";
        }

        void SetPath(Label p, Button b)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                p.Text = ofd.FileName;
                b.Enabled = true;
            }
        }

        void Send(string path)
        {
            if (!File.Exists(path) || connectedClient == null)
                return;

            FileStream fs = new FileStream(path, FileMode.Open);
            module.Transmit(fs);
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SetPath(path1, buttonSnd1);
        }

        private void button6_Click(object sender, EventArgs e)
        {
            SetPath(path2, buttonSnd2);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            SetPath(path3, buttonSnd3);
        }

        //send buttons
        private void button3_Click(object sender, EventArgs e)
        {
            Send(path1.Text);
        }

        private void buttonSnd2_Click(object sender, EventArgs e)
        {
            Send(path2.Text);
        }

        private void buttonSnd3_Click(object sender, EventArgs e)
        {
            Send(path3.Text);
        }
        //send buttons end

        private void button3_Click_1(object sender, EventArgs e)
        {
            buttonSnd1.PerformClick();
            buttonSnd2.PerformClick();
            buttonSnd3.PerformClick();
        }

        private void button4_Click(object sender, EventArgs e)
        {

        }
    }
}
