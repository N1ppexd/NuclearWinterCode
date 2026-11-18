using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    [CreateAssetMenu(fileName = "TileSO", menuName = "ScriptableObjects/TileSO")]
    public class TileSO : ScriptableObject
    {
        [SerializeField] private GameObject tilePrefab;
        [SerializeField] private bool isBaseTile;

        [SerializeField] private TileType tiletype;

        [Header("used in roomTiles to generate the area around themm...")]
        [SerializeField] private int generateRadius;

        [Header("used to control how far away the tile can be generated from other tiles at maximum")]
        [SerializeField] private int maxDistanceRadius;


        #region tile decorations and events
        [Space(10)] 
        [Header("Tile Decoration")]
        [SerializeField] private TileDecoration tileDecoration;

        [Space(10)] 
        [Header("Tile event")]
        [SerializeField] private TileCard tileCard;


        [Required]
        [SerializeField]
        [InfoBox("indicator effect indicating that there is an event or a card")]
        private GameObject eventIndicator;

        #endregion


        [Range(0,100f)]
        [SerializeField] private float movementCost = 1f;


        [Space(10)]
        [Header("Tile Movement reduction")]
        [InfoBox("The amount of movement that is reduced when the player moves on this tile")]
        [MinValue(1)]
        [SerializeField] private int movementReduction = 1;


        #region getters
        public float MovementCost => movementCost;
        public bool IsBaseTile => isBaseTile;
        public TileType TileType => tiletype;
        public int GenerateRadius => generateRadius;
        public int MaxDistanceRadius => maxDistanceRadius;
        public int MovementReduction => movementReduction;

        public GameObject EventIndicator => eventIndicator;
        

        public CustomTile Tile { get; private set; }
        public GameObject TilePrefab => tilePrefab;

        #endregion

        #region onvalidate
        private void OnValidate()
        {
            if(generateRadius < 0) 
                generateRadius = 0;

            if(maxDistanceRadius < generateRadius)
                maxDistanceRadius = generateRadius;
        }
        #endregion

        /// <summary>
        /// Set the tile for this room
        /// </summary>
        /// <param name="tile"></param>
        public void SetTile(CustomTile tile) => Tile = tile;


        /// <summary>
        /// Try to get the decoration objects
        /// </summary>
        /// <param name="canSpawn"></param>
        /// <returns></returns>
        public List<GameObject> GetDecorationObjects(out bool canSpawn) 
            => tileDecoration.GetDecorationObjects(out canSpawn);


        /// <summary>
        /// Try to trigger the event on the tile
        /// </summary>
        public CardSO GetTileCard(out bool canSpawn)
            => tileCard.GetCard(out canSpawn);

    }


    [System.Serializable]
    public class TileCard
    {
        public CardSO[] tileCards;

        public CardSO[] TileCards => tileCards;


        [Range(0f, 100f)]
        [SerializeField]
        [Space(10)]
        [Header("The chance of spawning an event %")]
        private float spawnChance = 0;


        public CardSO GetCard(out bool canSpawn)
        {
            if(tileCards == null || tileCards.Length == 0)
            {
                canSpawn = false;
                return null;
            }


            if (Random.Range(0f, 100f) < spawnChance)
            {
                canSpawn = true;
            }
            else canSpawn = false;

            return tileCards[Random.Range(0, tileCards.Length)];
        }
    }


    [System.Serializable]
    public class TileDecoration
    {
        [SerializeField] private GameObject[] spawnableObjets;
        public GameObject[] SpawnableObjects => spawnableObjets;


        [Range(0f, 100f)]
        [SerializeField]
        [Space(10)]
        [Header("The chance of spawning an object / decoration %")]
        private float spawnChance = 0;

        [Space(5)]
        [SerializeField]
        [Header("The type of choosing the decoration")]
        private DecorationChooseType decorationChooseType = DecorationChooseType.random;


        /// <summary>
        /// Get a random object from the list of spawnable objects
        /// </summary>
        /// <returns>a gameObject to be spawned</returns>
        public List<GameObject> GetDecorationObjects(out bool canSpawn)
        {
            //check if the spawn even happens


            if (spawnableObjets == null ||  spawnableObjets.Length <= 0)
            {
                canSpawn = false;
                return null;
            }


            canSpawn = Random.Range(0f, 100f) < spawnChance;

            List<GameObject> objects = new List<GameObject>();

            if (decorationChooseType == DecorationChooseType.random)
            {
                if (canSpawn)
                {
                    objects.Add(spawnableObjets[Random.Range(0, spawnableObjets.Length)]);
                }
                else return null;
            }

            else
            {
                if (canSpawn)
                {
                    objects.AddRange(spawnableObjets);
                }
                else return null;
            }

            return objects;
        }

        public enum DecorationChooseType
        {
            random,
            chooseAll
        }
    }

    public enum TileType
    {
        BaseTile,
        CityTile,
        CabinTile,
        VillageTile
    }
}
