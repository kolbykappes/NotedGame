using UnityEngine;

namespace Noted.Game
{
    public enum LightMode
    {
        Day,
        Quiet,
    }

    /// <summary>Fades the office between the lit day and the dim quiet time. Mood lights (the purple window, the terminal) stay on.</summary>
    public class LightingController : MonoBehaviour
    {
        [SerializeField] Light sun;
        [SerializeField] Light[] officeLights;
        [SerializeField] Color dayAmbient = new Color(0.52f, 0.58f, 0.57f);
        [SerializeField] Color quietAmbient = new Color(0.07f, 0.09f, 0.11f);
        [SerializeField] float daySun = 1.1f;
        [SerializeField] float quietSun = 0.05f;
        [SerializeField] float quietOfficeFactor = 0.08f;
        [SerializeField] float fadeSeconds = 1.5f;

        float[] officeBase;
        float blend; // 0 = day, 1 = quiet
        float target;

        void Awake()
        {
            officeBase = new float[officeLights != null ? officeLights.Length : 0];
            for (int i = 0; i < officeBase.Length; i++)
            {
                officeBase[i] = officeLights[i] != null ? officeLights[i].intensity : 0f;
            }
            Apply();
        }

        public void SetMode(LightMode mode, bool instant = false)
        {
            target = mode == LightMode.Quiet ? 1f : 0f;
            if (instant)
            {
                blend = target;
                Apply();
            }
        }

        void Update()
        {
            if (Mathf.Approximately(blend, target)) return;
            blend = Mathf.MoveTowards(blend, target, Time.unscaledDeltaTime / Mathf.Max(0.01f, fadeSeconds));
            Apply();
        }

        void Apply()
        {
            RenderSettings.ambientMode = UnityEngine.Rendering.AmbientMode.Flat;
            RenderSettings.ambientLight = Color.Lerp(dayAmbient, quietAmbient, blend);
            if (sun != null) sun.intensity = Mathf.Lerp(daySun, quietSun, blend);
            for (int i = 0; i < officeBase.Length; i++)
            {
                if (officeLights[i] != null) officeLights[i].intensity = officeBase[i] * Mathf.Lerp(1f, quietOfficeFactor, blend);
            }
        }
    }
}
