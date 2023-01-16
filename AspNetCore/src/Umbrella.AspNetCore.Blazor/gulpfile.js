const gulp = require("gulp");
const gulpSass = require("gulp-sass");
const dartSass = require("sass");

const sass = gulpSass(dartSass);

const paths = {
	source: "./**/*.razor.scss",
	output: "./",
};

gulp.task("build-scoped-sass", () => gulp.src(paths.source).pipe(sass().on("error", sass.logError)).pipe(gulp.dest(paths.output)));
gulp.task("watch-scoped-sass", () => gulp.watch(paths.source, gulp.series(["build-scoped-sass"])));