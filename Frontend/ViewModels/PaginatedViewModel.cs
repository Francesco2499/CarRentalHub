using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Frontend.ViewModels
{
    public abstract partial class PaginatedViewModel<TModel> : ViewModelBase
    {
        protected const int PageSize = 5;

        [ObservableProperty]
        private bool _isVisibleList = false;
        
        [ObservableProperty]
        protected ObservableCollection<TModel> _items = [];
        
        [ObservableProperty]
        protected int _currentPage = 1;
        
        [ObservableProperty]
        protected bool _isPreviousPageEnabled = false;
        
        [ObservableProperty]
        protected bool _isNextPageEnabled = false;
        
        [ObservableProperty]
        protected double _previousPageOpacity = 1.0;
        
        [ObservableProperty]
        protected double _nextPageOpacity = 1.0;

        [ObservableProperty]
        private string? _errorMessage;
        
        [ObservableProperty]
        protected bool _isPaginationVisible = false;
        
        protected List<TModel>? _allItems = [];
        
        protected abstract List<TModel>? LoadAllItemsAsync(); // Metodo che deve essere implementato nelle viewmodel concrete
        
        protected void UpdatePaginatedItems(List<TModel>? listItems)
        {
            var skip = (CurrentPage - 1) * PageSize;
            var results = listItems ?? _allItems;
            ErrorMessage = string.Empty;
            
            if (results != null) {
                Items.Clear();
                foreach (var item in results.Skip(skip).Take(PageSize))
                {
                    Items.Add(item);
                }

                IsPreviousPageEnabled = CurrentPage > 1;
                IsNextPageEnabled = CurrentPage * PageSize < results.Count;
                IsPaginationVisible = _allItems != null && _allItems.Count > PageSize;
                PreviousPageOpacity = IsPreviousPageEnabled ? 1.0 : 0.5;
                NextPageOpacity = IsNextPageEnabled ? 1.0 : 0.5;   
                if (results.Count == 0) {
                    ErrorMessage = "No results found. Please try a different search term.";
                }
            }
        }

        [RelayCommand]
        private void GoToPreviousPage()
        {
            if (IsPreviousPageEnabled)
            {
                CurrentPage--;
                UpdatePaginatedItems(null);
            }
        }

        [RelayCommand]
        private void GoToNextPage()
        {
            if (IsNextPageEnabled)
            {
                CurrentPage++;
                UpdatePaginatedItems(null);
            }
        }
        
        public void LoadItems()
        {
            _allItems = LoadAllItemsAsync();
            UpdatePaginatedItems(null);
        }
    }

}