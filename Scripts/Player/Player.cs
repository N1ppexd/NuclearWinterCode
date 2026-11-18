using System.Collections.Generic;
using TMPro;
using UnityEngine;
using MoreMountains.Feedbacks;
using N1ppeUtils;
using Sirenix.OdinInspector;
using System.Collections;

namespace NuclearWinter
{
    [RequireComponent(typeof(RangeLineManager))]
    public class Player : MonoBehaviour
    {
        public static Player Instance { get; private set; }

        private PathGenerator pathGenerator;

        private State state;


        [Required]
        [SerializeField] private PlayerSettingsSO playerSettingsSO;

        [Space(10)]
        [Required]
        [SerializeField] private RangeLineManager rangeLineManager;

        [Required]
        [SerializeField] private Material highlightedMaterial;


        //the maximum number of throws per a round
        [SerializeField] private int maxThrowsPerRound;
        [SerializeField] private int throwsLeft;
        [Required]
        [SerializeField] private TextMeshProUGUI dieAmountText, throwsLeftText;

        //the tile on which the player is currently standing
        private CustomTile currentTile;
        private CustomTile selectedTile;


        [Required]
        [SerializeField] private LineRenderer pathLine;

        [Space(10)]
        [Required]
        [SerializeField] private Animator animator;
        [Required]
        [SerializeField] private string moveAnim, idleAnim;
        [Required]
        [SerializeField] private float moveSpeed;


        [Space(10)]
        [Required]
        [SerializeField]
        private MMF_Player selectTileFeedback;
        [Space(10)]
        [Required]
        [SerializeField]
        private MMF_Player moveFeedback;

        [Space(10)]
        [Required]
        [SerializeField]
        private GameEvent newRoundEvent;



        private PlayerInput playerInput;

        private readonly List<CustomTile> currentPath = new();
        [HideInInspector]
        public List<CustomTile> visitedTiles = new();

        private List<CustomTile> possibleTiles = new();

        private bool isInfiniteThrows = false;



        private int seeRange;



        enum State
        {
            idleState,
            moveState,
            cardState
        }

        /// <summary>
        /// This is set from an event
        /// </summary>
        /// <param name="status"></param>
        public void SetCardState(bool status)
        {
            if (status)
                state = State.cardState;
            else state = State.idleState;
        }

        #region getters
        public Vector3 HighlightedTilePosition
        {
            get
            {
                if (selectedTile == null)
                {
                    return transform.position;
                }
                else return selectedTile.HexTile.TopPosition;
            }
        }

        //public CustomTile CurrentTile => currentPath[currentPath.Count - 1];
        public CustomTile CurrentTile => currentTile;

        private int moves;
        public int Moves
        {
            get => moves;
            set
            {
                dieAmountText.text = value.ToString();

                moves = value;

                //StartCoroutine(SetPossibleTiles(value));
            }
        }


        public int ThrowsLeft
        {
            get => throwsLeft;
            set
            {
                if(value < 0) value = 0;

                throwsLeft = value;

                throwsLeftText.text = throwsLeft.ToString();
            }
        }
        #endregion


        private void OnEnable()
        {
            if (Instance != null)
                Destroy(gameObject);
            else Instance = this;

            GameEvents.onGenerated += OnGenerated;
            GameEvents.onRoundRestarted += RestartRound;
            GameEvents.onBaseUpdated += OnUpdateBase;


            //set the default see range
            seeRange = playerSettingsSO.SeeRange;


            playerInput = new PlayerInput();
            playerInput.Enable();
            playerInput.Player.Fire.performed += ctx => Select();

            //set the throws to the maxrounds
            throwsLeft = maxThrowsPerRound;
            throwsLeftText.text = maxThrowsPerRound.ToString();


            if(rangeLineManager == null)
                rangeLineManager = GetComponent<RangeLineManager>();
        }


        private void OnDisable()
        {
            GameEvents.onGenerated -= OnGenerated;
            GameEvents.onRoundRestarted -= RestartRound;
            GameEvents.onBaseUpdated -= OnUpdateBase;

            playerInput.Player.Fire.performed -= ctx => Select();
            playerInput.Disable();
        }


        private void OnGenerated(bool hasGenerated)
        {
            if (hasGenerated)
            {
                SpawnPlayer();
            }
            else Debug.LogError("No map has been generated");
        }


        private void OnUpdateBase()
        {
            currentPath.Clear();

            for(int i = 0; i < GameManager.Instance.Round; i++)
            {
                List<CustomTile> tiles = HexagonOperations.GetTilesInRange(CustomGrid.CurrentGrid, CurrentTile, i);
                foreach(var tile in tiles)
                {
                    if (tile.TileSO.IsBaseTile)
                        return;

                    if (!visitedTiles.Contains(tile))
                        visitedTiles.Add(tile);
                }
            }
        }


        private void RestartRound()
        {
            try
            {
                currentTile = CustomGrid.CurrentGrid.GetBaseTile();
                transform.position = currentTile.HexTile.TopPosition;
            }
            catch
            {
                Debug.LogError("cant restart the round");
                return;
            }

            DisablePath();
            currentPath.Clear();
            ResetThrows();


        }

        [TerminalCommand("ResetThrows", "Reset the dice throws")]
        public void ResetThrows() => ThrowsLeft = maxThrowsPerRound;


        [TerminalCommand("SetInfiniteThrows", "SetInfiniteThrows(true or false) sets the amount of walk distance to indfinite or nornal")]
        public void SetInfiniteThrows(bool isInfinite) => isInfiniteThrows = isInfinite;



        public void IncreaseSeeRange(int increaseAmount)
        {
            seeRange += increaseAmount;


            RevealTile.RevealPath(visitedTiles, CustomGrid.CurrentGrid, seeRange);
        }


        /// <summary>
        /// Spawns the player
        /// </summary>
        private void SpawnPlayer()
        {
            pathGenerator = PathGenerator.Instance;

            state = State.idleState;

            CustomTile baseTile = pathGenerator.BaseTile;

            transform.position = baseTile.HexTile.TopPosition;

            currentTile = baseTile;

            visitedTiles.Add(baseTile);

            GameEvents.SpawnPlayer(this);
        }


        private void Select()
        {
            //if the mouse is over ui, the click doesn't count.
            if (NiilosExtensions.IsOverUi()) return;


            if (selectedTile == null || state != State.idleState)
            {
                return;
            }

            //interacts with it and so on..
            currentTile.HexTile.Interact();

            //focuses to player
            GameEvents.FocusOnPoint(selectedTile.HexTile.TopPosition);

            //this makes a terrain panel that informs player that there are less moves in the terrain...
            if(currentTile.TileSO.MovementReduction > 1) GameEvents.GoToTerrain();


            if (selectedTile.HexTile.TileSO.IsBaseTile)
            {
                if (!currentTile.HexTile.TileSO.IsBaseTile)
                {
                    DisablePath();

                    currentTile = selectedTile;
                    transform.position = currentTile.HexTile.TopPosition;

                    //play it
                    selectTileFeedback.PlayFeedbacks(selectedTile.HexTile.TopPosition);


                    moves = 0;
                    //starts a new round 
                    newRoundEvent?.Raise();
                }
            }


            else if (visitedTiles.Contains(selectedTile))
            {
                if(selectedTile == currentTile)
                {
                    return;
                }

                DisablePath();

                currentTile = selectedTile;
                transform.position = currentTile.HexTile.TopPosition;

                //play it
                selectTileFeedback.PlayFeedbacks(selectedTile.HexTile.TopPosition);

            }

            else if (currentPath.Count > 0 && currentPath.Contains(selectedTile))
            {
                //play it
                selectTileFeedback.PlayFeedbacks(selectedTile.HexTile.TopPosition);

                currentTile = currentPath[0];

                state = State.moveState;
                //player moves the currentMovement amount

                if(!isInfiniteThrows)
                    Moves -= currentPath.Count - 1;

                //shit
                if (currentTile.HasCard)
                {
                    currentTile.Card.UseCard();
                }

                //rmeove the last
                currentPath.Reverse();
                currentPath.RemoveAt(0);


                RevealTile.RevealPath(currentPath, CustomGrid.CurrentGrid, seeRange);


                //move player event
                GameEvents.PlayerMove();


                //add the selected path to visited tiles
                visitedTiles.AddRange(currentPath);

                foreach (var tile in visitedTiles)
                {
                    if (tile == null)
                        visitedTiles.Remove(tile);

                    if (tile.HexTile.TerrainTransform.TryGetComponent(out Renderer renderer))
                    {
                        if (tile.TileSO.IsBaseTile) continue;
                        renderer.sharedMaterial = highlightedMaterial;
                    }

                    DisablePath();
                }
            }
            /*s
            if(Moves > 0)
            {
                //rangeLineManager.SetRange(Moves);

                
            }*/

            //StartCoroutine(SetPossibleTiles(Moves));
        }



        private IEnumerator SetPossibleTiles(int amount)
        {
            if (possibleTiles.Count > 0)
            {
                foreach (var tile in possibleTiles)
                {
                    if (tile.HexTile != null)
                    {
                        tile.HexTile.SetPossibility(false);
                    }
                }

                possibleTiles.Clear();
            }

            if (amount > 0)
            {
                foreach (var tile in HexagonOperations.GetTilesInRange(CustomGrid.CurrentGrid, currentTile, amount))
                {
                    yield return null;

                    if (possibleTiles.Contains(tile))
                    {
                        if (tile.HexTile != null)
                        {
                            tile.HexTile.SetPossibility(true);
                        }
                        continue;
                    }

                    if (PathFinding.AStar(currentTile, tile, pathGenerator.MinPathHeight, pathGenerator.MaxPathHeight, CustomGrid.CurrentGrid.Grid, out List<CustomTile> path))
                    {
                        int moveCost = 0;
                        foreach (CustomTile tile2 in path)
                        {
                            moveCost += tile2.HexTile.TileSO.MovementReduction;
                            yield return null;
                        }


                        if (amount > 1 && moveCost > amount + 1)
                        {
                            continue;
                        }


                        if (tile.HexTile != null)
                        {
                            tile.HexTile.SetPossibility(true);
                        }


                        possibleTiles.AddRange(path);
                    }

                }
            }
        }



        private void Update()
        {
            switch(state)
            {
                case State.idleState:
                    Idle();
                    break;
                case State.moveState:
                    Move();
                    break;
            }
        }

        private void Idle()
        {
            if(!animator.GetCurrentAnimatorStateInfo(0).IsName(idleAnim))
                animator.Play(idleAnim);

            try
            {
                //check if mouse is hovering over a tile
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 300f))
                {
                    if (hit.transform.parent.TryGetComponent(out HexTile hexTile))
                    {
                        SetTileHighlight(hexTile);
                    }
                }
                else
                {
                    DisablePath();
                }
            }
            catch (System.Exception e)
            {
                Debug.Log(e);
            }
        }

        private void Move()
        {
            animator.Play(moveAnim);

            if(currentPath.Count == 0)
            {
                state = State.idleState;
                return;
            }

            Vector3 moveDir = (currentPath[0].HexTile.TopPosition - transform.position).normalized;

            transform.LookAt(transform.position + moveDir);
            
            transform.position += moveDir * moveSpeed * Time.deltaTime;

            if (Vector3.Distance(currentPath[0].HexTile.TopPosition, transform.position) < 0.5f)
            {
                transform.position = currentPath[0].HexTile.TopPosition;
                currentPath.RemoveAt(0);
            }
        }


        //Shit
        readonly List<HexTile> highlightedTiles = new();

        /// <summary>
        /// Search a path and highlight it
        /// </summary>
        /// <param name="hexTile"></param>
        private void SetTileHighlight(HexTile hexTile)
        {
            if (state != State.idleState) return;

            if(hexTile == null)
            {
                DisablePath();
                return;
            }

            if (!hexTile.TerrainTransform.TryGetComponent(out MeshRenderer _))
                return;

            if (hexTile.TileSO.IsBaseTile || visitedTiles.Contains(hexTile.CustomTile))
                selectedTile = hexTile.CustomTile;

            if (Moves <= 0)
            {
                DisablePath();
                return;
            }
            if (hexTile == null)
                return;


            

            //lol
            highlightedTiles.Add(hexTile);
            hexTile.Highlight();

            foreach(HexTile tile in new List<HexTile>(highlightedTiles))
            {
                if (tile != hexTile)
                {
                    try
                    {
                        tile.StopHighlight();
                    }
                    catch { }
                    
                    highlightedTiles.Remove(tile);
                }
            }

            if (PathFinding.AStar(currentTile, hexTile.CustomTile, pathGenerator.MinPathHeight, pathGenerator.MaxPathHeight, CustomGrid.CurrentGrid.Grid, out List<CustomTile> path))
            {

                currentPath.Clear();


                //Sets the path to false
                DisablePath();


                if (path.Count > Moves + 1 && !isInfiniteThrows) return;


                int moveCost = 0;
                foreach (CustomTile tile in path)
                {
                    moveCost += tile.HexTile.TileSO.MovementReduction;
                }


                if(Moves > 1 && moveCost > Moves + 1 && !isInfiniteThrows)
                {
                    DisablePath();
                    return;
                }


                foreach (CustomTile tile in path)
                {
                    //if (currentPath.Contains(tile.HexTile)) break;
                    currentPath.Add(tile);
                }

                currentPath.Reverse();

                pathLine.positionCount = currentPath.Count;

                for (int i = 0; i < path.Count; i++)
                {
                    pathLine.SetPosition(i, path[i].HexTile.TopPosition);
                }

                selectedTile = hexTile.CustomTile;
            }
            else
            {
                DisablePath();
            }
        }


        /// <summary>
        /// Sets the highlighed path disabled
        /// </summary>
        private void DisablePath()
        {
            pathLine.positionCount = 1;

            Vector3[] positions = { transform.position };
            pathLine.SetPositions(positions);

        }


        /// <summary>
        /// Called from the animation event
        /// </summary>
        public void MoveFeedback() => moveFeedback?.PlayFeedbacks(transform.position);
    }
}
