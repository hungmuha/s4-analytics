/*
webpack.config.js defines the configuration for the Webpack utility,
which is responsible for build tasks such as:
- compiling TypeScript to browser-compliant JavaScript
- compiling cssnext to browser-compliant CSS
- minifying JavaScript output
- bundling JavaScript output
- generating source maps for interactive debugging
*/

var path = require('path');
var webpack = require('webpack');
var ExtractTextPlugin = require('extract-text-webpack-plugin');
var extractCSS = new ExtractTextPlugin('main.css');
var merge = require('extendify')({ isDeep: true, arrays: 'concat' });
var cssnext = require('postcss-cssnext');
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
        extensions: [ '', '.js', '.ts' ]
    },
    module: {
        loaders: [
            { test: /\.ts$/, include: /ClientApp/, loader: 'ts-loader?silent=true' },
            { test: /\.html$/, loader: 'raw-loader' },
            { test: /\.css$/, loader: extractCSS.extract(['css','postcss']) }
        ]
    },
    postcss: function () {
        return {
            defaults: [cssnext]
        };
    },
    entry: {
        main: ['./ClientApp/main.ts']
    },
    output: {
        path: path.join(__dirname, 'wwwroot', 'dist'),
        filename: '[name].js',
        publicPath: '/dist/'
    },
    plugins: [
        extractCSS,
        failPlugin, // cause CI build to fail if webpack encounters errors (see https://github.com/TypeStrong/ts-loader/issues/108)
        new webpack.DllReferencePlugin({
            context: __dirname,
            manifest: require('./wwwroot/dist/angular-manifest.json')
        }),
        new webpack.DllReferencePlugin({
            context: __dirname,
            manifest: require('./wwwroot/dist/vendor-manifest.json')
        })
    ]
};

var devConfig = {
    devtool: 'inline-source-map'
};

var prodConfig = {
    plugins: [
        new webpack.optimize.OccurenceOrderPlugin(),
        new webpack.optimize.UglifyJsPlugin({
            compress: { warnings: false },
            minimize: true,
            mangle: false // Due to https://github.com/angular/angular/issues/6678
        })
    ]
};

module.exports = merge(commonConfig, isDevelopment ? devConfig : prodConfig);
