using System;
using Godot;
using System.Linq;
using System.Runtime.InteropServices;

public partial class AIPlayerManager : Node
{
    [Export] public string modelPath;
    
    [Export] private bool useNoise = true;
    [Export] private float noiseScale = 0.05f;
    
    [Export] private int stuckThreshold = 100;
    
    private MLP _mlp;
    private static IntPtr _dllHandle = IntPtr.Zero;
    
    private Random _rng = new Random();
    
    private int _lastScore = 0;
    private int _stepsSinceScore = 0;
    
    public override void _Ready()
    {
        // load the dll
        if (_dllHandle == IntPtr.Zero)
        {
            try 
            {
                // Convert "res://ML_lib.dll" to OS path
                string dllPath = ProjectSettings.GlobalizePath("res://ML_lib.dll");
                
                // Load it into memory
                _dllHandle = NativeLibrary.Load(dllPath);
                GD.Print($"Successfully loaded DLL from: {dllPath}");
            }
            catch (Exception e)
            {
                GD.PrintErr($"Failed to load native library: {e.Message}");
                return;
            }
        }

        //  Load the model
        if (!string.IsNullOrEmpty(modelPath))
        {
            string globalPath = ProjectSettings.GlobalizePath(modelPath);
            _mlp = MLP.Load(globalPath); 
        }
    }

    public bool[] GetNextAction(int[] gameState)
    {
        if (_mlp == null) 
        {
            GD.PrintErr("MLP model is not loaded.");
            return new bool[4]; // Return empty/default move
        }
        
        int currentScore = gameState[1];

        if (currentScore != _lastScore)
        {
            // Score changed, the snake is not stuck
            _lastScore = currentScore;
            _stepsSinceScore = 0;
        }
        else
        {
            // Score didn't change we count the step
            _stepsSinceScore++;
        }
        GD.Print($"Steps since score change: {_stepsSinceScore}:");
        
        // if the score didn't change for more than the threshold, we consider it stuck
        bool isStuck = _stepsSinceScore > stuckThreshold;

        double[] inputs = gameState
            .Skip(1)                  // Skip the first element (GameOver flag)
            .Select(x => (double)x)  
            .ToArray();

        // Predict
        double[] outputProbs = _mlp.Predict(inputs);
        
        // if the snake is considered stuck, we add some noise to try to break the loop
        if (useNoise && isStuck)
        {
            GD.Print($"Before noise: \n0: {outputProbs[0]} 1: {outputProbs[1]} 2: {outputProbs[2]} 3: {outputProbs[3]}");
            for (int i = 0; i < outputProbs.Length; i++)
            {
                // Generates random value between [-noiseScale, +noiseScale]
                double noise = (_rng.NextDouble() * 2.0 - 1.0) * noiseScale;
                outputProbs[i] += noise;
            }
            
            GD.Print("After noise:");
        }
        
        GD.Print($"0: {outputProbs[0]} 1: {outputProbs[1]} 2: {outputProbs[2]} 3: {outputProbs[3]}");
        
        // Find ArgMax (Index of the highest value)
        int maxIndex = -1;
        double maxValue = 0;

        for (int i = 0; i < outputProbs.Length; i++)
        {
            if (outputProbs[i] > maxValue)
            {
                maxValue = outputProbs[i];
                maxIndex = i;
            }
        }

        // Create bool[4] (One-Hot Encoding)
        bool[] actions = new bool[4];
        
        // Ensure the index is good
        if (maxIndex >= 0 && maxIndex < actions.Length)
        {
            actions[maxIndex] = true;
        }
    
        GD.Print($"Up: {actions[0]}, Right: {actions[1]}, Down: {actions[2]}, Left: {actions[3]}");
        GD.Print("----------------------------------------");
        
        return actions;
    }

    // Cleanup when the node is destroyed
    public override void _ExitTree()
    {
        _mlp?.Dispose();
    }
}