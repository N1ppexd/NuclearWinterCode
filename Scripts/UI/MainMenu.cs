using MoreMountains.Tools;
using UnityEngine;

namespace NuclearWinter
{
    public class MainMenu : MonoBehaviour
    {
        public string GameScene;
        public void PlayGame() => MMSceneLoadingManager.LoadScene(GameScene, "LoadingScreen");
        public void QuitGame() => Application.Quit();
    }
}
