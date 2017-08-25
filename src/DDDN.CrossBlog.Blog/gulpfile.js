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
    'copyLocalizationDocumentsFromAreasToWwwroot', 'copyLocalizationDocumentsFromModelToWwwroot',
    'copyAssetImageFiles',
    'bundleCssBlog', 'bundleCssDashboard',
    'copyNpmJsFilesToAssetsFolder', 'bundleBlogBodyJsFiles', 'bundleDashboardBodyJsFiles',
    'copyJqueryValidationLocalizationFolderFromNpmToAssets', 'copyJqueryValidationLocalizationFolderFromAssetsToWwwroot']);

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

/// copy resource files from Areas to wwwroot
gulp.task('copyLocalizationDocumentsFromAreasToWwwroot', function () {
    gulp
        .src('Areas/**/l10n/*.*')
        .pipe(gulp.dest('wwwroot/l10n/Areas'));
});

/// copy resource files from Model to wwwroot
gulp.task('copyLocalizationDocumentsFromModelToWwwroot', function () {
    gulp
        .src('Model/**/l10n/*.*')
        .pipe(gulp.dest('wwwroot/l10n/Model'));
});

/// copy image assets to wwwroot
gulp.task('copyAssetImageFiles', function () {
    gulp
        .src('assets/images/*.*')
        .pipe(gulp.dest('wwwroot/images'));
});

/// copy JavaScript files from node_modules to the JavaScript assets folder
var npmJsFiles = [
    'node_modules/jquery/dist/jquery.js',
    'node_modules/jquery-validation/dist/jquery.validate.js',
    'node_modules/popper.js/dist/umd/popper.js',
    'node_modules/bootstrap/dist/js/bootstrap.js'
];
gulp.task('copyNpmJsFilesToAssetsFolder', function () {
    gulp
        .src(npmJsFiles)
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
        .pipe(cleanCSS({ compatibility: 'ie9' }))
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
        .pipe(cleanCSS({ compatibility: 'ie9' }))
        .pipe(sourcemaps.write('/'))
        .pipe(gulp.dest('wwwroot/css'));
});

/// copy and uglify JavaScript assets to wwwroot
var assetsJsBlogBody = [
    'assets/scripts/jquery.js',
    'assets/scripts/popper.js',
    'assets/scripts/bootstrap.js'
];
gulp.task('bundleBlogBodyJsFiles', function () {
    return gulp.src(assetsJsBlogBody)
        .pipe(sourcemaps.init())
        .pipe(concat('blog.body.bundle.min.js'))
        .pipe(uglify())
        .pipe(sourcemaps.write('/'))
        .pipe(gulp.dest('wwwroot/js'));
});

/// copy and uglify JavaScript assets to wwwroot
var assetsJsDahsboardBody = [
    'assets/scripts/jquery.js',
    'assets/scripts/jquery.validate.js',
    'assets/scripts/popper.js',
    'assets/scripts/bootstrap.js'
];
gulp.task('bundleDashboardBodyJsFiles', function () {
    return gulp.src(assetsJsDahsboardBody)
        .pipe(sourcemaps.init())
        .pipe(concat('dashboard.body.bundle.min.js'))
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

/// copy jQuery validation localization folder
gulp.task('copyJqueryValidationLocalizationFolderFromNpmToAssets', function () {
    return gulp.src('node_modules/jquery-validation/dist/localization/**/*.*')
        .pipe(gulp.dest('assets/scripts/localization'));
});

/// copy jQuery validation localization folder
gulp.task('copyJqueryValidationLocalizationFolderFromAssetsToWwwroot', function () {
    return gulp.src('assets/scripts/localization/**/*.*')
        .pipe(gulp.dest('wwwroot/js/localization'));
});
