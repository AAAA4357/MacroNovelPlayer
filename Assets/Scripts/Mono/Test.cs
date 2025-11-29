using System.Collections.Generic;
using MNP.Core.DataStruct;
using MNP.Core.DataStruct.Animation;
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
        public Mesh Mesh2D;
        public Mesh Mesh3D;

        [Range(0, 10000)]
        public int testCount2D = 20;
        [Range(0, 2000)]
        public int testCount3D = 20;

        public List<AnimationElement> TestElements
        {
            get
            {
                List<AnimationElement> elements = new();
                for (int i = 0; i < testCount2D; i++)
                {
                    elements.Add(GetInstance2D());
                }
                for (int i = 0; i < testCount3D; i++)
                {
                    elements.Add(GetInstance3D());
                }
                return elements;
            }
        }

        public void ResumePlay()
        {
            World world = World.DefaultGameObjectInjectionWorld;
            SystemHandle systemHandle = world.GetExistingSystem<TimeSystem>();
            ref TimeSystem timeSystem = ref world.Unmanaged.GetUnsafeSystemRef<TimeSystem>(systemHandle);
            timeSystem.ResumeAll();
        }

        private AnimationElement GetInstance2D()
        {
            return new()
            {
                ID = (uint)new System.Random().Next(int.MinValue, int.MaxValue),
                TextureID = 0,
                Type = ObjectType.Object2D,
                Animations = GenerateAnimation2D()
            };
        }

        private AnimationElement GetInstance3D()
        {
            return new()
            {
                ID = (uint)new System.Random().Next(int.MinValue, int.MaxValue),
                TextureID = 0,
                Type = ObjectType.Object3D,
                Animations = GenerateAnimation2D()
            };
        }

        private AnimationList GenerateAnimation2D()
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
                        AnimationInterruptTimeList = new()
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
                Animation4DDictionary = new()
            };
        }
        

        private AnimationList GenerateAnimation3D()
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
                }
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
                            DurationTime = 2,
                            Enabled = true
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
                            Enabled = true,
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
                            Enabled = true,
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
                            Enabled = true,
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
                            Enabled = true,
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
                            LerpType = Float4LerpType.Squad,
                            Enabled = true
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
                            LerpType = Float4LerpType.Squad,
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