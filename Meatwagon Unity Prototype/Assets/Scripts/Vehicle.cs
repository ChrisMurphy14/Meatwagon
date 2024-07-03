//////////////////////////////////////////////////
// Author/s:            Chris Murphy
// Date created:        03.07.24
// Date last edited:    03.07.24
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Handles an individual vehicle within the battle scene.
[RequireComponent(typeof(BoxCollider2D))]
public class Vehicle : MonoBehaviour
{
    public Color SelectedColor = Color.green;
    [HideInInspector] public UnityEvent<Vehicle> OnLeftClicked;

    public bool IsSelected
    {
        get { return _isSelected; }
        set 
        {
            _isSelected = value;

            if(_isSelected)
            {
                _sprite.color = SelectedColor;
            }
            else
            {
                _sprite.color = _defaultColor;
            }
        }
    }


    private Color _defaultColor;
    private BoxCollider2D _boxCollider;
    private SpriteRenderer _sprite;
    private bool _isSelected;

    private void Awake()
    {
        if(OnLeftClicked == null)
        {
            OnLeftClicked = new UnityEvent<Vehicle>();
        }

        _boxCollider = GetComponent<BoxCollider2D>();
        _sprite = GetComponentInChildren<SpriteRenderer>();
        _defaultColor = _sprite.color;
    }

    private void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (_boxCollider.OverlapPoint(mousePosition) && OnLeftClicked != null)
            {
                OnLeftClicked.Invoke(this);
            }
        }
    }
}
