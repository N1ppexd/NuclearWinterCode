using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    // I found these things from this place:
    // https://www.redblobgames.com/grids/hexagons/
    public static class HexagonOperations
    {

        /// <summary>
        /// Transform vector 2 index coordinates to cubed coordinates
        /// </summary>
        /// <param name="index"></param>
        /// <param name="offsetType"></param>
        /// <param name="hexType"></param>
        /// <returns></returns>
        public static Vector3Int GetCubeCoordinates(Vector2Int index, OffsetType offsetType, HexType hexType)
        {
            //q, r, x refer to columns, rows and the third thing that the cubed coordinate system uses...
            var q = 0;
            var r = 0;
            var s = 0;

            if (hexType == HexType.pointyTop)
            {
                if (offsetType == OffsetType.odd)
                    q = index.x - (index.y - (index.y & 1)) / 2;
                else if (offsetType == OffsetType.even)
                    q = index.x - (index.y + (index.y & 1)) / 2;

                r = index.y;
                s = -q - r;
            }
            else if (hexType == HexType.flatTop)
            {
                q = index.x;
                if (offsetType == OffsetType.odd)
                    r = index.y - (index.x - (index.x & 1)) / 2;
                else if (offsetType == OffsetType.even)
                    r = index.y - (index.x + (index.x & 1)) / 2;
                s = -q - r;
            }

            //columns, rows and whatever the fuck that s even is..
            return new Vector3Int(q, r, s);
        }

        //the directions of the neighbours..
        public static Vector3Int[] cubeDirectionVectors =
            {
            new (1,0,-1),
            new (1,-1,0),
            new (0,-1,1),
            new (-1,0,1),
            new (-1,1,0),
            new (0,1,-1)
        };


        /// <summary>
        /// Get the neighbour of a tile
        /// </summary>
        /// <param name="cubeCoordinates"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector3Int GetNeighbour(Vector3Int cubeCoordinates, int direction)
        {
            return cubeCoordinates + cubeDirectionVectors[direction];
        }


        /// <summary>
        /// Distance between two tiles
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static float HexDistance(CustomTile a, CustomTile b)
        {
            return Mathf.Max(
                    Mathf.Abs(a.CubeCoordinates.x - b.CubeCoordinates.x),
                    Mathf.Abs(a.CubeCoordinates.y - b.CubeCoordinates.y),
                    Mathf.Abs(a.CubeCoordinates.z - b.CubeCoordinates.z)
                    );
        }


        /// <summary>
        /// Get all the tiles in <paramref name="range"/> from the center tile <paramref name="center"/>
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="center"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public static List<CustomTile> GetTilesInRange(CustomGrid grid, CustomTile center, int range)
        {
            //the list of tiles that eventually gets returned.
            List<CustomTile> tiles = new();


            for (int dx = -range; dx <= range; dx++)
            {
                for (int dy = Mathf.Max(-range, -dx - range); dy <= Mathf.Min(range, -dx + range); dy++)
                {
                    int dz = -dx - dy;
                    if (grid.TilePositions.TryGetValue(center.CubeCoordinates + new Vector3Int(dx, dy, dz), out CustomTile tile))
                    {
                        tiles.Add(tile);
                    }
                    else
                    {
                        Debug.Log(
                            $"The cube coordinate " +
                            $"{center.CubeCoordinates + new Vector3Int(dx, dy, dz)} " +
                            $"has no tile");
                    }
                }
            }

            return tiles;
        }



        /// <summary>
        /// Get the directin from int (the direction is the number of the neighbour whos dir you want)
        /// </summary>
        /// <param name="direction"></param>
        /// <returns></returns>
        public static Vector3Int CubeDirection(int direction) => cubeDirectionVectors[direction];


        public static CustomTile CubeAdd(CustomTile hex, Vector3Int vec, CustomGrid grid) => grid.TilePositions[hex.CubeCoordinates + vec];
        public static CustomTile CubeAdd(CustomTile a, CustomTile b, CustomGrid grid) => CubeAdd(a, b.CubeCoordinates, grid);


        /// <summary>
        /// Get the tile in cube coordinates from <paramref name="factor"/> away from the center <paramref name="hex"/>
        /// </summary>
        /// <param name="hex"></param>
        /// <param name="grid"></param>
        /// <param name="factor"></param>
        /// <returns></returns>
        public static CustomTile ScaleCube(CustomTile hex, CustomGrid grid, int factor)
        {
            Vector3Int cubeCoord = hex.CubeCoordinates;
            cubeCoord *= factor;

            return grid.TilePositions[cubeCoord];
        }



        /// <summary>
        /// Get all hexes in n around an hex
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="center"></param>
        /// <param name="radius"></param>
        /// <returns></returns>
        public static List<CustomTile> CubeRing(CustomGrid grid, CustomTile center, int radius)
        {
            List<CustomTile> tiles = new();

            if (radius <= 0) return null;


            CustomTile hex = CubeAdd(center, ScaleCube(grid.TilePositions[CubeDirection(5)], grid, radius), grid);


            for (int i = 1; i <= 6; i++)
            {
                for (int j = 0; j < radius; j++)
                {
                    tiles.Add(hex);

                    int k = i;
                    if (i == 6) k = 0;

                    try
                    {
                        hex = grid.TilePositions[GetNeighbour(hex.CubeCoordinates, k)];
                    }
                    catch
                    {
                        continue;
                    }
                }
            }


            return tiles;
        }
    }
}
