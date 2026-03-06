using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NuriyeApp.Services;
using System.Threading.Tasks;

namespace NuriyeApp.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _currentPassword = "";

        [ObservableProperty]
        private string _newPassword = "";

        [ObservableProperty]
        private string _confirmPassword = "";

        [ObservableProperty]
        private string _statusMessage = "";

        [ObservableProperty]
        private bool _isLoading = false;

        [RelayCommand]
        private async Task ChangePasswordAsync()
        {
            if (string.IsNullOrWhiteSpace(CurrentPassword) ||
                string.IsNullOrWhiteSpace(NewPassword) ||
                string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                StatusMessage = "모든 항목을 입력하세요.";
                return;
            }

            if (NewPassword != ConfirmPassword)
            {
                StatusMessage = "새 비밀번호가 일치하지 않습니다.";
                return;
            }

            IsLoading = true;
            StatusMessage = "";
            try
            {
                var success = await AuthService.Instance.ChangePasswordAsync(CurrentPassword, NewPassword);
                if (success)
                {
                    StatusMessage = "비밀번호가 변경되었습니다.";
                    CurrentPassword = "";
                    NewPassword = "";
                    ConfirmPassword = "";
                }
                else
                {
                    StatusMessage = "현재 비밀번호가 올바르지 않습니다.";
                }
            }
            catch
            {
                StatusMessage = "변경 중 오류가 발생했습니다.";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
