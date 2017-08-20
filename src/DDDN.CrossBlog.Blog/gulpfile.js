/// <binding BeforeBuild='_beforeBuild' />

var gulp = require('gulp'),
	notify = require('gulp-notify'),
	concat = require('gulp-concat'),
	uglify = require('gulp-uglify'),
	cleanCSS = require('gulp-clean-css'),
	sourcemaps = require('gulp-sourcemaps'),
	sass = require('gulp-sass'),
	autoprefixer = require('gulp-autoprefixer'),
	plumber = require('gulp-plumber'),
	rename = require('gulp-rename');

gulp.task('_beforeBuild', [
	'copyFromNodeFontawesomeFontfolderToAssets', 'copyFontsToWwwroot',
	'scssBuildBootstrap', 'scssBuildFontawesome',
	'copyLocalizationDocumentsFromAreasToWwwroot',
	'copyAssetImageFiles',
	'bundleCssBlog', 'bundleCssDashboard', 'bundleCssConfig',
	'copyNodeJsFiles', 'bundleJsBodyDefault']);

/// compile Bootstrap scss
gulp.task('scssBuildBootstrap', function () {
	return gulp.src('node_modules/bootstrap/scss/bootstrap.scss')
		.pipe(sass())
		.pipe(autoprefixer())
		.pipe(gulp.dest('assets/styles'));
});

/// compile Font Awesome scss
gulp.task('scssBuildFontawesome', function () {
	return gulp.src('node_modules/font-awesome/scss/font-awesome.scss')
		.pipe(sass())
		.pipe(autoprefixer())
		.pipe(plumber())
		.pipe(gulp.dest('assets/styles'));
});

/// copy resource files from assets to wwwroot
gulp.task('copyLocalizationDocumentsFromAreasToWwwroot', function () {
	gulp
		.src('Areas/**/l10n/*.*')
		.pipe(gulp.dest('wwwroot/l10n/Areas'));
});

/// copy image assets to wwwroot
gulp.task('copyAssetImageFiles', function () {
	gulp
		.src('assets/images/*.*')
		.pipe(gulp.dest('wwwroot/images'));
});

/// copy JavaScript files from node_modules to the JavaScript assets folder
var nodeJsFiles = [
	'node_modules/jquery/dist/jquery.slim.js',
	'node_modules/popper.js/dist/umd/popper.js',
	'node_modules/bootstrap/dist/js/bootstrap.js'
];
gulp.task('copyNodeJsFiles', function () {
	gulp
		.src(nodeJsFiles)
		.pipe(gulp.dest('assets/scripts'));
});

/// copy and clean Blog area CSS assets to wwwroot
var assetsCssBlog = [
	'assets/styles/font-awesome.css',
	'assets/styles/bootstrap.css',
	'assets/styles/blog.css'
];
gulp.task('bundleCssBlog', function () {
	return gulp.src(assetsCssBlog)
		.pipe(sourcemaps.init())
		.pipe(concat('blog.min.css'))
		.pipe(cleanCSS({ compatibility: 'ie8' }))
		.pipe(sourcemaps.write('/'))
		.pipe(gulp.dest('wwwroot/css'));
});

/// copy and clean Dashboard area CSS assets to wwwroot
var assetsCssDashboard = [
	'assets/styles/font-awesome.css',
	'assets/styles/bootstrap.css',
	'assets/styles/dashboard.css'
];
gulp.task('bundleCssDashboard', function () {
	return gulp.src(assetsCssDashboard)
		.pipe(sourcemaps.init())
		.pipe(concat('dashboard.min.css'))
		.pipe(cleanCSS({ compatibility: 'ie8' }))
		.pipe(sourcemaps.write('/'))
		.pipe(gulp.dest('wwwroot/css'));
});

/// copy and clean Config area CSS assets to wwwroot
var assetsCssConfig = [
	'assets/styles/font-awesome.css',
	'assets/styles/bootstrap.css',
	'assets/styles/config.css'
];
gulp.task('bundleCssConfig', function () {
	return gulp.src(assetsCssConfig)
		.pipe(sourcemaps.init())
		.pipe(concat('config.min.css'))
		.pipe(cleanCSS({ compatibility: 'ie8' }))
		.pipe(sourcemaps.write('/'))
		.pipe(gulp.dest('wwwroot/css'));
});

/// copy and uglify JavaScript assets to wwwroot
var assetsJsBodyDefault = [
	'assets/scripts/jquery.slim.js',
	'assets/scripts/popper.js',
	'assets/scripts/bootstrap.js'
];
gulp.task('bundleJsBodyDefault', function () {
	return gulp.src(assetsJsBodyDefault)
		.pipe(sourcemaps.init())
		.pipe(concat('bodyDefault.min.js'))
		.pipe(uglify())
		.pipe(sourcemaps.write('/'))
		.pipe(gulp.dest('wwwroot/js'));
});

/// copy fonts from font-awesome node package to assets folder
gulp.task('copyFromNodeFontawesomeFontfolderToAssets', function () {
	return gulp.src('node_modules/font-awesome/fonts/*.*')
		.pipe(gulp.dest('assets/fonts'));
});

/// copy fonts from assets to wwwroot
gulp.task('copyFontsToWwwroot', function () {
	return gulp.src('assets/fonts/*.*')
		.pipe(gulp.dest('wwwroot/fonts'));
});
