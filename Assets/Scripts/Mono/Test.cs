using MNP.Core.DataStruct;
using MNP.Core.DataStruct.Animations;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Systems;
using MNP.Core.DOTS.Systems.Managed;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;

namespace MNP.Mono
{
    public class Test : MonoBehaviour
    {
        public GameObject Quad;

        // Start is called before the first frame update
        void Start()
        {
            BezierCurve curve = new()
            {
                Dimension = 2,
                ControlPointP0 = new(new float[] { 0, 0 }, Allocator.Persistent),
                ControlPointP1 = new(new float[] { 0, 0 }, Allocator.Persistent),
                ControlPointP2 = new(new float[] { 2, 0 }, Allocator.Persistent),
                ControlPointP3 = new(new float[] { 2, 0 }, Allocator.Persistent)
            };
            EasingFunction function = new()
            {
                Segments = new(new float4[] { 0, 1, 1, 1 }, Allocator.Persistent)
            };
            AnimationProperty property = new()
            {
                Path = curve,
                Ease = function
            };
            BezierCurve curve1 = new()
            {
                Dimension = 1,
                ControlPointP0 = new(new float[] { 0 }, Allocator.Persistent),
                ControlPointP1 = new(new float[] { 0 }, Allocator.Persistent),
                ControlPointP2 = new(new float[] { 0 }, Allocator.Persistent),
                ControlPointP3 = new(new float[] { 0 }, Allocator.Persistent)
            };
            EasingFunction function1 = new()
            {
                Segments = new(new float4[] { 0, 1, 1, 1 }, Allocator.Persistent)
            };
            AnimationProperty property1 = new()
            {
                Path = curve1,
                Ease = function1
            };
            BezierCurve curve2 = new()
            {
                Dimension = 1,
                ControlPointP0 = new(new float[] { 0 }, Allocator.Persistent),
                ControlPointP1 = new(new float[] { 0 }, Allocator.Persistent),
                ControlPointP2 = new(new float[] { 0 }, Allocator.Persistent),
                ControlPointP3 = new(new float[] { 0 }, Allocator.Persistent)
            };
            EasingFunction function2 = new()
            {
                Segments = new(new float4[] { 0, 1, 1, 1 }, Allocator.Persistent)
            };
            AnimationProperty property2 = new()
            {
                Path = curve2,
                Ease = function2
            };
            Animation2DFrame startFrame = new()
            {
                Time = 0,
                Transform = new()
                {
                    Position = new(0, 0),
                    Rotation = 0,
                    Scale = new(0, 0)
                }
            };
            Animation2DFrame endFrame = new()
            {
                Time = 2,
                Transform = new()
                {
                    Position = new(2, 0),
                    Rotation = 0,
                    Scale = new(0, 0)
                }
            };
            Animation2D animation = new()
            {
                StartFrame = startFrame,
                EndFrame = endFrame,
                PositionProperty = property,
                RotationProperty = property1,
                ScaleProperty = property2
            };
            NativeList<Animation2D> animations = new(1, Allocator.Temp)
            {
                animation
            };
            Animation2DArrayComponent component = new()
            {
                Animations = animations.AsArray(),
                AnimationCount = animations.Length
            };
            World world = World.DefaultGameObjectInjectionWorld;
            EntityManager manager = world.EntityManager;
            const int testCount = 1;
            QuadInstance[] instances = new QuadInstance[testCount];
            for (int i = 0; i < testCount; i++)
            {
                Entity entity = manager.CreateEntity(typeof(ElementComponent),
                                                    typeof(AnimationTransform2DArrayComponent),
                                                    typeof(TimeComponent),
                                                    typeof(TimeEnabledComponent));
                manager.SetComponentData(entity, CreateTest());
                GameObject gameObject = Instantiate(Quad, new Vector3(0, 0, 0), Quaternion.identity);
                QuadInstance instance = gameObject.GetComponent<QuadInstance>();
                instance.manager = manager;
                instance.entity = entity;

                instances[i] = instance;
            }
            ManagedUpdateSystem system = world.GetExistingSystemManaged<ManagedUpdateSystem>();
            system.InstanceList = instances;
            animations.Dispose();
        }

        private AnimationTransform2DArrayComponent CreateTest()
        {
            NativeList<float> AnimationFrameStartArray = new(2, Allocator.Persistent)
            {
                0,
                5
            };
            NativeList<float> AnimationFrameDurationArray = new(2, Allocator.Persistent)
            {
                5,
                2
            };
            NativeList<float2x3> AnimationPathP0Array = new(2, Allocator.Persistent)
            {
                new(0, 0, 1, 0, 0, 1),
                new(2, 0, 1, 0, 0, 1)
            };
            NativeList<float2x3> AnimationPathP1Array = new(2, Allocator.Persistent)
            {
                new(0.666666f, 0, 1, 0, 0, 1),
                new(2, 30, 1, 0.666666f, 0, 1)
            };
            NativeList<float2x3> AnimationPathP2Array = new(2, Allocator.Persistent)
            {
                new(1.333333f, 0, 1, 0, 0, 1),
                new(2, 60, 1, 1.333333f, 0, 1)
            };
            NativeList<float2x3> AnimationPathP3Array = new(2, Allocator.Persistent)
            {
                new(2, 0, 1, 0, 0, 1),
                new(2, 90, 1, 2, 0, 1)
            };
            NativeList<float4> AnimationPositionEaseArray = new(2, Allocator.Persistent)
            {
                new(0, 1, 1, 1),
                new(0, 1, 1, 1)
            };
            NativeList<float4> AnimationRotationEaseArray = new(2, Allocator.Persistent)
            {
                new(0, 1, 1, 1),
                new(0, 1, 1, 1)
            };
            NativeList<float4> AnimationScaleEaseArray = new(2, Allocator.Persistent)
            {
                new(0, 1, 1, 1),
                new(0, 1, 1, 1)
            };
            NativeList<int> PositionEaseSpacingArray = new(2, Allocator.Persistent)
            {
                1,
                1
            };
            NativeList<int> RotationEaseSpacingArray = new(2, Allocator.Persistent)
            {
                1,
                1
            };
            NativeList<int> ScaleEaseSpacingArray = new(2, Allocator.Persistent)
            {
                1,
                1
            };
            AnimationTransform2DArrayComponent component = new()
            {
                AnimationCount = 2,
                AnimationFrameStartArray = AnimationFrameStartArray.ToArray(Allocator.Persistent),
                AnimationFrameDurationArray = AnimationFrameDurationArray.ToArray(Allocator.Persistent),
                AnimationPathP0Array = AnimationPathP0Array.ToArray(Allocator.Persistent),
                AnimationPathP1Array = AnimationPathP1Array.ToArray(Allocator.Persistent),
                AnimationPathP2Array = AnimationPathP2Array.ToArray(Allocator.Persistent),
                AnimationPathP3Array = AnimationPathP3Array.ToArray(Allocator.Persistent),
                AnimationPositionEaseArray = AnimationPositionEaseArray.ToArray(Allocator.Persistent),
                AnimationRotationEaseArray = AnimationRotationEaseArray.ToArray(Allocator.Persistent),
                AnimationScaleEaseArray = AnimationScaleEaseArray.ToArray(Allocator.Persistent),
                PositionEaseSpacingArray = PositionEaseSpacingArray.ToArray(Allocator.Persistent),
                RotationEaseSpacingArray = RotationEaseSpacingArray.ToArray(Allocator.Persistent),
                ScaleEaseSpacingArray = ScaleEaseSpacingArray.ToArray(Allocator.Persistent)
            };
            AnimationFrameStartArray.Dispose();
            AnimationFrameDurationArray.Dispose();
            AnimationPathP0Array.Dispose();
            AnimationPathP1Array.Dispose();
            AnimationPathP2Array.Dispose();
            AnimationPathP3Array.Dispose();
            AnimationPositionEaseArray.Dispose();
            AnimationRotationEaseArray.Dispose();
            AnimationScaleEaseArray.Dispose();
            PositionEaseSpacingArray.Dispose();
            RotationEaseSpacingArray.Dispose();
            ScaleEaseSpacingArray.Dispose();
            return component;
        }

        public void Interrupt()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            var system = world.GetExistingSystem<TimeSystem>();
            ref TimeSystem time = ref world.Unmanaged.GetUnsafeSystemRef<TimeSystem>(system);
            time.Interrupt();
        }
        
        public void Resume()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            var system = world.GetExistingSystem<TimeSystem>();
            ref TimeSystem time = ref world.Unmanaged.GetUnsafeSystemRef<TimeSystem>(system);
            time.Resume();
        }
    }
}
