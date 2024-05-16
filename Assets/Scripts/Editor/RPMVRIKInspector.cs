using ReadyPlayerMe.XR;
using RootMotion.FinalIK;
using UnityEditor;
using UnityEngine;

namespace ReadyPlayerMe.Editor
{
    [CustomEditor(typeof(VRIK))]
    public class RPMVRIKInspector : VRIKInspector
    {
        private const string SETUP_BUTTON_LABEL = "Setup RPM Avatar";
        private VRIK IKComponent => target as VRIK;

        public override void OnInspectorGUI()
        {
            if (GUILayout.Button(SETUP_BUTTON_LABEL))
            {
                VRIKHelper.Setup(IKComponent);
            }

            GUILayout.Space(5);
            base.OnInspectorGUI();
        }
    }
}