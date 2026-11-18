using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;
using UnityEngine.UI;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NuclearWinter
{
    public class SettingsPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown resolutionDropdown;
        [SerializeField] private TMP_Dropdown screenmodeDropdown;
        [SerializeField] private AudioMixer audioMixer;

        private Resolution[] resolutions;

        private void Start()
        {
            resolutions = Screen.resolutions;
            resolutions = (Resolution[])resolutions.Where(resolution => resolution.refreshRateRatio.value == 60).ToArray<Resolution>();

            resolutionDropdown.ClearOptions();

            List<string> options = new List<string>();

            int resolutionIndex = 0;

            for (int i = 0; i < resolutions.Length; i++)
            {
                string option = resolutions[i].width + " x " + resolutions[i].height;
                options.Add(option);

                if (resolutions[i].width == Screen.currentResolution.width && resolutions[i].height == Screen.currentResolution.height)
                {
                    resolutionIndex = i;
                }
            }
            resolutionDropdown.AddOptions(options);
            resolutionDropdown.value = resolutionIndex;
            resolutionDropdown.RefreshShownValue();

            screenmodeDropdown.RefreshShownValue();
        }

        public void SetResolution(int chosenResolution)
        {
            Resolution resolution = resolutions[chosenResolution];

            //Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
        }

        public void SetVolume (float volume)
        {
            audioMixer.SetFloat("MasterVolume", volume);
        }
        

        public void SetScreenMode ()
        {
            //Screen.fullScreen = !Screen.fullScreen;

            int index = screenmodeDropdown.value;

            if (index == 0)
            {
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.Windowed);
            }
            else if (index == 1)
            {
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.ExclusiveFullScreen);
            }
            else if (index == 2)
            {
                Screen.SetResolution(Screen.width, Screen.height, FullScreenMode.FullScreenWindow);
            }
        }
    }
}
