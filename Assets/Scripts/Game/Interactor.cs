using UnityEngine;
using UnityEngine.InputSystem;

namespace Noted.Game
{
    /// <summary>
    /// Looks at what is in front of the camera and runs the hold-to-complete interaction on it.
    /// Reads the shared Player/Interact action (E on keyboard) without editing its bindings. The off-book action of the second list
    /// is F, read straight from the keyboard so no binding is added.
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
        float heldAlt;

        /// <summary>Test hook: when true, E counts as held even with no key down (used when keyboard devices are disabled in the background).</summary>
        public bool SimulateInteractHeld { get; set; }

        /// <summary>Test hook: when true, F counts as held even with no key down.</summary>
        public bool SimulateAltHeld { get; set; }

        /// <summary>Test hook: makes the next zero-hold interaction fire as if E had just been pressed.</summary>
        public void SimulatePress() { simulatedPress = true; }

        /// <summary>Test hook: makes the next zero-hold off-book action fire as if F had just been pressed.</summary>
        public void SimulateAltPress() { simulatedAltPress = true; }

        bool simulatedPress;
        bool simulatedAltPress;

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
            heldAlt = 0f;
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

            string mainPrompt = null;
            float mainHold = 0f;
            bool hasMain = target != null && target.TryGetPrompt(out mainPrompt, out mainHold);
            string altPrompt = null;
            float altHold = 0f;
            bool hasAlt = target != null && target.TryGetAlternatePrompt(out altPrompt, out altHold);

            if (!hasMain && !hasAlt)
            {
                simulatedPress = false;
                simulatedAltPress = false;
                HasTarget = false;
                Prompt = null;
                Progress01 = 0f;
                held = 0f;
                heldAlt = 0f;
                return;
            }

            HasTarget = true;
            Prompt = hasMain && hasAlt ? mainPrompt + "\n" + altPrompt : (hasMain ? mainPrompt : altPrompt);

            float mainProgress = hasMain ? UpdateMain(target, mainHold) : ResetMain();
            float altProgress = hasAlt ? UpdateAlt(target, altHold) : ResetAlt();
            Progress01 = Mathf.Max(mainProgress, altProgress);
        }

        float ResetMain()
        {
            held = 0f;
            simulatedPress = false;
            return 0f;
        }

        float ResetAlt()
        {
            heldAlt = 0f;
            simulatedAltPress = false;
            return 0f;
        }

        float UpdateMain(Interactable target, float hold)
        {
            if (target.IsBlocked)
            {
                held = 0f;
                return 0f;
            }

            if (hold <= 0f)
            {
                held = 0f;
                if (interact.WasPressedThisFrame() || simulatedPress)
                {
                    simulatedPress = false;
                    target.Complete();
                }
                return 0f;
            }

            if (interact.IsPressed() || SimulateInteractHeld)
            {
                held += Time.deltaTime;
                float progress = Mathf.Clamp01(held / hold);
                if (held >= hold)
                {
                    held = 0f;
                    progress = 0f;
                    target.Complete();
                }
                return progress;
            }

            held = 0f;
            return 0f;
        }

        float UpdateAlt(Interactable target, float hold)
        {
            var keyboard = Keyboard.current;
            bool pressed = (keyboard != null && keyboard.fKey.wasPressedThisFrame) || simulatedAltPress;
            bool down = (keyboard != null && keyboard.fKey.isPressed) || SimulateAltHeld;

            if (hold <= 0f)
            {
                heldAlt = 0f;
                if (pressed)
                {
                    simulatedAltPress = false;
                    target.CompleteAlternate();
                }
                return 0f;
            }

            if (down)
            {
                heldAlt += Time.deltaTime;
                float progress = Mathf.Clamp01(heldAlt / hold);
                if (heldAlt >= hold)
                {
                    heldAlt = 0f;
                    progress = 0f;
                    target.CompleteAlternate();
                }
                return progress;
            }

            heldAlt = 0f;
            return 0f;
        }
    }
}
