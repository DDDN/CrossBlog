/// <binding ProjectOpened='_beforeBuild' />
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
	'COPY_FOLDER_Fontawesome_Fonts_Npm_Assets', 'COPY_FOLDER_Fonts_Assets_Wwwroot',
	'BUILD_SCSS_Bootstrap', 'BUILD_SCSS_Fontawesome',
   'COPY_FOLDER_l10n_Areas_Wwwroot', 'COPY_FOLDER_l10n_Models_Wwwroot', 'COPY_FOLDER_l10n_Views_Wwwroot',
	'COPY_FOLDER_Images_App_Wwwroot',
	'BUNDLE_CSS_Blog_Header', 'BUNDLE_CSS_Dashboard_Header', 'BUNDLE_CSS_Administration_Header',
	'COPY_JS_Npm_Assets', 'BUNDLE_JS_Blog_Body', 'BUNDLE_JS_Dashboard_Body', 'BUNDLE_JS_Administration_Body',
	'COPY_FOLDER_JQueryValidation_Localization_Npm_Assets', 'COPY_FOLDER_Localization_Assets_Wwwroot']);

/// compile Bootstrap scss
gulp.task('BUILD_SCSS_Bootstrap', function () {
	return gulp.src('node_modules/bootstrap/scss/bootstrap.scss')
		.pipe(sass())
		.pipe(autoprefixer())
		.pipe(gulp.dest('assets/styles'));
});

/// compile Font Awesome scss
gulp.task('BUILD_SCSS_Fontawesome', function () {
	return gulp.src('node_modules/font-awesome/scss/font-awesome.scss')
		.pipe(sass())
		.pipe(autoprefixer())
		.pipe(plumber())
		.pipe(gulp.dest('assets/styles'));
});

/// copy resource files from Areas to wwwroot
gulp.task('COPY_FOLDER_l10n_Areas_Wwwroot', function () {
	gulp
		.src('Areas/**/l10n/*.*')
		.pipe(gulp.dest('wwwroot/l10n/Areas'));
});

/// copy resource files from Model to wwwroot
gulp.task('COPY_FOLDER_l10n_Models_Wwwroot', function () {
	gulp
		.src('Models/**/l10n/*.*')
		.pipe(gulp.dest('wwwroot/l10n/Models'));
});

/// copy image assets to wwwroot
gulp.task('COPY_FOLDER_Images_App_Wwwroot', function () {
	gulp
		.src('assets/app/images/*.*')
		.pipe(gulp.dest('wwwroot/images'));
});

/// copy resource files from Views to wwwroot
gulp.task('COPY_FOLDER_l10n_Views_Wwwroot', function () {
   gulp
      .src('Views/**/l10n/*.*')
      .pipe(gulp.dest('wwwroot/l10n/Views'));
});

/// copy JavaScript files from node_modules to the JavaScript assets folder
var npmJsFiles = [
	'node_modules/jquery/dist/jquery.js',
	'node_modules/jquery-validation/dist/jquery.validate.js',
	'node_modules/popper.js/dist/umd/popper.js',
	'node_modules/bootstrap/dist/js/bootstrap.js'
];
gulp.task('COPY_JS_Npm_Assets', function () {
	gulp
		.src(npmJsFiles)
		.pipe(gulp.dest('assets/scripts'));
});

/// copy and clean Blog area CSS assets to wwwroot
var assetsCssBlog = [
	'assets/styles/font-awesome.css',
	'assets/styles/bootstrap.css',
	'assets/app/styles/blog.css'
];
gulp.task('BUNDLE_CSS_Blog_Header', function () {
	return gulp.src(assetsCssBlog)
		.pipe(sourcemaps.init())
		.pipe(concat('blog.bundle.min.css'))
		.pipe(cleanCSS({ compatibility: 'ie8' }))
		.pipe(sourcemaps.write('/'))
		.pipe(gulp.dest('wwwroot/css'));
});

/// copy and clean Blog area CSS assets to wwwroot
var assetsCssDashboard = [
	'assets/styles/font-awesome.css',
	'assets/styles/bootstrap.css',
	'assets/app/styles/dashboard.css'
];
gulp.task('BUNDLE_CSS_Dashboard_Header', function () {
	return gulp.src(assetsCssDashboard)
		.pipe(sourcemaps.init())
		.pipe(concat('dashboard.bundle.min.css'))
		.pipe(cleanCSS({ compatibility: 'ie8' }))
		.pipe(sourcemaps.write('/'))
		.pipe(gulp.dest('wwwroot/css'));
});

/// copy and clean Administration area CSS assets to wwwroot
var assetsCssAdministration = [
	'assets/styles/font-awesome.css',
	'assets/styles/bootstrap.css',
	'assets/app/styles/administration.css'
];
gulp.task('BUNDLE_CSS_Administration_Header', function () {
	return gulp.src(assetsCssAdministration)
		.pipe(sourcemaps.init())
		.pipe(concat('administration.bundle.min.css'))
		.pipe(cleanCSS({ compatibility: 'ie8' }))
		.pipe(sourcemaps.write('/'))
		.pipe(gulp.dest('wwwroot/css'));
});

/// copy and uglify JavaScript assets to wwwroot
var assetsJsBlogBody = [
	'assets/scripts/jquery.js',
	'assets/scripts/popper.js',
   'assets/scripts/bootstrap.js',
   'assets/app/scripts/common.js',
	'assets/app/scripts/blog.js'
];
gulp.task('BUNDLE_JS_Blog_Body', function () {
	return gulp.src(assetsJsBlogBody)
		.pipe(sourcemaps.init())
		.pipe(concat('blog.body.bundle.min.js'))
		.pipe(uglify())
		.pipe(sourcemaps.write('/'))
		.pipe(gulp.dest('wwwroot/js'));
});

/// copy and uglify JavaScript assets to wwwroot
var assetsJsBlogDashboard = [
	'assets/scripts/jquery.js',
	'assets/scripts/popper.js',
   'assets/scripts/bootstrap.js',
   'assets/app/scripts/common.js',
	'assets/app/scripts/dashboard.js'
];
gulp.task('BUNDLE_JS_Dashboard_Body', function () {
	return gulp.src(assetsJsBlogDashboard)
		.pipe(sourcemaps.init())
		.pipe(concat('dashboard.body.bundle.min.js'))
		.pipe(uglify())
		.pipe(sourcemaps.write('/'))
		.pipe(gulp.dest('wwwroot/js'));
});

/// copy and uglify JavaScript assets to wwwroot
var assetsJsAdministrationBody = [
	'assets/scripts/jquery.js',
	'assets/scripts/jquery.validate.js',
	'assets/scripts/popper.js',
   'assets/scripts/bootstrap.js',
   'assets/app/scripts/common.js',
	'assets/app/scripts/administration.js'
];
gulp.task('BUNDLE_JS_Administration_Body', function () {
	return gulp.src(assetsJsAdministrationBody)
		.pipe(sourcemaps.init())
		.pipe(concat('administration.body.bundle.min.js'))
		.pipe(uglify())
		.pipe(sourcemaps.write('/'))
		.pipe(gulp.dest('wwwroot/js'));
});

/// copy fonts from font-awesome node package to assets folder
gulp.task('COPY_FOLDER_Fontawesome_Fonts_Npm_Assets', function () {
	return gulp.src('node_modules/font-awesome/fonts/*.*')
		.pipe(gulp.dest('assets/fonts'));
});

/// copy fonts from assets to wwwroot
gulp.task('COPY_FOLDER_Fonts_Assets_Wwwroot', function () {
	return gulp.src('assets/fonts/*.*')
		.pipe(gulp.dest('wwwroot/fonts'));
});

/// copy jQuery validation localization folder to Asstes
gulp.task('COPY_FOLDER_JQueryValidation_Localization_Npm_Assets', function () {
	return gulp.src('node_modules/jquery-validation/dist/localization/**/*.*')
		.pipe(gulp.dest('assets/scripts/localization'));
});

/// copy localization folder to Wwwroot
gulp.task('COPY_FOLDER_Localization_Assets_Wwwroot', function () {
	return gulp.src('assets/scripts/localization/**/*.*')
		.pipe(gulp.dest('wwwroot/js/localization'));
});
