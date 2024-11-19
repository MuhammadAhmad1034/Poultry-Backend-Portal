using Google.Cloud.Firestore;


namespace PoultryPro_Portal.Models
{
    [FirestoreData]
    public class SupplierModel
    {
        
        public string Id { get; set; }
        [FirestoreProperty]
        public string Name { get; set; }
        [FirestoreProperty]
        public string Type { get; set; }
        [FirestoreProperty]
        public double Commission { get; set; }
        [FirestoreProperty]
        public string Address { get; set; }
        [FirestoreProperty]
        public string City { get; set; }
        [FirestoreProperty]
        public string Contact { get; set; }
        [FirestoreProperty]
        public InventoryModel Inventory { get; set; }
        [FirestoreProperty]
        public List<ShedModel> Sheds { get; set; }
        public List<Order> Orders { get; set; }
    }
}
