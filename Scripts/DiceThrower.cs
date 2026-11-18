using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    [RequireComponent(typeof(Rigidbody))]
    public class DiceThrower : MonoBehaviour
    {
        [System.Serializable]
        public struct DieFace
        {
            public DieFace(int value, Transform face)
            {
                faceValue = value;
                faceTransform = face;
            }
            public int faceValue;
            public Transform faceTransform;
        }


        private bool hit;


        [SerializeField] private DieFace[] faces;
        [SerializeField] private Transform dice;
        [SerializeField] private Rigidbody rb;


        [Space(10)]
        [SerializeField] private int playbackSpeed = 1;
       

        private int simulationResult;
        private bool isDone = false;

        public int SimulationResult => simulationResult;

        public Dictionary<int, Transform> faceValuePairs = new();



        private readonly List<DiceFrame> recordedFrames = new List<DiceFrame>();
        private int recordedFrameCount = 0;


        [SerializeField] private MMF_Player feedbackPlayer;
        [SerializeField] private MMF_Player hitFeedback;

        // Start is called before the first frame update
        void Start()
        {
            foreach(var face in faces)
            {
                Debug.Log("add face to valuepairs " +  face.faceValue + " and transform " + face.faceTransform);
                faceValuePairs.Add(face.faceValue, face.faceTransform);
            }

            SimulateDice();
        }

        public void SimulateValue(int value)
        {

            if (!isDone) { Debug.Log("not done"); return; }
            if (faceValuePairs[value] == null) { Debug.Log("no such key in the dictionary"); return; }
            if (faceValuePairs[simulationResult] == null) { Debug.Log($"simulation result doesnt exist. Simulation result is {simulationResult}"); return; }

            Transform desiredFace = faceValuePairs[value];
            Transform resultFace = faceValuePairs[simulationResult];

            rb.isKinematic = false;

            transform.SetPositionAndRotation(recordedFrames[0].position, recordedFrames[0].rotation);

            dice.transform.localRotation = Quaternion.Euler(0,0,0);

            //get the dot product between face normal vectors
            float dotProduct = Vector3.Dot(desiredFace.up, resultFace.up);

            if (dotProduct <= 0.99f && dotProduct >= -0.99f)
            {
                //take the angle between vectors
                float angle = Mathf.Acos(dotProduct / (desiredFace.up.magnitude * resultFace.up.magnitude));
                angle = Mathf.Rad2Deg * angle;

                Vector3 rotationAxis = Vector3.Cross(desiredFace.up, resultFace.up).normalized;

                dice.transform.Rotate(rotationAxis, angle, Space.World);
            }
            
            if(dotProduct < -0.99f)
            {
                dice.transform.Rotate(desiredFace.right, 180f, Space.World);
            }

            StartCoroutine(PlayBack(value));
        }

        private void SimulateDice()
        {
            //Set isKinematics to false so physics apply to it
            rb.isKinematic = false;

            #region randomInitialConditions
            float rotX = Random.Range(0f, 360f);
            float rotY = Random.Range(0f, 360f);
            float rotZ = Random.Range(0f, 360f);

            float forceX = Random.Range(0f, 30f);
            float forceY = Random.Range(0f, 30f);
            float forceZ = Random.Range(0f, 30f);

            float torqueX = Random.Range(0f, 10f);
            float torqueY = Random.Range(0f, 10f);
            float torqueZ = Random.Range(0f, 10f);
            #endregion

            Quaternion rotation = Quaternion.Euler(rotX, rotY, rotZ);
            Vector3 force = new Vector3(forceX, -forceY, forceZ);
            Vector3 torque = new Vector3(torqueX, torqueY, torqueZ);


            //clear the recorded rotations and positions
            recordedFrames.Clear();
            recordedFrameCount = 0;


            transform.rotation = rotation;
            rb.velocity = force;
            rb.maxAngularVelocity = 200;

            rb.AddTorque(torque, ForceMode.VelocityChange);

            Physics.simulationMode = SimulationMode.Script;

            for(int i = 0; i < 2000; i++)
            {
                if (this.hit)
                {
                    if (i > 0 && recordedFrames[i - 1].hit)
                        this.hit = false;
                }

                recordedFrames.Add(new DiceFrame()
                {
                    position = transform.position,
                    rotation = transform.rotation,
                    hit = this.hit
                });

                

                //more frames to the recording
                recordedFrameCount++;

                Physics.Simulate(Time.fixedDeltaTime);

                if (!IsMoving)
                {
                    simulationResult = GetResult(faces);
                    isDone = true;

                    if (simulationResult != 0)
                    {
                        Debug.Log($"Simulation result is {simulationResult}");
                    }
                    break;
                }
            }

            Physics.simulationMode = SimulationMode.FixedUpdate;
        }


        IEnumerator PlayBack(int desiredValue)
        {
            rb.isKinematic = true;

            isDone = false;

            for(int i = 0; i < recordedFrameCount; i += playbackSpeed)
            {
                Vector3 position = recordedFrames[i].position;
                Quaternion rotation = recordedFrames[i].rotation;

                bool currentHit = recordedFrames[i].hit;

                transform.SetPositionAndRotation(position, rotation);

                if(currentHit) hitFeedback?.PlayFeedbacks();

                yield return new WaitForFixedUpdate();
            }


            //play feedback
            MMF_FloatingText floatingText = feedbackPlayer.GetFeedbackOfType<MMF_FloatingText>();

            floatingText.Value = desiredValue.ToString();

            feedbackPlayer.PlayFeedbacks();

            isDone = true;
        }


        private int GetResult(DieFace[] faces)
        {
            float highestDot = -1;

            Transform result = null;
            int resultValue = 0;

            for (int i = 0; i < faces.Length; i++)
            {
                Transform t = faces[i].faceTransform;

                float currentDot = Vector3.Dot(faces[i].faceTransform.up, Vector3.up);

                if (currentDot > highestDot)
                {
                    highestDot = currentDot;
                    result = t;
                    resultValue = faces[i].faceValue;
                }
            }

            return resultValue;
        }

        private void OnCollisionEnter(Collision collision) => hit = true;
        private void OnCollisionExit(Collision collision) => hit = false;
        //private void OnCollisionStay(Collision collision) => hit = false;


        private bool IsMoving => rb.velocity != Vector3.zero && rb.angularVelocity != Vector3.zero;
    }


    public struct DiceFrame
    {
        public Vector3 position;
        public Quaternion rotation;
        public bool hit;
    }
}
