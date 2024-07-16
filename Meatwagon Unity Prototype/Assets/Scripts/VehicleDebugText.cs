//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        16.07.24
// Date last edited:    16.07.24
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Meatwagon
{
    // A TMP object that displays debug data for the associated Vehicle during runtime.
    [RequireComponent(typeof(TextMeshPro))]
    public class VehicleDebugText : MonoBehaviour
    {
        private Vehicle _parentVehicle;
        private TextMeshPro _textMeshPro;

        private void Start()
        {
            _parentVehicle = this.transform.parent.GetComponent<Vehicle>();
            _textMeshPro = GetComponent<TextMeshPro>();
        }

        private void Update()
        {
            _textMeshPro.text = _parentVehicle.RemainingTurnActions.ToString() + " AP";
        }
    }
}
