//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        16.07.24
// Date last edited:    16.07.24
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Meatwagon
{    
    // A NavTile derivative which automatically connects with other NavTiles that have the same tag and are within the specified range.
    public class AutoConnectNavTile : NavTile
    {
        // The radius of the area around this tile within which it will connect to other tiles when the scene starts.
        public float AdjacentTileConnectionRadius = 1.5f;


        protected void Start()
        {
            ConnectToAdjacentTiles(AdjacentTileConnectionRadius);
        }

        protected void ConnectToAdjacentTiles(float maxDistance)
        {
            ConnectedTiles = new List<NavTile>();
            foreach (NavTile tile in GameObject.FindObjectsOfType<NavTile>())
            {
                if (tile != this && (tile.transform.position - this.transform.position).magnitude <= maxDistance)
                {
                    ConnectedTiles.Add(tile);
                }
            }
        }
    }
}
