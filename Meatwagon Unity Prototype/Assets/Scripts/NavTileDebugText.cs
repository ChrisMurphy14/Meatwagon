//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        15.06.24
// Date last edited:    15.06.24
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

            _textMeshPro.text = _parentTile.TraversalCost.ToString(); 
        }
    }
}