using System;
using System.Collections.Generic;
using System.IO;
using System.Text;


namespace Server
{
    internal delegate JobSnd CreateJobCallback(Stream stream);
    public delegate void StreamUpdate(DataStreamContext cxt);

    public class Module
    {
        private CreateJobCallback CreateSendingJob;
        private int id;
        private static int idCounter = 0;

        internal void Init(CreateJobCallback callback, int id)
        {
            CreateSendingJob = callback;
            this.id = id;
        }

        internal BinaryWriter outgoingStream = null;
        internal BinaryReader incomingStream = null;

        public BinaryWriter Out => outgoingStream ?? (outgoingStream = new BinaryWriter(new MemoryStream()));
        public BinaryReader In => incomingStream;

        public StreamUpdate DataTransferred;
        public StreamUpdate DataAvailable;

        public int NextId()
        {
            return idCounter++;
        }

        //Sending
        public virtual void OnDataTransferred(DataStreamContext dsc)
        {
            DataTransferred?.Invoke(dsc);
        }

        public void Transmit(Stream transmit, int id = -1)
        {
            JobSnd res = CreateSendingJob(transmit);
            res.cxt.Identifier = id;
        }

        public void Send(int id = -1)
        {
            Stream bs = outgoingStream.BaseStream;
            if (bs.Length < 1) return;
            if (bs.CanRead && bs.CanSeek)
            {
                JobSnd res = CreateSendingJob(bs);
                res.cxt.Identifier = id;
            }
            else
                throw new IOException("The stream was unexpectedly closed or is not compatible.");
            outgoingStream = null;
        }

        public void Clear()
        {
            if (outgoingStream != null)
                outgoingStream.Close();
            outgoingStream = null;
        }

        //Receiving
        public virtual void OnDataAvailable(DataStreamContext dsc)
        {
            DataAvailable?.Invoke(dsc);
        }
    }

}
