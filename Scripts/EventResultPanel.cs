using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    public class EventResultPanel : MonoBehaviour
    {
        [Required]
        [SerializeField] private EventResultInfo eventResultInfoPrefab;

        [Required]
        [SerializeField] private Transform eventResultInfoParent;


        [Required]
        [SerializeField] private Transform failurePrefab, succesPrefab;


        public void SetFailure(ActionSO action)
        {
            succesPrefab.gameObject.SetActive(false);
            failurePrefab.gameObject.SetActive(true);

            Instantiate(eventResultInfoPrefab, eventResultInfoParent).SetInfo(action.FailResources.StatSprite, action.FailResourceChange);
        }

        public void SetSucces(ActionSO action)
        {
            succesPrefab.gameObject.SetActive(true);
            failurePrefab.gameObject.SetActive(false);

            Instantiate(eventResultInfoPrefab, eventResultInfoParent).SetInfo(action.SuccesResources.StatSprite, action.SuccesResourceChange);
        }


        public void OnDisable()
        {
            succesPrefab.gameObject.SetActive(false);
            failurePrefab.gameObject.SetActive(false);


            List<EventResultInfo> eventResultInfos = new List<EventResultInfo>(eventResultInfoParent.GetComponentsInChildren<EventResultInfo>());

            foreach(EventResultInfo eventResultInfo in eventResultInfos)
            {
                Destroy(eventResultInfo.gameObject);
            }
        }
    }
}
