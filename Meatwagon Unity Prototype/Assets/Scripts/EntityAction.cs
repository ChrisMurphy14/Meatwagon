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
        public Button StartActionButton;
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

                StartActionButton.interactable = _areButtonsInteractable;
                ConfirmActionButton.interactable = _areButtonsInteractable;
            }
        }

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
        
        protected virtual void Start()
        {
            StartActionButton.onClick.AddListener(StartAction);
            ConfirmActionButton.onClick.AddListener(ConfirmAction);

            StartActionButton.gameObject.SetActive(true);
            ConfirmActionButton.gameObject.SetActive(false);
        }               

        // Called when the player clicks the button that says they want to begin proceeding with this specific action.
        protected virtual void StartAction()
        {
            StartActionButton.gameObject.SetActive(false);
        }

        // Called when the player clicks the button that confirms they want to complete this action.
        protected virtual void ConfirmAction()
        {
            _gameEntity.RemainingTurnActions--;

            GameObject.FindObjectOfType<GameplayController>().EndGameEntityAction();
        }
    }
}
