//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        13.06.24
// Date last edited:    14.06.24
// References:          https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
//////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace Meatwagon
{
    // Keeps a list of all the NavTiles in the scene and is used to query whether valid paths between them exist or not.
    public class NavController : MonoBehaviour
    {
        public List<NavTile> NavTiles;

        public bool DoesNavTileExistInList(NavTile tile)
        {
            foreach (NavTile listTile in NavTiles)
            {
                if (listTile == tile)
                {
                    return true;
                }
            }

            return false;
        }


        public List<NavTile> CalculateShortestPathToTile(NavTile startTile, NavTile goalTile)
        {
            if (startTile == goalTile)
            {
                Debug.LogWarning("The startTile argument cannot be the same tile as the goalTile.");
                return null;
            }
            if (DoesNavTileExistInList(startTile) == false)
            {
                Debug.LogWarning("The startTile argument provided to the CalculateShortestPathToTile() function doesn't exist within the NavTiles list of the NavController.");
                return null;
            }
            if (DoesNavTileExistInList(goalTile) == false)
            {
                Debug.LogWarning("The goalTile argument provided to the CalculateShortestPathToTile() function doesn't exist within the NavTiles list of the NavController.");
                return null;
            }

            // Outcomes:
            // - Find a valid path between the two tiles and return the shortest path
            // - Find there is no valid path between the two tiles and return null/empty list?

            int infiniteDistance = 9999; // Used as the default value for tiles that haven't been visited.

            // Steps 1 + 2
            List<NavTile> unvisitedSet = new List<NavTile>();
            int currentNodeIndex = -1;
            for (int i = 0; i < NavTiles.Count; ++i)
            {
                if (NavTiles[i] == startTile)
                {
                    NavTiles[i].ShortestDistance = 0; // Set starting tile distance value to 0;
                    Debug.Log("Starting node index is: " + currentNodeIndex);
                }
                else
                {
                    NavTiles[i].ShortestDistance = infiniteDistance; // Counts as the 'infinite distance' value in this case for Dijkstra's algorithm.
                }

                unvisitedSet.Add(NavTiles[i]);
                Debug.Log("Added node to unvisited set with name " + NavTiles[i].name + " and starting distance value of " + NavTiles[i].ShortestDistance);
            }


            List<NavTile> shortestPath = new List<NavTile>();
            // Step 3
            while (shortestPath.Any() == false)
            {
                // If the unvisited set is empty and no path has been found, there's no valid path between the start and goal tiles.
                if (unvisitedSet.Any() == false)
                {
                    return null;
                }
                // Else if all the remaining distances in the unvisited set are equal to infinity, no further tiles can be reached and no valid path exists.
                bool allDistancesAreInfinite = true;
                foreach (NavTile tile in unvisitedSet)
                {
                    if (tile.ShortestDistance != infiniteDistance)
                    {
                        allDistancesAreInfinite = false;
                        break;
                    }
                }
                if (allDistancesAreInfinite)
                {
                    return null;
                }

                // Get the node in the unvisited set with the shortest distance.
                currentNodeIndex = 0;
                for (int i = 0; i < unvisitedSet.Count; i++)
                {
                    if (unvisitedSet[i].ShortestDistance < unvisitedSet[currentNodeIndex].ShortestDistance)
                    {
                        currentNodeIndex = i;
                    }
                }
                if (unvisitedSet[currentNodeIndex] == goalTile)
                {
                    shortestPath = GetShortestPathOnceCalculated(startTile, goalTile);
                    return shortestPath;
                }

                // Step 4
                foreach (NavTile adjacentTile in unvisitedSet[currentNodeIndex].ConnectedTiles)
                {
                    if (unvisitedSet[currentNodeIndex].ShortestDistance + 1 < adjacentTile.ShortestDistance)
                    {
                        adjacentTile.ShortestDistance = unvisitedSet[currentNodeIndex].ShortestDistance + 1;
                    }
                }

                // Step 5
                unvisitedSet.Remove(unvisitedSet[currentNodeIndex]);
            }

            return null;
        }



        private List<NavTile> GetShortestPathOnceCalculated(NavTile startTile, NavTile goalTile)
        {
            List<NavTile> shortestPath = new List<NavTile>();
            NavTile shortestPathTile = goalTile;

            while (shortestPath.Any() == false || shortestPath[0] != startTile)
            {
                shortestPath.Insert(0, shortestPathTile);

                if (shortestPathTile == startTile)
                {
                    break;
                }

                NavTile nextShortestPathTile = shortestPathTile.ConnectedTiles[0];
                foreach (NavTile adjacentTile in shortestPathTile.ConnectedTiles)
                {
                    if (adjacentTile.ShortestDistance < nextShortestPathTile.ShortestDistance)
                    {
                        nextShortestPathTile = adjacentTile;
                    }
                }

                shortestPathTile = nextShortestPathTile;
            }

            return shortestPath;
        }
        private NavTile SelectedTile;


        private void Awake()
        {
            //foreach (NavTile tile in NavTiles)
            //{
            //    if (tile == null)
            //    {
            //        Debug.LogException(new NullReferenceException("An item in the NavTiles list equals null."));
            //    }
            //}
        }

        private void Start()
        {
            NavTiles = new List<NavTile>();
            foreach (NavTile tile in GameObject.FindObjectsOfType<NavTile>())
            {
                NavTiles.Add(tile);
            }

            //NavTile startTile = NavTiles[3];
            //NavTile goalTile = NavTiles[0];

            //List<NavTile> testPath = CalculateShortestPathToTile(startTile, goalTile); // DEBUG!
            //Debug.Log("The shortest path between tiles " + startTile.name + " and " + goalTile.name + " is:");
            //foreach (NavTile tile in testPath)
            //{
            //    Debug.Log(tile.name);
            //}

        }

        private void Update()
        {
            foreach (NavTile tile in NavTiles)
            {
                if (tile.IsMouseOver())
                {
                    if(Input.GetMouseButtonDown(0))
                    {
                        if (SelectedTile != null)
                        {
                            SelectedTile.SetSelectedState(NavTile.SelectedState.Default);
                        }

                        
                        tile.SetSelectedState(NavTile.SelectedState.Selected);
                        SelectedTile = tile;
                    }

                    if (tile != SelectedTile)
                    {
                        

                        List<NavTile> tilePath = CalculateShortestPathToTile(SelectedTile, tile);
                        if(tilePath != null)
                        {
                            foreach(NavTile pathTile in tilePath)
                            {
                                if(pathTile != SelectedTile)
                                {
                                    pathTile.SetSelectedState(NavTile.SelectedState.Highlighted);
                                }
                            }
                        }
                    }
                }
                else if(tile != SelectedTile)
                {
                    tile.SetSelectedState(NavTile.SelectedState.Default);
                }
            }
        }
    }
}
