namespace MNP.Core.DataStruct.Unsafe
{
    public unsafe struct UnsafeArrayStruct<T> where T : unmanaged
    {
        public T** Ptr;
        public int* Index;
        public int* Length;
    }
}
