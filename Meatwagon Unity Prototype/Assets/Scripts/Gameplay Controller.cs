//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        03.07.24
// Date last edited:    03.07.24
//////////////////////////////////////////////////
using Meatwagon;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Controls the high-level flow of gameplay within the battle scene.
public class GameplayController : MonoBehaviour
{
    private List<Vehicle> _vehicles;

    private void Start()
    {
        _vehicles = new List<Vehicle>();
        foreach (Vehicle vehicle in GameObject.FindObjectsOfType<Vehicle>())
        {
            vehicle.OnLeftClicked.AddListener(OnVehicleLeftClicked);
            _vehicles.Add(vehicle);
            Debug.Log("Vehicle " + vehicle.name + " added.");
        }
    }

    private void Update()
    {
    }

    private void OnVehicleLeftClicked(Vehicle clickedVehicle)
    {
        foreach(Vehicle vehicle in _vehicles)
        {
            vehicle.IsSelected = false;
        }

        Debug.Log("Vehicle " + clickedVehicle.name + " was just left-clicked.");
        clickedVehicle.IsSelected = true;
    }
}