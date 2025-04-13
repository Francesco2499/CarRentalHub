using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Frontend.Models;

namespace Frontend.ViewModels
{
    public partial class SearchVehicleViewModel : SearchableViewModel<VehicleModel>
    {
        [ObservableProperty] private BookingModel? _booking;
        [ObservableProperty] private bool _isBookingSummaryVisible = false;
        [ObservableProperty] private bool _isVisibleSubTitle = true;
        [ObservableProperty] private string _firstErrorMessage = string.Empty;
        [ObservableProperty] private string _bookingMessage = string.Empty;
        [ObservableProperty] private bool _isVisibleSearchDate = true;
        [ObservableProperty] private string _location = string.Empty;
        [ObservableProperty] private VehicleModel? _selectedVehicle;
        private ShowroomModel? _showroom = null;

        public void SetShowroom(ShowroomModel? showroomSelected)
        {
            _showroom = showroomSelected;
            _ = LoadItems();
        }

        protected override async Task<List<VehicleModel>?> LoadAllItems()
        {
            if (_showroom != null)
            {
                return await Task.FromResult(_showroom.Vehicles);
            }
            return []; 
        }

        protected override List<VehicleModel> ApplySearch(List<VehicleModel> items, string query)
        {
            return [.. items.Where(v =>
                v.Model.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                v.Category.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase)
            )];
        }
    }
}
