<div align="center">

# W3 Box Rush

**A first-person warehouse night shift. Grab the boxes, throw them into the truck, beat the clock.**

![Unity](https://img.shields.io/badge/Unity-6.2-black?logo=unity&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![Blender](https://img.shields.io/badge/Blender-5.0-orange?logo=blender&logoColor=white)
![Hack Club Terra](https://img.shields.io/badge/Hack_Club-Terra_Week_3-ec3750)

<img width="3822" height="2125" alt="Screenshot 2026-11-09 at 18 25 18" src="https://github.com/user-attachments/assets/54ea6bd4-3e37-4c14-9c5d-b933d33d24b9" />
<img width="3832" height="2117" alt="Screenshot 2026-11-09 at 18 24 45" src="https://github.com/user-attachments/assets/f1769322-3846-4f35-9f71-2d37dc77bdab" />


[▶ **Play the demo**](https://ipalp.itch.io/boxrush) 

</div>

---

##  What is it?

You are the night worker in a dark warehouse. A box appears on the conveyor belt every few seconds and rides along it. Your job is to pick the boxes up and throw them into the delivery truck parked at the open dock.

Every box that lands in the truck gives you points, and the shift timer is always running, so you get one chance to set the highest score you can.

## Why I made it

This is my **first first-person game**. I wanted to learn how first-person movement and mouse look work in Unity, and how to build a physics-based box pickup where the box you hold still collides with the world, like the object pickup in Half-Life 2.

It is also the first time I modeled a whole environment in Blender (the warehouse, the conveyor belt and the boxes) and brought it into Unity myself. I made it for **Hack Club Terra**, as week 3 of my project series.

## How to play

| Key | Action |
|:---|:---|
| <kbd>W</kbd> <kbd>A</kbd> <kbd>S</kbd> <kbd>D</kbd> or arrow keys | Move |
| Mouse | Look around |
| <kbd>Shift</kbd> | Sprint |
| <kbd>Space</kbd> | Jump |
| <kbd>E</kbd> | Grab the box you are looking at / drop it |
| **Left click** | Throw the box you are holding |
| <kbd>Esc</kbd> | Free the mouse cursor (click the game window to lock it again) |

##  Features

- ✅ First-person movement and mouse look
- ✅ Physics box grabbing: held boxes follow the camera but still collide with the world
- ✅ Box throwing in the direction you are looking
- ✅ L-shaped conveyor belt, with a different direction for each arm
- ✅ Box spawner that drops a new box on the belt every few seconds
- ✅ Delivery truck zone that counts and removes the boxes you throw in
- ✅ Score and shift timer shown on screen
- ✅ Warehouse, conveyor belt and boxes modeled in Blender

##  How it's built

Made with **Unity 6.2** (6000.2.13f1) and **Blender 5.0**. All the scripts are in `Assets/Scripts`.

<details>
<summary><b>Script overview</b> (click to expand)</summary>

<br>

| Script | What it does |
|:---|:---|
| `FirstPersonController.cs` | Movement, jumping and mouse look |
| `BoxGrabber.cs` | Raycast to find a box, hold it in front of the camera with physics, drop and throw it |
| `MultiConveyorBelt.cs` | Moves boxes along each arm of the belt in its own direction |
| `BoxSpawner.cs` | Spawns a box prefab at a set interval |
| `DeliveryZone.cs` | Trigger inside the truck that adds points and removes the box |
| `GameManager.cs` | Score, shift timer, on-screen text and restart |

</details>

##  Run it yourself

1. Install **Unity 6.2 (6000.2.13f1)** through Unity Hub.
2. Clone this repository and add the project folder in Unity Hub.
3. Open the project, then open `Assets/Scenes/SampleScene`.
4. Press **Play**.

##  What's next

- [ ] Main menu
- [ ] Sound effects and music
- [ ] Visual effects (dust, impacts)
- [ ] Something to make the loop more interesting, like a quota per shift and points lost for missed boxes
