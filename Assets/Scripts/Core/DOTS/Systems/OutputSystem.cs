using System.Collections.Generic;
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
            SystemHandle handle = World.Unmanaged.GetExistingUnmanagedSystem<PostprocessingSystem>();
            ref PostprocessingSystem system = ref World.Unmanaged.GetUnsafeSystemRef<PostprocessingSystem>(handle);
            MaterialPropertyBlock propertyBlock = new();
            foreach (var elementComponent in SystemAPI.Query<RefRO<ElementComponent>>())
            {
                float4 position = system.PropertyArray[elementComponent.ValueRO.TransformPositionIndex];
                float4 rotation = system.PropertyArray[elementComponent.ValueRO.TransformRotationIndex];
                float4 scale = system.PropertyArray[elementComponent.ValueRO.TransformScaleIndex];
                Matrix4x4 matrix;
                switch (elementComponent.ValueRO.ObjectType)
                {
                    case ObjectType.Object2D:
                        if (!elementComponent.ValueRO.IsBlocked)
                        {
                            matrix = Matrix4x4.TRS((Vector2)position.xy, 
                                                   Quaternion.Euler(0, 0, rotation.x), 
                                                   (Vector2)scale.xy);
                            propertyBlock.SetTexture("_MainTex", Textures[elementComponent.ValueRO.TextureID]);
                            Graphics.DrawMesh(
                                Mesh2D,
                                matrix,
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
                            matrix = Matrix4x4.TRS(position.xyz, 
                                                   new Quaternion(rotation.x, rotation.y, rotation.z, rotation.w), 
                                                   scale.xyz);
                            propertyBlock.SetTexture("_MainTex", Textures[elementComponent.ValueRO.TextureID]);
                            Graphics.DrawMesh(
                                Mesh3D,
                                matrix,
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
        }
        
        protected override void OnDestroy()
        {
            
        }
    }
}
