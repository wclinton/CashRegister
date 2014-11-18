using System;
using System.Collections.Generic;
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
        public decimal PriceOrder(Order order)
        {

            if (order == null || order.OrderItems == null || order.OrderItems.Count == 0)
                throw new Exception("Unable to calculate Order Price.");


            var total = 0M;

            foreach (var orderItem in order.OrderItems)
            {
                total += PriceItem(orderItem);
            }

            var discount = CalculateCouponsDiscount(total, order.CouponCodes);

            total = total - discount;

            return total;
        }

  


        decimal PriceItem(OrderItem orderItem)
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
                    quantity = quantity - itemDiscount.DiscountQuantity;
            }

            return quantity * itemPrice.Price;
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
