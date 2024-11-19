using Google.Cloud.Firestore;

namespace PoultryPro_Portal.Models
{
    [FirestoreData]
    public class InventoryModel
    {
        [FirestoreProperty]
        public InventoryItem Eggs { get; set; }
        [FirestoreProperty]
        public InventoryItem Chicken { get; set; }
        [FirestoreProperty]
        public InventoryItem Feed { get; set; }
        [FirestoreProperty]
        public InventoryItem Hatchery { get; set; }
        [FirestoreProperty]
        public InventoryItem Medicine { get; set; }
    }

    [FirestoreData]
    public class InventoryItem
    {
        [FirestoreProperty]
        public int Quantity { get; set; }
        [FirestoreProperty]
        public string Type { get; set; }
    }
}
