using MNP.Core.DataStruct;
using MNP.Core.DataStruct.Unsafe;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;

namespace MNP.Core.DOTS.Jobs.LerpRuntime
{
    [BurstCompile]
    public partial struct ChunkDataSortJob<T> : IJob where T : unmanaged, IChunkData
    {
        public NativeList<T> Data;
    
        [BurstCompile]
        public void Execute()
        {
            NativeSortExtension.Sort(Data.AsArray(), new ChunkDataComparer<T>());
        }
    }
}
