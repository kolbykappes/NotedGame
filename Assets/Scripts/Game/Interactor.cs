using UnityEngine;
using UnityEngine.InputSystem;

namespace Noted.Game
{
    /// <summary>
    /// Looks at what is in front of the camera and runs the hold-to-complete interaction on it.
    /// Reads the shared Player/Interact action (E on keyboard) without editing its bindings.
    /// </summary>
    public class Interactor : MonoBehaviour
    {
        [SerializeField] InputActionAsset actions;
        [SerializeField] Camera playerCamera;
        [SerializeField] float baseRange = 2.6f;

        InputAction interact;
        Interactable current;
        Collider currentCollider;
        float held;

        /// <summary>Test hook: when true, E counts as held even with no key down (used when keyboard devices are disabled in the background).</summary>
        public bool SimulateInteractHeld { get; set; }

        /// <summary>Test hook: makes the next zero-hold interaction fire as if E had just been pressed.</summary>
        public void SimulatePress() { simulatedPress = true; }

        bool simulatedPress;

        public float RangeBonus { get; set; }
        public bool HasTarget { get; private set; }
        public string Prompt { get; private set; }
        public float Progress01 { get; private set; }

        void OnEnable()
        {
            var map = actions.FindActionMap("Player", true);
            interact = map.FindAction("Interact", true);
            map.Enable();
        }

        void OnDisable()
        {
            Clear();
        }

        void Clear()
        {
            current = null;
            currentCollider = null;
            held = 0f;
            HasTarget = false;
            Prompt = null;
            Progress01 = 0f;
        }

        void Update()
        {
            Interactable target = null;
            var camT = playerCamera.transform;
            if (Physics.Raycast(camT.position, camT.forward, out var hit, baseRange + RangeBonus, ~0, QueryTriggerInteraction.Ignore))
            {
                if (hit.collider != currentCollider)
                {
                    currentCollider = hit.collider;
                    current = hit.collider.GetComponentInParent<Interactable>();
                }
                target = current;
            }
            else
            {
                currentCollider = null;
                current = null;
            }

            if (target == null || !target.TryGetPrompt(out var prompt, out var hold))
            {
                simulatedPress = false;
                HasTarget = false;
                Prompt = null;
                Progress01 = 0f;
                held = 0f;
                return;
            }

            HasTarget = true;
            Prompt = prompt;

            if (target.IsBlocked)
            {
                Progress01 = 0f;
                held = 0f;
                return;
            }

            if (hold <= 0f)
            {
                Progress01 = 0f;
                if (interact.WasPressedThisFrame() || simulatedPress)
                {
                    simulatedPress = false;
                    target.Complete();
                }
                return;
            }

            if (interact.IsPressed() || SimulateInteractHeld)
            {
                held += Time.deltaTime;
                Progress01 = Mathf.Clamp01(held / hold);
                if (held >= hold)
                {
                    held = 0f;
                    Progress01 = 0f;
                    target.Complete();
                }
            }
            else
            {
                held = 0f;
                Progress01 = 0f;
            }
        }
    }
}
