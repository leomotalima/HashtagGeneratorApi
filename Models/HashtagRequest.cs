namespace HashtagGeneratorApi.Models
{
    public class HashtagRequest
    {
        public string Text { get; set; } = string.Empty;
        public int Count { get; set; } = 10;
        public string Model { get; set; } = "llama3.2:3b";
    }
}
