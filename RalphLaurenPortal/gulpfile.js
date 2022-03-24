/// <binding ProjectOpened='watch' />
'use strict';

const
    gulp = require('gulp'),
    concat = require('gulp-concat'),
    cssmin = require('gulp-cssmin'),
    sass = require('gulp-sass')(require('sass')),
    htmlmin = require('gulp-htmlmin'),
    terser = require('gulp-terser'),
    sourcemaps = require('gulp-sourcemaps'),
    merge = require('merge-stream'),
    del = require('del'),
    bundleconfig = require('./bundleconfig.json');

const regex = {
    css: /\.(css)$/,
    sass: /\.(scss)$/,
    html: /\.(html|htm)$/,
    js: /\.js$/
};

function minjs () {
    var tasks = getBundles(regex.js).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { since: gulp.lastRun(minjs), base: '.' })
            .pipe(concat(bundle.outputFileName))
            .pipe(sourcemaps.init())
            .pipe(terser())
            .pipe(sourcemaps.write('./'))
            .pipe(gulp.dest('.'));
    });
    return merge(tasks);
}

function mincss () {
    var tasks = getBundlesByInputFileNames(regex.css).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { since: gulp.lastRun(mincss), base: '.' })
            .pipe(sourcemaps.init())
            .pipe(cssmin())
            .pipe(concat(bundle.outputFileName))
            .pipe(sourcemaps.write('.'))
            .pipe(gulp.dest('.'));
    });
    return merge(tasks);
}

function compilesass () {
    var tasks = getBundlesByInputFileNames(regex.sass).map(function (bundle) {
        return gulp.src(bundle.inputFiles, { since: gulp.lastRun(compilesass), base: '.' })
            .pipe(sourcemaps.init())
            .pipe(sass().on('error', sass.logError))
            .pipe(concat(bundle.outputFileName))
            .pipe(sourcemaps.write('.'))
            .pipe(gulp.dest('.'));
    });
    return merge(tasks);
}

gulp.task('clean', function () {
    var files = bundleconfig.map(function (bundle) {
        return bundle.outputFileName;
    });
    return del(files);
});

gulp.task('css', gulp.series([mincss]));

gulp.task('sass', gulp.series([compilesass]));

gulp.task('js', gulp.series([minjs]));

gulp.task('all', gulp.series(['sass', 'css', 'js']));

gulp.task('watch', function () {
    getBundles(regex.js).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, minjs);
    });

    getBundlesByInputFileNames(regex.css).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, mincss);
    });

    getBundlesByInputFileNames(regex.sass).forEach(function (bundle) {
        gulp.watch(bundle.inputFiles, compilesass);
    });
});

function getBundles(regexPattern) {
    return bundleconfig.filter(function (bundle) {
        return regexPattern.test(bundle.outputFileName);
    });
}

function getBundlesByInputFileNames(regexPattern) {
    return bundleconfig.filter(function (bundle) {
        return regexPattern.test(bundle.inputFiles);
    });
}