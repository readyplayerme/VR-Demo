using System.Collections.Generic;
using ReadyPlayerMe.Core;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_EDITOR
using ReadyPlayerMe.Core.Editor;
using UnityEditor;
#endif

namespace ReadyPlayerMe.XR
{
    public class PerformanceAvatarGenerator : MonoBehaviour
    {
        [SerializeField] private List<GameObject> lowLODAvatars;
        [SerializeField] private List<GameObject> medLODAvatars;
        [SerializeField] private List<GameObject> highLODAvatars;

        [SerializeField] private string[] avatarIds;
        [SerializeField] private int avatarRows = 9;
        [SerializeField] private float distanceBetween = 0.8f;
        [SerializeField] private float lowestFPSAllowed = 60;

        [SerializeField] private UnityEvent<int> avatarCountChanged;
        [SerializeField] private RuntimeAnimatorController animationController;
        [SerializeField] private UnityEvent onCloseError;
        [SerializeField] private UnityEvent onError;

        private readonly Vector3 avatarRotation = new(0, 90, 0);

        private readonly List<GameObject> avatars = new();


        private Lod currentLod;
        private FPSCounter fpsCounterRef;
        private int totalAvatars;

        private FPSCounter fpsCounter
        {
            get
            {
                if (fpsCounterRef == null)
                {
                    fpsCounterRef = FindObjectOfType<FPSCounter>();
                }

                return fpsCounterRef;
            }
        }

        private void Start()
        {
            currentLod = Lod.Low;
        }


        public void SetLODLevel(Lod lod)
        {
            if (lod == currentLod)
            {
                return;
            }

            currentLod = lod;

            var currentAvatarCount = avatars.Count;
            for (var i = 0; i < currentAvatarCount; i++)
            {
                RemoveLastAvatar();
            }

            for (var i = 0; i < currentAvatarCount; i++)
            {
                AddAvatar();
            }
        }

        public void AddAvatar()
        {
            if (fpsCounter && fpsCounter.FPS < lowestFPSAllowed)
            {
                onError?.Invoke();
                return;
            }

            onCloseError?.Invoke();
            totalAvatars += 1;
            avatarCountChanged?.Invoke(totalAvatars);
            AddAvatar(totalAvatars % avatarIds.Length);
        }

        private void AddAvatar(int index)
        {
            var avatar = Instantiate(GetAvatar(index), transform.localToWorldMatrix.MultiplyPoint(GetNewPosition()),
                Quaternion.Euler(avatarRotation), transform);

            avatar.GetComponent<Animator>().runtimeAnimatorController = animationController;
            avatars.Add(avatar);
        }

        private GameObject GetAvatar(int index)
        {
            switch (currentLod)
            {
                case Lod.Low:
                    return lowLODAvatars[index];
                case Lod.Medium:
                    return medLODAvatars[index];
                case Lod.High:
                    return highLODAvatars[index];
                default:
                    return lowLODAvatars[index];
            }
        }

        [ContextMenu("Remove avatar")]
        public void RemoveLastAvatar()
        {
            if (avatars.Count == 0)
            {
                return;
            }

            onCloseError?.Invoke();
            totalAvatars -= 1;

            var avatar = avatars[^1];
            avatars.Remove(avatar);
            Destroy(avatar);
            avatarCountChanged?.Invoke(totalAvatars);
        }

        private Vector3 GetNewPosition()
        {
            var x = avatars.Count % avatarRows;
            var z = (int)((float)avatars.Count / avatarRows);
            return new Vector3(x * distanceBetween, 0, z * distanceBetween);
        }
#if UNITY_EDITOR

        private AvatarObjectLoader m_avatarLoader;

        private AvatarObjectLoader avatarLoader
        {
            get
            {
                if (m_avatarLoader == null)
                {
                    m_avatarLoader = new AvatarObjectLoader();
                    avatarLoader.OnFailed += Failed;
                    avatarLoader.OnCompleted += Completed;
                }

                return m_avatarLoader;
            }
        }

        private readonly List<string> avatarsToDownload = new();

        [ContextMenu("Download avatars")]
        public void DownloadAvatars()
        {
            avatarsToDownload.Clear();
            foreach (var avatarId in avatarIds)
            {
                LoadAvatar(avatarId, false);
            }
        }

        private void LoadAvatar(string avatarId, bool passCheck)
        {
            if (!passCheck)
            {
                avatarsToDownload.Add(avatarId);
            }

            if (avatarsToDownload.Count > 1 && !passCheck)
            {
                return;
            }

            var avatarLoaderSettings = AvatarLoaderSettings.LoadSettings();
            if (avatarLoaderSettings != null)
            {
                avatarLoader.AvatarConfig = avatarLoaderSettings.AvatarConfig;
                if (avatarLoaderSettings.GLTFDeferAgent != null)
                {
                    avatarLoader.GLTFDeferAgent = avatarLoaderSettings.GLTFDeferAgent;
                }
            }

            avatarLoader.LoadAvatar(avatarId);
        }

        private void Failed(object sender, FailureEventArgs args)
        {
            Debug.LogError($"{args.Type} - {args.Message}");
        }

        private void Completed(object sender, CompletionEventArgs args)
        {
            avatarsToDownload.RemoveAt(0);
            if (avatarsToDownload.Count > 0)
            {
                LoadAvatar(avatarsToDownload[0], true);
            }

            var avatarLoaderSettings = AvatarLoaderSettings.LoadSettings();
            var paramHash = AvatarCache.GetAvatarConfigurationHash(avatarLoaderSettings.AvatarConfig);
            var path = $"{DirectoryUtility.GetRelativeProjectPath(args.Avatar.name, paramHash)}/{args.Avatar.name}";
            if (!avatarLoaderSettings.AvatarCachingEnabled)
            {
                SDKLogger.LogWarning("AvatarPerformanceGenerator",
                    "Enable Avatar Caching to generate a prefab in the project folder.");
                return;
            }

            var avatar =
                PrefabHelper.CreateAvatarPrefab(args.Metadata, path, avatarConfig: avatarLoaderSettings.AvatarConfig);

            var avatarSource = AssetDatabase.LoadAssetAtPath(path + ".prefab", typeof(GameObject)) as GameObject;

            switch (avatarLoaderSettings.AvatarConfig.Lod)
            {
                case Lod.Low:
                    lowLODAvatars.Add(avatarSource);
                    break;
                case Lod.Medium:
                    medLODAvatars.Add(avatarSource);
                    break;
                case Lod.High:
                    highLODAvatars.Add(avatarSource);
                    break;
            }

            DestroyImmediate(avatar);
            DestroyImmediate(args.Avatar, true);
        }
#endif
    }
}