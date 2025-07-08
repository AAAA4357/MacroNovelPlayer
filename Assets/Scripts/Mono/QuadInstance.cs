using MNP.Core.DOTS.Components;
using Unity.Entities;
using UnityEngine;

namespace MNP.Mono
{
    [RequireComponent(typeof(MeshFilter))]
    [RequireComponent(typeof(MeshRenderer))]
    public class QuadInstance : MonoBehaviour
    {
        public Mesh DefaultMesh;

        public Material DefaultMaterial;

        public Texture2D Image;

        public Entity entity;

        public EntityManager manager;

        void Start()
        {
            GetComponent<MeshFilter>().mesh = DefaultMesh;
            GetComponent<MeshRenderer>().material = DefaultMaterial;

            GetComponent<MeshRenderer>().material.SetTexture("_MainTex", Image);
        }

        public void UpdateInstance()
        {
            if (entity == null || manager == null)
                return;
            ElementComponent element = manager.GetComponentData<ElementComponent>(entity);
            transform.position = new(element.Transform.Position.x, element.Transform.Position.y);
            transform.rotation = Quaternion.Euler(0, 0, element.Transform.Rotation);
            transform.localScale = new(element.Transform.Scale.x, element.Transform.Scale.y, 1);
        }
    }
}
