using UnityEngine;

namespace CursedCookbook.CameraSystem
{
    /// <summary>
    /// Moves this transform to the point the kitchen camera should frame.
    /// Use it as the Tracking Target of a CinemachineCamera
    /// </summary>
    public class KitchenCameraFocus : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [Tooltip("Centre of the room the camera frames.")]
        [SerializeField] private Transform roomAnchor;

        [Tooltip("0 = locked on the room centre, 1 = fully centred on the player.")]
        [Range(0f, 1f)] [SerializeField] private float followWeight = 0.15f;
        [Tooltip("How far (XZ) the focus may drift from the room anchor.")]
        [SerializeField] private Vector2 maxOffset = new(1.5f, 1f);

        private Vector3 Anchor => roomAnchor != null ? roomAnchor.position : Vector3.zero;

        private void OnEnable() => transform.position = DesiredFocus();

        // Runs before CinemachineBrain
        private void LateUpdate() => transform.position = DesiredFocus();

        private Vector3 DesiredFocus()
        {
            Vector3 a = Anchor;
            if (target == null) return a;
            Vector3 focus = Vector3.Lerp(a, target.position, followWeight);
            focus.x = Mathf.Clamp(focus.x, a.x - maxOffset.x, a.x + maxOffset.x);
            focus.y = a.y;
            focus.z = Mathf.Clamp(focus.z, a.z - maxOffset.y, a.z + maxOffset.y);
            return focus;
        }
    }
}
