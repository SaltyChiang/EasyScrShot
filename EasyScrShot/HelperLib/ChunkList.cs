using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EasyScrShot.HelperLib
{
    class ChunkList
    {
        private int Length;

        public ChunkList(byte[] file)
        {

        }
    }

    class Chunk
    {
        private uint Lenght;
        private string Type;
        private byte[] Data;
        private byte[] CRC;
    }
}
