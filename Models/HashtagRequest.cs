using System.Text.Json.Serialization;

namespace HashtagGeneratorApi.Models
{
    public class HashtagRequest
    {
        [JsonPropertyName("text")]
        public string Text { get; set; } = string.Empty;

        [JsonPropertyName("count")]
        public int Count { get; set; } = 10;

        [JsonPropertyName("model")]
        public string Model { get; set; } = "llama3.2:3b";
    }
}
