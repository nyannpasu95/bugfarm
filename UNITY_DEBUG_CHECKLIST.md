# Unity Component & Reference Debugging Checklist

## Why Seeds and Harvesting Might Not Work

Even though the code is correct, Unity requires proper **component references** and **setup in the Inspector**. Here's how to check everything step by step.

---

## PART 1: Check Player & Farming Setup

### Step 1: Find the Player GameObject
1. Open the **Farm** scene (Assets/Scenes/Farm.unity)
2. In the **Hierarchy** panel (left side), find the "Player" GameObject
3. Click on it to select it

### Step 2: Check PlayerFarming Component
1. Look at the **Inspector** panel (right side)
2. Find the **PlayerFarming** component
3. **CHECK THESE FIELDS** - They should NOT be "None":
   - ✅ Main Camera → Should reference "Main Camera"
   - ✅ Grid → Should reference the Grid GameObject in scene
   - ✅ Cursor Manager → Should reference CursorManager
   - ✅ Inventory Manager → Should reference InventoryManager
   - ✅ Grid Property Manager → Should reference GridPropertiesManager

**If any are "None (Missing)"**:
- Drag the correct GameObject from the Hierarchy into that field
- For Grid: Find "Grid" GameObject in Hierarchy
- For Managers: Find GameObjects with those components

---

## PART 2: Check Grid Properties Manager

### Step 3: Find GridPropertiesManager GameObject
1. In **Hierarchy**, search for "GridPropertiesManager" or find it under scene root
2. Click to select it

### Step 4: Check Inspector Settings
Look for these components/settings:

**GridPropertiesManager Component:**
- ✅ **SO Crop Details List** → Should have a ScriptableObject assigned
  - If "None": Go to Project → Assets/ScriptObject → Find "CropDetailsList"
  - Drag it into this field

- ✅ **Grid Properties List** → Check if any Grid Properties are assigned
  - If empty: Go to Project → Assets/ScriptObject/Map
  - Drag your GridProperties_SO files here

- ✅ **Dug Ground Tilemap** → Should reference a Tilemap
  - Find "DugGround" Tilemap in Hierarchy under Grid
  - Drag it into this field

- ✅ **Watered Ground Tilemap** → Should reference a Tilemap
  - Find "WateredGround" Tilemap in Hierarchy
  - Drag it into this field

### Step 5: Check Grid Range Settings
In GridPropertiesManager component:
- **Start Grid X**: Should be a negative number (e.g., -50)
- **Start Grid Y**: Should be a negative number (e.g., -50)
- **Grid Width**: Should be a large number (e.g., 100)
- **Grid Height**: Should be a large number (e.g., 100)

**This defines where farming is allowed!**

---

## PART 3: Check Crop System

### Step 6: Verify Crop ScriptableObject
1. Go to **Project** panel → Assets/ScriptObject/Crop
2. Find your **SO_CropDetailsList** file
3. Double-click to open it in Inspector
4. **Check if crops are defined:**
   - Should have a list of crops (wheat, tomato, etc.)
   - Each crop should have:
     - ✅ Seed Item Code (matches seed ID in ItemDataList)
     - ✅ Growth Days array (not empty)
     - ✅ Growth Prefab array (has prefabs for each stage)
     - ✅ Harvest Tool Item Code (array with tool IDs)
     - ✅ Required Harvest Actions (array with action counts)
     - ✅ Crop Produced Item Code (what you get when harvesting)

**Example for Wheat:**
```
Seed Item Code: 1001
Growth Days: [0, 1, 2, 3]
Growth Prefab: [wheat_stage0, wheat_stage1, wheat_stage2, wheat_stage3]
Harvest Tool Item Code: [0] (0 = hand, no tool needed)
Required Harvest Actions: [1] (1 click to harvest)
Crop Produced Item Code: [1002] (wheat item)
Crop Produced Min Quantity: [1]
Crop Produced Max Quantity: [3]
```

---

## PART 4: Check Inventory & Item System

### Step 7: Check InventoryManager
1. Find "InventoryManager" in Hierarchy
2. Check Inspector:
   - ✅ **Item Data List_SO** → Should have a ScriptableObject
   - ✅ **Player Bag_SO** → Should have a ScriptableObject

### Step 8: Verify Item Data
1. Go to **Project** → Assets/ScriptObject/Inventory
2. Open **ItemDataList_SO**
3. **Check if your items exist:**
   - Seeds (ID 1001, 1002, etc.)
   - Tools (Hoe ID 6001, Watering Can ID 5008)
   - Crops (Wheat ID 1002, etc.)

**For each item, verify:**
- ✅ Item ID (unique number)
- ✅ Item Type (Seed, Tool, Crop, etc.)
- ✅ Item Icon (has a sprite)
- ✅ Item On World Sprite (has a sprite)
- ✅ Can Pickedup = TRUE (if you want to pick it up)

### Step 9: Check If You Have Seeds in Inventory
1. Play the game
2. Press **B** key to open inventory
3. **Do you have seeds?**
   - If NO: You need to add starting items or buy from shop
   - If YES: Click on the seed to select it

---

## PART 5: Check Cursor System

### Step 10: Find GridCursor
1. In Hierarchy, find "Canvas" → "GridCursor" (or similar)
2. Check Inspector:
   - ✅ **Cursor Image** → Should reference an Image component
   - ✅ **Cursor Rect Transform** → Should reference its RectTransform
   - ✅ **Green Cursor Sprite** → Should have a green cursor sprite
   - ✅ **Red Cursor Sprite** → Should have a red cursor sprite

### Step 11: Check CursorManager
1. Find "CursorManager" GameObject in Hierarchy
2. Check that it has the **CursorManager** component
3. It should reference the GridCursor

---

## PART 6: Testing Step-by-Step

### Test 1: Can You Dig Ground?
1. Play the game
2. Press **B** to open inventory
3. **Click on the Hoe (ID 6001)** to select it
4. You should see a **green/red cursor** appear
5. Click on grass ground
6. **Expected**: Ground becomes brown (dug)

**If cursor doesn't appear:**
- GridCursor or CursorManager not set up
- ItemSelectedEvent not firing
- Check Console for errors

**If clicking does nothing:**
- Check Console logs (PlayerFarming has debug logs)
- Grid reference might be missing
- GridPropertyDetails might not exist for that tile

### Test 2: Can You Water Ground?
1. Select the **Watering Can (ID 5008)**
2. Click on DUG ground (brown tiles)
3. **Expected**: Ground becomes darker (watered)

### Test 3: Can You Plant Seeds?
1. Select a **Seed** from inventory
2. Click on WATERED ground
3. **Expected**:
   - Seed disappears from inventory (-1)
   - Small sprout appears on ground

**If seed doesn't plant:**
- Check if grid property `daysSinceDug > -1` (must be dug first)
- Check if `seedItemCode == -1` (no seed already planted)
- Check Console logs from PlayerFarming

### Test 4: Do Crops Grow?
1. After planting, wait for time to pass
2. Each day at midnight (00:00), crops should advance one stage
3. **Watch the time display** - does it advance?

**If crops don't grow:**
- Time system might not be working
- AdvanceGameDayEvent might not be connected
- GridPropertiesManager might not be listening to the event

### Test 5: Can You Harvest?
1. Wait for crop to reach final growth stage
2. Select the correct tool (or hand if no tool needed)
3. Click on the mature crop
4. **Expected**:
   - Crop disappears or resets (if regrowable)
   - Harvested items appear on ground or in inventory

**If harvest doesn't work:**
- Check if crop has **Crop component** attached
- Check if `cropGridPosition` is set correctly
- Check Console for errors
- Verify CropDetails has harvest tool configured

---

## PART 7: Common Issues & Solutions

### Issue 1: "Grid is null" error
**Solution:**
- PlayerFarming component needs Grid reference
- Drag Grid GameObject into the field

### Issue 2: "GridPropertiesManager Instance is null"
**Solution:**
- GridPropertiesManager must inherit from Singleton<GridPropertiesManager>
- Must be in the scene at start

### Issue 3: Cursor doesn't show up
**Solution:**
- Check CursorManager and GridCursor are properly linked
- Check if ItemSelectedEvent is being called (add debug logs)
- Verify GridCursor has all sprite references

### Issue 4: Clicking does nothing
**Solution:**
- Check Console for PlayerFarming debug logs
- If no logs appear: PlayerFarming might not be attached to Player
- If "No grid property" error: Grid range settings are wrong

### Issue 5: Seeds plant but don't grow
**Solution:**
- Time system not advancing days
- GridPropertiesManager not subscribed to AdvanceGameDayEvent
- Check GridPropertiesManager.cs OnEnable() method

### Issue 6: Can't harvest crops
**Solution:**
- Crop GameObject must have **Crop component**
- Crop component must have `cropGridPosition` set
- CropDetails must have `harvestToolItemCode` and `requiredHarvestActions`
- Selected tool must match the harvest tool

---

## PART 8: Debug Console Commands

Add these to GridPropertiesManager for testing:

```csharp
[ContextMenu("Debug: Force Advance Day")]
public void DebugAdvanceDay()
{
    AdvanceDay();
    Debug.Log("Manually advanced day for testing");
}

[ContextMenu("Debug: List All Grid Properties")]
public void DebugListGridProperties()
{
    Debug.Log($"Total grid properties: {gridPropertyDictionary.Count}");
    foreach (var kvp in gridPropertyDictionary)
    {
        var details = kvp.Value;
        Debug.Log($"[{kvp.Key}] Dug:{details.daysSinceDug}, Watered:{details.daysSinceWatered}, Seed:{details.seedItemCode}, Growth:{details.growthDays}");
    }
}
```

Right-click GridPropertiesManager component → "Debug: Force Advance Day" to test

---

## PART 9: Check Console for Errors

### How to Open Console
1. In Unity, go to **Window → General → Console**
2. Or press **Ctrl+Shift+C** (Windows) or **Cmd+Shift+C** (Mac)

### What to Look For
- **Red errors** (compilation errors - game won't work)
- **Yellow warnings** (won't break game but should fix)
- **Blue info** (debug logs from PlayerFarming)

### Common Errors:
```
"NullReferenceException: Object reference not set to an instance"
→ A reference is missing in Inspector

"The object of type X has been destroyed but you are still trying to access it"
→ GameObject was deleted but still being referenced

"MissingReferenceException: The object X has been destroyed"
→ A ScriptableObject or prefab reference is missing
```

---

## PART 10: Quick Test Scene Setup

If nothing works, try this minimal setup:

1. **Create Test Grid**:
   - Create new Scene
   - Add Grid GameObject (right-click Hierarchy → 2D Object → Tilemap → Grid)

2. **Add GridPropertiesManager**:
   - Create empty GameObject, rename to "GridPropertiesManager"
   - Add GridPropertiesManager component
   - Set Start X/Y to -10, Width/Height to 20

3. **Create Simple Crop**:
   - Right-click Project → Create → ScriptableObject → SO_CropDetailsList
   - Add one test crop with simple settings

4. **Test Player**:
   - Add Player GameObject with PlayerFarming component
   - Assign all references
   - Add some seeds to inventory

---

## Summary Checklist

Use this before asking for more help:

- [ ] Player has PlayerFarming component with ALL fields assigned
- [ ] GridPropertiesManager exists in scene with Grid range set
- [ ] GridPropertiesManager has all Tilemap references
- [ ] GridPropertiesManager has CropDetailsList assigned
- [ ] InventoryManager has ItemDataList assigned
- [ ] ItemDataList has seeds, tools, and crops defined
- [ ] Seeds have correct item type and ID
- [ ] CropDetailsList has crops matching seed IDs
- [ ] GridCursor has all sprite references
- [ ] Console shows no red errors
- [ ] You have seeds in inventory (press B to check)
- [ ] Time display advances (not stuck at 06:00)
- [ ] Console shows debug logs when clicking ground

**If all checked and still doesn't work, share:**
1. Screenshot of PlayerFarming component in Inspector
2. Screenshot of GridPropertiesManager component
3. Console error messages
4. What exactly happens when you click (nothing? error? wrong behavior?)

---

Good luck! Let me know which step fails and I can help troubleshoot further.
