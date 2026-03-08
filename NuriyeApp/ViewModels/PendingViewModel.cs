using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NuriyeApp.Models;
using NuriyeApp.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NuriyeApp.ViewModels
{
    public partial class PendingViewModel : ObservableObject
    {
        public ObservableCollection<Rental> Rentals { get; } = new();

        public string[] StaffList { get; } =
        {
            "미지정",
            "[암실부장] 김지원",
            "[회장] 유재동",
            "[부회장] 한지원",
            "[총무] 심종율",
            "[홍보부장] 이서윤",
            "[홍보차장] 김예은",
            "[홍보차장] 김기연"
        };

        [ObservableProperty]
        private Rental? _selectedRental;

        [ObservableProperty]
        private string _selectedStaff = "미지정";

        [ObservableProperty]
        private string _remarks = "";

        [ObservableProperty]
        private bool _isLoading = false;

        [ObservableProperty]
        private string _statusMessage = "";

        public bool HasSelection => SelectedRental != null;
        public bool HasNoSelection => SelectedRental == null;

        [RelayCommand]
        public async Task LoadAsync()
        {
            IsLoading = true;
            StatusMessage = "";
            try
            {
                var list = await SupabaseService.Instance.GetPendingRentalsAsync();
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

        partial void OnSelectedRentalChanged(Rental? value)
        {
            if (value != null)
            {
                SelectedStaff = value.Staff;
                Remarks = value.Remarks;
            }
            OnPropertyChanged(nameof(HasSelection));
            OnPropertyChanged(nameof(HasNoSelection));
        }

        [RelayCommand]
        private async Task ApproveAsync()
        {
            if (SelectedRental == null) return;
            IsLoading = true;
            try
            {
                await SupabaseService.Instance.UpdateRentalStatusAsync(
                    SelectedRental.Id, "확정", SelectedStaff, Remarks);
                StatusMessage = "승인 완료되었습니다.";
                await LoadAsync();
            }
            catch
            {
                StatusMessage = "처리 중 오류가 발생했습니다.";
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private async Task RejectAsync()
        {
            if (SelectedRental == null) return;
            IsLoading = true;
            try
            {
                await SupabaseService.Instance.UpdateRentalStatusAsync(
                    SelectedRental.Id, "취소", SelectedStaff, Remarks);
                StatusMessage = "반려 처리되었습니다.";
                await LoadAsync();
            }
            catch
            {
                StatusMessage = "처리 중 오류가 발생했습니다.";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
