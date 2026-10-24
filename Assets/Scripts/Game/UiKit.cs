using UnityEngine;
using UnityEngine.UI;

namespace Noted.Game
{
    /// <summary>Tiny helpers that build uGUI elements in code. Placeholder look: legacy Text on flat panels in the palette colours.</summary>
    public static class UiKit
    {
        public static readonly Color Cream = new Color32(0xEE, 0xE8, 0xE1, 0xFF);
        public static readonly Color DarkTeal = new Color32(0x26, 0x2E, 0x2F, 0xFF);
        public static readonly Color Navy = new Color32(0x17, 0x1C, 0x23, 0xFF);
        public static readonly Color WallTeal = new Color32(0x45, 0x53, 0x50, 0xFF);
        public static readonly Color Green = new Color32(0x37, 0x74, 0x70, 0xFF);
        public static readonly Color Purple = new Color32(0x78, 0x3F, 0x8C, 0xFF);
        public static readonly Color Yellow = new Color32(0xDA, 0xBD, 0x6A, 0xFF);

        public static Font DefaultFont()
        {
            return Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        }

        public static RectTransform Rect(Transform parent, string name, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var go = new GameObject(name, typeof(RectTransform));
            go.layer = parent.gameObject.layer;
            var rt = (RectTransform)go.transform;
            rt.SetParent(parent, false);
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
            return rt;
        }

        public static Image Panel(Transform parent, string name, Color color, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var rt = Rect(parent, name, anchorMin, anchorMax, offsetMin, offsetMax);
            var img = rt.gameObject.AddComponent<Image>();
            img.color = color;
            return img;
        }

        public static Text Label(Transform parent, string name, string text, int size, Color color, TextAnchor align,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var rt = Rect(parent, name, anchorMin, anchorMax, offsetMin, offsetMax);
            var t = rt.gameObject.AddComponent<Text>();
            t.font = DefaultFont();
            t.text = text;
            t.fontSize = size;
            t.color = color;
            t.alignment = align;
            t.supportRichText = true;
            t.horizontalOverflow = HorizontalWrapMode.Wrap;
            t.verticalOverflow = VerticalWrapMode.Overflow;
            t.raycastTarget = false;
            return t;
        }

        public static Button MakeButton(Transform parent, string name, string label, int size, Color background, Color textColor,
            Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var img = Panel(parent, name, background, anchorMin, anchorMax, offsetMin, offsetMax);
            var button = img.gameObject.AddComponent<Button>();
            button.targetGraphic = img;
            var colors = button.colors;
            colors.highlightedColor = new Color(1.25f, 1.25f, 1.25f, 1f);
            colors.pressedColor = new Color(0.8f, 0.8f, 0.8f, 1f);
            colors.disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.6f);
            button.colors = colors;
            var text = Label(img.transform, "Label", label, size, textColor, TextAnchor.MiddleCenter,
                Vector2.zero, Vector2.one, new Vector2(8, 4), new Vector2(-8, -4));
            text.raycastTarget = false;
            return button;
        }

        /// <summary>Gives on-screen text a dark outline so it stays readable over bright props.</summary>
        public static void AddOutline(Text t)
        {
            var o = t.gameObject.AddComponent<Outline>();
            o.effectColor = new Color(0.03f, 0.05f, 0.06f, 0.95f);
            o.effectDistance = new Vector2(2f, -2f);
        }

        public static Text ButtonText(Button b)
        {
            return b.GetComponentInChildren<Text>();
        }

        public static void ClearChildren(Transform t)
        {
            for (int i = t.childCount - 1; i >= 0; i--)
            {
                Object.Destroy(t.GetChild(i).gameObject);
            }
        }
    }
}
