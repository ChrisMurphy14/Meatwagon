//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        13.06.24
// Date last edited:    13.06.24
// References:          https://en.wikipedia.org/wiki/Dijkstra%27s_algorithm
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Meatwagon
{
    // Keeps a list of all the NavTiles in the scene and is used to query whether valid paths between them exist or not.
    public class NavController : MonoBehaviour
    {
        public List<NavTile> NavTiles;

        public void CalculateShortestPathToTile(NavTile startTile, NavTile goalTile)
        {



            //bool startTileInList = false;
            //bool goalTileInList = false;
            //foreach (NavTile tile in NavTiles)
            //{
            //    if (!startTileInList && tile == startTile)
            //    {
            //        startTileInList = true;
            //    }
            //    if (!goalTileInList && tile == goalTileInList)
            //    {
            //        goalTileInList = true;
            //    }
            //}
            //if (!startTileInList || !goalTileInList)
            //{
            //    Debug.LogWarning("The start and/or goal tiles for calculating the shortest path between don't exist in the NavController's NavTiles list.");
            //    return;
            //}

            int infinity = 9999;

            List<DijkstraNode> unvisitedSet = new List<DijkstraNode>();
            for (int i = 0; i < NavTiles.Count; ++i)
            {
                DijkstraNode node;
                node.Tile = NavTiles[i];
                if (NavTiles[i] == startTile)
                {
                    node.ShortestDistFromStart = 0; // Set starting tile distance value to 0;
                }
                else
                {
                    node.ShortestDistFromStart = infinity; // Counts as the 'infinite distance' value in this case for Dijkstra's algorithm.
                }

                unvisitedSet.Add(node);
                Debug.Log("Added node to unvisited set with name " + node.Tile.name + " and starting distance value of " + node.ShortestDistFromStart);
            }

            // Get the node from the unvisited set with the shortest distance from the starting node.
            int currentNodeIndex = 0;
            for (int i = 0; i < unvisitedSet.Count; ++i)
            {
                if (unvisitedSet[i].ShortestDistFromStart != infinity && unvisitedSet[i].ShortestDistFromStart < unvisitedSet[currentNodeIndex].ShortestDistFromStart)
                {
                    currentNodeIndex = 1;
                }
            }




                //List<NavTile> unvisitedSet = NavTiles;

                //int[] tileDistancesFromStart = new int[NavTiles.Count];
                //for (int i = 0; i < NavTiles.Count; i++)
                //{
                //    if (NavTiles[i] == startTile)
                //    {
                //        tileDistancesFromStart[i] = 0; // Set starting tile distance value to 0;
                //    }
                //    else
                //    {
                //        tileDistancesFromStart[i] = -1; // Counts as the 'infinite distance' value in this case for Dijkstra's algorithm.
                //    }
                //}

                //int shortestUnvisitedDistanceTileIndex = -1;
                //for(int i = 0; i > unvisitedSet.Count; ++i)
                //{
                //    if()
                //}

            }

        private void Start()
        {
            CalculateShortestPathToTile(NavTiles[0], NavTiles[3]); // DEBUG!
        }
    }

    public struct DijkstraNode
    {
        public NavTile Tile;
        public int ShortestDistFromStart;
    }
}
