function Typeof(obj)
{
	var returntype = typeof obj;
	console.log(returntype);
	if (returntype == "function")
	{
		returntype = obj.name;
	}
	else if (returntype.toLowerCase() == "object")
	{
		returntype = obj.GetType();
	}

	return returntype;
}

module.exports = Typeof;