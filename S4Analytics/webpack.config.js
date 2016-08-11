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
var merge = require('extendify')({ isDeep: true, arrays: 'concat' });
var cssnext = require('postcss-cssnext');
var failPlugin = require('webpack-fail-plugin');
var devConfig = require('./webpack.config.dev');
var prodConfig = require('./webpack.config.prod');
var isDevelopment = process.env.ASPNETCORE_ENVIRONMENT === 'Development';

module.exports = merge({
    resolve: {
        extensions: [ '', '.js', '.ts' ]
    },
    module: {
        loaders: [
            { test: /\.ts$/, include: /ClientApp/, loader: 'ts-loader?silent=true' },
            { test: /\.html$/, loader: 'raw-loader' },
            { test: /\.css$/, loader: "style-loader!css-loader!postcss-loader" }
        ]
    },
    postcss: function () {
        return {
            defaults: [cssnext]
        };
    },
    entry: {
        main: ['./ClientApp/boot-client.ts']
    },
    output: {
        path: path.join(__dirname, 'wwwroot', 'dist'),
        filename: '[name].js',
        publicPath: '/dist/'
    },
    plugins: [
        failPlugin, // cause CI build to fail if webpack encounters errors (see https://github.com/TypeStrong/ts-loader/issues/108)
        new webpack.DllReferencePlugin({
            context: __dirname,
            manifest: require('./wwwroot/dist/vendor-manifest.json')
        })
    ]
}, isDevelopment ? devConfig : prodConfig);
