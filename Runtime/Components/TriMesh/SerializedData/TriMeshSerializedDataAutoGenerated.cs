using System.Linq;
using Unity.Mathematics;
using UnityEngine;

namespace andywiecko.PBD2D.Components
{
    [PreferBinarySerialization]
    [CreateAssetMenu(
        fileName = "TriMeshSerializedData (Auto generated)",
        menuName = "PBD2D/TriMesh/TriMeshSerializedData (Auto generated)")]
    public class TriMeshSerializedDataAutoGenerated : TriMeshSerializedData
    {
        [SerializeField]
        private Texture2D texture = default;

        protected override void UpdateUVs()
        {
            UVs = Positions
                 .Select(i => (Vector2)(i / math.float2(texture.width / 100f, texture.height / 100f)))
                 .ToArray();
        }

        private void GetPointsFromCollider()
        {
            if (texture != null)
            {
                var go = new GameObject();
                var renderer = go.AddComponent<SpriteRenderer>();
                var size = new Vector2(texture.width, texture.height);
                renderer.sprite = Sprite.Create(texture, new Rect(Vector2.zero, size), pivot: Vector2.zero);

                var collider = go.AddComponent<PolygonCollider2D>();
                var path = new Path(collider.points.Select(i => (float2)i).ToArray());
                paths = new[] { path };

                DestroyImmediate(go);
            }
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall -= GetPointsFromCollider;
#endif 
        }

        protected override void OnValidate()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.delayCall += GetPointsFromCollider;
#endif

            base.OnValidate();
        }
    }
}
