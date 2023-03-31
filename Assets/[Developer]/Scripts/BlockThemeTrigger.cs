using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class BlockThemeTrigger : MonoBehaviour
{
    public Sprite Light, Dark;

    private void Awake()
    {
        Light = Resources.Load<Sprite>("Light");
        Dark = Resources.Load<Sprite>("Dark");
        TriggerThemeColor();
    }    
    public void TriggerThemeColor()
    {
        if (DDOL.Theme == 0) { transform.GetComponent<Image>().sprite = Light; }
        else { transform.GetComponent<Image>().sprite = Dark; }
    }
}
