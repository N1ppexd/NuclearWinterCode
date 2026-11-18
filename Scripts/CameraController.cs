using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace NuclearWinter
{
    public class CameraController : MonoBehaviour
    {

        [SerializeField] private Player player;


        [SerializeField] private float cameraSpeed = 50f;
        [SerializeField] private float rotationSpeed = 200f;

        [SerializeField] private CinemachineCameraOffset camOffset;

        private PlayerInput playerInput;


        private Vector3 camRotation;



        [SerializeField] private float minDistanceToPlayer;

        [SerializeField] private Transform xAxisFollower;
        [SerializeField] private Transform follower;


        [SerializeField] private float minYPosition;


        [SerializeField] private Vector3 offset;
        [SerializeField] private Vector3 offset2;

        [SerializeField] private float zoomSpeed = 10f;
        [SerializeField] private float minXRotation, maxXRotation;

        [Space(10)]
        [SerializeField] private LayerMask whatIsGround;


        Vector3 focusPos = Vector3.zero;


        bool isDragging = false;
        private Vector3 movement;


        private void OnEnable()
        {
            playerInput = new PlayerInput();
            playerInput.Enable();

            playerInput.Player.Move.performed += ctx => MoveCamera(ctx.ReadValue<Vector2>());
            playerInput.Player.Move.canceled += ctx => MoveCamera(ctx.ReadValue<Vector2>());

            playerInput.Player.Zoom.performed += ctx => ZoomCamera(ctx.ReadValue<Vector2>());
            playerInput.Player.Zoom.canceled += ctx => ZoomCamera(ctx.ReadValue<Vector2>());

            playerInput.Player.Drag.performed += _ => isDragging = true;
            playerInput.Player.Drag.canceled += _ => isDragging = false;

            playerInput.Player.Look.performed += DragCamera;
            playerInput.Player.Look.canceled += DragCamera;

            GameEvents.onFocused += FocusOnPlayer;
            GameEvents.onPlayerSpawned += OnPlayerSpawned;

            playerInput.Player.Focus.performed += ctx => FocusOnPlayer(player.transform.position);
            playerInput.Player.FocusOnPoint.performed += FocusOnPoint;

        }

        private void OnDisable()
        {
            playerInput.Player.Move.performed -= ctx => MoveCamera(ctx.ReadValue<Vector2>());
            playerInput.Player.Move.canceled -= ctx => MoveCamera(ctx.ReadValue<Vector2>());

            playerInput.Player.Zoom.performed -= ctx => ZoomCamera(ctx.ReadValue<Vector2>());
            playerInput.Player.Zoom.canceled -= ctx => ZoomCamera(ctx.ReadValue<Vector2>());

            playerInput.Player.Drag.performed -= _ => isDragging = true;
            playerInput.Player.Drag.canceled -= _ => isDragging = false;

            playerInput.Player.Look.performed -= DragCamera;
            playerInput.Player.Look.canceled -= DragCamera;

            GameEvents.onFocused -= FocusOnPlayer;
            GameEvents.onPlayerSpawned -= OnPlayerSpawned;

            playerInput.Player.Focus.performed -= ctx => FocusOnPlayer(player.transform.position);
            playerInput.Player.FocusOnPoint.performed -= FocusOnPoint;

            playerInput.Disable();
        }

        float xRotation = 0f;


        private void OnPlayerSpawned(Player player)
        {
            this.player = player;
            FocusOnPlayer(player.transform.position);
        }


        // Update is called once per frame
        void Update()
        {

            //focusPos = player.HighlightedTilePosition;

            //focusPos = player.transform.position;

            if (!isDragging)
            {
                transform.position = focusPos;
            }

            camOffset.m_Offset = follower.localToWorldMatrix * offset;


            transform.RotateAround(focusPos, Vector3.up, camRotation.y * rotationSpeed * Time.deltaTime);
            if (camRotation != Vector3.zero)
            {
                //next the rotation around the local x axis
                xRotation += camRotation.x * rotationSpeed * Time.deltaTime;

                //xRotation += xAxisFollower.localRotation.eulerAngles.x;

                xRotation = Mathf.Clamp(xRotation, minXRotation, maxXRotation);


                xAxisFollower.localRotation = Quaternion.Euler(xRotation, 0, 0);

            }
            /*
            else
            {
                xRotation = xAxisFollower.localEulerAngles.x;
                xRotation = Mathf.Clamp(xRotation, minXRotation, maxXRotation);

                xAxisFollower.transform.localRotation = Quaternion.Euler(xRotation, 0, 0);
            }*/
        }


        void MoveCamera(Vector2 direction)
        {
            if (!isDragging)
            {
                //focusPos = player.transform.position;
                camRotation = new Vector3(direction.y, -direction.x, 0);
            }

        }

        public void FocusOnPlayer(Vector3 point) => focusPos = point;

        void DragCamera(InputAction.CallbackContext context)
        {
            if (!isDragging) { return; }


            //camRotation = new(context.ReadValue<Vector2>().x, context.ReadValue<Vector2>().y, 0);

            camRotation = Vector3.zero;


            Vector2 movePos2D = context.ReadValue<Vector2>();
            Vector3 movePos = new Vector3(movePos2D.x, movePos2D.y, 0);
            
            if(Physics.Raycast(follower.position, - Vector3.up, 1f, whatIsGround))
            {
                if(movePos.y < 0)
                {
                    movePos.y = 0;
                }
            }

            float yPos =  movePos.y;
            /*
            if(transform.position.y + yPos <= minYPosition)
                yPos = 0;*/

            transform.localPosition += (-Camera.main.transform.right * movePos.x) * cameraSpeed* Time.deltaTime;
            transform.localPosition += (-transform.up * yPos) * cameraSpeed * Time.deltaTime;

            focusPos = transform.position;
        }



        void FocusOnPoint(InputAction.CallbackContext context)
        {
            if(context.performed)
            {
                if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, 300f))
                {
                    if(hit.transform.TryGetComponent(out HexTile tile))
                    {
                        focusPos = tile.TopPosition;
                    }
                }
            }
        }


        void ZoomCamera(Vector2 zoom)
        {
            if (offset.z + zoom.y * zoomSpeed * 0.01f >= 0) zoom = -zoom;
            offset += follower.InverseTransformDirection(0.01f * -zoom.y * zoomSpeed * -follower.transform.forward.normalized);
        }
    }
}
