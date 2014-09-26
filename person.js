var AcmeWeb = {};

(function(NS) {
    NS.Person = function(firstName, lastName) {
        if (!firstName) {
            throw "'firstName' argument cannot be null or empty";
        }

        var FirstName = firstName;
        this.__defineGetter__("FirstName", function(){
            console.log('FirstName getter says ' + FirstName);
            return FirstName;
        });

        this.__defineSetter__("FirstName", function(val){
            console.log('FirstName setter says ' + val);
            FirstName = val;
        });
    }
})(AcmeWeb);

exports.System = AcmeWeb;