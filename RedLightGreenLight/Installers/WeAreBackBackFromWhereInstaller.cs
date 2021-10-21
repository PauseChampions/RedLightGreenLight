using RedLightGreenLight.Configuration;
using RedLightGreenLight.GameScene;
using Zenject;

namespace RedLightGreenLight.Installers
{
    internal class WeAreBackBackFromWhereInstaller : Installer
    {
        public override void InstallBindings()
        {
            if (PluginConfig.Instance.ModEnabled)
            {
                Container.BindInterfacesAndSelfTo<RedLight>().FromNewComponentOnRoot().AsSingle();
                Container.BindInterfacesAndSelfTo<Judge>().FromNewComponentOnRoot().AsSingle();
                Container.Bind<AudioPlayer>().AsSingle();
            }
        }
    }
}
