const path = require('path');
const webpack = require('webpack');
const ExtractTextPlugin = require('extract-text-webpack-plugin');
const merge = require('webpack-merge');

module.exports = (env) => {
    const extractCSS = new ExtractTextPlugin('vendor.css');
    const isDevBuild = !(env && env.prod);

    const config = {
        stats: { modules: false },
        resolve: { extensions: ['.js'] },
        module: {
            rules: [
                { test: /\.(png|woff|woff2|eot|ttf|svg)(\?|$)/, use: 'url-loader?limit=100000' },
                { test: /\.css(\?|$)/, use: extractCSS.extract({ use: 'css-loader' }) }
            ]
        },
        entry: {
            vendor: [
                '@angular/common',
                '@angular/compiler',
                '@angular/core',
                '@angular/forms',
                '@angular/http',
                '@angular/platform-browser',
                '@angular/platform-browser-dynamic',
                '@angular/router',
                '@ng-bootstrap/ng-bootstrap',
                'bootstrap/dist/css/bootstrap.css',
                'core-js',
                'event-source-polyfill',
                'font-awesome/css/font-awesome.css',
                'lodash',
                'moment',
                'openlayers',
                'openlayers/dist/ol.css',
                'reflect-metadata',
                'rxjs',
                'zone.js'
            ]
        },
        output: {
            publicPath: '/dist/',
            filename: '[name].js',
            library: '[name]_[hash]',
            path: path.join(__dirname, 'wwwroot', 'dist')
        },
        plugins: [
            new webpack.ContextReplacementPlugin(/\@angular\b.*\b(bundles|linker)/, path.join(__dirname, './ClientApp')), // Workaround for https://github.com/angular/angular/issues/11580
            extractCSS,
            new webpack.DllPlugin({
                path: path.join(__dirname, 'wwwroot', 'dist', '[name]-manifest.json'),
                name: '[name]_[hash]'
            })
        ].concat(isDevBuild ? [] : [
            new webpack.optimize.UglifyJsPlugin()
        ])
    };

    return config;
}
