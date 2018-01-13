namespace Unosquare.FFME.Windows.Sample
{
    using OsInfo;
    using OsInfo.Extensions;
    using System;
    using System.Windows;
    using System.Windows.Media;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            if(Environment.OSVersion.IsLessThan(OsVersion.Vista))
            {
                RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
            }

            base.OnStartup(e);
        }

    }
}
