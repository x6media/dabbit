function Typeof(obj)
{
	var returntype = typeof obj;
	if (returntype == "function")
	{
		returntype = obj.name;
	}
	else if (returntype == "object")
	{
		console.log(arguments.callee.caller.name);
	}

	return returntype;
}

module.exports = Typeof;