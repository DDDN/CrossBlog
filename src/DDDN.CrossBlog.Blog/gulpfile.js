/// <binding BeforeBuild='_beforeBuild' />

var gulp = require('gulp'),
    notify = require('gulp-notify'),
    concat = require('gulp-concat'),
    uglify = require('gulp-uglify'),
    cleanCSS = require('gulp-clean-css'),
    sourcemaps = require('gulp-sourcemaps'),
    rename = require('gulp-rename');

gulp.task('_beforeBuild', [
    'copyAssetImageFiles',
    'bundleCssBlog', 'bundleCssDashboard', 'bundleCssConfig',
    'copyNodeJsFiles', 'bundleJsBodyDefault']);

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
        .pipe(gulp.dest('assets/js'));
});

/// copy and clean Blog area CSS assets to wwwroot
var assetsCssBlog = [
    'assets/css/bootstrap.css',
    'assets/css/blog.css'
];
gulp.task('bundleCssBlog', function () {
    return gulp.src(assetsCssBlog)
        .on('error', notify.onError("Error: <%= error.message %>"))
        .pipe(concat('blog.min.css'))
        .pipe(cleanCSS({ compatibility: 'ie8' }))
        .pipe(gulp.dest('wwwroot/css'));
});

/// copy and clean Dashboard area CSS assets to wwwroot
var assetsCssDashboard = [
    'assets/css/bootstrap.css',
    'assets/css/dashboard.css'
];
gulp.task('bundleCssDashboard', function () {
    return gulp.src(assetsCssDashboard)
        .pipe(concat('dashboard.min.css'))
        .pipe(cleanCSS({ compatibility: 'ie8' }))
        .pipe(gulp.dest('wwwroot/css'));
});

/// copy and clean Config area CSS assets to wwwroot
var assetsCssConfig = [
    'assets/css/bootstrap.css',
    'assets/css/config.css'
];
gulp.task('bundleCssConfig', function () {
    return gulp.src(assetsCssConfig)
        .pipe(concat('config.min.css'))
        .pipe(cleanCSS({ compatibility: 'ie8' }))
        .pipe(gulp.dest('wwwroot/css'));
});

/// copy and uglify JavaScript assets to wwwroot
var assetsJsBodyDefault = [
    'assets/js/jquery.slim.js',
    'assets/js/popper.js',
    'assets/js/bootstrap.js'
];
gulp.task('bundleJsBodyDefault', function () {
    return gulp.src(assetsJsBodyDefault)
        .pipe(concat('bodyDefault.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('wwwroot/js'));
});