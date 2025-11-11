using Unity.Burst;

namespace MNP.Core.DataStruct.Unsafe
{
    [BurstCompile]
    public unsafe struct UnsafeArray<T> : IChunkData where T : unmanaged
    {
        public T* Ptr;
        public int Index;
        public int Length;
        public int ChunkIndex { get; set; }
        public int EntityIndex { get; set; }
    }
}
