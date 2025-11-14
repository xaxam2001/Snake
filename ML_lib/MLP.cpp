//
// Created by maxim on 06/11/2025.
//

#include "MLP.hpp"

const Eigen::VectorXi* MLP::get_neuron_per_layer() const {
    return &NPL;
}

int MLP::get_input_size() const {
    return NPL(0);
}

int MLP::get_output_size() const {
    return NPL(L);
}

const std::vector<Eigen::MatrixXd>* MLP::get_weights() const {
    return &weights;
}

MLP::MLP(const Eigen::VectorXi &NPL, const bool isClassification) {
    this->NPL = NPL;
    this->L = NPL.size() - 1;
    this->isClassification = isClassification;

    this->weights = std::vector<Eigen::MatrixXd>(L+1); // weight of layer l going from neuron i to neuron j: W[l][i][j]
    this->X = std::vector<Eigen::VectorXd>(L+1);
    this->deltas = std::vector<Eigen::VectorXd>(L+1);

    weights[0] = Eigen::MatrixXd(); // no weights for input layer, to be consistent with indexing (W[1] are weights going to layer 1)

    for (int l = 0; l < L+1; l++) {
        if (l != 0)
        {
            weights[l] = Eigen::MatrixXd::Random(this->NPL(l-1)+1, this->NPL(l)+1); // +1 for bias neuron from layer l-1 to layer l
            weights[l].col(0).setZero(); // no weights going to bias neuron so set to 0.0, j == 0 corresponds to bias neuron
        }

        this->X[l] = Eigen::VectorXd::Zero(this->NPL(l)+1); // +1 for bias neuron
        this->X[l](0) = 1.0; // bias input
        this->deltas[l] = Eigen::VectorXd::Zero(this->NPL(l)+1); // +1 for bias neuron
    }
}

void MLP::propagate(const Eigen::VectorXd &X_input) {
    // ensure that the X size matches the MLP input size
    if (X_input.size() != this->NPL(0)) {
        throw std::runtime_error(
            "MLP::train, X_input.size doesn't match the size of the MLP input"
            "\nGot X_input.size(): " + std::to_string(X_input.size()) +
            "\nExpected: " + std::to_string(this->NPL(0))
        );
    }

    this->X[0].tail(X_input.size()) = X_input; // update the X row vector for input layer (keeping bias neuron)

    // update all layers until output
    for (int l = 1; l <= L; l++) {
        Eigen::VectorXd signal = this->X[l-1].transpose() * weights[l];

        if (this->isClassification || l != L) {
            this->X[l] = signal.array().tanh();
        }
        else {
            this->X[l] = signal;
        }
    }
}

Eigen::VectorXd MLP::predict(const Eigen::VectorXd &X_input) {
    propagate(X_input);
    return this->X[L].segment(1, X[L].size()-1); // return output without the bias
}

Eigen::VectorXd MLP::train(const Eigen::MatrixXd &X_input, const Eigen::MatrixXd &Y, const int num_iter, const double learning_rate, int error_list_size) {
    if (X_input.cols() != Y.cols()) {
        throw std::runtime_error(
            "MLP::train, X_input.cols doesn't match the number of Y.cols "
            "\nX_input.cols(): " + std::to_string(X_input.cols()) +
            "\nY.cols(): " + std::to_string(Y.cols())
        );
    }

    if (num_iter <= 0) {
        throw std::runtime_error(
            "MLP::train, num_iter must be > 0"
        );
    }

    if (error_list_size > num_iter) error_list_size = num_iter;

    Eigen::VectorXd error_list = Eigen::VectorXd::Zero(error_list_size);
    double MSE_cumul = 0.0;
    int error_idx = 0;
    int modulo = num_iter / error_list_size;
    if (modulo == 0) modulo = 1;

    // One vector without bias, one with bias for updates
    Eigen::VectorXd X_k(X_input.rows());

    Eigen::VectorXd Y_k_bias = Eigen::VectorXd::Ones(Y.rows() + 1);

    for (int i = 0; i < num_iter; i++) {
        const int randomIndex = rand() % X_input.cols();

        X_k = X_input.col(randomIndex); // get a random example

        Y_k_bias.tail(Y.rows()) = Y.col(randomIndex);

        propagate(X_k);

        deltas[L] = this->X[L] - Y_k_bias;

        MSE_cumul += deltas[L].array().square().mean();
        if ((i + 1) % modulo == 0) {
            error_list(error_idx++) = MSE_cumul / modulo;
            MSE_cumul = 0.0;
        }

        if (this->isClassification) {
            deltas[L] = deltas[L].array() * (1.0 - this->X[L].array().square()).array();
        }

        // get the deltas
        for (int l = L; l >= 2; l--) {
            deltas[l-1] = (1.0 - this->X[l-1].array().square()).matrix().cwiseProduct(weights[l] * deltas[l]); // (1 - X[l-1]**2) * (W[l] @ deltas[l])
        }

        // update the weights
        for (int l = 1; l <= L; l++) {
            weights[l] -= learning_rate * (this->X[l-1] * deltas[l].transpose());
        }
    }

    // Write leftover if exists
    if (MSE_cumul > 0 && error_idx < error_list_size) {
        const int remaining_samples = num_iter - error_idx * modulo;
        error_list(error_idx) = MSE_cumul / remaining_samples;
    }

    return error_list;
}
