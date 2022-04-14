using andywiecko.PBD2D.Core;

namespace andywiecko.PBD2D.Components
{
    public class TriMeshPointsGroundLineCollisionTuple : ComponentsTuple<
        ITriMeshPointsCollideWithGroundLine, IGroundLineCollideWithTriMeshPoints>,
        IPointLineCollisionTuple
    {
        public TriMeshPointsGroundLineCollisionTuple(ITriMeshPointsCollideWithGroundLine item1, IGroundLineCollideWithTriMeshPoints item2, World world) : base(item1, item2, world)
        {
        }

        public float Friction => (Item1.Friction + Item2.Friction) / 2f;
        public IPointCollideWithPlane PointComponent => Item1;
        public ILineCollideWithPoint LineComponent => Item2;
    }
}