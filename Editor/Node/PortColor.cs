using System;
using UnityEngine;
namespace TextureDesigner.Editor
{
    internal static class PortColor
    {
        public static readonly Color portColor = new Color(0.6f, 0.5f, 0.9f);
        private static readonly Color floatPortColor = new(0.5f, 0.9f, 0.9f);
        private static readonly Color stringPortColor = new(0.6f, 0.9f, 0.6f);
        private static readonly Color SingleChannelPortColor = new(0.75f, 0.75f, 0.75f);
        private static readonly Color TexturePortColor = new(1f, 0.8f, 0.95f);
        private static readonly Color ColorPortColor = new(1f, 0.85f, 0.5f);

        public static Color GetColor(Type type)
        {
            if (type == typeof(Socket))
                return portColor;

            if (type == typeof(FloatSocket))
                return floatPortColor;

            if (type == typeof(StringSocket))
                return stringPortColor;

            if (type == typeof(SingleChannelTextureSocket))
                return SingleChannelPortColor;

            if (type == typeof(TextureSocket))
                return TexturePortColor;

            if (type == typeof(ColorSocket))
                return ColorPortColor;

            return Color.white;
        }
    }
}
