using System;
using Godot;
using System.Linq;
using System.Runtime.InteropServices;

public partial class AIPlayerManager : Node
{
    [Export] private string modelPath;
    
    private MLP _mlp;
    private static IntPtr _dllHandle = IntPtr.Zero;
    
    public override void _Ready()
    {
        // 1. MANUALLY LOAD THE DLL
        // We only need to do this once per game session
        if (_dllHandle == IntPtr.Zero)
        {
            try 
            {
                // Convert "res://ML_lib.dll" to a real OS path (e.g., C:\Users\...\ML_lib.dll)
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

        // 2. Load the Model (Standard logic)
        if (!string.IsNullOrEmpty(modelPath))
        {
            string globalPath = ProjectSettings.GlobalizePath(modelPath);
            // Now this call to Core will work because the DLL is already loaded!
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

        double[] inputs = gameState
            .Skip(1)                  // Skip the first element (GameOver flag)
            .Select(x => (double)x)   // Convert int to double
            .ToArray();

        // Predict
        double[] outputProbs = _mlp.Predict(inputs);
        
        GD.Print($"Model Output Size: {outputProbs.Length}");
        GD.Print($"0: {outputProbs[0]} 1: {outputProbs[1]} 2: {outputProbs[2]} 3: {outputProbs[3]}");
        
        // Find ArgMax (Index of the highest value)
        int maxIndex = 0;
        double maxValue = outputProbs[0];

        for (int i = 1; i < outputProbs.Length; i++)
        {
            if (outputProbs[i] > maxValue)
            {
                maxValue = outputProbs[i];
                maxIndex = i;
            }
        }
        
        GD.Print($"Max: {maxIndex}");
        GD.Print($"Max Value: {maxValue}");
        
        
        // Create bool[4] (One-Hot Encoding)
        bool[] actions = new bool[4];
        
        // Ensure the index is within bounds (safety check)
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