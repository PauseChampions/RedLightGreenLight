using ReaxtIsASussyBaka.GameObjects;
using Zenject;

namespace ReaxtIsASussyBaka.Installers
{
    internal class WeAreBackBackFromWhereInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<RedLight>().FromNewComponentOnRoot().AsSingle();
            Container.BindInterfacesAndSelfTo<Timer>().FromNewComponentOnRoot().AsSingle();
        }
    }
}
