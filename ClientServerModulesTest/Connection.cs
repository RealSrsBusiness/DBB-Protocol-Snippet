using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;

namespace ClientServerModulesTest
{
    public delegate void ConnectionCallback(Connection c);

    public class Connection
    {
        //packet structure
        //ModuleID(6bit),LenghtDataType(2bit)|1-8bytesLenghtOfPacket|Payload

        internal const int CHUNK_SIZE = 512;
        Socket skt;
        Module[] modules;

        public ConnectionCallback connectionLost;

        enum State { FIRSTBYTE, HEADER, PAYLOAD }
        State stage;

        Random rngIn = new Random(100), rngOut = new Random(101);

        byte[] buffer;
        short rcvChunkSize = 0;

        List<JobSnd> sndJobs = new List<JobSnd>();
        List<byte> rcvJobs = new List<byte>();

        int receivingJob = -1;
        int sendingJob = -1;

        object _lock = new object();

        public Connection(Socket endpoint)
        {
            modules = new Module[2];
            for (int i = 0; i < modules.Length; i++)
            {
                Int32 id = i;
                modules[i] = new Module();
                modules[i].Init(delegate (Stream s) { return QueueJob(s, (byte)id); }, (byte)id);
            }

            skt = endpoint;

            stage = State.FIRSTBYTE;
            buffer = new byte[1];
            skt.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, Received, null);
        }

        public Module GetModule()
        {
            return modules[0];
        }

        /// <summary>
        /// DONT UPDATE THIS METHOD IN THIS PROJECT, USE OTHER PROJECT INSTEAD
        /// </summary>
        /// <param name="ar"></param>
        private void Received(IAsyncResult ar)
        {
            rcvChunkSize += (short)skt.EndReceive(ar);
            if (rcvChunkSize == buffer.Length) //we received the entire chunk
            {
                if (stage == State.FIRSTBYTE)
                {
                    byte stByte = buffer[0];
                    byte dataType = (byte)(stByte & 0xC0); //first 2 bits
                    byte trgtModule = (byte)(stByte & 0x3F); //last 6 bits

                    if (trgtModule == 0x0) //0 means there is no new data stream
                    {
                        if (dataType == 0x80) //delete sending job
                        {

                        }
                        else if (dataType == 0x40) //delete receiving job
                        {
                            //rcvJobs.RemoveAt(receivingJob);
                            //inform module, job was canceled
                        }
                        else //sync masks: 00 (0x0) 11 (0xC0)
                        {
                            stage = State.PAYLOAD;
                        }
                    }
                    else //new data stream trgtModule > 0 (1-63)
                    {
                        byte headerLenght = 0;
                        byte[] masks = { 0x00, 1, 0x40, 2, 0x80, 4, 0xC0, 8 };

                        for (int i = 0; i < masks.Length / 2; i++) //buggy
                        {
                            if (dataType == masks[i])
                            {
                                headerLenght = masks[i + 1];
                                break;
                            }
                        }

                        rcvJobs.Add((byte)(trgtModule - 1));
                        if (receivingJob < 0)
                            receivingJob = 0; //first job

                        buffer = new byte[headerLenght];
                        stage = State.HEADER;
                    }
                }
                else if (stage == State.HEADER) //reading the header
                {
                    long lenght = 0;
                    for (int i = 0; i < buffer.Length; i++)
                        lenght |= (long)buffer[i] << (8 * i);

                    Module m = modules[rcvJobs[receivingJob]];

                    // m.maxDataLenght = lenght;

                    int payloadChunk = (int)(lenght > CHUNK_SIZE ? CHUNK_SIZE : lenght);

                    buffer = new byte[payloadChunk];
                    stage = State.PAYLOAD;
                }
                else if (stage == State.PAYLOAD) //parse the payload
                {
                    MemoryStream ms = new MemoryStream(buffer);
                    Module m = modules[rcvJobs[receivingJob]];
                    //m._in = new BinaryReader(ms);
                    //m.BytesAvailable(); //inform module
                    //long byteCount = m.receivedBytes; //inaccessible after the stream has closed, so save it
                    //m._in.Close();

                    //if(byteCount < m.maxDataLenght) //is there still data that needs to be received?
                    //{
                    //    m.completedChunks++; //needs to be increased to calc total number of received bytes
                    //    receivingJob++; //advance to next job for now
                    //}                     
                    //else //no we're done, delete the job
                    //{
                    //    m.completedChunks = 0;
                    //    rcvJobs.RemoveAt(receivingJob);
                    //}

                    if (rcvJobs.Count < 1)
                        receivingJob = -1; //all jobs executed
                    else if (receivingJob >= rcvJobs.Count) //wrap around if necessary
                        receivingJob = 0;

                    buffer = new byte[1];
                    stage = State.FIRSTBYTE;
                }
                rcvChunkSize = 0; //reset
            }

            try
            {
                skt.BeginReceive(buffer, rcvChunkSize,
                    buffer.Length - rcvChunkSize, SocketFlags.None, Received, null);
            }
            catch
            {
                Reset();
            }
        }

        internal JobSnd QueueJob(Stream data, byte originId)
        {
            Debug.WriteLine("job queued");
            JobSnd job = new JobSnd { data = data, moduleId = originId, cxt = new DataStreamContext() };

            long pkLenght = data.Length;
            byte moduleId = (byte)(originId + 1); //module id + 1 to prevent the use of 0, 0 is reserved as sync byte; range 1-63
            moduleId &= 0x3F; //cut away first 2 bits to make room for type; moduleId + 0011 1111

            byte ll = 0; //byte lenght of data type

            //determine fitting data type
            if (pkLenght <= byte.MaxValue)
            {
                moduleId |= 0x00; // 0000 0000
                ll = 1;
            }
            else if (pkLenght <= ushort.MaxValue)
            {
                moduleId |= 0x40; // 0100 0000
                ll = 2;
            }
            else if (pkLenght <= uint.MaxValue)
            {
                moduleId |= 0x80; // 1000 0000
                ll = 4;
            }
            else //if (pkLenght <= long.MaxValue)
            {
                moduleId |= 0xC0; // 1100 0000
                ll = 8;
            }

            //creating header (moduleid, split up moduleidlenght)
            const byte ID_OFFSET = 1; //lenght of moduleid + data type
            byte[] header = new byte[ll + ID_OFFSET];

            header[0] = moduleId;

            for (int i = 0; i < ll; i++) //split "pkLenght" into single bytes
            {
                header[i + ID_OFFSET] = //header[(ll - (i + 1)) + MODULEIDLENGHT] //reversed counting (kinda pointless)
                    (byte)(pkLenght >> (8 * i));
            }
            job.header = header;

            //add the job, need to be locked?
            lock (_lock)
            {
                sndJobs.Add(job);
                if (sendingJob < 0) //no jobs are being executed, so start executing
                {
                    sendingJob++;
                    ProcessJob(job);
                }
            }

            return job;
        }

        //builds a chunk from a job
        private void ProcessJob(JobSnd job) //SendChunk
        {
            bool startOfPacket = job.data.Position < 1;
            if (startOfPacket) Console.WriteLine("new packet");
            long remainingBytes = job.data.Length - job.data.Position;
            int chunkSize = (int)(remainingBytes > CHUNK_SIZE ? CHUNK_SIZE : remainingBytes);
            //send a header(lenght) or zero byte (1), this informs the receiver if there is a new job about to be sent
            int headerSize = (startOfPacket ? job.header.Length : 1); 

            byte[] chunk;

            if (job.cxt.Cancelled) //canceled by user
            {
                chunk = new byte[1];
                chunk[0] = 0x80;
            }
            else
            {
                chunk = new byte[chunkSize + headerSize];
                if (startOfPacket)
                    Array.Copy(job.header, chunk, headerSize); //copy header to chunk
                else
                    chunk[0] = (byte)(rngOut.Next(0, 2) == 0 ? 0x0 : 0xC0); //sync masks: 00 (0x0) 11 (0xC0)

                job.data.Read(chunk, headerSize, chunkSize); //copy part of payload to chunk
                job.cxt.Processed = chunkSize;
                Console.WriteLine($"Number: {sendingJob}, Lenght: {chunk.Length}, First Byte {chunk[0]}");
            }

            skt.BeginSend(chunk, 0, chunk.Length, SocketFlags.None, DataSent, null);
        }

        private void DataSent(IAsyncResult ar)
        {
            lock(_lock)
            {
                skt.EndSend(ar);
                //is the last job done?
                JobSnd lastJob = sndJobs[sendingJob];

                //move on to next job. by just removing a job we get to the next one
                if (lastJob.cxt.Cancelled || lastJob.data.Position >= lastJob.data.Length)
                {
                    lastJob.header = null;
                    lastJob.cxt.Finished = true;
                    sndJobs.RemoveAt(sendingJob);
                }
                else
                    sendingJob++; //if the job is not removed we increase the jobcounter

                //callback module
                
                lastJob.cxt.Position = lastJob.data.Position;
                lastJob.cxt.Length = lastJob.data.Length;
                modules[lastJob.moduleId].OnDataTransferred(lastJob.cxt);

                if (sndJobs.Count > 0) //are there still jobs?
                {
                    if (sendingJob >= sndJobs.Count) //is this even a valid job id?
                        sendingJob = 0; //loop around
                    ProcessJob(sndJobs[sendingJob]);
                }
                else
                    sendingJob = -1; //we're done here boys
            }
        }

        private void Reset()
        {

        }

        //packet structure
        //ModuleID(6bit)|LenghtDataType(2bit)|1-8bytesLenghtOfPacket|Payload
    }
}
