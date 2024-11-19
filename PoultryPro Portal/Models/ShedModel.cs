using Google.Cloud.Firestore;

namespace PoultryPro_Portal.Models
{
    [FirestoreData]
    public class ShedModel
    {
        [FirestoreProperty]
        public int Id { get; set; }
        [FirestoreProperty]
        public string Type { get; set; }
        [FirestoreProperty]
        public int Capacity { get; set; }
        [FirestoreProperty]
        public int Current { get; set; }
        [FirestoreProperty]
        public string Address { get; set; }
    }
}
