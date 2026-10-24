using System.Collections.Generic;
using UnityEngine;

namespace Noted.Game
{
    public enum SfxId
    {
        Coin,
        StepDone,
        TaskDone,
        PullStart,
        PullTick,
        PullAccepted,
        Buy,
        Reply,
        Wake,
        Sleep,
        Flip,
        Wrong,
        Unlock,
    }

    /// <summary>
    /// Placeholder sound effects synthesised in code (no audio files), plus a faint building hum.
    /// All tones are first guesses; replace with real audio later.
    /// </summary>
    public class Sfx : MonoBehaviour
    {
        const int SampleRate = 44100;

        [SerializeField] [Range(0f, 1f)] float volume = 0.5f;
        [SerializeField] [Range(0f, 0.3f)] float humVolume = 0.05f;

        readonly Dictionary<SfxId, AudioClip> clips = new Dictionary<SfxId, AudioClip>();
        AudioSource oneShot;
        AudioSource hum;

        void Awake()
        {
            oneShot = gameObject.AddComponent<AudioSource>();
            oneShot.playOnAwake = false;
            oneShot.spatialBlend = 0f;

            clips[SfxId.Coin] = Tone("coin", 0.09f, new[] { 1046f, 1568f }, 0.55f, 38f);
            clips[SfxId.StepDone] = Tone("step", 0.14f, new[] { 523f, 659f }, 0.5f, 18f);
            clips[SfxId.TaskDone] = Chord("task", 0.5f, new[] { 523f, 659f, 784f }, 0.4f, 7f);
            clips[SfxId.PullStart] = Tone("pull", 0.7f, new[] { 140f, 98f }, 0.7f, 3f);
            clips[SfxId.PullTick] = Tone("tick", 0.04f, new[] { 1400f, 1400f }, 0.35f, 60f);
            clips[SfxId.PullAccepted] = Chord("accepted", 0.55f, new[] { 392f, 523f, 659f }, 0.4f, 6f);
            clips[SfxId.Buy] = Tone("buy", 0.18f, new[] { 660f, 990f }, 0.5f, 14f);
            clips[SfxId.Reply] = Tone("reply", 0.22f, new[] { 520f, 440f }, 0.4f, 9f);
            clips[SfxId.Wake] = Chord("wake", 0.6f, new[] { 330f, 440f, 554f }, 0.35f, 5f);
            clips[SfxId.Sleep] = Tone("sleep", 0.8f, new[] { 330f, 196f }, 0.45f, 3f);
            clips[SfxId.Flip] = Tone("flip", 0.06f, new[] { 300f, 180f }, 0.6f, 45f);
            clips[SfxId.Wrong] = Tone("wrong", 0.22f, new[] { 160f, 110f }, 0.55f, 9f);
            clips[SfxId.Unlock] = Chord("unlock", 0.7f, new[] { 392f, 494f, 587f, 784f }, 0.4f, 5f);

            hum = gameObject.AddComponent<AudioSource>();
            hum.clip = HumClip();
            hum.loop = true;
            hum.volume = humVolume;
            hum.spatialBlend = 0f;
            hum.Play();
        }

        public void Play(SfxId id, float pitch = 1f)
        {
            if (!clips.TryGetValue(id, out var clip)) return;
            oneShot.pitch = pitch;
            oneShot.PlayOneShot(clip, volume);
        }

        // A sine glide from the first to the second frequency with an exponential decay.
        static AudioClip Tone(string name, float seconds, float[] freqs, float amp, float decay)
        {
            int n = Mathf.CeilToInt(seconds * SampleRate);
            var data = new float[n];
            float phase = 0f;
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)SampleRate;
                float k = i / (float)(n - 1);
                float f = Mathf.Lerp(freqs[0], freqs[freqs.Length - 1], k);
                phase += 2f * Mathf.PI * f / SampleRate;
                float env = Mathf.Exp(-decay * t) * Mathf.Min(1f, i / 80f);
                data[i] = Mathf.Sin(phase) * amp * env;
            }
            return MakeClip(name, data);
        }

        // Notes struck one after another, each ringing on.
        static AudioClip Chord(string name, float seconds, float[] notes, float amp, float decay)
        {
            int n = Mathf.CeilToInt(seconds * SampleRate);
            var data = new float[n];
            float stagger = 0.07f;
            for (int j = 0; j < notes.Length; j++)
            {
                int start = Mathf.RoundToInt(j * stagger * SampleRate);
                for (int i = start; i < n; i++)
                {
                    float t = (i - start) / (float)SampleRate;
                    float env = Mathf.Exp(-decay * t) * Mathf.Min(1f, (i - start) / 80f);
                    data[i] += Mathf.Sin(2f * Mathf.PI * notes[j] * t) * amp * env / notes.Length * 1.6f;
                }
            }
            return MakeClip(name, data);
        }

        // Two seconds of 60 Hz and harmonics: whole cycles, so the loop has no click.
        static AudioClip HumClip()
        {
            int n = 2 * SampleRate;
            var data = new float[n];
            for (int i = 0; i < n; i++)
            {
                float t = i / (float)SampleRate;
                data[i] = 0.6f * Mathf.Sin(2f * Mathf.PI * 60f * t)
                        + 0.25f * Mathf.Sin(2f * Mathf.PI * 120f * t)
                        + 0.1f * Mathf.Sin(2f * Mathf.PI * 180f * t);
            }
            return MakeClip("hum", data);
        }

        static AudioClip MakeClip(string name, float[] data)
        {
            var clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}
