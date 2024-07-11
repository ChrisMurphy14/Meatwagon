//////////////////////////////////////////////////
// Author:              Chris Murphy
// Date created:        13.06.24
// Date last edited:    11.07.24
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
                
        public Material DefaultMaterial;
        public Material HighlightMaterial;
        public Material SelectedMaterial;
        // If the tile is inhabited (by a vehicle or other obstacle), it cannot be moved into by a vehicle and path detection will move around it.
        public bool IsInhabited;
        // The radius of the area around this tile within which it will connect to other tiles when the scene starts.
        public float AdjacentTileConnectionRadius = 1.5f;
        // The value used to represent 'infinite' distance in Dijkstra's pathfinding algorithm.
        public const int DijkstraInfiniteDistance = 99999;
        // The 'cost' of moving into this tile.
        public int TraversalCost = 1; 
        // Used by the NavController to implement Dijkstra's pathfinding algorithm - the shortest currently-calculated distance from this tile to the starting tile
        [HideInInspector] public int DijkstraShortestDistance; 

        public List<NavTile> GetConnectedTiles()
        {
            return _connectedTiles;
        }

        public List<NavTile> GetUninhabitedConnectedTiles()
        {
            List<NavTile> uninhabitedTiles = new List<NavTile>();
            foreach(NavTile tile in _connectedTiles)
            {
                if(!tile.IsInhabited)
                {
                    uninhabitedTiles.Add(tile);
                }
            }

            if(uninhabitedTiles.Count > 0)
            {
                return uninhabitedTiles;
            }
            else
            {
                return null;
            }
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


        private BoxCollider2D _boxCollider;
        private List<NavTile> _connectedTiles;
        private MeshRenderer _meshRenderer;
        private SelectedState _selectedState;

        private void Awake()
        {
            _boxCollider = GetComponent<BoxCollider2D>();
            _meshRenderer = GetComponent<MeshRenderer>();

            DijkstraShortestDistance = DijkstraInfiniteDistance;
        }

        private void Start()
        {
            ConnectToAdjacentTiles(AdjacentTileConnectionRadius);            
        }

        private void OnDrawGizmos()
        {
            //// Draws a blue line gizmo to show connections between tiles.
            //if (_connectedTiles != null && _connectedTiles.Any())
            //{
            //    Gizmos.color = UnityEngine.Color.blue;
            //    foreach (NavTile connectedTile in _connectedTiles)
            //    {
            //        Gizmos.DrawLine(this.transform.position, connectedTile.GetComponent<Transform>().position);
            //    }
            //}
        }

        private void ConnectToAdjacentTiles(float maxDistance)
        {
            _connectedTiles = new List<NavTile>();
            foreach (NavTile tile in GameObject.FindObjectsOfType<NavTile>())
            {
                if (tile != this && (tile.transform.position - this.transform.position).magnitude <= maxDistance)
                {
                    _connectedTiles.Add(tile);
                }
            }
        }
    }
}
