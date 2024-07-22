//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        13.06.24
// Date last edited:    19.07.24
//////////////////////////////////////////////////
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;


namespace Meatwagon
{    
    [RequireComponent(typeof(BoxCollider2D))]
    [RequireComponent(typeof(MeshRenderer))]
    [RequireComponent(typeof(Transform))]
    // A tile within the scene which a character can inhabit as well as navigating to and from.
    public class NavTile : MonoBehaviour
    {
        public enum SelectedState
        {
            Default,
            Highlighted,
            Selected
        }

        public List<NavTile> ConnectedTiles;
        public Material DefaultMaterial;
        public Material HighlightMaterial;
        public Material SelectedMaterial;
        // If the tile is inhabited (by a vehicle or other obstacle), it cannot be moved into by a vehicle and path detection will move around it.
        public bool IsInhabited;
        // The value used to represent 'infinite' distance in Dijkstra's pathfinding algorithm.
        public const int DijkstraInfiniteDistance = 99999;
        // The 'cost' of moving into this tile.
        public int TraversalCost = 1; 
        // Used by the NavController to implement Dijkstra's pathfinding algorithm - the shortest currently-calculated distance from this tile to the starting tile
        [HideInInspector] public int DijkstraShortestDistance; 

        public List<NavTile> GetConnectedTiles()
        {
            return ConnectedTiles;
        }

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
                        _meshRenderer.material = DefaultMaterial;
                        _selectedState = SelectedState.Default;

                        break;
                    }
                case SelectedState.Highlighted:
                    {
                        _meshRenderer.material = HighlightMaterial;
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


        protected BoxCollider2D _boxCollider;        
        protected MeshRenderer _meshRenderer;
        protected SelectedState _selectedState;

        protected virtual void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _meshRenderer = GetComponent<MeshRenderer>();

            DijkstraShortestDistance = DijkstraInfiniteDistance;
        }

        // DEBUG
        protected void OnDrawGizmos()
        {
            // Draws a blue line gizmo to show connections between tiles.
            if (Input.GetKey(KeyCode.T) && ConnectedTiles != null && ConnectedTiles.Any())
            {
                Gizmos.color = UnityEngine.Color.blue;
                foreach (NavTile connectedTile in ConnectedTiles)
                {
                    Gizmos.DrawLine(this.transform.position, connectedTile.GetComponent<Transform>().position);
                }
            }
        }
    }
}
