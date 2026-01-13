# Leaderboard Unity Editor Setup - Complete Detailed Guide

This is a **step-by-step guide** to set up the leaderboard feature in Unity Editor. Follow each step carefully.

## Prerequisites Checklist

Before starting, ensure you have:
- ✅ Unity Editor open with your project
- ✅ `MainScene 1` scene available
- ✅ FirebaseManager prefab exists
- ✅ Avatar sprites ready (6 sprites for avatars 0-5)
- ✅ A default avatar sprite

---

## PART 1: Create LeaderboardEntry Prefab

### Step 1.1: Create the Base GameObject

1. **Open any scene** (we'll create prefabs, then delete from scene)
2. **Right-click in Hierarchy** → **Create Empty**
3. **Name it**: `LeaderboardEntry`
4. **Set Position**: X=0, Y=0, Z=0

### Step 1.2: Create Background Panel

1. **Right-click on `LeaderboardEntry`** → **UI → Panel**
2. **Name it**: `BackgroundPanel`
3. **Select `BackgroundPanel`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Hold **Alt + Shift** and click **Stretch-Stretch** (bottom-right preset)
     - **Left**: 0
     - **Right**: 0
     - **Top**: 0
     - **Bottom**: 0
   - **Image Component**:
     - **Color**: White (or your theme color)
     - **Source Image**: None (or a background sprite if you have one)

4. **Set Height**:
   - In RectTransform, set **Height**: `80` (this is the entry height)

### Step 1.3: Add Rank Text

1. **Right-click on `BackgroundPanel`** → **UI → Text - TextMeshPro**
2. **Name it**: `RankText`
3. **Select `RankText`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Click **Left** preset
     - **Pos X**: `20`
     - **Pos Y**: `0`
     - **Width**: `50`
     - **Height**: `80`
   - **TextMeshProUGUI Component**:
     - **Text**: `1`
     - **Font Size**: `24`
     - **Alignment**: Center (both horizontal and vertical)
     - **Color**: Black (or your text color)

### Step 1.4: Add Avatar Image

1. **Right-click on `BackgroundPanel`** → **UI → Image**
2. **Name it**: `AvatarImage`
3. **Select `AvatarImage`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Click **Left** preset
     - **Pos X**: `80`
     - **Pos Y**: `0`
     - **Width**: `60`
     - **Height**: `60`
   - **Image Component**:
     - **Source Image**: Assign a default avatar sprite (temporary, will be set by script)

### Step 1.5: Add Username Text

1. **Right-click on `BackgroundPanel`** → **UI → Text - TextMeshPro**
2. **Name it**: `UsernameText`
3. **Select `UsernameText`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Click **Left** preset
     - **Pos X**: `150`
     - **Pos Y**: `0`
     - **Width**: `200`
     - **Height**: `80`
   - **TextMeshProUGUI Component**:
     - **Text**: `PlayerName`
     - **Font Size**: `22`
     - **Alignment**: Left (horizontal), Center (vertical)
     - **Color**: Black

### Step 1.6: Add Score Text

1. **Right-click on `BackgroundPanel`** → **UI → Text - TextMeshPro**
2. **Name it**: `ScoreText`
3. **Select `ScoreText`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Click **Right** preset
     - **Pos X**: `-20`
     - **Pos Y**: `0`
     - **Width**: `150`
     - **Height**: `80`
   - **TextMeshProUGUI Component**:
     - **Text**: `0`
     - **Font Size**: `24`
     - **Alignment**: Right (horizontal), Center (vertical)
     - **Color**: Black

### Step 1.7: Add LeaderboardEntry Component

1. **Select `LeaderboardEntry`** GameObject (the root)
2. **In Inspector**, click **Add Component**
3. **Search for**: `LeaderboardEntry`
4. **Click** to add the component

### Step 1.8: Assign References in LeaderboardEntry Component

**Select `LeaderboardEntry`** GameObject, in Inspector find **LeaderboardEntry** component:

1. **Rank Text**: Drag `RankText` from Hierarchy
2. **Avatar Image**: Drag `AvatarImage` from Hierarchy
3. **Username Text**: Drag `UsernameText` from Hierarchy
4. **Score Text**: Drag `ScoreText` from Hierarchy
5. **Background Image**: Drag `BackgroundPanel` from Hierarchy
6. **Avatar Sprites** (Array):
   - Click **+** to add 6 elements
   - Assign your 6 avatar sprites (indices 0-5)
7. **Default Avatar Sprite**: Assign a default sprite

### Step 1.9: Create the Prefab

1. **Drag `LeaderboardEntry`** from Hierarchy to `Assets/Prefabs/` folder
2. **Delete** `LeaderboardEntry` from Hierarchy (we don't need it in scene)

---

## PART 2: Create LeaderboardPopup Prefab

### Step 2.1: Create the Base GameObject

1. **Right-click in Hierarchy** → **Create Empty**
2. **Name it**: `LeaderboardPopup`
3. **Set Position**: X=0, Y=0, Z=0

### Step 2.2: Create Background Panel

1. **Right-click on `LeaderboardPopup`** → **UI → Panel**
2. **Name it**: `BackgroundPanel`
3. **Select `BackgroundPanel`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Click **Center** preset (middle preset)
     - **Pos X**: `0`
     - **Pos Y**: `0`
     - **Width**: `700`
     - **Height**: `900`
   - **Image Component**:
     - **Color**: Your theme color (e.g., slightly transparent white or dark)

### Step 2.3: Add Title Text

1. **Right-click on `BackgroundPanel`** → **UI → Text - TextMeshPro**
2. **Name it**: `TitleText`
3. **Select `TitleText`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Click **Top** preset
     - **Pos X**: `0`
     - **Pos Y**: `-30`
     - **Width**: `700`
     - **Height**: `50`
   - **TextMeshProUGUI Component**:
     - **Text**: `Leaderboard`
     - **Font Size**: `36`
     - **Alignment**: Center (both)
     - **Color**: Black

### Step 2.4: Add Close Button

1. **Right-click on `BackgroundPanel`** → **UI → Button - TextMeshPro**
2. **Name it**: `CloseButton`
3. **Select `CloseButton`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Click **Top-Right** preset
     - **Pos X**: `-20`
     - **Pos Y**: `-20`
     - **Width**: `40`
     - **Height**: `40`
   - **Button Component**: Leave default settings
   - **TextMeshProUGUI** (child): Set text to `X` or use an icon sprite

### Step 2.5: Create Tab System

#### Step 2.5a: Create Tab Container

1. **Right-click on `BackgroundPanel`** → **UI → Empty GameObject**
2. **Name it**: `TabContainer`
3. **Select `TabContainer`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Click **Top** preset
     - **Pos X**: `0`
     - **Pos Y**: `-80`
     - **Width**: `600`
     - **Height**: `50`
   - **Add Component** → **Horizontal Layout Group**:
     - **Spacing**: `10`
     - **Child Alignment**: Middle Center
     - **Child Force Expand**: ✅ **Width** (check this)
     - **Child Force Expand**: ❌ **Height** (uncheck this)

#### Step 2.5b: Create Tab 2D Button

1. **Right-click on `TabContainer`** → **UI → Button - TextMeshPro**
2. **Name it**: `Tab2DButton`
3. **Select `Tab2DButton`**, in Inspector:
   - **RectTransform**: Leave as is (Horizontal Layout Group will handle it)
   - **Button Component**: Leave default
   - **TextMeshProUGUI** (child): 
     - **Text**: `2D`
     - **Font Size**: `24`
     - **Alignment**: Center

4. **Create Indicator Image** (for active state):
   - **Right-click on `Tab2DButton`** → **UI → Image**
   - **Name it**: `Tab2DIndicator`
   - **Select `Tab2DIndicator`**, in Inspector:
     - **RectTransform**: Stretch-Stretch (Alt+Shift+click)
     - **Image Component**:
       - **Color**: Yellow/Orange (R:255, G:204, B:0, A:255)
       - **Source Image**: None (or a background sprite)

#### Step 2.5c: Create Tab 3D Button

1. **Right-click on `TabContainer`** → **UI → Button - TextMeshPro**
2. **Name it**: `Tab3DButton`
3. **Select `Tab3DButton`**, in Inspector:
   - **RectTransform**: Leave as is
   - **Button Component**: Leave default
   - **TextMeshProUGUI** (child):
     - **Text**: `3D`
     - **Font Size**: `24`
     - **Alignment**: Center

4. **Create Indicator Image**:
   - **Right-click on `Tab3DButton`** → **UI → Image**
   - **Name it**: `Tab3DIndicator`
   - **Select `Tab3DIndicator`**, in Inspector:
     - **RectTransform**: Stretch-Stretch
     - **Image Component**:
       - **Color**: Gray (R:180, G:180, B:180, A:255) - initially inactive
       - **Source Image**: None

### Step 2.6: Create Scroll Views (CRITICAL FOR SCROLLING)

#### Step 2.6a: Create 2D Tab Content with Scroll View

1. **Right-click on `BackgroundPanel`** → **UI → Scroll View**
2. **Name it**: `Tab2DContent`
3. **Select `Tab2DContent`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Hold **Alt + Shift** and click **Stretch-Stretch**
     - **Left**: `0`
     - **Right**: `0`
     - **Top**: `120` (leaves space for title and tabs)
     - **Bottom**: `200` (leaves space for current user panel)
   - **ScrollRect Component**:
     - **Content**: This will be auto-created, but verify it exists
     - **Horizontal**: ❌ Uncheck
     - **Vertical**: ✅ Check
     - **Movement Type**: Elastic
     - **Elasticity**: `0.1`
     - **Scroll Sensitivity**: `20`

4. **Find the Content GameObject** (child of `Tab2DContent`):
   - It should be named `Content` or similar
   - **Rename it**: `Content2D`
   - **Select `Content2D`**, in Inspector:
     - **RectTransform**:
       - **Anchor Presets**: Click **Top** preset
       - **Pos X**: `0`
       - **Pos Y**: `0`
       - **Width**: `680` (slightly less than parent width)
       - **Height**: `580` (initial height, will expand with content)
     - **Add Component** → **Vertical Layout Group**:
       - **Spacing**: `5`
       - **Padding**: Left: `10`, Right: `10`, Top: `10`, Bottom: `10`
       - **Child Alignment**: Upper Center
       - **Child Force Expand**: ✅ **Width** (check this)
       - **Child Force Expand**: ❌ **Height** (uncheck this)
       - **Child Control Size**: ✅ **Width** (check this)
       - **Child Control Size**: ❌ **Height** (uncheck this)
     - **Add Component** → **Content Size Fitter**:
       - **Vertical Fit**: Preferred Size
       - **Horizontal Fit**: Unconstrained

5. **Create Scrollbar** (if not auto-created):
   - **Right-click on `Tab2DContent`** → **UI → Scrollbar**
   - **Name it**: `ScrollbarVertical2D`
   - **Select `ScrollbarVertical2D`**, in Inspector:
     - **RectTransform**:
       - **Anchor Presets**: Click **Right** preset
       - **Pos X**: `-10`
       - **Width**: `20`
       - **Top**: `120`
       - **Bottom**: `200`
   - **Scrollbar Component**: Leave default
   - **In `Tab2DContent` ScrollRect**, assign:
     - **Vertical Scrollbar**: Drag `ScrollbarVertical2D`

#### Step 2.6b: Create 3D Tab Content with Scroll View

1. **Right-click on `BackgroundPanel`** → **UI → Scroll View**
2. **Name it**: `Tab3DContent`
3. **Select `Tab3DContent`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Stretch-Stretch (Alt+Shift+click)
     - **Left**: `0`
     - **Right**: `0`
     - **Top**: `120`
     - **Bottom**: `200`
   - **ScrollRect Component**: Same settings as 2D
   - **IMPORTANT**: In **GameObject** section, **uncheck** ✅ **Active** (hide it initially)

4. **Find the Content GameObject**:
   - **Rename it**: `Content3D`
   - **Select `Content3D`**, in Inspector:
     - **RectTransform**: Same as Content2D
     - **Add Component** → **Vertical Layout Group**: Same settings as Content2D
     - **Add Component** → **Content Size Fitter**: Same settings as Content2D

5. **Create Scrollbar**:
   - **Right-click on `Tab3DContent`** → **UI → Scrollbar**
   - **Name it**: `ScrollbarVertical3D`
   - **Select `ScrollbarVertical3D`**, same settings as 2D scrollbar
   - **In `Tab3DContent` ScrollRect**, assign:
     - **Vertical Scrollbar**: Drag `ScrollbarVertical3D`

### Step 2.7: Create Current User Panel

1. **Right-click on `BackgroundPanel`** → **UI → Panel**
2. **Name it**: `CurrentUserPanel`
3. **Select `CurrentUserPanel`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Click **Bottom** preset
     - **Pos X**: `0`
     - **Pos Y**: `0`
     - **Width**: `700`
     - **Height**: `180`
   - **Image Component**:
     - **Color**: Slightly darker than background (e.g., R:240, G:240, B:240)

#### Step 2.7a: Add Current User Avatar

1. **Right-click on `CurrentUserPanel`** → **UI → Image**
2. **Name it**: `CurrentUserAvatar`
3. **Select `CurrentUserAvatar`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Click **Left** preset
     - **Pos X**: `20`
     - **Pos Y**: `0`
     - **Width**: `80`
     - **Height**: `80`

#### Step 2.7b: Add Current User Username

1. **Right-click on `CurrentUserPanel`** → **UI → Text - TextMeshPro**
2. **Name it**: `CurrentUserUsername`
3. **Select `CurrentUserUsername`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Click **Left** preset
     - **Pos X**: `120`
     - **Pos Y**: `40`
     - **Width**: `200`
     - **Height**: `40`
   - **TextMeshProUGUI Component**:
     - **Text**: `Your Name`
     - **Font Size**: `24`
     - **Alignment**: Left

#### Step 2.7c: Add 2D Rank and Score

1. **Right-click on `CurrentUserPanel`** → **UI → Text - TextMeshPro**
2. **Name it**: `CurrentUserRank2D`
3. **Select `CurrentUserRank2D`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Click **Left** preset
     - **Pos X**: `120`
     - **Pos Y**: `0`
     - **Width**: `80`
     - **Height**: `30`
   - **TextMeshProUGUI Component**:
     - **Text**: `#1`
     - **Font Size**: `20`

4. **Right-click on `CurrentUserPanel`** → **UI → Text - TextMeshPro**
5. **Name it**: `CurrentUserScore2D`
6. **Select `CurrentUserScore2D`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Click **Left** preset
     - **Pos X**: `200`
     - **Pos Y**: `0`
     - **Width**: `150`
     - **Height**: `30`
   - **TextMeshProUGUI Component**:
     - **Text**: `Score: 0`
     - **Font Size**: `20`

#### Step 2.7d: Add 3D Rank and Score

1. **Right-click on `CurrentUserPanel`** → **UI → Text - TextMeshPro**
2. **Name it**: `CurrentUserRank3D`
3. **Select `CurrentUserRank3D`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Click **Right** preset
     - **Pos X**: `-200`
     - **Pos Y**: `0`
     - **Width**: `80`
     - **Height**: `30`
   - **TextMeshProUGUI Component**:
     - **Text**: `#1`
     - **Font Size**: `20`
     - **Alignment**: Right

4. **Right-click on `CurrentUserPanel`** → **UI → Text - TextMeshPro**
5. **Name it**: `CurrentUserScore3D`
6. **Select `CurrentUserScore3D`**, in Inspector:
   - **RectTransform**:
     - **Anchor Presets**: Click **Right** preset
     - **Pos X**: `-120`
     - **Pos Y**: `0`
     - **Width**: `150`
     - **Height**: `30`
   - **TextMeshProUGUI Component**:
     - **Text**: `Score: 0`
     - **Font Size**: `20`
     - **Alignment**: Right

### Step 2.8: Add Loading and Error Indicators

#### Step 2.8a: Loading Indicator

1. **Right-click on `BackgroundPanel`** → **UI → Text - TextMeshPro**
2. **Name it**: `LoadingIndicator`
3. **Select `LoadingIndicator`**, in Inspector:
   - **RectTransform**: Center it
   - **TextMeshProUGUI Component**:
     - **Text**: `Loading...`
     - **Font Size**: `24`
     - **Alignment**: Center

#### Step 2.8b: Error Text

1. **Right-click on `BackgroundPanel`** → **UI → Text - TextMeshPro**
2. **Name it**: `ErrorText`
3. **Select `ErrorText`**, in Inspector:
   - **RectTransform**: Center it
   - **TextMeshProUGUI Component**:
     - **Text**: `Failed to load leaderboard`
     - **Font Size**: `20`
     - **Color**: Red
   - **GameObject**: **Uncheck** ✅ **Active** (hide initially)

#### Step 2.8c: Empty Text

1. **Right-click on `BackgroundPanel`** → **UI → Text - TextMeshPro**
2. **Name it**: `EmptyText`
3. **Select `EmptyText`**, in Inspector:
   - **RectTransform**: Center it
   - **TextMeshProUGUI Component**:
     - **Text**: `No players yet`
     - **Font Size**: `20`
   - **GameObject**: **Uncheck** ✅ **Active** (hide initially)

### Step 2.9: Add LeaderboardPopup Component

1. **Select `LeaderboardPopup`** GameObject (the root)
2. **In Inspector**, click **Add Component**
3. **Search for**: `LeaderboardPopup`
4. **Click** to add the component

### Step 2.10: Assign ALL References in LeaderboardPopup Component

**Select `LeaderboardPopup`** GameObject, in Inspector find **LeaderboardPopup** component:

#### Tab System:
1. **Tab 2D Button**: Drag `Tab2DButton` from Hierarchy
2. **Tab 3D Button**: Drag `Tab3DButton` from Hierarchy
3. **Tab 2D Content**: Drag `Tab2DContent` GameObject
4. **Tab 3D Content**: Drag `Tab3DContent` GameObject
5. **Tab 2D Indicator**: Drag `Tab2DIndicator` Image component
6. **Tab 3D Indicator**: Drag `Tab3DIndicator` Image component

#### Scroll Views (CRITICAL):
1. **Scroll View 2D**: Drag `Tab2DContent` GameObject (the one with ScrollRect component)
2. **Scroll View 3D**: Drag `Tab3DContent` GameObject (the one with ScrollRect component)
3. **Content 2D**: Drag `Content2D` Transform (the child inside Tab2DContent)
4. **Content 3D**: Drag `Content3D` Transform (the child inside Tab3DContent)

#### Leaderboard Entry:
1. **Leaderboard Entry Prefab**: Drag `LeaderboardEntry` prefab from `Assets/Prefabs/` folder

#### Current User Display:
1. **Current User Panel**: Drag `CurrentUserPanel` GameObject
2. **Current User Avatar**: Drag `CurrentUserAvatar` Image
3. **Current User Username**: Drag `CurrentUserUsername` TextMeshProUGUI
4. **Current User Rank 2D**: Drag `CurrentUserRank2D` TextMeshProUGUI
5. **Current User Rank 3D**: Drag `CurrentUserRank3D` TextMeshProUGUI
6. **Current User Score 2D**: Drag `CurrentUserScore2D` TextMeshProUGUI
7. **Current User Score 3D**: Drag `CurrentUserScore3D` TextMeshProUGUI

#### Close Button:
1. **Close Button**: Drag `CloseButton` Button component

#### Loading & Error:
1. **Loading Indicator**: Drag `LoadingIndicator` GameObject
2. **Error Text**: Drag `ErrorText` TextMeshProUGUI
3. **Empty Text**: Drag `EmptyText` TextMeshProUGUI

#### Avatar Sprites:
1. **Avatar Sprites**: Click **+** to add 6 elements, assign your avatar sprites
2. **Default Avatar Sprite**: Assign default sprite

#### Settings:
1. **Max Leaderboard Entries**: `100`
2. **Active Tab Color**: Yellow (R:255, G:204, B:0, A:255)
3. **Inactive Tab Color**: Gray (R:180, G:180, B:180, A:255)

### Step 2.11: Create the Prefab

1. **Drag `LeaderboardPopup`** from Hierarchy to `Assets/Prefabs/` folder
2. **Delete** `LeaderboardPopup` from Hierarchy

---

## PART 3: Add Leaderboard Button to MainScene

### Step 3.1: Open MainScene 1

1. **File → Open Scene** → `Scenes/MainScene 1.unity`

### Step 3.2: Find Button Container

1. **Look in Hierarchy** for your main menu buttons (usually in Canvas or panel_content)
2. **Note the location** where you want the Leaderboard button

### Step 3.3: Create Leaderboard Button

1. **Right-click on your button container** → **UI → Button - TextMeshPro**
2. **Name it**: `LeaderboardButton`
3. **Select `LeaderboardButton`**, in Inspector:
   - **RectTransform**: Position it where you want (e.g., next to Profile button)
   - **Button Component**: Style it to match your other buttons
   - **TextMeshProUGUI** (child):
     - **Text**: `Leaderboard`
     - **Font Size**: Match your other buttons

### Step 3.4: Connect Button to MainScene Script

1. **Find the GameObject with `MainScene` script** (usually named `MainScene` or similar)
2. **Select `LeaderboardButton`**
3. **In Inspector**, find **Button** component
4. **Under OnClick()**, click **+** to add event
5. **Drag the GameObject with MainScene script** to the object field
6. **Click dropdown** → **MainScene → OnClickLeaderboard()**

### Step 3.5: Assign LeaderboardPopup Prefab to MainScene

1. **Select the GameObject with `MainScene` script**
2. **In Inspector**, find **MainScene** component
3. **Under Popup Prefabs**:
   - **Leaderboard Popup Prefab**: Drag `LeaderboardPopup` prefab from `Assets/Prefabs/` folder

---

## PART 4: Verify Scrolling Setup

### Critical Scrolling Components Checklist

For **EACH** scroll view (`Tab2DContent` and `Tab3DContent`):

✅ **ScrollRect Component**:
- ✅ Vertical: **Checked**
- ✅ Horizontal: **Unchecked**
- ✅ Content: Assigned to Content2D/Content3D
- ✅ Vertical Scrollbar: Assigned

✅ **Content GameObject** (Content2D/Content3D):
- ✅ **Vertical Layout Group** component added
- ✅ **Content Size Fitter** component added
  - Vertical Fit: **Preferred Size**
- ✅ RectTransform Height: Set to initial value (will expand automatically)

✅ **Scrollbar**:
- ✅ Created and assigned to ScrollRect
- ✅ Positioned on right side

### How Scrolling Works

1. **Vertical Layout Group** arranges entries vertically with spacing
2. **Content Size Fitter** expands Content height based on number of entries
3. **ScrollRect** detects when Content height > Viewport height
4. **Scrollbar** appears automatically when content is scrollable
5. **User can drag** or use scrollbar to scroll through entries

### Test Scrolling

To test if scrolling works:
1. **Temporarily add many LeaderboardEntry instances** to Content2D
2. **Make sure Content2D height** is larger than Tab2DContent viewport
3. **Play the scene** and check if you can scroll

---

## PART 5: Final Verification

### Checklist Before Testing

- [ ] LeaderboardEntry prefab created in Prefabs folder
- [ ] LeaderboardPopup prefab created in Prefabs folder
- [ ] All references assigned in LeaderboardPopup component
- [ ] Leaderboard button created in MainScene
- [ ] Button connected to OnClickLeaderboard()
- [ ] LeaderboardPopup prefab assigned to MainScene
- [ ] ScrollRect components have Content assigned
- [ ] Content GameObjects have Vertical Layout Group
- [ ] Content GameObjects have Content Size Fitter
- [ ] Scrollbars are created and assigned

### Test in Play Mode

1. **Press Play**
2. **Click Leaderboard button**
3. **Verify**:
   - ✅ Popup appears
   - ✅ Loading indicator shows
   - ✅ 2D tab is active (yellow indicator)
   - ✅ Leaderboard entries appear
   - ✅ Scrolling works (if many entries)
   - ✅ Click 3D tab → switches to 3D
   - ✅ Current user profile shows at bottom
   - ✅ Close button closes popup

---

## Troubleshooting Scrolling Issues

### Problem: Scrolling doesn't work

**Solution 1**: Check Content Size Fitter
- Content2D/Content3D must have **Content Size Fitter**
- **Vertical Fit** must be set to **Preferred Size**

**Solution 2**: Check Vertical Layout Group
- Content2D/Content3D must have **Vertical Layout Group**
- **Child Force Expand Width** should be checked
- **Child Force Expand Height** should be unchecked

**Solution 3**: Check Content Height
- Content height must be **larger than ScrollRect viewport**
- If Content height is too small, scrolling won't activate

**Solution 4**: Check ScrollRect Settings
- **Vertical** must be checked
- **Horizontal** must be unchecked
- **Content** must be assigned

### Problem: Entries overlap

**Solution**: Check Vertical Layout Group spacing
- Set **Spacing** to `5` or higher
- Ensure **Child Force Expand Height** is unchecked

### Problem: Scrollbar doesn't appear

**Solution**: 
- Verify scrollbar GameObject exists
- Check ScrollRect has scrollbar assigned
- Ensure Content height > Viewport height

---

## Quick Reference: Hierarchy Structure

```
LeaderboardPopup
└── BackgroundPanel
    ├── TitleText
    ├── CloseButton
    ├── TabContainer
    │   ├── Tab2DButton
    │   │   └── Tab2DIndicator
    │   └── Tab3DButton
    │       └── Tab3DIndicator
    ├── Tab2DContent (ScrollRect)
    │   ├── Content2D (Vertical Layout Group + Content Size Fitter)
    │   └── ScrollbarVertical2D
    ├── Tab3DContent (ScrollRect) [Initially Inactive]
    │   ├── Content3D (Vertical Layout Group + Content Size Fitter)
    │   └── ScrollbarVertical3D
    ├── CurrentUserPanel
    │   ├── CurrentUserAvatar
    │   ├── CurrentUserUsername
    │   ├── CurrentUserRank2D
    │   ├── CurrentUserScore2D
    │   ├── CurrentUserRank3D
    │   └── CurrentUserScore3D
    ├── LoadingIndicator
    ├── ErrorText [Initially Inactive]
    └── EmptyText [Initially Inactive]
```

---

## Summary

You've now created:
1. ✅ **LeaderboardEntry** prefab - Individual entry component
2. ✅ **LeaderboardPopup** prefab - Main popup with tabs and scrolling
3. ✅ **Leaderboard button** - Added to MainScene
4. ✅ **All connections** - Button → MainScene → Prefab

The scrolling system is fully set up with:
- ✅ ScrollRect components
- ✅ Vertical Layout Groups
- ✅ Content Size Fitters
- ✅ Scrollbars

**Next**: Test in Play Mode and verify everything works!

