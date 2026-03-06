using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NuriyeApp.Services;
using System;
using System.Threading.Tasks;

namespace NuriyeApp.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty]
        private string _password = "";

        [ObservableProperty]
        private string _errorMessage = "";

        [ObservableProperty]
        private bool _isLoading = false;

        public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

        partial void OnErrorMessageChanged(string value)
        {
            OnPropertyChanged(nameof(HasError));
        }

        public event System.Action? LoginSucceeded;

        [RelayCommand]
        private async Task LoginAsync()
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                ErrorMessage = "비밀번호를 입력하세요.";
                return;
            }

            IsLoading = true;
            ErrorMessage = "";

            try
            {
                var success = await AuthService.Instance.LoginAsync(Password);
                if (success)
                {
                    Password = "";
                    LoginSucceeded?.Invoke();
                }
                else
                {
                    ErrorMessage = "비밀번호가 올바르지 않습니다.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"오류: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}
