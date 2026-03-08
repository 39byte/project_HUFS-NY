# 작업 지시: NuriyeApp 커스텀 테마 시스템 구축 (ny-a.1.2)

## 배경
NuriyeApp은 WinUI 3 기반 데스크탑 앱으로, 현재 기본 WinUI 컨트롤만 사용 중이라 UI가 단조롭다.
기존 Python/Streamlit 웹사이트(`style.css`)의 브랜드 테마를 WinUI 3에 이식하여 시각적 완성도를 높인다.

## 브랜드 디자인 토큰 (웹사이트 기반)

### 컬러 팔레트
```
[Light Mode]
- Background:       #F5F5F5 (앱 전체 배경)
- Surface:          #FFFFFF (카드, 패널)
- SurfaceAlt:       #FAFAFA (비활성 영역, 탭 바)
- Text:             #1A1A1A
- TextSecondary:    #666666
- TextTertiary:     #999999
- Brand:            #B2DFDB (헤더바, 하이라이트)
- BrandDark:        #00897B (주요 버튼, 강조)
- BrandDeep:        #004D40 (텍스트 강조)
- BrandGlow:        rgba(178, 223, 219, 0.35)
- Border:           #E8E8E8
- InputBg:          #FFFFFF
- NavActive:        rgba(178, 223, 219, 0.3)
- AccentRed:        #EF5350
- AccentBlue:       #42A5F5
- AccentGreen:      #66BB6A
- AccentAmber:      #FFA726

[Dark Mode]
- Background:       #1E1E1E
- Surface:          #2D2D2D
- SurfaceAlt:       #252526
- Text:             #E0E0E0
- TextSecondary:    #A0A0A0
- TextTertiary:     #707070
- Brand:            #004D40 (헤더바)
- BrandDark:        #00695C (주요 버튼)
- BrandDeep:        #004246
- BrandGlow:        rgba(0, 77, 64, 0.4)
- Border:           #3E3E3E
- InputBg:          #3C3C3C
- NavActive:        rgba(0, 77, 64, 0.45)
- AccentRed:        #FF5252
- AccentBlue:       #448AFF
- AccentGreen:      #69F0AE
- AccentAmber:      #FFD740

[D-Day 배지 컬러]
- 여유(D-3 이상):   배경 #E8F5E9 / 텍스트 #2E7D32  (다크: #1B5E20 / #A5D6A7)
- 주의(D-2~D-1):   배경 #FFF3E0 / 텍스트 #E65100  (다크: #4E342E / #FFB74D)
- 긴급(D-Day/연체): 배경 #FFEBEE / 텍스트 #C62828  (다크: #4A1A1A / #FF8A80)
```

### 수치 토큰
```
- CornerRadius (카드):     12px
- CornerRadius (버튼/입력): 8px
- CornerRadius (배지):      8px
- CardShadow (Light):      0 2px 12px rgba(0,0,0,0.06)
- Spacing (섹션 간):       24px
- Spacing (요소 간):       14px
- NavigationView 좌측 Active Indicator: 3px solid BrandDark
```

---

## 작업 단계

### STEP 1: Themes 폴더 및 ResourceDictionary 생성

`NuriyeApp/Themes/` 폴더를 만들고 아래 파일들을 생성한다.

#### 1-1. `NuriyeApp/Themes/BrandColors.xaml`
- 위 컬러 팔레트의 모든 색상을 `<Color>` 및 `<SolidColorBrush>` 리소스로 정의
- Light/Dark 테마별로 `ResourceDictionary.ThemeDictionaries` 사용
- 키 네이밍 규칙: `NuriyeBrandBrush`, `NuriyeSurfaceBrush`, `NuriyeTextBrush` 등 `Nuriye` 접두사 사용

#### 1-2. `NuriyeApp/Themes/ControlStyles.xaml`
아래 컨트롤에 대한 암시적(Implicit) 또는 키 기반 스타일 정의:

**Button:**
- 기본 버튼: `NuriyeSurfaceBrush` 배경, `NuriyeBorderBrush` 테두리, CornerRadius 8
- AccentButton (Style Key: `NuriyeAccentButtonStyle`): `NuriyeBrandDarkBrush` 배경, 흰색 텍스트, CornerRadius 8

**NavigationViewItem:**
- 선택 시 좌측에 3px 두께의 `NuriyeBrandDarkBrush` 세로 인디케이터
- 선택 시 배경: `NuriyeNavActiveBrush`

**TabViewItem:**
- 선택 시 하단에 2px `NuriyeBrandDarkBrush` 보더 라인
- 미선택 시 보더 투명

**TextBox / PasswordBox / ComboBox / DatePicker:**
- CornerRadius 8, `NuriyeInputBgBrush` 배경, `NuriyeBorderBrush` 테두리

**ListView ItemContainer:**
- CornerRadius 10, 선택 시 `NuriyeNavActiveBrush` 배경

**InfoBar:**
- CornerRadius 10

#### 1-3. `NuriyeApp/Themes/CardStyles.xaml`
- `NuriyeCardStyle` (Border용): `NuriyeSurfaceBrush` 배경, CornerRadius 12, `NuriyeBorderBrush` 테두리
- `NuriyeCardAltStyle`: `NuriyeSurfaceAltBrush` 배경 버전
- `NuriyeSectionHeaderStyle` (TextBlock용): FontSize 15, FontWeight SemiBold, 하단에 2px BrandDark 보더

### STEP 2: App.xaml에 테마 리소스 연결

`App.xaml`의 `ResourceDictionary.MergedDictionaries`에 세 파일 추가:
```xml
<ResourceDictionary Source="Themes/BrandColors.xaml"/>
<ResourceDictionary Source="Themes/ControlStyles.xaml"/>
<ResourceDictionary Source="Themes/CardStyles.xaml"/>
```

### STEP 3: 커스텀 타이틀바 적용

`MainWindow.xaml.cs`에서:
- `ExtendsContentIntoTitleBar = true` 설정
- 타이틀바 영역에 `NuriyeBrandBrush` 배경색 적용
- 앱 타이틀 "📸 누리예 카메라 대여 관리" 표시
- 타이틀바 텍스트 색상: Light → `#004D40`, Dark → `#B2DFDB`

### STEP 4: ShellPage NavigationView 스타일링

`ShellPage.xaml`에서:
- `NavigationView`의 PaneTitle을 "누리예 대여"로 유지
- 각 `NavigationViewItem`에 커스텀 스타일 적용 (STEP 1-2에서 정의한 것)
- 하단 로그아웃 버튼에 기본 버튼 스타일 적용

### STEP 5: D-Day 배지 커스텀 컨트롤 생성

`NuriyeApp/Controls/DDayBadge.xaml(.cs)` UserControl 생성:
- DependencyProperty: `DDayText` (string), `Severity` (enum: Normal, Warning, Critical)
- Severity에 따라 위 D-Day 배지 컬러 자동 적용
- CornerRadius 8, Padding 8,4, FontWeight Bold, FontSize 13
- `OngoingPage.xaml`과 `PendingPage.xaml`의 기존 D-Day 표시를 이 컨트롤로 교체

### STEP 6: 각 페이지에 새 스타일 적용

#### 6-1. CalendarPage
- 월 네비게이션 버튼에 기본 버튼 스타일 적용
- 캘린더 전체를 `NuriyeCardStyle` Border로 감싸기
- 요일 헤더 행: `NuriyeSurfaceAltBrush` 배경
- 오늘 날짜 셀: `NuriyeBrandGlowBrush` 배경, Bold 텍스트

#### 6-2. RentalFormPage ⚠️ 레이아웃 변경 필수
현재 순수 세로 StackPanel 구조 → **2컬럼 레이아웃**으로 변경:

```
┌─────────────────────────────────────────┐
│              대여 신청 (타이틀)            │
├────────────────────┬────────────────────┤
│ [좌측 컬럼]         │ [우측 컬럼]         │
│                    │                    │
│ ■ 장비 선택         │ ■ 신청자 정보       │
│   카테고리 선택      │   이름             │
│   바디 선택         │   연락처            │
│   렌즈 선택         │                    │
│                    │ ■ 대여 기간         │
│ ■ 액세서리          │   시작일  반납일     │
│   [충전기][리더기]   │                    │
│   [가방] [삼각대]    │ ■ 대면 가능 시간    │
│                    │   대여 시작~종료     │
│ ■ 추가 요청사항     │   반납 시작~종료     │
│   (텍스트 입력)      │                    │
├────────────────────┴────────────────────┤
│          [ 신청하기 버튼 (전체 너비) ]      │
│          (결과 메시지 InfoBar)             │
└─────────────────────────────────────────┘
```

구현 방법:
- 최상위 ScrollViewer > StackPanel (MaxWidth 720, HorizontalAlignment Center)
- 타이틀 "대여 신청"
- **Grid (ColumnDefinitions: *, 20(gap), *)** 안에 좌우 컬럼 배치
- 좌측: 장비 선택 + 액세서리 + 추가 요청사항
- 우측: 신청자 정보 + 대여 기간 + 대면 가능 시간
- 각 섹션 제목에 `NuriyeSectionHeaderStyle` 적용 (하단 2px BrandDark 라인)
- 하단: 신청하기 버튼 (Grid.ColumnSpan="3"으로 전체 너비)
- 액세서리 ToggleButton들은 2x2 Grid 또는 WrapPanel 배치
- 대여 기간의 DatePicker 2개를 가로 배치 (Grid 2열)
- 대면 시간도 "시작~종료" ComboBox 쌍을 가로 배치

`RentalFormPage.xaml`을 완전히 다시 작성한다. ViewModel(`RentalFormViewModel.cs`) 바인딩은 기존 것을 유지한다.

#### 6-3. PendingPage / OngoingPage
- 리스트 아이템에 CornerRadius 10 카드 스타일 적용
- 선택된 아이템에 Brand 컬러 강조 보더
- 우측 패널(`ScrollViewer`)에 `NuriyeSurfaceAltBrush` 배경
- 승인/반려 버튼: 승인 → `NuriyeAccentButtonStyle`, 반려 → 기본 버튼

#### 6-4. MainPage
- TabView 탭 하단에 선택 인디케이터 스타일 적용
- 상단 타이틀바 영역의 "누리예 카메라 대여 관리" 텍스트 유지

#### 6-5. LoginPage
- 중앙 로그인 카드를 `NuriyeCardStyle` Border로 감싸기
- 로그인 버튼에 `NuriyeAccentButtonStyle` 적용
- 타이틀 텍스트 색상 Brand 계열로 변경

#### 6-6. HistoryPage / InventoryPage
- DataGrid의 AlternatingRowBackground를 `NuriyeSurfaceAltBrush`로 변경
- 헤더 행 배경을 `NuriyeSurfaceAltBrush`로

#### 6-7. SettingsPage
- 입력 필드에 새 TextBox/PasswordBox 스타일 자동 적용
- 변경 버튼에 `NuriyeAccentButtonStyle`

### STEP 7: 앱 하단 Footer 추가 (선택)

`ShellPage.xaml`의 `NavigationView` 안 하단에 작은 footer 텍스트:
- "제작 | 45-1기 암실차장 한지원 · Finance&AI융합학부"
- 색상: `NuriyeTextTertiaryBrush`, FontSize 10

---

## 주의사항

1. **기존 기능 보존**: ViewModel 로직, 데이터 바인딩, Supabase 연동은 일절 수정하지 않는다. 순수 UI/XAML 레이어만 변경한다.
2. **ThemeDictionaries 필수**: 모든 브러시는 Light/Dark 양쪽 모두 정의한다. 시스템 테마 전환 시 자동으로 반영되어야 한다.
3. **RentalFormPage 레이아웃**: 세로로만 길어지는 것을 방지하기 위해 반드시 2컬럼 구조로 변경한다. MaxWidth 720px 기준.
4. **NuGet 추가 금지**: 현재 패키지만으로 구현한다. (CommunityToolkit.Mvvm, CommunityToolkit.WinUI.UI.Controls.DataGrid, WindowsAppSDK, Supabase)
5. **컴파일 에러 없이** 완료해야 한다. 모든 리소스 키 참조와 x:Bind 경로를 정확히 맞춘다.
6. **csproj 업데이트**: 새로 추가한 XAML 파일(Themes/*.xaml, Controls/DDayBadge.xaml)이 빌드에 포함되도록 확인한다.

## 커밋

작업 완료 후 커밋 메시지:
```
feat(ui): 브랜드 테마 시스템 구축 및 전 페이지 적용 (ny-a.1.2)

- Themes/ 폴더에 BrandColors, ControlStyles, CardStyles ResourceDictionary 생성
- Light/Dark 모드 브랜드 컬러 팔레트 정의 (#B2DFDB / #004D40 계열)
- 커스텀 타이틀바 적용 (ExtendsContentIntoTitleBar)
- DDayBadge 커스텀 컨트롤 생성 (Normal/Warning/Critical)
- RentalFormPage 2컬럼 레이아웃으로 재설계
- 전 페이지에 카드 스타일, 배지 컬러, 섹션 헤더 스타일 적용
```
