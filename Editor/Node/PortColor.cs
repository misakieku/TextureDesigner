using System;
using UnityEngine;
namespace TextureDesigner.Editor
{
    internal static class PortColor
    {
        private static readonly Color SingleChannelPortColor = new(0.75f, 0.75f, 0.75f);
        private static readonly Color TexturePortColor = new(1f, 0.8f, 0.95f);
        private static readonly Color ColorPortColor = new(1f, 0.85f, 0.5f);

        public static Color GetColor(Type type)
        {
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
