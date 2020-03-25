using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

using E.ExploreDeezer.Core;
using E.ExploreDeezer.Core.ViewModels;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace E.ExploreDeezer.UWP.Views
{
    public class InformationDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var obj =  item as InformationEntry;
            FrameworkElement elemnt = container as FrameworkElement;

            switch(obj.Type)
            {
                case EInformationType.Textual:
                    return elemnt.FindName("InfoListTextCell") as DataTemplate;

                case EInformationType.Image:
                    return elemnt.FindName("InfoListImageCell") as DataTemplate;
            }

            return base.SelectTemplateCore(item, container);
        }
    }



    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class TracklistView : Page
    {
        public TracklistView()
        {
            this.InitializeComponent();
        }


        public ITracklistViewModel ViewModel => this.DataContext as ITracklistViewModel;


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            this.DataContext = ServiceRegistry.ViewModelFactory.CreateTracklistViewModel(e.Parameter as ITracklistViewModelParams);

            this.ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);

            this.ViewModel.PropertyChanged -= ViewModel_PropertyChanged;

            (this.ViewModel as IDisposable)?.Dispose();
        }


        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(ITracklistViewModel.InformationViewModel):
                    if (this.ViewModel.InformationViewModel != null)
                    {
                        this.InfoList.ItemsSource = this.ViewModel.InformationViewModel.Values;
                    }
                    break;

                case nameof(ITracklistViewModel.InformationViewModel.Values):
                    this.InfoList.ItemsSource = this.ViewModel.InformationViewModel.Values;
                    break;
            }
        }
    }
}
