using System;
using Cysharp.Threading.Tasks;
using MNP.Core.DataStruct;
using MNP.Core.DOTS.Systems;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace MNP.Core
{
    public class SceneLoader
    {
        public Slider Bar;
        public Canvas canvas;
        public GameObject TextInstance;

        public async UniTask LoadProject(MNProject project)
        {
            Debug.Log($"加载开始，项目名称{project.Name}");
            float time = Time.time;
            IProgress<float> progress = new Progress<float>(UpdateBar);
            SceneBaker baker = new()
            {
                TextInstance = TextInstance
            };
            await UniTask.RunOnThreadPool(() =>
            {
                OutputSystem system = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<OutputSystem>();
                system.Textures = project.Resource.Textures;
                system.Mesh3Ds = project.Resource.Object3DMeshs;
                Debug.Log($"项目{project.Name}资源加载完毕，共{system.Textures.Count}张贴图，{system.Mesh3Ds.Count}份模型网格");
            });
            await baker.BakeElements(project.Objects, progress);
            canvas.enabled = false;
            float totalTime = Time.time - time;
            Debug.Log($"加载完成，共加载了{project.Objects.Count}个物体，共{project.TotalPropertyCount + project.TotalStringCount}个动画属性，共耗时{totalTime}s");
        }

        void UpdateBar(float progress)
        {
            Bar.value = progress;
        }
    }
}