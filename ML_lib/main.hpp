#pragma once
#include <iostream>
#include <Eigen/Dense>
#include "LinearModel.hpp"
#include "MLP.hpp"

int main() {
    // Simple toy dataset
    Eigen::MatrixXd X(3, 2);
    X << 1, 1,
         2, 2,
         3, 1;

    Eigen::MatrixXd Y(3, 1);
    Y << 1,
         1,
         -1;

    std::cout << "Input matrix X:\n" << X << std::endl;
    std::cout << "Labels Y:\n" << Y << std::endl;

    // ===== MLP model tests ======
    Eigen::VectorXi NPL(3);
    NPL << 2, 2, 1;
    MLP mlp(NPL, true);

    for (int l = 0; l < NPL.size(); ++l) {
        std::cout << "weights: " << (*mlp.get_weights())[l] << std::endl;
        std::cout << "X: " << (*mlp.get_X())[l] << std::endl;
        std::cout << "deltas: " << (*mlp.get_deltas())[l] << std::endl;
    }

    Eigen::RowVectorXd error_list = mlp.train(X.transpose(), Y.transpose(), 1000, 0.1, 10);

    std::cout << "error list: " << error_list << std::endl;

    for (int i = 0; i < X.rows(); i++) {
        Eigen::VectorXd x_sample = X.row(i); // take the i-th training sample
        Eigen::VectorXd y_pred = mlp.predict(x_sample);
        std::cout << "Input: " << x_sample << std::endl
                  << "Prediction: " << y_pred << std::endl;
    }

    // ===== Regressor model test ======
    // LinearRegressor model(2);
    // model.train(X, Y);
    //
    // std::cout << "weights: " << *model.get_weights() << std::endl;
    // // Test predictions
    // std::cout << "Predictions:" << std::endl;
    // for (int i = 0; i < 7; ++i) {
    //     Eigen::RowVectorXd Xi(2);
    //     Xi << i, i-2;
    //     double pred = model.predict(Xi);
    //     std::cout << Xi << " => " << pred << std::endl;
    // }

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
