using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NuriyeApp.Models;
using NuriyeApp.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NuriyeApp.ViewModels
{
    public partial class HistoryViewModel : ObservableObject
    {
        public ObservableCollection<Rental> Rentals { get; } = new();

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string _statusMessage = "";

        [RelayCommand]
        public async Task LoadAsync()
        {
            IsLoading = true;
            StatusMessage = "";
            try
            {
                var list = await SupabaseService.Instance.GetRentalsAsync();
                Rentals.Clear();
                foreach (var r in list) Rentals.Add(r);
            }
            catch
            {
                StatusMessage = "데이터를 불러오는 중 오류가 발생했습니다.";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
