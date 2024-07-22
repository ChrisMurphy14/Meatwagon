//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        18.07.24
// Date last edited:    22.07.24
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Meatwagon
{
    // The derived action which allows the specified GameEntity to move to another position on their NavController-handled grid.
    public class MoveEntityAction : EntityAction
    {
        protected NavTile _selectedNavTile;
        protected bool _isPlayerChoosingPath = false;
        protected int _moveSpeed;

        protected void Update()
        {
            // If the left mouse button is clicked on a tile within the Speed range of the selected entity, tries to find a valid path to that destination tile and displays it as selected tiles.
            if (_isPlayerChoosingPath && Input.GetMouseButtonDown(0))
            {
                NavTile mouseOvertile = _navController.GetNavTileWithMouseOver();
                if (mouseOvertile != null && mouseOvertile != _gameEntity.CurrentNavTile)
                {
                    List<NavTile> pathFromSelectedVehicle = _navController.GetShortestPathBetweenTiles(_gameEntity.CurrentNavTile, mouseOvertile);
                    if (pathFromSelectedVehicle != null && pathFromSelectedVehicle.Count <= _moveSpeed + 1)
                    {
                        _navController.ResetAllTilesToDefaultSelectedState();
                        _navController.HighlightTilesInMovementRange(_gameEntity.CurrentNavTile, _gameEntity.Speed);
                        foreach (NavTile pathTile in pathFromSelectedVehicle)
                        {
                            pathTile.SetSelectedState(NavTile.SelectedState.Selected);
                        }

                        _selectedNavTile = mouseOvertile;

                        ConfirmActionButton.gameObject.SetActive(true);
                    }
                }
            }
        }

        // Called when the player clicks the button that says they want to begin proceeding with this specific action.
        protected override void StartAction()
        {
            _moveSpeed = _gameEntity.Speed;

            _navController.HighlightTilesInMovementRange(_gameEntity.CurrentNavTile, _moveSpeed);
            _isPlayerChoosingPath = true;

            base.StartAction();
        }

        // Called when the player clicks the button that confirms they want to complete this action.
        protected override void ConfirmAction()
        {
            _gameEntity.CurrentNavTile = _selectedNavTile;
            _selectedNavTile = null;

            base.ConfirmAction();
        }
    }
}
