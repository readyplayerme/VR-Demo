using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace ReadyPlayerMe.XR
{
    public class LODSample : MonoBehaviour
    {
        private const string VERTICES = "Vertices";
        private const string TRIANGLES = "Triangles";
        private const string FILE_SIZE = "File size";
        private const string TEXTURE_SIZE = "Texture size";
        private const string ATLAS_SIZE = "Atlas size";

        [SerializeField] private TextMeshProUGUI title;
        [SerializeField] private TextMeshProUGUI text;

        private SkinnedMeshRenderer[] meshRenderers;

        public void Init(GameObject model, LODSampleGroup sampleGroup)
        {
            meshRenderers = model.GetComponentsInChildren<SkinnedMeshRenderer>();
            if (meshRenderers.Length == 0)
            {
                Debug.LogError("Model " + model.name + "doesn't have skinnedMeshRenderer");
                return;
            }

            title.text = $"{sampleGroup.LODGroup} LOD";
            var dict = new Dictionary<string, string>
            {
                { VERTICES, VertexCount() },
                { TRIANGLES, TriangleCount() },
                { FILE_SIZE, sampleGroup.memoryUsage.ToString("F2") + "MB" }
            };
            if (meshRenderers.Length == 1)
            {
                dict.Add(TEXTURE_SIZE, meshRenderers[0].material.mainTexture.width.ToString());
            }

            if (sampleGroup.atlasSize != 0)
            {
                dict.Add(ATLAS_SIZE, sampleGroup.atlasSize.ToString());
            }

            text.text = string.Join("\n", dict.Select(kv => $"{kv.Key}: {kv.Value}"));
        }

        private string VertexCount()
        {
            var vertices = 0;
            foreach (var skinnedMeshRenderer in meshRenderers)
            {
                vertices += skinnedMeshRenderer.sharedMesh.vertexCount;
            }

            return vertices.ToString();
        }

        private string TriangleCount()
        {
            var triangles = 0;
            foreach (var skinnedMeshRenderer in meshRenderers)
            {
                triangles += skinnedMeshRenderer.sharedMesh.triangles.Length;
            }

            return triangles.ToString();
        }
    }
}