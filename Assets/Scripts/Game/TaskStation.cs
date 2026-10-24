using UnityEngine;

namespace Noted.Game
{
    /// <summary>A place where a task step happens (the coffee machine, the printer, the vent and so on).</summary>
    public class TaskStation : Interactable
    {
        [SerializeField] string stationId;
        [SerializeField] GameFlow flow;
        [SerializeField] Transform coinOrigin;
        [SerializeField] Renderer highlight;
        [SerializeField] Material normalMaterial;
        [SerializeField] Material activeMaterial;
        [SerializeField] GameObject marker;
        [SerializeField] float markerNearDistance = 7f;

        bool active;

        public string StationId => stationId;
        public Vector3 CoinOrigin => coinOrigin != null ? coinOrigin.position : transform.position + Vector3.up * 1.2f;

        public override bool TryGetPrompt(out string prompt, out float holdSeconds)
        {
            prompt = null;
            holdSeconds = 0f;
            var session = flow != null ? flow.Session : null;
            if (session == null || !flow.IsDay) return false;

            if (session.TryGetAvailableStep(stationId, out _, out var step))
            {
                holdSeconds = step.HoldSeconds * session.Config.TaskHoldScale * session.Mods.HoldMultiplier;
                prompt = "Hold E: " + step.Prompt;
                return true;
            }
            return false;
        }

        public override void Complete()
        {
            flow.CompleteStationStep(this);
        }

        void Update()
        {
            bool nowActive = false;
            var session = flow != null ? flow.Session : null;
            if (session != null && flow.IsDay) nowActive = session.TryGetAvailableStep(stationId, out _, out _);

            if (nowActive != active)
            {
                active = nowActive;
                if (highlight != null && normalMaterial != null && activeMaterial != null)
                {
                    highlight.sharedMaterial = active ? activeMaterial : normalMaterial;
                }
            }

            if (marker != null)
            {
                bool show = active && (session.Mods.MarkerEnabled || NearPlayer());
                if (marker.activeSelf != show) marker.SetActive(show);
            }
        }

        bool NearPlayer()
        {
            var p = flow.PlayerTransform;
            return p != null && (p.position - transform.position).sqrMagnitude < markerNearDistance * markerNearDistance;
        }
    }
}
