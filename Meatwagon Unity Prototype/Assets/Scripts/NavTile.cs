//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        13.06.24
// Date last edited:    13.06.24
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

namespace Meatwagon
{
    // A tile within the scene which a character can inhabit/navigate to and from.
    [RequireComponent(typeof(Transform))]
    public class NavTile : MonoBehaviour
    {
        public List<NavTile> ConnectedTiles;



        private void OnDrawGizmos()
        {
            Gizmos.color = UnityEngine.Color.yellow;
            Gizmos.DrawWireCube(this.transform.position, Vector3.one);

            Gizmos.color = UnityEngine.Color.blue;
            foreach (NavTile connectedTile in ConnectedTiles)
            {
                Gizmos.DrawLine(this.transform.position, connectedTile.GetComponent<Transform>().position);
            }
        }
    }


    // Contains data about a one-way connection between two NavTiles.
    //public class NavTileConnection
    //{
    //    public NavTile FromTile;
    //    public NavTile TowardsTile;
    //    //public int Cost;

    //    public void DrawConnectionGizmo(Color color)
    //    {
    //        Gizmos.color = color;
    //        Gizmos.DrawLine(FromTile.GetComponent<Transform>().position, TowardsTile.GetComponent<Transform>().position);
    //    }
    //}
}
