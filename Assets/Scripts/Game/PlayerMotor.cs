using Noted.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Noted.Game
{
    /// <summary>
    /// First-person ground movement on a CharacterController, moved in Update so the camera and body share one loop.
    /// Reads the Player/Move and Player/Sprint actions from the shared actions asset without editing its bindings.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class PlayerMotor : MonoBehaviour
    {
        [SerializeField] InputActionAsset actions;

        [Header("Speed (m/s)")]
        [SerializeField] float walkSpeed = 4f;
        [SerializeField] float sprintSpeed = 6.5f;

        [Header("Feel (seconds to reach full speed / to stop). Guesses to tune by hand.")]
        [SerializeField] float accelerationTime = 0.08f;
        [SerializeField] float decelerationTime = 0.06f;

        [Header("Gravity")]
        [SerializeField] float gravity = -20f;

        [Header("Comfort")]
        [Tooltip("When on, the player sprints unless the sprint input is held.")]
        [SerializeField] bool alwaysSprint;

        CharacterController controller;
        InputAction move;
        InputAction sprint;
        Vector3 planarVelocity;
        float verticalVelocity;

        public bool AlwaysSprint { get => alwaysSprint; set => alwaysSprint = value; }
        public Vector3 PlanarVelocity => planarVelocity;

        void Awake()
        {
            controller = GetComponent<CharacterController>();
        }

        void OnEnable()
        {
            var map = actions.FindActionMap("Player", true);
            move = map.FindAction("Move", true);
            sprint = map.FindAction("Sprint", true);
            map.Enable();
        }

        void OnDisable()
        {
            planarVelocity = Vector3.zero;
            verticalVelocity = 0f;
        }

        void Update()
        {
            float dt = Time.deltaTime;

            Vector2 input = Vector2.ClampMagnitude(move.ReadValue<Vector2>(), 1f);
            bool sprinting = sprint.IsPressed() != alwaysSprint;
            float speed = sprinting ? sprintSpeed : walkSpeed;

            // Desired velocity in world space from the body's yaw.
            Vector3 wish = (transform.right * input.x + transform.forward * input.y) * speed;

            bool hasInput = input.sqrMagnitude > 0.0001f;
            float rate = FirstPersonMath.RateFor(speed, hasInput ? accelerationTime : decelerationTime);
            var next = FirstPersonMath.ApproachPlanar(planarVelocity.x, planarVelocity.z, wish.x, wish.z, rate * dt);
            planarVelocity = new Vector3(next.x, 0f, next.z);

            if (controller.isGrounded && verticalVelocity < 0f)
            {
                verticalVelocity = -2f;
            }
            verticalVelocity += gravity * dt;

            Vector3 motion = planarVelocity + Vector3.up * verticalVelocity;
            controller.Move(motion * dt);
        }
    }
}
