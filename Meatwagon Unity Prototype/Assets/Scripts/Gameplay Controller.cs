//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        03.07.24
// Date last edited:    16.07.24
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


namespace Meatwagon
{
    // Controls the high-level flow of gameplay within the battle scene.
    public class GameplayController : MonoBehaviour
    {
        public Button DeselectVehicleButton;
        public Button DriveVehicleButton;
        public Button ConfirmDriveVehicleButton;
        public NavController RoadGridNavController;
        public TextMeshPro TurnCounterDisplay;        


        private enum PlayerActionType
        {
            None,
            Drive
        }

        private enum PlayerActionState
        {
            NothingSelected,
            EntitySelected,
            ActionSelected,
            ActionConfirming
        }

        private NavTile _selectedNavTile;
        private List<Vehicle> _vehicles;
        private Vehicle _selectedVehicle;
        private PlayerActionType _currentPlayerActionType;
        private PlayerActionState _currentPlayerActionState;
        private int turnCounter = -1;

        private void Awake()
        {
            _currentPlayerActionType = PlayerActionType.None;
            _currentPlayerActionState = PlayerActionState.NothingSelected;
        }

        private void Start()
        {
            DeselectVehicleButton.onClick.AddListener(DeselectVehicle);
            DriveVehicleButton.onClick.AddListener(DisplayDriveRange);
            ConfirmDriveVehicleButton.onClick.AddListener(DriveSelectedVehicleToSelectedTile);
            UpdateActionButtonsActiveStates();

            _vehicles = new List<Vehicle>();
            foreach (Vehicle vehicle in GameObject.FindObjectsOfType<Vehicle>())
            {
                vehicle.OnLeftClicked.AddListener(SelectVehicle);

                _vehicles.Add(vehicle);
            }

            StartNewTurn();
        }

        private void Update()
        {
            // DEBUG
            if(Input.GetKeyDown(KeyCode.Space))
            {
                StartNewTurn();
            }

            if (_currentPlayerActionType == PlayerActionType.Drive)
            {
                if (_currentPlayerActionState == PlayerActionState.ActionSelected || _currentPlayerActionState == PlayerActionState.ActionConfirming)
                {
                    // If the left mouse button is clicked on a tile within the Speed range of the selected vehicle, tries to find a valid path from the vehicle and displays it as selected tiles.
                    if (Input.GetMouseButtonDown(0))
                    {
                        NavTile mouseOvertile = RoadGridNavController.GetNavTileWithMouseOver();

                        if (mouseOvertile != null && mouseOvertile != _selectedVehicle.CurrentNavTile)
                        {
                            List<NavTile> pathFromSelectedVehicle = RoadGridNavController.GetShortestPathBetweenTiles(_selectedVehicle.CurrentNavTile, mouseOvertile);
                            if (pathFromSelectedVehicle != null && pathFromSelectedVehicle.Count <= _selectedVehicle.Speed + 1)
                            {
                                RoadGridNavController.ResetAllTilesToDefaultSelectedState();
                                RoadGridNavController.HighlightTilesInMovementRange(_selectedVehicle.CurrentNavTile, _selectedVehicle.Speed);
                                foreach (NavTile pathTile in pathFromSelectedVehicle)
                                {
                                    pathTile.SetSelectedState(NavTile.SelectedState.Selected);
                                }

                                _selectedNavTile = mouseOvertile;

                                _currentPlayerActionState = PlayerActionState.ActionConfirming;
                                UpdateActionButtonsActiveStates();
                            }
                        }
                    }
                }
            }
        }

        private void StartNewTurn()
        {
            DeselectVehicle();

            foreach(Vehicle vehicle in _vehicles)
            {
                vehicle.RemainingTurnActions = vehicle.ActionsPerTurn;
            }

            turnCounter++;
            TurnCounterDisplay.text = "Turn " + turnCounter;
        }

        // Updates which action buttons are active or deactivated based on the current action the player is attempting.
        private void UpdateActionButtonsActiveStates()
        {
            switch (_currentPlayerActionState)
            {
                case PlayerActionState.NothingSelected:
                    {
                        DeselectVehicleButton.gameObject.SetActive(false);

                        DriveVehicleButton.gameObject.SetActive(false);
                        ConfirmDriveVehicleButton.gameObject.SetActive(false);

                        break;
                    }
                case PlayerActionState.EntitySelected:
                    {
                        DeselectVehicleButton.gameObject.SetActive(true);

                        if (_selectedVehicle.RemainingTurnActions > 0)
                        {
                            DriveVehicleButton.gameObject.SetActive(true);
                        }
                        ConfirmDriveVehicleButton.gameObject.SetActive(false);

                        break;
                    }
                case PlayerActionState.ActionSelected:
                    {
                        if (_currentPlayerActionType == PlayerActionType.Drive)
                        {
                            DeselectVehicleButton.gameObject.SetActive(true);

                            DriveVehicleButton.gameObject.SetActive(false);
                            ConfirmDriveVehicleButton.gameObject.SetActive(false);
                        }

                        break;
                    }
                case PlayerActionState.ActionConfirming:
                    {
                        if (_currentPlayerActionType == PlayerActionType.Drive)
                        {
                            DeselectVehicleButton.gameObject.SetActive(true);

                            DriveVehicleButton.gameObject.SetActive(false);
                            ConfirmDriveVehicleButton.gameObject.SetActive(true);
                        }

                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        private void SelectVehicle(Vehicle vehicle)
        {
            if (_selectedVehicle == null)
            {
                _selectedVehicle = vehicle;
                _selectedVehicle.IsSelected = true;

                _currentPlayerActionState = PlayerActionState.EntitySelected;
                UpdateActionButtonsActiveStates();
            }
        }

        private void DeselectVehicle()
        {
            if (_selectedVehicle != null)
            {
                _selectedVehicle.IsSelected = false;
                _selectedVehicle = null;

                RoadGridNavController.ResetAllTilesToDefaultSelectedState();

                _currentPlayerActionType = PlayerActionType.None;
                _currentPlayerActionState = PlayerActionState.NothingSelected;
                UpdateActionButtonsActiveStates();
            }
        }

        private void DisplayDriveRange()
        {
            RoadGridNavController.HighlightTilesInMovementRange(_selectedVehicle.CurrentNavTile, _selectedVehicle.Speed);

            _currentPlayerActionType = PlayerActionType.Drive;
            _currentPlayerActionState = PlayerActionState.ActionSelected;
            UpdateActionButtonsActiveStates();
        }

        private void DriveSelectedVehicleToSelectedTile()
        {            
            _selectedVehicle.CurrentNavTile = _selectedNavTile;
            _selectedNavTile = null;

            _selectedVehicle.RemainingTurnActions--;
            DeselectVehicle();
        }
    }
}
