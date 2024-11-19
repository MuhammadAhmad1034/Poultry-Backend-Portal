using Google.Cloud.Firestore;

namespace PoultryPro_Portal.Models
{
    [FirestoreData]
    public class Order
    {
        [FirestoreProperty]
        public string Id { get; set; }
        [FirestoreProperty]
        public DateTime Date { get; set; }
        [FirestoreProperty]
        public string Type { get; set; }
        [FirestoreProperty]
        public int Quantity { get; set; }
        [FirestoreProperty]
        public double Rate { get; set; }
        [FirestoreProperty]
        public double Total { get; set; }
        [FirestoreProperty]
        public string Status { get; set; }
    }
}
