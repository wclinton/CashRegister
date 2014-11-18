using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using CashRegister.Databases;
using Newtonsoft.Json;

namespace CashRegister
{
    class Program
    {
        private static bool loadData;

        private static void Main(string [] args)
        {

            ParseArgs(args);

            if (loadData)
            { 
                LoadData();
            }

            Console.WriteLine("Loading the current order...");          
            var order = ReadFromJson<Order>("orders.json");
            Console.WriteLine("Processing the order...");          
            var cr = new CashRegister(new CashRegisterContext());

            var total =cr.PriceOrder(order);

            Console.WriteLine("The total for the order is: {0}",total.ToString("C"));            
        }

        private static void ParseArgs(IEnumerable<string> args)
        {
            foreach (var arg in args)
            {
                if (String.Compare("-loaddata", arg.Trim(), StringComparison.OrdinalIgnoreCase) == 0)
                    loadData = true;
            }
        }

        private static void LoadData()
        {
            using (var db = new CashRegisterContext())
            {

                Console.WriteLine("Clearing the Database...");            
                //Clear the database 
                db.Items.RemoveRange(db.Items);
                db.ItemPrices.RemoveRange(db.ItemPrices);
                db.Coupons.RemoveRange(db.Coupons);
                db.ItemDiscounts.RemoveRange(db.ItemDiscounts);

                Console.WriteLine("Loading Items...");    
                db.Items.AddRange(GetSampleItems());
                Console.WriteLine("Loading ItemPrices...");    
                db.ItemPrices.AddRange(GetSampleItemPrices());
                Console.WriteLine("Loading Coupons...");    
                db.Coupons.AddRange(GetSampleCoupons());
                Console.WriteLine("Loading Disounts...");    
                db.ItemDiscounts.AddRange(GetItemDiscounts());
                db.SaveChanges();
            }         
        }



        private static IEnumerable<Item> GetSampleItems()
        {
            return ReadCollectionFromJson<Item>("items.json");

        }

        private static IEnumerable<ItemPrice> GetSampleItemPrices()
        {
            var x = ReadCollectionFromJson<ItemPrice>("itemprices.json");
            return x;
        }

     
        private static IEnumerable<Coupon> GetSampleCoupons()
        {
            var x = ReadCollectionFromJson<Coupon>("coupons.json");

            return x;
        }
        

        private static IEnumerable<ItemDiscount> GetItemDiscounts()
        {
            var x = ReadCollectionFromJson<ItemDiscount>("itemdiscounts.json");

            return x;
        }


        static Collection<T> ReadCollectionFromJson<T>(string filename)
        {
            return ReadFromJson<Collection<T>>(filename);
        }

        static T ReadFromJson<T>(string filename)
        {
            using (var r = new StreamReader("Json\\"+filename))
            {
                var json = r.ReadToEnd();
                var items = JsonConvert.DeserializeObject<T>(json);

                return items;
            }
        }
    }
}
