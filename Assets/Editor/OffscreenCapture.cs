using System.IO;
using UnityEngine;

namespace Noted.EditorTools
{
    /// <summary>
    /// Renders a camera view to a PNG without Play Mode frames (kit rule 3a). Screen Space - Camera canvases are included;
    /// Screen Space - Overlay ones are not. Throwaway rigs are destroyed immediately on every path (rule 22).
    /// </summary>
    public static class OffscreenCapture
    {
        /// <summary>Renders an existing camera. Returns the written path, or an error string starting with "error:".</summary>
        public static string Render(Camera cam, string outPath, int width = 1280, int height = 720)
        {
            if (cam == null) return "error: no camera";
            var rt = new RenderTexture(width, height, 24, RenderTextureFormat.ARGB32, RenderTextureReadWrite.sRGB);
            var prevTarget = cam.targetTexture;
            var prevActive = RenderTexture.active;
            Texture2D tex = null;
            try
            {
                Canvas.ForceUpdateCanvases();
                cam.targetTexture = rt;
                cam.Render();
                RenderTexture.active = rt;
                tex = new Texture2D(width, height, TextureFormat.RGBA32, false, false);
                tex.ReadPixels(new Rect(0, 0, width, height), 0, 0);
                tex.Apply();
                Directory.CreateDirectory(Path.GetDirectoryName(outPath));
                File.WriteAllBytes(outPath, tex.EncodeToPNG());
                return outPath;
            }
            finally
            {
                cam.targetTexture = prevTarget;
                RenderTexture.active = prevActive;
                rt.Release();
                Object.DestroyImmediate(rt);
                if (tex != null) Object.DestroyImmediate(tex);
            }
        }

        /// <summary>Renders from a throwaway camera at a pose, with the scene's lights and objects as they are.</summary>
        public static string FromPose(Vector3 position, Vector3 eulerAngles, float verticalFov, string outPath, int width = 1280, int height = 720)
        {
            GameObject rig = null;
            try
            {
                rig = new GameObject("CaptureRig") { hideFlags = HideFlags.HideAndDontSave };
                var cam = rig.AddComponent<Camera>();
                cam.transform.SetPositionAndRotation(position, Quaternion.Euler(eulerAngles));
                cam.fieldOfView = verticalFov;
                cam.nearClipPlane = 0.05f;
                cam.farClipPlane = 200f;
                cam.clearFlags = CameraClearFlags.SolidColor;
                cam.backgroundColor = new Color(0.08f, 0.1f, 0.11f);
                return Render(cam, outPath, width, height);
            }
            finally
            {
                if (rig != null) Object.DestroyImmediate(rig);
            }
        }
    }
}
