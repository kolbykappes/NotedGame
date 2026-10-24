using UnityEngine;

namespace Noted.Game
{
    /// <summary>The tool locker in the admin room. In the quiet time, press E to browse and buy tools without going to bed.</summary>
    public class LockerInteractable : Interactable
    {
        [SerializeField] GameFlow flow;

        public override bool TryGetPrompt(out string prompt, out float holdSeconds)
        {
            prompt = "Press E: Open the tool locker";
            holdSeconds = 0f;
            return flow != null && flow.IsQuiet;
        }

        public override void Complete()
        {
            flow.OpenLocker();
        }
    }
}
