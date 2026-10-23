using TemperatureConverter.Lib;

namespace TemperatureConverter.Tests;

[TestClass]
public sealed class Test1
{
    [TestMethod]
    public void TestMethod1()
    {
        int a = 5;
        int b = 5;

        int answer1 = Class1.CelsiusToFahrenheit(a);
        int answer2 = Class1.CelsiusToFahrenheit(b);

        Assert.AreEqual(answer1,5);
        Assert.AreEqual(answer2,5);

    }
    [TestMethod]
    public void TestNumber2()
    {
        int answer1 = Class1.CelsiusToFahrenheit(1);

        Assert.IsNotNull(answer1);
    }
}
