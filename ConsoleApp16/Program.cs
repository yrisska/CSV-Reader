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
            using var sr = new StreamReader(filePath);
            sr.ReadLine();// Skip first line with field names
            string line;
            while ((line = sr.ReadLine()) != null)
            {
                // Line-by-line reading file and creating <Product> objects
                string[] values = line.Split(',');
                yield return new Product(values[0], values[1], values[2], values[3]);
            }
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            //Finding file
            Console.WriteLine("Please ensure that you have file \"products.csv\" in your \"Downloads\" directory.\nPress enter to continue");
            Console.ReadLine();
            string filePath = @"C:\Users\" + Environment.UserName + @"\Downloads\products.csv";
            if (!File.Exists(filePath))
            {
                Console.WriteLine("Cant find such file");
                Environment.Exit(1);
            }

            //Creating product reader object and product list
            var productReader = new ProductReader();
            List<Product> productList;

            //Filling list with ReadAll() method
            try
            {
                productList = new List<Product>(productReader.ReadAll(filePath));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

            //Output sorted by date products 
            Console.WriteLine("Products sorted by date:");
            foreach (var product in productList.OrderByDescending(product => product.ProductionDate))
                Console.WriteLine(product.ToString());
        }
    }
}
