cmake_minimum_required(VERSION 4.0)
project(ML_lib)

set(CMAKE_CXX_STANDARD 20)

set(EIGEN_DIR "C:/Libraries/Eigen/eigen-5.0.0")

include_directories(${EIGEN_DIR})

add_library(ML_lib SHARED
        library.hpp
        library.cpp
        LinearModel.cpp
        LinearModel.hpp
        MLP.cpp
        MLP.hpp
)

set_target_properties(ML_lib PROPERTIES PREFIX "")

add_executable(PerceptronDebug
        main.cpp
        main.hpp
        LinearModel.cpp
        LinearModel.hpp
        MLP.cpp
        MLP.hpp
)
target_include_directories(PerceptronDebug PRIVATE ${CMAKE_SOURCE_DIR})
#target_link_libraries(PerceptronDebug EIGEN_DIR)