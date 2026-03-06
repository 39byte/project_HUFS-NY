using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NuriyeApp.Models;
using NuriyeApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NuriyeApp.ViewModels
{
    public partial class OngoingViewModel : ObservableObject
    {
        public ObservableCollection<Rental> Rentals { get; } = new();

        [ObservableProperty]
        private Rental? _selectedRental;

        [ObservableProperty]
        private string _remarks = "";

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
                var list = await SupabaseService.Instance.GetOngoingRentalsAsync();
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
                Remarks = value.Remarks;
        }

        [RelayCommand]
        private async Task MarkReturnedAsync()
        {
            if (SelectedRental == null) return;
            IsLoading = true;
            try
            {
                var now = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
                await SupabaseService.Instance.UpdateRentalStatusAsync(
                    SelectedRental.Id, "반납완료", SelectedRental.Staff, Remarks, now);
                StatusMessage = "반납 완료 처리되었습니다.";
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
        private async Task RevertToPendingAsync()
        {
            if (SelectedRental == null) return;
            IsLoading = true;
            try
            {
                await SupabaseService.Instance.UpdateRentalStatusAsync(
                    SelectedRental.Id, "대기", SelectedRental.Staff, Remarks);
                StatusMessage = "대기 상태로 되돌렸습니다.";
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
        private async Task SaveRemarksAsync()
        {
            if (SelectedRental == null) return;
            IsLoading = true;
            try
            {
                await SupabaseService.Instance.UpdateRentalStatusAsync(
                    SelectedRental.Id, SelectedRental.Status, SelectedRental.Staff, Remarks);
                StatusMessage = "비고가 저장되었습니다.";
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
