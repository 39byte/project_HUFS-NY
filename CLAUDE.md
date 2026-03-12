# 작업 지시: 대여 신청 페이지(RentalFormPage) 버그 수정 3건

> 각 FIX 완료 후 즉시 커밋하라.
> 중단 시 `git log --oneline -3`으로 진행 상황을 확인하고 이어서 작업하라.
> 수정 전 반드시 `NuriyeApp/Views/RentalFormPage.xaml`의 현재 코드를 읽고 구조를 파악할 것.

---

## FIX 1: 렌즈 선택 시 '장비 선택' 열 폭 축소 버그

**파일**: `RentalFormPage.xaml`
**증상**: 렌즈 ComboBox에서 항목을 선택하면 좌측 '장비 선택' 컬럼의 폭이 좁아짐
**목표**: 렌즈 선택 여부와 관계없이 좌우 컬럼 폭이 고정되도록 수정. ComboBox의 크기가 선택된 내용에 따라 변동하지 않아야 한다.

```
커밋: fix(ui): 렌즈 선택 시 장비 선택 열 폭 축소 방지
```

---

## FIX 2: 대여 기간 DatePicker 레이아웃 변경

**파일**: `RentalFormPage.xaml`
**증상**: '대여 시작일'과 '반납 예정일' DatePicker가 가로로 나란히 배치되어 있어 일(day) 선택칸이 잘려서 안 보임
**목표**:
- '반납 예정일' DatePicker를 '대여 시작일' 아래로 이동하여 세로 배치로 변경
- 각 DatePicker가 충분한 가로 폭을 확보하여 년/월/일 선택칸이 모두 정상적으로 표시되도록 함

```
커밋: fix(ui): 대여 기간 DatePicker 세로 배치로 변경하여 일(day) 선택칸 표시
```

---

## FIX 3: DatePicker 내 년/월/일 선택칸 폭 균등화

**파일**: `RentalFormPage.xaml`
**증상**: DatePicker에서 월(month) 선택칸의 폭이 년(year), 일(day)보다 넓어서 불균형
**목표**: DatePicker 내부의 년/월/일 각 선택칸 폭을 동일하게 맞춤. WinUI 3의 DatePicker는 기본적으로 월 칸이 넓게 설정되어 있으므로, `DayVisible`, `MonthVisible`, `YearVisible` 속성은 유지하되 DatePicker 자체에 `HorizontalAlignment="Stretch"`를 적용하고, 필요하면 DatePicker의 ControlTemplate 또는 리소스 오버라이드로 각 칸의 비율을 균등하게 조정하라.

```
커밋: fix(ui): DatePicker 년/월/일 선택칸 폭 균등화
```

---

## 작업 규칙

1. ViewModel(`RentalFormViewModel.cs`)의 로직과 바인딩은 변경하지 않는다
2. 각 FIX 완료 후 빌드 에러가 없는지 확인한다

## 완료 확인

```bash
git log --oneline -3
```
