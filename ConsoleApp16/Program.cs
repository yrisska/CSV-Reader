using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Reader
{
    internal class Product
    {
        private readonly ushort _id;
        private readonly string _productionCode;
        private readonly string _type;
        private readonly DateTime _productionDate;

        public DateTime ProductionDate => _productionDate;

        public Product(string id, string productionCode, string type, string productionDate)
        {
            this._id = ushort.Parse(id);
            this._productionCode = productionCode;
            this._type = type;
            this._productionDate = DateTime.ParseExact(productionDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        }

        public override string ToString()
        {
            return $"{_id}, {_productionCode}, {_type}, {_productionDate.ToShortDateString()}";
        }
    }

    internal class ProductReader
    {
        public IEnumerable<Product> ReadAll(string filePath)
        {
            StreamReader sr = null;
            string line;
            ushort lineNumber = 1;  //line counter for csv file

            //Opening a file
            try
            {
                sr = new StreamReader(filePath);
                sr.ReadLine(); // Skip first line with field names
            }
            catch (Exception exception)
            {
                System.Console.WriteLine(exception.Message);
                Environment.Exit(0);
            }

            // Line-by-line reading file
            while ((line = sr.ReadLine()) != null)
            {
                lineNumber++;
                Product product = null;

                //Making Product object from line
                try
                {
                    var values = line.Split(',');
                    product = new Product(values[0], values[1], values[2], values[3]);
                }
                catch (IndexOutOfRangeException)
                {
                    System.Console.WriteLine($"Line {lineNumber} in file is missing commas" +
                                             $"\nPress enter to continue (skip the line)");
                    System.Console.ReadLine();
                    continue;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message + $"\nIn file at line {lineNumber}" +
                                             $"\nPress enter to continue (skip the object)");
                    System.Console.ReadLine();
                    continue;
                }

                yield return product;
            }
            sr.Close();
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            //Finding file
            Console.WriteLine("(Enter \"q\" to quit the program, enter \"default\" to search in \"Downloads\" directory.)");
            var filePath = "";
            do
            {
                Console.WriteLine("Enter correct full file path:");
                var userInput = Console.ReadLine();

                switch (userInput)
                {
                    case "q":
                        Environment.Exit(0);
                        break;
                    case "default":
                        filePath = @"C:\Users\" + Environment.UserName + @"\Downloads\products.csv";
                        break;
                    default:
                        filePath = userInput;
                        break;
                }

            } while (!File.Exists(filePath));

            //Creating product reader object
            var productReader = new ProductReader();

            //Filling list with ReadAll() method
            var productList = new List<Product>(productReader.ReadAll(filePath));

            //Output sorted by date products 
            Console.WriteLine("Products sorted by date:");
            foreach (var product in productList.OrderByDescending(product => product.ProductionDate)) 
                Console.WriteLine(product.ToString());
        }
    }
}
