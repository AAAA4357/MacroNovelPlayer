using MNP.Core.DataStruct.Unsafe;

namespace MNP.Core.DataStruct
{
    public struct ChunkData<T> : IChunkData where T : unmanaged
    {
        public T Value;
        public int ChunkIndex { get; set; }
        public int EntityIndex { get; set; }
    }
}
