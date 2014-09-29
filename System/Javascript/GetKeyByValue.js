
module.exports = function(obj, value ) {
    for( var prop in this ) {
        if( obj.hasOwnProperty( prop ) ) {
             if( obj[ prop ] === value )
                 return prop;
        }
    }
};