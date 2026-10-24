using UnityEngine;

namespace Noted.Game
{
    /// <summary>The feedback terminal. Always lit; usable whenever the player is free to walk to it.</summary>
    public class TerminalInteractable : Interactable
    {
        [SerializeField] GameFlow flow;

        public override bool TryGetPrompt(out string prompt, out float holdSeconds)
        {
            prompt = "Press E: File a complaint";
            holdSeconds = 0f;
            return flow != null && flow.Session != null && (flow.IsDay || flow.IsQuiet);
        }

        public override void Complete()
        {
            flow.OpenComplaint();
        }
    }
}
