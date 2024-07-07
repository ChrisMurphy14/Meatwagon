//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        03.07.24
// Date last edited:    07.07.24
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
        public Button DriveVehicleButton;
        public Button ConfirmDriveVehicleButton;
        public NavController SceneNavController;

        private enum PlayerActionType
        {
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
        private Vehicle _selectedVehicle;
        private PlayerActionType _currentPlayerActionType;
        private PlayerActionState _currentPlayerActionState;
        //private List<Vehicle> _vehicles;

        private void Awake()
        {
            _currentPlayerActionType = 0;
            _currentPlayerActionState = PlayerActionState.NothingSelected;
        }

        private void Start()
        {
            DeselectVehicleButton.onClick.AddListener(DeselectVehicle);
            DriveVehicleButton.onClick.AddListener(DisplayDriveOptions);
            ConfirmDriveVehicleButton.onClick.AddListener(MoveSelectedVehicleToSelectedTile);
            UpdateActionButtonsActiveStates();

            foreach (Vehicle vehicle in GameObject.FindObjectsOfType<Vehicle>())
            {
                vehicle.OnLeftClicked.AddListener(SelectVehicle);
            }
        }

        private void Update()
        {
            if (_currentPlayerActionType == PlayerActionType.Drive)
            {
                if (_currentPlayerActionState == PlayerActionState.ActionSelected || _currentPlayerActionState == PlayerActionState.ActionConfirming)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        NavTile mouseOvertile = SceneNavController.GetNavTileWithMouseOver();
                        List<NavTile> path = SceneNavController.GetShortestPathBetweenTiles(_selectedVehicle.CurrentNavTile, mouseOvertile);
                        if (mouseOvertile != null && mouseOvertile != _selectedVehicle.CurrentNavTile && path != null && path.Count <= _selectedVehicle.Speed + 1)
                        {
                            SceneNavController.HighlightTilesInMovementRange(_selectedVehicle.CurrentNavTile, _selectedVehicle.Speed);

                            foreach (NavTile tile in path)
                            {
                                tile.SetSelectedState(NavTile.SelectedState.Selected);
                            }

                            _selectedNavTile = mouseOvertile;

                            _currentPlayerActionState = PlayerActionState.ActionConfirming;
                            UpdateActionButtonsActiveStates();
                        }
                    }
                }
            }
        }

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

                        DriveVehicleButton.gameObject.SetActive(true);
                        ConfirmDriveVehicleButton.gameObject.SetActive(false);

                        break;
                    }
                case PlayerActionState.ActionSelected:
                    {
                        if(_currentPlayerActionType == PlayerActionType.Drive)
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
                //_currentPlayerActionType = PlayerActionType.Drive;
                UpdateActionButtonsActiveStates();

                //_playerActionState = PlayerActionState.VehicleSelected;
            }
        }

        private void DeselectVehicle()
        {
            if (_selectedVehicle != null)
            {
                _selectedVehicle.IsSelected = false;
                _selectedVehicle = null;               

                SceneNavController.ResetAllTilesToDefaultSelectedState();

                _currentPlayerActionState = PlayerActionState.NothingSelected;
                UpdateActionButtonsActiveStates();
            }
        }

        private void DisplayDriveOptions()
        {
            SceneNavController.HighlightTilesInMovementRange(_selectedVehicle.CurrentNavTile, _selectedVehicle.Speed);

            _currentPlayerActionState = PlayerActionState.ActionSelected;
            UpdateActionButtonsActiveStates();

            //_playerActionState = PlayerActionState.ChoosingVehicleMovement;
        }

        private void MoveSelectedVehicleToSelectedTile()
        {
            _selectedVehicle.CurrentNavTile = _selectedNavTile;

            _selectedNavTile = null;
            DeselectVehicle();
        }
    }
}
