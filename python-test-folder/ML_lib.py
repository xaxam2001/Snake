import ctypes
import numpy as np
import os

dll_path = "C:/Users/maxim/Godot Games/Snake/ML_lib/cmake-build-debug/ML_lib.dll"
dll_dir = "C:/Users/maxim/Godot Games/Snake/ML_lib/cmake-build-debug"

os.add_dll_directory(dll_dir)

lib = ctypes.CDLL(dll_path)

# lib.my_add.argtypes = [ctypes.c_int32, ctypes.c_int32]
# lib.my_add.restype = ctypes.c_int32

lib.create_perceptron_model.argtypes = [ctypes.c_int32] 
lib.create_perceptron_model.restype = ctypes.c_void_p 

lib.get_input_size.argtypes = [ctypes.c_void_p] 
lib.get_input_size.restype = ctypes.c_int32

lib.get_weights_data.argtypes = [ctypes.c_void_p]
lib.get_weights_data.restype =  ctypes.POINTER(ctypes.c_double)

lib.get_weights_rows.argtypes = [ctypes.c_void_p]
lib.get_weights_rows.restype =  ctypes.c_int32

lib.get_weights_cols.argtypes = [ctypes.c_void_p]
lib.get_weights_cols.restype =  ctypes.c_int32

lib.predict_one.argtypes = [
    ctypes.c_void_p,                  # model
    ctypes.POINTER(ctypes.c_double),  # X_data
    ctypes.c_int32                    # size
]
lib.predict_one.restype = ctypes.c_double

lib.predict_batch_flat.argtypes = [
    ctypes.c_void_p,                      # model
    ctypes.POINTER(ctypes.c_double),      # X_data
    ctypes.c_int32,                         # rows
    ctypes.c_int32,                         # cols
    ctypes.POINTER(ctypes.POINTER(ctypes.c_double)), # out_data (output)
    ctypes.POINTER(ctypes.c_int32),         # out_rows (output)
    ctypes.POINTER(ctypes.c_int32)          # out_cols (output)
]
lib.predict_batch_flat.restype = None

lib.free_buffer.argtypes = [ctypes.POINTER(ctypes.c_double)]
lib.free_buffer.restype = None

lib.train.argtypes = [
    ctypes.c_void_p,                      # model
    ctypes.POINTER(ctypes.c_double),      # X_data
    ctypes.c_int32,                         # X_rows
    ctypes.c_int32,                         # X_cols
    ctypes.POINTER(ctypes.c_double),      # Y_data
    ctypes.c_int32,                         # Y_rows
    ctypes.c_int32,                         # Y_cols
    ctypes.POINTER(ctypes.c_int32),         # out_size (output)
    ctypes.c_int32,                         # epochs
    ctypes.c_float                        # learning_rate
]
lib.train.restype = ctypes.POINTER(ctypes.c_double) # Returns pointer to error buffer

lib.release_perceptron_model.argtypes = [ctypes.c_void_p]
lib.release_perceptron_model.restype = None

# lib.sum_array.argtypes = [ctypes.POINTER(ctypes.c_float), ctypes.c_int32] 
# lib.sum_array.restype = ctypes.c_float 

# lib.get_array_of_incrementing_numbers.argtypes = [ctypes.c_int32] 
# lib.get_array_of_incrementing_numbers.restype = ctypes.POINTER(ctypes.c_float) 

# lib.delete_array.argtypes = [ctypes.POINTER(ctypes.c_float), ctypes.c_int32] 
# lib.delete_array.restype = None

# def my_add(a: int, b: int) -> int:
#     return lib.my_add(a, b)

def create_perceptron_model(input_size: float) -> ctypes.c_void_p:
    return lib.create_perceptron_model(input_size)

def predict_linear_model(model: ctypes.c_void_p) -> float:
    return lib.predict_linear_model(model)

def release_perceptron_model(model: ctypes.c_void_p) -> None:
    lib.release_perceptron_model(model)

# def sum_array(arr: np.ndarray) -> float:
#     float_array_pointer = np.ctypeslib.as_ctypes(arr)
#     return lib.sum_array(float_array_pointer, len(arr))

# def get_array_of_incrementing_numbers(size: int):
#     arr_ptr = lib.get_array_of_incrementing_numbers(size)
#     return arr_ptr

# def delete_array(arr_ptr: np.ndarray, length: int) -> None:
#     lib.delete_array(arr_ptr, length)