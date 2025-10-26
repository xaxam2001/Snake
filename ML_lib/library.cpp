#include "library.hpp"
#include <cstdint>

#if WIN32
#define DLLEXPORT __declspec(dllexport)
#else
#define DLLEXPORT
#endif

using MapMatrixXdRowMajor = Eigen::Map<const Eigen::Matrix<double, Eigen::Dynamic, Eigen::Dynamic, Eigen::RowMajor>>;

extern "C" {

    // ======== LinearModel methods ===============

    DLLEXPORT int32_t get_input_size(const LinearModel *model) {
        return model->get_input_size();
    }

    DLLEXPORT void get_weights_flat(
    const LinearModel* model,
    double** out_data,
    int32_t* out_rows,
    int32_t* out_cols)
    {
        const Eigen::MatrixXd& W = *model->get_weights();
        *out_rows = static_cast<int32_t>(W.rows());
        *out_cols = static_cast<int32_t>(W.cols());

        const int32_t total = static_cast<int32_t>(W.size());
        *out_data = static_cast<double*>(std::malloc(sizeof(double) * total));
        if (*out_data == nullptr) {
            *out_rows = *out_cols = 0;
            return;
        }
        std::memcpy(*out_data, W.data(), sizeof(double) * total);
    }

    DLLEXPORT void free_buffer(double* ptr) {
        std::free(ptr);
    }

    // ============= PerceptronClassifier related method ================

    DLLEXPORT PerceptronClassifier *create_perceptron_model(const int32_t input_size) {
        return new PerceptronClassifier(input_size);
    }

    DLLEXPORT double predict_one_perceptron_model(const PerceptronClassifier *model, const double* X_data, const int32_t size) {
        // Map the input buffer to an Eigen matrix (no copy)
        const Eigen::Map<const Eigen::RowVectorXd> X(X_data, size);

        // Do the prediction
        return model->predict(Eigen::RowVectorXd(X));
    }

    DLLEXPORT void predict_batch_flat_perceptron_model(
    const PerceptronClassifier* model,
    const double* X_data, const int32_t rows, const int32_t cols,
    double** out_data, int32_t* out_rows, int32_t* out_cols)
    {
        // Map the input buffer to an Eigen matrix (no copy)
        const MapMatrixXdRowMajor X(X_data, rows, cols);

        // Do the prediction
        Eigen::MatrixXd pred = model->predict(Eigen::MatrixXd(X));

        // Allocate a flat buffer for the output
        const int32_t total = static_cast<int32_t>(pred.size());
        *out_data = static_cast<double *>(std::malloc(total * sizeof(double)));
        std::memcpy(*out_data, pred.data(), total * sizeof(double));

        *out_rows = static_cast<int32_t>(pred.rows());
        *out_cols = static_cast<int32_t>(pred.cols());
    }

    DLLEXPORT void train_perceptron_model(
        PerceptronClassifier* model,
        const double* X_data, const int32_t X_rows, const int32_t X_cols,
        const double* Y_data, const int32_t Y_rows, const int32_t Y_cols,
        double** out_error_data,  // pointer to error buffer (output)
        int32_t* out_size,        // size of the error array (output)
        const int32_t epochs = 1000,
        const float learning_rate = 0.01f)
    {
        // Map input buffers to Eigen matrices (no copies)
        const MapMatrixXdRowMajor X(X_data, X_rows, X_cols);
        const MapMatrixXdRowMajor Y(Y_data, Y_rows, Y_cols);

        // Train the model and get the error vector
        Eigen::VectorXd error = model->train(Eigen::MatrixXd(X), Eigen::MatrixXd(Y), epochs, learning_rate);

        // Allocate output buffer
        *out_size = static_cast<int32_t>(error.size());
        *out_error_data = static_cast<double*>(std::malloc(error.size() * sizeof(double)));

        // Copy error data into the allocated buffer
        std::memcpy(*out_error_data, error.data(), error.size() * sizeof(double));
    }

    DLLEXPORT void release_perceptron_model(const PerceptronClassifier *model) {
        delete model;
    }

    // ============= LinearRegressor related method ================

    DLLEXPORT LinearRegressor *create_linear_regressor(const int32_t input_size) {
        return new LinearRegressor(input_size);
    }

    DLLEXPORT double predict_one_linear_regressor(const LinearRegressor *model, const double* X_data, const int32_t size) {
        // Map the input buffer to an Eigen matrix (no copy)
        const Eigen::Map<const Eigen::RowVectorXd> X(X_data, size);

        // Do the prediction
        return model->predict(Eigen::RowVectorXd(X));
    }

    DLLEXPORT void predict_batch_flat_linear_regressor(
    const LinearRegressor* model,
    const double* X_data, const int32_t rows, const int32_t cols,
    double** out_data, int32_t* out_rows, int32_t* out_cols)
    {
        // Map the input buffer to an Eigen matrix (no copy)
        const MapMatrixXdRowMajor X(X_data, rows, cols);

        // Do the prediction
        Eigen::MatrixXd pred = model->predict(Eigen::MatrixXd(X));

        // Allocate a flat buffer for the output
        const int32_t total = static_cast<int32_t>(pred.size());
        *out_data = static_cast<double *>(std::malloc(total * sizeof(double)));
        std::memcpy(*out_data, pred.data(), total * sizeof(double));

        *out_rows = static_cast<int32_t>(pred.rows());
        *out_cols = static_cast<int32_t>(pred.cols());
    }

    DLLEXPORT void train_linear_regressor(
        LinearRegressor* model,
        const double* X_data, const int32_t X_rows, const int32_t X_cols,
        const double* Y_data, const int32_t Y_rows, const int32_t Y_cols)
    {
        // Map input buffers to Eigen matrices (no copies)
        const MapMatrixXdRowMajor X(X_data, X_rows, X_cols);
        const MapMatrixXdRowMajor Y(Y_data, Y_rows, Y_cols);

        // Train the model
        model->train(Eigen::MatrixXd(X), Eigen::MatrixXd(Y));
    }

    DLLEXPORT void release_linear_regressor(const LinearRegressor *model) {
        delete model;
    }


}