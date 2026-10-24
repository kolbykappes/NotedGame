using UnityEngine;
using UnityEngine.InputSystem.UI;

namespace Noted.Game
{
    /// <summary>Gives the UI input module its default actions at runtime, so the mouse can click the cards and shop.</summary>
    [RequireComponent(typeof(InputSystemUIInputModule))]
    public class UiInputBootstrap : MonoBehaviour
    {
        void Awake()
        {
            var module = GetComponent<InputSystemUIInputModule>();
            if (module.actionsAsset == null)
            {
                module.AssignDefaultActions();
            }
        }
    }
}
