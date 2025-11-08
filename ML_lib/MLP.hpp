//
// Created by maxim on 06/11/2025.
//

#ifndef ML_LIB_MLP_H
#define ML_LIB_MLP_H

#include <Eigen/Dense>

class MLP {
private:
    Eigen::MatrixXd NPL; // Neuron per layer
    int L; // last layer index
    bool isClassification;
    Eigen::MatrixXd weights;

public:
    explicit MLP(Eigen::MatrixXd NPL, bool isClassification = true);
    ~MLP() = default;

    [[nodiscard]] const Eigen::MatrixXd* get_neuron_per_layer() const;
    [[nodiscard]] int get_input_size() const;
    [[nodiscard]] int get_output_size() const;
    [[nodiscard]] const Eigen::MatrixXd* get_weights() const;

    // TODO: add missing methods
};


#endif //ML_LIB_MLP_H