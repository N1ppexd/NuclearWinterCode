using NuclearWinter;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomGrid : MonoBehaviour
{

    public static CustomGrid CurrentGrid { get; private set; }

    
    [SerializeField] private int width = 10;
    [SerializeField] private int depth = 10;

    [SerializeField] private float cellSize = 1f;

    [SerializeField] private float cellOffset = 0;


    [SerializeField] private HexType hexType = HexType.pointyTop;

    [SerializeField] private OffsetType offsetType = OffsetType.odd;


    [SerializeField] private bool drawGrid = true;


    private CustomTile[,] grid;


    [SerializeField] private Color gridColor;

    //grid
    public CustomTile[,] Grid => grid;

    public CustomTile StartTile { get; private set; }
    public CustomTile EndTile { get; private set; }
    public float CellSize => cellSize;

    //hextype getter
    public HexType HexType => hexType;


    public Dictionary<Vector3Int, CustomTile> TilePositions = new();


    private void OnDrawGizmos()
    {
        if (!drawGrid) return;

        Gizmos.color = gridColor;

        Vector3 halfExtents = Vector3.zero;


        if(hexType == HexType.pointyTop)
        {
            float localWidth = cellSize / 2f * Mathf.Sqrt(3f);

            halfExtents = new Vector3(width * localWidth, 0, 3f / 4f * cellSize * depth) * 0.5f;
        }
        if(hexType == HexType.flatTop)
        {
            float localDepth = cellSize / 2f * Mathf.Sqrt(3f);
            halfExtents = new Vector3(3f / 4f * cellSize * width, 0, depth * localDepth) * 0.5f;
        }


        Vector3 center = transform.position - halfExtents;

        Vector3 offset = Vector3.zero;


        for (int x = 0; x < width; x++)
        {
            

            for (int z  = 0; z < depth; z++)
            {

                //calculate the center of the cell (x, y, z)
                Vector3 cellCenter = Vector3.zero;


                switch (hexType)
                {
                    case HexType.pointyTop:

                        float localWidth = cellSize / 2f * Mathf.Sqrt(3f);

                        if(offsetType == OffsetType.odd)
                        {
                            offset = (z % 2 == 0) ? Vector3.zero : new Vector3(localWidth, 0, 0) * 0.5f;
                        }
                        if(offsetType == OffsetType.even)
                        {
                            offset = (z % 2 == 0) ? new Vector3(localWidth, 0, 0) * 0.5f : Vector3.zero;
                        }
                        
                        //set the cell center
                        cellCenter = center + new Vector3((x + x * cellOffset) * localWidth, 0, 3f / 4f * cellSize * (z + z * cellOffset)) + offset;
                        break;

                    case HexType.flatTop:

                        float localDepth = cellSize / 2f * Mathf.Sqrt(3f);

                        if(offsetType == OffsetType.odd)
                        {
                            offset = (x % 2 == 0) ? Vector3.zero : new Vector3(0, 0, localDepth) * 0.5f;
                        }
                        else if(offsetType == OffsetType.even)
                        {
                            offset = (x % 2 == 0) ? new Vector3(0, 0, localDepth) * 0.5f : Vector3.zero;
                        }
                        

                        cellCenter = center + new Vector3(3f / 4f * cellSize * (x + x * cellOffset), 0, (z + z * cellOffset) * localDepth) + offset;
                        break;

                }

                ///Hexagon has 6 sides
                for (int i = 0; i < 6; i++)
                {
                    float angle = 60 * i;

                    angle *= Mathf.Deg2Rad;

                    if(hexType == HexType.pointyTop)
                    {
                        //dont draw at least some of the edges again
                        if (((i < 3 && x <= 0) || ( i>= 3 && x >= width - 1)) && (z > 0 && z < depth-1)) continue;

                        Vector3 pointA = cellCenter + cellSize / 2f * new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));
                        Vector3 pointB = cellCenter + cellSize / 2f * new Vector3(Mathf.Sin(angle + Mathf.Deg2Rad * 60), 0, Mathf.Cos(angle + Mathf.Deg2Rad * 60));

                        Gizmos.DrawLine(pointA, pointB);
                    }
                    if (hexType == HexType.flatTop)
                    {
                        //dont draw at least some of the edges again
                        if ((i < 3 && z <= 0) || (i >= 3 && z >= depth - 1) && (x > 0 && x < width - 1)) continue;

                        Vector3 pointA = cellCenter + cellSize / 2f * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
                        Vector3 pointB = cellCenter + cellSize / 2f * new Vector3(Mathf.Cos(angle + Mathf.Deg2Rad * 60), 0, Mathf.Sin(angle + Mathf.Deg2Rad * 60));

                        Gizmos.DrawLine(pointA, pointB);
                    }
                }
            }
        }
    }



    private void Awake()
    {
        //set instance
        if (CurrentGrid == null)
            CurrentGrid = this;
        else Destroy(this);

        GenerateGrid();
    }

    public void GenerateGrid()
    {

        grid = new CustomTile[width, depth];

        GenerateGrid(width, depth, cellSize);
    }

    /// <summary>
    /// Get the tile on the center of the grid
    /// </summary>
    /// <returns></returns>
    public CustomTile GetBaseTile()
    {
        if(grid == null)
        {
            Debug.LogError("grid is null :(");
            return null;
        }

        return grid[(grid.GetLength(0)-1)/2, (grid.GetLength(1) - 1) / 2];
    }


    //Generate a 2d hexagonal grid
    private void GenerateGrid(int width, int depth, float cellSize)
    {
        //2D hexagonal grid
        Vector3 halfExtents = Vector3.zero;


        if (hexType == HexType.pointyTop)
        {
            float localWidth = cellSize / 2f * Mathf.Sqrt(3f);

            halfExtents = new Vector3(width * localWidth, 0, 3f / 4f * cellSize * depth) * 0.5f;
        }
        if (hexType == HexType.flatTop)
        {
            float localDepth = cellSize / 2f * Mathf.Sqrt(3f);
            halfExtents = new Vector3(3f / 4f * cellSize * width, 0, depth * localDepth) * 0.5f;
        }

        Vector3 center = transform.position - halfExtents;

        Vector3 offset = Vector3.zero;

        //Generate the grid Hexagonal. the vertical rows are offset by half the cell size
        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < depth; z++)
            {
                Vector3 cellCenter = Vector3.zero;


                switch (hexType)
                {
                    case HexType.pointyTop:

                        float localWidth = cellSize / 2f * Mathf.Sqrt(3f);

                        if (offsetType == OffsetType.odd)
                        {
                            offset = (z % 2 == 0) ? Vector3.zero : new Vector3(localWidth, 0, 0) * 0.5f;
                        }
                        if (offsetType == OffsetType.even)
                        {
                            offset = (z % 2 == 0) ? new Vector3(localWidth, 0, 0) * 0.5f : Vector3.zero;
                        }

                        //set the cell center
                        cellCenter = center + new Vector3((x + x * cellOffset) * localWidth, 0, 3f / 4f * cellSize * (z + z * cellOffset)) + offset;
                        break;

                    case HexType.flatTop:

                        float localHeight = cellSize / 2f * Mathf.Sqrt(3f);

                        if (offsetType == OffsetType.odd)
                        {
                            offset = (x % 2 == 0) ? Vector3.zero : new Vector3(0, 0, localHeight) * 0.5f;
                        }
                        else if (offsetType == OffsetType.even)
                        {
                            offset = (x % 2 == 0) ? new Vector3(0, 0, localHeight) * 0.5f : Vector3.zero;
                        }


                        cellCenter = center + new Vector3(3f / 4f * cellSize * (x + x * cellOffset), 0, (z + z * cellOffset) * localHeight) + offset;
                        break;
                }


                //add a tile to the array...
                grid[x, z] = new CustomTile(new Vector2Int(x,z), cellCenter, hexType, offsetType, this);
            }
        }


        for(int x = 0; x < width; x++)
        {
            for(int z = 0; z < depth; z++)
            {
                if (grid[x,z] == null)
                {
                    Debug.LogError("Tile is null");
                    continue;
                }

                //Vector3 tilePos = grid[x, z].AxialToOddRow(grid[x,z].Position);

                List<CustomTile> neighbours = new List<CustomTile>();


                foreach(Vector3Int direction in HexagonOperations.cubeDirectionVectors)
                {
                    Vector3Int neighbourPos = grid[x, z].CubeCoordinates + direction;

                    if(TilePositions.ContainsKey(neighbourPos))
                    {
                        neighbours.Add(TilePositions[neighbourPos]);
                    }
                }

                
                //loop through the neighbours
                /*if(hexType == HexType.pointyTop)
                {
                    if(x > 0)
                    {
                        if (grid[x - 1, z] != null)
                            neighbours.Add(grid[x - 1, z]);
                    }
                    if(x < width - 1)
                    {
                        if (grid[x + 1, z] != null)
                            neighbours.Add(grid[x + 1, z]);

                        if(z < depth - 1)
                        {
                            if (grid[x + 1, z + 1] != null && Mathf.Abs(z % 2) != 0)
                                neighbours.Add(grid[x + 1, z + 1]);
                        }
                        if(z > 0)
                        {

                            if (grid[x + 1, z - 1] != null && Mathf.Abs(z % 2) != 0)
                                neighbours.Add(grid[x + 1, z - 1]);
                        }
                    }
                    if(z > 0)
                    {
                        if (grid[x, z - 1] != null)
                            neighbours.Add(grid[x, z - 1]);
                    }
                    
                    if(z < depth - 1)
                    {
                        if (grid[x, z + 1] != null)
                            neighbours.Add(grid[x, z + 1]);
                    }
                }
                if(hexType == HexType.flatTop)
                {
                    if (z > 0)
                        {
                        if (grid[x, z - 1] != null)
                            neighbours.Add(grid[x, z - 1]);
                        }
                        if (z < depth - 1)
                        {
                        if (grid[x, z + 1] != null)
                            neighbours.Add(grid[x, z + 1]);
                        
                        if (x < width - 1)
                            {
                            if (grid[x + 1, z + 1] != null && Mathf.Abs(x % 2) != 0)
                                neighbours.Add(grid[x + 1, z + 1]);
                            }
                        if (x > 0)
                            {
                            if (grid[x - 1, z + 1] != null && Mathf.Abs(x % 2) != 0)
                               neighbours.Add(grid[x - 1, z + 1]);
                            }
                        }
                        if (x > 0)
                        {
                         if (grid[x - 1, z] != null)
                            neighbours.Add(grid[x - 1, z]);
                        }
                         if (x < depth - 1)
                        {
                        if (grid[x + 1, z] != null)
                            neighbours.Add(grid[x + 1, z]);
                        }
                }*/


                grid[x,z].SetNeighbours(neighbours.ToArray());
            }
        }


        StartCoroutine(WaitGen());
    }

    IEnumerator WaitGen()
    {
        yield return new WaitForFixedUpdate();
        Debug.Log("on generated");
        GameEvents.GeneratedGrid(this);
    }
}

/// <summary>
/// Is the hexagon a pointy type or a flat type
/// </summary>
public enum HexType
{
    pointyTop, flatTop
}


public enum OffsetType
{
    odd, even
}
