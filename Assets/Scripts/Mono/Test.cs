using System.Collections.Generic;
using MNP.Core.DataStruct;
using MNP.Core.DataStruct.Animation;
using MNP.Core.DOTS.Systems;
using MNP.Helpers;
using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

namespace MNP.Mono
{
    public class Test : MonoBehaviour
    {
        public Texture2D Texture;
        public Material Material;
        public Mesh Mesh2D;
        public Mesh Mesh3D;
        public GameObject TextInstance;

        public Slider Changed;

        public bool EnableDependency;

        [Range(0, 10000)]
        public int testCount2D = 20;
        [Range(0, 2000)]
        public int testCount3D = 20;
        [Range(0, 200)]
        public int testCountText2D = 2;
        [Range(0, 100)]
        public int testCountText3D = 2;

        uint sourceID;

        void Start()
        {
            OutputSystem system = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<OutputSystem>();
            system.TestTexture = Texture;
            system.Material = Material;
            system.Mesh2D = Mesh2D;
            system.Mesh3D = Mesh3D;

            Changed.onValueChanged.AddListener(OnProgressChanged);
        }

        public MNProject TestProject
        {
            get => new()
            {
                Name = "测试工程",
                Description = "",
                CreateTime = System.DateTime.Now,
                Objects = objects
            };
        }

        List<MNObject> objects
        {
            get
            {
                List<MNObject> objects = new();
                if (EnableDependency)
                {
                    objects.Add(GetEmpty2D());
                }
                for (int i = 0; i < testCount2D; i++)
                {
                    objects.Add(GetInstance2D());
                }
                for (int i = 0; i < testCount3D; i++)
                {
                    objects.Add(GetInstance3D());
                }
                for (int i = 0; i < testCountText2D; i++)
                {
                    objects.Add(GetInstanceText2D());
                }
                for (int i = 0; i < testCountText3D; i++)
                {
                    objects.Add(GetInstanceText3D());
                }
                return objects;
            }
        }

        public void EnableTime()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle systemHandle = world.GetExistingSystem<TimeSystem>();
            ref TimeSystem timeSystem = ref world.Unmanaged.GetUnsafeSystemRef<TimeSystem>(systemHandle);
            timeSystem.StartTime();
        }

        public void DisableTime()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle systemHandle = world.GetExistingSystem<TimeSystem>();
            ref TimeSystem timeSystem = ref world.Unmanaged.GetUnsafeSystemRef<TimeSystem>(systemHandle);
            timeSystem.StopTime();
        }


        public void InterruptPlay()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle systemHandle = world.GetExistingSystem<TimeSystem>();
            ref TimeSystem timeSystem = ref world.Unmanaged.GetUnsafeSystemRef<TimeSystem>(systemHandle);
            timeSystem.InterruptAll();
        }

        public void ResumePlay()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle systemHandle = world.GetExistingSystem<TimeSystem>();
            ref TimeSystem timeSystem = ref world.Unmanaged.GetUnsafeSystemRef<TimeSystem>(systemHandle);
            timeSystem.ResumeAll();
        }

        public void PausePlay()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle systemHandle = world.GetExistingSystem<TimeSystem>();
            ref TimeSystem timeSystem = ref world.Unmanaged.GetUnsafeSystemRef<TimeSystem>(systemHandle);
            timeSystem.PauseTime();
        }

        public void ResumePausePlay()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle systemHandle = world.GetExistingSystem<TimeSystem>();
            ref TimeSystem timeSystem = ref world.Unmanaged.GetUnsafeSystemRef<TimeSystem>(systemHandle);
            timeSystem.ResumeTime();
        }

        public void OnProgressChanged(float value)
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle systemHandle = world.GetExistingSystem<TimeSystem>();
            ref TimeSystem timeSystem = ref world.Unmanaged.GetUnsafeSystemRef<TimeSystem>(systemHandle);
            timeSystem.SystemTime = 8 * value;
        }

        private MNObject GetEmpty2D()
        {
            sourceID =  (uint)new System.Random().Next(int.MinValue, int.MaxValue);
            return new()
            {
                ID = sourceID,
                Type = ObjectType.Empty2D,
                Animations = new()
                {
                    AnimationProperty1DList = new(),
                    Animation1DDictionary = new(),
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
                        }
                    },
                    Animation2DDictionary = new()
                    {
                        {UtilityHelper.Transorm2DPositionID, new()
                        {
                            new Animation2D()
                            {
                                StartValue = new(-3, -3),
                                EndValue = new(3, 3),
                                StartTime = 0,
                                DurationTime = 8,
                                EaseKeyframeList = GenerateLinearEaseList(),
                                LerpType = Float2LerpType.Linear
                            }
                        }}
                    },
                    AnimationProperty3DList = new(),
                    Animation3DDictionary = new(),
                    AnimationProperty4DList = new(),
                    Animation4DDictionary = new(),
                }
            };
        }

        private MNObject GetInstance2D()
        {
            return new()
            {
                ID = (uint)new System.Random().Next(int.MinValue, int.MaxValue),
                Type = ObjectType.Object2D,
                Animations = GenerateAnimation2D()
            };
        }

        private MNObject GetInstance3D()
        {
            return new()
            {
                ID = (uint)new System.Random().Next(int.MinValue, int.MaxValue),
                Type = ObjectType.Object3D,
                Animations = GenerateAnimation3D()
            };
        }

        private MNObject GetInstanceText2D()
        {
            return new()
            {
                ID = (uint)new System.Random().Next(int.MinValue, int.MaxValue),
                Type = ObjectType.Text2D,
                Animations = GenerateAnimationText2D()
            };
        }

        private MNObject GetInstanceText3D()
        {
            return new()
            {
                ID = (uint)new System.Random().Next(int.MinValue, int.MaxValue),
                Type = ObjectType.Text3D,
                Animations = GenerateAnimationText3D()
            };
        }

        private MNAnimation GenerateAnimationText2D()
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
                        AnimationInterruptTimeList = new(),
                        Dependency = EnableDependency ? new()
                        {
                            ObjectID = sourceID,
                            PropertyID = UtilityHelper.Transorm2DPositionID
                        } : null
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
                    }
                },
                Animation2DDictionary = new()
                {
                    {UtilityHelper.Transorm2DPositionID, Generate2DAnimation(true)},
                    {UtilityHelper.Transorm2DScaleID, Generate2DAnimation(false)}
                },
                AnimationProperty3DList = new(),
                Animation3DDictionary = new(),
                AnimationProperty4DList = new(),
                Animation4DDictionary = new(),
                AnimationPropertyStringList = new()
                {
                    new AnimationPropertyString()
                    {
                        ID = UtilityHelper.Text2DID,
                        StartTime = 0,
                        EndTime = 8,
                        IsStatic = false,
                        StaticValue = null,
                        AnimationInterruptTimeList = new()
                    }
                },
                AnimationStringDictionary = new()
                {
                    {UtilityHelper.Text2DID, GenerateTextAnimation()}
                }
            };
        }
        
        private MNAnimation GenerateAnimationText3D()
        {
            return new()
            {
                AnimationProperty1DList = new(),
                Animation1DDictionary = new(),
                AnimationProperty2DList = new(),
                Animation2DDictionary = new(),
                AnimationProperty3DList = new()
                {
                    new AnimationProperty3D()
                    {
                        ID = UtilityHelper.Transorm3DPositionID,
                        StartTime = 0,
                        EndTime = 8,
                        IsStatic = false,
                        StaticValue = null,
                        Type = PropertyType.Transform3DPosition,
                        AnimationInterruptTimeList = new()
                    },
                    new AnimationProperty3D()
                    {
                        ID = UtilityHelper.Transorm3DScaleID,
                        StartTime = 0,
                        EndTime = 8,
                        IsStatic = false,
                        StaticValue = null,
                        Type = PropertyType.Transform3DScale,
                        AnimationInterruptTimeList = new()
                    }
                },
                Animation3DDictionary = new()
                {
                    {UtilityHelper.Transorm3DPositionID, Generate3DAnimation(true)},
                    {UtilityHelper.Transorm3DScaleID, Generate3DAnimation(false)}
                },
                AnimationProperty4DList = new()
                {
                    new AnimationProperty4D()
                    {
                        ID = UtilityHelper.Transorm3DRotationID,
                        StartTime = 0,
                        EndTime = 8,
                        IsStatic = false,
                        StaticValue = null,
                        Type = PropertyType.Transform3DRotation,
                        AnimationInterruptTimeList = new()
                    }
                },
                Animation4DDictionary = new()
                {
                    {UtilityHelper.Transorm3DRotationID, Generate4DAnimation()}
                },
                AnimationPropertyStringList = new()
                {
                    new AnimationPropertyString()
                    {
                        ID = UtilityHelper.Text3DID,
                        StartTime = 0,
                        EndTime = 8,
                        IsStatic = false,
                        StaticValue = null,
                        AnimationInterruptTimeList = new()
                    }
                },
                AnimationStringDictionary = new()
                {
                    {UtilityHelper.Text3DID, GenerateTextAnimation()}
                }
            };
        }

        private MNAnimation GenerateAnimation2D()
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
                        AnimationInterruptTimeList = new(),
                        Dependency = EnableDependency ? new()
                        {
                            ObjectID = sourceID,
                            PropertyID = UtilityHelper.Transorm2DPositionID
                        } : null
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
                    }
                },
                Animation2DDictionary = new()
                {
                    {UtilityHelper.Transorm2DPositionID, Generate2DAnimation(true)},
                    {UtilityHelper.Transorm2DScaleID, Generate2DAnimation(false)}
                },
                AnimationProperty3DList = new(),
                Animation3DDictionary = new(),
                AnimationProperty4DList = new(),
                Animation4DDictionary = new(),
                AnimationPropertyStringList = new(),
                AnimationStringDictionary = new()
            };
        }

        private MNAnimation GenerateAnimation3D()
        {
            return new()
            {
                AnimationProperty1DList = new(),
                Animation1DDictionary = new(),
                AnimationProperty2DList = new(),
                Animation2DDictionary = new(),
                AnimationProperty3DList = new()
                {
                    new AnimationProperty3D()
                    {
                        ID = UtilityHelper.Transorm3DPositionID,
                        StartTime = 0,
                        EndTime = 8,
                        IsStatic = false,
                        StaticValue = null,
                        Type = PropertyType.Transform3DPosition,
                        AnimationInterruptTimeList = new()
                    },
                    new AnimationProperty3D()
                    {
                        ID = UtilityHelper.Transorm3DScaleID,
                        StartTime = 0,
                        EndTime = 8,
                        IsStatic = false,
                        StaticValue = null,
                        Type = PropertyType.Transform3DScale,
                        AnimationInterruptTimeList = new()
                    }
                },
                Animation3DDictionary = new()
                {
                    {UtilityHelper.Transorm3DPositionID, Generate3DAnimation(true)},
                    {UtilityHelper.Transorm3DScaleID, Generate3DAnimation(false)}
                },
                AnimationProperty4DList = new()
                {
                    new AnimationProperty4D()
                    {
                        ID = UtilityHelper.Transorm3DRotationID,
                        StartTime = 0,
                        EndTime = 8,
                        IsStatic = false,
                        StaticValue = null,
                        Type = PropertyType.Transform3DRotation,
                        AnimationInterruptTimeList = new()
                    }
                },
                Animation4DDictionary = new()
                {
                    {UtilityHelper.Transorm3DRotationID, Generate4DAnimation()}
                },
                AnimationPropertyStringList = new(),
                AnimationStringDictionary = new()
            };
        }

        private Vector2 RandomPosition2D
        {
            get => new(RandomFloat * 16f - 8f, RandomFloat * 9f - 4.5f);
        }

        private float RandomRotation2D
        {
            get => Random.value * 360f - 180f;
        }

        private Vector2 RandomScale2D
        {
            get => new(RandomFloat * 2f, RandomFloat * 2f);
        }

        private Vector3 RandomPosition3D
        {
            get => new(RandomFloat * 16f - 8f, RandomFloat * 9f - 4.5f, RandomFloat * 9f - 4.5f);
        }

        private Vector4 RandomRotation3D
        {
            get => ToVector4(Quaternion.Euler(RandomFloat * 360f - 180f, RandomFloat * 360f - 180f, RandomFloat * 360f - 180f));
        }

        private Vector3 RandomScale3D
        {
            get => new(RandomFloat * 2f, RandomFloat * 2f, RandomFloat * 2f);
        }

        private float RandomFloat
        {
            get => Random.value;
        }

        Vector4 ToVector4(Quaternion quaternion) => new(quaternion.x, quaternion.y, quaternion.z, quaternion.w);

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
                        last = RandomRotation2D;
                        result.Add(new()
                        {
                            StartValue = RandomRotation2D,
                            EndValue = last,
                            EaseKeyframeList = GenerateLinearEaseList(),
                            StartTime = i == 0 ? 0 : 4,
                            DurationTime = 2
                        });
                    }
                    else
                    {
                        result.Add(new()
                        {
                            StartValue = last,
                            EndValue = RandomRotation2D,
                            EaseKeyframeList = GenerateLinearEaseList(),
                            StartTime = i == 0 ? 2 : 6,
                            DurationTime = 2
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
                        last = Position ? RandomPosition2D : RandomScale2D;
                        result.Add(new()
                        {
                            StartValue = Position ? RandomPosition2D : RandomScale2D,
                            EndValue = last,
                            Control0Value = Position ? RandomPosition2D : RandomScale2D,
                            Control1Value = Position ? RandomPosition2D : RandomScale2D,
                            EaseKeyframeList = GenerateLinearEaseList(),
                            StartTime = i == 0 ? 0 : 4,
                            DurationTime = 2,
                            LerpType = Float2LerpType.AverageBezier
                        });
                    }
                    else
                    {
                        result.Add(new()
                        {
                            StartValue = last,
                            EndValue = Position ? RandomPosition2D : RandomScale2D,
                            Control0Value = Position ? RandomPosition2D : RandomScale2D,
                            Control1Value = Position ? RandomPosition2D : RandomScale2D,
                            EaseKeyframeList = GenerateLinearEaseList(),
                            StartTime = i == 0 ? 2 : 6,
                            DurationTime = 2,
                            LerpType = Float2LerpType.AverageBezier
                        });
                    }
                }
            }
            return result;
        }
        
        private List<Animation3D> Generate3DAnimation(bool Position)
        {
            List<Animation3D> result = new();
            for (int i = 0; i < 2; i++)
            {
                Vector3 last = Vector3.zero;
                for (int j = 0; j < 2; j++)
                {
                    if (j == 0)
                    {
                        last = Position ? RandomPosition3D : RandomScale3D;
                        result.Add(new()
                        {
                            StartValue = Position ? RandomPosition3D : RandomScale3D,
                            EndValue = last,
                            Control0Value = Position ? RandomPosition3D : RandomScale3D,
                            Control1Value = Position ? RandomPosition3D : RandomScale3D,
                            EaseKeyframeList = GenerateLinearEaseList(),
                            StartTime = i == 0 ? 0 : 4,
                            DurationTime = 2,
                            LerpType = Float3LerpType.AverageBezier
                        });
                    }
                    else
                    {
                        result.Add(new()
                        {
                            StartValue = last,
                            EndValue = Position ? RandomPosition3D : RandomScale3D,
                            Control0Value = Position ? RandomPosition3D : RandomScale3D,
                            Control1Value = Position ? RandomPosition3D : RandomScale3D,
                            EaseKeyframeList = GenerateLinearEaseList(),
                            StartTime = i == 0 ? 2 : 6,
                            DurationTime = 2,
                            LerpType = Float3LerpType.AverageBezier
                        });
                    }
                }
            }
            return result;
        }
        
        private List<Animation4D> Generate4DAnimation()
        {
            List<Animation4D> result = new();
            for (int i = 0; i < 2; i++)
            {
                Vector4 last = Vector4.zero;
                for (int j = 0; j < 2; j++)
                {
                    if (j == 0)
                    {
                        last = RandomRotation3D;
                        result.Add(new()
                        {
                            StartValue = RandomRotation3D,
                            EndValue = last,
                            Control0Value = RandomRotation3D,
                            Control1Value = RandomRotation3D,
                            EaseKeyframeList = GenerateLinearEaseList(),
                            StartTime = i == 0 ? 0 : 4,
                            DurationTime = 2,
                            LerpType = Float4LerpType.AverageSquad
                        });
                    }
                    else
                    {
                        result.Add(new()
                        {
                            StartValue = last,
                            EndValue = RandomRotation3D,
                            Control0Value = RandomRotation3D,
                            Control1Value = RandomRotation3D,
                            EaseKeyframeList = GenerateLinearEaseList(),
                            StartTime = i == 0 ? 2 : 6,
                            DurationTime = 2,
                            LerpType = Float4LerpType.AverageSquad
                        });
                    }
                }
            }
            return result;
        }

        private List<AnimationString> GenerateTextAnimation()
        {
            List<AnimationString> result = new()
            {
                new AnimationString()
                {
                    Value = "测试1",
                    StartTime = 0,
                    DurationTime = 2
                },
                new AnimationString()
                {
                    Value = "测试2",
                    StartTime = 2,
                    DurationTime = 2
                },
                new AnimationString()
                {
                    Value = "测试3",
                    StartTime = 4,
                    DurationTime = 2
                },
                new AnimationString()
                {
                    Value = "测试4",
                    StartTime = 6,
                    DurationTime = 2
                }
            };
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