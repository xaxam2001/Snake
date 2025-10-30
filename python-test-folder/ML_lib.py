import ctypes
import numpy as np
import os

dll_path = "C:/Users/maxim/Godot Games/Snake/ML_lib/cmake-build-release/ML_lib.dll"
dll_dir = "C:/Users/maxim/Godot Games/Snake/ML_lib/cmake-build-release"

os.add_dll_directory(dll_dir)

lib = ctypes.CDLL(dll_path)

lib.get_input_size.argtypes = [ctypes.c_void_p] 
lib.get_input_size.restype = ctypes.c_int32

lib.get_weights_flat.argtypes = [
    ctypes.c_void_p,
    ctypes.POINTER(ctypes.POINTER(ctypes.c_double)),  # out_data
    ctypes.POINTER(ctypes.c_int32),                   # out_rows
    ctypes.POINTER(ctypes.c_int32)                    # out_cols
]
lib.get_weights_flat.restype = None

lib.free_buffer.argtypes = [ctypes.POINTER(ctypes.c_double)]
lib.free_buffer.restype = None

lib.create_perceptron_model.argtypes = [ctypes.c_int32] 
lib.create_perceptron_model.restype = ctypes.c_void_p 

lib.predict_one_perceptron_model.argtypes = [
    ctypes.c_void_p,                  # model
    ctypes.POINTER(ctypes.c_double),  # X_data
    ctypes.c_int32                    # size
]
lib.predict_one_perceptron_model.restype = ctypes.c_double

lib.predict_batch_flat_perceptron_model.argtypes = [
    ctypes.c_void_p,                      # model
    ctypes.POINTER(ctypes.c_double),      # X_data
    ctypes.c_int32,                         # rows
    ctypes.c_int32,                         # cols
    ctypes.POINTER(ctypes.POINTER(ctypes.c_double)), # out_data (output)
    ctypes.POINTER(ctypes.c_int32),         # out_rows (output)
    ctypes.POINTER(ctypes.c_int32)          # out_cols (output)
]
lib.predict_batch_flat_perceptron_model.restype = None

lib.train_perceptron_model.argtypes = [
    ctypes.c_void_p,                      # model
    ctypes.POINTER(ctypes.c_double),      # X_data
    ctypes.c_int32, ctypes.c_int32,       # X_rows, X_cols
    ctypes.POINTER(ctypes.c_double),      # Y_data
    ctypes.c_int32, ctypes.c_int32,       # Y_rows, Y_cols
    ctypes.POINTER(ctypes.POINTER(ctypes.c_double)),  # out_error_data
    ctypes.POINTER(ctypes.c_int32),       # out_size
    ctypes.c_int32,                       # epochs
    ctypes.c_float                        # learning_rate
]
lib.train_perceptron_model.restype = None

lib.release_perceptron_model.argtypes = [ctypes.c_void_p]
lib.release_perceptron_model.restype = None

lib.create_linear_regressor.argtypes = [ctypes.c_int32] 
lib.create_linear_regressor.restype = ctypes.c_void_p 

lib.predict_one_linear_regressor.argtypes = [
    ctypes.c_void_p,                  # model
    ctypes.POINTER(ctypes.c_double),  # X_data
    ctypes.c_int32                    # size
]
lib.predict_one_linear_regressor.restype = ctypes.c_double

lib.predict_batch_flat_linear_regressor.argtypes = [
    ctypes.c_void_p,                      # model
    ctypes.POINTER(ctypes.c_double),      # X_data
    ctypes.c_int32,                         # rows
    ctypes.c_int32,                         # cols
    ctypes.POINTER(ctypes.POINTER(ctypes.c_double)), # out_data (output)
    ctypes.POINTER(ctypes.c_int32),         # out_rows (output)
    ctypes.POINTER(ctypes.c_int32)          # out_cols (output)
]
lib.predict_batch_flat_linear_regressor.restype = None

lib.train_linear_regressor.argtypes = [
    ctypes.c_void_p,                      # model
    ctypes.POINTER(ctypes.c_double),      # X_data
    ctypes.c_int32, ctypes.c_int32,       # X_rows, X_cols
    ctypes.POINTER(ctypes.c_double),      # Y_data
    ctypes.c_int32, ctypes.c_int32       # Y_rows, Y_cols
]
lib.train_linear_regressor.restype = None

lib.release_linear_regressor.argtypes = [ctypes.c_void_p]
lib.release_linear_regressor.restype = None

def _to_c_ptr(arr: np.ndarray):
    arr = np.ascontiguousarray(arr, dtype=np.float64)
    return arr.ctypes.data_as(ctypes.POINTER(ctypes.c_double))

class PerceptronModel:

    def __init__(self, input_size: int):
        self.model_ptr = lib.create_perceptron_model(input_size)

    def predict_batch(self, X: np.ndarray) -> float:
        X_ptr = _to_c_ptr(X)
        out_data_ptr = ctypes.POINTER(ctypes.c_double)()
        out_rows = ctypes.c_int32()
        out_cols = ctypes.c_int32()
        lib.predict_batch_flat_perceptron_model(
            self.model_ptr,
            X_ptr,
            X.shape[0],
            X.shape[1],
            ctypes.byref(out_data_ptr),
            ctypes.byref(out_rows),
            ctypes.byref(out_cols)
        )
        out_array = np.ctypeslib.as_array(out_data_ptr, shape=(out_rows.value * out_cols.value,))
        out_copy = np.copy(out_array).reshape((out_rows.value, out_cols.value))
        lib.free_buffer(out_data_ptr)
        return out_copy
    
    def release(self) -> None:
        lib.release_perceptron_model(self.model_ptr)

    def __del__(self):
        if hasattr(self, "model_ptr") and self.model_ptr:
            self.release()

    def get_weights(self) -> np.ndarray:
        weights_ptr = ctypes.POINTER(ctypes.c_double)()
        out_rows = ctypes.c_int32()
        out_cols = ctypes.c_int32()
        lib.get_weights_flat(
            self.model_ptr,
            ctypes.byref(weights_ptr),
            ctypes.byref(out_rows),
            ctypes.byref(out_cols)
        )
        weights_array = np.ctypeslib.as_array(weights_ptr, shape=(out_rows.value * out_cols.value,))
        weights_copy = np.copy(weights_array).reshape((out_rows.value, out_cols.value))
        lib.free_buffer(weights_ptr)
        return weights_copy
    
    def train(self, X: np.ndarray, Y: np.ndarray, epochs: int, learning_rate: float) -> np.ndarray:

        # flatten and get pointers
        X_ptr = _to_c_ptr(X)
        Y_ptr = _to_c_ptr(Y)

        error_ptr = ctypes.POINTER(ctypes.c_double)()
        out_size = ctypes.c_int32()

        if len(Y.shape) == 1:
            Y = Y.reshape((Y.shape[0], 1))

        lib.train_perceptron_model(
            self.model_ptr,
            X_ptr, X.shape[0], X.shape[1],
            Y_ptr, Y.shape[0], Y.shape[1],
            ctypes.byref(error_ptr),
            ctypes.byref(out_size),
            epochs,
            learning_rate
        )

        error_array = np.ctypeslib.as_array(error_ptr, shape=(out_size.value,))
        error_copy = np.copy(error_array)
        lib.free_buffer(error_ptr)
        return error_copy

class LinearRegressor:

    def __init__(self, input_size: int):
        self.model_ptr = lib.create_linear_regressor(input_size)

    def predict_batch(self, X: np.ndarray) -> float:
        X_ptr = _to_c_ptr(X)
        out_data_ptr = ctypes.POINTER(ctypes.c_double)()
        out_rows = ctypes.c_int32()
        out_cols = ctypes.c_int32()
        lib.predict_batch_flat_linear_regressor(
            self.model_ptr,
            X_ptr,
            X.shape[0],
            X.shape[1],
            ctypes.byref(out_data_ptr),
            ctypes.byref(out_rows),
            ctypes.byref(out_cols)
        )
        out_array = np.ctypeslib.as_array(out_data_ptr, shape=(out_rows.value * out_cols.value,))
        out_copy = np.copy(out_array).reshape((out_rows.value, out_cols.value))
        lib.free_buffer(out_data_ptr)
        return out_copy
    
    def release(self) -> None:
        lib.release_linear_regressor(self.model_ptr)

    def __del__(self):
        if hasattr(self, "model_ptr") and self.model_ptr:
            self.release()

    def get_weights(self) -> np.ndarray:
        weights_ptr = ctypes.POINTER(ctypes.c_double)()
        out_rows = ctypes.c_int32()
        out_cols = ctypes.c_int32()
        lib.get_weights_flat(
            self.model_ptr,
            ctypes.byref(weights_ptr),
            ctypes.byref(out_rows),
            ctypes.byref(out_cols)
        )
        weights_array = np.ctypeslib.as_array(weights_ptr, shape=(out_rows.value * out_cols.value,))
        weights_copy = np.copy(weights_array).reshape((out_rows.value, out_cols.value))
        lib.free_buffer(weights_ptr)
        return weights_copy
    
    def train(self, X: np.ndarray, Y: np.ndarray) -> None:

        # flatten and get pointers
        X_ptr = _to_c_ptr(X)
        Y_ptr = _to_c_ptr(Y)

        if len(Y.shape) == 1:
            Y = Y.reshape((Y.shape[0], 1))

        lib.train_linear_regressor(
            self.model_ptr,
            X_ptr, X.shape[0], X.shape[1],
            Y_ptr, Y.shape[0], Y.shape[1],
        )
