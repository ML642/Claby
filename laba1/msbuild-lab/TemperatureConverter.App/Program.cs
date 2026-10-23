using TemperatureConverter.Lib;

double[] temperatures = { -20, 0, 20, 100 };

foreach (double temperature in temperatures){
   
    double result =  TemperatureUtils.CelsiusToFahrenheit(temperature);

    Console.WriteLine($"{temperature} C = {result} F");

}


//Console.WriteLine("Hello, World!");
