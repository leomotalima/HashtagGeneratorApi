namespace HashtagGeneratorApi.Models;

public class HashtagRequest
{
    public string Texto { get; set; } = string.Empty;
    public int Quantidade { get; set; } = 5;
}
