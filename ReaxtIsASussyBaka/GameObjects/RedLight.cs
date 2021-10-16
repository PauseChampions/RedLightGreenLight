using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Zenject;

namespace ReaxtIsASussyBaka.GameObjects
{
    internal class RedLight : MonoBehaviour, IInitializable
    {
        private AudioTimeSyncController audioTimeSyncController;
        private BeatmapObjectCallbackController beatmapObjectCallbackController;

        [Inject]
        public void Construct(AudioTimeSyncController audioTimeSyncController, BeatmapObjectCallbackController beatmapObjectCallbackController)
        {
            this.audioTimeSyncController = audioTimeSyncController;
            this.beatmapObjectCallbackController = beatmapObjectCallbackController;
        }

        public void Initialize()
        {
        }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (audioTimeSyncController.state == AudioTimeSyncController.State.Playing)
                {
                    OnRedLight();
                }
                else if (audioTimeSyncController.state == AudioTimeSyncController.State.Paused)
                {
                    OnBlueLight();
                }
            }
        }

        private void OnRedLight()
        {
            audioTimeSyncController.Pause();
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event0, 5, 1));
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event2, 5, 1));
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event3, 5, 1));
            beatmapObjectCallbackController.SendBeatmapEventDidTriggerEvent(new BeatmapEventData(audioTimeSyncController.songTime, BeatmapEventType.Event4, 5, 1));
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
