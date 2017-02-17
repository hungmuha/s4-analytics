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
        extensions: [ '.js', '.ts' ]
    },
    module: {
        rules: [
            { test: /\.ts$/, include: /ClientApp/, loaders: ['ts-loader?silent=true', 'angular2-template-loader'] },
            { test: /\.html$/, loader: 'raw-loader' },
            { test: /\.css/, loader: extractCSS.extract(['css-loader', 'postcss-loader']) }
        ]
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
        // see https://github.com/angular/angular/issues/11580
        new webpack.ContextReplacementPlugin(
          /angular(\\|\/)core(\\|\/)(esm(\\|\/)src|src)(\\|\/)linker/,
          './src'
        ),
        extractCSS,
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
        new webpack.optimize.OccurrenceOrderPlugin(),
        new webpack.optimize.UglifyJsPlugin({
            compress: { warnings: false },
            minimize: true,
            mangle: false // Due to https://github.com/angular/angular/issues/6678
        })
    ]
};

module.exports = merge(commonConfig, isDevelopment ? devConfig : prodConfig);
