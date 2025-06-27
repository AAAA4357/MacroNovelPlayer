using System;
using MNP.Core.Enum;
using MNP.Helper;

namespace MNP.Core.DataStruct
{
    public class EasingLerp
    {
        public EasingLerp(EasingType easing, LerpType lerp, EasingLerpDelegate customEasing = null)
        {
            switch (easing)
            {
                case EasingType.Sudden:
                    easeLerp = EasingLerpHelper.SuddenEase;
                    break;
                case EasingType.MiddleSudden:
                    easeLerp = EasingLerpHelper.MiddleSuddenEase;
                    break;
                case EasingType.LateSudden:
                    easeLerp = EasingLerpHelper.LateSuddenEase;
                    break;
                case EasingType.Linear:
                    easeLerp = EasingLerpHelper.LinearEase;
                    break;
                case EasingType.Quadratic:
                    switch (lerp)
                    {
                        case LerpType.InLerp:
                            easeLerp = EasingLerpHelper.InQuadraticEase;
                            break;
                        case LerpType.OutLerp:
                            easeLerp = EasingLerpHelper.OutQuadraticEase;
                            break;
                        case LerpType.InOutLerp:
                            easeLerp = EasingLerpHelper.InOutQuadraticEase;
                            break;
                    }
                    break;
                case EasingType.Cubic:
                    switch (lerp)
                    {
                        case LerpType.InLerp:
                            easeLerp = EasingLerpHelper.InCubicEase;
                            break;
                        case LerpType.OutLerp:
                            easeLerp = EasingLerpHelper.OutCubicEase;
                            break;
                        case LerpType.InOutLerp:
                            easeLerp = EasingLerpHelper.InOutCubicEase;
                            break;
                    }
                    break;
                case EasingType.Quartic:
                    switch (lerp)
                    {
                        case LerpType.InLerp:
                            easeLerp = EasingLerpHelper.InQuarticEase;
                            break;
                        case LerpType.OutLerp:
                            easeLerp = EasingLerpHelper.OutQuarticEase;
                            break;
                        case LerpType.InOutLerp:
                            easeLerp = EasingLerpHelper.InOutQuarticEase;
                            break;
                    }
                    break;
                case EasingType.Quintic:
                    switch (lerp)
                    {
                        case LerpType.InLerp:
                            easeLerp = EasingLerpHelper.InQuinticEase;
                            break;
                        case LerpType.OutLerp:
                            easeLerp = EasingLerpHelper.OutQuinticEase;
                            break;
                        case LerpType.InOutLerp:
                            easeLerp = EasingLerpHelper.InOutQuinticEase;
                            break;
                    }
                    break;
                case EasingType.Sine:
                    switch (lerp)
                    {
                        case LerpType.InLerp:
                            easeLerp = EasingLerpHelper.InSineEase;
                            break;
                        case LerpType.OutLerp:
                            easeLerp = EasingLerpHelper.OutSineEase;
                            break;
                        case LerpType.InOutLerp:
                            easeLerp = EasingLerpHelper.InOutSineEase;
                            break;
                    }
                    break;
                case EasingType.Exponential:
                    switch (lerp)
                    {
                        case LerpType.InLerp:
                            easeLerp = EasingLerpHelper.InExponentialEase;
                            break;
                        case LerpType.OutLerp:
                            easeLerp = EasingLerpHelper.OutExponentialEase;
                            break;
                        case LerpType.InOutLerp:
                            easeLerp = EasingLerpHelper.InOutExponentialEase;
                            break;
                    }
                    break;
                case EasingType.NaturalExponential:
                    switch (lerp)
                    {
                        case LerpType.InLerp:
                            easeLerp = EasingLerpHelper.InNaturalExponentialEase;
                            break;
                        case LerpType.OutLerp:
                            easeLerp = EasingLerpHelper.OutNaturalExponentialEase;
                            break;
                        case LerpType.InOutLerp:
                            easeLerp = EasingLerpHelper.InOutNaturalExponentialEase;
                            break;
                    }
                    break;
                case EasingType.Circular:
                    switch (lerp)
                    {
                        case LerpType.InLerp:
                            easeLerp = EasingLerpHelper.InCircularEase;
                            break;
                        case LerpType.OutLerp:
                            easeLerp = EasingLerpHelper.OutCircularEase;
                            break;
                        case LerpType.InOutLerp:
                            easeLerp = EasingLerpHelper.InOutCircularEase;
                            break;
                    }
                    break;
                case EasingType.Elastic:
                    switch (lerp)
                    {
                        case LerpType.InLerp:
                            easeLerp = EasingLerpHelper.InElasticEase;
                            break;
                        case LerpType.OutLerp:
                            easeLerp = EasingLerpHelper.OutElasticEase;
                            break;
                        case LerpType.InOutLerp:
                            easeLerp = EasingLerpHelper.InOutElasticEase;
                            break;
                    }
                    break;
                case EasingType.Back:
                    switch (lerp)
                    {
                        case LerpType.InLerp:
                            easeLerp = EasingLerpHelper.InBackEase;
                            break;
                        case LerpType.OutLerp:
                            easeLerp = EasingLerpHelper.OutBackEase;
                            break;
                        case LerpType.InOutLerp:
                            easeLerp = EasingLerpHelper.InOutBackEase;
                            break;
                    }
                    break;
                case EasingType.Bounce:
                    switch (lerp)
                    {
                        case LerpType.InLerp:
                            easeLerp = EasingLerpHelper.InBounceEase;
                            break;
                        case LerpType.OutLerp:
                            easeLerp = EasingLerpHelper.OutBounceEase;
                            break;
                        case LerpType.InOutLerp:
                            easeLerp = EasingLerpHelper.InOutBounceEase;
                            break;
                    }
                    break;
                case EasingType.Custom:
                    if (customEasing is null)
                        throw new ArgumentNullException("创建自定义缓动委托不可为空");
                    easeLerp = customEasing;
                    break;
            }
        }

        public delegate float EasingLerpDelegate(float t);
        private EasingLerpDelegate easeLerp;

        public float Ease(float t) => easeLerp(t);
    }
}
