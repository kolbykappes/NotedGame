using System;
using System.Collections;
using Noted.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Noted.Game
{
    /// <summary>
    /// The pull. Hard cut to a third-person camera, the player's avatar walks to the console and the code types itself in,
    /// then a hard cut back. Short, skippable, and never a swinging camera move.
    /// </summary>
    public class PullSequence : MonoBehaviour
    {
        [SerializeField] Camera pullCamera;
        [SerializeField] Transform avatar;
        [SerializeField] Renderer lanyardRenderer;
        [SerializeField] Material lanyardGrey;
        [SerializeField] Material lanyardBlue;
        [SerializeField] Transform avatarStart;
        [SerializeField] Transform avatarEnd;
        [SerializeField] UnityEngine.UI.Text consoleScreen;
        [SerializeField] float walkSeconds = 1.4f;
        [SerializeField] float secondsPerDigit = 0.28f;
        [SerializeField] float holdSeconds = 0.5f;

        bool skipRequested;
        Coroutine running;

        public bool IsPlaying => running != null;
        public float TotalSeconds(int digits) => walkSeconds + digits * secondsPerDigit + holdSeconds;

        public void Play(string code, int rank, Action finished)
        {
            if (running != null) return;
            running = StartCoroutine(Run(code, rank, finished));
        }

        IEnumerator Run(string code, int rank, Action finished)
        {
            skipRequested = false;
            if (lanyardRenderer != null) lanyardRenderer.sharedMaterial = rank >= 1 ? lanyardBlue : lanyardGrey;
            consoleScreen.text = "ENTER CODE\n> ";
            avatar.gameObject.SetActive(true);
            avatar.SetPositionAndRotation(avatarStart.position, avatarStart.rotation);
            pullCamera.enabled = true;

            float t = 0f;
            while (t < walkSeconds && !skipRequested)
            {
                t += Time.deltaTime;
                float k = Mathf.Clamp01(t / walkSeconds);
                avatar.position = Vector3.Lerp(avatarStart.position, avatarEnd.position, k);
                yield return null;
            }

            avatar.SetPositionAndRotation(avatarEnd.position, avatarEnd.rotation);
            string shown = "";
            for (int i = 0; i < code.Length && !skipRequested; i++)
            {
                shown += code[i] + " ";
                consoleScreen.text = "ENTER CODE\n> " + shown;
                float d = 0f;
                while (d < secondsPerDigit && !skipRequested)
                {
                    d += Time.deltaTime;
                    yield return null;
                }
            }

            consoleScreen.text = "ENTER CODE\n> " + CodeGenerator.Spaced(code) + "\nACCEPTED";
            float h = 0f;
            while (h < holdSeconds && !skipRequested)
            {
                h += Time.deltaTime;
                yield return null;
            }

            pullCamera.enabled = false;
            avatar.gameObject.SetActive(false);
            running = null;
            finished?.Invoke();
        }

        void Update()
        {
            if (running == null) return;
            var k = Keyboard.current;
            var m = Mouse.current;
            if ((k != null && (k.spaceKey.wasPressedThisFrame || k.escapeKey.wasPressedThisFrame || k.enterKey.wasPressedThisFrame))
                || (m != null && m.leftButton.wasPressedThisFrame))
            {
                skipRequested = true;
            }
        }

        /// <summary>Puts everything back to the resting state (used when the scene starts).</summary>
        public void ResetState()
        {
            if (running != null) StopCoroutine(running);
            running = null;
            if (pullCamera != null) pullCamera.enabled = false;
            if (avatar != null) avatar.gameObject.SetActive(false);
        }
    }
}
