/*
webpack.config.js defines the configuration for the Webpack utility,
which is responsible for build tasks such as:
- compiling TypeScript to browser-compliant JavaScript
- compiling cssnext to browser-compliant CSS
- minifying JavaScript output
- bundling JavaScript output
- generating source maps for interactive debugging
*/

const path = require('path');
const webpack = require('webpack');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const merge = require('webpack-merge');
const CheckerPlugin = require('awesome-typescript-loader').CheckerPlugin;

module.exports = (env) => {
    const extractCSS = new ExtractTextPlugin('main.css');
    const isDevBuild = !(env && env.prod);
    const outputDir = './wwwroot/dist';

    const config = {
        stats: { modules: false },
        context: __dirname,
        resolve: { extensions: ['.js', '.ts'] },
        entry: { main: './ClientApp/main.ts' },
        output: {
            filename: '[name].js',
            publicPath: '/dist/', // Webpack dev middleware, if enabled, handles requests for this URL prefix
            path: path.join(__dirname, outputDir)
        },
        module: {
            rules: [
                { test: /\.ts$/, include: /ClientApp/, use: ['awesome-typescript-loader?silent=true', 'angular2-template-loader'] },
                { test: /\.html$/, use: 'html-loader?minimize=false' },
                { test: /\.css(\?|$)/, use: extractCSS.extract({ use: ['css-loader', 'postcss-loader'] }) },
                { test: /\.(png|jpg|jpeg|gif|svg)$/, use: 'url-loader?limit=25000' }
            ]
        },
        plugins: [
            extractCSS,
            new CheckerPlugin(),
            new webpack.DllReferencePlugin({
                context: __dirname,
                manifest: require('./wwwroot/dist/vendor-manifest.json')
            })
        ].concat(isDevBuild ? [
            // Plugins that apply in development builds only
            new webpack.SourceMapDevToolPlugin({
                filename: '[file].map', // Remove this line if you prefer inline source maps
                moduleFilenameTemplate: path.relative(outputDir, '[resourcePath]') // Point sourcemap entries to the original file locations on disk
            })
        ] : [
                // Plugins that apply in production builds only
                new webpack.optimize.UglifyJsPlugin()
            ])
    };

    return config;
};
