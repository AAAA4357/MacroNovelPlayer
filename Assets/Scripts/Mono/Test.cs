using System.Collections.Generic;
using MNP.Core.DataStruct;
using MNP.Core.DataStruct.Animation;
using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.Managed;
using MNP.Core.DOTS.Systems;
using MNP.Helpers;
using Unity.Entities;
using UnityEngine;

namespace MNP.Mono
{
    public class Test : MonoBehaviour
    {
        public Texture2D Texture;
        public Material Material;
        public Mesh Mesh;

        // Start is called before the first frame update
        void Start()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            EntityManager manager = world.EntityManager;
            OutputSystem system = world.GetExistingSystemManaged<OutputSystem>();
            system.Textures = new()
            {
                Texture
            };
            system.TestTexture = Texture;
            system.Material = Material;
            system.Mesh = Mesh;
            const int testCount = 20;
            for (int i = 0; i < testCount; i++)
            {
                Entity entity = manager.CreateEntity(typeof(ManagedAnimationListComponent),
                                                     typeof(ElementComponent));
                manager.SetComponentData(entity, GetInstance());
                manager.SetComponentData(entity, GenerateAnimation());
            }
        }
        
        public void ResumePlay()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle systemHandle = world.GetExistingSystem<TimeSystem>();
            ref TimeSystem timeSystem = ref world.Unmanaged.GetUnsafeSystemRef<TimeSystem>(systemHandle);
            timeSystem.ResumeAll();
        }

        private ElementComponent GetInstance()
        {
            return new()
            {
                ID = new System.Random().Next(int.MinValue, int.MaxValue),
                TextureID = 0
            };
        }

        private ManagedAnimationListComponent GenerateAnimation()
        {
            return new()
            {
                AnimationProperty1DList = new()
                {
                    new AnimationProperty1D()
                    {
                        ID = UtilityHelper.Transorm2DRotationID,
                        StartTime = 0,
                        EndTime = 8,
                        IsStatic = false,
                        StaticValue = null,
                        Type = PropertyType.Transform2DRotation,
                        AnimationInterruptTimeList = new()
                        {
                            4
                        }
                    }
                },
                Animation1DDictionary = new()
                {
                    {UtilityHelper.Transorm2DRotationID, Generate1DAnimation()}
                },
                AnimationProperty2DList = new()
                {
                    new AnimationProperty2D()
                    {
                        ID = UtilityHelper.Transorm2DPositionID,
                        StartTime = 0,
                        EndTime = 8,
                        IsStatic = false,
                        StaticValue = null,
                        Type = PropertyType.Transform2DPosition,
                        AnimationInterruptTimeList = new()
                        {
                            4
                        }
                    },
                    new AnimationProperty2D()
                    {
                        ID = UtilityHelper.Transorm2DScaleID,
                        StartTime = 0,
                        EndTime = 8,
                        IsStatic = false,
                        StaticValue = null,
                        Type = PropertyType.Transform2DScale,
                        AnimationInterruptTimeList = new()
                        {
                            4
                        }
                    }
                },
                Animation2DDictionary = new()
                {
                    {UtilityHelper.Transorm2DPositionID, Generate2DAnimation(true)},
                    {UtilityHelper.Transorm2DScaleID, Generate2DAnimation(false)}
                },
                AnimationProperty3DList = new(),
                Animation3DDictionary = new()
            };
        }

        private Vector2 RandomPosition
        {
            get => new(RandomFloat * 16f - 8f, RandomFloat * 9f - 4.5f);
        }

        private float RandomRotation
        {
            get => Random.value * 360f - 180f;
        }

        private Vector2 RandomScale
        {
            get => new(RandomFloat * 2f, RandomFloat * 2f);
        }

        private float RandomFloat
        {
            get => Random.value;
        }

        private List<Animation1D> Generate1DAnimation()
        {
            List<Animation1D> result = new();
            for (int i = 0; i < 2; i++)
            {
                float last = 0;
                for (int j = 0; j < 2; j++)
                {
                    if (j == 0)
                    {
                        last = RandomRotation;
                        result.Add(new()
                        {
                            StartValue = RandomRotation,
                            EndValue = last,
                            EaseKeyframeList = GenerateLinearEaseList(),
                            StartTime = i == 0 ? 0 : 4,
                            DurationTime = 2,
                            Enabled = true
                        });
                    }
                    else
                    {
                        result.Add(new()
                        {
                            StartValue = last,
                            EndValue = RandomRotation,
                            EaseKeyframeList = GenerateLinearEaseList(),
                            StartTime = i == 0 ? 2 : 6,
                            DurationTime = 2,
                            Enabled = true
                        });
                    }
                }
            }
            return result;
        }
        
        private List<Animation2D> Generate2DAnimation(bool Position)
        {
            List<Animation2D> result = new();
            for (int i = 0; i < 2; i++)
            {
                Vector2 last = Vector2.zero;
                for (int j = 0; j < 2; j++)
                {
                    if (j == 0)
                    {
                        last = Position ? RandomPosition : RandomScale;
                        result.Add(new()
                        {
                            StartValue = Position ? RandomPosition : RandomScale,
                            EndValue = last,
                            Control0Value = Position ? RandomPosition : RandomScale,
                            Control1Value = Position ? RandomPosition : RandomScale,
                            EaseKeyframeList = GenerateLinearEaseList(),
                            StartTime = i == 0 ? 0 : 4,
                            DurationTime = 2,
                            Linear = false,
                            Enabled = true
                        });
                    }
                    else
                    {
                        result.Add(new()
                        {
                            StartValue = last,
                            EndValue = Position ? RandomPosition : RandomScale,
                            Control0Value = Position ? RandomPosition : RandomScale,
                            Control1Value = Position ? RandomPosition : RandomScale,
                            EaseKeyframeList = GenerateLinearEaseList(),
                            StartTime = i == 0 ? 2 : 6,
                            DurationTime = 2,
                            Linear = false,
                            Enabled = true
                        });
                    }
                }
            }
            return result;
        }
        
        private List<AnimationEaseKeyframe> GenerateLinearEaseList()
        {
            return new()
            {
                new AnimationEaseKeyframe()
                {
                    KeyTime = 0,
                    Value = 0,
                    InTan = float.NaN,
                    OutTan = 1
                },
                new AnimationEaseKeyframe()
                {
                    KeyTime = 1,
                    Value = 1,
                    InTan = 1,
                    OutTan = float.NaN
                }
            };
        }
    }
}