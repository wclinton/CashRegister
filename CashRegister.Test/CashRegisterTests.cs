using System;
using System.Collections.Generic;
using System.Linq;
using CashRegister.Databases;
using CashRegister.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace CashRegister.Test
{
    [TestClass]
    public class CashRegisterTests
    {

        private Mock<IItemService> mockItemService;
        private Mock<IItemPriceService> mockItemPriceService;
        private Mock<IItemDiscountService> mockItemDiscountService;
        private Mock<ICouponService> mockCouponService;

        private CashRegister cashRegister;


        [TestInitialize]

        public void Setup()
        {

            mockItemService = new Mock<IItemService>();
            mockItemPriceService = new Mock<IItemPriceService>();
            mockItemDiscountService = new Mock<IItemDiscountService>();
            mockCouponService = new Mock<ICouponService>();

            SetCoupons();
            SetItemPrices();
            SetItems();

            cashRegister = new CashRegister(mockItemService.Object, mockItemDiscountService.Object, mockItemPriceService.Object,
                mockCouponService.Object);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void CannotPriceNonExistingItem()
        {
            var orderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    ItemId = 9999,
                    Quantity = 3,
                    Uom = "each"
                },               
            };

            var order = new Order
            {
                OrderItems = orderItems,
            };

            cashRegister.ProcessOrder(order);

        }

        [TestMethod]
        public void CanPriceOneItem()
        {
            var orderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    ItemId = 1,
                    Quantity = 3,
                    Uom = "each"
                },               
            };

            var order = new Order
            {
                OrderItems = orderItems,
            };

            var processOrder = cashRegister.ProcessOrder(order);
            Assert.AreEqual(3, processOrder.Total);
        }

        [TestMethod]
        public void CanPriceMultpleItem()
        {



            var orderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    ItemId = 1,
                    Quantity = 3,
                    Uom = "each"
                },
                 new OrderItem
                {
                    ItemId = 2,
                    Quantity = 6,
                    Uom = "each"
                }
            };

            var order = new Order
            {
                OrderItems = orderItems,
            };

            var processedOrder = cashRegister.ProcessOrder(order);

            Assert.AreEqual(15, processedOrder.Total);
        }

        [TestMethod]

        public void CanPriceMultpleItemAndUom()
        {




            var orderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    ItemId = 1,
                    Quantity = 3,
                    Uom = "each"
                },
                 new OrderItem
                {
                    ItemId = 1,
                    Quantity = 1.5M,
                    Uom = "lb"
                }
            };

            var order = new Order
            {
                OrderItems = orderItems,
            };

            var processOrder = cashRegister.ProcessOrder(order);

            Assert.AreEqual(63, processOrder.Total);
        }

        [TestMethod]
        public void CanHandleSingleCoupon()
        {



            var orderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    ItemId = 1,
                    Quantity = 3,
                    Uom = "each",
                },
                 new OrderItem
                {
                    ItemId = 2,
                    Quantity = 6,
                    Uom = "each",
                }
            };

            var order = new Order
            {
                OrderItems = orderItems,
                CouponCodes = new List<string> { "BUY10SAVE1" }
            };

            var processOrder = cashRegister.ProcessOrder(order);

            Assert.AreEqual(14, processOrder.Total);
        }

        [TestMethod]
        public void CanHandleItemDiscount()
        {
            SetItemDiscounts();
         
            //Should get a discount here of buy 50 get 5 free - so 50x40 = 2000
            var processedOrder = cashRegister.ProcessOrder(GetTestOrder(1,55,"lb"));           
            Assert.AreEqual(2000, processedOrder.Total);

            //Should get a discount here too - but total should be the same = 2000
            processedOrder = cashRegister.ProcessOrder(GetTestOrder(1, 51, "lb"));
            Assert.AreEqual(2000, processedOrder.Total);

            //Should get a discount of 5 items so should only pay for 51 items = 2040
            processedOrder = cashRegister.ProcessOrder(GetTestOrder(1, 56, "lb"));           
            Assert.AreEqual(2040, processedOrder.Total);

            //Should still only get a discount of 5 items so should only pay for 145 items = 2040
            processedOrder = cashRegister.ProcessOrder(GetTestOrder(1, 150, "lb"));
            Assert.AreEqual(5800, processedOrder.Total);

        }

        private static Order GetTestOrder(int itemId, decimal quantity, string uom)
        {
            var orderItems = new List<OrderItem>
            {
                new OrderItem
                {
                    ItemId = itemId,
                    Quantity = quantity,
                    Uom = uom,
                },
            };

            var order = new Order
            {
                OrderItems = orderItems,
            };
            return order;
        }


        private void SetItems()
        {
            mockItemService.Setup(x => x.Get(It.IsAny<int>()))
                .Returns((int itemId) => GetItem(itemId));
        }


        private Item GetItem(int id)
        {
            return GetTestItems().SingleOrDefault(x => x.Id == id);
        }

        private List<Item> GetTestItems()
        {
            var items = new List<Item>
            {
                new Item
                {
                    Name = "Test Item1",
                    Id = 1,
                },
                new Item
                {
                    Name = "Test Item2",
                    Id = 2,
                },
            };
            return items;
        }

        private void SetItemPrices()
        {
            mockItemPriceService.Setup(x => x.GetItemPriceByItemId(It.IsAny<int>(), It.IsAny<string>()))
                .Returns((int itemId, string uom) => GetItemPriceByItemId(itemId, uom
                    ));
        }

        private static ItemPrice GetItemPriceByItemId(int itemId, string uom)
        {
            return GetTestItemPrices().SingleOrDefault(x => x.ItemId == itemId && x.Uom == uom);
        }

        private static IEnumerable<ItemPrice> GetTestItemPrices()
        {
            var itemPrices = new List<ItemPrice>
            {
                new ItemPrice
                {                   
                  
                    ItemId = 1,
                    Price = 1M,
                    Uom = "each"
                },

                 new ItemPrice
                {                   
                   
                    ItemId = 1,
                    Price = 40M,
                    Uom = "lb"
                },
                new ItemPrice
                {                    
                  
                    ItemId = 2,
                    Price = 2,
                    Uom = "each"
                },
                  new ItemPrice
                {            
                    ItemId = 9999,
                    Price = 3,
                    Uom = "each"
                }
            };
            return itemPrices;
        }

        private void SetCoupons()
        {
            mockCouponService.Setup(x => x.GetCoupon(It.IsAny<string>())).Returns((string code) => GetCoupon(code));
        }


        private Coupon GetCoupon(string code)
        {
            return GetTestCoupons().SingleOrDefault(x => x.Code == code);
        }

        private static IEnumerable<Coupon> GetTestCoupons()
        {
            var coupons = new List<Coupon>
            {
                new Coupon
                {
                    Code = "BUY10SAVE1",
                    MinValue = 10.0M,
                    Discount = 1M,
                },
                new Coupon
                {
                    Code = "BUY50SAVE6",
                    MinValue = 50.0M,
                    Discount = 6M,
                },

                new Coupon
                {
                    Code = "BUY100SAVE15",
                    MinValue = 100.0M,
                    Discount = 15M,
                },
            };
            return coupons;
        }

        private void SetItemDiscounts()
        {
            mockItemDiscountService.Setup(x => x.GetItemDiscountByItemId(It.IsAny<int>(), It.IsAny<string>())).Returns((int itemId, string code) => GetItemDiscount(itemId, code));
        }

        private static ItemDiscount GetItemDiscount(int itemId, string uom)
        {
            return GetTestItemDiscounts().SingleOrDefault(x => x.ItemId == itemId && x.Uom == uom);
        }

        private static IEnumerable<ItemDiscount> GetTestItemDiscounts()
        {
            var discounts = new List<ItemDiscount>
            {
                new ItemDiscount
                {
                   ItemId = 1,
                   Uom = "lb",
                   MinQuantity = 50,
                   DiscountQuantity = 5,
                },
            };
            return discounts;
        }
    }
}
