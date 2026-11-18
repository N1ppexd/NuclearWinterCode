using NuclearWinter;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Profiling;

public class PathGenerator : MonoBehaviour
{

    public static PathGenerator Instance { get; private set; }


    [Space(5)]
    [Header("The Player settings used to unhide the ones we need to see")]
    [SerializeField] private PlayerSettingsSO playerSettings;


    [Space(10)]
    [SerializeField] private TileSO pathTile;

    [Space(10)]
    [SerializeField] private GetTileSO getTileSO;


    [SerializeField] private float mapScale, lacunarity;
    [Range(0,1f)]
    [SerializeField] private float mapPersistence;
    [SerializeField] private int octaves;
    [SerializeField] private float heightMultiplier;

    //the room tiles used to connect rooms....
    private TileSO[] roomTilesSO;


    [Space(10)]
    [SerializeField] private Transform waterPlane;

    [Space(10)]
    [Header("The connections between the rooms")]
    [SerializeField] private TileConnectionsSO tileConnectionsSO;



    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private Transform canvas;

    [Space(10)]
    [SerializeField] private GameEvent onGeneratedEvent, startGenerationEvent;

    #region lists
    private  List<GameObject> rooms = new();
    private readonly List<GameObject> pathObjects = new ();
    private readonly List<CustomTile> usedTiles = new ();
    private readonly List<CustomTile> roomTiles = new ();
    //these are set around a room to prevet the rooms from being too close to each other
    private readonly List<CustomTile> aroundRoomTiles = new();
    private readonly List<GameObject> environmentTiles = new();
    #endregion

    //private bool isGenerating = false;

    [Space(20)]
    [SerializeField] private AnimationCurve worldHeightCurve;
    [Range(0,1f)]
    [SerializeField] private float maxPathHeight = 0f;
    [Range(0,1f)]
    [SerializeField] private float minPathHeight = 0f;


    [Space(10)]
    [SerializeField] private Vector2Int chunkSize = new Vector2Int(10, 10);

    private float minHexHeight = 0f;


    private CustomGrid customGrid;

    #region getters
    public CustomTile BaseTile => CustomGrid.CurrentGrid.GetBaseTile();

    public float MinPathHeight => minPathHeight;
    public float MaxPathHeight => maxPathHeight;
    public float MinHexHeight => minHexHeight;
    public float HeightMultiplier => heightMultiplier;
    public AnimationCurve HeightCurve => worldHeightCurve;
   

    public List<CustomTile> RoomTiles => roomTiles;
    public List<CustomTile> UsedTiles => usedTiles;

    #endregion


    #region onValidate
    private void OnValidate()
    {

        if(minPathHeight > maxPathHeight)
        {
            maxPathHeight = minPathHeight;
        }

        if(lacunarity < 1f)
        {
            lacunarity = 1f;
        }

        if(octaves < 1)
        {
            octaves = 1;
        }
        if(heightMultiplier < 1)
        {
            heightMultiplier = 1;
        }
    }
    #endregion


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);

        //customGrid.GenerateGrid();
    }


    private void Start()
    {
        try
        {
            customGrid = CustomGrid.CurrentGrid;
        }
        catch { return; }
        
        GenerateRooms();
    }

    /*
    private void OnEnable()
    {
        GameEvents.onGeneratedGrid += GenerateRooms;
        GameEvents.onGeneratedGrid += (CustomGrid currentGrid) => Debug.Log("shiit");
    }

    private void OnDisable()
    {
        GameEvents.onGeneratedGrid -= GenerateRooms;
        GameEvents.onGeneratedGrid -= (CustomGrid currentGrid) => Debug.Log("shiit");
    }*/


    /// <summary>
    /// Generate the rooms to the grid
    /// </summary>
    private void GenerateRooms()
    {
        //customGrid = currentGrid;

        Profiler.BeginSample("GenerateRooms");

        //start the start event shit
        startGenerationEvent?.Raise();

        CustomTile[,] grid = customGrid.Grid;


        //set the connections

        if(tileConnectionsSO == null || tileConnectionsSO.TilesSO == null)
        {
            Debug.LogError("TileConnectionsSO is null!");
            return;
        }

        //clear the connections, then set them again...
        tileConnectionsSO.TileConnections?.Clear();
        tileConnectionsSO = Instantiate(tileConnectionsSO);
        tileConnectionsSO.SetConnections();
        //if roomTilesSO is null, use the roomsTilesSO from the tileConnectionsSO
        roomTilesSO = tileConnectionsSO.TilesSO;

        Debug.Log("toomTIlesSO length = " + roomTilesSO.Length);

        //isGenerating = true;
        //clear al old rooms first
        if(rooms != null)
        {
            foreach (var room in rooms)
            {
                Destroy(room);
            }
        }
        if(pathObjects != null)
        {
            foreach (var path in pathObjects)
            {
                Destroy(path);
            }

            pathObjects.Clear();
        }
        usedTiles?.Clear();
        roomTiles?.Clear();
        aroundRoomTiles?.Clear();
        rooms?.Clear();
        foreach(var tile in environmentTiles)
            Destroy(tile);
        environmentTiles?.Clear();

        foreach (CustomTile tile in grid)
        {
            tile.Visited = false;
        }

        Resources.UnloadUnusedAssets();


        minHexHeight = Mathf.Infinity;
       
        StartCoroutine(Generate(grid));

        Profiler.EndSample();
    }


    IEnumerator Generate(CustomTile[,] grid)
    {
        while (true)
        {
            yield return null;
            //Generate the fucking terrain noise shit

            int seed = (int)System.DateTime.Now.Ticks;

            float[,] heightMap = NoiseGenerator.GenerateNoiseMap(grid.GetLength(0), grid.GetLength(1), mapScale, octaves, mapPersistence, lacunarity, seed, Vector2.zero);

            for (int x = 0; x < heightMap.GetLength(0); x++)
            {
                for (int y = 0; y < heightMap.GetLength(1); y++)
                {
                    //set the fucking height
                    grid[x, y].SetHeight(heightMap[x, y]);

                    if (heightMap[x, y] < minHexHeight)
                        minHexHeight = heightMap[x, y];
                }
            }


            //Set the waterplane to water line
            float waterHeight = getTileSO.SandMinHeight * worldHeightCurve.Evaluate(getTileSO.SandMinHeight) * heightMultiplier;
            waterPlane.transform.position = new Vector3(waterPlane.transform.position.x, waterHeight + 0.5f, waterPlane.transform.position.z);

            bool startAgain = false;
            //temp used tiles is used to see if the paths are possible
            List<CustomTile> tempUsedTiles = new();


            CustomTile baseTile = customGrid.GetBaseTile();
            roomTiles.Add(baseTile);


            if (baseTile.TerrainHeight > maxPathHeight || baseTile.TerrainHeight < minHexHeight)
            {
                startAgain = true;

            }

            int i = 0;

            while (true && !startAgain)
            {
                yield return null;
                //set the base tile position and so on
                if (roomTilesSO[i].IsBaseTile)
                {
                    //Set the movement cost to the base tile...
                    AreaGenerator.SetTile(roomTilesSO[i], baseTile, this, customGrid, out GameObject baseTilePrefab);

                    roomTilesSO[i].SetTile(baseTile);

                    rooms.Add(baseTilePrefab);

                    i++; //i has to rise otherwise the application freezes
                    continue;
                }



                //get random position within grid (start and end pos are set in CustomGrid.cs)
                int maxX = grid.GetLength(0);
                int maxZ = grid.GetLength(1);

                int currentX = Random.Range(1, maxX - 1);
                int currentZ = Random.Range(1, maxZ - 1);


                CustomTile currentTile = grid[currentX, currentZ];
                Vector3 randomPos = currentTile.Position;



                //dont spawn on the same tile twice
                if (currentTile == baseTile) continue;
                if (roomTiles.Contains(currentTile)) continue;
                if (aroundRoomTiles.Contains(currentTile)) continue;

                if (currentTile.TerrainHeight > maxPathHeight || currentTile.TerrainHeight < minHexHeight)
                {
                    continue;
                }


                //cant spawn on top of the base...
                if (IsInRadius(currentTile, baseTile, roomTilesSO[i].GenerateRadius, customGrid)) continue;


                roomTiles.Add(currentTile);


                List<CustomTile> aroundTiles = GetAroundTiles(currentX, currentZ, maxX, maxZ);


                //Set the neighjbours and diagonals to visited
                foreach (var tile in aroundTiles) aroundRoomTiles.Add(tile);


                AreaGenerator.SetTile(roomTilesSO[i], currentTile, this, customGrid, out GameObject tilePrefab);
                rooms.Add(tilePrefab);



                //set the tile to the current room tile scriptableObject
                roomTilesSO[i].SetTile(currentTile);
                i++;

                if (i >= roomTilesSO.Length)
                    break;
            }




            //done very badly at the moment. i probanbly need to use scriptable objects and other stuff like that to connect the nodes and then check them in order
            if (!startAgain)
            {

                foreach (var connection in tileConnectionsSO.TileConnections)
                {
                    if (connection.A.Tile == null || connection.B.Tile == null)
                    {
                        Debug.LogError("Tile is null!");
                        startAgain = true;
                        break;
                    }

                    //see if its possible to find all the connections properly
                    if (PathFinding.AStar(connection.A.Tile, connection.B.Tile, minPathHeight, maxPathHeight, grid, out List<CustomTile> path, tempUsedTiles, roomTiles))
                    {
                        foreach (CustomTile tile in path)
                        {
                            if (roomTiles.Contains(tile)) continue;

                            //temporarily used tiles,
                            //so that the path cant be generated on top of another path
                            tempUsedTiles.Add(tile);
                        }
                    }
                    else startAgain = true;
                }
            }


            //Start the loop again if it is not possible to find a path to the room
            if (startAgain)
            {

                Debug.LogWarning("Had to start again...");


                foreach (var tiles in grid)
                {
                    tiles.Visited = false;
                }

                foreach (var room in rooms)
                {
                    Destroy(room);
                }

                rooms?.Clear();
                roomTiles?.Clear();
                usedTiles?.Clear();
                aroundRoomTiles?.Clear();

                continue;
            }

            //lol
            GameEvents.SetBaseTiles(baseTile);

            break;
        }


        //generate paths between the rooms
        foreach (var connection in tileConnectionsSO.TileConnections)
        {
            GeneratePath(connection.A.Tile, connection.B.Tile, grid);
        }


        for(int x = 0; x < grid.GetLength(0); x++)
        {
            for(int z = 0; z < grid.GetLength(1); z++)
            {
                if (grid[x, z] == null) continue;

                if (usedTiles.Contains(grid[x, z]) || roomTiles.Contains(grid[x, z])) continue;

                AreaGenerator.SetTile(getTileSO.GetTerrainTile(grid[x, z].TerrainHeight), grid[x, z], this, customGrid, out GameObject tilePrefab);
                environmentTiles.Add(tilePrefab);


                //yield return null when the chunk is done
                if(x % chunkSize.x == 0 && z % chunkSize.y == 0)
                {
                    yield return null;
                }
            }
        }



        foreach (CustomTile room in roomTiles)
        {
            //do not enlarge the base
            if (room.TileSO.IsBaseTile) continue;

            AreaGenerator.GenerateArea(room, customGrid, this, room.TileSO.GenerateRadius, room.TileSO);
        }

        

        foreach(var tile in grid)
        {
            if (tile == null) continue;

            if (tile.TileSO.IsBaseTile) continue;

            if(!HexagonOperations.GetTilesInRange(customGrid, BaseTile, playerSettings.SeeRange).Contains(tile))
            {
                tile.HexTile.Hide();
            }
        }


        //isGenerating = false;
        //event that the generation is done
        GameEvents.Generated(true);

        //finishes the loading screen for example...
        onGeneratedEvent?.Raise();
    }


    /// <summary>
    /// Is the tile in this radius?
    /// </summary>
    /// <param name="center"></param>
    /// <param name="target"></param>
    /// <param name="radius"></param>
    /// <param name="grid"></param>
    /// <returns></returns>
    private bool IsInRadius(CustomTile center, CustomTile target, int radius, CustomGrid grid)
    {
        List<CustomTile> tiles = HexagonOperations.GetTilesInRange(grid, center, radius);
        return tiles.Contains(target);
    }



    /// <summary>
    /// Generate a path from a to b
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    private void GeneratePath(CustomTile a, CustomTile b, CustomTile[,] grid)
    {

        Profiler.BeginSample("GeneratePath");


        //Get the quickest path, then travelse it and instance the path objects
        if (!PathFinding.AStar(a, b, minPathHeight, maxPathHeight, grid, out List<CustomTile> path, usedTiles, roomTiles))
        {
            Debug.LogError("path cannot be found");
            return;
        }


        //draw the path
        for (int i = 0; i < path.Count; i++)
        {
            CustomTile tile = path[i];
            //dont spawn it on the a or b
            if (tile != a && tile != b)
            {

                GameObject tilePrefab = Instantiate(pathTile.TilePrefab, tile.Position, Quaternion.identity);

                //set the cost to the tile....
                tile.SetCost(pathTile.MovementCost);

                //set height and tile reference
                if (tilePrefab.TryGetComponent(out HexTile hexTile))
                {
                    hexTile.SetTile(tile, customGrid, pathTile);
                    hexTile.SetHeight(minHexHeight * worldHeightCurve.Evaluate(minHexHeight) * heightMultiplier, tile.TerrainHeight * worldHeightCurve.Evaluate(tile.TerrainHeight) * heightMultiplier);
                }

                pathObjects.Add(tilePrefab);

                //add the current tile to the used tiles.
                usedTiles.Add(tile);
            }
        }

        path.Clear();
        Profiler.EndSample();
    }

    //very shitty method, but it works
    private List<CustomTile> GetAroundTiles(int currentX, int currentZ, int maxX, int maxZ)
    {
        List<CustomTile> neighbours = new();
        //sets all of the neighbours and the diagonals to visited
        for (int x = currentX - 1; x <= currentX + 1; x++)
        {
            if (x < 0 || x >= maxX) continue;

            for (int z = currentZ - 1; z <= currentZ + 1; z++)
            {
                if (z < 0 || z >= maxZ) continue;

                if (customGrid.Grid[x, z] != null)
                {
                    neighbours.Add(customGrid.Grid[x, z]);
                }
            }
        }

        return neighbours;
    }
}
