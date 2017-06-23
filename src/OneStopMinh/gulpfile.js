/// <binding BeforeBuild='build' ProjectOpened='watch' />
var gulp = require('gulp');
var browserify = require('browserify');
var source = require('vinyl-source-stream');
var jshint = require('gulp-jshint');
var uglify = require('gulp-uglify');
var utilities = require('gulp-util');
var concat = require('gulp-concat');
var del = require('del');
var buildProduction = utilities.env.production;
var browserSync = require('browser-sync').create();
var sass = require('gulp-sass');
var sourcemaps = require('gulp-sourcemaps');
var livereload = require('gulp-livereload');




var lib = require('bower-files')({
    "overrides": {
        "bootstrap": {
            "main": [
              "less/bootstrap.less",
              "dist/css/bootstrap.css",
              "dist/js/bootstrap.js"
            ]
        }
    }
});

var config = {
    //Include all js files but exclude any min.js files
    src: ['./wwwroot/**/*.js', '!./wwwroot/**/*.min.js']
}

gulp.task('reload', function () {
    // Change the filepath, when you want to live reload a different page in your project.
    livereload.reload("./");
});

gulp.task('jsBrowserify', ['concatInterface'], function () {
    return browserify({ entries: ['./wwwroot/tmp/allConcat.js'] })
  .bundle()
  .pipe(source('app.js'))
  .pipe(gulp.dest('./wwwroot/build/js'));
});

gulp.task('concatInterface', function () {
    return gulp.src(['./wwwroot/js/*-interface.js'])
    .pipe(concat('allConcat.js'))
    .pipe(gulp.dest('./wwwroot/tmp'));
});

gulp.task("minifyScrpts", ["jsBrowserify"], function () {
    return gulp.src("./build/js/app/js")
    .pipe(uglify())
    .pipe(gulp.dest("./wwwroot/build/js"));
});

gulp.task("clean", function () {
    return del(['build', 'tmp']);
});

gulp.task('jshint', function () {
    return gulp.src(['./wwwroot/js/*.js'])
.pipe(jshint())
.pipe(jshint.reporter('default'));
});

gulp.task("build", ["clean"], function () {
    if (buildProduction) {
        gulp.start('minifyScripts');
    } else {
        gulp.start('jsBrowserify');
    }
    gulp.start('bower');
    gulp.start('cssBuild');
});

gulp.task('bowerJS', function () {
    return gulp.src(lib.ext('js').files)
      .pipe(concat('vendor.min.js'))
      .pipe(uglify())
      .pipe(gulp.dest('./wwwroot/build/js'));
});

gulp.task('bowerCSS', function () {
    return gulp.src(lib.ext('css').files)
    .pipe(concat('vendor.css'))
    .pipe(gulp.dest('./wwwroot/build/css'));
});

gulp.task('bower', ['bowerJS', 'bowerCSS']);

gulp.task('serve', function () {
    browserSync.init({
        server: {
            baseDir: "./"
        }
    });
    gulp.watch([config.src], ['jsBuild']);
    gulp.watch(['bower.json'], ['bowerBuild']);
    gulp.watch(['*.html'], ['htmlBuild']);
});

gulp.task('watch', function () {
    livereload.listen();
    gulp.watch(['./wwwroot/js/*.js', './wwwroot/css/*.scss'], function (event) {
        if (event.type === 'changed') {
            gulp.start('build');
            gulp.start('reload');
        }
    });
});

gulp.task('jsBuild', ['jsBrowserify', 'jshint'], function () {
    browserSync.reload();
});

gulp.task('bowerBuild', ['bower'], function () {
    browserSync.reload();
});

gulp.task('htmlBuild', function () {
    browserSync.reload();
});

gulp.task('cssBuild', function () {
    return gulp.src(['./wwwroot/css/*.scss'])
    .pipe(sourcemaps.init())
    .pipe(sass())
    .pipe(sourcemaps.write())
    .pipe(gulp.dest('./wwwroot/build/css'))
    .pipe(browserSync.stream());
});
