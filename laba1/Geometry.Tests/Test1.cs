namespace Geometry.Tests;
using TemperatureConverter.Lib;


[TestClass]
public sealed class Test1
{
    [TestMethod]
    public void TestMethod1()
    {
        double width = 5;
        double height = 2;

        double result = RectangleUtils.CalculateArea(width,height);

        Assert.AreEqual(10,result,1e-7);
    }
    [TestMethod]
    public void SecondTest()
    {
        double width = 5;
        double height = 32;
        double result = RectangleUtils.CalculateArea(width,height);
        Assert.IsNotNull(result);
    }
}
