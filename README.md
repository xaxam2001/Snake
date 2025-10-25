# Interdisciplinary Snake Game Development

## 🔍 Project Overview

This repository contains a multi-disciplinary project combining **Godot Development** and **Machine Learning**.
The core objectives are:
- Developing a **Snake arcade game** following the .NET standard and using Godot v4.5 for the visual.
- Developing a Machine Learning **C++ Library** and **train a model** using imitation learning to effectively play the game.

**The final product demonstrates a self-learning agent capable of playing the game trained using real players data**

## 💻 Technologies & Frameworks

* **Game Engine:** Godot Engine (Version 4.5)
* **Language:** C#, C++20, Python 3.11

## 🕹️ How to play

### Godot version
Go to https://github.com/xaxam2001/Snake/releases/tag/v1.1.0 and download the .zip archive. Extract it and launch the `Snake.exe` executable.

### CMD version
Go to https://github.com/xaxam2001/Snake/releases/tag/v1.1.0 and download the .exe file. You can either:
- Launch the `.exe` file like any other program. Use the `directional arrows` or `WASD` to move. The difficulty is set on Normal by default.
- Run it through the command line `.\Snake.v1.0.0.exe [difficulty]` and choose the difficulty from `0: easy` to `2: hard`.

## 📁 Repository Organisation

- `Core` contains the C# Library used for the Snake Game logic. It is completely separated from the visual and can be use with any game engine.
- `ML_lib` contains the C++ Library for the machine learning agents.
- `snake-cmd-visual` contains the C# project for a simple Snake displaying in the console.
- `snake-godot-visual` contains the Godot project for the Godot visual version of the snake game.
- `python-test-folder` contains the Python source code for testing the ML_lib.

## 📜 Project Status & Roadmap

### ✔️ Completed

- C# Core library to run the Snake logic.
- Command line visual for debug and play the snake game.
- Godot 2D visual for the Snake Game

### ➡️	Direct Next Steps

- Implementing the ML_lib for the Linear Model (classification and regression).
- Implementing a recorder to start collecting the Data

### 📆 Future Implementation

- Implementing the complete ML_lib (Multi Layer Perceptron, Radial Basis Function Network).
- Build different model using the ML_lib.
- Train these different models using collected player data.

## 🎓 Authors & Contact

* **Maxime Chalumeau** - (https://github.com/xaxam2001)
* **Tom Dordet** - (https://github.com/TomDordet)
