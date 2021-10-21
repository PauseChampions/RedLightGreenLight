using System;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;
using IPA.Utilities;
using System.Threading;

namespace RedLightGreenLight.GameScene
{
    internal class AudioPlayer
    {
        private readonly CachedMediaAsyncLoader cachedMediaAsyncLoader;
        private readonly AudioTimeSyncController audioTimeSyncController;
        private readonly System.Random rdm;
        private AudioSource audioSource;

        private readonly DirectoryInfo redLight = new DirectoryInfo(Path.Combine(UnityGame.UserDataPath, "HelloMontrealCrew", "RedLight"));
        private readonly DirectoryInfo greenLight = new DirectoryInfo(Path.Combine(UnityGame.UserDataPath, "HelloMontrealCrew", "GreenLight"));
        private readonly DirectoryInfo prr = new DirectoryInfo(Path.Combine(UnityGame.UserDataPath, "HelloMontrealCrew", "Gun"));

        public event Action ClipFinishedEvent;

        public AudioPlayer(CachedMediaAsyncLoader cachedMediaAsyncLoader, AudioTimeSyncController audioTimeSyncController)
        {
            this.cachedMediaAsyncLoader = cachedMediaAsyncLoader;
            this.audioTimeSyncController = audioTimeSyncController;
            rdm = new System.Random();
        }

        public void PlayRedLight() => PlayClip(redLight, true);
        public void PlayGreenLight() => PlayClip(greenLight);
        public void PlayPrr() => PlayClip(prr);

        private async void PlayClip(DirectoryInfo dir, bool notifyFinished = false)
        {
            var files = dir.GetFiles();
            var file = files[rdm.Next(0, files.Length)];

            if (file.Exists)
            {
                var audioClip = await cachedMediaAsyncLoader.LoadAudioClipAsync(file.FullName, CancellationToken.None);
                if (audioClip != null)
                {
                    if (audioSource == null)
                    {
                        audioSource = new GameObject("RedLight AudioSource").AddComponent<AudioSource>();
                        audioSource.outputAudioMixerGroup = audioTimeSyncController.audioSource.outputAudioMixerGroup;
                    }
                    audioSource.PlayOneShot(audioClip, 15f);

                    if (notifyFinished)
                    {
                        await Task.Delay(audioClip.length.Milliseconds() + 250);
                        ClipFinishedEvent?.Invoke();
                    }
                }
            }
        }
    }
}
