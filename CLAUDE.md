# 작업 지시: PendingPage UI 수정 (승인 대기 탭)

> 이 작업은 독립적인 3개 단계로 구성되어 있다. 각 단계 완료 후 커밋하라.
> 중단 시 git log로 마지막 커밋을 확인하면 어디서 이어야 하는지 알 수 있다.

---

## STEP 1: 탭 이름 옆 빨간 배지 (숫자 표시)

**현재**: "승인 대기" 탭에 대기 건수 표시 없음
**목표**: "승인 대기" 탭 이름 오른쪽에 빨간 원형 배지로 대기 건수 표시 (예: 빨간 동그라미 안에 흰색 "2")

### 수정 파일
- `NuriyeApp/Views/MainPage.xaml`
- `NuriyeApp/Views/MainPage.xaml.cs`

### 구현 방법
1. "승인 대기" TabViewItem의 Header를 단순 문자열 대신 StackPanel으로 교체:
```xml
<TabViewItem IsClosable="False">
    <TabViewItem.Header>
        <StackPanel Orientation="Horizontal" Spacing="6">
            <TextBlock Text="승인 대기" VerticalAlignment="Center"/>
            <Border x:Name="PendingBadge"
                    Background="#EF5350"
                    CornerRadius="10"
                    MinWidth="20" Height="20"
                    Padding="5,0"
                    VerticalAlignment="Center"
                    Visibility="Collapsed">
                <TextBlock x:Name="PendingBadgeText"
                           Foreground="White"
                           FontSize="11"
                           FontWeight="Bold"
                           HorizontalAlignment="Center"
                           VerticalAlignment="Center"/>
            </Border>
        </StackPanel>
    </TabViewItem.Header>
    ...
</TabViewItem>
```

2. `MainPage.xaml.cs`에서 PendingPageControl의 데이터 로드 완료 후 배지 업데이트:
   - `PendingPageControl.ViewModel.Rentals.CollectionChanged` 이벤트 구독
   - 또는 탭 전환 시(`MainTabView_SelectionChanged`) 승인 대기 탭 진입 전에 갱신
   - 건수가 0이면 `PendingBadge.Visibility = Collapsed`, 1 이상이면 `Visible`로 설정
   - `PendingBadgeText.Text = count.ToString()`

### 커밋
```
fix(ui): 승인 대기 탭에 빨간 배지로 대기 건수 표시
```

---

## STEP 2: 승인 대기 목록 카드 UI 개선

**현재**: 리스트 아이템이 평면적이고 신청자/장비/날짜 정보가 밀집되어 가독성 낮음
**목표**: 각 항목이 둥근 카드 형태, 내부 레이아웃을 아래 구조로 변경

```
┌─────────────────────────────────────────────────────┐
│ 정하은                              2026-03-08 09:32 │
│ 010-1234-5678                                       │
│                                                     │
│ Nikon D7500 + AF-S 50mm f/1.4                       │
│                                                     │
│ [03.12 ~ 03.15]  SD리더기                            │
│  (초록 배지)       (회색 배지)                         │
└─────────────────────────────────────────────────────┘
```

### 수정 파일
- `NuriyeApp/Views/PendingPage.xaml` — ListView의 ItemTemplate만 교체

### ItemTemplate 교체 내용

```xml
<DataTemplate x:DataType="models:Rental">
    <Border Background="{ThemeResource NuriyeSurfaceBrush}"
            BorderBrush="{ThemeResource NuriyeBorderBrush}"
            BorderThickness="1"
            CornerRadius="10"
            Padding="16"
            Margin="0,0,0,8">
        <Grid RowSpacing="6">
            <Grid.RowDefinitions>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="Auto"/>
                <RowDefinition Height="Auto"/>
            </Grid.RowDefinitions>

            <!-- Row 0: 신청자 이름 + 날짜 -->
            <Grid Grid.Row="0">
                <Grid.ColumnDefinitions>
                    <ColumnDefinition Width="*"/>
                    <ColumnDefinition Width="Auto"/>
                </Grid.ColumnDefinitions>
                <StackPanel Grid.Column="0" Spacing="2">
                    <TextBlock Text="{x:Bind Applicant}"
                               Style="{StaticResource BodyStrongTextBlockStyle}"/>
                    <TextBlock Text="{x:Bind Contact}"
                               Style="{StaticResource CaptionTextBlockStyle}"
                               Foreground="{ThemeResource TextFillColorSecondaryBrush}"/>
                </StackPanel>
                <TextBlock Grid.Column="1"
                           Text="{x:Bind SubmittedAt}"
                           Style="{StaticResource CaptionTextBlockStyle}"
                           Foreground="{ThemeResource TextFillColorTertiaryBrush}"
                           VerticalAlignment="Top"/>
            </Grid>

            <!-- Row 1: 장비명 -->
            <TextBlock Grid.Row="1"
                       Text="{x:Bind Equipment}"
                       TextWrapping="Wrap"
                       Margin="0,4,0,0"/>

            <!-- Row 2: 기간 배지 + 액세서리 배지 -->
            <StackPanel Grid.Row="2" Orientation="Horizontal" Spacing="8" Margin="0,4,0,0">
                <Border Background="#E8F5E9" CornerRadius="4" Padding="8,3">
                    <TextBlock Foreground="#2E7D32" FontSize="11" FontWeight="SemiBold">
                        <Run Text="{x:Bind StartDate}"/>
                        <Run Text=" ~ "/>
                        <Run Text="{x:Bind EndDate}"/>
                    </TextBlock>
                </Border>
                <Border Background="{ThemeResource NuriyeSurfaceAltBrush}"
                        CornerRadius="4" Padding="8,3">
                    <TextBlock Text="{x:Bind Accessories}"
                               FontSize="11"
                               Foreground="{ThemeResource TextFillColorSecondaryBrush}"/>
                </Border>
            </StackPanel>
        </Grid>
    </Border>
</DataTemplate>
```

> **리소스 키 참고**: `NuriyeSurfaceBrush` 등이 없으면 아래 WinUI 기본 리소스로 대체:
> - `NuriyeSurfaceBrush` → `CardBackgroundFillColorDefaultBrush`
> - `NuriyeBorderBrush` → `CardStrokeColorDefaultBrush`
> - `NuriyeSurfaceAltBrush` → `CardBackgroundFillColorSecondaryBrush`

### 커밋
```
fix(ui): 승인 대기 목록 카드형 레이아웃 및 배지 UI 적용
```

---

## STEP 3: 우측 "신청 처리" 패널 정리

**현재**: InfoBar에 파란 아이콘이 붙어있고, 장비 정보 카드가 목업과 다름
**목표**: InfoBar를 제거하고, 깔끔한 Border 카드로 장비 정보 표시

### 수정 파일
- `NuriyeApp/Views/PendingPage.xaml` — 우측 ScrollViewer 내부만 수정

### 변경 사항

1. **InfoBar를 Border 카드로 교체**: 기존 `<InfoBar>` 제거 → `<Border>` 카드
```xml
<Border Background="{ThemeResource SystemFillColorAttentionBackgroundBrush}"
        CornerRadius="10"
        Padding="16"
        Margin="0,0,0,16">
    <StackPanel Spacing="4">
        <TextBlock Text="{x:Bind ViewModel.SelectedRental.Equipment, Mode=OneWay}"
                   Style="{StaticResource BodyStrongTextBlockStyle}"
                   TextWrapping="Wrap"/>
        <TextBlock Foreground="{ThemeResource TextFillColorSecondaryBrush}">
            <Run Text="{x:Bind ViewModel.SelectedRental.StartDate, Mode=OneWay}"/>
            <Run Text=" ~ "/>
            <Run Text="{x:Bind ViewModel.SelectedRental.EndDate, Mode=OneWay}"/>
        </TextBlock>
        <TextBlock Text="{x:Bind ViewModel.SelectedRental.MeetingTime, Mode=OneWay}"
                   Style="{StaticResource CaptionTextBlockStyle}"
                   Foreground="{ThemeResource TextFillColorSecondaryBrush}"/>
        <TextBlock Style="{StaticResource CaptionTextBlockStyle}"
                   Foreground="{ThemeResource TextFillColorSecondaryBrush}">
            <Run Text="액세서리: "/>
            <Run Text="{x:Bind ViewModel.SelectedRental.Accessories, Mode=OneWay}"/>
        </TextBlock>
    </StackPanel>
</Border>
```

2. **승인/반려 버튼**: 승인 → `AccentButtonStyle` 유지, 반려 → 기본 스타일 유지. 동일 너비 가로 배치.

3. 기존 `x:Name="RentalInfoBar"` 참조가 code-behind에 있으면 제거.

### 커밋
```
fix(ui): 신청 처리 패널 — InfoBar를 카드로 교체, 레이아웃 정리
```

---

## 작업 완료 확인

세 커밋이 모두 있으면 완료:
```bash
git log --oneline -3
# 예상:
# fix(ui): 신청 처리 패널 — InfoBar를 카드로 교체, 레이아웃 정리
# fix(ui): 승인 대기 목록 카드형 레이아웃 및 배지 UI 적용
# fix(ui): 승인 대기 탭에 빨간 배지로 대기 건수 표시
```
