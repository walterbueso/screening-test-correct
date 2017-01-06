using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace berndt_screening_test
{
    class Program
    {
        static void Main(string[] args)
        {
            StringBuilder csvContent = new StringBuilder();
            string csvpath = @"C:\Users\walte\Desktop\sc-test-answer\report.csv";
            
            var filePath = File.OpenRead("access.log");
            StreamReader file = new StreamReader(filePath);
            
            string ex = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            Regex reg = new Regex(ex);

            string fileLine;
            string[] file_key_arr;
            var ips = new List<string>();

            //Assuming access log file key position wont change
            //Read each line, split spaces, and add key to file_key_arr array
            while ((fileLine = file.ReadLine()) != null)
            {
                file_key_arr = fileLine.Split(' ');
               
               //Make sure each line in file has the key values and not just iis comments
               //Make sure ip mathces regular expression
               //Exclude ips with beginning with 207.112
               //Make sure port 80
               //make sure method = GET
                if (file_key_arr.Length == 21 && reg.IsMatch(file_key_arr[2]) && !file_key_arr[2].StartsWith("207.114") && file_key_arr[7].Equals("80") && file_key_arr[8].Equals("GET"))
                {
                    //Add IP to ips List 
                    ips.Add(file_key_arr[2]);
                }
            }

            //Sort by most request then by highest octets
            var sorted = ips.OrderByDescending(Version.Parse)
                 .GroupBy(x => x)
                 .Select(g => new { Value = g.Key, Count = g.Count() })
                 .OrderByDescending(x => x.Count);
            
            //Append to string builder
            foreach (var item in sorted)
            {
                Console.WriteLine("{0}, \"{1}\"", item.Count, item.Value);
                csvContent.AppendLine(item.Count + "," + item.Value);
            }

            //create new csv with report
            File.AppendAllText(csvpath, csvContent.ToString());

            file.Dispose();
        }
    }
}
