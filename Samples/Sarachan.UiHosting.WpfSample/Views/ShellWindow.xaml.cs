using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Sarachan.Mvvm.Collections;
using Sarachan.Mvvm.Collections.View;
using Sarachan.UiHosting.Wpf;

namespace Sarachan.UiHosting.WpfSample.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class ShellWindow : Window
    {
        public ViewModel ViewModel => (ViewModel)DataContext;

        public ShellWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            throw new Exception("ButtonClickException");
        }

        int count;
        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.Dict.Add($"{DateTime.Now}_{count}", count++);
            TestStartThread();
        }

        private int _threadCount;
        private async void TestStartThread()
        {
            //var thread = ActivatorUtilities.CreateInstance<WpfThread>(ViewModel.Provider);
            //var dispatcher = await thread.RunDispatcher($"WpfThread {_threadCount++}");
            //var logger = ViewModel.Logger;
            //dispatcher.Invoke(() =>
            //{
            //    logger.LogInformation("Thread Started");
            //});
        }

        private void RemoveButton_Click(object sender, RoutedEventArgs e)
        {
            var first = ViewModel.Dict.Keys.FirstOrDefault();
            if (first != null)
            {
                ViewModel.Dict.Remove(first);
            }
        }
    }

    public class ViewModel : ObservableObject
    {
        public ILogger Logger { get; }
        public IServiceProvider Provider { get; }

        public ObservableDictionary<string, int> Dict { get; } = new();
        
        public IStandardListView<KeyValuePair<string, int>> DictView { get; }

        public ViewModel(IServiceProvider provider,
            ILogger<ViewModel> logger)
        {
            Provider = provider;
            Logger = logger;

            DictView = Dict.BuildView(emitter =>
            {
                return emitter;
            }).CreateStandardView(false);
        }
    }
}
