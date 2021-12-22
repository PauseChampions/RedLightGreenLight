using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Utilities;
using RedLightGreenLight.GameScene;
using RedLightGreenLight.Installers;
using RedLightGreenLight.UI;
using SiraUtil.Zenject;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using UnityEngine;
using IPALogger = IPA.Logging.Logger;

namespace RedLightGreenLight
{
    [Plugin(RuntimeOptions.DynamicInit)]
    public class Plugin
    {
        internal static Plugin Instance { get; private set; }
        internal static IPALogger Log { get; private set; }

        [Init]
        /// <summary>
        /// Called when the plugin is first loaded by IPA (either when the game starts or when the plugin is enabled if it starts disabled).
        /// [Init] methods that use a Constructor or called before regular methods like InitWithConfig.
        /// Only use [Init] with one Constructor.
        /// </summary>
        public Plugin(IPALogger logger, Zenjector zenjector)
        {
            Instance = this;
            Plugin.Log = logger;
            zenjector.Install(Location.Menu, Container => Container.BindInterfacesTo<ModifierViewController>().AsSingle());
            zenjector.Install<WeAreBackBackFromWhereInstaller>(Location.StandardPlayer);
        }

        #region BSIPA Config
        //Uncomment to use BSIPA's config
        [Init]
        public void InitWithConfig(Config conf)
        {
            Configuration.PluginConfig.Instance = conf.Generated<Configuration.PluginConfig>();
        }
        #endregion


        #region Disableable

        /// <summary>
        /// Called when the plugin is enabled (including when the game starts if the plugin is enabled).
        /// </summary>
        [OnEnable]
        public void OnEnable()
        {
            if (!AudioPlayer.ShouldInitialize())
            {
                ExtractZip();
            }
        }

        private async void ExtractZip()
        {
            byte[] zip = BeatSaberMarkupLanguage.Utilities.GetResource(Assembly.GetExecutingAssembly(), "RedLightGreenLight.RedLightGreenLight.zip");
            Stream zipStream = new MemoryStream(zip);
            try
            {
                ZipArchive archive = new ZipArchive(zipStream, ZipArchiveMode.Read);
                string path = AudioPlayer.rootDir.Parent.FullName;
                if (!Directory.Exists(path))
                    Directory.CreateDirectory(path);
                await Task.Run(() =>
                {
                    foreach (var entry in archive.Entries)
                    {
                        if (!string.IsNullOrWhiteSpace(entry.Name))
                        {
                            FileInfo file = new FileInfo(Path.Combine(path, entry.FullName));
                            if (!Directory.Exists(file.DirectoryName))
                                Directory.CreateDirectory(file.DirectoryName);
                            entry.ExtractToFile(file.FullName, true);
                        }
                    }
                }).ConfigureAwait(false);
                archive.Dispose();
            }
            catch (Exception e)
            {
                Plugin.Log.Error($"Unable to extract ZIP! Exception: {e}");
                return;
            }
            zipStream.Close();
        }

        /// <summary>
        /// Called when the plugin is disabled and on Beat Saber quit. It is important to clean up any Harmony patches, GameObjects, and Monobehaviours here.
        /// The game should be left in a state as if the plugin was never started.
        /// Methods marked [OnDisable] must return void or Task.
        /// </summary>
        [OnDisable]
        public void OnDisable()
        {
        }

        #endregion
    }
}
