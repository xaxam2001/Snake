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
Go to https://github.com/xaxam2001/Snake/releases/tag/v2.0.0 and download the Godot .zip archive. Extract it and launch the `Snake.exe` executable.
- Use the `directional arrows` or `WASD` to move. The difficulty is set on Normal by default.
- Choose to enable the AI agent or not (if yes, you can choose a model by specifying the path of one of the bin file contained in the `Models` folder).

### CMD version
Go to https://github.com/xaxam2001/Snake/releases/tag/2.0.0 and download the .exe file. You can either:
- Launch the `.exe` file like any other program. Use the `directional arrows` or `WASD` to move. The difficulty is set on Normal by default.
- Run it through the command line `.\Snake.v1.2.0.exe [difficulty]` and choose the difficulty from `0: easy` to `2: hard`.

## 📁 Repository Organisation

- `Core` contains the C# Library used for the Snake Game logic. It is completely separated from the visual and can be use with any game engine.
- `ML_lib` contains the C++ Library for the machine learning agents.
- `snake-cmd-visual` contains the C# project for a simple Snake displaying in the console.
- `snake-godot-visual` contains the Godot project for the Godot visual version of the snake game.
- `Model training` contains the Python source code for testing the ML_lib.
- `Models` contains all the pretrained models used by the AI agent in the game.
- `PowerPoint Presentation` contains the different presentations made for this project.
- `Data` contains the `csv` files recorded during gameplay for training the models.

## Models naming convention

The models are named following this convention: `[NumberOfExamples]X_[Layers]_[number of iteration]_[learning rate]_[proportion of train].bin`

Best model so far: `46696X_[520, 128, 64, 4]_1400000iter_0.01+0.001lr_0.9train.bin` 

## 📜 Project Status & Roadmap

### ✔️ Completed

- C# Core library to run the Snake logic.
- Command line visual for debug and play the snake game.
- Godot 2D visual for the Snake Game
- Implementing the ML_lib for the Linear Model (classification and regression).
- Implementing a recorder to start collecting the Data
- Implementing the complete ML_lib (Multi Layer Perceptron, Radial Basis Function Network).
- Build different model using the ML_lib.
- Train these different models using collected player data.

### 📆 Future Implementation

Done for now.

## 🎓 Author & Contact

* **Maxime Chalumeau** - (https://github.com/xaxam2001)
