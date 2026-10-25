using UnityEngine;

namespace Noted.Game
{
    /// <summary>Something the player can hold E on. Subclasses say whether it is usable now and what finishing it does.</summary>
    public abstract class Interactable : MonoBehaviour
    {
        /// <summary>False when not usable right now. A hold of zero fires on the key press.</summary>
        public abstract bool TryGetPrompt(out string prompt, out float holdSeconds);

        public abstract void Complete();

        /// <summary>True when the prompt is shown as information only (for example "hands full") and cannot be completed.</summary>
        public virtual bool IsBlocked => false;

        /// <summary>
        /// The off-book action on the same prop (F), used by the second list. False when there is none right now.
        /// A hold of zero fires on the key press.
        /// </summary>
        public virtual bool TryGetAlternatePrompt(out string prompt, out float holdSeconds)
        {
            prompt = null;
            holdSeconds = 0f;
            return false;
        }

        public virtual void CompleteAlternate()
        {
        }
    }
}
