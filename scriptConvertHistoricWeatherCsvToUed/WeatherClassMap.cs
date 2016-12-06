using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scriptConvertHistoricWeatherCsvToUed
{
    public class WeatherClassMap : CsvClassMap<WeatherData>
    {
        public WeatherClassMap()
        {
            Map(m => m.DayOfYear).Index(0);
            Map(m => m.Precipitation).Index(1);
            Map(m => m.MaximumRelativeHumidity).Index(2);
            Map(m => m.MinimumRelativeHumidity).Index(3);
            Map(m => m.SolarRadiation).Index(4);
            Map(m => m.MaximumTemperature).Index(5);
            Map(m => m.MinimumTemperature).Index(6);
            Map(m => m.WindSpeed).Index(7);
        }
    }
}
