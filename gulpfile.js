/// <binding />
var gulp = require("gulp");
var flatten = require("gulp-flatten");

gulp.task("copy", function ()
{
    gulp.src("./artifacts/bin/**/Release/*.nupkg")
        .pipe(flatten())
        .pipe(gulp.dest("./output"));
});