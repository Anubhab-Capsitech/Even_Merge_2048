# TextMeshPro Input Field Setup Guide

This guide explains how to properly set up a TextMeshPro Input Field for the username input in ProfileSetupPopup.

## Structure Overview

A TextMeshPro Input Field (`TMP_InputField`) creates **3 child GameObjects** automatically when you create it:

```
UsernameInput (TMP_InputField)
├── Text Area (RectTransform with RectMask2D)
│   └── Text (TextMeshProUGUI) - The actual text that appears
├── Placeholder (TextMeshProUGUI) - The placeholder text
└── Text (TextMeshProUGUI) - Legacy text component (can be deleted)
```

## Step-by-Step Setup

### 1. Create the Input Field

1. Right-click on your parent GameObject (e.g., `PopupContent`)
2. Select: **UI → Input Field - TextMeshPro**
3. Name it: `UsernameInput`

### 2. Configure the Root GameObject (UsernameInput)

**RectTransform:**
- Set position and size as needed
- Recommended: Width: 400-500, Height: 50-60
- Anchor: Center-Middle

**TMP_InputField Component Settings:**

**Basic Settings:**
- **Text Component**: Drag the `Text Area/Text` child GameObject here
- **Placeholder**: Drag the `Placeholder` child GameObject here
- **Text**: Leave empty (this is the initial value)
- **Character Limit**: 0 (unlimited) or set a max (e.g., 20)

**Content Type:**
- Select: **Standard** (for username) or **Alphanumeric** (if you want only letters/numbers)

**Input Type:**
- **Standard**: Normal keyboard
- **Auto Correct**: Off
- **Rich Text**: Off (unless you want formatting)

**Line Type:**
- **Single Line**: For username (recommended)

**Keyboard Type:**
- **Default** or **ASCIICapable**

**Character Validation:**
- **None** (or use a custom validator if needed)

**Other Settings:**
- **Caret Blink Rate**: 0.85 (default)
- **Caret Width**: 1
- **Selection Color**: White with some transparency (e.g., R:1, G:1, B:1, A:0.5)
- **Text Color**: Black or your theme color
- **Font Asset**: Assign your TextMeshPro font asset
- **Font Size**: 36-48 (adjust based on your design)

### 3. Configure Text Area Child

**RectTransform:**
- Set to **Stretch-Stretch** (fill the parent)
- Left: 10, Right: 10, Top: 5, Bottom: 5 (padding)

**RectMask2D Component:**
- This is automatically added - **DO NOT REMOVE**
- It clips text that goes outside the input field

### 4. Configure Text Area/Text Child

**RectTransform:**
- Set to **Stretch-Stretch** (fill Text Area)
- Left: 0, Right: 0, Top: 0, Bottom: 0

**TextMeshProUGUI Component:**
- **Text**: Leave empty (populated by user input)
- **Font Asset**: Same as Input Field
- **Font Size**: Same as Input Field (or slightly smaller)
- **Color**: Black or your text color
- **Alignment**: Left (for left-to-right languages)
- **Overflow**: Truncate or Overflow
- **Raycast Target**: **UNCHECK THIS** (prevents blocking clicks)

### 5. Configure Placeholder Child

**RectTransform:**
- Set to **Stretch-Stretch** (fill Text Area)
- Left: 0, Right: 0, Top: 0, Bottom: 0

**TextMeshProUGUI Component:**
- **Text**: "Please state your name....." (or your placeholder text)
- **Font Asset**: Same as Input Field
- **Font Size**: Same as Input Field
- **Color**: Gray (e.g., R:0.5, G:0.5, B:0.5, A:0.5) - semi-transparent
- **Alignment**: Left
- **Font Style**: Italic (optional, for visual distinction)
- **Raycast Target**: **UNCHECK THIS**

### 6. Visual Styling (Optional)

**Add Background Image:**
- Add an `Image` component to the root `UsernameInput` GameObject
- Set a background color or sprite
- Set **Raycast Target**: Checked (needed for input field to work)

**Add Border:**
- Create a child GameObject with an `Image` component
- Use a border sprite or 9-sliced sprite
- Position it behind the input field

## Common Issues and Solutions

### Issue 1: Text not visible
**Solution:**
- Check that `Text Area/Text` is assigned in the Input Field's **Text Component** field
- Ensure TextMeshProUGUI has a valid Font Asset assigned
- Check text color is not transparent

### Issue 2: Placeholder not showing
**Solution:**
- Check that `Placeholder` is assigned in the Input Field's **Placeholder** field
- Ensure Placeholder TextMeshProUGUI has text set
- Check placeholder color (should be visible but different from input text)

### Issue 3: Input field not clickable
**Solution:**
- Ensure the root GameObject or a child has an `Image` component with **Raycast Target** checked
- Check that nothing is blocking the input field (higher in hierarchy or with higher Canvas Sort Order)

### Issue 4: Text goes outside bounds
**Solution:**
- Ensure `Text Area` has `RectMask2D` component (should be automatic)
- Check that `Text Area/Text` RectTransform is set to Stretch-Stretch within Text Area

### Issue 5: Cursor/caret not visible
**Solution:**
- Check **Caret Color** in TMP_InputField (should be dark/visible)
- Increase **Caret Width** if too thin
- Ensure **Caret Blink Rate** is > 0

## Inspector Reference Checklist

When assigning in `ProfileSetupPopup` component:

✅ **Username Input**: Drag the root `UsernameInput` GameObject (the one with TMP_InputField component)

The script will automatically access:
- `usernameInput.text` - Gets/sets the input text
- `usernameInput.onValueChanged` - Event for text changes

## Example Inspector Values

**TMP_InputField:**
```
Text Component: Text Area/Text (TextMeshProUGUI)
Placeholder: Placeholder (TextMeshProUGUI)
Text: (empty)
Character Limit: 0
Content Type: Standard
Line Type: Single Line
Font Size: 42
Text Color: Black (R:0, G:0, B:0, A:1)
Caret Color: Dark Gray (R:0.2, G:0.2, B:0.2, A:1)
Selection Color: Light Blue (R:0.5, G:0.8, B:1, A:0.5)
```

**Text Area/Text (TextMeshProUGUI):**
```
Text: (empty - auto-populated)
Font Size: 42
Color: Black
Alignment: Left
Raycast Target: ❌ Unchecked
```

**Placeholder (TextMeshProUGUI):**
```
Text: "Please state your name....."
Font Size: 42
Color: Gray (R:0.5, G:0.5, B:0.5, A:0.5)
Alignment: Left
Font Style: Italic
Raycast Target: ❌ Unchecked
```

## Testing

1. Play the scene
2. Click on the input field
3. Keyboard should appear (in builds) or you can type (in editor)
4. Placeholder should disappear when you start typing
5. Text should be visible and clipped if too long
6. Cursor should blink

