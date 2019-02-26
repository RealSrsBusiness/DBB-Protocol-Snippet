using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using System.Windows.Forms;


namespace ClientServerModulesTest
{
    class Program
    {
        
        [STAThread]
        static void Main(string[] args)
        {


            //MemoryStream ms = new MemoryStream();
            //BinaryWriter bw = new BinaryWriter(ms);
            //BinaryReader br = new BinaryReader(ms);
            //bw.Write("test test");

            //ms.Close();
            //Console.WriteLine(ms.Length);
            //Console.Read();


            //ms.SetLength(ms.Length - 4);
            //ms.Position = 0;
            //Console.WriteLine(br.ReadString());
            //Console.Read();



            //long l = 11195; //72623859790382856L;

            //byte[] lBytes = new byte[2];
            //for (int i = 0; i < lBytes.Length; i++)
            //{
            //    lBytes[i] = (byte)(l >> (8 * i));
            //}

            //long h = 0;
            //for (int i = 0; i < lBytes.Length; i++)
            //{
            //    h |= (long)lBytes[i] << (8 * i);
            //}

            //Console.WriteLine($"{l},\n{h}");
           // Console.Read();

            //Random r = new Random(4201337);
            //Random r2 = new Random(1337420);

            //for (int i = 0; i < 100; i++)
            //{
            //    Console.WriteLine(r.Next(3) + "" + r2.Next(3));
            //}

            //Console.Read();

            Console.WriteLine("Started controller...");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Control());


        }

    }
}
