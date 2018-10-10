/// <binding ProjectOpened='___DEV' />
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

/// compile Bootstrap scss
gulp.task('BUILD_SCSS_Bootstrap', done => {
	//return
	gulp.src('node_modules/bootstrap/scss/bootstrap.scss')
		.pipe(sass())
		.pipe(autoprefixer())
		.pipe(gulp.dest('assets_tmp/styles'));
	done();
});

/// compile Font Awesome scss
gulp.task('BUILD_SCSS_Fontawesome', done => {
	//return
	gulp.src('node_modules/font-awesome/scss/font-awesome.scss')
		.pipe(sass())
		.pipe(autoprefixer())
		.pipe(plumber())
		.pipe(gulp.dest('assets_tmp/styles'));
	done();
});

/// copy resource files from Controllers to wwwroot
gulp.task('COPY_FOLDER_l10n_Controllers_Wwwroot', done => {
	gulp
		.src('Controllers/**/l10n/*.*')
		.pipe(gulp.dest('wwwroot/l10n/Controllers'));
	done();
});

/// copy resource files from Controllers to wwwroot
gulp.task('COPY_FOLDER_l10n_BusinessLayer_Wwwroot', done => {
	gulp
		.src('BusinessLayer/**/l10n/*.*')
		.pipe(gulp.dest('wwwroot/l10n/BusinessLayer'));
	done();
});

/// copy resource files from Model to wwwroot
gulp.task('COPY_FOLDER_l10n_Models_Wwwroot', done => {
	gulp
		.src('Models/**/l10n/*.*')
		.pipe(gulp.dest('wwwroot/l10n/Models'));
	done();
});

/// copy resource files from Views to wwwroot
gulp.task('COPY_FOLDER_l10n_Views_Wwwroot', done => {
	gulp
		.src('Views/**/l10n/*.*')
		.pipe(gulp.dest('wwwroot/l10n/Views'));
	done();
});

/// copy image assets to wwwroot
gulp.task('COPY_FOLDER_Images_App_Wwwroot', done => {
	gulp
		.src('assets_app/images/*.*')
		.pipe(gulp.dest('wwwroot/images'));
	done();
});

/// copy JavaScript files from node_modules to the JavaScript assets folder
var npmJsFiles = [
	'node_modules/jquery/dist/jquery.js',
	'node_modules/jquery-validation/dist/jquery.validate.js',
	'node_modules/popper.js/dist/umd/popper.js',
	'node_modules/bootstrap/dist/js/bootstrap.js'
];
gulp.task('COPY_JS_Npm_Assets', done => {
	gulp
		.src(npmJsFiles)
		.pipe(gulp.dest('assets_tmp/scripts'));
	done();
});

/// copy Css files from node_modules to the styles assets folder
var npmCssFiles = [
	'node_modules/bootstrap/dist/css/bootstrap.css',
	'node_modules/font-awesome/css/font-awesome.css'
];
gulp.task('COPY_CSS_Npm_Assets', done => {
	gulp
		.src(npmCssFiles)
		.pipe(gulp.dest('assets_tmp/styles'));
	done();
});

/// copy Blog CSS assets to wwwroot
var assetsCssBlog = [
	'assets_tmp/styles/font-awesome.css',
	'assets_tmp/styles/bootstrap.css',
	'assets_app/styles/blog.css'
];
gulp.task('BUNDLE_CSS_Blog_Header', done => {
	//return
	gulp.src(assetsCssBlog)
		.pipe(sourcemaps.init())
		.pipe(concat('blog.bundle.min.css'))
		.pipe(cleanCSS({ compatibility: 'ie8' }))
		.pipe(sourcemaps.write('/'))
		.pipe(gulp.dest('wwwroot/css'));
	done();
});

/// copy Dashboard CSS assets to wwwroot
var assetsCssDashboard = [
	'assets_tmp/styles/font-awesome.css',
	'assets_tmp/styles/bootstrap.css',
	'assets_app/styles/dashboard.css'
];
gulp.task('BUNDLE_CSS_Dashboard_Header', done => {
	//return
	gulp.src(assetsCssDashboard)
		.pipe(sourcemaps.init())
		.pipe(concat('dashboard.bundle.min.css'))
		.pipe(cleanCSS({ compatibility: 'ie8' }))
		.pipe(sourcemaps.write('/'))
		.pipe(gulp.dest('wwwroot/css'));
	done();
});

/// copy and uglify Blog JavaScript assets to wwwroot
var assetsJsBlogBody = [
	'assets_tmp/scripts/jquery.js',
	'assets_tmp/scripts/popper.js',
	'assets_tmp/scripts/bootstrap.js',
	'assets_app/scripts/common.js',
	'assets_app/scripts/blog.js'
];
gulp.task('BUNDLE_JS_Blog_Body', done => {
	//return
	gulp.src(assetsJsBlogBody)
		.pipe(sourcemaps.init())
		.pipe(concat('blog.body.bundle.min.js'))
		.pipe(uglify())
		.pipe(sourcemaps.write('/'))
		.pipe(gulp.dest('wwwroot/js'));
	done();
});

/// copy and uglify Dashboard JavaScript assets to wwwroot
var assetsJsBlogDashboard = [
	'assets_tmp/scripts/jquery.js',
	'assets_tmp/scripts/jquery.validate.js',
	'assets_tmp/scripts/popper.js',
	'assets_tmp/scripts/bootstrap.js',
	'assets_app/scripts/common.js',
	'assets_app/scripts/dashboard.js'
];
gulp.task('BUNDLE_JS_Dashboard_Body', done => {
	//return
	gulp.src(assetsJsBlogDashboard)
		.pipe(sourcemaps.init())
		.pipe(concat('dashboard.body.bundle.min.js'))
		.pipe(uglify())
		.pipe(sourcemaps.write('/'))
		.pipe(gulp.dest('wwwroot/js'));
	done();
});

/// copy fonts from font-awesome node package to assets folder
gulp.task('COPY_FOLDER_Fontawesome_Fonts_Npm_Assets', done => {
	//return
	gulp.src('node_modules/font-awesome/fonts/*.*')
		.pipe(gulp.dest('assets_tmp/fonts'));
	done();
});

/// copy fonts from assets to wwwroot
gulp.task('COPY_FOLDER_Fonts_Assets_Wwwroot', done => {
	//return
	gulp.src('assets_tmp/fonts/*.*')
		.pipe(gulp.dest('wwwroot/fonts'));
	done();
});

/// copy jQuery validation localization folder to Asstes
gulp.task('COPY_FOLDER_JQueryValidation_Localization_Npm_Assets', done => {
	//return
	gulp.src('node_modules/jquery-validation/dist/localization/**/*.*')
		.pipe(gulp.dest('assets_tmp/scripts/localization'));
	done();
});

/// copy localization folder to Wwwroot
gulp.task('COPY_FOLDER_Localization_Assets_Wwwroot', done => {
	//return
	gulp.src('assets_tmp/scripts/localization/**/*.*')
		.pipe(gulp.dest('wwwroot/js/localization'));
	done();
});

/// copy Assets folder to Wwwroot
gulp.task('COPY_FOLDER_Assets_Wwwroot', done => {
	//return
	gulp.src(['assets_tmp/**/*.*', '!assets_app/**/*.*'])
		.pipe(gulp.dest('wwwroot'));
	done();
});

/// copy assets_app folder to Wwwroot
gulp.task('COPY_FOLDER_AssetsApp_Wwwroot', done => {
	//return
	gulp.src('assets_app/**/*.*')
		.pipe(gulp.dest('wwwroot'));
	done();
});


gulp.task('_PublishSeries', gulp.series([
	'COPY_FOLDER_Fontawesome_Fonts_Npm_Assets',
	'COPY_FOLDER_Fonts_Assets_Wwwroot',
	'BUILD_SCSS_Bootstrap',
	'BUILD_SCSS_Fontawesome',
	'COPY_FOLDER_l10n_Controllers_Wwwroot',
	'COPY_FOLDER_l10n_BusinessLayer_Wwwroot',
	'COPY_FOLDER_l10n_Models_Wwwroot',
	'COPY_FOLDER_l10n_Views_Wwwroot',
	'COPY_FOLDER_Images_App_Wwwroot',
	'BUNDLE_CSS_Blog_Header',
	'BUNDLE_CSS_Dashboard_Header',
	'COPY_JS_Npm_Assets',
	'BUNDLE_JS_Blog_Body',
	'BUNDLE_JS_Dashboard_Body',
	'COPY_FOLDER_JQueryValidation_Localization_Npm_Assets',
	'COPY_FOLDER_Localization_Assets_Wwwroot'
]), () => { });


gulp.task('_DevSeries', gulp.series([
	'COPY_JS_Npm_Assets',
	'COPY_CSS_Npm_Assets',
	'COPY_FOLDER_Fontawesome_Fonts_Npm_Assets',
	'COPY_FOLDER_JQueryValidation_Localization_Npm_Assets',
	'COPY_FOLDER_l10n_Controllers_Wwwroot',
	'COPY_FOLDER_l10n_BusinessLayer_Wwwroot',
	'COPY_FOLDER_l10n_Models_Wwwroot',
	'COPY_FOLDER_l10n_Views_Wwwroot',
	'COPY_FOLDER_Assets_Wwwroot',
	'COPY_FOLDER_AssetsApp_Wwwroot'
]), () => { });


// A 'default' task is required by Gulp v4
gulp.task('___PUBLISH', gulp.series('_PublishSeries'));
gulp.task('___DEV', gulp.series('_DevSeries'));