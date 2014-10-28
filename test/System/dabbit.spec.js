var SysObject = require("dabbit");

function evalFn(str) {
  return new Function(""+str);
}

function nothingWriter ()
{
    this.write = function (data) 
    { 
        if (!String.IsNullOrEmpty(data))
        {
            //Console.WriteLine(data); 
            writtenLines.push(data); 
        }

    }
}

describe("System.Object", function(){
  var obj;

  beforeEach(function(){
    obj = new SysObject;
  });

  afterEach(function(){
    obj = null;
  });

  it("GetType returns the object type", function(){
    expect(obj.GetType()).toEqual("Object")
  })
})