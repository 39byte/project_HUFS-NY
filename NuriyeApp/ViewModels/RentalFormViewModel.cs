using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NuriyeApp.Models;
using NuriyeApp.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace NuriyeApp.ViewModels
{
    public partial class RentalFormViewModel : ObservableObject
    {
        private List<InventoryItem> _allInventory = new();

        public ObservableCollection<string> Categories { get; } = new();
        public ObservableCollection<InventoryItem> Bodies { get; } = new();
        public ObservableCollection<InventoryItem> Lenses { get; } = new();

        // 시간 선택용 (0~23시)
        public string[] HourItems { get; } = Enumerable.Range(0, 24).Select(h => $"{h}시").ToArray();

        [ObservableProperty] private string? _selectedCategory;
        [ObservableProperty] private InventoryItem? _selectedBody;
        [ObservableProperty] private InventoryItem? _selectedLens;

        // 액세서리
        [ObservableProperty] private bool _hasCharger;
        [ObservableProperty] private bool _hasSdReader;
        [ObservableProperty] private bool _hasBag;
        [ObservableProperty] private bool _hasTripod;

        // 신청자 정보
        [ObservableProperty] private string _applicantName = "";
        [ObservableProperty] private string _contact = "";

        // 대여 기간
        [ObservableProperty] private DateTimeOffset _startDate = DateTimeOffset.Now;
        [ObservableProperty] private DateTimeOffset _endDate = DateTimeOffset.Now.AddDays(1);

        // 대면 가능 시간 (SelectedIndex = 시간)
        [ObservableProperty] private int _rentalTimeStart = 9;
        [ObservableProperty] private int _rentalTimeEnd = 18;
        [ObservableProperty] private int _returnTimeStart = 9;
        [ObservableProperty] private int _returnTimeEnd = 18;

        [ObservableProperty] private string _extraRequest = "";

        [ObservableProperty] private string _statusMessage = "";
        [ObservableProperty] private bool _isLoading = false;
        [ObservableProperty] private bool _isSuccess = false;

        public bool HasError => !IsSuccess && !string.IsNullOrEmpty(StatusMessage);

        partial void OnStatusMessageChanged(string value) => OnPropertyChanged(nameof(HasError));
        partial void OnIsSuccessChanged(bool value) => OnPropertyChanged(nameof(HasError));

        [RelayCommand]
        public async Task LoadInventoryAsync()
        {
            IsLoading = true;
            try
            {
                _allInventory = await SupabaseService.Instance.GetInventoryAsync();

                Categories.Clear();
                foreach (var cat in _allInventory
                    .Where(i => i.Category == "Body" && !string.IsNullOrEmpty(i.BodyCategory))
                    .Select(i => i.BodyCategory)
                    .Distinct())
                    Categories.Add(cat);
            }
            catch (Exception ex)
            {
                StatusMessage = $"장비 목록 오류: {ex.Message}";
            }
            finally { IsLoading = false; }
        }

        partial void OnSelectedCategoryChanged(string? value)
        {
            Bodies.Clear();
            SelectedBody = null;
            if (value == null) return;
            foreach (var b in _allInventory.Where(i => i.Category == "Body" && i.BodyCategory == value))
                Bodies.Add(b);
        }

        partial void OnSelectedBodyChanged(InventoryItem? value)
        {
            Lenses.Clear();
            SelectedLens = null;
            var lenses = _allInventory.Where(i => i.Category == "Lens").AsEnumerable();
            if (value != null)
            {
                if (value.Brand?.Contains("Canon") == true)
                    lenses = lenses.Where(l => l.Brand?.Contains("Canon") == true || l.Brand?.Contains("Tamron") == true);
                if (value.Format == "FF")
                    lenses = lenses.Where(l => l.Format == "FF");
            }
            foreach (var l in lenses) Lenses.Add(l);
        }

        [RelayCommand]
        private async Task SubmitAsync()
        {
            StatusMessage = "";
            IsSuccess = false;

            if (string.IsNullOrWhiteSpace(ApplicantName) || string.IsNullOrWhiteSpace(Contact))
            {
                StatusMessage = "이름과 연락처를 입력하세요.";
                return;
            }
            if (SelectedBody == null && SelectedLens == null)
            {
                StatusMessage = "바디 또는 렌즈를 선택하세요.";
                return;
            }
            if (EndDate < StartDate)
            {
                StatusMessage = "반납 예정일이 대여 시작일보다 앞설 수 없습니다.";
                return;
            }
            if ((EndDate - StartDate).TotalDays > 7)
            {
                StatusMessage = "대여 기간은 최대 7일입니다.";
                return;
            }

            IsLoading = true;
            try
            {
                var startStr = StartDate.Date.ToString("yyyy-MM-dd");
                var endStr = EndDate.Date.ToString("yyyy-MM-dd");
                var equipment = BuildEquipment();

                var conflict = await SupabaseService.Instance.CheckConflictAsync(equipment, startStr, endStr);
                if (conflict)
                {
                    StatusMessage = "해당 기간에 이미 예약된 장비입니다.";
                    return;
                }

                var rental = new Rental
                {
                    Applicant = ApplicantName,
                    Contact = Contact,
                    Equipment = equipment,
                    StartDate = startStr,
                    EndDate = endStr,
                    MeetingTime = $"대여: {RentalTimeStart}~{RentalTimeEnd}시 / 반납: {ReturnTimeStart}~{ReturnTimeEnd}시",
                    Staff = "미지정",
                    Status = "대기",
                    Remarks = "",
                    Accessories = BuildAccessories(),
                    ExtraRequest = string.IsNullOrWhiteSpace(ExtraRequest) ? "없음" : ExtraRequest,
                    SubmittedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                };

                await SupabaseService.Instance.SubmitRentalAsync(rental);
                IsSuccess = true;
                StatusMessage = "신청이 완료되었습니다! 담당자 배정 후 확정 연락드립니다.";
                ResetForm();
            }
            catch (Exception ex)
            {
                StatusMessage = $"신청 오류: {ex.Message}";
            }
            finally { IsLoading = false; }
        }

        private string BuildEquipment()
        {
            var parts = new List<string>();
            if (SelectedBody != null) parts.Add($"{SelectedBody.Brand} {SelectedBody.ModelName}");
            if (SelectedLens != null) parts.Add($"{SelectedLens.Brand} {SelectedLens.ModelName}");
            return string.Join(" + ", parts);
        }

        private string BuildAccessories()
        {
            var acc = new List<string>();
            if (HasCharger) acc.Add("충전기");
            if (HasSdReader) acc.Add("SD 리더기");
            if (HasBag) acc.Add("가방");
            if (HasTripod) acc.Add("삼각대");
            return acc.Count > 0 ? string.Join(", ", acc) : "없음";
        }

        private void ResetForm()
        {
            ApplicantName = ""; Contact = ""; ExtraRequest = "";
            HasCharger = false; HasSdReader = false; HasBag = false; HasTripod = false;
            SelectedBody = null; SelectedLens = null; SelectedCategory = null;
            StartDate = DateTimeOffset.Now; EndDate = DateTimeOffset.Now.AddDays(1);
            RentalTimeStart = 9; RentalTimeEnd = 18;
            ReturnTimeStart = 9; ReturnTimeEnd = 18;
        }
    }
}
