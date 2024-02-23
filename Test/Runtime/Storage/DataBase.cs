namespace Test.Runtime.Storage;
using System;
using RingEngine.Runtime.Storage;

[TestClass]
public class TestDataBase
{
    [TestMethod]
    public void TestJson()
    {
        var db = new DataBase();
        db.PC = 114;
        db.data["aaa"] = "114514";
    }
}
