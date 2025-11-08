using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.Managed;
using Unity.Entities;
using UnityEngine;

namespace MNP.Mono
{
    public class Test : MonoBehaviour
    {
        public GameObject Quad;

        // Start is called before the first frame update
        void Start()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            EntityManager manager = world.EntityManager;
            const int testCount = 1000;
            for (int i = 0; i < testCount; i++)
            {
                Entity entity = manager.CreateEntity(typeof(ElementComponent),
                                                     typeof(TimeComponent),
                                                     typeof(TimeEnabledComponent));
            }
        }
    }
}