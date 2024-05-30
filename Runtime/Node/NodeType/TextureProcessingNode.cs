using UnityEngine;

namespace TextureDesigner
{
    /// <summary>
    /// Represents a node used for texture processing.
    /// </summary>
    public class TextureProcessingNode : TextureDesignerNode
    {
        /// <summary>
        /// The width of the input texture.
        /// </summary>
        public int InputWidth;

        /// <summary>
        /// The height of the input texture.
        /// </summary>
        public int InputHeight;

        /// <summary>
        /// The multiplier for the width of the output texture. 0 = 1, 1 = 2, 2 = 4, 3 = 8, etc.
        /// </summary>
        [Header("Output Size")]
        [Range(-8, 8)]
        [InspectorInput]
        public int WidthMultiplier;

        /// <summary>
        /// The multiplier for the height of the output texture. 0 = 1, 1 = 2, 2 = 4, 3 = 8, etc.
        /// </summary>
        [Range(-8, 8)]
        [InspectorInput]
        public int HeightMultiplier;

        /// <summary>
        /// Gets the size of the output texture.
        /// </summary>
        public Vector2Int OutputSize
        {
            get
            {
                var _widthMultiplier = (int)Mathf.Pow(2, WidthMultiplier);
                var _heightMultiplier = (int)Mathf.Pow(2, HeightMultiplier);
                return new Vector2Int(InputWidth * _widthMultiplier, InputHeight * _heightMultiplier);
            }
        }
    }
}
