using UnityEngine;

namespace NuclearWinter
{

    [CreateAssetMenu(fileName = "GetTileSO", menuName = "ScriptableObjects/GetTileSO", order = 1)]
    public class GetTileSO : ScriptableObject
    {
        [SerializeField] private TileSO waterTile, sandTile, terrainTile, rockyTerrainTile, snowPeakTile;


        [Range(0f, 1f)]
        [SerializeField] private float sandMinHeight, terrainMinHeight, rockyTerrainMinHeight, snowPeakMinHeight;


        private void OnValidate()
        {
            if(sandMinHeight > terrainMinHeight)
            {
                terrainMinHeight = sandMinHeight;
            }
            if(terrainMinHeight > rockyTerrainMinHeight)
            {
                rockyTerrainMinHeight = terrainMinHeight;
            }
            if(rockyTerrainMinHeight > snowPeakMinHeight)
            {
                snowPeakMinHeight = rockyTerrainMinHeight;
            }
        }


        public float SandMinHeight => sandMinHeight;
        public float TerrainMinHeight => terrainMinHeight;
        public float RockyTerrainMinHeight => rockyTerrainMinHeight;
        public float SnowPeakMinHeight => snowPeakMinHeight;


        public TileSO GetTerrainTile(float height)
        {
            if (height >= rockyTerrainMinHeight)
            {
                if (height >= snowPeakMinHeight)
                {
                    return snowPeakTile;
                }
                else
                {
                    return rockyTerrainTile;
                }
            }
            else
            {
                if(height < terrainMinHeight)
                {
                    if(height < sandMinHeight)
                        return waterTile;
                    else return sandTile;
                }
                return terrainTile;
            }
        }
    }
}
