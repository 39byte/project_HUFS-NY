using Microsoft.Windows.AppNotifications;
using Microsoft.Windows.AppNotifications.Builder;

namespace NuriyeApp.Services
{
    public class NotificationService
    {
        private static NotificationService? _instance;
        public static NotificationService Instance => _instance ??= new NotificationService();

        private bool _isRegistered = false;

        public void Register()
        {
            try
            {
                AppNotificationManager.Default.Register();
                _isRegistered = true;
            }
            catch
            {
                // 패키지 ID 없이 실행 중 (Unpackaged) 이면 토스트 알림 비활성화
                _isRegistered = false;
            }
        }

        public void SendNewRentalNotification(string applicant, string equipment, string startDate)
        {
            if (!_isRegistered) return;

            try
            {
                var builder = new AppNotificationBuilder()
                    .AddText("새 대여 신청이 접수되었습니다")
                    .AddText($"신청자: {applicant}")
                    .AddText($"장비: {equipment}  |  대여 시작일: {startDate}");

                AppNotificationManager.Default.Show(builder.BuildNotification());
            }
            catch { }
        }
    }
}
