//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        18.07.24
// Date last edited:    27.07.24
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Meatwagon
{
    // The derived action which allows the specified Vehicle (the GameEntity must be one) to move to another position on their NavController-handled grid.
    public class DriveEntityAction : MoveEntityAction
    {
        public override void Initialize(NavController navController, GameEntity entity)
        {
            _vehicleEntity = entity.GetComponent<Vehicle>();
            if (_vehicleEntity == null)
            {
                Debug.LogError("The entity parameter of the Initialize() function must specifically be a Vehicle for the DriveEntityAction to function correctly.");
                return;
            }

            _vehicleDriver = null;
            if (_vehicleEntity.DriverTile.IsInhabited)
            {
                _vehicleDriver = _vehicleEntity.DriverTile.GetInhabitant();
            }

            AreButtonsInteractable = false;
            if (_vehicleEntity.RemainingTurnActions > 0 && _vehicleDriver != null && _vehicleDriver.RemainingTurnActions > 0)
            {
                AreButtonsInteractable = true;
            }

            base.Initialize(navController, _vehicleEntity);
        }


        protected GameEntity _vehicleDriver;
        protected Vehicle _vehicleEntity;

        // Called when the player clicks the button that says they want to begin proceeding with this specific action.
        protected override void BeginAction()
        {
            _moveSpeed = _vehicleEntity.Speed + _vehicleDriver.Speed;

            _navController.HighlightTilesInMovementRange(_gameEntity.CurrentNavTile, _moveSpeed);
            _isPlayerChoosingPath = true;

            BeginActionButton.gameObject.SetActive(false);
        }

        // Called when the player clicks the button that confirms they want to complete this action.
        protected override void ConfirmAction()
        {
            _vehicleDriver.RemainingTurnActions--;

            base.ConfirmAction();
        }
    }
}
