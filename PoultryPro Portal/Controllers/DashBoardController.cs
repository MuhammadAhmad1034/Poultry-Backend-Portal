using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;
using PoultryPro_Portal.Models;
using PoultryPro_Portal.Services;
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
        public async Task<IActionResult> GetTopSuppliersByCity(string city)
        {
            
            if (string.IsNullOrEmpty(city))
                return BadRequest("City parameter is required.");

            try
            {
                var supplierCollection = _firestoreDb.Collection("Suppliers");
                var query = supplierCollection
                    .WhereEqualTo("City", city)
                    .OrderBy("Commission") // Sort by Commission (or other criteria)
                    .Limit(10);

                var snapshot = await query.GetSnapshotAsync();

                var suppliers = snapshot.Documents
                    .Select(doc => doc.ConvertTo<SupplierModel>())
                    .ToList();

                return Json(suppliers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
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

        private static string GenerateRandomOrderType()
        {
            string[] orderTypes = { "Chicken/Broiler", "Chicken/Layer", "Eggs/Desi", "Hatchery", "Medicine", "Feed","Eggs/Broiler" };
            Random random = new Random();
            int index = random.Next(orderTypes.Length);
            return orderTypes[index];
        }

    

        private static string GenerateRandomProvince()
        {
            string[] provinces = {
        "Punjab", "Sindh", "Khyber Pakhtunkhwa", "Balochistan", "Gilgit-Baltistan", "Azad Jammu and Kashmir"
    };
            Random random = new Random();
            int index = random.Next(provinces.Length);
            return provinces[index];
        }

        private static string GenerateRandomCity(string province)
        {
            Dictionary<string, string[]> cities = new Dictionary<string, string[]>()
    {
        {"Punjab", new string[] {"Lahore", "Faisalabad", "Rawalpindi", "Gujranwala", "Multan", "Sargodha", "Sialkot", "Sheikhupura","Jhelum"}},
        {"Sindh", new string[] {"Karachi", "Hyderabad", "Sukkur", "Larkana", "Nawabshah"}},
        {"Khyber Pakhtunkhwa", new string[] {"Peshawar", "Abbottabad", "Kohat", "Bannu", "Swat"}},
        {"Balochistan", new string[] {"Quetta", "Khuzdar", "Turbat", "Sibi", "Noshki", "Chaman"}},
        {"Gilgit-Baltistan", new string[] {"Gilgit", "Skardu", "Naran", "Shigar", "Khaplu"}},
        {"Azad Jammu and Kashmir", new string[] {"Muzaffarabad", "Mirpur", "Bhimber", "Kotli", "Rawalakot"}}
    };

            Random random = new Random();
            string[] provinceCities = cities[province];
            int index = random.Next(provinceCities.Length);
            return provinceCities[index];
        }

        public async Task AddData()
        {
            List<OrderModel> dummyOrders = new List<OrderModel>();
            var random = new Random();
            string[] statusOptions = { "Unassigned", "Pending", "In Progress", "Completed" };
            for (int i = 1; i <= 50; i++)
            {
                string province = GenerateRandomProvince();
                dummyOrders.Add(new OrderModel
                {
                    OrderNo = $"ORD-{i:D3}",
                    BookerName = $"Customer {i}",
                    Phone = $"({random.Next(100, 999)})-{random.Next(100, 999)}-{random.Next(1000, 9999)}",
                    Status = statusOptions[random.Next(statusOptions.Length)],
                    OrderType = GenerateRandomOrderType(),
                    Address = $"Street {random.Next(1, 100)}",
                    province = province,
                    city = GenerateRandomCity(province)
                });
            }

            // Add each dummy order to Firestore
            foreach (OrderModel order in dummyOrders)
            {
                await _firestoreDb.Collection("Orders").AddAsync(order)
                    .ContinueWith(task =>
                    {
                        if (task.IsFaulted)
                        {
                            Console.WriteLine("Error adding order: " + task.Exception);
                        }
                        else
                        {
                            Console.WriteLine("Order added successfully: " + task.Result);
                        }
                    });
            }
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
