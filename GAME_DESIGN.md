# Brick Breaker - Game Design Document

## Game Overview

A modern reimagining of classic brick breaker gameplay optimized for mobile touchscreens. Players use intuitive slingshot controls to launch balls at numbered bricks, with difficulty and rewards increasing progressively.

## Core Gameplay Loop

1. **Aim**: Drag finger to aim trajectory
2. **Launch**: Release to fire balls sequentially
3. **Break**: Balls bounce and damage bricks
4. **Collect**: Gather balls that return to starting position
5. **Progress**: Clear bricks to advance levels

## Game Mechanics

### Slingshot Controls

**Input Method:**
- Touch and drag from launch area
- Visual trajectory line shows ball path
- Release to launch all collected balls

**Launch Physics:**
- Drag distance determines launch force
- Minimum force: 5 units
- Maximum force: 20 units
- All balls launch with same angle/force
- Small delay between each ball (0.1s)

### Ball System

**Ball Properties:**
- Constant speed (5-15 units)
- No gravity
- Bounces off walls and bricks
- Destroys on touching ground
- Each deals 1 damage per hit

**Cumulative Collection:**
- Start with 1 ball
- +1 ball per level completed
- Persist across rounds
- All must return before next round
- Can purchase more balls via IAP

**Ball Physics:**
- Continuous collision detection
- Slight random angle on bounce (prevents loops)
- Trail effect for visual feedback
- Minimum speed enforcement

### Brick System

**Brick Health:**
```
health = baseHealth + level + rowIndex
```
- Base health: 1
- Increases with level number
- Increases with row position (higher = more health)
- Example: Level 5, Row 3 = 1 + 5 + 3 = 9 health

**Brick Layout:**
- Grid: 7 columns × dynamic rows
- Spacing: 0.1 units
- Width: 1.2 units
- Height: 0.6 units
- New row spawned each level

**Brick Behavior:**
- Takes damage on ball collision
- Displays current health as number
- Color changes based on health
- Destroys when health reaches 0
- Awards points based on position

### Scoring System

**Points Awarded:**
```
score = (baseHealth + rowIndex) × baseScore
```
- Base score: 10 points
- Higher rows worth more points
- Displayed in real-time

**High Score:**
- Tracked locally via PlayerPrefs
- Top 10 scores saved
- Displays player name, score, level, date
- Gold/Silver/Bronze highlighting

### Level Progression

**Level Start:**
- Spawn new row of bricks
- Balls reset to collected amount
- Bricks from previous level remain

**Level Complete:**
- All bricks destroyed
- Reward: +1 ball to collection
- Show level complete screen (2s)
- Interstitial ad every 3 levels
- Auto-advance to next level

**Difficulty Curve:**
- More bricks each level
- Higher brick health each level
- More balls to manage (cumulative)
- Strategic importance increases

### Game Over Conditions

**Trigger:**
- All balls lost AND
- Bricks reach bottom row (row ≤ 0)

**Options:**
1. Watch rewarded ad to continue
   - Restores all balls
   - Moves bricks up one row
2. Restart game
   - Reset to level 1
   - Reset ball count to 1
   - Score resets to 0

### Round System

**Round Start:**
- Player has X balls (collected amount)
- All balls must be launched
- Aim and shoot

**Round End:**
- Triggered when all balls lost (touched ground)
- If bricks remain: Move bricks down one row
- Check game over condition
- If safe: New round starts

## Visual Design

### Minimalist Aesthetic

**Color Palette:**
- Background: Dark blue-gray (#1A1A26)
- Bricks: Color gradient by health
  - Light Blue → Green → Yellow → Orange → Red → Purple
- Ball: White or light blue with trail
- UI: Clean white text, subtle shadows

**UI Style:**
- Flat design language
- Thin borders
- Smooth animations
- Clear typography
- Minimalist buttons

**Visual Feedback:**
- Ball trail effect
- Brick health numbers update
- Smooth color transitions
- Particle effects on destroy
- Screen shake on impacts (optional)

## Audio Design

### Sound Effects

**Gameplay:**
- Ball launch: Soft "whoosh"
- Ball bounce: Light tap (low volume)
- Brick hit: Sharp "thud"
- Brick destroy: Satisfying pop
- Level complete: Positive jingle
- Game over: Descending tones

**UI:**
- Button click: Soft tap
- Panel open/close: Slide sound
- Purchase: Cash register ding
- Achievement: Chime

**Music:**
- Ambient electronic background
- Low volume (0.5)
- Seamless loop
- Can be toggled off

### Haptic Feedback

**Events:**
- Ball launch: Light vibration
- Brick hit: Light vibration
- Brick destroy: Medium vibration
- Level complete: Heavy vibration
- Game over: Heavy vibration
- Can be toggled off

## Monetization Strategy

### Ad Implementation

**Banner Ads:**
- Position: Bottom of screen
- Always visible during gameplay
- Can be hidden after IAP

**Interstitial Ads:**
- Frequency: Every 3 levels
- Timing: After level complete screen
- Skippable after 5 seconds

**Rewarded Video Ads:**
- Trigger: Game over screen
- Reward: Continue game (restore balls + move bricks up)
- Optional: Never forced
- Good conversion opportunity

**Ad Balance:**
- Not intrusive during active play
- Strategic timing (natural breaks)
- Value exchange (rewarded ads)
- Removable via IAP

### In-App Purchases

**Products:**

1. **Remove Ads** - $2.99
   - Non-consumable
   - Removes banner + interstitial
   - Keeps rewarded ads (optional)
   - Most popular IAP

2. **Extra Balls (Small)** - $0.99
   - Consumable
   - +5 balls immediately
   - Quick boost for stuck players

3. **Extra Balls (Medium)** - $1.99
   - Consumable
   - +15 balls
   - Better value proposition

4. **Extra Balls (Large)** - $4.99
   - Consumable
   - +50 balls
   - Best value
   - "Whale" tier

**IAP Strategy:**
- Clear value proposition
- No pay-to-win mechanics
- Accelerate enjoyment
- Optional convenience
- Fair pricing

## User Experience

### First-Time User Experience (FTUE)

1. **Launch**: Show main menu immediately
2. **First Play**:
   - Brief text: "Drag to aim, release to shoot"
   - Visual arrow pointing to launch area
3. **First Brick Hit**: "Bricks show health!"
4. **First Level Complete**: "You earned an extra ball!"
5. **No more tutorials**: Learn by playing

### UI/UX Flow

**Main Menu:**
```
[Brick Breaker Logo]
[Play Button]
[Leaderboard] [Settings] [Shop]
```

**Gameplay Screen:**
```
Score: 0    Level: 1    Balls: 1/1    [Pause]

[Game Area with Bricks, Balls, Trajectory]

[Banner Ad]
```

**Game Over Screen:**
```
Game Over
Final Score: 1234
New High Score!

[Watch Ad to Continue]
[Restart]
[Main Menu]
```

**Leaderboard:**
```
🏆 Leaderboard

Rank | Name    | Score | Level
1    | Player1 | 5432  | Lvl 12
2    | Player2 | 4321  | Lvl 10
...
```

### Settings

**Options:**
- Sound On/Off
- Music On/Off
- Haptics On/Off
- SFX Volume slider
- Music Volume slider

### Accessibility

- Large touch targets (minimum 44×44 dp)
- Clear color contrast
- No color-only information
- Audio can be disabled
- Haptics can be disabled
- No time pressure
- Pause available anytime

## Technical Specifications

### Performance Targets

- **Frame Rate**: 60 FPS constant
- **Load Time**: < 3 seconds
- **App Size**: < 50 MB
- **Memory**: < 200 MB RAM usage
- **Battery**: Minimal drain

### Platform Requirements

- **Minimum**: Android 5.1 (API 22)
- **Target**: Android 13 (API 33)
- **Orientation**: Portrait only
- **Resolution**: 1080×1920 baseline
- **Aspect Ratios**: 16:9 to 19:9

### Analytics Events (Future)

```
- game_start
- level_complete (level, score)
- game_over (level, score, reason)
- ad_watched (type, result)
- iap_initiated (product_id)
- iap_completed (product_id, price)
- session_length
```

## Game Balance

### Difficulty Curve

**Early Game (Levels 1-5):**
- Low brick health (1-6)
- Few rows (1-5)
- Easy to progress
- Hook players

**Mid Game (Levels 6-15):**
- Medium brick health (7-20)
- Multiple rows (6-10)
- Strategy matters
- Collect more balls

**Late Game (15+):**
- High brick health (20+)
- Many rows (10+)
- Requires precision
- Use extra balls

### Balancing Levers

If too easy:
- Increase brick health multiplier
- Reduce ball speed
- Reduce launch force max

If too hard:
- Decrease brick health
- Start with 2-3 balls
- Offer free ball IAP

## Future Features (Post-Launch)

### Potential Additions:

1. **Power-Ups:**
   - Multi-ball (split into 3)
   - Laser ball (goes through bricks)
   - Explosive ball (area damage)
   - Slow-mo (better aim)

2. **Daily Challenges:**
   - Special levels
   - Unique rewards
   - Encourage daily play

3. **Skins/Themes:**
   - Ball skins (IAP)
   - Brick themes
   - Background themes
   - Customization revenue

4. **Online Leaderboard:**
   - Global rankings
   - Friends leaderboard
   - Weekly competitions

5. **Achievements:**
   - "Score 1000 points"
   - "Reach level 10"
   - "Collect 20 balls"
   - Google Play achievements

6. **Social Features:**
   - Share high score
   - Challenge friends
   - Replay system

## Success Metrics

### Launch Targets:

- **Day 1 Retention**: > 40%
- **Day 7 Retention**: > 20%
- **Average Session**: > 5 minutes
- **Ad Fill Rate**: > 80%
- **IAP Conversion**: > 2%
- **Crash Rate**: < 1%

### Revenue Goals:

- **ARPU**: $0.10 - $0.50
- **ARPPU**: $5.00 - $15.00
- **Ad Revenue**: 70% of total
- **IAP Revenue**: 30% of total

## Competitive Analysis

### Similar Games:

1. **Ballz** (Ketchapp)
   - Numbered bricks
   - Physics-based
   - Our advantage: Better progression

2. **BBTan** (111%)
   - Level-based
   - Power-ups
   - Our advantage: Simpler, less clutter

3. **Brick Breaker Quest**
   - Classic gameplay
   - Our advantage: Modern controls, monetization

### Unique Selling Points:

- Slingshot controls (not paddle)
- Cumulative ball collection
- Progressive difficulty
- Minimalist design
- Fair monetization
- No energy system (play anytime)

## Design Pillars

1. **Simplicity**: Easy to learn, hard to master
2. **Satisfaction**: Every brick destroyed feels good
3. **Progression**: Always getting stronger
4. **Fairness**: Never pay-to-win
5. **Polish**: Smooth, responsive, beautiful

---

**Version**: 1.0
**Last Updated**: 2026-01-20
**Status**: Ready for Development
