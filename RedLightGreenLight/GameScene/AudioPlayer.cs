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
        private readonly AudioClipAsyncLoader audioClipAsyncLoader;
        private readonly AudioTimeSyncController audioTimeSyncController;
        private readonly System.Random rdm;
        private AudioSource audioSource;

        private readonly DirectoryInfo redLighDir = new DirectoryInfo(Path.Combine(UnityGame.UserDataPath, nameof(RedLightGreenLight), "RedLight"));
        private readonly DirectoryInfo greenLightDir = new DirectoryInfo(Path.Combine(UnityGame.UserDataPath, nameof(RedLightGreenLight), "GreenLight"));
        private readonly DirectoryInfo gunDir = new DirectoryInfo(Path.Combine(UnityGame.UserDataPath, nameof(RedLightGreenLight), "Gun"));

        private FileInfo[] redLightAudioFiles, greenLightAudioFiles, gunAudioFiles;

        public event Action ClipFinishedEvent;

        public AudioPlayer(AudioClipAsyncLoader audioClipAsyncLoader, AudioTimeSyncController audioTimeSyncController)
        {
            this.audioClipAsyncLoader = audioClipAsyncLoader;
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
            AudioClip audioClip = await audioClipAsyncLoader.Load(audioToPlay.FullName);
            if (audioClip != null)
            {
                if (audioSource == null)
                {
                    audioSource = new GameObject("RedLight AudioSource").AddComponent<AudioSource>();
                    audioSource.outputAudioMixerGroup = audioTimeSyncController.GetField<AudioSource, AudioTimeSyncController>("_audioSource").outputAudioMixerGroup;
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
