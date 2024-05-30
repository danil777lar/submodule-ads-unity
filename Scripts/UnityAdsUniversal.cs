using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

namespace Larje.Core.Services
{
    public class UnityAdsUniversal : IUnityAdsLoadListener, IUnityAdsShowListener
    {
        private readonly bool _logEnabled;
        private readonly string _id;

        private Action _onAdShowStart;
        private Action _onAdShowClick;
        private Action _onAdShowComplete;
        private Action _onAdShowFailed;

        public bool Loaded { get; private set; }

        public UnityAdsUniversal(string id, bool logEnabled)
        {
            _logEnabled = logEnabled;
            _id = id;
        }

        public void LoadAd()
        {
            TryLog("Loading Ad: " + _id);

            Advertisement.Load(_id, this);
        }

        public void OnUnityAdsFailedToLoad(string id, UnityAdsLoadError error, string message)
        {
            if (id != _id) return;
            TryLog($"Error loading Ad Unit: {id} - {error.ToString()} - {message}");
        }

        public void ShowAd(Action onAdShowStart, Action onAdShowClick, Action onAdShowComplete, Action onAdShowFailed)
        {
            _onAdShowStart = onAdShowStart;
            _onAdShowClick = onAdShowClick;
            _onAdShowComplete = onAdShowComplete;
            _onAdShowFailed = onAdShowFailed;

            ShowAd();
        }

        public void ShowAd()
        {
            TryLog("Showing Ad: " + _id);

            Loaded = false;
            Advertisement.Show(_id, this);
        }

        public void OnUnityAdsAdLoaded(string id)
        {
            if (id != _id) return;

            Loaded = true;
        }

        public void OnUnityAdsShowStart(string id)
        {
            if (id != _id) return;

            _onAdShowStart?.Invoke();
        }

        public void OnUnityAdsShowClick(string id)
        {
            if (id != _id) return;

            _onAdShowClick?.Invoke();
        }

        public void OnUnityAdsShowComplete(string id, UnityAdsShowCompletionState showCompletionState)
        {
            if (id != _id) return;

            _onAdShowComplete?.Invoke();
            LoadAd();
        }

        public void OnUnityAdsShowFailure(string id, UnityAdsShowError error, string message)
        {
            if (id != _id) return;
            TryLog($"Error showing Ad Unit {id}: {error.ToString()} - {message}");

            _onAdShowFailed?.Invoke();
            LoadAd();
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