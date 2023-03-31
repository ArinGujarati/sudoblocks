/*******************************************************************************************
	NOTE : 
	THIS IS A PLACE HOLDER EDITOR SPECIC SCRIPT CODE. IF YOU WANT TO MONETIZE GAME WITH UNITY ADS
	OR ADMOB THEN PLEAS IMPORT RESPECTIVE SDK PLUGIN. AFTER IMPORTING SDK PLESE IMPORT AD SUPPORT
	SDK FROM THE Hyperbyte/Features/AdSupport EDITOR MENU AND IMPORT REQUIRED ADSUPPORT SDK.

	IF YOU ANY QUATIONS OR ISSUE, PLEASE CONTACT US VIA FORUM OR SUPPORT EMAIL AND WE WILL HELP YOU
	FIX YOUR ISSUE.	
*******************************************************************************************/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAdManager : Singleton<BaseAdManager> {

	//Please set these values from the inspector as this is public fields. 
	//If you want to set from here anyways, please mark these fields as "[System.NonSerialized]" or private.	




	//Initialize Unity Ads. 
	void Start()
	{
		#if UNITY_ANDROID
		#elif UNITY_IOS
		#endif
	}

	//Show banner ads. Not supported in this version.
	public void ShowBanner() {
		if(AdManager.Instance.isAdsAllowed()) { 

		}
	}

	//Show banner ads. Not supported in this version.
	public void HideBanner() {
		if(AdManager.Instance.isAdsAllowed()) { 

		}
	}

	//Checks if interstial is loaded and ready to be shown.
	public bool isInterstitialAvailable() {
		return false;
	}

	//Show Intestitial. Call this method when you want to interstitial.
	public void ShowInterstitial() {
		if(AdManager.Instance.isAdsAllowed()) {
			Invoke("StartInterstitialWithDelay",0.1F);
		}
	}

	//A safe delay before starting an ad.
	void StartInterstitialWithDelay() 
	{
	}


	//A Delay based result forward just be safe. Added as just a safety bridge to prevent app from being unresponsive.
	IEnumerator OnInterstitialFinished(bool isCompleted) {
		yield return new WaitForSeconds(0.1F);
		AdManager.Instance.OnInterstitialFinished(isCompleted);
	}

	//Checks if rewarded video is loaded and ready to be shown.
	public bool isRewardedAvailable() {
		return false;
	}

	//Show Intestitial. Call this method when you want to interstitial.
	public void ShowRewarded() {
		Invoke("StartRewardedWithDelay",0.05F);
	}

	//A safe delay before starting an ad.
	void StartRewardedWithDelay() 
	{
		StartCoroutine(OnRewardedFinished(true));
	}

	//A Delay based result forward just be safe. Added as just a safety bridge to prevent app from being unresponsive.
	IEnumerator OnRewardedFinished(bool isCompleted) {
		yield return new WaitForSeconds(0.1F);
		AdManager.Instance.OnRewardedFinished(isCompleted);
	}
}