using NuclearWinter;
using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    public class CustomTile
    {

        #region getters and setters
        public Vector3 Position { get; private set; }
        public Vector2Int Index { get; private set; }
        //has the tile been visited?
        public bool Visited { get; set; }
        //neighbours of the tile
        public CustomTile[] Neighbours { get; private set; }
        //parent of the tile
        public CustomTile Parent { get; private set; }

        public float TerrainHeight { get; private set; }

        public HexType HexagonType { get; private set; }
        public OffsetType OffsetType { get; private set; }

        public HexTile HexTile { get; private set; }
        public TileSO TileSO => HexTile.TileSO;

        //set wealkable to true at first
        public bool Walkable { get; set; } = true;

        public float Cost { get; private set; }

        public float GCost { get; set; } = Mathf.Infinity;
        public float HeuristicCost { get; set; }
        public float FCost => GCost + HeuristicCost;

        public Vector3Int CubeCoordinates { get; private set; }
        #endregion


        public CardSO Card { get; private set; }
        public bool HasCard => Card != null;

        public void SetCard(CardSO card) => Card = card;



        public CustomTile(Vector2Int index, Vector3 position, HexType hexType, OffsetType offsetType, CustomGrid grid)
        {
            HexagonType = hexType;
            Index = index;
            Position = position;

            CubeCoordinates = HexagonOperations.GetCubeCoordinates(index, offsetType, hexType);

            grid.TilePositions.Add(CubeCoordinates, this);
        }

        public void SetTile(HexTile hexTile) => HexTile = hexTile;
        public void SetNeighbours(CustomTile[] neighbours) => Neighbours = neighbours;
        public void SetParent(CustomTile parent) => Parent = parent;

        public void SetCost(float cost) => Cost = cost;

        /// <summary>
        /// Set the height of this tile of the terrain
        /// </summary>
        /// <param name="height"></param>
        public void SetHeight(float height) => TerrainHeight = height;


    }
}
