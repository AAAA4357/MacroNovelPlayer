using System.Collections.Generic;
using MNP.Core.DataStruct;
using MNP.Core.DOTS.Components;
using Unity.Entities;
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
            MaterialPropertyBlock propertyBlock = new();
            Entities.ForEach((in ElementComponent elementComponent) =>
            {
                switch (elementComponent.ObjectType)
                {
                    case ObjectType.Object2D:
                        if (!elementComponent.IsBlocked)
                        {
                            propertyBlock.SetTexture("_MainTex", Textures[elementComponent.TextureID]);
                            Graphics.DrawMesh(
                                Mesh2D,
                                elementComponent.TransformMatrix,
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
                        if (!elementComponent.IsBlocked)
                        {
                            propertyBlock.SetTexture("_MainTex", Textures[elementComponent.TextureID]);
                            Graphics.DrawMesh(
                                Mesh3D,
                                elementComponent.TransformMatrix,
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
            }).WithoutBurst().Run();
        }
        
        protected override void OnDestroy()
        {
            
        }
    }
}
