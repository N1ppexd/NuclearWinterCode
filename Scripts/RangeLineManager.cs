using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    [RequireComponent(typeof(Player))]
    public class RangeLineManager : MonoBehaviour
    {

        [SerializeField] private LineRenderer lineRenderer;

        Player player;


        private void Awake() => player = GetComponent<Player>();



        private void OnEnable()
        {
            GameEvents.onDiceThrown += SetRange;
            GameEvents.onPlayerMoved += ClearRange;

        }
        private void OnDisable()
        {
            GameEvents.onDiceThrown -= SetRange;
            GameEvents.onPlayerMoved -= ClearRange;
        }



        /// <summary>
        /// Clear the range...
        /// </summary>
        private void ClearRange() => lineRenderer.enabled = false;


        /// <summary>
        /// Set the range
        /// </summary>
        /// <param name="range"></param>
        public void SetRange(int range)
        {
            if (range <= 0) return;

            if(player == null) 
            { 
                Debug.Log("player is null for some reason"); 
                return; 
            }


            lineRenderer.enabled = true;



            CustomTile currentTile = player.CurrentTile;


            if (currentTile == null)
            {
                Debug.Log("currentTile is null");
                return;
            }


            List<CustomTile> ringTiles = HexagonOperations.CubeRing(CustomGrid.CurrentGrid, currentTile, range);

            List<Vector3> ringPositions = new();

            lineRenderer.positionCount = ringTiles.Count + 1;

            foreach(CustomTile tile in ringTiles)
            {
                if (tile == null) return;

                //make the line on top of the hexes with some small offset
                ringPositions.Add(tile.HexTile.TopPosition + Vector3.up * 0.2f);
            }

            ringPositions.Add(ringPositions[0]);

            //Sets the positions to the line renderer and it should draw a line around the hexagon in a certain range...
            lineRenderer.SetPositions(ringPositions.ToArray());
        }
    }
}
