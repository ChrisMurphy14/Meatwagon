//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        15.06.24
// Date last edited:    15.07.24
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Meatwagon
{
    // A TMP object that displays debug data for the associated NavTile during runtime.
    [RequireComponent(typeof(TextMeshPro))]
    public class NavTileDebugText : MonoBehaviour
    {       
        private NavTile _parentTile;
        private TextMeshPro _textMeshPro;

        private void Start()
        {
            _parentTile = this.transform.parent.GetComponent<NavTile>();
            _textMeshPro = GetComponent<TextMeshPro>();

            // DEBUG
            _textMeshPro.text = _parentTile.name;
        }

        private void Update()
        {
            //_textMeshPro.text = _parentTile.DijkstraShortestDistance.ToString();
        }
    }
}
