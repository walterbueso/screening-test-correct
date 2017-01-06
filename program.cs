using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace berndt_screening_test
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"C:\Users\walte\Desktop\berndt-screening-test\berndt-screening-test\access.log";
            //string csvpath = @"C:\Users\walte\Desktop\berndt-screening-test\berndt-screening-test\report.csv";

            StreamReader file = new StreamReader(File.OpenRead(path));
            string ex = @"^((25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)$";
            Regex reg = new Regex(ex);
            int counter = 0;
            string line;
            string[] bline;
            var ips = new List<string>();


            while ((line = file.ReadLine()) != null)
            {

                bline = line.Split(' ');
                /*
                Add current ip to ips list if these conditions are met:
                -Current line length being read is = 21
                -element on bline[2] (client ip) matches the regular expression
                -the current ip does not start with 207.114
                -current port = 80
                -current method = GET
                */
                if (bline.Length == 21 && reg.IsMatch(bline[2]) && !bline[2].StartsWith("207.114") && bline[7].Equals("80") && bline[8].Equals("GET"))
                {
                    ips.Add(bline[2]);
                }
                counter++;

            }

            var s = ips.OrderByDescending(Version.Parse)
                 .GroupBy(x => x)
                 .Select(g => new { Value = g.Key, Count = g.Count() })
                 .OrderByDescending(x => x.Count);

            foreach (var i in s)
            {
                Console.WriteLine("{0}, \"{1}\"", i.Count, i.Value);
            }

            file.Dispose();
        }
    }
}
