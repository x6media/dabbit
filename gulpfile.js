var gulp = require('gulp');
var watch = require('gulp-watch');
var jasmine = require('gulp-jasmine');
var jshint = require('gulp-jshint');
var jshintStylish = require('jshint-stylish');

var JAVASCRIPTS = ['System/**/*.js', 'dabbit/**/*.js'];

var TESTS = ['test/**/*.spec.js', 'test/**/*.spec.coffee'];

function jshintFiles(files) {
  return files
    .pipe(jshint('.jshintrc'))
    .pipe(jshint.reporter(jshintStylish));
}

function testFiles(files) {
  return files
    .pipe(jasmine());
}

gulp.task('runTests', function(){
  testFiles(gulp.src(JAVASCRIPTS))
});

gulp.task('testJavascript', function(){
  testFiles(gulp.src(TESTS))
})

gulp.task('lintJavascript', function(){
  jshintFiles(gulp.src(JAVASCRIPTS));
});

gulp.task('default', function(){
  watch(JAVASCRIPTS, jshintFiles);
  watch(TESTS, testFiles);
});