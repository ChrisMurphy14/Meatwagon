//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        03.07.24
// Date last edited:    19.07.24
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
        public Button DeselectEntityButton;
        public NavController RoadGridNavController;
        public NavController TestVehicleNavController;  // DEBUG
        public TextMeshPro TurnCounterDisplay;


        private List<GameEntity> _gameEntities;
        private GameEntity _selectedGameEntity;
        private NavController _activeNavController;
        private int turnCounter = -1;               

        private void Start()
        {
            DeselectEntityButton.onClick.AddListener(DeselectGameEntity);
            DeselectEntityButton.gameObject.SetActive(false);

            _gameEntities = new List<GameEntity>();
            foreach (GameEntity entity in GameObject.FindObjectsOfType<GameEntity>())
            {
                entity.OnLeftClicked.AddListener(SelectGameEntity);
                _gameEntities.Add(entity);
            }

            SetActiveNavController(RoadGridNavController);

            StartNewTurn();
        }

        private void Update()
        {
            // DEBUG
            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartNewTurn();
            }         
            if(Input.GetKeyDown(KeyCode.Tab))
            {
                DeselectGameEntity();

                SetActiveNavController(_activeNavController == RoadGridNavController ? TestVehicleNavController : RoadGridNavController);
            }
        }

        private void StartNewTurn()
        {
            DeselectGameEntity();

            foreach (GameEntity vehicle in _gameEntities)
            {
                vehicle.RemainingTurnActions = vehicle.ActionsPerTurn;
            }

            turnCounter++;
            TurnCounterDisplay.text = "Turn " + turnCounter;
        }

        private void SetActiveNavController(NavController navController)
        {
            RoadGridNavController.gameObject.SetActive(false);
            TestVehicleNavController.gameObject.SetActive(false);

            _activeNavController = navController;
            _activeNavController.gameObject.SetActive(true);
        }

        private void SelectGameEntity(GameEntity gameEntity)
        {
            if (_selectedGameEntity == null)
            {
                DeselectEntityButton.gameObject.SetActive(true);

                _selectedGameEntity = gameEntity;
                _selectedGameEntity.IsSelected = true;
            }
        }

        public void DeselectGameEntity()
        {
            if (_selectedGameEntity != null)
            {
                DeselectEntityButton.gameObject.SetActive(false);

                _selectedGameEntity.IsSelected = false;
                _selectedGameEntity = null;

                _activeNavController.ResetAllTilesToDefaultSelectedState();
            }
        }
    }
}
