using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Diagnostics;

namespace Server
{
    internal delegate void ConnectionState();

    public class Connection
    {
        //packet structure
        //ModuleID(6bit),LenghtDataType(2bit)|1-8bytesLenghtOfPacket|Payload

        internal const int CHUNK_SIZE = 512;
        Socket skt;
        Module[] modules;
 
        internal event ConnectionState connectionLost;

        enum State { FIRSTBYTE, HEADER, PAYLOAD }
        State stage;

        Random rngIn = new Random(101), rngOut = new Random(100);

        byte[] buffer;
        short rcvChunkSize = 0;

        List<JobSnd> sndJobs = new List<JobSnd>();
        List<JobRcv> rcvJobs = new List<JobRcv>();

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

        private void Received(IAsyncResult ar)
        {
            rcvChunkSize += (short)skt.EndReceive(ar);
            bool removeJob = false;

            if (rcvChunkSize == buffer.Length) //we received the entire chunk
            {
                if(stage == State.FIRSTBYTE)
                {
                    byte stByte = buffer[0];
                    byte dataTypeStatus = (byte)(stByte & 0xC0); //first 2 bits, type of 
                    byte trgtModule = (byte)(stByte & 0x3F); //last 6 bits

                    if(trgtModule == 0x0) //0 means there is no new data stream
                    {
                        byte status = dataTypeStatus;
                        if(status == 0x80) //(10) job ended before it could finish (most likely canceled by other endpoint)
                        {
                            removeJob = true;
                            JobRcv j = rcvJobs[receivingJob];
                            j.cxt.Cancelled = true;
                            j.cxt.Processed = 0;
                            modules[j.moduleId].OnDataAvailable(j.cxt);
                            stage = State.FIRSTBYTE;
                        }
                        else if(status == 0x40)//(01) unused
                        {
                            Console.WriteLine("WARNING: Unused bitmask occured. [01]");
                        }
                        else //sync masks: 00 (0x0) 11 (0xC0)
                        {
                            if((rngIn.Next(0, 2) == 0 ? 0x0 : 0xC0) == status)
                            {
                                JobRcv job = rcvJobs[receivingJob];
                                buffer = new byte[job.NextChunkSize()];
                                stage = State.PAYLOAD;
                            }
                            else
                            {
                                Console.WriteLine("Out of sync.");
                            }
                        }
                    }
                    else //new data stream trgtModule > 0 (1-63)
                    {
                        Console.WriteLine("----New data stream: " + stByte +
                            "datatypestatus: " + dataTypeStatus + " trgtmodule" + trgtModule);
                        byte headerLenght = 0;
                        byte dataType = dataTypeStatus;

                        byte[] masks = { 0x00, 1, 0x40, 2, 0x80, 4, 0xC0, 8 };
                        for (int i = 0; i < masks.Length; i = i + 2)
                        {
                            if (dataType == masks[i])
                            {
                                headerLenght = masks[i + 1];
                                break;
                            }
                        }

                        //new job
                        JobRcv job = new JobRcv();
                        job.cxt = new DataStreamContext();
                        job.moduleId = (byte)(trgtModule - 1);

                        rcvJobs.Add(job);
                        receivingJob = rcvJobs.Count - 1; //process newly added job

                        buffer = new byte[headerLenght];
                        stage = State.HEADER;
                    }

                }
                else if(stage == State.HEADER) //reading the header
                {
                    long lenght = 0;
                    for (int i = 0; i < buffer.Length; i++)
                        lenght |= (long)buffer[i] << (8 * i);

                    JobRcv job = rcvJobs[receivingJob];
                    job.cxt.Length = lenght;
                    
                   // Console.WriteLine($"Total lenght: {job.totalDataLenght}, #{receivingJob}");

                    buffer = new byte[job.NextChunkSize()];
                    stage = State.PAYLOAD;
                }

                else if(stage == State.PAYLOAD) //parse the payload
                {
                    JobRcv job = rcvJobs[receivingJob];
                    job.cxt.Processed = job.NextChunkSize();
                    job.cxt.Position += buffer.Length;
                    
                    if (job.cxt.Position >= job.cxt.Length) //is this job done?
                    {
                        removeJob = true;
                        job.cxt.Finished = true;
                    }
                    else//advance job, start from 0 if needed
                        receivingJob++;
 
                    MemoryStream ms = new MemoryStream(buffer);
                    Module m = modules[job.moduleId];
                    m.incomingStream = new BinaryReader(ms);
                    m.OnDataAvailable(job.cxt); //inform module

                    //cleanup
                    m.incomingStream.Close();
                    m.incomingStream = null;

                    //Console.WriteLine($"Payload Size: {buffer.Length}, Data so far: {byteCount}, #{receivingJob}");

                    if (receivingJob >= rcvJobs.Count)
                        receivingJob = 0;

                    buffer = new byte[1];
                    stage = State.FIRSTBYTE;
                }

                rcvChunkSize = 0; //reset
            }

            if(removeJob)
            {
                Console.WriteLine("Job finished or canceled.");
                JobRcv job = rcvJobs[receivingJob];
                
                rcvJobs.RemoveAt(receivingJob);
                if (rcvJobs.Count < 1)
                    receivingJob = -1; //all jobs executed
            }

            try
            {
                Console.WriteLine($"Reading {buffer.Length} bytes");
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

        private void ProcessJob(JobSnd j)
        {
            bool startOfPacket = j.data.Position < 1;
            long remainingBytes = j.data.Length - j.data.Position;
            int chunkSize = (int)(remainingBytes > CHUNK_SIZE ? CHUNK_SIZE : remainingBytes);
            //send a header(lenght) or zero byte (1), this informs the receiver if there is a new job about to be sent
            int headerSize = (startOfPacket ? j.header.Length : 1); 

            byte[] chunk = new byte[chunkSize + headerSize];
            if (startOfPacket)
                Array.Copy(j.header, chunk, headerSize);

            j.data.Read(chunk, headerSize, chunkSize);
            skt.BeginSend(chunk, 0, chunk.Length, SocketFlags.None, DataSent, null);
        }

        private void DataSent(IAsyncResult ar)
        {
            //is the last job done?
            JobSnd lastJob = sndJobs[sendingJob];
            //move on to next job. by just removing a job we get to the next one
            if(lastJob.data.Position >= lastJob.data.Length)
                sndJobs.RemoveAt(sendingJob);
            else
                sendingJob++; //if the job is not removed we increase the jobcounter

            if (sndJobs.Count > 0) //are there still jobs?
            {
                if (sendingJob >= sndJobs.Count) //is this even a valid job id?
                    sendingJob = 0; //loop around
                ProcessJob(sndJobs[sendingJob]);
            }
            else
                sendingJob = -1; //we're done here boys
        }

        private void Reset()
        {
            connectionLost();
        }

        //packet structure
        //ModuleID(6bit)|LenghtDataType(2bit)|1-8bytesLenghtOfPacket|Payload
    }
}
