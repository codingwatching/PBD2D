using andywiecko.BurstCollections;
using andywiecko.PBD2D.Core;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

namespace andywiecko.PBD2D.Systems
{
    [AddComponentMenu("PBD2D:Systems/Extended Data/External Edge Bounding Volume Tree System")]
    public class ExternalEdgeBoundingVolumeTreeSystem : BaseSystem<IExternalEdgeBoundingVolumeTree>
    {
        [BurstCompile]
        private struct UpdateAABBsJob : IJobParallelFor
        {
            private NativeIndexedArray<Id<ExternalEdge>, AABB> aabbs;
            private NativeIndexedArray<Id<ExternalEdge>, ExternalEdge>.ReadOnly externalEdges;
            private NativeIndexedArray<Id<Point>, float2>.ReadOnly positions;
            private readonly float margin;

            public UpdateAABBsJob(IExternalEdgeBoundingVolumeTree component)
            {
                aabbs = component.AABBs.Value;
                externalEdges = component.ExternalEdges.Value.AsReadOnly();
                positions = component.Positions.Value.AsReadOnly();
                margin = component.Margin;
            }

            public JobHandle Schedule(JobHandle dependencies)
            {
                return this.Schedule(aabbs.Length, innerloopBatchCount: 64, dependencies);
            }

            public void Execute(int i)
            {
                var eId = (Id<ExternalEdge>)i;
                aabbs[eId] = externalEdges[eId].ToAABB(positions, margin);
            }
        }

        public override JobHandle Schedule(JobHandle dependencies)
        {
            foreach (var component in References)
            {
                dependencies = new UpdateAABBsJob(component).Schedule(dependencies);
                dependencies = component.Tree.Value.UpdateLeavesVolumes(
                    volumes: component.AABBs.Value.GetInnerArray().AsReadOnly(), dependencies);
            }

            return dependencies;
        }
    }
}
