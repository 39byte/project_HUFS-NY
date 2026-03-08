# 작업 지시: UI 버그 수정 및 개선 (7건)

> 독립적인 7개 작업이다. 각 작업 완료 후 즉시 커밋하라.
> 중단 시 `git log --oneline -7`로 진행 상황을 확인할 수 있다.

---

## FIX 1: 승인 대기 카드 선택 시 하이라이트 영역 불일치

**증상**: 카드를 선택하면 밝아지는 영역(ListView 기본 SelectionHighlight)이 카드 Border 위치/크기와 맞지 않음
**원인**: ListView ItemTemplate 안에 Border를 넣었지만, ListViewItem 자체의 선택 효과가 별도로 존재

### 수정 파일: `NuriyeApp/Views/PendingPage.xaml`

### 해결 방법
ListView에 `ItemContainerStyle`을 추가하여 기본 선택 효과를 투명으로 만들고, 대신 DataTemplate 내 Border에 선택 시각 효과를 위임한다.

```xml
<ListView.ItemContainerStyle>
    <Style TargetType="ListViewItem">
        <Setter Property="HorizontalContentAlignment" Value="Stretch"/>
        <Setter Property="Padding" Value="0"/>
        <Setter Property="Margin" Value="0"/>
    </Style>
</ListView.ItemContainerStyle>
```

핵심: `HorizontalContentAlignment="Stretch"`와 `Padding="0"`으로 ListViewItem 내부 여백을 제거하여 카드 Border와 선택 하이라이트 영역을 일치시킨다.

### 커밋
```
fix(ui): 승인 대기 카드 선택 하이라이트 영역 정렬
```

---

## FIX 2: 승인 대기 카드 가로 길이 통일

**증상**: 카드마다 내용 길이에 따라 가로 폭이 다름
**원인**: ListView 아이템이 내용에 맞춰 축소됨

### 수정 파일: `NuriyeApp/Views/PendingPage.xaml`

### 해결 방법
FIX 1의 `ItemContainerStyle`에 이미 `HorizontalContentAlignment="Stretch"`가 포함되어 있다. 이것이 적용되면 모든 카드가 ListView 전체 너비로 확장된다.

추가로 DataTemplate 내 최상위 Border에 아래 속성을 확인:
- `HorizontalAlignment="Stretch"` (명시적 추가)
- `MinWidth` 제거 (있다면)

### 커밋
```
fix(ui): 승인 대기 카드 가로 길이 전체 너비로 통일
```

> FIX 1과 FIX 2는 동일 파일의 연관 수정이므로 하나의 커밋으로 합쳐도 무방하다.

---

## FIX 3: 승인 대기 카드 전체 크기 20% 확대

**증상**: 카드 내 텍스트와 여백이 작아 가시성 부족
**목표**: 패딩, 폰트 크기, 배지 크기를 약 20% 키움

### 수정 파일: `NuriyeApp/Views/PendingPage.xaml` — DataTemplate 내부

### 변경 사항

| 요소 | 현재 | 변경 |
|------|------|------|
| 카드 Border Padding | `16` | `20` |
| 카드 Border Margin(하단) | `8` | `10` |
| 신청자 이름 (BodyStrongTextBlockStyle) | 기본(14) | `FontSize="17"` 명시 |
| 연락처 (CaptionTextBlockStyle) | 기본(12) | `FontSize="14"` 명시 |
| 장비명 TextBlock | 기본(14) | `FontSize="16"` 명시 |
| 날짜 신청일시 (CaptionTextBlockStyle) | 기본(12) | `FontSize="13"` 명시 |
| 기간 배지 내 텍스트 FontSize | `11` | `13` |
| 기간 배지 Padding | `8,3` | `10,4` |
| 액세서리 배지 내 텍스트 FontSize | `11` | `13` |
| 액세서리 배지 Padding | `8,3` | `10,4` |
| Grid RowSpacing | `6` | `8` |

### 커밋
```
fix(ui): 승인 대기 카드 크기 및 텍스트 20% 확대
```

---

## FIX 4: 승인 대기 카드 배경색을 배경보다 밝게

**증상**: 다크 모드에서 카드 배경이 앱 배경과 비슷하거나 어두워서 구분이 안 됨
**목표**: 카드가 배경보다 확실히 밝아야 함

### 수정 파일: `NuriyeApp/Views/PendingPage.xaml` — DataTemplate 내 Border

### 해결 방법
카드 Border의 Background를 변경:

**현재** (둘 중 하나일 것):
```xml
Background="{ThemeResource NuriyeSurfaceBrush}"
<!-- 또는 -->
Background="{ThemeResource CardBackgroundFillColorDefaultBrush}"
```

**변경**:
```xml
Background="{ThemeResource LayerFillColorDefaultBrush}"
```

`LayerFillColorDefaultBrush`는 WinUI 기본 리소스로, 다크 모드에서 배경(#1E1E1E)보다 밝은 회색(약 #3A3A3A)을 제공한다. 만약 이것도 부족하면 `CardBackgroundFillColorDefaultBrush`를 사용하되, 앱 전체 배경이 이보다 어두운지 확인한다.

또는 Themes/BrandColors.xaml에 커스텀 브러시를 정의:
```xml
<!-- Dark -->
<SolidColorBrush x:Key="NuriyeCardBrush" Color="#383838"/>
<!-- Light -->
<SolidColorBrush x:Key="NuriyeCardBrush" Color="#FFFFFF"/>
```

### 커밋
```
fix(ui): 승인 대기 카드 배경색을 앱 배경보다 밝게 조정
```

---

## FIX 5: 라이트 모드 전환 시 어두운 초록색이 민트색으로 안 바뀌는 오류

**증상**: 다크 모드의 진한 초록(#004D40, #00695C 등)이 라이트 모드로 전환해도 그대로 유지됨
**원인**: 하드코딩된 색상값이 ThemeDictionaries를 거치지 않고 직접 사용됨

### 수정 대상 파일들
- `NuriyeApp/Themes/BrandColors.xaml` — 테마 리소스 확인/추가
- `NuriyeApp/Views/PendingPage.xaml` — 하드코딩 색상 제거
- `NuriyeApp/Views/ShellPage.xaml` — NavigationView 관련
- `NuriyeApp/Views/MainPage.xaml` — TabView 관련

### 해결 방법

1. **Themes/BrandColors.xaml에 아래 리소스가 Light/Dark 양쪽에 정의되어 있는지 확인**:

```xml
<ResourceDictionary.ThemeDictionaries>
    <ResourceDictionary x:Key="Light">
        <SolidColorBrush x:Key="NuriyeBrandBrush" Color="#B2DFDB"/>
        <SolidColorBrush x:Key="NuriyeBrandDarkBrush" Color="#00897B"/>
        <SolidColorBrush x:Key="NuriyeNavActiveBrush" Color="#4DB2DFDB"/>
        <SolidColorBrush x:Key="NuriyeAccentBrush" Color="#00897B"/>
    </ResourceDictionary>
    <ResourceDictionary x:Key="Dark">
        <SolidColorBrush x:Key="NuriyeBrandBrush" Color="#004D40"/>
        <SolidColorBrush x:Key="NuriyeBrandDarkBrush" Color="#00695C"/>
        <SolidColorBrush x:Key="NuriyeNavActiveBrush" Color="#73004D40"/>
        <SolidColorBrush x:Key="NuriyeAccentBrush" Color="#00695C"/>
    </ResourceDictionary>
</ResourceDictionary.ThemeDictionaries>
```

2. **모든 XAML 파일에서 하드코딩된 초록 계열 색상을 검색하여 ThemeResource로 교체**:

```bash
# 이 명령으로 하드코딩된 색상을 찾아라
grep -rn "#004D40\|#00695C\|#004246\|#1B5E20" NuriyeApp/Views/ NuriyeApp/Themes/
```

- `Background="#004D40"` → `Background="{ThemeResource NuriyeBrandBrush}"`
- `Background="#00695C"` → `Background="{ThemeResource NuriyeAccentBrush}"`
- 기간 배지의 `Background="#E8F5E9"` / `Foreground="#2E7D32"`도 ThemeResource로 전환:
  - BrandColors.xaml에 `NuriyeDateBadgeBgBrush` 추가 (Light: `#E8F5E9`, Dark: `#1B5E20`)
  - BrandColors.xaml에 `NuriyeDateBadgeFgBrush` 추가 (Light: `#2E7D32`, Dark: `#A5D6A7`)

3. **AccentButtonStyle의 Background도 확인**: 승인 버튼 등에서 `Background="#00695C"` 같은 하드코딩이 있으면 `NuriyeAccentBrush`로 교체

### 커밋
```
fix(ui): 라이트/다크 모드 전환 시 브랜드 컬러 정상 반영
```

---

## FIX 6: 대여 신청 탭에서 렌즈 선택 시 "장비 선택" 영역 좁아짐

**증상**: 렌즈 ComboBox에서 항목을 선택하면 좌측 "장비 선택" 컬럼이 좁아짐
**원인**: 렌즈명이 길어서 ComboBox가 확장되면서 Grid 컬럼 비율이 변동하거나, ComboBox에 고정 Width가 없어 내용에 따라 크기가 변함

### 수정 파일: `NuriyeApp/Views/RentalFormPage.xaml`

### 해결 방법

1. 좌우 2컬럼 Grid의 ColumnDefinitions를 `*`에서 고정 비율로 변경:
```xml
<Grid.ColumnDefinitions>
    <ColumnDefinition Width="*" MinWidth="300"/>
    <ColumnDefinition Width="20"/>
    <ColumnDefinition Width="*" MinWidth="300"/>
</Grid.ColumnDefinitions>
```

2. 모든 ComboBox에 `HorizontalAlignment="Stretch"` 확인 및 `MaxWidth` 제거

3. 렌즈 ComboBox의 ItemTemplate 내 TextBlock에 `TextTrimming="CharacterEllipsis"` 추가하여 긴 렌즈명이 넘치지 않게 처리

### 커밋
```
fix(ui): 대여 신청 렌즈 선택 시 레이아웃 축소 방지
```

---

## FIX 7: 대여 신청 탭에서 바디/렌즈 선택 취소 기능

**증상**: 한번 선택한 바디/렌즈를 해제할 수 없음
**목표**: "선택 안 함" 옵션을 추가하거나, 선택을 null로 되돌릴 수 있게 함

### 수정 파일
- `NuriyeApp/ViewModels/RentalFormViewModel.cs`
- `NuriyeApp/Views/RentalFormPage.xaml`

### 해결 방법

**방법 A (권장): ComboBox에 PlaceholderText 활용 + 클리어 버튼**

각 ComboBox 옆에 작은 "✕" 버튼을 추가:

```xml
<Grid ColumnSpacing="4">
    <Grid.ColumnDefinitions>
        <ColumnDefinition Width="*"/>
        <ColumnDefinition Width="Auto"/>
    </Grid.ColumnDefinitions>
    <ComboBox Grid.Column="0"
              Header="카메라 바디"
              ItemsSource="{x:Bind ViewModel.Bodies, Mode=OneWay}"
              SelectedItem="{x:Bind ViewModel.SelectedBody, Mode=TwoWay}"
              HorizontalAlignment="Stretch"
              PlaceholderText="바디 선택 (선택)">
        <!-- 기존 ItemTemplate 유지 -->
    </ComboBox>
    <Button Grid.Column="1"
            Content="✕"
            VerticalAlignment="Bottom"
            Margin="0,0,0,0"
            Padding="8,6"
            Command="{x:Bind ViewModel.ClearBodyCommand}"/>
</Grid>
```

렌즈 ComboBox에도 동일하게 적용, `ClearLensCommand` 사용.

**ViewModel 수정** (`RentalFormViewModel.cs`):

```csharp
[RelayCommand]
private void ClearBody()
{
    SelectedBody = null;
}

[RelayCommand]
private void ClearLens()
{
    SelectedLens = null;
}
```

이 두 메서드만 추가하면 된다. 기존 `OnSelectedBodyChanged`에서 `null` 처리는 이미 되어 있다(`RefreshLenses(null)` 호출).

### 커밋
```
feat(ui): 대여 신청 바디/렌즈 선택 취소(초기화) 버튼 추가
```

---

## 작업 완료 확인

```bash
git log --oneline -7
```

7개 (또는 FIX 1+2 합쳐서 6개) 커밋이 모두 있으면 완료.
