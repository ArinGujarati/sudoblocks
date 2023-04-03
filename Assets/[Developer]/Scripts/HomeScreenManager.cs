using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.iOS;
public class HomeScreenManager : MonoBehaviour
{
    public static HomeScreenManager Instance;
    public bool StartGame, IsGameOverOrWin;
    public GameObject homescree;
    public GameObject MusicObject, canvas, ShopPanel;
    public Text ScoreText;
    public Sprite[] OnOff;
    public GameObject[] HomeScreenThemeObject;
    public Sprite[] Light_HomeScreen, Dark_HomeScreen;
    public GameObject LevelsPrefabs;
    public GameObject LevelLast;
    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        Input.multiTouchEnabled = false;
        Time.timeScale = 1;
        HomeScreenThemeSet();
    }
    public void SoundOnOff()
    {
        DDOL.Instance.ButtonClick();
        if (DDOL.SoundVolume == 1)
        {
            DDOL.SoundVolume = 0;
            DDOL.Instance.backgrounssource.Stop();
            int Index = 1;
            if (DDOL.Theme == 0) { Index = 1; }
            else { Index = 3; }
            MusicObject.GetComponent<Image>().sprite = OnOff[Index];
        }
        else
        {
            DDOL.SoundVolume = 1;
            DDOL.Instance.backgrounssource.Play();
            int Index = 0;
            if (DDOL.Theme == 0) { Index = 0; }
            else { Index = 2; }
            MusicObject.GetComponent<Image>().sprite = OnOff[Index];
        }
    }
    public void HomeScreenThemeSet()
    {
        ScoreText.text = "Score: " + DDOL.Score;
        HomeScreenThemeObject[10].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + DDOL.Change;
        HomeScreenThemeObject[11].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + DDOL.Rotate;
        HomeScreenThemeObject[12].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + DDOL.Undo;
        HomeScreenThemeObject[13].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + DDOL.destroy;
        if (DDOL.Theme == 0)
        {
            HomeScreenThemeObject[0].GetComponent<Image>().sprite = Light_HomeScreen[0];
            HomeScreenThemeObject[1].GetComponent<Image>().sprite = Light_HomeScreen[1];
            HomeScreenThemeObject[2].GetComponent<Image>().sprite = Light_HomeScreen[2];
            HomeScreenThemeObject[3].GetComponent<Image>().sprite = Light_HomeScreen[3];
            HomeScreenThemeObject[4].GetComponent<Image>().sprite = Light_HomeScreen[4];
            HomeScreenThemeObject[5].GetComponent<Image>().sprite = Light_HomeScreen[5];
            HomeScreenThemeObject[6].GetComponent<Image>().sprite = Light_HomeScreen[6];
            HomeScreenThemeObject[7].GetComponent<Image>().sprite = Light_HomeScreen[7];
            HomeScreenThemeObject[8].GetComponent<Image>().sprite = Light_HomeScreen[8];
            HomeScreenThemeObject[9].GetComponent<Image>().sprite = Light_HomeScreen[9];
            HomeScreenThemeObject[10].GetComponent<Image>().sprite = Light_HomeScreen[10];
            HomeScreenThemeObject[11].GetComponent<Image>().sprite = Light_HomeScreen[11];
            HomeScreenThemeObject[12].GetComponent<Image>().sprite = Light_HomeScreen[12];
            HomeScreenThemeObject[13].GetComponent<Image>().sprite = Light_HomeScreen[13];
        }
        else
        {
            HomeScreenThemeObject[0].GetComponent<Image>().sprite = Dark_HomeScreen[0];
            HomeScreenThemeObject[1].GetComponent<Image>().sprite = Dark_HomeScreen[1];
            HomeScreenThemeObject[2].GetComponent<Image>().sprite = Dark_HomeScreen[2];
            HomeScreenThemeObject[3].GetComponent<Image>().sprite = Dark_HomeScreen[3];
            HomeScreenThemeObject[4].GetComponent<Image>().sprite = Dark_HomeScreen[4];
            HomeScreenThemeObject[5].GetComponent<Image>().sprite = Dark_HomeScreen[5];
            HomeScreenThemeObject[6].GetComponent<Image>().sprite = Dark_HomeScreen[6];
            HomeScreenThemeObject[7].GetComponent<Image>().sprite = Dark_HomeScreen[7];
            HomeScreenThemeObject[8].GetComponent<Image>().sprite = Dark_HomeScreen[8];
            HomeScreenThemeObject[9].GetComponent<Image>().sprite = Dark_HomeScreen[9];
            HomeScreenThemeObject[10].GetComponent<Image>().sprite = Dark_HomeScreen[10];
            HomeScreenThemeObject[11].GetComponent<Image>().sprite = Dark_HomeScreen[11];
            HomeScreenThemeObject[12].GetComponent<Image>().sprite = Dark_HomeScreen[12];
            HomeScreenThemeObject[13].GetComponent<Image>().sprite = Dark_HomeScreen[13];
        }
        if (DDOL.SoundVolume == 1)
        {
            int Index = 0;
            if (DDOL.Theme == 0) { Index = 0; }
            else { Index = 2; }
            MusicObject.GetComponent<Image>().sprite = OnOff[Index];
        }
        else
        {
            int Index = 1;
            if (DDOL.Theme == 0) { Index = 1; }
            else { Index = 3; }
            MusicObject.GetComponent<Image>().sprite = OnOff[Index];
        }
    }
    void DelayClone()
    {
        LevelLast = Instantiate(LevelsPrefabs, canvas.transform);
    }
    public void SetLevelNumberOpen()
    {
        Time.timeScale = 1;
        IsGameOverOrWin = false;
        Invoke(nameof(DelayClone), .05f);
        homescree.SetActive(false);
    }
    public void GameOver()
    {
        if (!IsGameOverOrWin)
        {
            Time.timeScale = 0;
            DDOL.Instance.OverClick();
            IsGameOverOrWin = true;
        }
    }
    public void GameWin()
    {
        if (!IsGameOverOrWin)
        {
            IsGameOverOrWin = true;
            DDOL.Instance.GameWinClick();
        }
    }

    public void ButtonClick(string Value)
    {
        switch (Value)
        {
            case "play":
                DDOL.Instance.ButtonClick();
                SetLevelNumberOpen();

                break;
            case "backtohome":
                DDOL.Instance.ButtonClick();
                ShopPanel.SetActive(false);
                break;
            case "Shop":
                DDOL.Instance.ButtonClick();
                ShopPanel.SetActive(true);
                break;
            case "Home":
                DDOL.Instance.ButtonClick();
                IsGameOverOrWin = false;
                Time.timeScale = 1;
                HomeScreenThemeSet();
                homescree.SetActive(true);
                if (LevelLast != null) Destroy(LevelLast);
                break;
            case "Share":
                DDOL.Instance.ButtonClick();
                new NativeShare().SetTitle("Share");
                new NativeShare().SetSubject("Share It").SetText("Share app & support us").Share();
                break;
            case "Setting":
                DDOL.Instance.ButtonClick();
                break;
            case "RateUs":
                DDOL.Instance.ButtonClick();
#if UNITY_ANDROID
                Application.OpenURL("https://play.google.com/store/apps/details?id=" + Application.identifier);
#elif UNITY_IOS
        Device.RequestStoreReview();
#endif
                break;
            case "Restat":
                DDOL.Instance.ButtonClick();
                Destroy(LevelLast);
                SetLevelNumberOpen();
                break;
            case "Next":
                DDOL.Instance.ButtonClick();
                Destroy(LevelLast);
                HomeScreenThemeSet();
                SetLevelNumberOpen();
                break;
            case "Undo":
                if (DDOL.Score >= 20)
                {                    
                    DDOL.Instance.ButtonClick();
                    DDOL.Score -= 20;
                    DDOL.Undo++;
                    ScoreText.text = "Score: " + DDOL.Score;
                    HomeScreenThemeObject[12].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + DDOL.Undo;
                }
                break;
            case "Destory":
                if (DDOL.Score >= 20)
                {
                    DDOL.Instance.ButtonClick();
                    DDOL.Score -= 20;
                    DDOL.destroy++;
                    ScoreText.text = "Score: " + DDOL.Score;
                    HomeScreenThemeObject[13].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + DDOL.destroy;
                }
                break;
            case "Rotate":
                if (DDOL.Score >= 20)
                {
                    DDOL.Instance.ButtonClick();
                    DDOL.Score -= 20;
                    DDOL.Rotate++;
                    ScoreText.text = "Score: " + DDOL.Score;
                    HomeScreenThemeObject[11].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + DDOL.Rotate;
                }
                break;
            case "Change":
                if (DDOL.Score >= 20)
                {
                    DDOL.Instance.ButtonClick();
                    DDOL.Score -= 20;
                    DDOL.Change++;
                    ScoreText.text = "Score: " + DDOL.Score;
                    HomeScreenThemeObject[10].transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + DDOL.Change;
                }
                break;            
        }
    }
}
