using System;
using System.Collections.Generic;
using System.Linq;
using CashRegister.Databases;
using CashRegister.Services;

namespace CashRegister
{
    public class CashRegister
    {      
       private readonly IItemService itemService;
        private readonly IItemDiscountService itemDiscountService;
        private readonly IItemPriceService itemPriceService;
        private readonly ICouponService couponService;

        public CashRegister(IContext db)
        {
            itemService = new ItemService(db);
            itemDiscountService = new ItemDiscountService(db);
            itemPriceService = new ItemPriceService(db);
            couponService = new CouponService(db);
        }

        public CashRegister(IItemService itemService, IItemDiscountService itemDiscountService, IItemPriceService itemPriceService, ICouponService couponService)
        {
            this.itemService = itemService;
            this.itemDiscountService = itemDiscountService;
            this.itemPriceService = itemPriceService;
            this.couponService = couponService;
        }

     

        /// <summary>
        /// Prices out the total of an order - constisting of item ids qu
        /// 
        /// Each order may contain multiple coupons but it is assumed that since the coupons are only discounts on price. 
        /// Than only one coupon will apply at a time and that the highest value Coupon will be used.
        /// 
        /// </summary>
        /// <param name="order"></param>
        /// <returns></returns>
        public ProcessedOrder ProcessOrder(Order order)
        {

            if (order == null || order.OrderItems == null || order.OrderItems.Count == 0)
                throw new Exception("Unable to calculate Order Price.");


            var total = 0M;

            var processedItems = new List<ProcessedOrderItem>();

            foreach (var orderItem in order.OrderItems)
            {
                var processedItem = PriceItem(orderItem);

                processedItems.Add(processedItem);

                total += processedItem.Total;
            }

            var discount = CalculateCouponsDiscount(total, order.CouponCodes);

            total = total - discount;

            return new ProcessedOrder(order, processedItems,total);
        }

        public void ShowOrder(ProcessedOrder processedOrder)
        {

            Console.WriteLine("{0,-5} {1,-20} {2,-9} {3,-5} {4,-9} {5,-9} {6}", "Id", "Item", "Price", "UOM", "Qty", "Total", "Discount Applied");
            foreach (var item in processedOrder.ProcessedOrderItems)
            {

                var hasDiscountMarker = "";
                if (item.HasDiscount)
                    hasDiscountMarker = "*";

                var s = string.Format("{0,-5} {1,-20} {2,-9:C} {3,-5} {4,-9} {5,-9:C} {6}", item.Item.Id, item.Item.Name, item.Price, item.Uom, item.Quantity, item.Total,hasDiscountMarker);




                Console.WriteLine(s);
            }

            Console.WriteLine("");

            var codes = processedOrder.CouponCodes.Aggregate("", (current, code) => current + (code + " "));

            Console.WriteLine("Coupons: {0}",codes);

            Console.WriteLine("{0,-5} {1,-20} {2,-9} {3,-5} {4,-9} {5,-9:C}", "Total", "", "", "", "", processedOrder.Total);
        }

  


        ProcessedOrderItem PriceItem(OrderItem orderItem)
        {       
            var item = itemService.Get(orderItem.ItemId);

            if (item == null)            
                throw new Exception("Item not found with id: " + orderItem.ItemId);            

            var itemPrice = itemPriceService.GetItemPriceByItemId(orderItem.ItemId, orderItem.Uom);

            if (itemPrice == null)
                throw new Exception("price cannot be found for item id: " + orderItem.ItemId + " uom:"+ orderItem.Uom);
            var itemDiscount = itemDiscountService.GetItemDiscountByItemId(orderItem.ItemId, orderItem.Uom);

            var quantity = orderItem.Quantity;

            if (itemDiscount != null)
            {
                if (orderItem.Quantity > itemDiscount.MinQuantity)
                {
                    if (orderItem.Quantity > itemDiscount.MinQuantity + itemDiscount.DiscountQuantity)
                    {
                        //subtract the whole thing
                        quantity = quantity - itemDiscount.DiscountQuantity;
                    }
                    else
                    {
                        quantity = itemDiscount.MinQuantity;
                    }                   
                }
            }

            var processedOrderItem = new ProcessedOrderItem
            {
                Item = item,
                Quantity = orderItem.Quantity,
                HasDiscount = quantity != orderItem.Quantity,
                Total = quantity*itemPrice.Price,
                Uom = orderItem.Uom,
                Price = itemPrice.Price,
            };



            return processedOrderItem;
        }

//        ItemDiscount GetItemDiscountByItemId(int id, string uom)
//        {
//            return db.ItemDiscounts.Single(x => x.ItemId == id && x.Uom == uom);
//        }


//        ItemPrice GetItemPriceByItemId(int id, string uom)
//        {
//
//          
//            return db.ItemPrices.SingleOrDefault(x => x.ItemId == id && x.Uom == uom);
//        }


        decimal CalculateCouponsDiscount(decimal total, IEnumerable<string> couponCodes)
        {
            if (couponCodes == null)
                return 0;

            var maxDiscount = 0M;
            foreach (var couponCode in couponCodes)
            {
                var discount = CalculateCouponDiscount(total, couponCode);

                maxDiscount = Math.Max(discount, maxDiscount);
            }

            return maxDiscount;
        }

        decimal CalculateCouponDiscount(decimal total, string couponCode)
        {
            var coupon = couponService.GetCoupon(couponCode);

            if (coupon == null) return 0;
            return total >= coupon.MinValue ? coupon.Discount : 0;
        }


       
    }
}
