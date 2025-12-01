//
// Created by maxim on 06/11/2025.
//

#include "MLP.hpp"

#include <numeric>
#include <random>

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
    return  this->X[L].segment(1, X[L].size()-1); // return output without the bias
}

TrainingResults MLP::train(const Eigen::MatrixXd &X_input, const Eigen::MatrixXd &Y,
                           const int num_iter, const double learning_rate,
                           double train_proportion, int error_list_size) {
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

    // ===== clamp values =====
    if (error_list_size > num_iter) error_list_size = num_iter;
    if (train_proportion > 1) train_proportion = 1.0f;
    if (train_proportion < 0) train_proportion = 0.0f;

    // ===== Split the data =====
    long total_samples = X_input.cols();
    long train_count = (long)(total_samples * train_proportion);
    long test_count = total_samples - train_count;

    // ==== create random index ======
    std::vector<int> indices(total_samples);
    std::iota(indices.begin(), indices.end(), 0);
    std::random_device rd;
    std::mt19937 g(rd());
    std::shuffle(indices.begin(), indices.end(), g);

    // ==== create sub-matrix and associated Y =====
    Eigen::MatrixXd X_train(X_input.rows(), train_count);
    Eigen::MatrixXd Y_train(Y.rows(), train_count);
    Eigen::MatrixXd X_test(X_input.rows(), test_count);
    Eigen::MatrixXd Y_test(Y.rows(), test_count);

    for(long i = 0; i < total_samples; i++) {
        if(i < train_count) {
            X_train.col(i) = X_input.col(indices[i]);
            Y_train.col(i) = Y.col(indices[i]);
        } else {
            X_test.col(i - train_count) = X_input.col(indices[i]);
            Y_test.col(i - train_count) = Y.col(indices[i]);
        }
    }

    // setup other variables
    Eigen::VectorXd train_error_list = Eigen::VectorXd::Zero(error_list_size);
    Eigen::VectorXd test_error_list = Eigen::VectorXd::Zero(error_list_size);

    double MSE_cumul = 0.0;
    int error_idx = 0;
    int modulo = num_iter / error_list_size;
    if (modulo == 0) modulo = 1;

    // One vector without bias, one with bias for updates
    Eigen::VectorXd X_k(X_input.rows());
    Eigen::VectorXd Y_k_bias = Eigen::VectorXd::Ones(Y.rows() + 1);

    for (int i = 0; i < num_iter; i++) {
        const int randomIndex = rand() % train_count;

        X_k = X_train.col(randomIndex); // get a random example

        Y_k_bias.tail(Y.rows()) = Y_train.col(randomIndex);

        propagate(X_k);

        deltas[L] = this->X[L] - Y_k_bias;

        MSE_cumul += deltas[L].array().square().mean();

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

        // get the error
        if ((i + 1) % modulo == 0 && error_idx < error_list_size) {
            train_error_list(error_idx) = MSE_cumul / modulo;
            MSE_cumul = 0.0;

            // update the test error
            if (test_count > 0) {
                double MSE_cumul_test = 0.0;
                // We iterate over the test set to get an accurate metric
                for(int t = 0; t < test_count; t++) {
                    Eigen::VectorXd X_test_sample = X_test.col(t);
                    Eigen::VectorXd Y_test_target = Eigen::VectorXd::Ones(Y.rows() + 1);
                    Y_test_target.tail(Y.rows()) = Y_test.col(t);

                    propagate(X_test_sample);

                    Eigen::VectorXd diff = this->X[L] - Y_test_target;
                    MSE_cumul_test += diff.array().square().mean();
                }
                test_error_list(error_idx) = MSE_cumul_test / test_count;
            } else {
                test_error_list(error_idx) = 0.0;
            }

            error_idx++;
        }
    }

    // // Write leftover if exists
    // if (MSE_cumul > 0 && error_idx < error_list_size) {
    //     const int remaining_samples = num_iter - error_idx * modulo;
    //     train_error_list(error_idx) = MSE_cumul / remaining_samples;
    // }

    return {train_error_list, test_error_list};
}

void MLP::save(const std::string &filepath) const {
    std::ofstream out(filepath, std::ios::binary | std::ios::trunc); // Add trunc to force overwrite
    if (!out.is_open()) {
        throw std::runtime_error("Cannot open file for writing: " + filepath);
    }

    out.write(reinterpret_cast<const char*>(&isClassification), sizeof(bool));

    int layers_count = NPL.size();
    out.write(reinterpret_cast<const char*>(&layers_count), sizeof(int));
    out.write(reinterpret_cast<const char*>(NPL.data()), NPL.size() * sizeof(int));

    // Consistent loop: l=1 to L
    for (int l = 1; l <= L; l++) {
        const auto &w = weights[l];
        // Always write, even if empty (though it shouldn't be)
        // This keeps the file stream in sync with the load loop.
        out.write(reinterpret_cast<const char*>(w.data()), w.size() * sizeof(double));
    }

    out.close();
}

void MLP::load(const std::string &filepath) {
    std::ifstream in(filepath, std::ios::binary);
    if (!in.is_open()) {
        throw std::runtime_error("Cannot open file for reading: " + filepath);
    }

    // 1. Read Header
    in.read(reinterpret_cast<char*>(&isClassification), sizeof(bool));

    int layers_count = 0;
    in.read(reinterpret_cast<char*>(&layers_count), sizeof(int));

    // 2. Resize NPL
    this->NPL.resize(layers_count);
    in.read(reinterpret_cast<char*>(this->NPL.data()), layers_count * sizeof(int));

    // 3. Re-initialize Structures
    this->L = layers_count - 1;
    this->weights.clear();
    this->weights.resize(L + 1);
    this->X.clear();
    this->X.resize(L + 1);
    this->deltas.clear();
    this->deltas.resize(L + 1);

    // 4. Resize and Read Matrices
    for (int l = 0; l <= L; l++) {
        // --- Init X and Deltas ---
        // FIX: Match Constructor logic. Always size NPL(l) + 1 for bias.
        int neuron_count = NPL(l) + 1;

        this->X[l] = Eigen::VectorXd::Zero(neuron_count);
        this->X[l](0) = 1.0; // FIX: Bias is at index 0, not at the end.

        this->deltas[l] = Eigen::VectorXd::Zero(neuron_count);

        // --- Init Weights (ONLY if l > 0) ---
        if (l > 0) {
            // FIX: Correct dimensions matching the constructor
            int rows = NPL(l - 1) + 1; // Previous Layer + Bias
            int cols = NPL(l) + 1;     // Current Layer + Bias

            this->weights[l] = Eigen::MatrixXd::Zero(rows, cols);

            // Read the data
            in.read(reinterpret_cast<char*>(this->weights[l].data()), rows * cols * sizeof(double));
        }
    }

    in.close();
}
