//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        13.06.24
// Date last edited:    15.07.24
// References:          https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
//////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace Meatwagon
{
    // Handles all the NavTiles within the scene and can be used to calculate the paths between them.
    public class NavController : MonoBehaviour
    {
        // Returns a list of NavTiles representing the shortest path from the start (first) tile to the goal (final) tile - returns null if no valid paths exist between the two.
        public List<NavTile> GetShortestPathBetweenTiles(NavTile startTile, NavTile goalTile)
        {
            if (startTile == null)
            {
                Debug.LogError("The startTile argument cannot be null.");
                return null;
            }
            if (goalTile == null)
            {
                Debug.LogError("The goalTile argument cannot be null.");
                return null;
            }
            if (startTile == goalTile)
            {
                Debug.LogError("The startTile argument cannot be the same tile as the goalTile.");
                return null;
            }

            CalculateShortestTileDistancesFromOrigin(startTile);

            // If the goal tile has a distance value equal to infinity, it could not be reached by any paths beginning from the starting tile.
            if (goalTile.DijkstraShortestDistance == NavTile.DijkstraInfiniteDistance)
            {
                return null;
            }



            List<NavTile> shortestPath = new List<NavTile>();
            NavTile shortestPathTile = goalTile;

            int pathLength = goalTile.DijkstraShortestDistance;
            Debug.Log("Path length is: " + pathLength);

            for (int i = pathLength - 1; i >= 0; i--)
            {
                shortestPath.Insert(0, shortestPathTile);

                if (shortestPathTile == startTile)
                {
                    break;
                }

                NavTile nextShortestPathTile = null;
                foreach (NavTile connectedTile in shortestPathTile.GetConnectedTiles())
                {
                    if (nextShortestPathTile == null && !connectedTile.IsInhabited)
                    {
                        nextShortestPathTile = connectedTile;
                    }
                    else if (connectedTile.DijkstraShortestDistance < nextShortestPathTile.DijkstraShortestDistance)
                    {
                        nextShortestPathTile = connectedTile;
                    }
                }

                shortestPathTile = nextShortestPathTile;
            }

            return shortestPath;







            //NavTile shortestPathTile = goalTile;
            //for(int i = 0; i < 999; ++i)
            //{
            //    shortestPath.Insert(0, shortestPathTile);

            //    if (shortestPathTile == startTile)
            //        break;

            //    NavTile nextShortestPathTile = null;
            //    foreach (NavTile adjacentTile in shortestPathTile.GetUninhabitedConnectedTiles())
            //    {
            //        if (nextShortestPathTile == null || adjacentTile.DijkstraShortestDistance < nextShortestPathTile.DijkstraShortestDistance)
            //        {
            //            nextShortestPathTile = adjacentTile;
            //        }
            //    }
            //    shortestPathTile = nextShortestPathTile;
            //}


        }

        public NavTile GetNavTileWithMouseOver()
        {
            foreach (NavTile tile in _navTiles)
            {
                if (tile.IsMouseOver())
                {
                    return tile;
                }
            }

            return null;
        }

        public void ResetAllTilesToDefaultSelectedState()
        {
            foreach (NavTile tile in _navTiles)
            {
                tile.SetSelectedState(NavTile.SelectedState.Default);
            }
        }

        public void HighlightTilesInMovementRange(NavTile originTile, int range)
        {
            CalculateShortestTileDistancesFromOrigin(originTile);

            foreach (NavTile navTile in _navTiles)
            {
                if (navTile.DijkstraShortestDistance <= range && !navTile.IsInhabited)
                {
                    navTile.SetSelectedState(NavTile.SelectedState.Highlighted);
                }
            }
        }


        private List<NavTile> _navTiles;

        private List<NavTile> FindAllNavTilesInScene()
        {
            List<NavTile> navTiles = new List<NavTile>();
            foreach (NavTile tile in GameObject.FindObjectsOfType<NavTile>())
            {
                navTiles.Add(tile);
            }

            return navTiles;
        }

        private void Start()
        {
            _navTiles = FindAllNavTilesInScene();
        }

        // Updates the DijkstraShortestDistance value of every NavTile to reflect its pathfinding distance from the originTile using Dijkstra's algorithm.
        private void CalculateShortestTileDistancesFromOrigin(NavTile originTile)
        {
            if (originTile == null)
            {
                Debug.LogWarning("The originTile argument cannot be null.");
                return;
            }

            // Create a set of unvisited tiles, giving the start tile an initial distance of 0 and all others a distance of infinity.
            List<NavTile> unvisitedTiles = new List<NavTile>();
            for (int i = 0; i < _navTiles.Count; ++i)
            {
                if (_navTiles[i] == originTile)
                {
                    _navTiles[i].DijkstraShortestDistance = 0;
                }
                else
                {
                    _navTiles[i].DijkstraShortestDistance = NavTile.DijkstraInfiniteDistance;
                }

                // Only the starting tile can be added to the set if it's inhabited, all other inhabited tiles are ignored.
                //if (_navTiles[i] == originTile || !_navTiles[i].IsInhabited)
                //{
                    unvisitedTiles.Add(_navTiles[i]);
                //}
            }

            // The index of the tile with the smallest distance value within the unvisted set.
            int shortestPathTileIndex;
            while (true)
            {
                // If the unvisited set is empty, the algorithm is complete.
                if (unvisitedTiles.Any() == false)
                {
                    return;
                }
                // Else if all the remaining distances in the unvisited set are equal to infinity, no further tiles can be reached from the origin tile and the algorithm is complete.
                bool allDistancesAreInfinite = true;
                foreach (NavTile tile in unvisitedTiles)
                {
                    if (tile.DijkstraShortestDistance != NavTile.DijkstraInfiniteDistance)
                    {
                        allDistancesAreInfinite = false;
                        break;
                    }
                }
                if (allDistancesAreInfinite)
                {
                    return;
                }

                // Get the node in the unvisited set with the shortest distance.
                shortestPathTileIndex = 0;
                for (int i = 0; i < unvisitedTiles.Count; i++)
                {
                    if (unvisitedTiles[i].DijkstraShortestDistance < unvisitedTiles[shortestPathTileIndex].DijkstraShortestDistance)
                    {
                        shortestPathTileIndex = i;
                    }
                }


                // Compares the distance values of the unvisited neighbours with the distance if crossed into from the current tile and updates their distance value if the latter is smaller. 
                foreach (NavTile connectedTile in unvisitedTiles[shortestPathTileIndex].GetConnectedTiles())
                {
                    if (!connectedTile.IsInhabited && unvisitedTiles[shortestPathTileIndex].DijkstraShortestDistance + 1 < connectedTile.DijkstraShortestDistance)
                    {
                        connectedTile.DijkstraShortestDistance = unvisitedTiles[shortestPathTileIndex].DijkstraShortestDistance + 1;
                    }
                }


                // Removes the current tile from the unvisited set.
                unvisitedTiles.Remove(unvisitedTiles[shortestPathTileIndex]);
            }
        }
    }
}
