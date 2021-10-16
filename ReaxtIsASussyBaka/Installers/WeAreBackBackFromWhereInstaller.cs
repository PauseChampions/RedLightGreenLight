using ReaxtIsASussyBaka.Configuration;
using ReaxtIsASussyBaka.GameScene;
using Zenject;

namespace ReaxtIsASussyBaka.Installers
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
