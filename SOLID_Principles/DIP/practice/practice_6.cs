using System;

public class HardwareWeatherSensor : ITemperatureProvider
{
    public double ReadCurrentTemperatureCelsius()
    {
        return 22.5;
    }
}

interface ITemperatureProvider //Your choice of IWeatherSensor is great, but to achieve a truly abstract, role-based architecture, you could name it after the action it performs rather than the physical object itself (a sensor).
{
   double ReadCurrentTemperatureCelsius();
}

interface IWeatherDashboard 
{
    void DisplayWeatherReport();
}

class WeatherDashboard : IWeatherDashboard 
{
    private ITemperatureProvider _sensor;

    public WeatherDashboard(ITemperatureProvider sensor)
    {
        _sensor = sensor;
    }

    public void DisplayWeatherReport()
    {
        double temp = _sensor.ReadCurrentTemperatureCelsius();
        Console.WriteLine($"Current Temperature: {temp}°C");
    }
}
