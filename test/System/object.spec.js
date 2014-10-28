var SysObject = require("../../System/Object");

describe("System.Object", function(){
  var obj;

  beforeEach(function(){
    obj = new SysObject;
  });

  afterEach(function(){
    obj = null;
  })

  it("GetType returns the object type", function(){
    expect(obj.GetType()).toEqual("Object")
  })
})