# Escape-Rooms-MyoVersion

# Project for Gesture Based UI Development 2018
Project for Gesture Based UI Development.
The module is taught to undergraduate students at [GMIT](http://www.gmit.ie) in the Department of Computer Science and Applied Physics.
The lecturer is Damien Costello.

---


## Design and implementation
This project contains game build in Unity2017 using multiple C# reusable scripts and assets from [pluralsight](https://app.pluralsight.com). The project also based on my previous, mobile development project, with additional improvements including a new game level, [Myo Armband](https://www.myo.com/) control and new menu.

[Myo Armband](https://www.myo.com/) hardware used to control the player and interact with objects. 

[Microsoft Azure](https://azure.microsoft.com) database was used to store and retrieve players list in the final menu.

## Problems / difficulties

The main difficulty for me was related to making it possible adequately control the player using the gyro sensor. At the beginning player constantly moved around the screen without stopping which made it difficult to control. I had to create variables to activate movement only at specific angles.

### Game design

The game contains 5 main parts:
1. Starting menu with start and quit buttons:

![](https://i.imgur.com/gdBEOcB.jpg)

2. First room/level:

![](https://i.imgur.com/tkMsf0W.png)

3. Second room/level:

![](https://i.imgur.com/xEbO0bX.jpg?1.jpg)

4. Third room/level:

![](https://i.imgur.com/W9qqv8e.png)

5. Final menu with the ability to store retrieve players list from Azure simple table database:

![](https://i.imgur.com/LJO9MXB.png)

# Video Demo<a name = "demo"></a>

## Quick 2 Minute Video
<kbd>[![IMAGE ALT TEXT HERE](https://image.ibb.co/jLv6Mx/MyoVideo.png)](https://youtu.be/0z4STHLcCFY)</kbd>

## How to run

1. Download [RunEscapeRoomGame.7z](https://drive.google.com/file/d/11WEoYJ6ik2Fi4emEQUw1gm3jZSJ8_mZn/view?usp=sharing) folder from my google drive.

2. Unzip to any location.

3. Start the game by executing EscapeRoomsMyo .exe file

Please note that [Myo Armband](https://www.myo.com/) must be connected to the PC in order to be able to play the game.

### Controls

The player can be controlled by [Myo Armband](https://www.myo.com/). Use double tap gesture or "E" to switch the triggers.
### Walkthrough game

In the first level, the user needs to push the statue and use appearing trigger for opening door to the next level.

The same idea applies to the second level but the player needs to push statue through the teleporter.

At the third level, the player needs to use the second, the first, and then the third trigger to open the door. Then push the statue to make a visible fourth trigger which will open all the traps. If the player falls into the trap that will teleport him back to the first level.

When the player reaches the last door, a menu will appear letting him save his name.

### References:

Myo Armband https://www.myo.com/

Pluralsight: https://www.pluralsight.com/courses/unity-designing-game-puzzles

Main menu: https://www.youtube.com/watch?v=_1wMnE06PeU

Timer: https://www.youtube.com/watch?time_continue=170&v=w33cOjMT0fE

Azure set up: https://www.youtube.com/watch?v=R8adpelztJA&feature=youtu.be
