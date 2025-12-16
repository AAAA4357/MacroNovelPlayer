using System;
using Cysharp.Threading.Tasks;
using MNP.Core;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Systems;
using Unity.Entities;
using UnityEngine;

public class PlayManager : MonoBehaviour
{
    public async UniTask PlayAnim(SceneLoader loader, string path, PlayingSceneManager manager, Action callback)
    {
        manager.LoadingCanvas.EnterCanvas();
        bool success = await loader.LoadProject(path);
        manager.LoadingCanvas.ExitCanvas();
        if (!success)
        {
            manager.LoadCanvas.EnterCanvas();
            return;
        }
        await UniTask.Delay(1);
        await UniTask.RunOnThreadPool(() =>
        {
            SystemHandle handle = World.DefaultGameObjectInjectionWorld.Unmanaged.GetExistingUnmanagedSystem<TimeSystem>();
            ref TimeSystem system = ref World.DefaultGameObjectInjectionWorld.Unmanaged.GetUnsafeSystemRef<TimeSystem>(handle);
            system.StartTime();
        });
        await ClearupObjects(callback);
    }

    async UniTask ClearupObjects(Action callback)
    {
        await UniTask.WaitUntil(() =>
        {
            SystemHandle handle = World.DefaultGameObjectInjectionWorld.Unmanaged.GetExistingUnmanagedSystem<TimeSystem>();
            ref TimeSystem system = ref World.DefaultGameObjectInjectionWorld.Unmanaged.GetUnsafeSystemRef<TimeSystem>(handle);
            return system.Ended;
        });
        EntityManager manager = World.DefaultGameObjectInjectionWorld.EntityManager;
        EntityQuery query = manager.CreateEntityQuery(typeof(CleanComponent));
        manager.DestroyEntity(query);
        callback.Invoke();
    }
}
