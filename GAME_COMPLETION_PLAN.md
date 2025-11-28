# Unity Farming Game - Completion Plan

## Project Status: 40-50% Complete (Basic), 25-30% Complete (Full-Featured)

---

## PHASE 1: CRITICAL BUG FIXES (Must Do First)

### 1.1 Fix Time System Bug [CRITICAL]
**File:** `Assets/Scripts/time/TimeSetting.cs:65`
**Problem:** `CallAdvanceGameDayEvent()` fires every frame instead of once per day
**Impact:** Crops instantly mature, grid resets constantly
**Fix:**
```csharp
// Remove line 65: CallAdvanceGameDayEvent();
// Add proper day transition logic with boolean flag to prevent multiple calls
```

### 1.2 Fix Crop Harvesting [CRITICAL]
**File:** `Assets/Scripts/Crop/Crop.cs:29-62`
**Problem:** `ProcessToolAction` doesn't check if enough actions done to trigger harvest
**Impact:** Players cannot harvest crops (game-breaking)
**Fix:**
```csharp
// In ProcessToolAction, after line 61, add:
if (harvestActionCount >= cropDetails.RequiredHarvestActions)
{
    HarvestCrop(isPlayerPosition, actionTool);
}
```

### 1.3 Fix Empty ScriptableObject Methods
**File:** `Assets/Scripts/Inventory/InventoryBag_SO.cs:9-18`
**Problem:** ScriptableObject has Start/Update (never execute)
**Fix:** Remove these dead code methods

### 1.4 Remove Emergency Fix Script
**File:** `Assets/Scripts/FixTemp/FarmingEmergencyFix.cs`
**Problem:** Band-aid solution indicates fragile core systems
**Fix:** Resolve underlying issues, remove this script

---

## PHASE 2: COMPLETE CORE GAME LOOPS (Essential Gameplay)

### 2.1 Complete Crop Harvesting System
**Priority:** HIGH
**Files to modify:**
- `Assets/Scripts/Crop/Crop.cs` - Connect harvest logic
- `Assets/Scripts/Player/PlayerFarming.cs` - Tool usage validation
- `Assets/Scripts/UI/Cursor/CursorManager.cs` - Tool-specific cursors

**Tasks:**
- [ ] Fix harvest trigger logic
- [ ] Add harvest animation integration
- [ ] Implement multi-tool harvest support (axe, sickle, etc.)
- [ ] Add harvest particle effects
- [ ] Play sound effects on harvest
- [ ] Update grid properties after harvest

### 2.2 Complete Tool Selection System
**Priority:** HIGH
**Files to modify:**
- `Assets/Scripts/UI/SlotUI.cs` - Add tool selection event
- `Assets/Scripts/Player/PlayerFarming.cs` - Listen for tool changes
- `Assets/Scripts/UI/Cursor/CursorManager.cs` - Update cursor based on tool

**Tasks:**
- [ ] Connect slot click to tool selection
- [ ] Update cursor when tool changes
- [ ] Show tool range indicator
- [ ] Disable farming when no tool selected
- [ ] Add visual feedback for active tool

### 2.3 Implement Save/Load System
**Priority:** HIGH
**Files to modify:**
- `Assets/Scripts/Inventory/InventoryManager.cs` - Implement ISaveable
- `Assets/Scripts/Map/GridPropertiesManager.cs` - Implement ISaveable
- `Assets/Scripts/Crop/Crop.cs` - Implement ISaveable
- `Assets/Scripts/Player/PlayerMoney.cs` - Implement ISaveable
- `Assets/Scripts/time/TimeSetting.cs` - Implement ISaveable

**Tasks:**
- [ ] Save inventory items and slots
- [ ] Save grid properties (dug, watered, crops)
- [ ] Save crop growth stages
- [ ] Save player money and position
- [ ] Save current day/time
- [ ] Save task/building progress
- [ ] Add auto-save on scene transition
- [ ] Add manual save button
- [ ] Add multiple save slots
- [ ] Add save file corruption handling

### 2.4 Complete Event System
**Priority:** MEDIUM
**Files to modify:**
- `Assets/Scripts/EventCenter/EventHandler.cs` - Document events
- Various systems - Connect unused events

**Tasks:**
- [ ] Connect HarvestActionEffect event to crop actions
- [ ] Connect MovementEvent to player movement
- [ ] Add sound effect events
- [ ] Add particle effect events
- [ ] Document event flow in comments

---

## PHASE 3: CORE FARMING FEATURES (Standard Expectations)

### 3.1 Tool Upgrade System
**Priority:** HIGH
**New files to create:**
- `Assets/Scripts/Tools/ToolController.cs`
- `Assets/Scripts/Tools/ToolDetails.cs`
- `Assets/Scripts/Tools/ToolUpgradeUI.cs`

**Tasks:**
- [ ] Create tool upgrade tiers (Copper → Iron → Gold → Iridium)
- [ ] Add tool efficiency bonuses (fewer actions needed)
- [ ] Add tool range bonuses (larger area)
- [ ] Create upgrade cost system
- [ ] Add upgrade shop/blacksmith building
- [ ] Add tool durability (optional)
- [ ] Add visual differences for tool tiers

### 3.2 Stamina/Energy System
**Priority:** MEDIUM
**New files to create:**
- `Assets/Scripts/Player/PlayerStamina.cs`
- `Assets/Scripts/UI/StaminaBarUI.cs`

**Tasks:**
- [ ] Create stamina stat (0-100)
- [ ] Deduct stamina for actions (farming, watering, harvesting)
- [ ] Tool tier affects stamina cost
- [ ] Regenerate stamina over time
- [ ] Sleep to fully restore stamina
- [ ] Stamina affects movement speed when low
- [ ] Add stamina UI bar
- [ ] Eating food restores stamina
- [ ] Prevent actions when stamina depleted

### 3.3 Fishing System
**Priority:** MEDIUM
**New files to create:**
- `Assets/Scripts/Fishing/FishingController.cs`
- `Assets/Scripts/Fishing/FishDetails.cs`
- `Assets/Scripts/Fishing/FishingMinigame.cs`
- `Assets/Scripts/Fishing/SO_FishDetailsList.cs`

**Tasks:**
- [ ] Create fishing rod tool
- [ ] Add fishable water tiles to grid properties
- [ ] Implement cast/wait/bite detection
- [ ] Create minigame (timing bar or keep-fish-in-zone)
- [ ] Add fish rarity system
- [ ] Different fish per season/time/weather
- [ ] Treasure chests (rare drops)
- [ ] Fishing skill progression
- [ ] Fish tank/aquarium display

### 3.4 Cooking System
**Priority:** MEDIUM
**New files to create:**
- `Assets/Scripts/Cooking/CookingController.cs`
- `Assets/Scripts/Cooking/RecipeDetails.cs`
- `Assets/Scripts/Cooking/SO_RecipeList.cs`
- `Assets/Scripts/UI/CookingUI.cs`

**Tasks:**
- [ ] Create recipe ScriptableObjects
- [ ] Ingredient requirements
- [ ] Cooking minigame or auto-cook
- [ ] Cooked food provides buffs (speed, stamina, money bonus)
- [ ] Recipe discovery system
- [ ] Cooking skill progression
- [ ] Kitchen building requirement
- [ ] Fridge storage for ingredients

### 3.5 Season System
**Priority:** MEDIUM
**Files to modify:**
- `Assets/Scripts/time/TimeSetting.cs` - Add season tracking
- `Assets/Scripts/Crop/CropDetails.cs` - Add seasonal crops
- `Assets/Scripts/Map/GridPropertiesManager.cs` - Season visuals

**Tasks:**
- [ ] 4 seasons (Spring, Summer, Fall, Winter)
- [ ] 28 days per season
- [ ] Crops die if not in-season (except greenhouse)
- [ ] Different forageables per season
- [ ] Visual changes (snow, autumn leaves, etc.)
- [ ] Season-specific festivals/events
- [ ] Seasonal music tracks
- [ ] Season transition cutscene

### 3.6 Weather System
**Priority:** LOW
**New files to create:**
- `Assets/Scripts/Weather/WeatherController.cs`
- `Assets/Scripts/Weather/WeatherDetails.cs`

**Tasks:**
- [ ] Weather types (Sunny, Rain, Storm, Snow)
- [ ] Random weather generation
- [ ] Rain auto-waters crops
- [ ] Storm damages crops (chance)
- [ ] Weather affects fishing
- [ ] Weather forecast system
- [ ] Visual effects (rain particles, clouds)
- [ ] Lightning strikes during storms

---

## PHASE 4: SOCIAL & PROGRESSION (Engagement Features)

### 4.1 NPC System
**Priority:** HIGH
**New files to create:**
- `Assets/Scripts/NPC/NPCController.cs`
- `Assets/Scripts/NPC/NPCDetails.cs`
- `Assets/Scripts/NPC/NPCSchedule.cs`
- `Assets/Scripts/NPC/SO_NPCList.cs`

**Tasks:**
- [ ] Create 10-15 NPC characters
- [ ] Daily schedules (move between locations)
- [ ] Idle animations and behaviors
- [ ] Collision detection for conversation
- [ ] Unique personalities and dialogue
- [ ] Gift preferences (likes/dislikes)
- [ ] Birthday system

### 4.2 Dialogue System
**Priority:** HIGH
**New files to create:**
- `Assets/Scripts/Dialogue/DialogueController.cs`
- `Assets/Scripts/Dialogue/DialogueNode.cs`
- `Assets/Scripts/UI/DialogueUI.cs`

**Tasks:**
- [ ] Text box UI with character portraits
- [ ] Multiple dialogue trees per NPC
- [ ] Branching conversations (choices)
- [ ] Dialogue changes based on friendship level
- [ ] Seasonal and event-specific dialogue
- [ ] Localization support
- [ ] Text scrolling animation
- [ ] Sound effects for dialogue

### 4.3 Relationship/Friendship System
**Priority:** MEDIUM
**New files to create:**
- `Assets/Scripts/Social/RelationshipManager.cs`
- `Assets/Scripts/Social/FriendshipDetails.cs`

**Tasks:**
- [ ] Friendship points (0-10 hearts)
- [ ] Gain points from gifts and talking
- [ ] Lose points from ignoring or bad gifts
- [ ] Heart events at milestones (2, 4, 6, 8, 10 hearts)
- [ ] Romance and marriage system (optional)
- [ ] Friendship UI panel
- [ ] Gift giving animation
- [ ] Thank you dialogue after gifts

### 4.4 Quest System
**Priority:** MEDIUM
**New files to create:**
- `Assets/Scripts/Quest/QuestController.cs`
- `Assets/Scripts/Quest/QuestDetails.cs`
- `Assets/Scripts/UI/QuestLogUI.cs`

**Tasks:**
- [ ] Quest types (delivery, collection, combat, exploration)
- [ ] Quest board in town
- [ ] NPC-given quests
- [ ] Timed quests
- [ ] Quest chains
- [ ] Quest rewards (money, items, friendship)
- [ ] Quest tracking UI
- [ ] Quest completion notifications

### 4.5 Achievement System
**Priority:** LOW
**New files to create:**
- `Assets/Scripts/Achievements/AchievementManager.cs`
- `Assets/Scripts/Achievements/AchievementDetails.cs`
- `Assets/Scripts/UI/AchievementUI.cs`

**Tasks:**
- [ ] 50+ achievements
- [ ] Categories (farming, fishing, social, exploration, combat)
- [ ] Hidden achievements
- [ ] Achievement rewards (cosmetics, titles)
- [ ] Achievement notification popup
- [ ] Achievement menu/viewer
- [ ] Steam/platform integration (if applicable)

---

## PHASE 5: ADVANCED FARMING & CONTENT

### 5.1 Greenhouse
**Priority:** MEDIUM
**New files to create:**
- `Assets/Scripts/Buildings/GreenhouseController.cs`

**Tasks:**
- [ ] Unlock via quest or money
- [ ] Season-independent growing
- [ ] Limited space
- [ ] Auto-watering upgrade (sprinklers)
- [ ] Visual interior scene
- [ ] More expensive but profitable

### 5.2 Artisan Goods (Kegs, Preservers, etc.)
**Priority:** MEDIUM
**New files to create:**
- `Assets/Scripts/Artisan/ArtisanMachine.cs`
- `Assets/Scripts/Artisan/ArtisanRecipe.cs`
- `Assets/Scripts/UI/ArtisanUI.cs`

**Tasks:**
- [ ] Keg (wheat → beer, grapes → wine)
- [ ] Preserves Jar (fruits → jam, vegetables → pickles)
- [ ] Cheese Press (milk → cheese)
- [ ] Mayonnaise Machine (eggs → mayo)
- [ ] Oil Maker (sunflower → oil)
- [ ] Processing time (real-time hours)
- [ ] Quality affects output value
- [ ] Crafting recipes to build machines

### 5.3 Livestock System
**Priority:** MEDIUM
**New files to create:**
- `Assets/Scripts/Animals/AnimalController.cs` (expand from AnimalWander)
- `Assets/Scripts/Animals/AnimalDetails.cs`
- `Assets/Scripts/Buildings/BarnController.cs`
- `Assets/Scripts/Buildings/CoopController.cs`

**Tasks:**
- [ ] Purchase animals from ranch
- [ ] Build barns and coops
- [ ] Feed animals daily (hay)
- [ ] Pet animals for friendship
- [ ] Collect products (milk, eggs, wool)
- [ ] Animal happiness affects product quality
- [ ] Breeding system
- [ ] Animal shop UI
- [ ] Barn/coop upgrade tiers (capacity)

### 5.4 Mining System
**Priority:** LOW
**New files to create:**
- `Assets/Scripts/Mining/MineController.cs`
- `Assets/Scripts/Mining/OreNode.cs`
- `Assets/Scripts/Mining/MineFloor.cs`

**Tasks:**
- [ ] Mine entrance building
- [ ] Procedurally generated floors
- [ ] Rock/ore nodes
- [ ] Pickaxe tool
- [ ] Ore types (copper, iron, gold, gems)
- [ ] Combat encounters (monsters)
- [ ] Elevators to return to floors
- [ ] Chest rewards
- [ ] Mining skill progression

### 5.5 Crafting System
**Priority:** MEDIUM
**New files to create:**
- `Assets/Scripts/Crafting/CraftingController.cs`
- `Assets/Scripts/Crafting/CraftingRecipe.cs`
- `Assets/Scripts/UI/CraftingUI.cs`

**Tasks:**
- [ ] Workbench building
- [ ] Recipe unlocking system
- [ ] Crafting categories (tools, furniture, machines, etc.)
- [ ] Ingredient requirement display
- [ ] Batch crafting
- [ ] Crafting animations
- [ ] Learn recipes from NPCs or finding them

### 5.6 Fertilizer System
**Priority:** LOW
**Files to modify:**
- `Assets/Scripts/Map/GridPropertyDetails.cs` - Add fertilizer property
- `Assets/Scripts/Crop/Crop.cs` - Fertilizer affects growth

**Tasks:**
- [ ] Basic fertilizer (faster growth)
- [ ] Quality fertilizer (better harvest quality)
- [ ] Deluxe fertilizer (both benefits)
- [ ] Apply before planting
- [ ] Fertilizer persists until harvest
- [ ] Buy from shop or craft
- [ ] Visual indicator on fertilized soil

---

## PHASE 6: POLISH & QUALITY OF LIFE

### 6.1 Audio System
**Priority:** MEDIUM
**New files to create:**
- `Assets/Scripts/Audio/AudioManager.cs`
- `Assets/Scripts/Audio/SoundDetails.cs`

**Tasks:**
- [ ] Implement AudioManager singleton
- [ ] Sound effects for all actions (plant, water, harvest, walk, UI clicks)
- [ ] Background music for each scene
- [ ] Seasonal music variations
- [ ] Volume controls (master, music, SFX)
- [ ] Audio mixer
- [ ] Ambient sounds (birds, wind, rain)

### 6.2 Settings Menu
**Priority:** MEDIUM
**New files to create:**
- `Assets/Scripts/UI/SettingsUI.cs`

**Tasks:**
- [ ] Graphics settings (resolution, fullscreen, quality)
- [ ] Audio settings (volume sliders)
- [ ] Keybind customization
- [ ] Language selection
- [ ] Save settings to PlayerPrefs
- [ ] Reset to defaults button

### 6.3 Tutorial System
**Priority:** MEDIUM
**New files to create:**
- `Assets/Scripts/Tutorial/TutorialManager.cs`
- `Assets/Scripts/Tutorial/TutorialStep.cs`

**Tasks:**
- [ ] First-time player detection
- [ ] Step-by-step guide (move, plant, water, harvest, shop)
- [ ] Highlight UI elements
- [ ] Block player input during instructions
- [ ] Skip tutorial option
- [ ] Tooltips for first-time actions
- [ ] Grandfather introduction cutscene

### 6.4 Minimap/Map
**Priority:** LOW
**New files to create:**
- `Assets/Scripts/UI/MinimapController.cs`
- `Assets/Scripts/UI/WorldMapUI.cs`

**Tasks:**
- [ ] Minimap in corner showing nearby area
- [ ] Player icon and direction
- [ ] NPC locations
- [ ] Full map menu (press M)
- [ ] Reveal map as player explores
- [ ] Location markers
- [ ] Quest markers

### 6.5 Calendar & Journal
**Priority:** LOW
**New files to create:**
- `Assets/Scripts/UI/CalendarUI.cs`
- `Assets/Scripts/UI/JournalUI.cs`

**Tasks:**
- [ ] Visual calendar showing current day
- [ ] Mark NPC birthdays
- [ ] Mark festival dates
- [ ] Journal for notes
- [ ] Crop growth predictions
- [ ] Fish availability guide
- [ ] Recipe list

### 6.6 Code Cleanup
**Priority:** HIGH
**Files to modify:** All script files

**Tasks:**
- [ ] Remove all Debug.Log statements or wrap in `#if UNITY_EDITOR`
- [ ] Translate Chinese comments to English
- [ ] Standardize naming conventions (PascalCase for classes)
- [ ] Add XML documentation comments
- [ ] Remove magic numbers (replace with enums/constants)
- [ ] Add null checks where missing
- [ ] Cache expensive lookups (FindObjectOfType)
- [ ] Optimize string concatenation (use StringBuilder)
- [ ] Remove redundant singleton implementations
- [ ] Add consistent error handling

---

## PHASE 7: OPTIMIZATION & TESTING

### 7.1 Performance Optimization
**Priority:** MEDIUM
**Tasks:**
- [ ] Profile with Unity Profiler
- [ ] Optimize GridPropertiesManager dictionary lookups
- [ ] Object pooling for crop sprites/items
- [ ] Cull off-screen objects
- [ ] Optimize tilemap rendering
- [ ] Reduce Update() overhead
- [ ] Async scene loading improvements
- [ ] Memory leak detection

### 7.2 Mobile Support (Optional)
**Priority:** LOW
**Tasks:**
- [ ] Touch input detection
- [ ] Virtual joystick UI
- [ ] Touch-friendly UI scaling
- [ ] Performance optimizations for mobile
- [ ] Platform-specific builds

### 7.3 Controller Support
**Priority:** LOW
**Tasks:**
- [ ] Unity Input System integration
- [ ] Controller button mapping
- [ ] UI navigation with controller
- [ ] Cursor simulation with joystick
- [ ] Controller button prompts in UI

### 7.4 Testing
**Priority:** HIGH
**Tasks:**
- [ ] Playtest full game loop (plant → harvest → sell → buy)
- [ ] Test all tools on all crops
- [ ] Test save/load in all scenes
- [ ] Test NPC schedules and dialogue
- [ ] Test quest completion
- [ ] Test edge cases (full inventory, no money, etc.)
- [ ] Fix any discovered bugs
- [ ] Performance testing on target hardware

---

## PHASE 8: CONTENT CREATION (Ongoing)

### 8.1 Crops
**Current:** Basic crops exist
**Needed:**
- [ ] 30+ crop varieties
- [ ] 5-10 crops per season
- [ ] Multi-harvest crops (tomatoes, berries)
- [ ] Giant crops (rare 3x3 mega crops)
- [ ] Flower crops (for bees/gifts)
- [ ] Fruit trees (apple, cherry, orange, etc.)

### 8.2 Fish
**Current:** None
**Needed:**
- [ ] 40+ fish species
- [ ] Seasonal fish
- [ ] Time-of-day fish
- [ ] Location-specific fish (river vs ocean)
- [ ] Legendary fish (boss fish)
- [ ] Trash items (boots, seaweed)

### 8.3 Items
**Current:** Basic tools and seeds
**Needed:**
- [ ] Cooking ingredients
- [ ] Crafting materials
- [ ] Ores and gems
- [ ] Artisan goods
- [ ] Forageables (berries, mushrooms)
- [ ] Furniture items
- [ ] Decorations
- [ ] Seeds for all crops

### 8.4 Buildings
**Current:** Shop, tree, basic structures
**Needed:**
- [ ] Barn (3 upgrade tiers)
- [ ] Coop (3 upgrade tiers)
- [ ] Greenhouse
- [ ] Mill
- [ ] Silo (hay storage)
- [ ] Shed (extra storage)
- [ ] Stable (horse)
- [ ] Well (water source)
- [ ] Junimo Hut (auto-harvest)

### 8.5 NPCs
**Current:** None (only placeholder)
**Needed:**
- [ ] 15-20 villagers
- [ ] Each with unique personality
- [ ] Dialogue trees (500+ lines per NPC)
- [ ] Heart events (cutscenes)
- [ ] Gift preferences
- [ ] Daily schedules

### 8.6 Quests
**Current:** Only tree task system
**Needed:**
- [ ] 50+ side quests
- [ ] Main story questline
- [ ] Daily quests
- [ ] Weekly challenges
- [ ] Community center bundles (collection rewards)

---

## IMPLEMENTATION PRIORITY MATRIX

### Must Have (Core Gameplay)
1. Fix time system bug
2. Fix crop harvesting
3. Complete tool selection
4. Implement save/load
5. Add tool upgrades
6. Create stamina system
7. Build NPC system
8. Implement dialogue
9. Add fishing
10. Code cleanup

### Should Have (Expected Features)
11. Cooking system
12. Season system
13. Relationship system
14. Quest system
15. Greenhouse
16. Artisan goods
17. Livestock
18. Crafting system
19. Audio system
20. Settings menu

### Nice to Have (Polish)
21. Weather system
22. Mining
23. Achievements
24. Minimap
25. Calendar/Journal
26. Tutorial system
27. Controller support
28. Mobile support

---

## ESTIMATED TIME TO COMPLETION

Assuming 1 full-time developer:

- **Phase 1 (Bug Fixes):** 1 week
- **Phase 2 (Core Loops):** 3 weeks
- **Phase 3 (Core Features):** 6 weeks
- **Phase 4 (Social/Progression):** 8 weeks
- **Phase 5 (Advanced Content):** 6 weeks
- **Phase 6 (Polish):** 4 weeks
- **Phase 7 (Optimization):** 2 weeks
- **Phase 8 (Content Creation):** Ongoing (12+ weeks)

**Total Estimated Time:** 6-9 months for a complete, polished farming game

---

## MINIMAL VIABLE PRODUCT (MVP)

If you want a playable game quickly, focus on:
1. Phase 1 (Bug fixes) - 1 week
2. Phase 2.1-2.3 (Harvesting, tools, save/load) - 2 weeks
3. Phase 3.1-3.2 (Tool upgrades, stamina) - 2 weeks
4. Phase 6.1 (Audio) - 1 week
5. Phase 6.6 (Code cleanup) - 1 week
6. Create 15 crops, 5 NPCs, 10 quests - 3 weeks

**MVP Timeline:** 10 weeks (2.5 months)

---

## NEXT IMMEDIATE STEPS

1. **Fix TimeSetting.cs bug** (line 65)
2. **Fix Crop.cs harvest logic** (add threshold check)
3. **Test that crops can be planted, grown, and harvested**
4. **Remove debug logging from PlayerFarming.cs**
5. **Begin save/load implementation**

---

## RESOURCES NEEDED

### Art Assets
- NPC character sprites (portraits + overworld)
- Additional crop growth stage sprites
- Fish sprites
- Tool upgrade sprites
- Building interior tiles
- UI elements (icons, buttons, panels)
- Particle effects (harvest, water, etc.)

### Audio Assets
- Background music (5-10 tracks)
- Sound effects (100+ SFX for actions)
- Ambient sounds (nature, weather)
- Voice clips (optional, for NPCs)

### Writing
- NPC dialogue (5000+ lines)
- Quest descriptions (100+ quests)
- Item descriptions
- Tutorial text
- Achievement descriptions

---

## CONCLUSION

This farming game has a **solid foundation** but needs significant work to be complete. The architecture is good, but critical bugs prevent core gameplay. Focus on Phase 1 and 2 first to make the game playable, then expand with social features and content.

The existing systems (inventory, grid, UI) are well-designed and can support the missing features with minimal refactoring.

**Recommended Approach:**
1. Fix bugs (1 week)
2. Complete core loop (3 weeks)
3. Playtest MVP
4. Expand features based on feedback
5. Add content and polish

Good luck! This has the potential to be a great farming game once the bugs are fixed and features are completed.
