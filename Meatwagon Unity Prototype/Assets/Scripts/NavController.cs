//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        13.06.24
// Date last edited:    18.07.24
// References:          https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
//////////////////////////////////////////////////
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Tilemaps;


namespace Meatwagon
{
    // Handles a set of NavTiles within the scene and can be used to calculate the paths between them.
    public class NavController : MonoBehaviour
    {
        // The NavTiles which this specific controller handles - must be manually allocated in the editor.
        public List<NavTile> NavTiles;

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
            if (!IsNavTileInNavControllerList(startTile))
            {
                Debug.LogError("The startTile provided is not present in NavTiles list of the NavController.");
                return null;
            }
            if (!IsNavTileInNavControllerList(goalTile))
            {
                Debug.LogError("The goalTile provided is not present in NavTiles list of the NavController.");
                return null;
            }

            CalculateShortestTileDistancesFromOrigin(startTile);

            // If the goal tile has a distance value equal to infinity, it could not be reached by any paths beginning from the starting tile.
            if (goalTile.DijkstraShortestDistance == NavTile.DijkstraInfiniteDistance)
            {
                return null;
            }

            // Creates a path list by starting from the goal tile and iterating backwards through each connected tile with the shortest Dijkstra distance value until reaching the start tile.
            List<NavTile> shortestPath = new List<NavTile>();
            NavTile shortestPathTile = goalTile;
            int pathLength = goalTile.DijkstraShortestDistance;
            while (true)
            {
                shortestPath.Insert(0, shortestPathTile);

                if (shortestPathTile == startTile)
                {
                    break;
                }

                NavTile nextShortestPathTile = null;
                foreach (NavTile connectedTile in shortestPathTile.GetConnectedTiles())
                {
                    if (nextShortestPathTile == null)
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
        }

        public NavTile GetNavTileWithMouseOver()
        {
            foreach (NavTile tile in NavTiles)
            {
                if (tile.IsMouseOver())
                {
                    return tile;
                }
            }

            return null;
        }

        // Returns whether the specified NavTile is present in the NavTiles list of the NavController.
        public bool IsNavTileInNavControllerList(NavTile queryTile)
        {            
            foreach (NavTile tile in NavTiles)
            {
                if (queryTile == tile)
                {
                    return true;
                }
            }

            return false;
        }

        public void ResetAllTilesToDefaultSelectedState()
        {
            foreach (NavTile tile in NavTiles)
            {
                tile.SetSelectedState(NavTile.SelectedState.Default);
            }
        }

        public void HighlightTilesInMovementRange(NavTile originTile, int range)
        {
            CalculateShortestTileDistancesFromOrigin(originTile);

            foreach (NavTile navTile in NavTiles)
            {
                if (navTile.DijkstraShortestDistance <= range)
                {
                    navTile.SetSelectedState(NavTile.SelectedState.Highlighted);
                }
            }
        }
                

        // Updates the DijkstraShortestDistance value of every NavTile to reflect its pathfinding distance from the originTile using Dijkstra's algorithm (note - automatically ignores if the originTile is inhabited.)
        private void CalculateShortestTileDistancesFromOrigin(NavTile originTile)
        {
            if (originTile == null)
            {
                Debug.LogError("The originTile argument cannot be null.");
                return;
            }
            if(!IsNavTileInNavControllerList(originTile))
            {
                Debug.LogError("The originTile provided is not present in NavTiles list of the NavController.");
                return;
            }

            // Create a set of unvisited tiles, giving the start tile an initial distance of 0 and all others a distance of infinity.
            List<NavTile> unvisitedTiles = new List<NavTile>();
            for (int i = 0; i < NavTiles.Count; ++i)
            {
                if (NavTiles[i] == originTile)
                {
                    NavTiles[i].DijkstraShortestDistance = 0;
                }
                else
                {
                    NavTiles[i].DijkstraShortestDistance = NavTile.DijkstraInfiniteDistance;
                }
                unvisitedTiles.Add(NavTiles[i]);
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

                // Compares the distance values of the unvisited neighbours with the distance if crossed into from the current tile and updates their distance value if the latter is smaller - this is also the point at which inhabited tiles are ignored so that their distance always remains infinite and therefore excluded from pathfinding.
                foreach (NavTile connectedTile in unvisitedTiles[shortestPathTileIndex].GetConnectedTiles())
                {
                    if (!connectedTile.IsInhabited && unvisitedTiles[shortestPathTileIndex].DijkstraShortestDistance + unvisitedTiles[shortestPathTileIndex].TraversalCost < connectedTile.DijkstraShortestDistance)
                    {
                        connectedTile.DijkstraShortestDistance = unvisitedTiles[shortestPathTileIndex].DijkstraShortestDistance + unvisitedTiles[shortestPathTileIndex].TraversalCost;
                    }
                }

                // Removes the current tile from the unvisited set.
                unvisitedTiles.Remove(unvisitedTiles[shortestPathTileIndex]);
            }
        }
    }
}
