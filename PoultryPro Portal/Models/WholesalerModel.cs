using Google.Cloud.Firestore;
using static Google.Cloud.Firestore.V1.StructuredQuery.Types;

namespace PoultryPro_Portal.Models
{
    [FirestoreData]
    public class WholesalerModel
    {
        
        public string Id { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string Address { get; set; }
        [FirestoreProperty]
        public string City { get; set; }
        [FirestoreProperty]
        public string Contact { get; set; }
        [FirestoreProperty]
        public double TotalRevenue { get; set; }
        [FirestoreProperty]
        public List<Order> CurrentOrders { get; set; }
        [FirestoreProperty]
        public List<Order> OrderHistory { get; set; }
    }
}
