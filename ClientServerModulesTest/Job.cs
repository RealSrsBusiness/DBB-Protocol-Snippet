using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClientServerModulesTest
{
    struct JobSnd
    {
        internal byte moduleId;
        internal Stream data;
        internal byte[] header;
        internal DataStreamContext cxt;
    }

    struct JobRcv
    {
        internal byte moduleId;
        internal DataStreamContext cxt;

        internal short NextChunkSize()
        {
            long remainingBytes = cxt.Length - cxt.Position;
            return (short)(remainingBytes > Connection.CHUNK_SIZE ? Connection.CHUNK_SIZE : remainingBytes);
        }
    }

    public class DataStreamContext
    {
        public int Identifier { get; set; }
        public bool Cancelled { get; set; }

        public long Length { get; internal set; }
        public long Position { get; internal set; }
        public int Processed { get; internal set; }
        public bool Finished { get; internal set; }

        public bool IsStart()
        {
            return Position == Connection.CHUNK_SIZE;
        }
    }

}
