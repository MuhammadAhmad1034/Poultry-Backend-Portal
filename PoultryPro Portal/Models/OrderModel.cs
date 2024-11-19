using Google.Cloud.Firestore;

namespace PoultryPro_Portal.Models
{
    [FirestoreData]
    public class OrderModel
    {
        [FirestoreProperty]
        public string OrderNo { get; set; }
        [FirestoreProperty]
        public string BookerName { get; set; }
        [FirestoreProperty]
        public string Phone { get; set; }
        [FirestoreProperty]
        public string Status { get; set; }
        [FirestoreProperty]
        public string OrderType { get; set; }
        [FirestoreProperty]
        public string Address { get; set; }
        [FirestoreProperty]
        public string city {  get; set; }
        [FirestoreProperty]
        public string province { get; set; }
    }
}
