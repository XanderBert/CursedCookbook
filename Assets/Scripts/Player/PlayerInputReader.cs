using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CursedCookbook.Player
{
    public class PlayerInputReader : MonoBehaviour
    {
        [SerializeField] private InputActionAsset actions;
        [SerializeField] private string actionMap = "Player";

        public Vector2 Move { get; private set; }
        public bool DashHeld { get; private set; }

        /// <summary>Pick up / put down / use a station.</summary>
        public event Action Interact;
        /// <summary>Chop, stir, whisk, throw — the "work" button.</summary>
        public event Action Use;

        private InputAction _move, _interact, _use, _dash;

        private void Awake()
        {
            var map = actions.FindActionMap(actionMap, throwIfNotFound: true);
            _move = map.FindAction("Move", true);
            _interact = map.FindAction("Interact", true);
            _use = map.FindAction("Attack", true);
            _dash = map.FindAction("Sprint", true);
        }

        private void OnEnable()
        {
            _interact.performed += OnInteract;
            _use.performed += OnUse;
            _move.actionMap.Enable();
        }

        private void OnDisable()
        {
            _interact.performed -= OnInteract;
            _use.performed -= OnUse;
            _move.actionMap.Disable();
        }

        private void Update()
        {
            Move = Vector2.ClampMagnitude(_move.ReadValue<Vector2>(), 1f);
            DashHeld = _dash.IsPressed();
        }

        private void OnInteract(InputAction.CallbackContext _) => Interact?.Invoke();
        private void OnUse(InputAction.CallbackContext _) => Use?.Invoke();
    }
}
