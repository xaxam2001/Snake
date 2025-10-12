import ctypes
import numpy as np
import os

dll_path = "C:/Users/maxim/Godot Games/Snake/ML_lib/cmake-build-debug/ML_lib.dll"
dll_dir = "C:/Users/maxim/Godot Games/Snake/ML_lib/cmake-build-debug"

os.add_dll_directory(dll_dir)

lib = ctypes.CDLL(dll_path)

lib.my_add.argtypes = [ctypes.c_int32, ctypes.c_int32]
lib.my_add.restype = ctypes.c_int32

lib.create_linear_model.argtypes = [ctypes.c_float, ctypes.c_float] 
lib.create_linear_model.restype = ctypes.c_void_p 

lib.predict_linear_model.argtypes = [ctypes.c_void_p] 
lib.predict_linear_model.restype = ctypes.c_float 

lib.release_linear_model.argtypes = [ctypes.c_void_p] 
lib.release_linear_model.restype = None 

lib.sum_array.argtypes = [ctypes.POINTER(ctypes.c_float), ctypes.c_int32] 
lib.sum_array.restype = ctypes.c_float 

lib.get_array_of_incrementing_numbers.argtypes = [ctypes.c_int32] 
lib.get_array_of_incrementing_numbers.restype = ctypes.POINTER(ctypes.c_float) 

lib.delete_array.argtypes = [ctypes.POINTER(ctypes.c_float), ctypes.c_int32] 
lib.delete_array.restype = None

def my_add(a: int, b: int) -> int:
    return lib.my_add(a, b)

def create_linear_model(a: float, b: float) -> ctypes.c_void_p:
    return lib.create_linear_model(a, b)

def predict_linear_model(model: ctypes.c_void_p) -> float:
    return lib.predict_linear_model(model)

def release_linear_model(model: ctypes.c_void_p) -> None:
    lib.release_linear_model(model)

def sum_array(arr: np.ndarray) -> float:
    arr = arr.astype(np.float32)  # Ensure the array is of type float32
    return lib.sum_array(arr.ctypes.data_as(ctypes.POINTER(ctypes.c_float)), arr.size)

def get_array_of_incrementing_numbers(size: int) -> np.ndarray:
    arr_ptr = lib.get_array_of_incrementing_numbers(size)
    arr = np.ctypeslib.as_array(arr_ptr, shape=(size,))
    return arr.copy()  # Return a copy to avoid issues after deletion

def delete_array(arr: np.ndarray) -> None:
    arr = arr.astype(np.float32)  # Ensure the array is of type float32
    lib.delete_array(arr.ctypes.data_as(ctypes.POINTER(ctypes.c_float)), arr.size)