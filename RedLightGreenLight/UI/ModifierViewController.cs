using BeatSaberMarkupLanguage.Attributes;
using BeatSaberMarkupLanguage.GameplaySetup;
using RedLightGreenLight.Configuration;
using System;
using System.ComponentModel;
using Zenject;

namespace RedLightGreenLight.UI
{
    internal class ModifierViewController : IInitializable, IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void Initialize()
        {
            GameplaySetup.instance.AddTab("RedLight", "RedLightGreenLight.UI.ModifierView.bsml", this, MenuType.Solo | MenuType.Campaign | MenuType.Custom);
        }

        public void Dispose()
        {
            GameplaySetup.instance?.RemoveTab("RedLight");
        }

        [UIValue("mod-enabled")]
        private bool ModEnabled
        {
            get => PluginConfig.Instance.ModEnabled;
            set
            {
                PluginConfig.Instance.ModEnabled = value;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(ModEnabled)));
            }
        }
    }
}
