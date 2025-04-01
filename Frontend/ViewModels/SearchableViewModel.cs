using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;

namespace Frontend.ViewModels
{
    public abstract partial class SearchableViewModel<TModel> : PaginatedViewModel<TModel>
    {
        [ObservableProperty] private string _searchQuery = string.Empty;
        [ObservableProperty] private bool _enableShowAll = false;
        private List<TModel>? filteredResults;
        
        protected abstract List<TModel> ApplySearch(List<TModel> items, string query);

        protected virtual List<TModel> ApplySearchByBookingId(List<TModel> items, string query)
        {
            return items;
        }

        public void ResetPagination() {
            UpdatePaginatedItems(_allItems);
        }

        [RelayCommand]
        private void SearchItems(string parameter)
        {
            if (_allItems != null) {
                ErrorMessage = string.Empty;
                filteredResults = _allItems;

                if (parameter == "all") {
                    UpdatePaginatedItems(filteredResults, true);
                    EnableShowAll = false;
                    SearchQuery = string.Empty;
                    return;
                }

                if (!string.IsNullOrEmpty(SearchQuery)) {
                    EnableShowAll = true;

                    if (parameter == "text") {
                        if (MyRegex().IsMatch(SearchQuery)) {
                            filteredResults = ApplySearchByBookingId(_allItems, SearchQuery);
                        } else {
                            filteredResults = ApplySearch(_allItems, SearchQuery);
                        }
                    }
                            
                } else {
                    ErrorMessage = "Digita qualcosa nella barra di ricerca";
                    return;
                } 

                UpdatePaginatedItems(filteredResults, true);
            }      

            SearchQuery = string.Empty;
        }

                [RelayCommand]
        private void GoToPreviousPage()
        {
            if (IsPreviousPageEnabled)
            {
                CurrentPage--;
                UpdatePaginatedItems(filteredResults ?? _allItems);
            }
        }

        [RelayCommand]
        private void GoToNextPage()
        {
            if (IsNextPageEnabled)
            {
                CurrentPage++;
                UpdatePaginatedItems(filteredResults ?? _allItems);
            }
        }
        

        [GeneratedRegex("^[0-9]{1,10}?$")]
        private static partial Regex MyRegex();
    }
}