using System.Collections.Generic;
using MNP.Core.DataStruct;
using MNP.Core.DataStruct.Animation;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.Managed;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

namespace MNP.Core.DOTS.Systems
{
    [UpdateInGroup(typeof(MNPSystemGroup))]
    [UpdateAfter(typeof(PostprocessingSystem))]
    partial class OutputSystem : SystemBase
    {
        public List<Texture2D> Textures;
        public Material Material;

        private Mesh _mesh;
        private MaterialPropertyBlock _propertyBlock;

        protected override void OnCreate()
        {
            // 1. 创建网格（四边形）
            _mesh = CreateQuadMesh();

            // 2. 创建材质并启用GPU实例化
            Material = new Material(Shader.Find("Custom/UnlitInstancedTransparent"));
            Material.enableInstancing = true; // 关键步骤！

            // 3. 初始化属性块
            _propertyBlock = new MaterialPropertyBlock();

            // 4. 设置渲染队列为透明（可选，但推荐）
            Material.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

            RequireForUpdate<ElementComponent>();
        }

        protected override void OnUpdate()
        {
            Entities.ForEach((ManagedAnimationPropertyListComponent managedAnimationPropertyListComponent,
                              in ElementComponent elementComponent) =>
            {
                Vector2 position = managedAnimationPropertyListComponent.Property2DList["2DTransform_Position"].Value;
                float rotation = managedAnimationPropertyListComponent.Property1DList["2DTransform_Rotation"].Value;
                Vector2 scale = managedAnimationPropertyListComponent.Property2DList["2DTransform_Scale"].Value;
                Matrix4x4 matrix = Matrix4x4.TRS(position,
                                                 Quaternion.Euler(0, 0, rotation),
                                                 scale);
                _propertyBlock.SetTexture("Texture", Textures[elementComponent.TextureID]);
                Graphics.DrawMesh(
                    _mesh,
                    matrix,
                    Material,
                    0,                               // Layer
                    null,                            // Camera (null = 主相机)
                    0,                               // Submesh index
                    _propertyBlock                   // 实例特有的属性
                );
            }).WithoutBurst().Run();
        }

        private Mesh CreateQuadMesh()
        {
            var mesh = new Mesh();

            // 顶点
            mesh.vertices = new Vector3[] {
            new Vector3(-0.5f, -0.5f, 0),
            new Vector3(0.5f, -0.5f, 0),
            new Vector3(0.5f, 0.5f, 0),
            new Vector3(-0.5f, 0.5f, 0)
        };

            // 三角形
            mesh.triangles = new int[] {
            0, 1, 2,  // 第一个三角形
            0, 2, 3   // 第二个三角形
        };

            // UV坐标
            mesh.uv = new Vector2[] {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(1, 1),
            new Vector2(0, 1)
        };

            // 法线（朝前）
            mesh.normals = new Vector3[] {
            Vector3.forward,
            Vector3.forward,
            Vector3.forward,
            Vector3.forward
        };

            return mesh;
        }

        protected override void OnDestroy()
        {
            // 清理资源
            if (Material != null)
                Object.Destroy(Material);
            if (_mesh != null)
                Object.Destroy(_mesh);
        }
    }
}
