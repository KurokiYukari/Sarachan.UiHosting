using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sarachan.UiHosting.Navigation
{
    public interface INavigationService
    {
        void Navigate(IUiContext context, object viewModel, NavigationArgs.Navigate args);
    }
}
