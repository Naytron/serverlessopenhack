using Newtonsoft.Json;

public class Rating {
    public string id { get; set; }
    public string userId { get; set; }
    public string productId { get; set; }
    public string locationName { get; set; }
    public int rating { get; set; }
    public string userNotes { get; set; }
    public string timestamp { get; set; }

}


/* {
  "userId": "cc20a6fb-a91f-4192-874d-132493685376",
  "productId": "4c25613a-a3c2-4ef3-8e02-9c335eb23204",
  "locationName": "Sample ice cream shop",
  "rating": 5,
  "userNotes": "I love the subtle notes of orange in this ice cream!"
} */