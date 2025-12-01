using System.Runtime.InteropServices;

internal static class NativeMLP
{
    private const string DllPath = "ML_lib.dll"; 

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr create_mlp(int[] layers, int length, bool isClassification);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern void release_mlp(IntPtr model);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern void save_mlp_model(IntPtr model, [MarshalAs(UnmanagedType.LPStr)] string filepath);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern IntPtr load_mlp_model([MarshalAs(UnmanagedType.LPStr)] string filepath);

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern void predict_one_mlp(
        IntPtr model, 
        double[] sample_input, 
        int sample_size, 
        out IntPtr out_result, 
        out int out_count
    );

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern void train_mlp(
        IntPtr model,
        double[] X, int X_rows, int X_cols,
        double[] Y, int Y_rows, int Y_cols,
        out IntPtr out_train_errors, out int out_train_size,
        out IntPtr out_test_errors, out int out_test_size,
        int num_iter,
        float learning_rate,
        double train_proportion,
        int error_list_size
    );

    [DllImport(DllPath, CallingConvention = CallingConvention.Cdecl)]
    public static extern void free_buffer(IntPtr ptr);
}
