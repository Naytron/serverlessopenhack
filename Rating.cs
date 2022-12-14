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
