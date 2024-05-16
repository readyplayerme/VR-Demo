using RootMotion.FinalIK;
using UnityEngine;

namespace ReadyPlayerMe.XR
{
    public class HandsController : MonoBehaviour
    {
        [SerializeField] private GameObject target;
        [SerializeField] private VRIK vrik;

        private IKSolver solver;

        private void Awake()
        {
            solver = vrik.GetIKSolver();
        }

        private void OnEnable()
        {
            solver.OnPreUpdate += UpdatePosition;
        }

        private void OnDisable()
        {
            solver.OnPreUpdate -= UpdatePosition;
        }

        private void UpdatePosition()
        {
            target.transform.SetPositionAndRotation(transform.position, transform.rotation);
        }
    }
}