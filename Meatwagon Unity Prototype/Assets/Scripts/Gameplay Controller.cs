//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        03.07.24
// Date last edited:    06.07.24
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Meatwagon
{
    // Controls the high-level flow of gameplay within the battle scene.
    public class GameplayController : MonoBehaviour
    {
        public Button DeselectVehicleButton;
        public Button MoveVehicleButton;
        public NavController SceneNavController;

        private enum GameplayState
        {
            NothingSelected,
            VehicleSelected,
            ChoosingVehicleMovement,
            ConfirmVehicleMovment
        }

        private Vehicle SelectedVehicle;
        private GameplayState _gameplayState;
        //private List<Vehicle> _vehicles;

        private void Awake()
        {
            _gameplayState = GameplayState.NothingSelected;
        }

        private void Start()
        {
            DeselectVehicleButton.onClick.AddListener(DeselectVehicle);
            MoveVehicleButton.onClick.AddListener(DisplaySelectedVehicleMovementRange);
            SetVehicleButtonsActiveState(false);

            //_vehicles = new List<Vehicle>();
            foreach (Vehicle vehicle in GameObject.FindObjectsOfType<Vehicle>())
            {
                vehicle.OnLeftClicked.AddListener(SelectVehicle);
                //_vehicles.Add(vehicle);
            }
        }

        private void Update()
        {
            //if(_gameplayState == GameplayState.ChoosingVehicleMovement)
            //{
            //    if (Input.GetMouseButtonDown(0))
            //    {
            //        NavTile mouseOvertile = SceneNavController.GetNavTileWithMouseOver();                    
            //    }
            //}
        }

        private void SetVehicleButtonsActiveState(bool areActive)
        {
            DeselectVehicleButton.gameObject.SetActive(areActive);
            MoveVehicleButton.gameObject.SetActive(areActive);
        }

        private void SelectVehicle(Vehicle vehicle)
        {
            if (SelectedVehicle == null)
            {
                SelectedVehicle = vehicle;
                SelectedVehicle.IsSelected = true;

                SetVehicleButtonsActiveState(true);

                _gameplayState = GameplayState.VehicleSelected;
            }
        }

        private void DeselectVehicle()
        {
            if (SelectedVehicle != null)
            {
                SelectedVehicle.IsSelected = false;
                SelectedVehicle = null;

                SetVehicleButtonsActiveState(false);

                SceneNavController.ResetAllTilesToDefaultSelectedState();

                _gameplayState = GameplayState.NothingSelected;
            }
        }

        private void DisplaySelectedVehicleMovementRange()
        {
            SceneNavController.HighlightTilesInMovementRange(SelectedVehicle.CurrentNavTile, SelectedVehicle.Speed);

            _gameplayState = GameplayState.ChoosingVehicleMovement;
        }
    }
}
