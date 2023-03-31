/***************************************************************************************
 * THIS SCRIPT IS RESPONSIBLE FOR HANDLING IN-GAME ADS. YOU CALL RELAVANT METHODS FROM
 * THIS SCRIPT TO SHOW BANNER ADS, INTERSTITIAL AND REWARDED VIDEO ADS. ADMANAGER IS 
 * DEPENDENT ON THE BASEADMANAGER. A METHOD CALLED TO ADMANAGER WILL GET FORWARDED TO
 * BASE ADMANAGER. BASE ADMANAGER WILL BE BASED ON YOUR SETUP AND SELECTION OF YOUR
 * AD NETWORK SELECTION.
 ***************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class AdManager : Singleton<AdManager> 
{
    // Register this event to get Reward completion callback.
    public static event Action<bool> OnRewardedFinishedEvent;
    bool adsAllowed = true;

    void Start() {
        // If remove ads in-app purchased, interstitial and banner won't be loaded.
        if(PlayerPrefs.HasKey("isAdFree")) {
            adsAllowed = false;
        }
    }

    // Check if ad can be shown or not. If remove ads in-app purchased, this will return false, else true.
    public bool isAdsAllowed() {
        return adsAllowed;
    }

    // This will be called on successfully purchase of remove-ads IAP.
    public void SetAdFree() {
        adsAllowed = false;
        BaseAdManager.Instance.HideBanner();
    }

    // Shows Banner.
    public void ShowBanner() {
        BaseAdManager.Instance.ShowBanner();
    }

    // Hides the banner ad.
    public void HideBanner() {
        BaseAdManager.Instance.HideBanner();
    }

    // Checks if Interstitial is loaded and available. 
    public bool isInterstitialAvailable() {
		return BaseAdManager.Instance.isInterstitialAvailable();
	}

    // Interstitial or skippable video will load on calling this method.
    public void ShowInterstitial() {
        BaseAdManager.Instance.ShowInterstitial();
    }

     // Checks if Rewarded Video is loaded and available. 
    public bool isRewardedAvailable() {
		return BaseAdManager.Instance.isRewardedAvailable();
	}
    
    // Rewarded video will be shown on calling this method.
    public void ShowRewarded() {
        BaseAdManager.Instance.ShowRewarded();
    }  

    // All the registered callback for "OnRewardedFinishedEvent" will get called on completion of rewarded video.
    // isCompleted will be false if video not completed for any reason.
    public void OnRewardedFinished(bool isCompleted) {
        if(OnRewardedFinishedEvent != null) {
            OnRewardedFinishedEvent.Invoke(isCompleted);
        }
    }

    // All the registered callback for "OnInterstitialFinished" will get called on completion of interstitial video.
    // isCompleted will be false if interstitial didn't loaded for any reason.
    public void OnInterstitialFinished(bool isCompleted) {

    }
}
