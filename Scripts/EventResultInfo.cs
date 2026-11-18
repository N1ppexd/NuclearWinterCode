using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace NuclearWinter
{
    public class EventResultInfo : MonoBehaviour
    {
        [SerializeField] private Image resourceSprite;

        [SerializeField] private TextMeshProUGUI resourceAmount;

        public void SetInfo(Sprite sprite, int amount)
        {
            resourceSprite.sprite = sprite;
            resourceAmount.text = amount.ToString();
        }
    }
}
