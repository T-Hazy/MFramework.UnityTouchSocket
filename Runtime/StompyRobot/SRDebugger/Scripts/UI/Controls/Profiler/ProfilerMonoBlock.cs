﻿namespace SRDebugger.UI.Controls
{
    using System;
    using SRF;
    using UnityEngine;
    using UnityEngine.UI;
#if UNITY_5_5_OR_NEWER
    using UnityEngine.Profiling;
#endif

    public class ProfilerMonoBlock : SRMonoBehaviourEx
    {
        private float _lastRefresh;

        [RequiredField]
        public Text CurrentUsedText;

        [RequiredField]
        public GameObject NotSupportedMessage;

        [RequiredField]
        public Slider Slider;

        [RequiredField]
        public Text TotalAllocatedText;
        private bool _isSupported;

        protected override void OnEnable()
        {
            base.OnEnable();

#if UNITY_5_6_OR_NEWER
            this._isSupported = Profiler.GetMonoUsedSizeLong() > 0;
#else
            _isSupported = Profiler.GetMonoUsedSize() > 0;
#endif

            this.NotSupportedMessage.SetActive(!this._isSupported);
            this.CurrentUsedText.gameObject.SetActive(this._isSupported);

            this.TriggerRefresh();
        }

        protected override void Update()
        {
            base.Update();

            if (SRDebug.Instance.IsDebugPanelVisible && (Time.realtimeSinceStartup - this._lastRefresh > 1f))
            {
                this.TriggerRefresh();
                this._lastRefresh = Time.realtimeSinceStartup;
            }
        }

        public void TriggerRefresh()
        {
            long max;
            long current;

#if UNITY_5_6_OR_NEWER
            max = this._isSupported ? Profiler.GetMonoHeapSizeLong() : GC.GetTotalMemory(false);
            current = Profiler.GetMonoUsedSizeLong();
#else
            max = _isSupported ? Profiler.GetMonoHeapSize() : GC.GetTotalMemory(false);
            current = Profiler.GetMonoUsedSize();
#endif

            var maxMb = (max >> 10);
            maxMb /= 1024; // On new line to workaround IL2CPP bug

            var currentMb = (current >> 10);
            currentMb /= 1024;

            this.Slider.maxValue = maxMb;
            this.Slider.value = currentMb;

            this.TotalAllocatedText.text = "Total: <color=#FFFFFF>{0}</color>MB".Fmt(maxMb);

            if (currentMb > 0)
            {
                this.CurrentUsedText.text = "<color=#FFFFFF>{0}</color>MB".Fmt(currentMb);
            }
        }

        public void TriggerCollection()
        {
            GC.Collect();
            this.TriggerRefresh();
        }
    }
}
