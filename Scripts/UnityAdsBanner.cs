using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Larje.Core.Services
{
    public class UnityAdsBanner
    {
        private bool _logEnabled;
        private string _id;
        private readonly BannerPosition _bannerPosition;
        
        public bool Showing { get; private set; }
        
        public event Action EventShown;
        public event Action EventHidden;

        public UnityAdsBanner(string id, BannerPosition bannerPosition, bool logEnabled)
        {
            _id = id;
            _bannerPosition = bannerPosition;
            _logEnabled = logEnabled;

            Advertisement.Banner.SetPosition(_bannerPosition);
        }

        public void LoadBanner()
        {
            BannerLoadOptions options = new BannerLoadOptions
            {
                loadCallback = OnBannerLoaded,
                errorCallback = OnBannerError
            };

            Advertisement.Banner.Load(_id, options);
        }

        private void OnBannerLoaded()
        {
            TryLog("Banner loaded");
            ShowBannerAd();
        }

        private void OnBannerError(string message)
        {
            TryLog($"Banner Error: {message}");
        }

        private void ShowBannerAd()
        {
            BannerOptions options = new BannerOptions
            {
                clickCallback = OnBannerClicked,
                hideCallback = OnBannerHidden,
                showCallback = OnBannerShown
            };

            Advertisement.Banner.Show(_id, options);
        }

        private void HideBannerAd()
        {
            Advertisement.Banner.Hide();
        }

        private void OnBannerClicked()
        {
        }

        private void OnBannerShown()
        {
            Showing = true;
            EventShown?.Invoke();
        }

        private void OnBannerHidden()
        {
            Showing = false;
            EventHidden?.Invoke();
        }

        private void TryLog(string text)
        {
            if (_logEnabled)
            {
                Debug.Log(text);
            }
        }
    }
}