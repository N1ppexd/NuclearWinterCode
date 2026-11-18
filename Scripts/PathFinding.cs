using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace NuclearWinter
{
    public static class PathFinding
    {
        public static bool SearchPath(CustomTile a, CustomTile b, CustomTile[,] grid, float minPathHeight, float maxPathHeight, out List<CustomTile> path, List<CustomTile> usedTiles = null, List<CustomTile> roomTiles = null)
        {
            //breadth first search
            Queue<CustomTile> queue = new Queue<CustomTile>();

            //set all tiles to unvisited
            foreach (var tile in grid)
            {
                tile.Visited = false;

                if(roomTiles != null)
                {
                    if (roomTiles.Contains(tile))
                    {
                        if (tile != a && tile != b)
                        {
                            //this makes it so that the path doesnt block the neighbors of the other rooms...
                            SetNeighboursVisited(tile);
                        }
                    }
                }
            }


            //Sets the neighbors in y axis up and down to be visited in a and b,
            //so that the path has to travel from the sides
            SetNeighboursVisited(a, Axis.Y);
            SetNeighboursVisited(b, Axis.Y);

            a.Visited = true;
            queue.Enqueue(a);

            //making breadth first search

            while (queue.Count > 0)
            {
                CustomTile current = queue.Dequeue();

                if (current == b)
                {
                    Debug.Log("Found path!");
                    path = GetPath(a, b);
                    return true;
                }

                foreach (var neighbour in current.Neighbours)
                {
                    if (neighbour != null)
                    {
                        //cannot use same tiles twice
                        if (!neighbour.Visited && ((usedTiles != null && !usedTiles.Contains(neighbour)) || usedTiles == null) && neighbour.TerrainHeight >= minPathHeight && neighbour.TerrainHeight < maxPathHeight)
                        {
                            neighbour.Visited = true;

                            if (roomTiles!= null && roomTiles.Contains(neighbour))
                            {
                                if (neighbour != a && neighbour != b)
                                {
                                    Debug.Log("neighbour is room tile! Cannot pass through it");
                                    continue;
                                }
                            }

                            neighbour.SetParent(current);
                            queue.Enqueue(neighbour);
                        }
                    }
                    else
                    {
                        Debug.Log("neighbour is null!");
                    }
                }
            }

            queue.Clear();
            path = null;
            return false;
        }


        /// <summary>
        /// Sets all of the neighbours of a node visited
        /// </summary>
        /// <param name="tile"></param>
        public static void SetNeighboursVisited(CustomTile tile)
        {
            foreach (var neighborTile in tile.Neighbours)
            {
                if (neighborTile != null)
                {
                    neighborTile.Visited = true;
                }
            }
        }

        /// <summary>
        /// Set the neighbours in y axis visited
        /// </summary>
        /// <param name="tile"></param>
        public static void SetNeighboursVisited(CustomTile tile, Axis axis)
        {
            foreach (var neighbor in tile.Neighbours)
            {
                if (neighbor != null)
                {
                    switch (axis)
                    {
                        case Axis.X:
                            if (neighbor.Index.x != tile.Index.x)
                            {
                                neighbor.Visited = true;
                            }
                            break;
                        case Axis.Z:
                            if (neighbor.Index.y != tile.Index.y)
                            {
                                neighbor.Visited = true;
                            }
                            break;

                    }
                }
            }
        }

        /// <summary>
        /// Get the path from a to b
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static List<CustomTile> GetPath(CustomTile a, CustomTile b)
        {
            List<CustomTile> path = new List<CustomTile>();

            CustomTile current = b;


            while (current != a)
            {
                if (path.Contains(current))
                {
                    Debug.Log("Path contains current! " + current.Position);
                    break;
                }
                path.Add(current);
                current = current.Parent;
            }
            path.Add(a);

            path.Reverse();

            return path;
        }




        /// <summary>
        /// A star algorithm test
        /// Find the shortest path from <paramref name="a"/> to <paramref name="b"/>
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool AStar(CustomTile a, CustomTile b, float minPathHeight, float maxPathHeight, CustomTile[,] grid, out List<CustomTile> path, List<CustomTile> usedTiles = null, List<CustomTile> roomTiles = null)
        {
            CustomTile startNode = a;
            CustomTile endNode = b;

            foreach (var tile in grid)
            {
                if (tile != null)
                {
                    tile.Visited = false;
                }
            }

            if (!endNode.Walkable
                || endNode.TerrainHeight < minPathHeight
                || endNode.TerrainHeight > maxPathHeight
                || (usedTiles != null && usedTiles.Contains(endNode)))
            {
                path = null;
                Debug.Log("cant find path to that target");
                return false;
            }


            float startNodeCost = 0;
            //float endNodeCost = 0;

            startNode.GCost = startNodeCost;
            //endNode.GCost = endNodeCost;


            List<CustomTile> openSet = new List<CustomTile>();
            HashSet<CustomTile> closedSet = new HashSet<CustomTile>();

            bool foundPath = false;

            //add the start node to the open set
            openSet.Add(startNode);

            while(openSet.Count > 0)
            {
                //find lowes f cost neighbour

                //get the currentNode
                CustomTile currentNode = openSet[0];

                //f(n) = g(n) + h(n)
                for(int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost 
                        || (openSet[i].FCost == currentNode.FCost 
                        && openSet[i].HeuristicCost < currentNode.HeuristicCost))
                    {
                        currentNode = openSet[i];
                    }
                }
                //remove the current node from the open set and add it to the closed set
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);

                if(currentNode == endNode)
                {
                    foundPath = true;
                    break;
                }


                //loop through the neighbours in the current tile and check if they are walkable
                foreach(CustomTile neighbour in currentNode.Neighbours)
                {
                    if (neighbour == null)
                    {
                        continue;
                    }

                    if (closedSet.Contains(neighbour) || !neighbour.Walkable 
                        || neighbour.TerrainHeight < minPathHeight 
                        || neighbour.TerrainHeight > maxPathHeight
                        || (usedTiles != null && usedTiles.Contains(neighbour)))
                    {
                        continue;
                    }

                    //path cannot pass through "room" or more like target tiles in the middle of a path...
                    if(roomTiles != null && roomTiles.Contains(neighbour))
                    {
                        if (neighbour != a && neighbour != b)
                        {
                            Debug.Log("neighbour is room tile! Cannot pass through it");
                            continue;
                        }
                    }

                    float cost = currentNode.GCost + neighbour.Cost;

                    if (cost < neighbour.GCost || !openSet.Contains(neighbour))
                    {
                        //set the neighbour g cost to the cost
                        neighbour.GCost = cost;
                        //set the neighbour h cost based on distance THIS IS BAD!
                        neighbour.HeuristicCost = HexagonOperations.HexDistance(neighbour, endNode);
                        //set the parent of the neighbour to the current node
                        neighbour.SetParent(currentNode);

                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }

            if(foundPath)
                path = GetPath(startNode, endNode);
            else path = null;

            return foundPath;
        }
    }
    public enum Axis
    {
        X,
        Y,
        Z
    }
}
