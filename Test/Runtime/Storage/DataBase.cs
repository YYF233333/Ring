namespace Test.Runtime.Storage;

using RingEngine.Runtime.Storage;

[TestClass]
public class TestDataBase
{
    [TestMethod]
    public void TestJson()
    {
        var db = new DataBase { PC = 114 };
    }
}
