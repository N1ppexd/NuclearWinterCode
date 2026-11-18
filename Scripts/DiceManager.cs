using N1ppeUtils;
using System.Collections;
using TMPro;
using UnityEngine;

namespace NuclearWinter
{
    [RequireComponent(typeof(Player))]
    public class DiceManager : MonoBehaviour
    {


        [SerializeField] private TextMeshProUGUI diceText;
        private PlayerInput playerInput;
        private Player player;


        [SerializeField] private GameObject dicePanel;


        [SerializeField] private int amountOfDice, maxDiceValue;

        [SerializeField] private DiceThrower[] dices;

        [Space(10)]
        [SerializeField] GameEvent throwEvent, stopThrowEvent;

        [SerializeField] StringEvent diceResultEvent;


        private void OnValidate()
        {
            if(amountOfDice < 1) amountOfDice = 1;
            if(maxDiceValue < 1) maxDiceValue = 1;
        }


        private void OnEnable()
        {
            //get player
            player = transform.GetComponent<Player>();


            playerInput = new PlayerInput();
            playerInput.Enable();
            playerInput.Player.ThrowDice.performed += ctx => ThrowDice(); 

        }


        private void OnDisable()
        {

            playerInput.Player.ThrowDice.performed -= ctx => ThrowDice();
            playerInput.Disable();
        }


        public void ThrowDice()
        {
            if(Time.timeScale == 0) return;
            //cannot throw dice if the player still has moves left
            /*if(player.Moves > 0 || player.ThrowsLeft <= 0)
            {
                Debug.Log("You still have moves left");
                return;
            }*/

            if (player.ThrowsLeft <= 0) return;

            player.ThrowsLeft--;


            throwEvent.Raise();

            //stop all coroutines
            StopAllCoroutines();


            StartCoroutine(DiceAnimation());
        }



        IEnumerator DiceAnimation()
        {


            //int result = Random.Range(amountOfDice, amountOfDice * maxDiceValue);
            int result = 0;
            foreach(DiceThrower diceThrower in dices)
            {
                int currentResult = Random.Range(1, maxDiceValue);
                result += currentResult;

                diceThrower.SimulateValue(currentResult);
                yield return NiilosExtensions.GetWait(1f);
            }



            diceText.text = result.ToString();

            diceResultEvent.Raise(result.ToString());
            GameEvents.DiceResult(result);

            player.Moves = result;

            yield return NiilosExtensions.GetWait(1f);


            stopThrowEvent.Raise();
        }
    }
}
