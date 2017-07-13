var gulp = require('gulp');
var less = require('gulp-less');
var cleanCSS = require('gulp-clean-css');
var rename = require("gulp-rename");
var uglify = require('gulp-uglify');

// Compile LESS files from /less into /css
gulp.task('less-partywifi', function() {
    return gulp.src('wwwroot/less/partywifi.less')
        .pipe(less())
        .pipe(gulp.dest('wwwroot/css'));
});

gulp.task('less-slideshow', function() {
    return gulp.src('wwwroot/less/slideshow.less')
        .pipe(less())
        .pipe(gulp.dest('wwwroot/css'));
});

gulp.task('less', ['less-partywifi', 'less-slideshow']);

// Minify compiled CSS
gulp.task('minify-css', ['less'], function() {
    return gulp.src(['wwwroot/css/*.css', '!wwwroot/css/*.min.css'])
        .pipe(cleanCSS({ compatibility: 'ie8' }))
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest('wwwroot/css'));
});

// Minify JS
gulp.task('minify-js', function() {
    return gulp.src(['wwwroot/js/*.js', '!wwwroot/js/*.min.js'])
        .pipe(uglify())
        .pipe(rename({ suffix: '.min' }))
        .pipe(gulp.dest('wwwroot/js'));
});

// Copy libraries from /node_modules into wwwroot/lib/
gulp.task('copy', function() {
    gulp.src(['node_modules/bootstrap/dist/**/*', '!**/npm.js', '!**/bootstrap-*', '!**/*.map'])
        .pipe(gulp.dest('wwwroot/lib/bootstrap'));

    gulp.src(['node_modules/tether/dist/**/*'])
        .pipe(gulp.dest('wwwroot/lib/tether'));

    gulp.src(['node_modules/jquery/dist/jquery.min.js'])
        .pipe(gulp.dest('wwwroot/lib/jquery'));

    gulp.src(['node_modules/magnific-popup/dist/*'])
        .pipe(gulp.dest('wwwroot/lib/magnific-popup'));

    gulp.src(['node_modules/knockout/build/output/knockout-latest.js'])
        .pipe(gulp.dest('wwwroot/lib/knockout'));

    gulp.src(['node_modules/animate.css/animate.min.css'])
        .pipe(gulp.dest('wwwroot/lib/animate.css'));

    gulp.src(['node_modules/moment/min/moment.min.js'])
        .pipe(gulp.dest('wwwroot/lib/moment'));


    gulp.src([
        'node_modules/font-awesome/**',
        '!node_modules/font-awesome/**/*.map',
        '!node_modules/font-awesome/.npmignore',
        '!node_modules/font-awesome/*.txt',
        '!node_modules/font-awesome/*.md',
        '!node_modules/font-awesome/*.json'
    ])
    .pipe(gulp.dest('wwwroot/lib/font-awesome'));
});

// Run everything
gulp.task('default', ['less', 'minify-css', 'minify-js', 'copy']);

// Dev task with browserSync
gulp.task('dev', ['less', 'minify-css', 'minify-js'], function() {
    gulp.watch('wwwroot/less/*.less', ['less']);
    gulp.watch('wwwroot/css/*.css', ['minify-css']);
    gulp.watch('wwwroot/js/*.js', ['minify-js']);
});