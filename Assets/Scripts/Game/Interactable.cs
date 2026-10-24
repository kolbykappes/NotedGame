using UnityEngine;

namespace Noted.Game
{
    /// <summary>Something the player can hold E on. Subclasses say whether it is usable now and what finishing it does.</summary>
    public abstract class Interactable : MonoBehaviour
    {
        /// <summary>False when not usable right now. A hold of zero fires on the key press.</summary>
        public abstract bool TryGetPrompt(out string prompt, out float holdSeconds);

        public abstract void Complete();
    }
}
