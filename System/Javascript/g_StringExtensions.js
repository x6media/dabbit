// First, checks if it isn't implemented yet.
if (!String.prototype.format) {
  // http://stackoverflow.com/a/5077091/486058
  String.prototype.format = function () {
    var args = (arguments.length > 0 && typeof arguments[0] != "string" ? arguments[0] : arguments);
    return this.replace(/\{\{|\}\}|\{(\d+)\}/g, function (m, n) {
      if (m == "{{") { return "{"; }
      if (m == "}}") { return "}"; }
      return args[n];
    });
  };
}

if (!String.prototype.Empty) {
	String.prototype.Empty = "";
}

if (!String.IsNullOrEmpty) {

	String.IsNullOrEmpty = function(str) {
		return !str || str.trim().length == 0;
	};
}

// http://stackoverflow.com/questions/646628/how-to-check-if-a-string-startswith-another-string
if (typeof String.prototype.startsWith != 'function') {
  String.prototype.startsWith = function (str){
    return this.slice(0, str.length) == str;
  };
}

if (typeof String.prototype.endsWith != 'function') {
  String.prototype.endsWith = function (str){
    return this.slice(-str.length) == str;
  };
}