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
            const int testCount = 10000;
            for (int i = 0; i < testCount; i++)
            {
                Entity entity = manager.CreateEntity(typeof(ManagedAnimationListComponent),
                                                     typeof(ElementComponent));
                manager.SetComponentData(entity, GetInstance());
                manager.SetComponentData(entity, GenerateAnimation());
            }
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
                        Type = PropertyType.Transform2DRotation
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
                        Type = PropertyType.Transform2DPosition
                    },
                    new AnimationProperty2D()
                    {
                        ID = UtilityHelper.Transorm2DScaleID,
                        StartTime = 0,
                        EndTime = 8,
                        IsStatic = false,
                        StaticValue = null,
                        Type = PropertyType.Transform2DScale
                    }
                },
                Animation2DDictionary = new()
                {
                    {UtilityHelper.Transorm2DPositionID, Generate2DAnimation(false)},
                    {UtilityHelper.Transorm2DScaleID, Generate2DAnimation(true)}
                },
                AnimationProperty3DList = new(),
                Animation3DDictionary = new()
            };
        }

        private Vector2 RandomVector2
        {
            get => new(RandomFloat * 4f - 2f, RandomFloat * 2f - 1f);
        }

        private Vector2 RandomVector2NoNegative
        {
            get => new(RandomFloat * 2f, RandomFloat);
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
                        last = RandomFloat * 360;
                        result.Add(new()
                        {
                            StartValue = RandomFloat * 360,
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
                            EndValue = RandomFloat * 360,
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
        
        private List<Animation2D> Generate2DAnimation(bool NoNegative)
        {
            List<Animation2D> result = new();
            for (int i = 0; i < 2; i++)
            {
                Vector2 last = Vector2.zero;
                for (int j = 0; j < 2; j++)
                {
                    if (j == 0)
                    {
                        last = NoNegative ? RandomVector2NoNegative : RandomVector2;
                        result.Add(new()
                        {
                            StartValue = NoNegative ? RandomVector2NoNegative : RandomVector2,
                            EndValue = last,
                            Control0Value = NoNegative ? RandomVector2NoNegative : RandomVector2,
                            Control1Value = NoNegative ? RandomVector2NoNegative : RandomVector2,
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
                            EndValue = NoNegative ? RandomVector2NoNegative : RandomVector2,
                            Control0Value = NoNegative ? RandomVector2NoNegative : RandomVector2,
                            Control1Value = NoNegative ? RandomVector2NoNegative : RandomVector2,
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