using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DDOL : MonoBehaviour
{
    public static DDOL Instance;
    public AudioClip BtnClick, Oversound,winClip;
    public AudioSource backgrounssource,effectssource;
    private void Start()
    {
        if (Instance == null)
        {
            DontDestroyOnLoad(gameObject);
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        if (SoundVolume == 1)
        {
            backgrounssource.Play();
            effectssource.Play();
        }
        else
        {
            backgrounssource.Stop();
            effectssource.Stop();
        }
    }

    public void ButtonClick()
    {
        if (SoundVolume == 1)
        {
            effectssource.PlayOneShot(BtnClick);
        }
    }
    public void OverClick()
    {
        if (SoundVolume == 1)
        {
            effectssource.PlayOneShot(Oversound);
        }
    }
    public void GameWinClick()
    {
        if (SoundVolume == 1)
        {
            effectssource.PlayOneShot(winClip);
        }
    }
    public static int SoundVolume
    {
        get => PlayerPrefs.GetInt("Sound", 1);
        set => PlayerPrefs.SetInt("Sound", value);
    }
    public static int Theme
    {
        get => PlayerPrefs.GetInt("Theme", 0);
        set => PlayerPrefs.SetInt("Theme", value);
    }
    public static int Undo
    {
        get => PlayerPrefs.GetInt("Undo", 3);
        set => PlayerPrefs.SetInt("Undo", value);
    }
    public static int destroy
    {
        get => PlayerPrefs.GetInt("Destroy", 3);
        set => PlayerPrefs.SetInt("Destroy", value);
    }
    public static int Rotate
    {
        get => PlayerPrefs.GetInt("Rotate", 3);
        set => PlayerPrefs.SetInt("Rotate", value);
    }
    public static int Change
    {
        get => PlayerPrefs.GetInt("Change", 3);
        set => PlayerPrefs.SetInt("Change", value);
    }

}
