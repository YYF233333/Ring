namespace Test
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var folder = Path.GetFileNameWithoutExtension(@"res://folder/main.gd");
            Assert.AreEqual(@"main", folder);
        }
    }
}