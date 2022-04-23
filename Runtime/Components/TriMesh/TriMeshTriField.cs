using andywiecko.BurstMathUtils;
using andywiecko.PBD2D.Core;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace andywiecko.PBD2D.Components
{
    public interface ITriField
    {

    }

    [RequireComponent(typeof(TriMesh))]
    [RequireComponent(typeof(TriMeshExternalEdges))]
    [AddComponentMenu("PBD2D:TriMesh.Components/Extended Data/Tri Field")]
    public class TriMeshTriField : BaseComponent
    {
        public Ref<TriFieldLookup> TriFieldLookup { get; private set; }

        [SerializeField, Range(0, 10)]
        private int samples = 4;

        private TriMeshExternalEdges externalEdges;
        private TriMesh triMesh;

        public void Start()
        {
            triMesh = GetComponent<TriMesh>();
            externalEdges = GetComponent<TriMeshExternalEdges>();

            DisposeOnDestroy(
                TriFieldLookup = new TriFieldLookup(trianglesCount: triMesh.Triangles.Value.Length, samples, Allocator.Persistent)
            );

            var dependencies = TriFieldLookup.Value.Initialize(default);
            TriFieldLookup.Value.GenerateMapping(
                triMesh.Positions.Value.AsReadOnly(),
                triMesh.Triangles.Value.AsReadOnly(),
                triMesh.Edges.Value.AsReadOnly(),
                externalEdges.ExternalEdges.Value.AsReadOnly(),
                dependencies
            ).Complete();
        }

        public void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;

            var lookup = TriFieldLookup.Value.AsReadOnly();
            var positions = triMesh.Positions.Value.AsReadOnly();
            var external = externalEdges.ExternalEdges.Value.AsReadOnly();
            var edges = triMesh.Edges.Value.AsReadOnly();

            Gizmos.color = Color.red;
            foreach (var t in triMesh.Triangles.Value.AsReadOnly())
            {
                var (pA, pB, pC) = positions.At3(t);
                foreach (var b in lookup.Barycoords)
                {
                    var p = pA * b.x + pB * b.y + pC * b.z;
                    Gizmos.DrawSphere(p.ToFloat3(), 0.03f);
                }
            }

            Gizmos.color = Color.blue;
            foreach (var (tId, t) in triMesh.Triangles.Value.AsReadOnly().IdsValues)
            {
                var (pA, pB, pC) = positions.At3(t);
                foreach (var b in lookup.Barycoords)
                {
                    var externalId = lookup.GetExternalEdge(tId, b);
                    var edgeId = external[externalId];
                    var (e0, e1) = positions.At2(edges[edgeId]);
                    var p = pA * b.x + pB * b.y + pC * b.z;
                    // TODO: this should be cached, it can be valuable!
                    MathUtils.PointClosestPointOnLineSegment(p, e0, e1, out var q);
                    Gizmos.DrawLine(p.ToFloat3(), q.ToFloat3());
                }
            }
        }
    }
}