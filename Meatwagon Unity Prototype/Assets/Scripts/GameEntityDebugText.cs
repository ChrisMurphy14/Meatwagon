//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        16.07.24
// Date last edited:    19.07.24
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Meatwagon
{
    // A TMP object that displays debug data for the associated GameEntity during runtime.
    [RequireComponent(typeof(TextMeshPro))]
    public class GameEntityDebugText : MonoBehaviour
    {
        private GameEntity _parentEntity;
        private TextMeshPro _textMeshPro;

        private void Start()
        {
            _parentEntity = this.transform.parent.GetComponent<GameEntity>();
            _textMeshPro = GetComponent<TextMeshPro>();
        }

        private void Update()
        {
            _textMeshPro.text = _parentEntity.RemainingTurnActions.ToString() + " AP";
        }
    }
}
