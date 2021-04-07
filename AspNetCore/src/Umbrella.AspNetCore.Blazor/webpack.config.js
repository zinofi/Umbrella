const path = require("path");
const paths = require("./webpack.paths");
const CheckerPlugin = require('awesome-typescript-loader').CheckerPlugin;
const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");
const BundleAnalyzerPlugin = require("webpack-bundle-analyzer").BundleAnalyzerPlugin;
const TerserJsPlugin = require("terser-webpack-plugin");
const OptimizeCSSAssetsPlugin = require("optimize-css-assets-webpack-plugin");

module.exports = (env, argv) =>
{
	// Default to development mode
	let isDevMode = true;

	if (argv.mode)
		isDevMode = argv.mode === "development";

	const analyze = argv.analyze || false;

	console.log(`Development Mode: ${isDevMode}`);
	console.log(`Bundle Analyzer: ${analyze}`);

	return [{
		mode: isDevMode ? "development" : "production",
		devtool: 'source-map',
		entry: {
			"umbrella-blazor": "scripts"
		},

		resolve: {
			extensions: ['.js', '.ts', '.json'],
			alias: {
				styles: paths.styles,
				scripts: paths.scripts
			}
		},
		output: {
			filename: "[name].js",
			chunkFilename: "[name].js",
			path: path.resolve(__dirname, paths.dist)
		},
		module: {
			rules: [
				{ test: /\.ts/, exclude: /(node_modules|bower_components)/, use: "awesome-typescript-loader?silent=true" },
				{
					test: /\.(css|scss)$/,
					exclude: /(node_modules|bower_components)/,
					use: [MiniCssExtractPlugin.loader,
					{
						// Use the css-loader to parse and minify CSS imports.
						loader: 'css-loader',
						options: { sourceMap: true }
					},
					{
						// Use the postcss-loader to add vendor prefixes via autoprefixer.
						loader: 'postcss-loader',
						options: {
							postcssOptions: { config: paths.postcssConfig },
							sourceMap: true
						}
					},
					{
						// Use the sass-loader to parse and minify CSS imports.
						loader: 'sass-loader',
						options: { sourceMap: true }
					}]
				}
			]
		},
		optimization: {
			minimizer: [
				!isDevMode ? new TerserJsPlugin({
					parallel: true,
					terserOptions: {
						ecma: 2015,
						compress: {
							passes: 2
						},
						output: {
							comments: false
						},
						keep_fnames: true
					},
					extractComments: false
				}) : null,
				!isDevMode ? new OptimizeCSSAssetsPlugin({
					cssProcessorOptions: {
						safe: true,
						discardComments: { removeAll: true },
						map: { inline: false }
					}
				}) : null
			].filter(x => x)
		},
		plugins: [
			// Remove existing assets from dist folder.
			new CleanWebpackPlugin(),
			new CheckerPlugin(),
			new MiniCssExtractPlugin({
				filename: "[name].css"
			}),
		].concat(analyze ? [new BundleAnalyzerPlugin({ analyzerMode: "static" })] : []).filter(x => x)
	}];
};