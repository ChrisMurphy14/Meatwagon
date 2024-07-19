//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        03.07.24
// Date last edited:    19.07.24
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Meatwagon
{
    // The base class for an individual vehicle or character which navigates and performs actions within the battle scene.
    [RequireComponent(typeof(BoxCollider2D))]
    public class GameEntity : MonoBehaviour
    {
        // The prefab used to spawn the buttons & script that handle the 'Move' action of the entity.
        public GameObject MoveEntityActionPrefab;
        // The specific nav controller which handles the set of nav tiles that the entity can traverse.
        public NavController NavigationController;    
        // The initial navigation tile which the entity will inhabit on scene start.
        public NavTile InitialNavTile;
        [HideInInspector] public UnityEvent<GameEntity> OnLeftClicked;
        public Color SelectedColor = Color.green;
        // The distance towards the camera that the entity is offset from the position of its current NavTile to avoid clipping.
        public float HeightOffsetFromNavTile = 0.1f;
        public int ActionsPerTurn = 2;
        [HideInInspector] public int RemainingTurnActions;
        public int Speed = 1;        

        public NavTile CurrentNavTile
        {
            get { return _currentNavTile; }
            set
            {
                if (_currentNavTile != null)
                {
                    _currentNavTile.IsInhabited = false;
                }

                _currentNavTile = value;
                _currentNavTile.IsInhabited = true;
                this.transform.position = _currentNavTile.transform.position + Vector3.back * HeightOffsetFromNavTile;
            }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;

                if (_isSelected)
                {
                    if (RemainingTurnActions > 0)
                    {
                        InstantiateEntityActions();
                    }

                    _sprite.color = SelectedColor;
                }
                else
                {
                    DestroyEntityActions();

                    _sprite.color = _defaultColor;
                }
            }
        }
               
        
        private BoxCollider2D _boxCollider;       
        private EntityAction _moveEntityAction;
        private NavTile _currentNavTile;
        private SpriteRenderer _sprite;
        private Color _defaultColor;
        private bool _isSelected;

        private void Awake()
        {
            if (OnLeftClicked == null)
            {
                OnLeftClicked = new UnityEvent<GameEntity>();
            }
            _boxCollider = GetComponent<BoxCollider2D>();
            _sprite = GetComponentInChildren<SpriteRenderer>();
            _defaultColor = _sprite.color;
        }

        private void Start()
        {
            CurrentNavTile = InitialNavTile; 
        }

        private void Update()
        {
            if (Input.GetMouseButtonDown(0))
            {
                Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                if (_boxCollider.OverlapPoint(mousePosition) && OnLeftClicked != null)
                {
                    OnLeftClicked.Invoke(this);
                }
            }
        }

        // Uses the entity action prefabs to spawn the entity action buttons & scripts used to control the entity while it's selected.
        private void InstantiateEntityActions()
        {
            // Parents the entity action object to the scene canvas so that the buttons are displayed properly.
            _moveEntityAction = Instantiate(MoveEntityActionPrefab, GameObject.FindGameObjectWithTag("SceneCanvas").transform).GetComponent<EntityAction>();
            _moveEntityAction.Initialize(NavigationController, this);
        }

        // Destroys any instantiated entity action objects.
        private void DestroyEntityActions()
        {
            if (_moveEntityAction != null)
            {
                GameObject.Destroy(_moveEntityAction.gameObject);
            }
        }
    }
}
