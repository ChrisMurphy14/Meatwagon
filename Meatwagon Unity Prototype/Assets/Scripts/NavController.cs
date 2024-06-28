//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        13.06.24
// Date last edited:    28.06.24
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
    // Handles all the _navTiles within the scene and can be used to calculate the shortest paths between them.
    public class NavController : MonoBehaviour
    {
        // Returns a list of NavTiles representing the shortist path from the start (first) tile to the goal (final) tile - returns null if no valid paths exist between the two.
        public List<NavTile> GetShortestPathBetweenTiles(NavTile startTile, NavTile goalTile)
        {
            if (startTile == goalTile)
            {
                Debug.LogWarning("The startTile argument cannot be the same tile as the goalTile.");
                return null;
            }
            if (DoesNavTileExistInList(startTile) == false)
            {
                Debug.LogWarning("The startTile argument provided to the GetShortestPathBetweenTiles() function doesn't exist within the _navTiles list of the NavController.");
                return null;
            }
            if (DoesNavTileExistInList(goalTile) == false)
            {
                Debug.LogWarning("The goalTile argument provided to the GetShortestPathBetweenTiles() function doesn't exist within the _navTiles list of the NavController.");
                return null;
            }

            // Used as the default value for tiles that haven't been visited yet during the algorithm.
            int infiniteDistance = 9999;

            // Create a set of unvisited tiles, giving the start tile an initial distance of 0 and all others a distance of infinity.
            List<NavTile> unvisitedTiles = new List<NavTile>();
            for (int i = 0; i < _navTiles.Count; ++i)
            {
                if (_navTiles[i] == startTile)
                {
                    _navTiles[i].DijkstraShortestDistance = 0;
                }
                else
                {
                    _navTiles[i].DijkstraShortestDistance = infiniteDistance;
                }
                unvisitedTiles.Add(_navTiles[i]);
            }

            List<NavTile> shortestPath = new List<NavTile>();
            // The index of the tile with the smallest distance value within the unvisted set.
            int shortestPathTileIndex;
            while (shortestPath.Any() == false)
            {
                // If the unvisited set is empty and no path has been found, there's no valid path between the start and goal tiles.
                if (unvisitedTiles.Any() == false)
                {
                    return null;
                }
                // Else if all the remaining distances in the unvisited set are equal to infinity, no further tiles can be reached and no valid path exists.
                bool allDistancesAreInfinite = true;
                foreach (NavTile tile in unvisitedTiles)
                {
                    if (tile.DijkstraShortestDistance != infiniteDistance)
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
                shortestPathTileIndex = 0;
                for (int i = 0; i < unvisitedTiles.Count; i++)
                {
                    if (unvisitedTiles[i].DijkstraShortestDistance < unvisitedTiles[shortestPathTileIndex].DijkstraShortestDistance)
                    {
                        shortestPathTileIndex = i;
                    }
                }

                // If the goal tile currently has the smallest distance, the shortest path between it and the starting tile has been found.
                if (unvisitedTiles[shortestPathTileIndex] == goalTile)
                {
                    shortestPath = GetShortestPathOnceCalculated(startTile, goalTile);
                    return shortestPath;
                }

                // Compares the distance values of the unvisited neighbours with the distance if crossed into from the current tile and updates their distance value if the latter is smaller. 
                foreach (NavTile connectedTile in unvisitedTiles[shortestPathTileIndex].GetConnectedTiles())
                {
                    if (unvisitedTiles[shortestPathTileIndex].DijkstraShortestDistance + connectedTile.TraversalCost < connectedTile.DijkstraShortestDistance)
                    {
                        connectedTile.DijkstraShortestDistance = unvisitedTiles[shortestPathTileIndex].DijkstraShortestDistance + connectedTile.TraversalCost;
                    }
                }

                // Removes the current tile from the unvisited set.
                unvisitedTiles.Remove(unvisitedTiles[shortestPathTileIndex]);
            }

            return null;
        }

        public bool DoesNavTileExistInList(NavTile tile)
        {
            foreach (NavTile listTile in _navTiles)
            {
                if (listTile == tile)
                {
                    return true;
                }
            }

            return false;
        }

        private List<NavTile> _navTiles;
        private NavTile _selectedStartTile;
        private NavTile _selectedGoalTile;

        private List<NavTile> FindAllNavTilesInScene()
        {
            List<NavTile> navTiles = new List<NavTile>();
            foreach (NavTile tile in GameObject.FindObjectsOfType<NavTile>())
            {
                navTiles.Add(tile);
            }

            return navTiles;
        }

        // Returns an ordered list from the start tile to the goal tile representing the shortest path between the two once all of the appropriate shortest distance values have been calculated via Dijkstra's Algorithm.
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

                NavTile nextShortestPathTile = shortestPathTile.GetConnectedTiles()[0];
                foreach (NavTile adjacentTile in shortestPathTile.GetConnectedTiles())
                {
                    if (adjacentTile.DijkstraShortestDistance < nextShortestPathTile.DijkstraShortestDistance)
                    {
                        nextShortestPathTile = adjacentTile;
                    }
                }

                shortestPathTile = nextShortestPathTile;
            }

            // DEBUG
            int pathTraversalCostTotal = 0;
            // Skips the cost of the first tile - traversal cost only matters when moving into each tile, not out of.
            for (int i = 1; i < shortestPath.Count; ++i)
            {
                pathTraversalCostTotal += shortestPath[i].TraversalCost;
            }
            Debug.Log("Starting tile is: " + shortestPath[0] + ", goal tile is: " + shortestPath[shortestPath.Count - 1]);
            Debug.Log("The total traversal cost of the path is: " + pathTraversalCostTotal);


            return shortestPath;
        }

        private void Start()
        {
            _navTiles = FindAllNavTilesInScene();
        }

        private void Update()
        {
            UpdateSeletedTiles();
        }

        private void UpdateSeletedTiles()
        {
            bool selectedTilesChanged = false;
            if (Input.GetMouseButtonDown(0))
            {
                bool mouseNotOverAnyTiles = true;

                // Allows for two tiles to be selected at a time via mouseclick.
                foreach (NavTile tile in _navTiles)
                {
                    if (tile.IsMouseOver())
                    {
                        mouseNotOverAnyTiles = false;

                        if (tile != _selectedStartTile && tile != _selectedGoalTile)
                        {
                            if(_selectedStartTile == null)
                            {
                                _selectedStartTile = tile;
                                _selectedStartTile.SetSelectedState(NavTile.SelectedState.Selected);

                                selectedTilesChanged = true;
                            }
                            else if (_selectedGoalTile == null)
                            {
                                _selectedGoalTile = tile;
                                _selectedGoalTile.SetSelectedState(NavTile.SelectedState.Selected);

                                selectedTilesChanged = true;
                            }            
                        }
                    }
                }

                if(mouseNotOverAnyTiles)
                {
                    ResetAllTilesToDefaultSelectedState();
                }

                // If the two selected tiles have changed, calculates and highlights the shortest valid path between them
                if (selectedTilesChanged && _selectedStartTile != null && _selectedGoalTile != null)
                {
                    // Resets all non-selected tiles to default.
                    foreach (NavTile tile in _navTiles)
                    {
                        if (tile.GetSelectedState() != NavTile.SelectedState.Selected)
                        {
                            tile.SetSelectedState(NavTile.SelectedState.Default);
                        }
                    }

                    List<NavTile> shortestPathBetweenSelected = GetShortestPathBetweenTiles(_selectedStartTile, _selectedGoalTile);
                    if (shortestPathBetweenSelected != null)
                    {
                        foreach (NavTile pathTile in shortestPathBetweenSelected)
                        {
                            if (pathTile.GetSelectedState() != NavTile.SelectedState.Selected)
                            {
                                pathTile.SetSelectedState(NavTile.SelectedState.Highlighted);
                            }
                        }
                    }

                }
            }
        }

        private void ResetAllTilesToDefaultSelectedState()
        {
            foreach (NavTile tile in _navTiles)
            {
                tile.SetSelectedState(NavTile.SelectedState.Default);
            }

            _selectedStartTile = null;
            _selectedGoalTile = null;
        }
    }
}
