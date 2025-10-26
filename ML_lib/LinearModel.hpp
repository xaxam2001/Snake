//
// Created by Maxime Chalumeau on 12/10/2025.
//

#ifndef ML_LIB_LINEARMODEL_H
#define ML_LIB_LINEARMODEL_H

#pragma once
#include <Eigen/Dense>

class LinearModel {
protected:
    int input_size;
    Eigen::MatrixXd weights;

public:
    explicit LinearModel(int input_size);
    virtual ~LinearModel() = default;

    [[nodiscard]] int get_input_size() const;
    [[nodiscard]] const Eigen::MatrixXd* get_weights() const;


    [[nodiscard]] virtual double predict(const Eigen::RowVectorXd& X) const = 0;

    [[nodiscard]] virtual Eigen::MatrixXd predict(const Eigen::MatrixXd& X) const = 0;
};

class PerceptronClassifier final : public LinearModel {
public:
    using LinearModel::LinearModel; // using LinearModel constructor (equivalent to PerceptronClassifier(int input_size) : LinearModel(input_size) {})

    [[nodiscard]] double predict(const Eigen::RowVectorXd& X) const override;
    [[nodiscard]] Eigen::MatrixXd predict(const Eigen::MatrixXd& X) const override;
    Eigen::VectorXd train(const Eigen::MatrixXd& X, const Eigen::MatrixXd& Y, int epochs=1000, float learning_rate=0.01);
};

class LinearRegressor final : public LinearModel {
public:
    using LinearModel::LinearModel; // using LinearModel constructor (equivalent to PerceptronClassifier(int input_size) : LinearModel(input_size) {})

    [[nodiscard]] double predict(const Eigen::RowVectorXd& X) const override;
    [[nodiscard]] Eigen::MatrixXd predict(const Eigen::MatrixXd& X) const override;
    void train(const Eigen::MatrixXd& X, const Eigen::MatrixXd& Y);
};

#endif //ML_LIB_LINEARMODEL_H