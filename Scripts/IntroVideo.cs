using MoreMountains.Tools;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Video;

namespace NuclearWinter
{
    public class IntroVideo : MonoBehaviour
    {

        private bool canSkip;

        [SerializeField] private string GameScene;
        [Required]
        [SerializeField]
        private VideoPlayer videoPlayer;

        public void SetSkipAvailable() => canSkip = true;


        private PlayerInput playerInput;
        private void OnEnable()
        {
            playerInput = new PlayerInput();
            playerInput.Enable();

            playerInput.UI.Cancel.performed += _ => Skip();
            videoPlayer.loopPointReached += _ => Skip();
        }

        private void OnDisable()
        {
            playerInput.UI.Cancel.performed -= _ => Skip();
            videoPlayer.loopPointReached -= _ => Skip();

            playerInput.Disable();
        }

        /// <summary>
        /// Skip the cutscene...
        /// </summary>
        private void Skip()
        {
            if (canSkip)
            {
                MMSceneLoadingManager.LoadScene(GameScene, "LoadingScreen");
            }
        }
    }
}
