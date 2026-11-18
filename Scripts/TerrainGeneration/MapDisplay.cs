using UnityEngine;

namespace NuclearWinter
{
    public class MapDisplay : MonoBehaviour
    {
        public Renderer textureRenderer;


        /// <summary>
        /// Draw a texture from a noise map.
        /// </summary>
        /// <param name="noiseMap"></param>
        public void DrawNoiseMap(float[,] noiseMap)
        {
            int width = noiseMap.GetLength(0);
            int heigth = noiseMap.GetLength(1);

            Texture2D texture = new Texture2D(width, heigth);

            Color[] colorMap = new Color[width * heigth];

            for (int y = 0; y < heigth; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    colorMap[y * width + x] = Color.Lerp(Color.black, Color.white, noiseMap[x, y]);
                }
            }
            texture.SetPixels(colorMap);
            texture.Apply();
            DrawTexture(texture);
        }


        /// <summary>
        /// This displays the texture after it has been drawn
        /// </summary>
        /// <param name="texture"></param>
        public void DrawTexture(Texture2D texture)
        {
            //textureRenderer.material.mainTexture = texture;
            textureRenderer.sharedMaterial.mainTexture = texture;
            textureRenderer.transform.localScale = new Vector3(texture.width, 1, texture.height);
        }
    }
}
