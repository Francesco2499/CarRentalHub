using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Frontend.ViewModels
{
    public abstract partial class SearchableViewModel<TModel> : PaginatedViewModel<TModel>
    {
        [ObservableProperty] private string _searchQuery = string.Empty;  // Per la ricerca testuale (es. modello veicolo)

         [ObservableProperty] private bool _enableShowAll = false;
        protected abstract List<TModel> ApplySearch(List<TModel> items, string query);
        protected virtual List<TModel> ApplySearchByUserId(List<TModel> items, int query)
        {
            return items; // Default: non applica alcun filtro
        }

        protected virtual List<TModel> ApplySearchByBookingId(List<TModel> items, int query)
        {
            return items; // Default: non applica alcun filtro
        }

        [RelayCommand]
        private void SearchItems(object parameter)
        {
            if (_allItems != null) {
                ErrorMessage = string.Empty;
                List<TModel> filteredResults = _allItems;

                if (parameter.ToString() == "all") {
                    UpdatePaginatedItems(filteredResults);
                    EnableShowAll = false;
                    SearchQuery = string.Empty;
                    return;
                }

                if (!string.IsNullOrEmpty(SearchQuery)) {
                    EnableShowAll = true;

                    switch (parameter)
                    {
                        case "text":
                            filteredResults = ApplySearch(_allItems, SearchQuery);
                            break;
                        case "user":
                            filteredResults = ApplySearchByUserId(_allItems, int.Parse(SearchQuery));
                            break;
                        case "booking":
                            filteredResults = ApplySearchByBookingId(_allItems, int.Parse(SearchQuery));
                            break;
                    }

                    UpdatePaginatedItems(filteredResults);
                }
                else {
                    ErrorMessage = "Please enter a search term before proceeding.";
                }

                
                SearchQuery = string.Empty;
            }

        }
    }


}