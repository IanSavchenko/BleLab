using System;
using Windows.ApplicationModel;
using Windows.System;
using Caliburn.Micro;

namespace BleLab.ViewModels
{
    public class AboutViewModel : PropertyChangedBase
    {
        public string Version => GetAppVersion();

        public void ForkMe()
        {
            Launcher.LaunchUriAsync(new Uri(@"https://github.com/IanSavchenko/BleLab"));
        }

        private static string GetAppVersion()
        {

            Package package = Package.Current;
            PackageId packageId = package.Id;
            PackageVersion version = packageId.Version;

            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }

    }
}
