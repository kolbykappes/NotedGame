using Noted.Core;
using UnityEngine;

namespace Noted.Game
{
    /// <summary>A place where a job step happens: a pick-up, a delivery, a switch, a reading, or a mini-game.</summary>
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
        bool blocked;

        public string StationId => stationId;
        public Vector3 CoinOrigin => coinOrigin != null ? coinOrigin.position : transform.position + Vector3.up * 1.2f;

        public override bool IsBlocked => blocked;

        public override bool TryGetPrompt(out string prompt, out float holdSeconds)
        {
            blocked = false;
            prompt = null;
            holdSeconds = 0f;
            var session = flow != null ? flow.Session : null;
            if (session == null || !flow.IsDay) return false;

            if (session.TryGetAvailableStep(stationId, out _, out var step))
            {
                switch (step.Kind)
                {
                    case StepKind.Hold:
                        holdSeconds = step.HoldSeconds * session.Config.TaskHoldScale * session.Mods.HoldMultiplier;
                        prompt = "Hold E: " + step.Prompt;
                        break;
                    default:
                        // A switch or a mini-game: one press.
                        holdSeconds = 0f;
                        prompt = "Press E: " + step.Prompt;
                        break;
                }
                return true;
            }

            if (session.IsWaitingAt(stationId, out float left))
            {
                blocked = true;
                prompt = "Wait " + Mathf.CeilToInt(left) + " s, then come back to switch it on.";
                return true;
            }

            if (session.IsHandsFullAt(stationId))
            {
                blocked = true;
                prompt = "Hands full. Deliver what you are carrying first.";
                return true;
            }
            return false;
        }

        public override void Complete()
        {
            flow.ActivateStation(this);
        }

        // The second list: an off-book step at this prop, done with F.
        public override bool TryGetAlternatePrompt(out string prompt, out float holdSeconds)
        {
            prompt = null;
            holdSeconds = 0f;
            var session = flow != null ? flow.Session : null;
            if (session == null || !flow.IsDay) return false;
            if (!session.TryGetAlternateStep(stationId, out _, out var step)) return false;

            if (step.Kind == StepKind.Hold)
            {
                holdSeconds = step.HoldSeconds * session.Config.TaskHoldScale * session.Mods.HoldMultiplier;
                prompt = "Hold F: " + step.Prompt;
            }
            else
            {
                prompt = "Press F: " + step.Prompt;
            }
            return true;
        }

        public override void CompleteAlternate()
        {
            flow.ActivateAlternate(this);
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
