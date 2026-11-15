# Box Rush: Development Process

How Box Rush (Week 3) was built, in the order it was built, and how each system works.

> [!NOTE]
> **About the commit history:** this project was pushed to GitHub in a large commitw instead of committing as I went. That was a mistake, and this document is my attempt to make the development process verifiable anyway. Every step below matches a devlog entry on Terra, with its date and the time I logged. From Week 4 onward I commit before posting each devlog on Terra.

---

## Overview

| | |
|---|---|
| **Game** | A first-person night-shift warehouse game. Boxes spawn on an L-shaped conveyor belt, you grab them with a physics-based pickup, and you throw them into a delivery truck to score points before the shift timer runs out. |
| **Engine** | Unity 6 with the Universal Render Pipeline |
| **Art** | Environment and props modeled in Blender |
| **Logged time (Terra devlogs)** | 3h 13m + 3h 34m + 2h 21m, about 9 hours in three entries|

## Timeline (matches my Terra devlogs)

| Devlog | Date | Logged time | What was built |
|---|---|---|---|
| 1 | Nov 8 | 3h 13m | First-person movement (WASD, jumping, sprinting) and the first 3D assets |
| 2 | Nov 9 | 3h 34m | Box pickup and throw mechanic, and the warehouse model in Blender |
| 3 | Nov 9 | 2h 21m | Conveyor belt system, box generator, the game location, UI, and the game loop (timer and points) |

---

## 1. Player movement and camera (devlog 1)

**Script:** `FirstPersonController`, on a player object with a `CharacterController`.

| Feature | How it works |
|---|---|
| **Looking** | The mouse delta, multiplied by `lookSensitivity`, turns the whole player body left and right (yaw). Up and down (pitch) is applied only to a separate `cameraPivot` object and clamped to 85°, so the view can't flip over. |
| **Walking** | WASD input is converted into a direction relative to where the body is facing, and normalized so moving diagonally isn't faster. |
| **Sprinting** | Holding Left Shift switches from `walkSpeed` (4) to `sprintSpeed` (7). |
| **Gravity** | The script keeps its own `verticalVelocity`. Gravity (-20) is added every frame. While grounded, the velocity is held at a small negative value so the controller stays stuck to the floor. |
| **Jumping** | The upward speed comes from `sqrt(jumpHeight * -2 * gravity)`, so `jumpHeight` is in real units. |
| **Moving** | Horizontal movement and vertical velocity are combined and passed to `CharacterController.Move`. |
| **Cursor** | Locked at start. Escape unlocks it, and a click locks it again. While it is unlocked, look and move are skipped. |
| **Input** | Uses Unity's Input System when enabled and falls back to the old Input Manager otherwise (`#if ENABLE_INPUT_SYSTEM`). |

---

## 2. Grabbing, holding and throwing boxes (devlog 2)

**Script:** `BoxGrabber`, on the player.

The goal was a physics-based pickup where the box is still a real physics object while you carry it, instead of being glued to the camera.

**Controls:** `E` grabs a box (or drops the one you're holding), and left click throws it. Input is ignored while the cursor is unlocked.

### Settings

| Setting | Default | Meaning |
|---|---|---|
| `grabRange` | 3 | How far the grab ray reaches |
| `maxMass` | 20 | Boxes heavier than this can't be picked up |
| `holdDistance` | 1.8 | How far in front of the camera the box is held |
| `followStrength` | 15 | How strongly the box is pulled toward the hold point |
| `breakDistance` | 3 | If the box ends up this far from the hold point, it is dropped |
| `throwSpeed` | 14 | Speed of a thrown box |

### Picking up a box

1. A ray is cast from the camera, `grabRange` units forward, ignoring trigger colliders. The object hit must have a Rigidbody, must not be kinematic, and must not be heavier than `maxMass`.
2. The script remembers whether the box had gravity, then turns gravity **off** for as long as it's held.
3. It collects all colliders of the box and tells Unity to **ignore collisions between those colliders and the player's `CharacterController`** (`Physics.IgnoreCollision`).

> [!IMPORTANT]
> **Why the held box ignores the player:** a box that is pulled to a spot right in front of the camera would otherwise collide with the player's own capsule. Players could push themselves around, get stuck, or stand on the box they're holding and launch themselves into the air. Ignoring player collisions while the box is held prevents all of that. The box still collides with everything else (walls, belts, other boxes), and collision with the player is switched back on when the box is released.

### Holding the box (every physics step)

- The target point is `holdDistance` units in front of the camera.
- Instead of teleporting the box or parenting it to the camera, the script sets the box's **velocity** toward the target, scaled by `followStrength`. The box behaves as if it's pulled by a spring: it follows smoothly, and it is stopped by walls instead of passing through them.
- Its angular velocity is set to zero so it doesn't spin while carried.
- If the box gets stuck and ends up more than `breakDistance` from where it should be (for example, pinned behind a shelf), it is dropped automatically.

### Releasing and throwing

Releasing restores the box's gravity and its collision with the player. Throwing does the same, and then sets the box's velocity to the camera's forward direction times `throwSpeed`, so the box flies where you're looking.

---

## 3. Conveyor belt physics (devlog 3)

**Script:** `MultiConveyorBelt`.

The belt is a single GameObject with one Box Collider for each straight section. Each collider has its own direction in a `directions` list, so one script drives the whole L shape (for example, section 0 moves along +Z and section 1 along +X).

- It runs in `OnCollisionStay`, so it applies to whatever is touching the belt on every physics step.
- It ignores any Rigidbody that is kinematic or has gravity turned off, so only free physics boxes are moved. A box being carried (gravity off) is not pushed by the belt.
- It only reacts to contacts on top of the belt. A contact counts if its surface normal points mostly up (`normal.y >= 0.7`), so a box leaning against the side isn't pushed.
- It finds which collider the box touched (by its index in the object's collider list) and uses the matching direction.
- It sets the box's horizontal velocity to `direction * speed` but keeps its current vertical velocity, so gravity and landing still work normally. Setting the velocity directly (instead of adding force) gives a constant belt speed.
- A small preprocessor check chooses `linearVelocity` or `velocity` depending on the Unity version.
- `OnDrawGizmos` draws a yellow arrow for each section in the Scene view, so the directions can be checked without running the game.

---

## 4. Box spawner (devlog 3)

**Script:** `BoxSpawner`.

| Setting | Default | Meaning |
|---|---|---|
| `interval` | 4 s | Time between spawns |
| `maxAlive` | 10 | No new boxes spawn while this many spawned boxes still exist |
| `randomYaw` | 15° | Each box is turned by a random amount up to this, so they don't all look identical |

- A timer counts up in `Update`. Every `interval` seconds it spawns a box prefab at the spawner's position.
- It keeps a list of the boxes it has spawned and removes entries that have been destroyed. If the number still alive is at or above `maxAlive`, it skips that spawn, so the belt can't fill up with boxes.
- `OnDrawGizmos` draws a green wire cube where the boxes appear.

---

## 5. Delivery zone and scoring (devlog 3)

**Script:** `DeliveryZone`, used with `GameManager`.

- The zone is a trigger collider over the truck bed. When something enters, `OnTriggerEnter` finds the Rigidbody that owns the collider (`attachedRigidbody`), which also works if the collider is on a child object.
- It ignores bodies that are kinematic or have gravity turned off. A box you're holding has gravity off (see section 2), so **only a thrown box scores**, not one you carry into the truck.
- A `HashSet<Rigidbody>` remembers every box that has already scored, so a box that touches the trigger twice, or has more than one collider, can only score once.
- A new box adds `points` (10 by default) through `GameManager.Instance.AddScore(points)`, and the box is destroyed after `destroyDelay` seconds so you can see it land.

---

## 6. Game loop, timer and UI (devlog 3)

**Script:** `GameManager`, a singleton (`GameManager.Instance`) that other scripts use to report score.

- It stores the score and a `timeLeft` that starts at `shiftLength` (120 seconds) and counts down every frame.
- When the time reaches zero, the game is marked over, the cursor is unlocked, and `AddScore` stops adding points.
- The score and the remaining time (as minutes:seconds) are drawn on the screen using `OnGUI`. When the shift ends, a "SHIFT OVER" screen shows the final score.
- Pressing `R` after the shift ends reloads the scene to start a new round.

---

## 7. Blender work

I modeled the warehouse environment and the props, including the boxes, in Blender, then exported them to Unity. The Lapse recordings for the Blender work are linked in the Terra devlogs.

---

## How the systems fit together

The boxes are ordinary Rigidbodies with gravity on. The conveyor belt and the delivery zone both react only to Rigidbodies that are not kinematic and have gravity on. When the player grabs a box, `BoxGrabber` turns its gravity off, so the belt stops moving it and the delivery zone ignores it. When the box is released or thrown, gravity comes back, and the belt and the zone treat it as a normal box again.

---

## Commit practice from now on

For Week 4 and later I commit after every finished step (for example "add main menu scene" or "fix delivery zone collider") and attach those commits to my devlogs, so the history shows how the project grew.
