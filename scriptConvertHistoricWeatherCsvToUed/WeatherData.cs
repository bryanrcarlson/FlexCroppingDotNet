using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace scriptConvertHistoricWeatherCsvToUed
{
    public class WeatherData
    {
        public int DayOfYear { get; set; }
        public double Precipitation { get; set; }
        public double MaximumRelativeHumidity { get; set; }
        public double MinimumRelativeHumidity { get; set; }
        public double SolarRadiation { get; set; }
        public double MaximumTemperature { get; set; }
        public double MinimumTemperature { get; set; }
        public double WindSpeed { get; set; }
    }
}
