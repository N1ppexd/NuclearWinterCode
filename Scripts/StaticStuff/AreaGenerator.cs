using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    public static class AreaGenerator
    {

        /// <summary>
        /// Set the tile and its height and other stuff..
        /// </summary>
        /// <param name="tileSO"></param>
        /// <param name="tile"></param>
        public static void SetTile(TileSO tileSO, CustomTile tile, PathGenerator pathGenerator, CustomGrid grid, out GameObject tilePrefab)
        {

            //Set the movement cost to the terrain tile
            tile.SetCost(tileSO.MovementCost);

            tilePrefab = Object.Instantiate(tileSO.TilePrefab, tile.Position, Quaternion.identity);

            AnimationCurve curve = pathGenerator.HeightCurve;
            float minHexHeight = pathGenerator.MinHexHeight;
            float heightMultiplier = pathGenerator.HeightMultiplier;

            //set height and tile reference
            if (tilePrefab.TryGetComponent(out HexTile hexTile))
            {
                hexTile.SetTile(tile, grid, tileSO);
                hexTile.SetHeight(minHexHeight * curve.Evaluate(minHexHeight) * heightMultiplier, 
                    tile.TerrainHeight * curve.Evaluate(tile.TerrainHeight) * heightMultiplier);
            }
        }



        /// <summary>
        /// Generate an area around <paramref name="centerTile"/> on <paramref name="grid"/> with <paramref name="range"/>
        /// </summary>
        /// <param name="centerTile"></param>
        /// <param name="grid"></param>
        /// <param name="range"></param>
        public static void GenerateArea(CustomTile centerTile, CustomGrid grid, PathGenerator pathGenerator, int range, TileSO tileSO)
        {
            if (centerTile == null) return;
            if (centerTile.HexTile == null) return;
            if (grid == null) return;


            if (centerTile == null) { Debug.LogError("centerTile is null"); return; }
            if (range < 0) { Debug.LogError("range has to be positive"); return; }



            //this is the coordinate system used to locate neghboring hexagons easily
            Dictionary<Vector3Int, CustomTile> cubeCoordSystem = grid.TilePositions;

            //take the cube coordinates of the center tile
            Vector3 centerCubeCoord = centerTile.CubeCoordinates;

            //get all of the tiles that are within the range...
            List<CustomTile> tilesInRange = HexagonOperations.GetTilesInRange(grid, centerTile, range);

            //the current tile scriptableObject
            //TileSO tileSO = centerTile.Tile.TileSO;

            foreach (CustomTile tile in tilesInRange)
            {
                if (tile == null)
                {
                    Debug.LogError("tile is null");
                    continue;
                }

                //set the tile to the same tile as the center tile for every tile in the range.
                GameObject.Destroy(tile.HexTile.gameObject);

                SetTile(tileSO, tile, pathGenerator, grid, out GameObject tilePrefab);

                if (tilePrefab != null)
                {

                }
            }
        }

    }
}
