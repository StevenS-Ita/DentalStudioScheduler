using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Mch.Internal.UnifiedContextDb.Test
{
    [TestFixture]
    public class OrderTest : BaseClassTest
    {
        [Test]
        public void BogusGenerateOrders_Should_SetPropertiesCorrectly()
        {
            var orders = _fakeDataGenerate.GenerateOrders(50);

            var firstOrder = orders.FirstOrDefault();

            Assert.Multiple(() =>
            {
                Assert.AreEqual("ORD-", firstOrder.OrderNr.Substring(0, 4));
                Assert.IsTrue(firstOrder.Priority >= 1 && firstOrder.Priority <= 5);
                Assert.IsNotNull(firstOrder.OrderStatus);
                Assert.IsTrue(firstOrder.Quantity >= 1 && firstOrder.Quantity <= 1000);
                Assert.IsNotNull(firstOrder.PickingLabel);
                Assert.IsTrue(firstOrder.CreatedAt <= DateTime.Now);
                Assert.IsTrue(firstOrder.RequestedDeliveryAt >= DateTime.Now);
                Assert.IsTrue(firstOrder.CNC_Order != null);
            });
        }

        [Test]
        public void WriteReadOrders_Should_WorkCorrectly()
        {
            var orders = _fakeDataGenerate.GenerateOrders(1);
            var order = orders.First();

            var retrieved = _context.Orders
                .Include(o => o.Article)
                .Include(o => o.Commission)
                .Include(o => o.CNC)
                .Include(o => o.CNC_Order)
                .FirstOrDefault(o => o.OrderId == order.OrderId);

            Assert.Multiple(() =>
            {
                Assert.IsNotNull(retrieved);
                Assert.AreEqual(order.OrderNr, retrieved.OrderNr);
                Assert.AreEqual(order.Quantity, retrieved.Quantity);
                Assert.AreEqual(order.ArticleId, retrieved.ArticleId);
                Assert.AreEqual(order.CommissionId, retrieved.CommissionId);
                Assert.AreEqual(order.CNC_Id, retrieved.CNC_Id);
                Assert.IsTrue(retrieved.OrderNr.StartsWith("ORD-"));
                Assert.IsTrue(retrieved.Priority >= 1 && retrieved.Priority <= 5);
                Assert.IsNotNull(retrieved.OrderStatus);
            });
        }
    }
}
