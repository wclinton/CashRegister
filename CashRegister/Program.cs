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
        private static void Main()
        {                   
            LoadData();
            var order = ReadFromJson<Order>("orders.json");
            var cr = new CashRegister(new CashRegisterContext());

            var total =cr.PriceOrder(order);

            Console.WriteLine("The total for the order is: {0}",total.ToString("C"));
            Console.ReadLine();          
        }

        private static void LoadData()
        {
            using (var db = new CashRegisterContext())
            {                
                //Clear the database 
                db.Items.RemoveRange(db.Items);
                db.ItemPrices.RemoveRange(db.ItemPrices);
                db.Coupons.RemoveRange(db.Coupons);
                db.ItemDiscounts.RemoveRange(db.ItemDiscounts);
               
                db.Items.AddRange(GetSampleItems());
                db.ItemPrices.AddRange(GetSampleItemPrices());
                db.Coupons.AddRange(GetSampleCoupons());
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
            using (var r = new StreamReader(filename))
            {
                var json = r.ReadToEnd();
                var items = JsonConvert.DeserializeObject<T>(json);

                return items;
            }
        }
    }
}
