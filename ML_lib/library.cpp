#include "library.hpp"
#include <cstdint>

#if WIN32
#define DLLEXPORT __declspec(dllexport)
#else
#define DLLEXPORT
#endif

extern "C" {
    DLLEXPORT PerceptronClassifier *create_perceptron_model(const int32_t input_size) {
        return new PerceptronClassifier(input_size);
    }

    DLLEXPORT int32_t get_input_size(const PerceptronClassifier *model) {
        return model->get_input_size();
    }

    DLLEXPORT const double* get_weights_data(const PerceptronClassifier* model) {
        return model->get_weights()->data();
    }

    DLLEXPORT int32_t get_weights_rows(const PerceptronClassifier* model) {
        return static_cast<int32_t>(model->get_weights()->rows());
    }

    DLLEXPORT int32_t get_weights_cols(const PerceptronClassifier* model) {
        return static_cast<int32_t>(model->get_weights()->cols());
    }

    DLLEXPORT double predict_one(const PerceptronClassifier *model, const double* X_data, const int32_t size) {
        // Map the input buffer to an Eigen matrix (no copy)
        const Eigen::Map<const Eigen::RowVectorXd> X(X_data, size);

        // Do the prediction
        return model->predict(Eigen::RowVectorXd(X));
    }

    DLLEXPORT void predict_batch_flat(
    const PerceptronClassifier* model,
    const double* X_data, const int32_t rows, const int32_t cols,
    double** out_data, int32_t* out_rows, int32_t* out_cols)
    {
        // Map the input buffer to an Eigen matrix (no copy)
        const Eigen::Map<const Eigen::MatrixXd> X(X_data, rows, cols);

        // Do the prediction
        Eigen::MatrixXd pred = model->predict(Eigen::MatrixXd(X));

        // Allocate a flat buffer for the output
        const int32_t total = static_cast<int32_t>(pred.size());
        *out_data = static_cast<double *>(std::malloc(total * sizeof(double)));
        std::memcpy(*out_data, pred.data(), total * sizeof(double));

        *out_rows = static_cast<int32_t>(pred.rows());
        *out_cols = static_cast<int32_t>(pred.cols());
    }

    DLLEXPORT void free_buffer(double* ptr) {
        std::free(ptr);
    }

    DLLEXPORT double* train(
        PerceptronClassifier* model,
        const double* X_data, const int32_t X_rows, const int32_t X_cols,
        const double* Y_data, const int32_t Y_rows, const int32_t Y_cols,
        int32_t* out_size,
        const int32_t epochs=1000, const float learning_rate=0.01)
    {
        // Map the input buffer to an Eigen matrix (no copy)
        const Eigen::Map<const Eigen::MatrixXd> X(X_data, X_rows, X_cols);
        const Eigen::Map<const Eigen::MatrixXd> Y(Y_data, Y_rows, Y_cols);

        Eigen::VectorXd error = model->train(Eigen::MatrixXd(X), Eigen::MatrixXd(Y), epochs, learning_rate);

        *out_size = static_cast<int32_t>(error.size());

        double* out_buffer = static_cast<double*>(std::malloc(error.size() * sizeof(double)));
        std::memcpy(out_buffer, error.data(), error.size() * sizeof(double));
        return out_buffer;
    }

    DLLEXPORT void release_perceptron_model(const PerceptronClassifier *model) {
        delete model;
    }

    DLLEXPORT float sum_array(const float* array, int32_t array_length) {
        float sum = 0.0;
        for (auto i = 0; i < array_length; i++) {
            sum += array[i];
        }
        return sum;
    }

    DLLEXPORT float* get_array_of_incrementing_numbers(int32_t num_elements) {
        auto array = new float[num_elements];
        for (auto i = 0; i < num_elements; i++) {
            array[i] = static_cast<float>(i);
        }
        return array;
    }

    DLLEXPORT void delete_array(const float* array, int32_t array_length) {
        delete [] array;
    }

    DLLEXPORT int32_t my_add(int32_t a, int32_t b) {
        return a + b;
    }
}