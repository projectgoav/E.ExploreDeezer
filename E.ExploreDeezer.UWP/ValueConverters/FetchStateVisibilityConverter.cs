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
            : this(new EFetchState[1] { expectedState} )
        { }

        public FetchStateVisibilityConverter(IEnumerable<EFetchState> expectedStates)
        {
            this.ExpectedStates = new HashSet<EFetchState>(expectedStates);
        }


        public IEnumerable<EFetchState> ExpectedStates { get; private set; }


        public object Convert(object value, Type targetType, object parameter, string language)
        {
            Assert.That(value is EFetchState);

            EFetchState fetchState = (EFetchState)value;

            return this.ExpectedStates.Contains(fetchState) ? Visibility.Visible
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

    /* To make things a little less complex on the UI size of things
     * we've treated 'Empty' and 'Error' fetch states to be the same.
     * 
     * TODO: 'Error' could have a retry option for transient issues
     *       that may occur. */
    public class EmptyOrErrorFetchStateVisibilityConverter : FetchStateVisibilityConverter
    {
        public EmptyOrErrorFetchStateVisibilityConverter()
            : base(new EFetchState[2] { EFetchState.Empty, EFetchState.Error })
        { }
    }
}
