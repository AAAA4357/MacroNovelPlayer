using System;
using Cysharp.Threading.Tasks;
using MNP.Core.DataStruct;
using MNP.Core.DOTS.Systems;
using MNP.Mono;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace MNP.Core
{
    public class SceneLoader
    {
        public Slider Bar;
        public Test test;
        public Canvas canvas;

        public async UniTask LoadScene()
        {
            MNProject project = test.TestProject;
            Debug.Log($"加载开始，项目名称{project.Name}，预计耗时{project.TotalPropertyCount / (float)260:F2}s");
            float time = Time.time;
            IProgress<float> progress = new Progress<float>(UpdateBar);
            SceneBaker baker = new();
            await UniTask.RunOnThreadPool(() =>
            {
                SystemHandle handle = World.DefaultGameObjectInjectionWorld.Unmanaged.GetExistingUnmanagedSystem<PostprocessingSystem>();
                ref PostprocessingSystem system = ref World.DefaultGameObjectInjectionWorld.Unmanaged.GetUnsafeSystemRef<PostprocessingSystem>(handle);
                system.PropertyArray = new(project.TotalPropertyCount, Allocator.Persistent);
            });
            await baker.BakeElements(project.Objects, progress);
            canvas.enabled = false;
            float totalTime = Time.time - time;
            Debug.Log($"加载完成，共加载了{project.Objects.Count}个物体，共{project.TotalPropertyCount}个动画属性，共耗时{totalTime}s");
        }

        void UpdateBar(float progress)
        {
            Bar.value = progress;
        }
    }
}