//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        03.07.24
// Date last edited:    20.07.24
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


namespace Meatwagon
{
    // Controls the high-level flow of gameplay within the battle scene.
    public class GameplayController : MonoBehaviour
    {
        public Button DeselectEntityButton;
        public Button VehicleViewButton;
        public Button RoadGridViewButton;
        public NavController RoadGridNavController;
        public TextMeshPro TurnCounterDisplay;
        public TextMeshPro SelectedEntityNameDisplay;
        public TextMeshPro SelectedEntitySpeedDisplay;
        public TextMeshPro VehicleViewVehicleNameDisplay;

        // Call once a GameEntity finishes taking an action in order to ready it for any remaining actions while keeping it selected.
        public void EndGameEntityAction()
        {
            if (_selectedGameEntity != null)
            {
                GameEntity gameEntity = _selectedGameEntity;
                DeselectGameEntity();
                SelectGameEntity(gameEntity);
            }
        }

        public void DeselectGameEntity()
        {
            if (_selectedGameEntity != null)
            {
                DeselectEntityButton.gameObject.SetActive(false);
                if (VehicleViewButton.gameObject.activeInHierarchy && _activeNavController == RoadGridNavController)
                {
                    VehicleViewButton.gameObject.SetActive(false);
                }

                _selectedGameEntity.IsSelected = false;
                _selectedGameEntity = null;

                _activeNavController.ResetAllTilesToDefaultSelectedState();

                DisableSelectedGameEntityStatDisplays();
            }
        }


        private List<GameEntity> _gameEntities;
        private GameEntity _selectedGameEntity;
        private NavController _activeNavController;
        private int _turnCounter = -1;

        private void Start()
        {
            _gameEntities = new List<GameEntity>();
            foreach (GameEntity entity in GameObject.FindObjectsOfType<GameEntity>())
            {
                entity.OnLeftClicked.AddListener(SelectGameEntity);
                _gameEntities.Add(entity);
            }

            SetActiveNavController(RoadGridNavController);

            DeselectEntityButton.onClick.AddListener(DeselectGameEntity);
            DeselectEntityButton.gameObject.SetActive(false);
            VehicleViewButton.onClick.AddListener(ActivateSelectedVehicleView);
            VehicleViewButton.gameObject.SetActive(false);
            RoadGridViewButton.onClick.AddListener(ActivateRoadGridView);
            RoadGridViewButton.gameObject.SetActive(false);

            VehicleViewVehicleNameDisplay.gameObject.SetActive(false);
            DisableSelectedGameEntityStatDisplays();

            StartNewTurn();

            foreach (Vehicle vehicle in GameObject.FindObjectsOfType<Vehicle>())
            {
                vehicle.VehicleViewNavController.gameObject.SetActive(false);     
            }
        }

        private void Update()
        {
            // DEBUG
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartNewTurn();
            }
        }

        private void StartNewTurn()
        {
            DeselectGameEntity();

            foreach (GameEntity vehicle in _gameEntities)
            {
                vehicle.RemainingTurnActions = vehicle.ActionsPerTurn;
            }

            _turnCounter++;
            TurnCounterDisplay.text = "Turn " + _turnCounter;
        }

        private void SetActiveNavController(NavController navController)
        {
            DeselectGameEntity();

            if (_activeNavController != null)
            {
                _activeNavController.gameObject.SetActive(false);
            }

            navController.gameObject.SetActive(true);
            _activeNavController = navController;
        }

        private void ActivateRoadGridView()
        {
            if(_activeNavController != RoadGridNavController)
            {
                SetActiveNavController(RoadGridNavController);

                VehicleViewVehicleNameDisplay.gameObject.SetActive(false);
                RoadGridViewButton.gameObject.SetActive(false);
            }
        }

        private void ActivateSelectedVehicleView()
        {
            if (_selectedGameEntity == null)
            {
                Debug.LogError("A game entity must first be selected in order for the ActivateSelectedVehicleView() function to be called.");
                return;
            }
            if (_activeNavController != RoadGridNavController)
            {
                Debug.LogError("The current NavController must be the road grid in order for the ActivateSelectedVehicleView() function to be called.");
                return;
            }
            Vehicle vehicle = _selectedGameEntity.gameObject.GetComponent<Vehicle>();
            if (vehicle == null)
            {
                Debug.LogError("The selected game entity must have a Vehicle component attached for the ActivateSelectedVehicleView() function to be called.");
                return;
            }

            SetActiveNavController(vehicle.VehicleViewNavController);

            VehicleViewButton.gameObject.SetActive(false);
            RoadGridViewButton.gameObject.SetActive(true);
            VehicleViewVehicleNameDisplay.gameObject.SetActive(true);
            VehicleViewVehicleNameDisplay.text = vehicle.GameName;
        }

        private void SelectGameEntity(GameEntity gameEntity)
        {
            if (_selectedGameEntity == null)
            {
                DeselectEntityButton.gameObject.SetActive(true);
                Vehicle vehicle = gameEntity.gameObject.GetComponent<Vehicle>();
                if (vehicle != null)
                {
                    VehicleViewButton.gameObject.SetActive(true);
                }

                _selectedGameEntity = gameEntity;
                _selectedGameEntity.IsSelected = true;

                EnableSelectedEntityStatDisplays();
            }
        }

        private void EnableSelectedEntityStatDisplays()
        {
            if (_selectedGameEntity == null)
            {
                Debug.LogError("EnableSelectedEntityStatDisplays() can only be called if _selectedGameEntity != null.");
                return;
            }

            SelectedEntityNameDisplay.gameObject.SetActive(true);
            SelectedEntityNameDisplay.text = _selectedGameEntity.GameName;

            SelectedEntitySpeedDisplay.gameObject.SetActive(true);
            SelectedEntitySpeedDisplay.text = "Speed: " + _selectedGameEntity.Speed;
        }

        private void DisableSelectedGameEntityStatDisplays()
        {
            SelectedEntityNameDisplay.gameObject.SetActive(false);
            SelectedEntitySpeedDisplay.gameObject.SetActive(false);
        }
    }
}
