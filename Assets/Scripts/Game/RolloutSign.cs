using UnityEngine;
using UnityEngine.UI;

namespace Noted.Game
{
    /// <summary>The wall sign in the administration room. Before the reveal it is a slogan; afterwards it shows the company's rollout.</summary>
    public class RolloutSign : MonoBehaviour
    {
        [SerializeField] Text label;

        /// <param name="stage">The reveal stage (0 clean to 3 harm in numbers).</param>
        public void Set(int stage, float rollout, float reliability)
        {
            if (label == null) return;
            label.text = stage <= 0
                ? "WITHIN TOLERANCE"
                : "ROLLOUT " + Mathf.RoundToInt(rollout) + "%\nRELIABILITY " + Mathf.RoundToInt(reliability) + "%";
        }
    }
}
