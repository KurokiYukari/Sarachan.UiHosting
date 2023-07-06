using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Sarachan.UiHosting.Wpf;
using Sarachan.UiHosting.WpfSample.Views;

namespace Sarachan.UiHosting.WpfSample
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
        //, IInitializeComponent
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var shell = new ShellWindow();
            shell.Show();
        }
    }
}
