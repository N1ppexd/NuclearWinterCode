using MoreMountains.Feedbacks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NuclearWinter
{
    public class ButtonHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [SerializeField] private MMF_Player hoverPlayer;

        public void OnPointerEnter(PointerEventData eventData)
        {
            hoverPlayer.Direction = MMFeedbacks.Directions.TopToBottom;
            hoverPlayer.PlayFeedbacks();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            hoverPlayer.Direction = MMFeedbacks.Directions.BottomToTop;
            hoverPlayer.PlayFeedbacks();
        }

        public void OnSelect(BaseEventData eventData)
        {
            hoverPlayer.Direction = MMFeedbacks.Directions.TopToBottom;
            hoverPlayer.PlayFeedbacks();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            hoverPlayer.Direction = MMFeedbacks.Directions.BottomToTop;
            hoverPlayer.PlayFeedbacks();
        }

    }
}
