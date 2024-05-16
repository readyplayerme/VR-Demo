using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.XR
{
    public class LODSampleController : MonoBehaviour
    {
        private const float BYTES_IN_MB = 1024f * 1024f;
        private const float GAP_SIZE = 1.2f;

        [SerializeField] private List<LODSampleGroup> group;

        [SerializeField] private GameObject lodUI;

        private void Start()
        {
            for (var i = 0; i < group.Count; i++)
            {
                var x = GetX(i);
                var sample = group[i];

                var instantiatedModel = Instantiate(sample.Model, transform);
                instantiatedModel.transform.localPosition = new Vector3(x, 0, 0);

                var lodSample = Instantiate(lodUI, transform).GetComponent<LODSample>();
                lodSample.transform.localPosition = new Vector3(x, lodSample.transform.localPosition.y, 0);

                lodSample.Init(instantiatedModel, sample);
            }
        }

        private float GetX(int i)
        {
            var max = GAP_SIZE * (group.Count - 1);
            return i * GAP_SIZE - max / 2;
        }
#if UNITY_EDITOR
        private void OnValidate()
        {
            foreach (var lodSampleGroup in group)
            {
                lodSampleGroup.memoryUsage = AssetSize(lodSampleGroup.Model);
            }
        }

        private float AssetSize(GameObject glbFile)
        {
            if (!glbFile)
            {
                return 0;
            }

            var assetPath = AssetDatabase.GetAssetPath(glbFile);
            var fullPath = Path.Combine(Application.dataPath, assetPath.Replace("Assets/", ""));

            if (File.Exists(fullPath))
            {
                var sizeInBytes = new FileInfo(fullPath).Length;
                var sizeInMb = sizeInBytes / BYTES_IN_MB;

                return sizeInMb;
            }

            return 0;
        }

#endif
    }

    [Serializable]
    public class LODSampleGroup
    {
        public string LODGroup;
        public int atlasSize;
        public GameObject Model;
        public float memoryUsage;
    }
}