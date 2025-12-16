using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;
using MNP.Core.DataStruct;
using MNP.Core.DOTS.Systems;
using Newtonsoft.Json;
using TMPro;
using Unity.Entities;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace MNP.Core
{
    public class SceneLoader
    {
        public Slider ProgressBar;
        public TextMeshProUGUI Hint;
        public GameObject TextInstance;

        public async UniTask<bool> LoadProject(string projectPath)
        {
            Hint.text = "加载主文件...";
            string json = File.ReadAllText(projectPath + "\\main.json");
            MNProject project = JsonConvert.DeserializeObject<MNProject>(json);
            Debug.Log($"加载开始，项目名称{project.Name}");
            float time = Time.time;
            IProgress<float> progress = new Progress<float>(UpdateBar);
            SceneBaker baker = new()
            {
                TextInstance = TextInstance
            };
            Hint.text = "加载资源...";
            List<Texture2D> textures = new();
            List<Mesh> meshes = new();
            Queue<string> textureQueue = new();
            Queue<string> meshQueue = new();
            while (textureQueue.Count > 0)
            {
                string path = textureQueue.Dequeue();
                string finalPath = projectPath + "\\" + path;
                if (!File.Exists(finalPath))
                {
                    Debug.LogWarning($"项目缺少资源文件{path}，请检查后重试");
                    return false;
                }
                string url = "file://" + finalPath;
                using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
                await request.SendWebRequest().ToUniTask(progress);
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"项目缺少资源文件{path}加载失败，请检查后重试");
                    return false;
                }
                Texture2D texture = DownloadHandlerTexture.GetContent(request);
                textures.Add(texture);
                await UniTask.Yield();
            }
            while (meshQueue.Count > 0)
            {
                string path = meshQueue.Dequeue();
                string finalPath = projectPath + "\\" + path;
                if (!File.Exists(finalPath))
                {
                    Debug.LogWarning($"项目缺少资源文件{path}，请检查后重试");
                    return false;
                }
                string url = "file://" + finalPath;
                using UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
                await request.SendWebRequest().ToUniTask(progress);
                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogWarning($"项目缺少资源文件{path}加载失败，请检查后重试");
                    return false;
                }
                string content = request.downloadHandler.text;
                //TODO:obj->Mesh
                await UniTask.Yield();
            }
            OutputSystem system = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<OutputSystem>();
            system.Textures = textures;
            system.Mesh3Ds = meshes;
            Debug.Log($"项目{project.Name}资源加载完毕，共{system.Textures.Count}张贴图，{system.Mesh3Ds.Count}份模型网格");
            Hint.text = "加载元素...";
            await baker.BakeElements(project.Objects, progress);
            float totalTime = Time.time - time;
            Debug.Log($"加载完成，共加载了{project.Objects.Count}个物体，共{project.TotalPropertyCount + project.TotalStringCount}个动画属性，共耗时{totalTime}s");
            Hint.text = "加载完毕";
            return true;
        }

        void UpdateBar(float progress)
        {
            ProgressBar.value = progress;
        }
    }
}