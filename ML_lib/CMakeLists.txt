cmake_minimum_required(VERSION 4.0)
project(ML_lib)

set(CMAKE_CXX_STANDARD 20)

set(EIGEN_DIR "C:/Libraries/Eigen/eigen-5.0.0")

include_directories(${EIGEN_DIR})

add_library(ML_lib SHARED
        library.hpp
        library.cpp
        PerceptronClassifier.cpp
        PerceptronClassifier.hpp)

set_target_properties(ML_lib PROPERTIES PREFIX "")
