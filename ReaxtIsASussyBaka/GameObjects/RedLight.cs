using ReaxtIsASussyBaka.Configuration;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

namespace ReaxtIsASussyBaka.GameObjects
{
    internal class RedLight : MonoBehaviour, IInitializable, IDisposable
    {
        private AudioTimeSyncController audioTimeSyncController;
        private BeatmapObjectCallbackController beatmapObjectCallbackController;
        private Timer timer;
        private System.Random rdm;
        private Queue<float> pausePoints;

        [Inject]
        public void Construct(AudioTimeSyncController audioTimeSyncController, BeatmapObjectCallbackController beatmapObjectCallbackController, Timer timer)
        {
            this.audioTimeSyncController = audioTimeSyncController;
            this.beatmapObjectCallbackController = beatmapObjectCallbackController;
            this.timer = timer;
        }

        public void Initialize()
        {
            rdm = new System.Random();
            timer.TimerStoppedEvent += OnBlueLight;

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
                    extraPauseOdds = extraPauseOdds + oddIncrement;
                    oddIncrement /= 2;
                }
            }
        }

        public void Dispose()
        {
            timer.TimerStoppedEvent -= OnBlueLight;
        }

        private float GetRandomFloatFromMinMax(float min, float max) => (float)(rdm.NextDouble() * (min - max) + min);

        public void Update()
        {
            if (audioTimeSyncController.songTime >= pausePoints.Peek())
            {
                OnRedLight();
            }
        }

        private void OnRedLight()
        {
            audioTimeSyncController.Pause();
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event0, 5, 1));
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event2, 5, 1));
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event3, 5, 1));
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event4, 5, 1));
            timer.StartTimer(PluginConfig.Instance.RedLightTime);
            pausePoints.Dequeue();

            enabled = pausePoints.Count > 0;
        }

        private void OnBlueLight()
        {
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event0, 1, 1));
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event2, 1, 0));
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event3, 1, 0));
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event4, 1, 1));
            audioTimeSyncController.Resume();
        }
    }
}
