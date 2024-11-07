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
