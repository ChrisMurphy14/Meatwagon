//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        03.07.24
// Date last edited:    11.07.24
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


namespace Meatwagon
{
    // Handles an individual vehicle within the battle scene.
    [RequireComponent(typeof(BoxCollider2D))]
    public class Vehicle : MonoBehaviour
    {
        public Color SelectedColor = Color.green;
        public NavTile InitialNavTile;
        [HideInInspector] public UnityEvent<Vehicle> OnLeftClicked;
        public int Speed = 1;

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                _isSelected = value;

                if (_isSelected)
                {
                    _sprite.color = SelectedColor;
                }
                else
                {
                    _sprite.color = _defaultColor;
                }
            }
        }

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
                this.transform.position = _currentNavTile.transform.position;
            }
        }


        private Color _defaultColor;
        private BoxCollider2D _boxCollider;
        private NavTile _currentNavTile;
        private SpriteRenderer _sprite;
        private bool _isSelected;

        private void Awake()
        {
            if (OnLeftClicked == null)
            {
                OnLeftClicked = new UnityEvent<Vehicle>();
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
    }
}
