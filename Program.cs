using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace CrimeAnalyzer
{
    class Program
    {
        private static List<Crimes> ValueList = new List<Crimes>();

        static void Main(string[] args)
        {
            var currentPath = Directory.GetCurrentDirectory();
            var csvPath = string.Empty;
            var reportPath = string.Empty;

            if (args.Length != 2)
            {
                Console.WriteLine("Please enter valid syntax : CrimeAnalyzer <crime_csv_file_path> <report_file_path>");
                Console.ReadLine();
                return;
            }
            else
            {
                csvPath = args[0];
                reportPath = args[1];

                if (!csvPath.Contains("\\"))
                {
                    csvPath = Path.Combine(currentPath, csvPath);
                }
                if (!reportPath.Contains("\\"))
                {
                    reportPath = Path.Combine(currentPath, reportPath);
                }
            }

            if (File.Exists(csvPath))
            {
                if (ReadFile(csvPath))
                {
                    try
                    {
                        var report = File.Create(reportPath);
                        report.Close();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Unable to create file. Error: " + e.Message);
                    }
                    WriteReport(reportPath);
                }
            }
            else
            {
                Console.Write("Crime data file not found.");
            }
            Console.ReadLine();
        }

        private static bool ReadFile(string filePath)
        {
            try
            {
                int length = 0;
                var lines = File.ReadAllLines(filePath);
                for (int index = 0; index < lines.Length; index++)
                {
                    var line = lines[index];
                    var data = line.Split(',');

                    if (index == 0)
                    {
                        length = data.Length;
                    }
                    else
                    {
                        if (length != data.Length)
                        {
                            return false;
                        }
                        else
                        {
                            try
                            {
                                Crimes values = new Crimes();
                                values.Year = Convert.ToInt32(data[0]);
                                values.Population = Convert.ToInt32(data[1]);
                                values.ViolentCrime = Convert.ToInt32(data[2]);
                                values.Murder = Convert.ToInt32(data[3]);
                                values.Rape = Convert.ToInt32(data[4]);
                                values.Robbery = Convert.ToInt32(data[5]);
                                values.AggravatedAssault = Convert.ToInt32(data[6]);
                                values.PropertyCrime = Convert.ToInt32(data[7]);
                                values.Burglary = Convert.ToInt32(data[8]);
                                values.Theft = Convert.ToInt32(data[9]);
                                values.MotorVehicleTheft = Convert.ToInt32(data[10]);
                                ValueList.Add(values);
                            }
                            catch (InvalidCastException)
                            {
                                Console.WriteLine($"Table includes data that is not the correct type");
                                return false;
                            }
                        }
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine("Error reading data from csv file. Error: " + e.Message);
                throw e;
            }
        }

        private static void WriteReport(string filePath)
        {
            try
            {
                if (ValueList.Any() && ValueList != null)
                {
                    // Header
                    StringBuilder builder = new StringBuilder();
                    builder.Append("Crime Analyzer Report");
                    builder.Append(Environment.NewLine);
                    builder.Append(Environment.NewLine);

                    //Question 1 and 2
                    var min = ValueList.Min(x => x.Year);
                    var max = ValueList.Max(x => x.Year);
                    var range = max - min + 1;
                    builder.Append($"Period: {min}-{max} ({range} years)");
                    builder.Append(Environment.NewLine);
                    builder.Append(Environment.NewLine);

                    //Question 3
                    var years = from values in ValueList
                                where values.Murder < 15000
                                select values.Year;

                    var yearsStr = string.Empty;
                    for (int i = 0; i < years.Count(); i++)
                    {
                        yearsStr += years.ElementAt(i).ToString();
                        if (i < years.Count() - 1)
                            yearsStr += ", ";
                    }
                    builder.Append($"Years murders per year < 15000: {yearsStr}");
                    builder.Append(Environment.NewLine);

                    //Question 4
                    var robberies = from values in ValueList
                                    where values.Robbery > 500000
                                    select values;

                    var robberiesStr = string.Empty;
                    for (int i = 0; i < robberies.Count(); i++)
                    {
                        Crimes values = robberies.ElementAt(i);
                        robberiesStr += $"{values.Year} = {values.Robbery}";
                        if (i < robberies.Count() - 1) robberiesStr += ", ";
                    }
                    builder.Append($"Robberies per year > 500000: {robberiesStr}");
                    builder.Append(Environment.NewLine);

                    //Question 5
                    var crimes = from values in ValueList
                                 where values.Year == 2010
                                 select values;

                    Crimes valueAt2010 = crimes.First();
                    var crimePerCapita = (double)valueAt2010.ViolentCrime / (double)valueAt2010.Population;
                    builder.Append($"Violent crime per capita rate (2010): {crimePerCapita}");
                    builder.Append(Environment.NewLine);

                    //Question 6
                    var avgMurder = ValueList.Sum(x => x.Murder) / ValueList.Count;
                    builder.Append($"Average murder per year (all years): {avgMurder}");
                    builder.Append(Environment.NewLine);

                    //Question 7
                    var murders94to97 = ValueList.Where(x => x.Year >= 1994 && x.Year <= 1997).Sum(y => y.Murder);
                    var avgMurder94to97 = murders94to97 / ValueList.Count;
                    builder.Append($"Average murder per year (1994-1997): {avgMurder94to97}");
                    builder.Append(Environment.NewLine);

                    //Question 8
                    var murders10to13 = ValueList.Where(x => x.Year >= 2010 && x.Year <= 2013).Sum(y => y.Murder);
                    var avgMurder10to13 = murders10to13 / ValueList.Count;
                    builder.Append($"Average murder per year (2010-2013): {avgMurder10to13}");
                    builder.Append(Environment.NewLine);

                    //Question 9
                    var minTheft = ValueList.Where(x => x.Year >= 1999 && x.Year <= 2004).Min(x => x.Theft);
                    builder.Append($"Minimum thefts per year (1999-2004): {minTheft}");
                    builder.Append(Environment.NewLine);

                    //Question 10
                    var maxTheft = ValueList.Where(x => x.Year >= 1999 && x.Year <= 2004).Max(x => x.Theft);
                    builder.Append($"Maximum thefts per year (1999-2004): {maxTheft}");
                    builder.Append(Environment.NewLine);

                    //Question 11
                    var maxVehicleTheft = ValueList.OrderByDescending(x => x.MotorVehicleTheft).First().Year;
                    builder.Append($"Year of highest number of motor vehicle thefts: {maxVehicleTheft}");
                    builder.Append(Environment.NewLine);

                    using (var stream = new StreamWriter(filePath))
                    {
                        stream.Write(builder.ToString());
                    }
                }
                else
                {
                    Console.WriteLine("File is empty.");
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                throw e;
            }
        }
    }
}