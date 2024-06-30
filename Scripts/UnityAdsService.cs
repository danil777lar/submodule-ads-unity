using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Serialization;

namespace Larje.Core.Services
{
    [BindService(typeof(IAdsService))]
    public class UnityAdsService : Service, IAdsService, IUnityAdsInitializationListener
    {
        [Header("System")]
        [SerializeField, Min(0f)] private float delayBetweenAds = 20f;
        [SerializeField] private bool testMode = true;
        [SerializeField] private bool logsEnabled = true;
        [SerializeField] private bool enableAds = true;
        
        [Header("Keys")] 
        [SerializeField] private Keys androidKeys;
        [SerializeField] private Keys iosKeys;
        
        [Header("Banner")] 
        [SerializeField] private bool useBanner;
        [SerializeField] private BannerPosition bannerPosition = BannerPosition.BOTTOM_CENTER;

        private bool _blockInterstitial;
        private Keys _keys;

        private UnityAdsUniversal _interstitial;
        private UnityAdsUniversal _rewarded;
        private UnityAdsBanner _banner;

        public bool Initialized { get; private set; }
        public bool InterstitialAdAvailable => Initialized && _interstitial.Loaded;
        public bool RewardedAdAvailable => Initialized && _rewarded.Loaded;
        public bool BannerShowing => _banner.Showing;
        public float BannerHeight => Screen.dpi / 160f * 50f;
        
        public event Action EventBannerShown;
        public event Action EventBannerHidden;

        public override void Init()
        {
#if UNITY_IOS
            _keys = iosKeys;
#elif UNITY_ANDROID
            _keys = androidKeys;
#elif UNITY_EDITOR
            _keys = androidKeys;
#endif

            if (!Advertisement.isInitialized && Advertisement.isSupported)
            {
                _interstitial = new UnityAdsUniversal(_keys.InterstitialId, logsEnabled);
                
                _rewarded = new UnityAdsUniversal(_keys.RewardedId, logsEnabled);
                
                _banner = new UnityAdsBanner(_keys.BannerId, bannerPosition, logsEnabled);
                _banner.EventShown += () => EventBannerShown?.Invoke();
                _banner.EventHidden += () => EventBannerHidden?.Invoke();

                Advertisement.Initialize(_keys.GameId, testMode, this);
            }
        }

        public void OnInitializationComplete()
        {
            Initialized = true;

            if (enableAds)
            {
                _interstitial.LoadAd();
                _rewarded.LoadAd();
                _banner.LoadBanner();
            }

            Debug.Log("Unity Ads initialization complete.");
        }

        public void OnInitializationFailed(UnityAdsInitializationError error, string message)
        {
            Debug.Log($"Unity Ads Initialization Failed: {error.ToString()} - {message}");
        }

        public void ShowInterstitial()
        {
            if (_blockInterstitial)
            {
                return;
            }
            
            if (Initialized)
            {
                _interstitial.ShowAd();

                _blockInterstitial = true;
                DOVirtual.DelayedCall(delayBetweenAds, () => _blockInterstitial = false, true);
            }
        }

        public void ShowRewarded(Action onAdShowStart, Action onAdShowClick, Action onAdShowComplete,
            Action onAdShowFailed)
        {
            if (Initialized)
            {
                _rewarded.ShowAd(onAdShowStart, onAdShowClick, onAdShowComplete, onAdShowFailed);
            }
        }

        [Serializable]
        private class Keys
        {
            [field: SerializeField] public string GameId { get; private set; }
            [Header("Ads")] 
            [field: SerializeField] public string InterstitialId;
            [field: SerializeField] public string RewardedId;
            [field: SerializeField] public string BannerId;
        }
    }
}