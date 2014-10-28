var restify = require("restify");
var http = require('http');
var fs = require("fs");
var WebSocketServer = require('websocket').server;

var System = new require("./System")(__dirname + "/System");
var Console = System.Console;

var dabbit = new require("./System")(__dirname + "/dabbit");

var obj = new System.Object();
var ex = new System.Exception();
var arex = new System.ArgumentException("ohi there");