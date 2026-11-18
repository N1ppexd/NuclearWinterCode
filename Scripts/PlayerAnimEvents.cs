using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace NuclearWinter
{
    public class PlayerAnimEvents : MonoBehaviour
    {

        [Required]
        [SerializeField]
        private Player player;


        public void Footstep() => player?.MoveFeedback();
    }
}
