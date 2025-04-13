using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Threading.Tasks;
using System;

namespace Frontend.ViewModels
{
    public abstract partial class PaginatedViewModel<TModel> : ViewModelBase
    {
        protected const int PageSize = 5;
        [ObservableProperty] private bool _isVisibleList = false;
        [ObservableProperty] protected ObservableCollection<TModel> _items = [];
        [ObservableProperty] protected int _currentPage = 1;
        [ObservableProperty] protected bool _isPreviousPageEnabled = false; 
        [ObservableProperty] protected bool _isNextPageEnabled = false;
        [ObservableProperty] protected double _previousPageOpacity = 1.0;
        [ObservableProperty] protected double _nextPageOpacity = 1.0;
        [ObservableProperty]  private string? _errorMessage;
        [ObservableProperty] protected bool _isPaginationVisible = false;
        protected List<TModel>? _allItems = [];
        
        protected abstract Task<List<TModel>?> LoadAllItems();
        
        protected void UpdatePaginatedItems(List<TModel>? listItems, bool returnToFirstPage = false)
        {
            if (returnToFirstPage)
                CurrentPage = 1;

            var results = listItems ?? _allItems;
            Items.Clear();
            ErrorMessage = string.Empty;

            if (results == null || results.Count == 0)
            {
                ErrorMessage = "Nessun risultato trovato. Cambia i parametri di ricerca.";
                UpdatePaginationControls(0);
                return;
            }

            var maxPage = (int)Math.Ceiling(results.Count / (double)PageSize);
            if (CurrentPage > maxPage)
                CurrentPage = maxPage;

            var skip = (CurrentPage - 1) * PageSize;
            foreach (var item in results.Skip(skip).Take(PageSize))
                Items.Add(item);

            UpdatePaginationControls(results.Count);
        }

        private void UpdatePaginationControls(int totalCount)
        {
            IsPreviousPageEnabled = CurrentPage > 1;
            IsNextPageEnabled = CurrentPage * PageSize < totalCount;
            IsPaginationVisible = _allItems != null && _allItems.Count > PageSize;
            PreviousPageOpacity = IsPreviousPageEnabled ? 1.0 : 0.5;
            NextPageOpacity = IsNextPageEnabled ? 1.0 : 0.5;
        }

        public void ResetPagination() {
            UpdatePaginatedItems(_allItems);
        }
        
        public async Task LoadItems()
        {
            _allItems = await LoadAllItems();
            UpdatePaginatedItems(null);
        }
    }

}