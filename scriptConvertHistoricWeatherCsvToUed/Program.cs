using System;
using System.Collections.Generic;
using System.IO;
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
            DirectoryInfo dataDirectory;
            List<string> Years = new List<string>();
            List<string> Locations = new List<string>();
            int FileCount = -1;

            /// Open either working directory or dir as specified in args
            if (args.Length == 0)
                dataDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            else
                throw new NotImplementedException();

            /// Setup and validate directories and objects
            ///     Read all dir, convert to years, add to list Years
            ///     Open first dir, count num files, convert to location points, and to Locations
            ///     Open all dirs, ensure file count is same, ensure locations consistant
            foreach (DirectoryInfo d in dataDirectory.GetDirectories())
            {
                // Assumes dir name y{YYYY}, removes beginning "y"
                string year = d.Name.ToString().Remove(0,1);
                Years.Add(year);

                // Check that directories have the same number of files
                if (FileCount == -1)
                    FileCount = d.GetFiles().Count();
                else
                {
                    if (FileCount != d.GetFiles().Count())
                        throw new Exception("One or more directories have a different number of files");
                }

                // Check that directories have the same location IDs
                if (Locations.Count == 0)
                    Locations = getLocations(d);
                else
                {
                    if (Locations.Except(getLocations(d)).Count() > 0)
                        throw new Exception("Non consistant location files");
                }
            }

            ///     Ensure all locations have geocoordinate info (in location_geocoordinates.csv)
            ///     

        }

        static List<string> getLocations(DirectoryInfo directory)
        {
            List<string> locations = new List<string>();

            foreach (FileInfo f in directory.GetFiles())
            {
                // Assume filenames as "year{YYYY}_{Location}.csv"
                string location = f.Name.Split('_')[1].Split('.')[0];
                locations.Add(location);
            }

            return locations;
        }
    }
}
