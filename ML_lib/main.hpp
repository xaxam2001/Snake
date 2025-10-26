#pragma once
#include <iostream>
#include <Eigen/Dense>
#include "LinearModel.hpp"

int main() {
    // Simple toy dataset
    Eigen::MatrixXd X(3, 2);
    X << 1, 1,
         2, 2,
         3, 1;

    Eigen::MatrixXd Y(3, 1);
    Y << 2,
         3,
         2.5;

    std::cout << "Input matrix X:\n" << X << std::endl;
    std::cout << "Labels Y:\n" << Y << std::endl;

    // ===== Regressor model test ======
    LinearRegressor model(2);
    model.train(X, Y);

    std::cout << "weights: " << *model.get_weights() << std::endl;
    // Test predictions
    std::cout << "Predictions:" << std::endl;
    for (int i = 0; i < 7; ++i) {
        Eigen::RowVectorXd Xi(2);
        Xi << i, i-2;
        double pred = model.predict(Xi);
        std::cout << Xi << " => " << pred << std::endl;
    }

    // ===== Perceptron model test =====
    // PerceptronClassifier model(2);
    //
    // // Train
    // std::cout << "Training..." << std::endl;
    // Eigen::VectorXd error_history = model.train(X, Y, 1000, 0.01f);
    //
    // std::cout << "Error history:\n" << error_history.transpose() << std::endl;
    //
    // // Test predictions
    // std::cout << "Predictions:" << std::endl;
    // for (int i = 0; i < X.rows(); ++i) {
    //     double pred = model.predict(Eigen::RowVectorXd(X.row(i)));
    //     std::cout << X.row(i) << " => " << pred << std::endl;
    // }

    return 0;
}
