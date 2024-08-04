//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        18.07.24
// Date last edited:    22.07.24
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


namespace Meatwagon
{
    // The base class used to handle the UI buttons and entity behaviour of an action that they can take (e.g. 'Move', 'Attack', etc.).
    public class EntityAction : MonoBehaviour
    {
        public Button BeginActionButton;
        public Button ConfirmActionButton;     

        public bool AreButtonsInteractable
        {
            get
            {
                return _areButtonsInteractable;
            }
            set
            {
                _areButtonsInteractable = value;

                BeginActionButton.interactable = _areButtonsInteractable;
                ConfirmActionButton.interactable = _areButtonsInteractable;
            }
        }

        //public bool IsActionBeingPerformed()
        //{

        //}

        public virtual void Initialize(NavController navController, GameEntity entity)
        {
            this._navController = navController;
            this._gameEntity = entity;
        }
              

        // The entity (character, vehicle, etc.) that is performing this action.
        protected GameEntity _gameEntity;
        // The specific nav controller used to handle the set of nav tiles associated with the entity performing this action.
        protected NavController _navController;        
        protected bool _areButtonsInteractable = true;
        //protected bool _isActionBeingPerformed = false;
        
        protected virtual void Start()
        {
            BeginActionButton.onClick.AddListener(BeginAction);
            ConfirmActionButton.onClick.AddListener(ConfirmAction);

            BeginActionButton.gameObject.SetActive(true);
            ConfirmActionButton.gameObject.SetActive(false);
        }               

        // Called when the player clicks the button that says they want to begin proceeding with this specific action.
        protected virtual void BeginAction()
        {
            BeginActionButton.gameObject.SetActive(false);

            //_isActionBeingPerformed = true;
        }

        // Called when the player clicks the button that confirms they want to complete this action.
        protected virtual void ConfirmAction()
        {
            _gameEntity.RemainingTurnActions--;

            GameObject.FindObjectOfType<GameplayController>().EndGameEntityAction();
        }
    }
}
