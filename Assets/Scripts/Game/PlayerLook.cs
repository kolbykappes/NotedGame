using Noted.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Noted.Game
{
    /// <summary>
    /// Mouse and stick look. Yaw turns the body, pitch tilts the camera. Runs in Update with the same loop as PlayerMotor.
    /// Mouse delta is scaled by a constant (no Time.deltaTime); stick input is a rate, so it is scaled by deltaTime.
    /// </summary>
    public class PlayerLook : MonoBehaviour
    {
        [SerializeField] InputActionAsset actions;
        [SerializeField] Camera playerCamera;

        [Header("Comfort")]
        [Tooltip("Horizontal field of view in degrees. Unity's Camera.fieldOfView is vertical, so this is converted.")]
        [Range(70f, 120f)] [SerializeField] float horizontalFov = 100f;
        [Tooltip("Degrees of turn per mouse count.")]
        [SerializeField] float mouseSensitivity = 0.1f;
        [Tooltip("Degrees per second at full stick deflection.")]
        [SerializeField] float stickDegreesPerSecond = 180f;
        [SerializeField] bool invertY;
        [SerializeField] float pitchLimit = 85f;

        InputAction look;
        float pitch;
        float appliedAspect;
        float appliedFov;

        public float HorizontalFov { get => horizontalFov; set => horizontalFov = value; }
        public float MouseSensitivity { get => mouseSensitivity; set => mouseSensitivity = value; }
        public bool InvertY { get => invertY; set => invertY = value; }

        /// <summary>Levels the camera, for use after a teleport.</summary>
        public void ResetPitch()
        {
            pitch = 0f;
            if (playerCamera != null) playerCamera.transform.localRotation = Quaternion.identity;
        }

        void OnEnable()
        {
            var map = actions.FindActionMap("Player", true);
            look = map.FindAction("Look", true);
            map.Enable();
            pitch = FirstPersonMath.ClampPitch(NormalizePitch(playerCamera.transform.localEulerAngles.x), pitchLimit);
            LockCursor(true);
            appliedAspect = -1f;
        }

        void OnDisable()
        {
            LockCursor(false);
        }

        void Update()
        {
            HandleCursorToggle();
            if (Cursor.lockState == CursorLockMode.Locked)
            {
                ApplyLook();
            }
            ApplyFov();
        }

        void ApplyLook()
        {
            Vector2 value = look.ReadValue<Vector2>();
            bool fromMouse = look.activeControl == null || look.activeControl.device is Mouse;
            Vector2 degrees = fromMouse
                ? value * mouseSensitivity
                : value * (stickDegreesPerSecond * Time.deltaTime);

            transform.Rotate(0f, degrees.x, 0f, Space.Self);
            pitch = FirstPersonMath.ApplyPitch(pitch, degrees.y, invertY, pitchLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
        }

        void ApplyFov()
        {
            float aspect = playerCamera.aspect;
            if (Mathf.Approximately(aspect, appliedAspect) && Mathf.Approximately(horizontalFov, appliedFov))
            {
                return;
            }

            playerCamera.fieldOfView = Camera.HorizontalToVerticalFieldOfView(horizontalFov, aspect);
            appliedAspect = aspect;
            appliedFov = horizontalFov;
        }

        void HandleCursorToggle()
        {
            var keyboard = Keyboard.current;
            var mouse = Mouse.current;
            if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
            {
                LockCursor(false);
            }
            else if (mouse != null && mouse.leftButton.wasPressedThisFrame && Cursor.lockState != CursorLockMode.Locked)
            {
                LockCursor(true);
            }
        }

        static void LockCursor(bool locked)
        {
            Cursor.lockState = locked ? CursorLockMode.Locked : CursorLockMode.None;
            Cursor.visible = !locked;
        }

        static float NormalizePitch(float eulerX)
        {
            return eulerX > 180f ? eulerX - 360f : eulerX;
        }
    }
}
