using System.Collections.Generic;
using System.Linq;
using MNP.Core.DOTS.Components;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(PostprocessingSystem))]
    public partial class OutputSystem : SystemBase
    {
        public Texture2D TestTexture;
        public List<Texture2D> Textures;
        public Material Material;
        public Mesh Mesh2D;
        public Mesh Mesh3D;

        public NativeList<float4x4> Matrix2DList;
        public NativeList<float4x4> Matrix3DList;
        public List<TextMeshPro> TextList;

        protected override void OnCreate()
        {
            RequireForUpdate<ElementComponent>();
            RequireForUpdate<BakeReadyComponent>();
        }

        protected override void OnUpdate()
        {
            Matrix2DList.Clear();
            Matrix3DList.Clear();
            Dependency = new Output2DJob()
            {
                ListWriter = Matrix2DList.AsParallelWriter()
            }.ScheduleParallel(Dependency);
            Dependency = new Output3DJob()
            {
                ListWriter = Matrix3DList.AsParallelWriter()
            }.ScheduleParallel(Dependency);
            CompleteDependency();
            Matrix4x4[] matrix4X42Ds = Matrix2DList.AsArray().Select(x => (Matrix4x4)x).ToArray();
            Graphics.DrawMeshInstanced(Mesh2D,
                                       0,
                                       Material,
                                       matrix4X42Ds,
                                       matrix4X42Ds.Length);
            Matrix4x4[] matrix4X43Ds = Matrix3DList.AsArray().Select(x => (Matrix4x4)x).ToArray();
            Graphics.DrawMeshInstanced(Mesh3D,
                                       0,
                                       Material,
                                       matrix4X43Ds,
                                       matrix4X43Ds.Length);
            SystemHandle handle = World.Unmanaged.GetExistingUnmanagedSystem<PostprocessingSystem>();
            ref PostprocessingSystem system = ref World.Unmanaged.GetUnsafeSystemRef<PostprocessingSystem>(handle);
            new OutputText2DJob()
            {
                PropertyArray = system.PropertyArray
            }.Run();
            new OutputText3DJob()
            {
                PropertyArray = system.PropertyArray
            }.Run();
        }

        protected override void OnDestroy()
        {
            Matrix2DList.Dispose();
            Matrix3DList.Dispose();
        }
    }
}
