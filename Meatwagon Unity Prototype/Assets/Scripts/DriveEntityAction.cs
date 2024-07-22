//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        18.07.24
// Date last edited:    19.07.24
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

            AreButtonsInteractable = _vehicleEntity.IsDriverTileInhabited();

            base.Initialize(navController, _vehicleEntity);
        }
        

        protected Vehicle _vehicleEntity;

        // Called when the player clicks the button that says they want to begin proceeding with this specific action.
        protected override void StartAction()
        {
            //                _moveSpeed = _vehicleEntity.Speed + ;
            

            _navController.HighlightTilesInMovementRange(_gameEntity.CurrentNavTile, _moveSpeed);
            _isPlayerChoosingPath = true;

            base.StartAction();
        }
    }
}
