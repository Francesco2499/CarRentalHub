using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Frontend.Models;
using Frontend.Services;


namespace Frontend.ViewModels
{
    public partial class SearchShowroomViewModel : SearchableViewModel<ShowroomModel>
    {        
        [ObservableProperty] private BookingModel? _booking;
        [ObservableProperty] private bool _isBookingSummaryVisible = false;
        [ObservableProperty] private bool _isVisibleSubTitle = true;
        [ObservableProperty] private string _firstErrorMessage = string.Empty;
        [ObservableProperty] private string _bookingMessage = string.Empty;
        [ObservableProperty] private bool _isVisibleSearchDate = true;
        [ObservableProperty] private string _location = string.Empty;
        [ObservableProperty] private VehicleModel? _selectedVehicle;
        private DateTime? _startDate;
        [ObservableProperty] private DateTime? _endDate;

        public DateTime? StartDate
        {
            get => _startDate;
            set
            {
                SetProperty(ref _startDate, value);
                
                // Imposta EndDate al giorno successivo, solo se StartDate è selezionata
                if (value.HasValue)
                {
                    EndDate = value.Value.AddDays(1);
                }
            }
        }

        protected override List<ShowroomModel> LoadAllItems()
        {
            return VehicleService.GetAvailableShowrooms(StartDate, EndDate);
        }

        protected override List<ShowroomModel> ApplySearch(List<ShowroomModel> items, string query)
        {
            return [.. items.Where(v =>
                    v.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    v.Location.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) 
                )];
        }
    }
}
