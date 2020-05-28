using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel;

using E.ExploreDeezer.Core;
using E.ExploreDeezer.Core.Mvvm;
using E.ExploreDeezer.Core.Collections;


namespace E.ExploreDeezer.UWP.ViewModels
{
    public interface IContentUserControlViewModel<TCollection> : IViewModel
    {
        EFetchState FetchState { get; }
        IObservableCollection<TCollection> Collection { get; }

        void OnItemSelected(int index);
    }


    internal class ContentUserControlViewModel<TCollection> : ViewModelBase,
                                                              IContentUserControlViewModel<TCollection>
    {
        private readonly IViewModel parentViewModel;
        private readonly string collectionPropertyName;
        private readonly string fetchStatePropertyName;
        private readonly Action<int> onItemSelectedCallback;
        private readonly Func<EFetchState> fetchStateSelector;
        private readonly Func<IObservableCollection<TCollection>> collectionSelector;

        public ContentUserControlViewModel(IViewModel parent,
                                           string collectionPropertyName,
                                           string fetchStatePropertyName,
                                           Func<IObservableCollection<TCollection>> collectionSelector,
                                           Func<EFetchState> fetchStateSelector,
                                           Action<int> onItemSelected,
                                           IPlatformServices platformServices)
            : base(platformServices)
        {
            this.parentViewModel = parent;

            this.onItemSelectedCallback = onItemSelected;
            this.collectionPropertyName = collectionPropertyName;
            this.fetchStatePropertyName = fetchStatePropertyName;
            this.fetchStateSelector = fetchStateSelector;
            this.collectionSelector = collectionSelector;

            this.parentViewModel.PropertyChanged += OnParentPropertyChanged;
        }


        // IContentUserControlViewModel
        public EFetchState FetchState => this.fetchStateSelector();
        public IObservableCollection<TCollection> Collection => this.collectionSelector();

        public void OnItemSelected(int index)
            => this.onItemSelectedCallback(index);


        private void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == this.fetchStatePropertyName)
                base.RaisePropertyChangedSafe(nameof(FetchState));

            else if (e.PropertyName == this.collectionPropertyName)
                base.RaisePropertyChangedSafe(nameof(Collection));
        }



        // IDisposable
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                this.parentViewModel.PropertyChanged -= OnParentPropertyChanged;
            }

            base.Dispose(disposing);
        }
    }
}
