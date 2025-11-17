using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Components.LerpRuntime.Transform2D;
using MNP.Core.DOTS.Components.Transform2D;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs.Transform2D
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent))]
    public partial struct PreprocessTransform2DJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecbWriter;

        [BurstCompile]
        public void Execute(ref Transform2DPropertyInfoComponent propertyInfoComponent, in PosTransform2DTimeComponent posTimeComponent, in RotTransform2DTimeComponent rotTimeComponent, in SclTransform2DTimeComponent sclTimeComponent, in Entity entity, [ChunkIndexInQuery] int chunkIndexInQuery)
        {
            bool pos = CheckPos(ref propertyInfoComponent, in posTimeComponent);
            bool rot = CheckRot(ref propertyInfoComponent, in rotTimeComponent);
            bool scl = CheckScl(ref propertyInfoComponent, in sclTimeComponent);
            if (pos || rot || scl)
            {
                if (propertyInfoComponent.PositionLerpEnabled ||
                    propertyInfoComponent.RotationLerpEnabled ||
                    propertyInfoComponent.ScaleLerpEnabled)
                {
                    return;
                }
                ecbWriter.AddComponent<LerpEnabledComponent>(chunkIndexInQuery, entity);
            }
            else
            {
                if (!propertyInfoComponent.PositionLerpEnabled &&
                    !propertyInfoComponent.RotationLerpEnabled &&
                    !propertyInfoComponent.ScaleLerpEnabled)
                {
                    return;
                }
                ecbWriter.RemoveComponent<LerpEnabledComponent>(chunkIndexInQuery, entity);
            }
        }

        private bool CheckPos(ref Transform2DPropertyInfoComponent propertyInfoComponent, in PosTransform2DTimeComponent timeComponent)
        {
            if (timeComponent.Time < propertyInfoComponent.PositionStartTime ||
                timeComponent.Time > propertyInfoComponent.PositionEndTime)
            {
                propertyInfoComponent.PositionLerpEnabled = false;
                return false;
            }
            else
            {
                propertyInfoComponent.PositionLerpEnabled = true;
                return true;
            }
        }

        private bool CheckRot(ref Transform2DPropertyInfoComponent propertyInfoComponent, in RotTransform2DTimeComponent timeComponent)
        {
            if (timeComponent.Time < propertyInfoComponent.RotationStartTime ||
                timeComponent.Time > propertyInfoComponent.RotationEndTime)
            {
                propertyInfoComponent.RotationLerpEnabled = false;
                return false;
            }
            else
            {
                propertyInfoComponent.RotationLerpEnabled = true;
                return true;
            }
        }

        private bool CheckScl(ref Transform2DPropertyInfoComponent propertyInfoComponent, in SclTransform2DTimeComponent timeComponent)
        {
            if (timeComponent.Time < propertyInfoComponent.ScaleStartTime ||
                timeComponent.Time > propertyInfoComponent.ScaleEndTime)
            {
                propertyInfoComponent.ScaleLerpEnabled = false;
                return false;
            }
            else
            {
                propertyInfoComponent.ScaleLerpEnabled = true;
                return true;
            }
        }
    }
}
