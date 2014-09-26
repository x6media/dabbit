require('./Javascript/g_ObjectExtensions');

function Object() {
    var objectType = this.getName();

    this.GetType = function() { 
        return objectType; 
    }
}


module.exports = Object;