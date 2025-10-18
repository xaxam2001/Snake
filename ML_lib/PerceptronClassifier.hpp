//
// Created by Maxime Chalumeau on 12/10/2025.
//

#ifndef ML_LIB_LINEARMODEL_H
#define ML_LIB_LINEARMODEL_H

#include <Eigen/Dense>

class PerceptronClassifier {
    int input_size;
    Eigen::MatrixXd weights;

public:
    explicit PerceptronClassifier(int input_size);

    int get_input_size() const;

    const Eigen::MatrixXd* get_weights() const;

    double predict(const Eigen::RowVectorXd& X) const;

    Eigen::MatrixXd predict(const Eigen::MatrixXd& X) const;

    Eigen::VectorXd train(const Eigen::MatrixXd& X, const Eigen::MatrixXd& Y, const int epochs=1000, const float learning_rate=0.01);
};


#endif //ML_LIB_LINEARMODEL_H