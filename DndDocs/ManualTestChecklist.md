# Manual Test Checklist - Multiclass Implementation

## Test 1: Multiclass Level Display
- [ ] Create character with Paladin 8 / Rogue 4
- [ ] Add subclasses: Oath of Devotion / Thief
- [ ] Generate PDF
- [ ] Verify Level field shows "8/4" (not "12")
- [ ] Verify Class shows "Paladin / Rogue"
- [ ] Verify Subclass shows "Oath of Devotion / Thief" (truncated at 30 chars if needed)
- [ ] Verify PROF BONUS calculated from total level 12 (should be +4)

## Test 2: Special Traits Separation
- [ ] In Manual Overrides (tab 7), add special traits: "Darkvision", "Fey Ancestry"
- [ ] Generate PDF
- [ ] Verify SPECIES TRAITS includes species traits first
- [ ] Verify "Special Traits:" header appears after species traits
- [ ] Verify custom traits listed under Special Traits section
- [ ] Verify CLASS FEATURES 1 contains only class features (no traits mixed in)

## Test 3: All Equipment Categories
- [ ] Select items from all categories:
  - Armor (e.g., Plate Armor)
  - Weapons (e.g., Longsword, Longbow)
  - Adventuring Gear (e.g., Rope, Torch)
  - Tools (e.g., Thieves' Tools)
  - Magic Items (e.g., +1 Longsword)
  - Mounts & Vehicles (e.g., Riding Horse)
  - Poisons (e.g., Basic Poison)
- [ ] Generate PDF
- [ ] Verify all items appear in EQUIPMENT section
- [ ] Verify long names truncated at 24 chars with ellipsis

## Test 4: Selector UX (Sticky Actions)
- [ ] Navigate to Feats step (tab 4)
- [ ] Add a feat, verify page/search preserved
- [ ] Scroll selected feats list, verify footer buttons visible
- [ ] Repeat for Spells step (tab 5)
- [ ] Repeat for Equipment step (tab 6)

## Test 5: Single Class Edge Case
- [ ] Create single-class character (e.g., Wizard 12)
- [ ] Generate PDF
- [ ] Verify Level shows "12" (not "12/" or split)
- [ ] Verify Subclass shows single name (e.g., "School of Evocation")

## Test 6: Truncation Tooltips
- [ ] Create character with very long subclass name (if exists in data)
- [ ] View character in CharacterSheetDisplay
- [ ] Hover over truncated names, verify full name in tooltip

## Test 7: Empty Special Traits
- [ ] Create character without adding special traits
- [ ] Generate PDF
- [ ] Verify no "Special Traits:" header appears in SPECIES TRAITS
