using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NuriyeApp.Models;
using NuriyeApp.Services;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace NuriyeApp.ViewModels
{
    public partial class InventoryViewModel : ObservableObject
    {
        public ObservableCollection<InventoryItem> Items { get; } = new();

        [ObservableProperty]
        private InventoryItem? _selectedItem;

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
                var list = await SupabaseService.Instance.GetInventoryAsync();
                Items.Clear();
                foreach (var item in list) Items.Add(item);
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

        [RelayCommand]
        private void AddItem()
        {
            Items.Add(new InventoryItem { Status = "대여가능" });
        }

        [RelayCommand]
        private void RemoveItem()
        {
            if (SelectedItem != null)
                Items.Remove(SelectedItem);
        }

        [RelayCommand]
        private async Task SaveAsync()
        {
            IsLoading = true;
            try
            {
                await SupabaseService.Instance.UpsertInventoryAsync(new System.Collections.Generic.List<InventoryItem>(Items));
                StatusMessage = "저장되었습니다.";
            }
            catch
            {
                StatusMessage = "저장 중 오류가 발생했습니다.";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
