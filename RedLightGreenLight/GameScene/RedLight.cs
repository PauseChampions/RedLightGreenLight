using RedLightGreenLight.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace RedLightGreenLight.GameScene
{
    internal class RedLight : MonoBehaviour, IInitializable, IDisposable
    {
        private AudioTimeSyncController audioTimeSyncController;
        private BeatmapObjectCallbackController beatmapObjectCallbackController;
        private Judge judge;
        private System.Random rdm;
        private Queue<float> pausePoints;

        private bool isEnabling;

        [Inject]
        public void Construct(AudioTimeSyncController audioTimeSyncController, BeatmapObjectCallbackController beatmapObjectCallbackController, Judge judge)
        {
            this.audioTimeSyncController = audioTimeSyncController;
            this.beatmapObjectCallbackController = beatmapObjectCallbackController;
            this.judge = judge;
        }

        public void Initialize()
        {
            rdm = new System.Random();

            pausePoints = new Queue<float>();
            float songLength = audioTimeSyncController.songLength;
            int counter = 0;

            while (++counter * PluginConfig.Instance.PauseDelay < songLength)
            {
                float minTime, maxTime;
                minTime = counter * PluginConfig.Instance.PauseDelay;
                maxTime = minTime + PluginConfig.Instance.PauseDelay;
                pausePoints.Enqueue(GetRandomFloatFromMinMax(minTime, maxTime));
                float extraPauseOdds = .5f;
                float oddIncrement = extraPauseOdds / 2;
                while (rdm.NextDouble() >= extraPauseOdds)
                {
                    pausePoints.Enqueue(GetRandomFloatFromMinMax(minTime, maxTime));
                    extraPauseOdds += oddIncrement;
                    oddIncrement /= 2;
                }
            }

            judge.TimerStartedEvent += OnRedLight;
            judge.TimerStoppedEvent += OnGreenLight;
        }

        public void Dispose()
        {
            judge.TimerStartedEvent -= OnRedLight;
            judge.TimerStoppedEvent -= OnGreenLight;
        }

        private float GetRandomFloatFromMinMax(float min, float max) => (float)(rdm.NextDouble() * (min - max) + min);

        public void Update()
        {
            if (audioTimeSyncController.songTime >= pausePoints.Peek())
            {
                if (isEnabling)
                {
                    pausePoints.Dequeue();
                }
                else
                {
                    judge.StartTimer(PluginConfig.Instance.RedLightTime);
                    audioTimeSyncController.Pause();
                    isEnabling = true;
                    pausePoints.Dequeue();
                }
            }
        }

        private void OnRedLight()
        {
            isEnabling = false;

            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event0, 5, 1));
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event2, 5, 1));
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event3, 5, 1));
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event4, 5, 1));

            enabled = pausePoints.Count > 0;
        }

        private void OnGreenLight()
        {
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event0, 1, 1));
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event2, 1, 0));
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event3, 1, 0));
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event4, 1, 1));
            
            audioTimeSyncController.Resume();
        }
    }
}
