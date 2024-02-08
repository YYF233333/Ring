using LiteDB;
using System.Text;

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

        [TestMethod]
        public void test()
        {
            var db = new LiteDatabase(":memory:");
            var fs = db.GetStorage<string>();
            fs.Upload("test", "test.txt", new MemoryStream(Encoding.UTF8.GetBytes("aaaabbbbccccdddd")));
            var ret = new MemoryStream();
            fs.Download("test", ret);
            var s = Encoding.UTF8.GetString(ret.ToArray());
            Assert.AreEqual("aaaabbbbccccdddd", s);
        }
    }
}