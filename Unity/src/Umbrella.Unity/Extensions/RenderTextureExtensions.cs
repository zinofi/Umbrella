using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbrella.Utilities;

namespace UnityEngine
{
    public static class RenderTextureExtensions
    {
        public static Texture2D ToTexture2D(this RenderTexture renderTexture)
        {
            Guard.ArgumentNotNull(renderTexture, nameof(renderTexture));

            RenderTexture previousTexture = RenderTexture.active;
            RenderTexture.active = renderTexture;

            Texture2D texture = new Texture2D(renderTexture.width, renderTexture.height);
            texture.ReadPixels(new Rect(0, 0, texture.width, texture.height), 0, 0);
            texture.Apply(false);

            RenderTexture.active = previousTexture;

            return texture;
        }
    }
}