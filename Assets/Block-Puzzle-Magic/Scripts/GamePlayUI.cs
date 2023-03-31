using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GamePlayUI : Singleton<GamePlayUI>
{

    [SerializeField] private GameObject alertWindow;
    Text txtAlertText;
    public GameOverReason currentGameOverReson;
    public GameObject LossPanel, ScoreTEXT, BlockShapePanel;
    public GameObject[] Change_Theme_Object;
    public Sprite[] Light_Theme, Dark_Theme;

    /// <summary>
    /// Start this instance.
    /// </summary>
    void Start()
    {
        txtAlertText = alertWindow.transform.GetChild(0).GetComponentInChildren<Text>();
        SetTheme();
    }

    public void OnPauseButtonPressed()
    {
        if (InputManager.Instance.canInput())
        {
            AudioManager.Instance.PlayButtonClickSound();
            StackManager.Instance.pauseSceen.SetActive(true);
        }
    }
    public void HomeButtonClick()
    {
        HomeScreenManager.Instance.ButtonClick("Home");
    }
    public void RestartLevelClick()
    {
        HomeScreenManager.Instance.ButtonClick("Restat");

    }
    public void ThemButtonClick()
    {
        if (DDOL.Theme == 0) { DDOL.Theme = 1; }
        else { DDOL.Theme = 0; }
        SetTheme();
    }
    public void SetTheme()
    {
        if (DDOL.Theme == 0)
        {
            Change_Theme_Object[0].GetComponent<Image>().sprite = Light_Theme[0];
            Change_Theme_Object[1].GetComponent<Image>().sprite = Light_Theme[1];
            Change_Theme_Object[2].GetComponent<Image>().sprite = Light_Theme[2];
            Change_Theme_Object[3].GetComponent<Image>().sprite = Light_Theme[3];
            Change_Theme_Object[4].GetComponent<Image>().sprite = Light_Theme[4];
            Change_Theme_Object[5].GetComponent<Image>().sprite = Light_Theme[5];
            Change_Theme_Object[6].GetComponent<Image>().sprite = Light_Theme[6];
            Change_Theme_Object[7].GetComponent<Image>().sprite = Light_Theme[9];
            Change_Theme_Object[8].GetComponent<Image>().color = Color.black;
            Change_Theme_Object[9].GetComponent<Image>().color = Color.black;
            Change_Theme_Object[10].GetComponent<Image>().color = Color.black;
            Change_Theme_Object[11].GetComponent<Image>().color = Color.black;
            bool IsFirstBlock = false;
            foreach (var item in GameBoardGenerator.Instance.AllBlock)
            {
                if (!IsFirstBlock) { item.GetComponent<Image>().sprite = Light_Theme[7]; IsFirstBlock = true; }
                else { item.GetComponent<Image>().sprite = Light_Theme[8]; IsFirstBlock = false; }
                if (item.transform.GetChild(0).GetComponent<Image>().sprite != null)
                {
                    item.transform.GetChild(0).GetComponent<Image>().sprite = Light_Theme[10];
                }
            }
            foreach (var item in BlockShapeSpawner.Instance.ShapeContainers)
            {
                if (item.childCount != 0)
                {
                    foreach (Transform sec in item.transform.GetChild(0).transform)
                    {
                        sec.transform.GetComponent<Image>().sprite = Light_Theme[10];
                    }
                }
            }
        }
        else
        {
            Change_Theme_Object[0].GetComponent<Image>().sprite = Dark_Theme[0];
            Change_Theme_Object[1].GetComponent<Image>().sprite = Dark_Theme[1];
            Change_Theme_Object[2].GetComponent<Image>().sprite = Dark_Theme[2];
            Change_Theme_Object[3].GetComponent<Image>().sprite = Dark_Theme[3];
            Change_Theme_Object[4].GetComponent<Image>().sprite = Dark_Theme[4];
            Change_Theme_Object[5].GetComponent<Image>().sprite = Dark_Theme[5];
            Change_Theme_Object[6].GetComponent<Image>().sprite = Dark_Theme[6];
            Change_Theme_Object[7].GetComponent<Image>().sprite = Dark_Theme[9];
            Change_Theme_Object[8].GetComponent<Image>().color = Color.white;
            Change_Theme_Object[9].GetComponent<Image>().color = Color.white;
            Change_Theme_Object[10].GetComponent<Image>().color = Color.white;
            Change_Theme_Object[11].GetComponent<Image>().color = Color.white;         
            bool IsFirstBlock = false;
            foreach (var item in GameBoardGenerator.Instance.AllBlock)
            {
                if (!IsFirstBlock) { item.GetComponent<Image>().sprite = Dark_Theme[7]; IsFirstBlock = true; }
                else { item.GetComponent<Image>().sprite = Dark_Theme[8]; IsFirstBlock = false; }
                if (item.transform.GetChild(0).GetComponent<Image>().sprite != null)
                {
                    item.transform.GetChild(0).GetComponent<Image>().sprite = Dark_Theme[10];
                }
            }
            foreach (var item in BlockShapeSpawner.Instance.ShapeContainers)
            {
                if (item.childCount != 0)
                {
                    foreach (Transform sec in item.transform.GetChild(0).transform)
                    {
                        sec.transform.GetComponent<Image>().sprite = Dark_Theme[10];
                    }
                }
            }
        }
        ScoreTEXT.GetComponent<UIFontColor>().ColorOfScore();
    }
    public void ShowAlert()
    {
        alertWindow.SetActive(true);
        if (!IsInvoking("CloseAlert"))
        {
            Invoke("CloseAlert", 2F);
        }
    }

    /// <summary>
    /// Closes the alert.
    /// </summary>
    void CloseAlert()
    {
        alertWindow.SetActive(false);
    }

    /// <summary>
    /// Shows the rescue.
    /// </summary>
    /// <param name="reason">Reason.</param>
    public void ShowRescue(GameOverReason reason)
    {
        currentGameOverReson = reason;
        StartCoroutine(ShowRescueScreen(reason));
    }

    /// <summary>
    /// Shows the rescue screen.
    /// </summary>
    /// <returns>The rescue screen.</returns>
    /// <param name="reason">Reason.</param>
    IEnumerator ShowRescueScreen(GameOverReason reason)
    {
        #region time mode
        if (GameController.gameMode == GameMode.TIMED || GameController.gameMode == GameMode.CHALLENGE)
        {
            GamePlay.Instance.timeSlider.PauseTimer();
        }
        #endregion

        switch (reason)
        {
            case GameOverReason.OUT_OF_MOVES:
                txtAlertText.SetLocalizedTextForTag("txt-out-moves");
                break;
            case GameOverReason.BOMB_COUNTER_ZERO:
                txtAlertText.SetLocalizedTextForTag("txt-bomb-blast");
                break;
            case GameOverReason.TIME_OVER:
                txtAlertText.SetLocalizedTextForTag("txt-time-over");
                break;
        }

        yield return new WaitForSeconds(0.5F);
        alertWindow.SetActive(true);
        foreach (var item in BlockShapePanel.GetComponent<BlockShapeSpawner>().ShapeContainers)
        {
            if (item.childCount != 0) item.GetChild(0).GetComponent<Canvas>().enabled = false;
        }
        yield return new WaitForSeconds(1.5F);
        alertWindow.SetActive(false);
        Time.timeScale = 0;
        DDOL.Instance.OverClick();
        LossPanel.SetActive(true);
        StackManager.Instance.recueScreen.Activate();
    }
}

/// <summary>
/// Game over reason.
/// </summary>
public enum GameOverReason
{
    OUT_OF_MOVES = 0,
    BOMB_COUNTER_ZERO = 1,
    TIME_OVER
}