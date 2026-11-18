using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    public class HeightMapGenerator : MonoBehaviour
    {

        public enum DrawMode { NoiseMap, ColorMap, Mesh };
        public DrawMode drawMode;

        public int mapWidth;
        public int mapHeight;
        public float noiseScale;

        public int octaves;
        [Range(0, 1)]
        public float persistance;
        public float lacunarity;

        public int seed;
        public Vector2 offset;

        public bool autoUpdate;

        //public TerrainType[] regions;

        private void Start()
        {
            GenerateMap();

            StartCoroutine(UpdateTexture());
        }


        IEnumerator UpdateTexture()
        {
            yield return new WaitForSeconds(1f);
            if(autoUpdate)
                GenerateMap();

            //get the seed
            seed = (int)System.DateTime.Now.Ticks;

            StartCoroutine(UpdateTexture());
        }

        public void GenerateMap()
        {
            float[,] noiseMap = NoiseGenerator.GenerateNoiseMap(mapWidth, mapHeight, noiseScale, octaves, persistance, lacunarity, seed, offset);
            MapDisplay display = FindObjectOfType<MapDisplay>();

            //draw the noise map
            display.DrawNoiseMap(noiseMap);

            /*
            Color[] colorMap = new Color[mapWidth * mapHeight];
            for (int y = 0; y < mapHeight; y++)
            {
                for(int x = 0; x < mapWidth; x++)
                {
                    float currentHeight = noiseMap[x, y];
                    for(int i = 0; i < regions.Length; i++)
                    {
                        if(currentHeight <= regions[i].height)
                        {
                            colorMap[y * mapWidth + x] = regions[i].color;
                            break;
                        }
                    }
                }
            }*/

            /*MapDisplay display = FindObjectOfType<MapDisplay>();
            if (drawMode == DrawMode.NoiseMap)
            {
                display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
            }
            else if (drawMode == DrawMode.ColorMap)
            {
                display.DrawTexture(TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
            }
            else if (drawMode == DrawMode.Mesh)
            {
                display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap), TextureGenerator.TextureFromColorMap(colorMap, mapWidth, mapHeight));
            }*/
        }

        private void OnValidate()
        {
            if (mapWidth < 1)
            {
                mapWidth = 1;
            }
            if (mapHeight < 1)
            {
                mapHeight = 1;
            }

            if (lacunarity < 1f)
            {
                lacunarity = 1f;
            }

            if (octaves < 1)
            {
                octaves = 1;
            }
        }
    }
}
