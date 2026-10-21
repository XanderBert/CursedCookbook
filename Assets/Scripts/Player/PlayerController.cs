using UnityEngine;

namespace CursedCookbook.Player
{
    [RequireComponent(typeof(CharacterController), typeof(PlayerInputReader))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float dashSpeed = 8f;
        [SerializeField] private float acceleration = 40f;
        [SerializeField] private float deceleration = 50f;
        [SerializeField] private float turnSpeed = 900f;
        [SerializeField] private float gravity = -25f;

        [Header("Carrying")]
        [Tooltip("0 = no load, 1 = heaviest. Set by the carry system later.")]
        [Range(0f, 1f)] public float carryWeight;
        [SerializeField] private float heavySpeedMultiplier = 0.6f;

        [Header("Bounce (visual only)")]
        [SerializeField] private Transform visual;
        [SerializeField] private float bounceHeight = 0.08f;
        [SerializeField] private float bounceFrequency = 9f;

        [SerializeField] private Transform cameraTransform;

        private CharacterController _cc;
        private PlayerInputReader _input;
        private Vector3 _velocity;
        private float _verticalVelocity;
        private float _bouncePhase;
        private Vector3 _visualBasePos;

        public Vector3 Velocity => _velocity;

        private void Awake()
        {
            _cc = GetComponent<CharacterController>();
            _input = GetComponent<PlayerInputReader>();
            if (cameraTransform == null && UnityEngine.Camera.main != null) cameraTransform = UnityEngine.Camera.main.transform;
            if (visual != null) _visualBasePos = visual.localPosition;
        }

        private void Update()
        {
            Vector3 wishDir = CameraRelative(_input.Move);
            float maxSpeed = (_input.DashHeld ? dashSpeed : moveSpeed) * Mathf.Lerp(1f, heavySpeedMultiplier, carryWeight);
            Vector3 target = wishDir * maxSpeed;
            float rate = wishDir.sqrMagnitude > 0.01f ? acceleration : deceleration;
            _velocity = Vector3.MoveTowards(_velocity, target, rate * Time.deltaTime);

            _verticalVelocity = _cc.isGrounded ? -2f : _verticalVelocity + gravity * Time.deltaTime;
            _cc.Move((_velocity + Vector3.up * _verticalVelocity) * Time.deltaTime);

            if (wishDir.sqrMagnitude > 0.01f)
            {
                var look = Quaternion.LookRotation(wishDir, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, look, turnSpeed * Time.deltaTime);
            }

            UpdateBounce();
        }

        private Vector3 CameraRelative(Vector2 input)
        {
            if (cameraTransform == null) return new Vector3(input.x, 0f, input.y);
            Vector3 fwd = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
            Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
            return Vector3.ClampMagnitude(fwd * input.y + right * input.x, 1f);
        }

        private void UpdateBounce()
        {
            if (visual == null) return;
            float speed01 = _velocity.magnitude / Mathf.Max(moveSpeed, 0.01f);
            _bouncePhase += Time.deltaTime * bounceFrequency * Mathf.Clamp01(speed01);
            float y = speed01 > 0.05f ? Mathf.Abs(Mathf.Sin(_bouncePhase)) * bounceHeight : 0f;
            visual.localPosition = Vector3.Lerp(visual.localPosition, _visualBasePos + Vector3.up * y, 20f * Time.deltaTime);
        }
    }
}
