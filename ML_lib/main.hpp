#include <iostream>
#include <Eigen/Dense>
#include "PerceptronClassifier.hpp"  // adjust the include path as needed

int main() {
    // Simple toy dataset: AND logic gate
    Eigen::MatrixXd X(3, 2);
    X << 1, 1,
         2, 3,
         3, 3;

    Eigen::MatrixXd Y(3, 1);
    Y << 1,
         -1,
         -1;

    std::cout << "Input matrix X:\n" << X << std::endl;
    std::cout << "Labels Y:\n" << Y << std::endl;

    // Create model
    PerceptronClassifier model(2);

    // Train
    std::cout << "Training..." << std::endl;
    Eigen::VectorXd error_history = model.train(X, Y, 400, 0.01f);

    std::cout << "Error history:\n" << error_history.transpose() << std::endl;

    // Test predictions
    std::cout << "Predictions:" << std::endl;
    for (int i = 0; i < X.rows(); ++i) {
        double pred = model.predict(Eigen::RowVectorXd(X.row(i)));
        std::cout << X.row(i) << " => " << pred << std::endl;
    }

    return 0;
}
