//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        16.07.24
// Date last edited:    22.07.24
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


        protected override void Awake()
        {
            if (tag == "Untagged")
            {
                Debug.LogError("All instances of AutoConnectNavTile must have a distinctive tag in order to filter which of the other tiles in the scene to connect to.");
            }

            base.Awake();
        }

        protected void Start()
        {
            ConnectToAdjacentTiles(AdjacentTileConnectionRadius);
        }

        protected void ConnectToAdjacentTiles(float maxDistance)
        {
            ConnectedTiles = new List<NavTile>();
            foreach (NavTile tile in GameObject.FindObjectsOfType<NavTile>())
            {
                if (tile != this && tile.tag == this.tag && (tile.transform.position - this.transform.position).magnitude <= maxDistance)
                {
                    ConnectedTiles.Add(tile);
                }
            }
        }
    }
}
