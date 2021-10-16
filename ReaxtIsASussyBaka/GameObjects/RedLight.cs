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
        private GameEnergyCounter gameEnergyCounter;
        private Timer timer;
        private System.Random rdm;
        private Queue<float> pausePoints;

        private Transform leftController;
        private Transform rightController;

        private const float positionRange = 1f;
        private const float rotationRange = 15f;

        private Vector3 leftControllerOriginalPos;
        private Vector3 leftControllerOriginalRot;

        private Vector3 rightControllerOriginalPos;
        private Vector3 rightControllerOriginalRot;

        [Inject]
        public void Construct(AudioTimeSyncController audioTimeSyncController, BeatmapObjectCallbackController beatmapObjectCallbackController, Timer timer,
            SaberManager saberManager, GameEnergyCounter gameEnergyCounter)
        {
            this.audioTimeSyncController = audioTimeSyncController;
            this.beatmapObjectCallbackController = beatmapObjectCallbackController;
            this.gameEnergyCounter = gameEnergyCounter;
            this.timer = timer;

            leftController = saberManager?.leftSaber.GetComponentInParent<VRController>().transform;
            rightController = saberManager?.rightSaber.GetComponentInParent<VRController>().transform;
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

            if (timer.enabled)
            {
                if (!(PositionAndRotationWithinRange(leftController, leftControllerOriginalPos, leftControllerOriginalRot) &&
                    PositionAndRotationWithinRange(rightController, rightControllerOriginalPos, rightControllerOriginalRot)))
                {
                    gameEnergyCounter.ProcessEnergyChange(-gameEnergyCounter.energy);
                }
            }
        }

        private bool PositionAndRotationWithinRange(Transform controller, Vector3 originalPos, Vector3 originalRot)
        {
            bool xPositionWithinRange = originalPos.x + positionRange > controller.position.x && originalPos.x - positionRange < controller.position.x;
            bool yPositionWithinRange = originalPos.y + positionRange > controller.position.y && originalPos.y - positionRange < controller.position.y;
            bool zPositionWithinRange = originalPos.z + positionRange > controller.position.z && originalPos.z - positionRange < controller.position.z;

            bool xRotationWithinRange = originalRot.x + rotationRange > controller.eulerAngles.x && originalRot.x - rotationRange < controller.eulerAngles.x;
            bool yRotationWithinRange = originalRot.y + rotationRange > controller.eulerAngles.y && originalRot.y - rotationRange < controller.eulerAngles.y;
            bool zRotationWithinRange = originalRot.z + rotationRange > controller.eulerAngles.z && originalRot.z - rotationRange < controller.eulerAngles.z;

            return xPositionWithinRange && yPositionWithinRange && zPositionWithinRange && xRotationWithinRange && yRotationWithinRange && zRotationWithinRange;
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

            leftControllerOriginalPos = leftController.position;
            leftControllerOriginalRot = leftController.eulerAngles;

            rightControllerOriginalPos = rightController.position;
            rightControllerOriginalRot = rightController.eulerAngles;

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
