//
// Created by maxim on 06/11/2025.
//

#ifndef ML_LIB_MLP_H
#define ML_LIB_MLP_H

#include <Eigen/Dense>

class MLP {
    Eigen::VectorXi NPL; // Neuron per layer
    int L; // last layer index
    bool isClassification;
    std::vector<Eigen::MatrixXd> weights;
    std::vector<Eigen::VectorXd> X;
    std::vector<Eigen::VectorXd> deltas;

    void propagate(const Eigen::VectorXd& X_input);

public:
    explicit MLP(const Eigen::VectorXi &NPL, const bool isClassification = true);
    ~MLP() = default;

    [[nodiscard]] const Eigen::VectorXi* get_neuron_per_layer() const;
    [[nodiscard]] int get_input_size() const;
    [[nodiscard]] int get_output_size() const;
    [[nodiscard]] const std::vector<Eigen::MatrixXd>* get_weights() const;
    [[nodiscard]] const std::vector<Eigen::VectorXd>* get_X() const {
        return &X;
    }
    [[nodiscard]] const std::vector<Eigen::VectorXd>* get_deltas() const {
        return &deltas;
    }

    [[nodiscard]] Eigen::VectorXd predict(const Eigen::VectorXd& X_input);
    [[nodiscard]] Eigen::MatrixXd predict(const Eigen::MatrixXd& X_input);

    [[nodiscard]] Eigen::VectorXd train(const Eigen::MatrixXd& X_input, const Eigen::MatrixXd& Y, int num_iter, double learning_rate, int error_list_size);
};


#endif //ML_LIB_MLP_H