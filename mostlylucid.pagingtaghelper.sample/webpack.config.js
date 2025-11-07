const TerserPlugin = require('terser-webpack-plugin');
const path = require('path');

module.exports = (env, argv) => {

    const isProduction = env && env.production === true;

    return {
        mode: isProduction ? 'production' : 'development', // Set mode based on environment
        entry: './src/js/main.js', // Your entry file
        output: {
            filename: 'main.js',
            path: path.resolve(__dirname, 'wwwroot/js/dist'), // Corrected output directory path
        },
        resolve: {
            extensions: ['.js'], // Optional but fine to include
        },
        optimization: {
            minimize: isProduction, // Only minimize in production mode
            minimizer: isProduction ? [
                new TerserPlugin({
                    terserOptions: {
                        mangle: true, // Enable variable and function name mangling
                        format: {
                            comments: false, // Remove all comments
                        },
                        compress: {
                            drop_console: true, // Drop console statements
                            drop_debugger: true, // Drop debugger statements
                            pure_funcs: ['console.log', 'console.info'], // Remove specific console calls
                            passes: 2, // Run compression twice for better results
                        },
                    },
                    extractComments: false, // Don't extract comments to separate file
                }),
            ] : [],
        },
        // No source maps for production (smaller output)
        devtool: isProduction ? false : 'eval-source-map'
    };
};