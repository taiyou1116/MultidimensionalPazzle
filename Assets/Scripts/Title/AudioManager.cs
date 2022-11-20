using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
public class AudioManager : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    public AudioSource[] sounds;
    public AudioSource bgm;

    //SEの音量調節
    public void SetBGMBolume(Slider slider)
    {
        mixer.SetFloat("BGM",slider.value);
    }
    public void SetSEBolume(Slider slider)
    {
        mixer.SetFloat("SE",slider.value);
    }
}
