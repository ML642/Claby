namespace TemperatureConverter.Lib;

public static class TemperatureUtils
{
    public static double CelsiusToFahrenheit(double temperature)
    {
        return temperature * 9 / 5 + 32;
    }
}