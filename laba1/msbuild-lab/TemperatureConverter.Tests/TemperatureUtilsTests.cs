using TemperatureConverter.Lib;


[TestClass]
public class TemperatureUtilsTests{
    [TestMethod]
    public void CelsiusToFahrenheit_Zero_Returns32(){
            double input = 0;
            double result = TemperatureUtils.CelsiusToFahrenheit(input);

            Assert.AreEqual(32, result, 1e-7);
    }

    [TestMethod]
    public void CelsiusToFahrenheit_Minus40_ReturnsMinus40()
    {
        double input = -40;

        double result = TemperatureUtils.CelsiusToFahrenheit(input);

        Assert.AreEqual(-40, result, 1e-7);
    }
}
