using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UED_simple;
using Ued;

namespace scriptConvertHistoricWeatherCsvToUed
{
    class Program
    {
        /// Open either working directory or dir as specified in args
        /// Setup and validate directories and objects
        ///     Read all dir, convert to years, add to list Years
        ///     Open first dir, count num files, convert to location points, and to LocationIds
        ///     Open all dirs, ensure file count is same, ensure locations consistant
        ///     Ensure all locations have geocoordinate info (in location_geocoordinates.csv)
        /// Create UED object
        ///     recreate output dir
        ///     lat/lon, meta-data
        ///     Add to Ueds
        /// Foreach Ued in Ueds
        ///     Foreach year in Years
        ///         Open corresponding directory
        ///         Read corresponding weather csv file
        ///         Write weather variables to Ued
        ///     Save Ued

        const string RELATIVE_PATH_TO_LOCATIONS = @"location_geocoordinates.csv";
        const string RELATIVE_PATH_TO_UED = @"ued_files";
        const int DEFAULT_SCREENING_HEIGHT = 2;

        static void Main(string[] args)
        {
            DirectoryInfo dataDirectory;
            List<string> Years = new List<string>();
            List<string> LocationIds = new List<string>();
            int FileCount = -1;

            /// Open either working directory or dir as specified in args
            if (args.Length == 0)
                dataDirectory = new DirectoryInfo(Environment.CurrentDirectory);
            else
                throw new NotImplementedException();

            /// Setup and validate directories and objects
            ///     Read all dir, convert to years, add to list Years
            ///     Open first dir, count num files, convert to location points, and to LocationIds
            ///     Open all dirs, ensure file count is same, ensure locations consistant
            foreach (DirectoryInfo d in dataDirectory.GetDirectories("y*"))
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
                if (LocationIds.Count == 0)
                    LocationIds = getLocations(d);
                else
                {
                    if (LocationIds.Except(getLocations(d)).Count() > 0)
                        throw new Exception("Non consistant location files");
                }
            }

            ///     Ensure all locations have geocoordinate info (in location_geocoordinates.csv)
            List<Location> Locations = new List<Location>();
            using (TextReader reader = File.OpenText(RELATIVE_PATH_TO_LOCATIONS))
            {
                CsvReader csv = new CsvReader(reader);
                Locations = csv.GetRecords<Location>().ToList();
            }
            // LocationIds count should be same as Locations
            if (LocationIds.Count != Locations.Count)
                throw new Exception("Not all location files have corresponding geocoordinates - count is not equal");

            List<Location> matchingObjects = Locations
                .Where(l => LocationIds.Contains(l.Fid.ToString())).ToList();
            if (matchingObjects.Count != Locations.Count)
                throw new Exception("Not all location files have corresponding geocoordinates - count is not equal");

            /// Create UED object
            ///     remake output directory   
            DirectoryInfo pathToUedOutput = new DirectoryInfo(Path.Combine(
                dataDirectory.ToString(), RELATIVE_PATH_TO_UED));
            if (Directory.Exists(pathToUedOutput.ToString()))
                Directory.Delete(pathToUedOutput.ToString(), true);

            Directory.CreateDirectory(pathToUedOutput.ToString());

            ///     lat/lon, meta-data
            List<FileInfo> uedFiles = new List<FileInfo>();
            foreach(Location loc in Locations)
            {
                //FileInfo uedFile = new FileInfo(
                //    Path.Combine(
                //        pathToUedOutput.ToString(),  
                //        loc.Fid.ToString() + ".UED"));

                using (Database db = new Database(
                    Path.Combine(
                        dataDirectory.ToString(), 
                        RELATIVE_PATH_TO_UED, 
                        loc.Fid.ToString() + ".UED")))
                {
                    db.set_geolocation(
                        loc.Latitude,
                        loc.Longitude,
                        loc.Elevation,
                        DEFAULT_SCREENING_HEIGHT,
                        loc.Fid.ToString(),
                        loc.Fid.ToString(),
                        "",
                        "",
                        "",
                        "Created with scriptConvertHistoricWeatherCsvToUed");

                    // Loop through all folders and read location's csv file
                    foreach(string year in Years)
                    {
                        string dirName = "y" + year;
                        string fileName = "year" + year + "_" + 
                            loc.Fid.ToString() + ".csv";
                        FileInfo file = new FileInfo(Path.Combine(
                            dataDirectory.ToString(), 
                            dirName, 
                            fileName));

                        using (TextReader reader = File.OpenText(
                            file.ToString()))
                        {
                            CsvReader csv = new CsvReader(reader);
                            csv.Configuration.RegisterClassMap<WeatherClassMap>();
                            var weather = csv.GetRecords<WeatherData>();

                            foreach(var w in weather)
                            {
                                // Set rH max
                                db.set_for_date(
                                    (float)w.MaximumRelativeHumidity,
                                    getDateInt(year, w.DayOfYear),
                                    (uint)Ued.Core.UedVariableCode.Weather_RelativeHumidityMax,
                                    (uint)Ued.Core.UedUnitCode.Percent,
                                    (uint)Ued.Core.UedQualityCode.calculated_quality);

                                // Set rH min
                                db.set_for_date(
                                    (float)w.MinimumRelativeHumidity,
                                    getDateInt(year, w.DayOfYear),
                                    (uint)Ued.Core.UedVariableCode.Weather_RelativeHumidityMin,
                                    (uint)Ued.Core.UedUnitCode.Percent,
                                    (uint)Ued.Core.UedQualityCode.calculated_quality);

                                // Set solar rad
                                db.set_for_date(
                                    (float)w.SolarRadiation,
                                    getDateInt(year, w.DayOfYear),
                                    (uint)Ued.Core.UedVariableCode.Weather_SolarRadiation,
                                    (uint)Ued.Core.UedUnitCode.MegaJoulesPerSquareMeterDay,
                                    (uint)Ued.Core.UedQualityCode.calculated_quality);

                                // Set wind speed
                                db.set_for_date(
                                    (float)w.WindSpeed,
                                    getDateInt(year, w.DayOfYear),
                                    (uint)Ued.Core.UedVariableCode.Weather_WindSpeed,
                                    (uint)Ued.Core.UedUnitCode.MetersPerSecond,
                                    (uint)Ued.Core.UedQualityCode.calculated_quality);

                                // Set temp max
                                db.set_for_date(
                                    (float)w.MaximumTemperature,
                                    getDateInt(year, w.DayOfYear),
                                    (uint)Ued.Core.UedVariableCode.Weather_TemperatureMax,
                                    (uint)Ued.Core.UedUnitCode.CelsiusDegree,
                                    (uint)Ued.Core.UedQualityCode.calculated_quality);

                                // Set temp min
                                db.set_for_date(
                                    (float)w.MinimumTemperature,
                                    getDateInt(year, w.DayOfYear),
                                    (uint)Ued.Core.UedVariableCode.Weather_TemperatureMin,
                                    (uint)Ued.Core.UedUnitCode.CelsiusDegree,
                                    (uint)Ued.Core.UedQualityCode.calculated_quality);

                                // Set precip
                                db.set_for_date(
                                    (float)w.Precipitation,
                                    getDateInt(year, w.DayOfYear),
                                    (uint)Ued.Core.UedVariableCode.Weather_Precipitation,
                                    (uint)Ued.Core.UedUnitCode.Milimeter,
                                    (uint)Ued.Core.UedQualityCode.calculated_quality);
                            }
                        }
                    }

                    db.close();
                }

                ///     Add to Ueds
                //uedFiles.Add(uedFile);
            } 
            

        }
        static int getDateInt(string year, int dayOfYear)
        {
            return (1000 * Convert.ToInt32(year)) + dayOfYear;
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
