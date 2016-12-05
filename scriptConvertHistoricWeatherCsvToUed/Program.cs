using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ued;

namespace scriptConvertHistoricWeatherCsvToUed
{
    class Program
    {
        /// Open either working directory or dir as specified in args
        /// Setup and validate directories and objects
        ///     Read all dir, convert to years, add to list Years
        ///     Open first dir, count num files, convert to location points, and to Locations
        ///     Open all dirs, ensure file count is same, ensure locations consistant
        ///     Ensure all locations have geocoordinate info (in location_geocoordinates.csv)
        /// Create UED object
        ///     (should I create this class in a Ued library?)
        ///     Assign years, lat/lon, meta-data
        ///     Add to Ueds
        /// Foreach Ued in Ueds
        ///     Foreach year in Years
        ///         Open corresponding directory
        ///         Read corresponding weather csv file
        ///         Write weather variables to Ued
        ///     Save Ued
        static void Main(string[] args)
        {
            
        }
    }
}
