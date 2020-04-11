using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

using E.ExploreDeezer.Core;

namespace E.ExploreDeezer.UWP.ValueConverters
{
    public class FetchStateVisibilityConverter : IValueConverter
    {

        public FetchStateVisibilityConverter(EFetchState expectedState)
        {
            this.ExpectedState = expectedState;
        }


        public EFetchState ExpectedState { get; private set; }


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Assert.That(value is EFetchState);

            EFetchState fetchState = (EFetchState)value;

            return fetchState == this.ExpectedState ? Visibility.Visible
                                                    : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException();
        }
    }


    public class EmptyFetchStateVisibilityConverter : FetchStateVisibilityConverter
    {
        public EmptyFetchStateVisibilityConverter()
            : base(EFetchState.Empty)
        { }
    }

    public class ErrorFetchStateVisibilityConverter : FetchStateVisibilityConverter
    {
        public ErrorFetchStateVisibilityConverter()
            : base(EFetchState.Error)
        { }
    }

    public class LoadingFetchStateVisibilityConverter : FetchStateVisibilityConverter
    {
        public LoadingFetchStateVisibilityConverter()
            : base(EFetchState.Loading)
        { }
    }

    public class ContentAvailableFetchStateVisibilityConverter : FetchStateVisibilityConverter
    {
        public ContentAvailableFetchStateVisibilityConverter()
            : base(EFetchState.Available)
        { }
    }
}
