using UnityEngine;
using UnityEngine.UI;

namespace Noted.Game
{
    /// <summary>
    /// A badge-locked door for one area. Closed: a solid slab, a red light and "ACCESS PENDING". Open: the slab slides up out of the way,
    /// the light turns green and the sign says "ACCESS GRANTED".
    /// </summary>
    public class AreaDoor : MonoBehaviour
    {
        [SerializeField] string areaId;
        [SerializeField] Transform slab;
        [SerializeField] Collider slabCollider;
        [SerializeField] Renderer led;
        [SerializeField] Material ledClosed;
        [SerializeField] Material ledOpen;
        [SerializeField] Text sign;
        [SerializeField] string closedText = "ACCESS PENDING";
        [SerializeField] string openText = "ACCESS GRANTED";
        [SerializeField] float slideHeight = 2.6f;
        [SerializeField] float slideSeconds = 0.7f;

        Vector3 closedPosition;
        bool captured;
        bool open;
        float t; // 0 closed, 1 open

        public string AreaId => areaId;
        public bool IsOpen => open;

        void Capture()
        {
            if (captured) return;
            captured = true;
            closedPosition = slab.localPosition;
        }

        void Awake()
        {
            Capture();
        }

        public void SetOpen(bool shouldOpen, bool instant)
        {
            Capture();
            open = shouldOpen;
            if (slabCollider != null) slabCollider.enabled = !shouldOpen;
            if (led != null) led.sharedMaterial = shouldOpen ? ledOpen : ledClosed;
            if (sign != null) sign.text = shouldOpen ? openText : closedText;
            if (instant)
            {
                t = shouldOpen ? 1f : 0f;
                Apply();
            }
        }

        void Update()
        {
            float target = open ? 1f : 0f;
            if (Mathf.Approximately(t, target)) return;
            t = Mathf.MoveTowards(t, target, Time.deltaTime / Mathf.Max(0.05f, slideSeconds));
            Apply();
        }

        void Apply()
        {
            if (slab == null) return;
            slab.localPosition = closedPosition + Vector3.up * (slideHeight * t);
        }
    }
}
