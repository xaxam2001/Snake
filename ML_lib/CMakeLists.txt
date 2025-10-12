cmake_minimum_required(VERSION 4.0)
project(ML_lib)

set(CMAKE_CXX_STANDARD 20)

add_library(ML_lib SHARED
        library.hpp
        library.cpp
        LinearModel.cpp
        LinearModel.h)

set_target_properties(ML_lib PROPERTIES PREFIX "")
