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

using E.ExploreDeezer.Core.ViewModels;

namespace E.ExploreDeezer.UWP.Controls
{
    public class InformationDataTemplateSelector : DataTemplateSelector
    {
        protected override DataTemplate SelectTemplateCore(object item, DependencyObject container)
        {
            var obj =  item as InformationEntry;
            FrameworkElement elemnt = container as FrameworkElement;

            switch (obj.Type)
            {
                case EInformationType.Textual:
                    return elemnt.FindName("InfoListTextCell") as DataTemplate;

                case EInformationType.Image:
                    return elemnt.FindName("InfoListImageCell") as DataTemplate;
            }

            return base.SelectTemplateCore(item, container);
        }
    }


    public sealed partial class TracklistInformation : UserControl
    {
        public TracklistInformation()
        {
            this.InitializeComponent();
        }


        public IInformationViewModel ViewModel => this.DataContext as IInformationViewModel;
    }
}
