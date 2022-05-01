using andywiecko.BurstCollections;
using andywiecko.PBD2D.Core;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Mathematics;

namespace andywiecko.PBD2D.Components
{
    public class PointTriFieldCollisionTuple : ComponentsTuple<
          ITriMeshPointsCollideWithTriMeshTriField, ITriMeshTriFieldCollideWithTriMeshPoints>,
          IPointTriFieldCollisionTuple, IBoundingVolumeTreesIntersectionTuple
    {
        public PointTriFieldCollisionTuple(ITriMeshPointsCollideWithTriMeshTriField item1, ITriMeshTriFieldCollideWithTriMeshPoints item2, World world) : base(item1, item2, world)
        {
        }

        public Ref<NativeList<IdPair<Point, Triangle>>> PotentialCollisions { get; private set; }
        public Ref<NativeList<IdPair<Point, ExternalEdge>>> Collisions { get; private set; }

        public IPointCollideWithTriField PointsComponent => Item1;
        public ITriFieldCollideWithPoint TriFieldComponent => Item2;

        Ref<NativeBoundingVolumeTree<AABB>> IBoundingVolumeTreesIntersectionTuple.Tree1 => Item1.Tree;
        Ref<NativeBoundingVolumeTree<AABB>> IBoundingVolumeTreesIntersectionTuple.Tree2 => Item2.Tree;
        Ref<NativeList<int2>> IBoundingVolumeTreesIntersectionTuple.Result => result;

        public AABB Bounds1 => Item1.Bounds;
        public AABB Bounds2 => Item2.Bounds;

        private Ref<NativeList<int2>> result;

        protected override void Initialize()
        {
            var allocator = Allocator.Persistent;

            DisposeOnDestroy(
                PotentialCollisions = new NativeList<IdPair<Point, Triangle>>(initialCapacity: 256, allocator),
                Collisions = new NativeList<IdPair<Point, ExternalEdge>>(initialCapacity: 256, allocator)
            );

            result = UnsafeUtility.As<NativeList<IdPair<Point, Triangle>>, NativeList<int2>>(ref PotentialCollisions.Value);
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void CheckStructLayouts()
        {
            if (UnsafeUtility.SizeOf<IdPair<Point, Triangle>>() != UnsafeUtility.SizeOf<int2>())
            {
                UnityEngine.Debug.LogError(
                    $"[{nameof(TriMeshCapsulesTriMeshCapsulesCollisionTuple)}]: " +
                    $"{nameof(IdPair<CollidableEdge>)} has different layout than {nameof(int2)}. " +
                    "Buffer after reinterpretion will contain garbage data!"
                );
            }
        }
#endif

        protected override bool InstantiateWhen(ITriMeshPointsCollideWithTriMeshTriField c1, ITriMeshTriFieldCollideWithTriMeshPoints c2) => c1.EntityId != c2.EntityId;

    }
}