//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        03.07.24
// Date last edited:    22.07.24
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Meatwagon
{
    // Handles a vehicle game entity which can take actions on the road grid as well as being zoomed in on for intra-vehicle combat.
    public class Vehicle : GameEntity
    {
        // The NavController used to handle the 'zoomed' view of the vehicle and the character GameEntities inhabiting it.
        public NavController VehicleViewNavController;
        // The tile which must be inhabited by a character in order to for the vehicle to use the 'Drive' action.
        public NavTile DriverTile;

        public bool IsDriverTileInhabited()
        {
            return DriverTile.IsInhabited;
        }

        //public int GetDriverSpeed()
        //{
            
        //}
    }
}
