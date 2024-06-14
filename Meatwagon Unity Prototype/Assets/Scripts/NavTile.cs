//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        13.06.24
// Date last edited:    14.06.24
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Meatwagon
{
    // A tile within the scene which a character can inhabit/navigate to and from.
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(Transform))]
    public class NavTile : MonoBehaviour
    {
        public enum SelectedState
        {
            Default,
            Highlighted,
            Selected
        }

        public List<NavTile> ConnectedTiles;
        public Material defaultMaterial;
        public Material highlightMaterial;
        public Material SelectedMaterial;
        public int ShortestDistance;

        public bool IsMouseOver()
        {
            Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            if (_boxCollider.OverlapPoint(mousePosition))
            {
                return true;
            }
            else
            {
                return false;
            }

        }

        public SelectedState GetSelectedState()
        {
            return _selectedState;
        }

        public void SetSelectedState(SelectedState selectedState)
        {
            switch(selectedState)
            {
                case SelectedState.Default:
                    {
                        _meshRenderer.material = defaultMaterial;
                        _selectedState = SelectedState.Default;

                        break;
                    }
                case SelectedState.Highlighted:
                    {
                        _meshRenderer.material = highlightMaterial;
                        _selectedState = SelectedState.Highlighted;

                        break;
                    }
                case SelectedState.Selected:
                    {
                       

                        _meshRenderer.material = SelectedMaterial;
                        _selectedState = SelectedState.Selected;

                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }



        private BoxCollider2D _boxCollider;
        private MeshRenderer _meshRenderer;
        private SelectedState _selectedState;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _meshRenderer = GetComponent<MeshRenderer>();
        }

        private void Start()
        {
            foreach (NavTile tile in GameObject.FindObjectsOfType<NavTile>())
            {
                if ((tile.transform.position - this.transform.position).magnitude == 1.0f)
                {
                    ConnectedTiles.Add(tile);
                }
            }
        }

        private void Update()
        {
            //if (Input.GetMouseButtonDown(0))
            //{
            //Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            //if (_boxCollider.OverlapPoint(mousePosition))
            //{
            //    if (Input.GetMouseButtonDown(0))
            //    {
            //        SetSelectedState(SelectedState.Selected);
            //    }
            //    else
            //    {
            //        SetSelectedState(SelectedState.Highlighted);
            //    }
            //}
            //else if(GetSelectedState() == SelectedState.Highlighted)
            //{
            //    SetSelectedState(SelectedState.Default);
            //}
            //}
        }


        private void OnDrawGizmos()
        {
            //Gizmos.color = UnityEngine.Color.yellow;
            //Gizmos.DrawWireCube(this.transform.position, Vector3.one);

            if (ConnectedTiles.Any() != false)
            {
                Gizmos.color = UnityEngine.Color.blue;
                foreach (NavTile connectedTile in ConnectedTiles)
                {
                    Gizmos.DrawLine(this.transform.position, connectedTile.GetComponent<Transform>().position);
                }
            }
        }
    }


    // Contains data about a one-way connection between two NavTiles.
    //public class NavTileConnection
    //{
    //    public NavTile FromTile;
    //    public NavTile TowardsTile;
    //    //public int Cost;

    //    public void DrawConnectionGizmo(Color color)
    //    {
    //        Gizmos.color = color;
    //        Gizmos.DrawLine(FromTile.GetComponent<Transform>().position, TowardsTile.GetComponent<Transform>().position);
    //    }
    //}
}
