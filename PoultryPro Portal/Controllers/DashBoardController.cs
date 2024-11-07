using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using PoultryPro_Portal.Models;
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;

namespace PoultryPro_Portal.Controllers
{
    public class DashboardController : Controller
    {
        private readonly FirestoreDb _firestoreDb;

        public DashboardController(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }
        [HttpGet]
        public async Task<IActionResult> GetOrdersByType(string type)
        {
            var orders = new List<OrderModel>();
            var ordersCollection = _firestoreDb.Collection("Orders");

            // Query orders by type
            var query = ordersCollection.WhereEqualTo("OrderType", type);
            var querySnapshot = await query.GetSnapshotAsync();

            foreach (var doc in querySnapshot.Documents)
            {
                orders.Add(doc.ConvertTo<OrderModel>());
            }

            return Json(orders);
        }

        //private static string GenerateRandomOrderType()
        //{
        //    string[] orderTypes = { "Chicken/Broiler","Chicken/Layer","Eggs/Golden","Hatchery", "Medicine", "Chicken Feed", };
        //    Random random = new Random();
        //    int index = random.Next(orderTypes.Length);
        //    return orderTypes[index];
        //}
        //public async Task AddData()
        //{


        //    List<OrderModel> dummyOrders = new List<OrderModel>();
        //    var random = new Random();

        //    for (int i = 1; i <= 100; i++)
        //    {
        //        dummyOrders.Add(new OrderModel
        //        {
        //            OrderNo = $"ORD-{i:D3}",
        //            BookerName = $"Customer {i}",
        //            Phone = $"({random.Next(100, 999)})-{random.Next(100, 999)}-{random.Next(1000, 9999)}",
        //            Status = random.Next(3) == 0 ? "Completed" : (random.Next(2) == 0 ? "Processing" : "Pending"),
        //            OrderType = GenerateRandomOrderType(),
        //            Address = $"123 Main St, City {random.Next(1, 100)}"
        //        });
        //    }

        //    // Add each dummy order to Firestore
        //    foreach (OrderModel order in dummyOrders)
        //    {
        //        await _firestoreDb.Collection("Orders").AddAsync(order)
        //            .ContinueWith(task => {
        //                if (task.IsFaulted)
        //                {
        //                    Console.WriteLine("Error adding order: " + task.Exception);
        //                }
        //                else
        //                {
        //                    Console.WriteLine("Order added successfully: " + task.Result);
        //                }
        //            });
        //    }
        //}


        public IActionResult AgentDashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            // Query Firestore to get orders
            var orders = new List<OrderModel>();
            var ordersCollection = await _firestoreDb.Collection("Orders").GetSnapshotAsync();
            foreach (var doc in ordersCollection.Documents)
            {
                orders.Add(doc.ConvertTo<OrderModel>());
            }

            return Json(orders);
        }
        

        [HttpPost]
        public async Task<IActionResult> UpdateOrderStatus(string orderNo, string status)
        {
            try
            {
                var ordersCollection = _firestoreDb.Collection("Orders");
                var querySnapshot = await ordersCollection.WhereEqualTo("OrderNo", orderNo).GetSnapshotAsync();

                if (querySnapshot.Documents.Count == 0)
                    return NotFound("Order not found.");

                var orderDocument = querySnapshot.Documents[0].Reference;
                await orderDocument.UpdateAsync("Status", status);

                return Ok();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error updating order status: {ex.Message}");
                return StatusCode(500, "Error updating order status.");
            }
        }
        [HttpGet]
        public async Task<IActionResult> TestFirestoreConnection()
        {
            try
            {
                var testDoc = await _firestoreDb.Collection("Orders").Document("99x6apGyHcCWrgP6LQOP").GetSnapshotAsync();

                if (testDoc.Exists)
                {
                    return Ok("Firestore connection is successful!");
                }
                else
                {
                    return Ok("Firestore connected, but no test document found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Firestore connection error: {ex.Message}");
                return StatusCode(500, "Error connecting to Firestore.");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetOrderById(string orderNo)
        {
            var ordersCollection = _firestoreDb.Collection("Orders");
            var querySnapshot = await ordersCollection.WhereEqualTo("OrderNo", orderNo).GetSnapshotAsync();

            if (querySnapshot.Documents.Count == 0)
                return NotFound("Order not found");

            var order = querySnapshot.Documents[0].ConvertTo<OrderModel>();
            return Json(order);
        }


        [HttpGet]
        public async Task<IActionResult> GetFilteredOrders(string searchType, string searchTerm, string status)
        {
            var orders = new List<OrderModel>();
            var ordersCollection = _firestoreDb.Collection("Orders");

            Query query = ordersCollection;

            if (!string.IsNullOrEmpty(searchType) && !string.IsNullOrEmpty(searchTerm))
            {
                query = query.WhereEqualTo(searchType, searchTerm);
            }

            if (!string.IsNullOrEmpty(status) && status != "all")
            {
                query = query.WhereEqualTo("Status", status);
            }

            var querySnapshot = await query.GetSnapshotAsync();
            foreach (var doc in querySnapshot.Documents)
            {
                orders.Add(doc.ConvertTo<OrderModel>());
            }

            return Json(orders);
        }

        [HttpGet]
        public async Task<IActionResult> TestSingleDocument()
        {
            try
            {
                var docRef = _firestoreDb.Collection("Orders").Document("99x6apGyHcCWrgP6LQOP");
                var docSnapshot = await docRef.GetSnapshotAsync();

                if (docSnapshot.Exists)
                {
                    var order = docSnapshot.ConvertTo<OrderModel>();
                    return Json(order);
                }
                else
                {
                    return NotFound("Document not found.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Firestore error: {ex.Message}");
                return StatusCode(500, "Firestore connection error.");
            }
        }

    }
}
