using Unity.Mathematics;

namespace andywiecko.PBD2D.Core
{
    public readonly struct Line
    {
        public readonly float2 Point;
        public readonly float2 Normal;

        public Line(float2 point, float2 normal)
        {
            Point = point;
            Normal = normal;
        }

        public void Deconstruct(out float2 point, out float2 normal)
        {
            point = Point;
            normal = Normal;
        }
    }
}