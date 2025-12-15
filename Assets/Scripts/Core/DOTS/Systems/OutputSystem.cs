using System.Collections.Generic;
using MNP.Core.DOTS.Components;
using TMPro;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(PostprocessingSystem))]
    public partial class OutputSystem : SystemBase
    {
        public Texture2D TestTexture;
        public List<Texture2D> Textures;
        public List<Mesh> Mesh3Ds;
        public Material Material;
        public Mesh Mesh2D;

        public List<Matrix4x4> Matrix4x42Ds;
        public List<Vector4> UVs;
        public List<TextMeshPro> TextList;
        
        MaterialPropertyBlock PropertyBlock = new();

        protected override void OnCreate()
        {
            RequireForUpdate<ElementComponent>();
            RequireForUpdate<BakeReadyComponent>();
        }

        protected override void OnUpdate()
        {
            EntityManager.GetAllUniqueSharedComponents(out NativeList<TextureComponent> textures, Allocator.TempJob);

            Matrix4x42Ds.Clear();
            UVs = new();
            foreach (TextureComponent component in textures)
            {
                PropertyBlock.Clear();
                PropertyBlock.SetTexture("_MainTex", Textures[component.TextureID]);
                foreach ((var element, var uv) in SystemAPI.Query<RefRO<ElementComponent>, RefRO<UVComponent>>()
                                                 .WithSharedComponentFilter(component)
                                                 .WithAll<Object2DComponent>())
                {
                    Matrix4x42Ds.Add(element.ValueRO.TransformMatrix);
                    UVs.Add(uv.ValueRO.UV);
                }
                PropertyBlock.SetVectorArray("_UVRect", UVs);
                Graphics.DrawMeshInstanced(Mesh2D,
                                           0,
                                           Material,
                                           Matrix4x42Ds.ToArray(),
                                           Matrix4x42Ds.Count,
                                           PropertyBlock);
            }
            
            foreach (var element in SystemAPI.Query<RefRO<ElementComponent>>()
                                             .WithAll<Object3DComponent>())
            {
                PropertyBlock.Clear();
                PropertyBlock.SetTexture("_MainTex", Textures[element.ValueRO.Object3DTextureID]);
                Graphics.DrawMesh(Mesh3Ds[element.ValueRO.Object3DMeshID],
                                  element.ValueRO.TransformMatrix,
                                  Material,
                                  0,                               // Layer
                                  null,                            // Camera (null = 主相机)
                                  0,                               // Submesh index
                                  PropertyBlock);
            }
            
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
    }
}
