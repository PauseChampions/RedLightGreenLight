using System;
using UnityEngine;
using System.Threading.Tasks;
using System.IO;
using IPA.Utilities;
using System.Reflection;
using System.Threading;

namespace ReaxtIsASussyBaka.GameScene
{
    internal class AudioPlayer
    {
        private readonly CachedMediaAsyncLoader cachedMediaAsyncLoader;
        private readonly AudioTimeSyncController audioTimeSyncController;
        private AudioSource audioSource;

        private readonly FileInfo redLight = new FileInfo(Path.Combine(UnityGame.UserDataPath, "HelloMontrealCrew", "RedLight.ogg"));
        private readonly FileInfo greenLight = new FileInfo(Path.Combine(UnityGame.UserDataPath, "HelloMontrealCrew", "GreenLight.ogg"));
        private readonly FileInfo prr = new FileInfo(Path.Combine(UnityGame.UserDataPath, "HelloMontrealCrew", "Prr.ogg"));

        public event Action ClipFinishedEvent;

        public AudioPlayer(CachedMediaAsyncLoader cachedMediaAsyncLoader, AudioTimeSyncController audioTimeSyncController)
        {
            this.cachedMediaAsyncLoader = cachedMediaAsyncLoader;
            this.audioTimeSyncController = audioTimeSyncController;
        }

        public void PlayRedLight() => PlayClip(redLight, "ReaxtIsASussyBaka.VoiceClips.RedLight.ogg", true);

        public void PlayGreenLight() => PlayClip(greenLight, "ReaxtIsASussyBaka.VoiceClips.GreenLight.ogg");
        public void PlayPrr() => PlayClip(prr, "ReaxtIsASussyBaka.VoiceClips.Prr.ogg");


        private async void PlayClip(FileInfo file, string assemblyPath, bool notifyFinished = false)
        {
            if (!file.Directory.Exists)
            {
                redLight.Directory.Create();
            }

            if (!file.Exists)
            {
                using (var manifestStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(assemblyPath))
                {
                    using (var fs = File.Create(file.FullName))
                    {
                        await manifestStream.CopyToAsync(fs);
                    }
                }
            }

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
