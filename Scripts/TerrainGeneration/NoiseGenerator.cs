using UnityEngine;

namespace NuclearWinter
{
    public static class NoiseGenerator
    {
        /// <summary>
        /// Generate a noise map with the given parameters.
        /// </summary>
        /// <param name="mapWidth"></param>
        /// <param name="mapHeight"></param>
        /// <param name="seed"></param>
        /// <param name="scale"></param>
        /// <param name="octaves"></param>
        /// <param name="persistance"></param>
        /// <param name="lacunarity"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int octaves, float persistance, float lacunarity, int seed, Vector2 offset)
        {
            float[,] noiseMap = new float[mapWidth, mapHeight];


            if(scale <= 0f)
            {
                scale = 0.0001f;
            }

            //create a random number generator
            System.Random prng = new System.Random(seed);
            Vector2[] octaveOffsets = new Vector2[octaves];

            for(int i = 0; i < octaves; i++)
            {
                offset = new Vector2(prng.Next(-100000, 100000), prng.Next(-100000, 100000));
            }

            float maxNoiseHeight = float.MinValue;
            float minNoiseHeight = float.MaxValue;

            for (int x = 0; x < mapWidth; x++)
            {
                for (int y = 0; y < mapHeight; y++)
                {
                    float sampleX = (x + offset.x) / scale + offset.x;
                    float sampleY = (y + offset.y) / scale + offset.y;

                    float amplitude = 1f;
                    float frequency = 1f;

                    float noiseHeight = 0f;

                    for (int i = 0; i < octaves; i++)
                    {
                        float perlinValue = Mathf.PerlinNoise(sampleX * frequency, sampleY * frequency) * 2f - 1f;
                        noiseHeight += perlinValue * amplitude;

                        amplitude *= persistance;
                        frequency *= lacunarity;
                    }

                    if(noiseHeight > maxNoiseHeight)
                    {
                        maxNoiseHeight = noiseHeight;
                    }
                    else if(noiseHeight < minNoiseHeight)
                    {
                        minNoiseHeight = noiseHeight;
                    }

                    noiseMap[x, y] = noiseHeight;
                }
            }


            for(int x = 0; x < mapWidth; x++)
            {
                for(int y = 0; y < mapHeight; y++)
                {
                    noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
                }
            }

            return noiseMap;
        }
    }
}
