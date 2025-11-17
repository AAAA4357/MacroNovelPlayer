using System.Collections.Generic;
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
        public Mesh Mesh;

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
                if (!elementComponent.IsBlocked)
                {
                    propertyBlock.SetTexture("_MainTex", Textures[elementComponent.TextureID]);
                    Graphics.DrawMesh(
                        Mesh,
                        elementComponent.TransformMatrix,
                        Material,
                        0,                               // Layer
                        null,                            // Camera (null = 主相机)
                        0,                               // Submesh index
                        propertyBlock                   // 实例特有的属性
                    );
                    propertyBlock.Clear();
                }
            }).WithoutBurst().Run();
        }
        
        protected override void OnDestroy()
        {
            
        }
    }
}
