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
            GameplaySetup.instance.AddTab("Hi", "RedLightGreenLight.UI.ModifierView.bsml", this, MenuType.Solo);
        }

        public void Dispose()
        {
            GameplaySetup.instance?.RemoveTab("Hi");
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
