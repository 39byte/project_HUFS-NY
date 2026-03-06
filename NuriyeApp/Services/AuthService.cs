using System.Threading.Tasks;

namespace NuriyeApp.Services
{
    public class AuthService
    {
        private static AuthService? _instance;
        public static AuthService Instance => _instance ??= new AuthService();

        public bool IsLoggedIn { get; private set; } = false;

        public async Task<bool> LoginAsync(string password)
        {
            string stored;
            try
            {
                stored = await SupabaseService.Instance.GetSettingAsync("admin_password");
            }
            catch
            {
                stored = "";
            }
            if (string.IsNullOrEmpty(stored)) stored = "1111";
            IsLoggedIn = password == stored;
            return IsLoggedIn;
        }

        public void Logout() => IsLoggedIn = false;

        public async Task<bool> ChangePasswordAsync(string currentPassword, string newPassword)
        {
            var stored = await SupabaseService.Instance.GetSettingAsync("admin_password");
            if (string.IsNullOrEmpty(stored)) stored = "1111";
            if (currentPassword != stored) return false;
            await SupabaseService.Instance.UpdateSettingAsync("admin_password", newPassword);
            return true;
        }
    }
}
