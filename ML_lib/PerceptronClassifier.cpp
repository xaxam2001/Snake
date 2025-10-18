//
// Created by Maxime Chalumeau on 12/10/2025.
//

#include "PerceptronClassifier.hpp"
#include <stdlib.h>
#include <time.h>

PerceptronClassifier::PerceptronClassifier(const int input_size) {
    this->input_size = input_size ;
    weights = Eigen::MatrixXd(input_size + 1, 1);
}

Eigen::MatrixXd PerceptronClassifier::predict(const Eigen::MatrixXd& X) const {
    // X is NxM with N samples and M = input_size
    Eigen::MatrixXd X_bias = Eigen::MatrixXd(X.rows(), X.cols() + 1); // we add one for the bias
    X_bias << Eigen::MatrixXd::Ones(X.rows(), 1), X; // adding a column of one in the first column of X_bias

    Eigen::MatrixXd prediction = X_bias * weights; // do the X*W

    prediction = (prediction.array() >= 0.0).cast<double>(); // convert value into 1 and 0 (1 if >=0 0 otherwise)

    prediction = (prediction * 2.0).array() - 1.0; // make it range from -1 to 1

    return prediction;
}

double PerceptronClassifier::predict(const Eigen::RowVectorXd& X) const {
    return predict(Eigen::MatrixXd(X))(0,0);
}

Eigen::VectorXd PerceptronClassifier::train(const Eigen::MatrixXd& X, const Eigen::MatrixXd& Y, const int epochs, const float learning_rate) {
    double mean_squared_error = 0.0;
    Eigen::VectorXd error_list = Eigen::VectorXd::Zero(epochs/100);

    Eigen::RowVectorXd X_k = Eigen::RowVectorXd::Ones(X.cols() + 1);

    srand( time(nullptr));

    for (int i = 0; i < epochs; i++) {
        int randomIndex = rand() % X.rows();

        // get the random element as a colum vector with a one in first position
        X_k.block(1, 0, X.cols(), 1) = X.row(randomIndex).transpose();

        // get the corresponding class (ground truth
        double Y_k = Y(randomIndex, 0);

        // predict the value of the random element X_k
        double g_x_k = predict(X_k);

        // Update the weights using the Rosenblatt rule
        weights += learning_rate * (Y_k - g_x_k) * X_k.transpose();

        // compute the mean square error
        mean_squared_error += (Y_k - g_x_k) * (Y_k - g_x_k);

        if (i % 100 == 0 && i != 0) {
            error_list[i/100] = mean_squared_error / 100.0;
            mean_squared_error = 0.0;
        }
    }

    return error_list;
}

int PerceptronClassifier::get_input_size() const {
    return input_size;
}

const Eigen::MatrixXd* PerceptronClassifier::get_weights() const {
    return &weights;
}


