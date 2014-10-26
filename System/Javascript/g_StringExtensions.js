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

  Object.defineProperty(String.prototype, 'format', { 
    enumerable: false, 
    value: String.prototype.format 
  });
}

if (!String.prototype.Empty) {
	String.prototype.Empty = "";
  Object.defineProperty(String.prototype, 'Empty', { 
    enumerable: false, 
    value: String.prototype.Empty 
  });
}

if (!String.IsNullOrEmpty) {

	String.IsNullOrEmpty = function(str) {
		return !str || str.trim().length == 0;
	};
  
}


if (typeof String.prototype.startsWith != 'function') {
  String.prototype.startsWith = function (str){
    return this.indexOf(str) == 0;
  };
  Object.defineProperty(String.prototype, 'startsWith', { 
    enumerable: false, 
    value: String.prototype.startsWith 
  });
}

if (typeof String.prototype.endsWith != 'function') {
  String.prototype.endsWith = function (str){
    return this.indexOf(str) == this.length - str.length;
  };
  Object.defineProperty(String.prototype, 'endsWith', { 
    enumerable: false, 
    value: String.prototype.endsWith 
  });
}


// http://anilsoni85.blogspot.com/2009/11/c-like-trim-function-in-javascript.html
if (!String.prototype.TrimEnd) {

  String.prototype.TrimEnd=function(c) {
    c = ' ';
    var i=this.length-1;
    for(;i>=0 && (this.charAt(i)==" " || this.charAt(i)=="\r" || this.charAt(i)=="\n" ) ;i--);
    return this.substring(0,i+1);
  }
  Object.defineProperty(String.prototype, 'TrimEnd', { 
    enumerable: false, 
    value: String.prototype.TrimEnd 
  });
}


if (!String.prototype.CompareTo) {

  String.prototype.CompareTo = function(s) {
      var f = this.toLowerCase();
      var lon = (f.length > s.length ? f.length : s.length);

      s = s.toLowerCase();

      for(var i = 0; i < lon; i++)
      {
          if (!f[i])
              return -1;
          if (!s[i])
              return 1;

          if (f[i] < s[i])
              return -1;
          else if (f[i] > s[i])
              return 1;
      }
      
      return 0;
  }
}