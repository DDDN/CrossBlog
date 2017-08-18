/// <binding BeforeBuild='copyFiles' />

var gulp = require("gulp");

gulp.task('copyFiles', function () {
    gulp
        .src('img/*.*')
        .pipe(gulp.dest('wwwroot/img'));
});