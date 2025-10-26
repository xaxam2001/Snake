//
// Created by Maxime Chalumeau on 12/10/2025.
//

#include "LinearModel.hpp"
#include <cstdlib>
#include <ctime>

LinearModel::LinearModel(const int input_size) {
    this->input_size = input_size ;
    weights = Eigen::MatrixXd::Random(input_size + 1, 1);
}

int LinearModel::get_input_size() const {
    return input_size;
}

const Eigen::MatrixXd* LinearModel::get_weights() const {
    return &weights;
}

Eigen::MatrixXd PerceptronClassifier::predict(const Eigen::MatrixXd& X) const {
    if (X.cols() != weights.rows() - 1) {
        throw std::runtime_error(
            "PerceptronClassifier::predict: Input X has wrong number of columns. "
            "Expected " + std::to_string(weights.rows() - 1) +
            ", got " + std::to_string(X.cols())
        );
    }

    // X is NxM with N samples and M = input_size
    Eigen::MatrixXd X_bias = Eigen::MatrixXd(X.rows(), X.cols() + 1); // we add one for the bias
    X_bias << Eigen::MatrixXd::Ones(X.rows(), 1), X; // adding a column of one in the first column of X_bias

    Eigen::MatrixXd prediction = X_bias * weights; // do the X*W

    prediction = (prediction.array() >= 0.0).cast<double>(); // convert value into 1 and 0 (1 if >=0 0 otherwise)

    prediction = (prediction * 2.0).array() - 1.0; // make it range from -1 to 1

    return prediction;
}

double PerceptronClassifier::predict(const Eigen::RowVectorXd& X) const {
    if (X.size() != weights.rows() - 1) {
        throw std::runtime_error(
            "PerceptronClassifier::predict: Input X has wrong number of columns. "
            "Expected " + std::to_string(weights.rows() - 1) +
            ", got " + std::to_string(X.cols())
        );
    }

    return predict(Eigen::MatrixXd(X))(0,0);
}

Eigen::VectorXd PerceptronClassifier::train(const Eigen::MatrixXd& X, const Eigen::MatrixXd& Y, const int epochs, const float learning_rate) {
    double mean_squared_error = 0.0;
    Eigen::VectorXd error_list = Eigen::VectorXd::Zero(epochs/100 - 1);

    // One vector without bias, one with bias for updates
    Eigen::RowVectorXd X_k(X.cols());
    Eigen::RowVectorXd X_k_bias = Eigen::RowVectorXd::Ones(X.cols() + 1);

    srand(static_cast<unsigned>(time(nullptr)));

    for (int i = 0; i < epochs; i++) {
        const int randomIndex = rand() % X.rows();

        // Extract the random sample (no bias)
        X_k = X.row(randomIndex);

        // Prepare the bias-augmented vector for weight updates
        X_k_bias.tail(X.cols()) = X_k;

        // get the corresponding class (ground truth
        const double Y_k = Y(randomIndex, 0);

        // predict the value of the random element X_k
        const double g_x_k = predict(X_k);

        // Update the weights using the Rosenblatt rule
        weights += learning_rate * (Y_k - g_x_k) * X_k_bias.transpose();

        // compute the mean square error
        mean_squared_error += (Y_k - g_x_k) * (Y_k - g_x_k);

        if (i % 100 == 0 && i != 0) {
            error_list[i/100 - 1] = mean_squared_error / 100.0;
            mean_squared_error = 0.0;
        }
    }

    return error_list;
}

Eigen::MatrixXd LinearRegressor::predict(const Eigen::MatrixXd& X) const {
    if (X.cols() != weights.rows() - 1) {
        throw std::runtime_error(
            "LinearRegressor::predict: Input X has wrong number of columns. "
            "Expected " + std::to_string(weights.rows() - 1) +
            ", got " + std::to_string(X.cols())
        );
    }

    // X is NxM with N samples and M = input_size
    Eigen::MatrixXd X_bias = Eigen::MatrixXd(X.rows(), X.cols() + 1); // we add one for the bias
    X_bias << Eigen::MatrixXd::Ones(X.rows(), 1), X; // adding a column of one in the first column of X_bias

    Eigen::MatrixXd prediction = X_bias * weights; // do the X*W

    return prediction;
}

double LinearRegressor::predict(const Eigen::RowVectorXd& X) const {
    if (X.size() != weights.rows() - 1) {
        throw std::runtime_error(
            "LinearRegressor::predict: Input X has wrong number of columns. "
            "Expected " + std::to_string(weights.rows() - 1) +
            ", got " + std::to_string(X.cols())
        );
    }

    return predict(Eigen::MatrixXd(X))(0,0);
}

void LinearRegressor::train(const Eigen::MatrixXd &X, const Eigen::MatrixXd &Y) {
    // X is NxM with N samples and M = input_size
    Eigen::MatrixXd X_bias = Eigen::MatrixXd(X.rows(), X.cols() + 1); // we add one for the bias
    X_bias << Eigen::MatrixXd::Ones(X.rows(), 1), X; // adding a column of one in the first column of X_bias

    // use formula ((X^T * X)^-1 * X^T)*Y to compute the weights
    weights = X_bias.colPivHouseholderQr().solve(Y);
}

