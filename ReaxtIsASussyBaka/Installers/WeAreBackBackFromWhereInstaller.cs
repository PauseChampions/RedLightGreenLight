using ReaxtIsASussyBaka.GameObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Zenject;

namespace ReaxtIsASussyBaka.Installers
{
    internal class WeAreBackBackFromWhereInstaller : Installer
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<RedLight>().FromNewComponentOnRoot().AsSingle();
        }
    }
}
