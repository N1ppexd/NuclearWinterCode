using MoreMountains.Feedbacks;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace NuclearWinter
{
    public class HexTile : MonoBehaviour, IInteractive
    {
        private CustomTile customTile;

        public CustomTile CustomTile => customTile;


        private float height;
        private float width;


        private TileSO tileSO;

        [SerializeField] private Transform terrainTransform, fogTransform;

        [Space(20)]
        [SerializeField] private MeshRenderer meshRenderer;
        [SerializeField] private Material highlightMaterial, normalMaterial;


        [Required]
        [SerializeField] private GameObject higlightObj;



        [SerializeField] private MMF_Player highlightFeedbacks;
        [SerializeField] private GameEvent highlightEvent;
 

        private List<GameObject> decorationObjects = new List<GameObject>();

        public List<GameObject> DecorationObjects => decorationObjects;

        private bool isHighlighted = false;

        public TileSO TileSO => tileSO;

        public Transform TerrainTransform => terrainTransform;


        //this is set if there is an event on the hexagon
        public Transform EventIndicator {  get; private set; }



        public bool IsVisible { get; private set; }


        public void Hide() => SetVisibility(false);
        public void Show() => SetVisibility(true);

        public void SetVisibility(bool visible)
        {
            IsVisible = visible;

            fogTransform.gameObject.SetActive(!visible);
            terrainTransform.gameObject.SetActive(visible);
        }


        public void SetTile(CustomTile customTile, CustomGrid grid, TileSO tileSO)
        {
            this.customTile = customTile;
            this.customTile.SetTile(this);

            Width = grid.CellSize;


            if(tileSO == null)
            {
                Debug.LogError("thy the fuck is it null?");
            }

            if(grid.HexType == HexType.flatTop)
            {
                transform.Rotate(transform.up * 90f);
            }

            this.tileSO = tileSO;

            SetVisibility(true);
        }

        public void SetHeight(float minHeight, float currentHeight)
        {
            height = -minHeight + Mathf.Abs(currentHeight);

            terrainTransform.position += transform.up * height * 0.5f;
            terrainTransform.localScale = new Vector3(terrainTransform.localScale.x, height, terrainTransform.localScale.z);


            ///Set the decoration for the tile and put it to the top
            ///
            decorationObjects = tileSO.GetDecorationObjects(out bool canSpawn);

            if (canSpawn)
            {
                //spawn all decoration. I need to make some sort of an offset system too but this works just fine now
                foreach(var obj in decorationObjects)
                {
                    Transform decorationTransform = Instantiate(obj, terrainTransform).transform;
                    decorationTransform.position = TopPosition;

                    decorationTransform.localScale =
                        new Vector3(decorationTransform.localScale.x / transform.localScale.x,
                        decorationTransform.localScale.y / terrainTransform.localScale.y,
                        decorationTransform.localScale.z / transform.localScale.z);

                    //rotates the decoration randomly
                    decorationTransform.Rotate(transform.up * Random.Range(0f, 360f));
                }

                
            }

            CardSO cardSO = tileSO.GetTileCard(out bool canSpawnCard);

            if (canSpawnCard)
            {
                customTile.SetCard(cardSO);

                //set the event indicator here
                EventIndicator = Instantiate(TileSO.EventIndicator, terrainTransform).transform;
                EventIndicator.position = TopPosition;

                EventIndicator.localScale =
                    new Vector3(EventIndicator.localScale.x / transform.localScale.x,
                    EventIndicator.localScale.y / terrainTransform.localScale.y,
                    EventIndicator.localScale.z / transform.localScale.z);
            }
        }



        public void Highlight()
        {
            if(!isHighlighted)
            {
                highlightFeedbacks.Direction = MMFeedbacks.Directions.TopToBottom;
                highlightFeedbacks.PlayFeedbacks();
                isHighlighted = true;
                highlightEvent?.Raise();
            }
        }

        public void Interact()
        {
            if(EventIndicator != null && EventIndicator.gameObject.activeSelf)
            {
                EventIndicator.gameObject.SetActive(false);
            }
        }

        public void StopHighlight()
        {
            if(isHighlighted)
            {
                highlightFeedbacks.Direction = MMFeedbacks.Directions.BottomToTop;
                isHighlighted = false;
                highlightFeedbacks.PlayFeedbacks();
            }
            
        }



        public void SetPossibility(bool on)
        {
            //terrainTransform.GetComponent<MeshRenderer>().material = on ? highlightMaterial : normalMaterial;

            higlightObj.SetActive(on);

        }



        public float Width
        {
            get => width;

            set
            {
                width = value;
                transform.localScale = new Vector3(width, transform.localScale.y, width);
            }
        }

        public Vector3 TopPosition => terrainTransform.position + transform.up * height * 0.5f;
    }
}
