var path = require('path');
var webpack = require('webpack');
var ExtractTextPlugin = require('extract-text-webpack-plugin');
var extractCSS = new ExtractTextPlugin('vendor.css');
var merge = require('extendify')({ isDeep: true, arrays: 'concat' });
var failPlugin = require('webpack-fail-plugin');
var isDevelopment =
    process.env.ASPNETCORE_ENVIRONMENT === 'Local' ||
    process.env.ASPNETCORE_ENVIRONMENT === 'Development';

if (process.env.ASPNETCORE_ENVIRONMENT) {
    console.log('ASP.NET Core ' + process.env.ASPNETCORE_ENVIRONMENT + ' environment detected.');
}
else {
    console.log('No ASP.NET Core environment detected.');
}

var commonConfig = {
    resolve: {
        extensions: [ '', '.js' ]
    },
    module: {
        loaders: [
            { test: /\.(eot|woff|woff2|ttf|svg|png|jpe?g|gif)(\?\S*)?$/, loader: 'url-loader?limit=100000' },
            { test: /\.css/, loader: extractCSS.extract(['css']) }
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
            'font-awesome/css/font-awesome.css',
            'lodash',
            'moment',
            'reflect-metadata',
            'rxjs',
            'zone.js'
        ]
    },
    output: {
        path: path.join(__dirname, 'wwwroot', 'dist'),
        filename: '[name].js',
        library: '[name]_[hash]',
    },
    plugins: [
        extractCSS,
        failPlugin, // cause CI build to fail if webpack encounters errors (see https://github.com/TypeStrong/ts-loader/issues/108)
        new webpack.optimize.OccurenceOrderPlugin(),
        new webpack.DllPlugin({
            path: path.join(__dirname, 'wwwroot', 'dist', '[name]-manifest.json'),
            name: '[name]_[hash]'
        })
    ]
};

var devConfig = { };

var prodConfig = {
    plugins: [
        new webpack.optimize.UglifyJsPlugin({
            compress: { warnings: false },
            minimize: true,
            mangle: false // Due to https://github.com/angular/angular/issues/6678
        })
    ]
};

module.exports = merge(commonConfig, isDevelopment ? devConfig : prodConfig);
