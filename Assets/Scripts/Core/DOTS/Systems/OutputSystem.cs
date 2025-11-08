using System.Collections.Generic;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.Managed;
using MNP.Helpers;
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
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((ManagedAnimationPropertyListComponent managedAnimationPropertyListComponent,
                              in ElementComponent elementComponent) =>
            {
                Vector2 position = managedAnimationPropertyListComponent.Property2DList[UtilityHelper.TransormPositionID].Value;
                float rotation = managedAnimationPropertyListComponent.Property1DList[UtilityHelper.TransormRotationID].Value;
                Vector2 scale = managedAnimationPropertyListComponent.Property2DList[UtilityHelper.TransormScaleID].Value;
                Matrix4x4 matrix = Matrix4x4.TRS(position,
                                                 Quaternion.Euler(0, 0, rotation),
                                                 scale);
                MaterialPropertyBlock propertyBlock = new();
                propertyBlock.SetTexture("_MainTex", TestTexture);//Textures[elementComponent.TextureID]
                Graphics.DrawMesh(
                    Mesh,
                    matrix,
                    Material,
                    0,                               // Layer
                    null,                            // Camera (null = 主相机)
                    0,                               // Submesh index
                    propertyBlock                   // 实例特有的属性
                );
            }).WithoutBurst().Run();
        }
        
        protected override void OnDestroy()
        {
            
        }
    }
}
