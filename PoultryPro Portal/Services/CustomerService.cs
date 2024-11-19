using Google.Cloud.Firestore;
using PoultryPro_Portal.Models;

namespace PoultryPro_Portal.Services
{
    public class CustomerService
    {
        private readonly FirestoreDb _firestoreDb;

        public CustomerService(FirestoreDb firestoreDb)
        {
            _firestoreDb = firestoreDb;
        }
       

        public async Task<List<SupplierModel>> GetSuppliersAsync(int page = 1, int itemsPerPage = 10)
        {
            try
            {
                var supplierCollection = _firestoreDb.Collection("Suppliers")
                                         .Limit(itemsPerPage)
                                         .Offset((page - 1) * itemsPerPage);
                var suppliersSnapshot = await supplierCollection.GetSnapshotAsync();
                return suppliersSnapshot.Documents.Select(d =>
                {
                    var supplier = d.ConvertTo<SupplierModel>();
                    supplier.Id = d.Id; // Make sure to set the document ID
                    return supplier;
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting suppliers: {ex.Message}");
                throw;
            }
        }

        public async Task<List<WholesalerModel>> GetWholesalersAsync()
        {
            try
            {
                var wholesalersSnapshot = await _firestoreDb.Collection("Wholesalers").GetSnapshotAsync();
                return wholesalersSnapshot.Documents.Select(d =>
                {
                    var wholesaler = d.ConvertTo<WholesalerModel>();
                    wholesaler.Id = d.Id; // Make sure to set the document ID
                    return wholesaler;
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting wholesalers: {ex.Message}");
                throw;
            }
        }

        public async Task<List<SupplierModel>> SearchSuppliersAsync(string query, string searchType)
        {
            try
            {
                var suppliersRef = _firestoreDb.Collection("Suppliers");
                Query firestoreQuery = suppliersRef;

                if (!string.IsNullOrEmpty(query))
                {
                    firestoreQuery = suppliersRef.WhereGreaterThanOrEqualTo(searchType, query)
                                               .WhereLessThanOrEqualTo(searchType, query + "\uf8ff");
                }

                var querySnapshot = await firestoreQuery.GetSnapshotAsync();
                return querySnapshot.Documents.Select(d =>
                {
                    var supplier = d.ConvertTo<SupplierModel>();
                    supplier.Id = d.Id;
                    return supplier;
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching suppliers: {ex.Message}");
                throw;
            }
        }

        public async Task<List<WholesalerModel>> SearchWholesalersAsync(string query, string searchType)
        {
            try
            {
                var wholesalersRef = _firestoreDb.Collection("Wholesalers");
                Query firestoreQuery = wholesalersRef;

                if (!string.IsNullOrEmpty(query))
                {
                    firestoreQuery = wholesalersRef.WhereGreaterThanOrEqualTo(searchType, query)
                                                 .WhereLessThanOrEqualTo(searchType, query + "\uf8ff");
                }

                var querySnapshot = await firestoreQuery.GetSnapshotAsync();
                return querySnapshot.Documents.Select(d =>
                {
                    var wholesaler = d.ConvertTo<WholesalerModel>();
                    wholesaler.Id = d.Id;
                    return wholesaler;
                }).ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error searching wholesalers: {ex.Message}");
                throw;
            }
        }


      
        public async Task<SupplierModel> GetSupplierDetailsAsync(string id)
        {
            var docSnapshot = await _firestoreDb.Collection("Suppliers").Document(id).GetSnapshotAsync();
            if (!docSnapshot.Exists)
            {
                return null;
            }
            var supplier = docSnapshot.ConvertTo<SupplierModel>();
            supplier.Id = docSnapshot.Id;
            return supplier;
        }

        public async Task<WholesalerModel> GetWholesalerDetailsAsync(string id)
        {
            var docSnapshot = await _firestoreDb.Collection("Wholesalers").Document(id).GetSnapshotAsync();
            if (!docSnapshot.Exists)
            {
                return null;
            }
            var wholesaler = docSnapshot.ConvertTo<WholesalerModel>();
            wholesaler.Id = docSnapshot.Id;
            return wholesaler;
        }


        public async Task SeedDataAsync()
        {
            var suppliersCollection = _firestoreDb.Collection("Suppliers");
            var wholesalersCollection = _firestoreDb.Collection("Wholesalers");

            var cities = new[]
            {
        "Lahore", "Karachi", "Faisalabad", "Rawalpindi", "Gujranwala", "Peshawar", "Multan", "Hyderabad", "Islamabad", "Quetta",
        "Bahawalpur", "Sargodha", "Sialkot", "Sukkur", "Larkana", "Sheikhupura", "Rahim Yar Khan", "Mardan", "Gujrat", "Nawabshah"
    };

            var supplierTypes = new[] { "Broiler Supplier", "Layer Supplier", "Egg Supplier" };
            var wholesalerTypes = new[] { "Punjab Wholesalers", "Sindh Wholesalers", "KPK Wholesalers" };

            for (int i = 1; i <= 50; i++)
            {
                var supplier = new SupplierModel
                {
                    Name = $"Supplier {i}",
                    Type = GetRandomSupplierType(supplierTypes),
                    Commission = GetRandomCommission(),
                    City = GetRandomCity(cities),
                    Contact = GetRandomPhoneNumber(),
                    Inventory = GetRandomInventory(),
                    Sheds = GetRandomSheds(cities),
                    Orders = GetRandomOrders()
                };

                await suppliersCollection.AddAsync(supplier);
            }

            for (int i = 1; i <= 50; i++)
            {
                var wholesaler = new WholesalerModel
                {
                    Name = $"{GetRandomWholesalerType(wholesalerTypes)} {i}",
                    City = GetRandomCity(cities),
                    Contact = GetRandomPhoneNumber(),
                    TotalRevenue = GetRandomRevenue(),
                    CurrentOrders = GetRandomCurrentOrders(),
                    OrderHistory = GetRandomOrderHistory()
                };

                await wholesalersCollection.AddAsync(wholesaler);
            }
        }

        private string GetRandomSupplierType(string[] supplierTypes)
        {
            Random random = new Random();
            int index = random.Next(supplierTypes.Length);
            return supplierTypes[index];
        }

        private double GetRandomCommission()
        {
            Random random = new Random();
            return random.NextDouble() * 10;
        }

        private string GetRandomCity(string[] cities)
        {
            Random random = new Random();
            int index = random.Next(cities.Length);
            return cities[index];
        }

        private string GetRandomPhoneNumber()
        {
            Random random = new Random();
            string phoneNumber = $"({random.Next(100, 999)})-{random.Next(100, 999)}-{random.Next(1000, 9999)}";
            return phoneNumber;
        }

        private InventoryModel GetRandomInventory()
        {
            Random random = new Random();
            return new InventoryModel
            {
                Eggs = new InventoryItem { Quantity = random.Next(1000, 10000), Type = "Desi" },
                Chicken = new InventoryItem { Quantity = random.Next(1000, 10000), Type = "Broiler" },
                Feed = new InventoryItem { Quantity = random.Next(1000, 10000), Type = "Layer Feed" },
                Hatchery = new InventoryItem { Quantity = random.Next(1000, 10000), Type = "Hatching Eggs" },
                Medicine = new InventoryItem { Quantity = random.Next(100, 1000), Type = "Vaccines" }
            };
        }

        private List<ShedModel> GetRandomSheds(string[] cities)
        {
            Random random = new Random();
            List<ShedModel> sheds = new List<ShedModel>();

            for (int i = 1; i <= random.Next(1, 5); i++)
            {
                sheds.Add(new ShedModel
                {
                    Id = i,
                    Type = "Broiler",
                    Capacity = random.Next(1000, 10000),
                    Current = random.Next(1000, 10000),
                    Address = $"{random.Next(1, 100)} {GetRandomCity(cities)}"
                });
            }

            return sheds;
        }

        private List<Order> GetRandomOrders()
        {
            Random random = new Random();
            List<Order> orders = new List<Order>();

            for (int i = 1; i <= random.Next(1, 5); i++)
            {
                orders.Add(new Order
                {
                    Id = $"order{i}",
                    Date = DateTime.UtcNow,
                    Type = "Broiler",
                    Quantity = random.Next(100, 1000),
                    Rate = random.Next(100, 1000),
                    Total = random.Next(10000, 100000),
                    Status = "Completed"
                });
            }

            return orders;
        }

        private string GetRandomWholesalerType(string[] wholesalerTypes)
        {
            Random random = new Random();
            int index = random.Next(wholesalerTypes.Length);
            return wholesalerTypes[index];
        }

        private double GetRandomRevenue()
        {
            Random random = new Random();
            return random.NextDouble() * 1000000;
        }

        private List<Order> GetRandomCurrentOrders()
        {
            Random random = new Random();
            List<Order> orders = new List<Order>();

            for (int i = 1; i <= random.Next(1, 5); i++)
            {
                orders.Add(new Order
                {
                    Id = $"order{i}",
                    Date = DateTime.UtcNow,
                    Type = "Broiler",
                    Quantity = random.Next(100, 1000),
                    Rate = random.Next(100, 1000),
                    Total = random.Next(10000, 100000),
                    Status = "Pending"
                });
            }

            return orders;
        }

        private List<Order> GetRandomOrderHistory()
        {
            Random random = new Random();
            List<Order> orders = new List<Order>();

            for (int i = 1; i <= random.Next(1, 5); i++)
            {
                orders.Add(new Order
                {
                    Id = $"order{i}",
                    Date = DateTime.UtcNow.AddMonths(-random.Next(1, 12)),
                    Type = "Broiler",
                    Quantity = random.Next(100, 1000),
                    Rate = random.Next(100, 1000),
                    Total = random.Next(10000, 100000),
                    Status = "Completed"
                });
            }

            return orders;
        }
    }
}
