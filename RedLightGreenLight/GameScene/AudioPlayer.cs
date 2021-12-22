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

        private readonly DirectoryInfo redLighDir = new DirectoryInfo(Path.Combine(UnityGame.UserDataPath, "HelloMontrealCrew", "RedLight"));
        private readonly DirectoryInfo greenLightDir = new DirectoryInfo(Path.Combine(UnityGame.UserDataPath, "HelloMontrealCrew", "GreenLight"));
        private readonly DirectoryInfo gunDir = new DirectoryInfo(Path.Combine(UnityGame.UserDataPath, "HelloMontrealCrew", "Gun"));

        private FileInfo[] redLightAudioFiles, greenLightAudioFiles, gunAudioFiles;

        public event Action ClipFinishedEvent;

        public AudioPlayer(CachedMediaAsyncLoader cachedMediaAsyncLoader, AudioTimeSyncController audioTimeSyncController)
        {
            this.cachedMediaAsyncLoader = cachedMediaAsyncLoader;
            this.audioTimeSyncController = audioTimeSyncController;
            redLightAudioFiles = redLighDir.GetFiles();
            greenLightAudioFiles = greenLightDir.GetFiles();
            gunAudioFiles = gunDir.GetFiles();
            rdm = new System.Random();
        }

        public void PlayRedLight() => PlayClip(redLightAudioFiles, true);
        public void PlayGreenLight() => PlayClip(greenLightAudioFiles);
        public void PlayGun() => PlayClip(gunAudioFiles);

        private async void PlayClip(FileInfo[] audioFiles, bool notifyFinished = false)
        {
            FileInfo audioToPlay = audioFiles[rdm.Next(0, audioFiles.Length)];
            AudioClip audioClip = await cachedMediaAsyncLoader.LoadAudioClipAsync(audioToPlay.FullName, CancellationToken.None);
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
                    await Task.Delay(audioClip.length.TotalSeconds() * 1000 + audioClip.length.Milliseconds() + 125);
                    ClipFinishedEvent?.Invoke();
                }
            }
        }
    }
}
