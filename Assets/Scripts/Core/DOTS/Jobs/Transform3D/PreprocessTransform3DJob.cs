using MNP.Core.DOTS.Components;
using MNP.Core.DOTS.Components.LerpRuntime;
using MNP.Core.DOTS.Components.LerpRuntime.Transform3D;
using MNP.Core.DOTS.Components.Transform3D;
using Unity.Burst;
using Unity.Entities;

namespace MNP.Core.DOTS.Jobs.Transform3D
{
    [BurstCompile]
    [WithAll(typeof(TimeEnabledComponent))]
    public partial struct PreprocessTransform3DJob : IJobEntity
    {
        public EntityCommandBuffer.ParallelWriter ecbWriter;

        [BurstCompile]
        public void Execute(ref Transform3DPropertyInfoComponent propertyInfoComponent, in PosTransform3DTimeComponent posTimeComponent, in RotTransform3DTimeComponent rotTimeComponent, in SclTransform3DTimeComponent sclTimeComponent, in Entity entity, [ChunkIndexInQuery] int chunkIndexInQuery)
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

        private bool CheckPos(ref Transform3DPropertyInfoComponent propertyInfoComponent, in PosTransform3DTimeComponent timeComponent)
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

        private bool CheckRot(ref Transform3DPropertyInfoComponent propertyInfoComponent, in RotTransform3DTimeComponent timeComponent)
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

        private bool CheckScl(ref Transform3DPropertyInfoComponent propertyInfoComponent, in SclTransform3DTimeComponent timeComponent)
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
