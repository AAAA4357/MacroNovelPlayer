using System.Collections.Generic;
using MNP.Core.DataStruct.Unsafe;

namespace MNP.Core.DataStruct
{
    public struct ChunkDataComparer<T> : IComparer<T> where T : IChunkData
    {
        public int Compare(T x, T y)
        {
            int chunkCompare = x.ChunkIndex.CompareTo(y.ChunkIndex);
            if (chunkCompare != 0)
            {
                return chunkCompare;
            }
            return x.EntityIndex.CompareTo(y.EntityIndex);
        }
    }
}
