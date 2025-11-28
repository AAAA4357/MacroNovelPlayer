using System.Collections.Generic;
using System.Linq;
using MNP.Core.DataStruct;
using MNP.Core.DOTS.Components;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(PostprocessingSystem))]
    partial class OutputSystem : SystemBase
    {
        public Texture2D TestTexture;
        public List<Texture2D> Textures;
        public Material Material;
        public Mesh Mesh2D;
        public Mesh Mesh3D;

        protected override void OnCreate()
        {
            RequireForUpdate<ElementComponent>();
            RequireForUpdate<BakeReadyComponent>();
        }

        protected override void OnUpdate()
        {
            EntityQuery query = GetEntityQuery(typeof(ElementComponent));
            NativeArray<ElementComponent> elements = query.ToComponentDataArray<ElementComponent>(Allocator.Temp);
            Matrix4x4[] matrices = elements.Where(x => x.ObjectType == ObjectType.Object2D).Select(x => (Matrix4x4)x.TransformMatrix).ToArray();
            Graphics.DrawMeshInstanced(Mesh2D,
                                       0,
                                       Material,
                                       matrices,
                                       matrices.Length);
            matrices = elements.Where(x => x.ObjectType == ObjectType.Object3D).Select(x => (Matrix4x4)x.TransformMatrix).ToArray();
            Graphics.DrawMeshInstanced(Mesh3D,
                                       0,
                                       Material,
                                       matrices,
                                       matrices.Length);
            elements.Dispose();
            /*
            MaterialPropertyBlock propertyBlock = new();
            foreach (var elementComponent in SystemAPI.Query<RefRO<ElementComponent>>())
            {
                switch (elementComponent.ValueRO.ObjectType)
                {
                    case ObjectType.Object2D:
                        if (!elementComponent.ValueRO.IsBlocked)
                        {
                            propertyBlock.SetTexture("_MainTex", Textures[elementComponent.ValueRO.TextureID]);
                            Graphics.DrawMesh(
                                Mesh2D,
                                elementComponent.ValueRO.TransformMatrix,
                                Material,
                                0,                               // Layer
                                null,                            // Camera (null = 主相机)
                                0,                               // Submesh index
                                propertyBlock                   // 实例特有的属性
                            );
                            propertyBlock.Clear();
                        }
                        break;
                    case ObjectType.Object3D:
                        if (!elementComponent.ValueRO.IsBlocked)
                        {
                            propertyBlock.SetTexture("_MainTex", Textures[elementComponent.ValueRO.TextureID]);
                            Graphics.DrawMesh(
                                Mesh3D,
                                elementComponent.ValueRO.TransformMatrix,
                                Material,
                                0,                               // Layer
                                null,                            // Camera (null = 主相机)
                                0,                               // Submesh index
                                propertyBlock                   // 实例特有的属性
                            );
                            propertyBlock.Clear();
                        }
                        break;
                    case ObjectType.Text:
                        break;
                }
            }
            */
        }
    }
}
