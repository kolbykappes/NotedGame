using UnityEngine;

namespace Noted.Game
{
    /// <summary>The cot. Usable only in the quiet time; press E to sleep and go to the night shop.</summary>
    public class CotInteractable : Interactable
    {
        [SerializeField] GameFlow flow;

        public override bool TryGetPrompt(out string prompt, out float holdSeconds)
        {
            prompt = "Press E: Sleep";
            holdSeconds = 0f;
            return flow != null && flow.IsQuiet;
        }

        public override void Complete()
        {
            flow.Sleep();
        }
    }
}
