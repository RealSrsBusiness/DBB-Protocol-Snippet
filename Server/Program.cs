using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections;
using System.Threading;
using System.IO;
using System.Windows.Forms;

namespace Server
{
    class Program
    {
        ManualResetEvent pauseEvent = new ManualResetEvent(false);

        [STAThread]
        static void Main(string[] args)
        {
            new Program();
        }
        
        Socket skt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        //Socket udp = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

        byte[] buffer = new byte[512];
        Module m;

        public Program()
        {
            Console.WriteLine("host file started...");
            do
            {
                try
                {
                    Console.Write("trying to connect...");
                    skt.Connect("127.0.0.1", 6225);
                    if (skt.Connected) break;
                }
                catch (Exception)
                {
                    Console.WriteLine("cant connect");
                }
            } while (true);


            Console.WriteLine("connected");
            Connection con = new Connection(skt);
            m = con.GetModule();
            m.DataAvailable = new StreamUpdate(datarc);
            
            while (true)
            {
                string input = Console.ReadLine().ToLower();

                switch(input)
                {
                    case "send":
                        OpenFileDialog ofd = new OpenFileDialog();
                        if(ofd.ShowDialog() == DialogResult.OK && File.Exists(ofd.FileName))
                        {
                            JobSnd j = new JobSnd() { moduleId = 0, data = new FileStream(ofd.FileName, FileMode.Open) };
                            //con.QueueJob(j);
                        }
                        break;
                }
            }
 
        }

        FileStream[] fs = new FileStream[3];

        private void datarc(DataStreamContext cxt)
        {
            if (cxt.IsStart())
            {
                cxt.Identifier = m.NextId();
                fs[cxt.Identifier] = new FileStream(
                    Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\out\\" + cxt.Identifier + ".exe", 
                    FileMode.Create);
            }

            if (cxt.Cancelled)
            {
                Console.WriteLine("job was cancelled");
            }
            else
            {
                byte[] data = m.In.ReadBytes(cxt.Processed);
                fs[cxt.Identifier].Write(data, 0, data.Length);
            }

            if (cxt.Finished)
            {
                fs[cxt.Identifier].Close();
                Console.WriteLine("file written " + cxt.Position + " " + cxt.Length);
            }

               

        }
    }
}
