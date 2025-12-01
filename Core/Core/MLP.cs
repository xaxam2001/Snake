using System.Runtime.InteropServices;

public class MLP : IDisposable
{
    private IntPtr _modelPtr;

    public MLP(int[] layers, bool isClassification = true)
    {
        _modelPtr = NativeMLP.create_mlp(layers, layers.Length, isClassification);
    }

    private MLP(IntPtr ptr)
    {
        _modelPtr = ptr;
    }

    public static MLP Load(string path)
    {
        IntPtr ptr = NativeMLP.load_mlp_model(path);
        if (ptr == IntPtr.Zero) 
            throw new Exception($"Failed to load model from {path}. Check file path or format.");
        
        return new MLP(ptr);
    }

    public void Save(string path)
    {
        CheckDisposed();
        NativeMLP.save_mlp_model(_modelPtr, path);
    }

    public double[] Predict(double[] input)
    {
        CheckDisposed();

        NativeMLP.predict_one_mlp(
            _modelPtr, 
            input, 
            input.Length, 
            out IntPtr resultPtr, 
            out int resultSize
        );

        // Marshal data from C++ to C#
        double[] result = new double[resultSize];
        Marshal.Copy(resultPtr, result, 0, resultSize);

        // Free C++ memory
        NativeMLP.free_buffer(resultPtr);

        return result;
    }

    public (double[] TrainErrors, double[] TestErrors) Train(
        double[] X_flat, int rows, int cols, 
        double[] Y_flat, int y_rows, int y_cols,
        int numIter = 1000, float lr = 0.01f, double trainSplit = 0.8, int listSize = 100)
    {
        CheckDisposed();

        NativeMLP.train_mlp(
            _modelPtr,
            X_flat, rows, cols,
            Y_flat, y_rows, y_cols,
            out IntPtr ptrTrain, out int sizeTrain,
            out IntPtr ptrTest, out int sizeTest,
            numIter, lr, trainSplit, listSize
        );

        double[] trainRes = new double[sizeTrain];
        Marshal.Copy(ptrTrain, trainRes, 0, sizeTrain);

        double[] testRes = new double[sizeTest];
        Marshal.Copy(ptrTest, testRes, 0, sizeTest);

        NativeMLP.free_buffer(ptrTrain);
        NativeMLP.free_buffer(ptrTest);

        return (trainRes, testRes);
    }

    public void Dispose()
    {
        if (_modelPtr != IntPtr.Zero)
        {
            NativeMLP.release_mlp(_modelPtr);
            _modelPtr = IntPtr.Zero;
        }
    }

    private void CheckDisposed()
    {
        if (_modelPtr == IntPtr.Zero) 
            throw new ObjectDisposedException("MLP model has been disposed.");
    }
}