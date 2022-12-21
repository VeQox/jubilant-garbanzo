namespace Client
{
  using System.Globalization;
  using Newtonsoft.Json;
  using Newtonsoft.Json.Converters;

  public partial class Player
  {
    [JsonProperty("Coords")]
    public Coords Coords { get; set; }

    [JsonProperty("Char")]
    public string Char { get; set; }

    [JsonProperty("id")]
    public int Id { get; set; }

    [JsonProperty("score")]
    public int Score { get; set; }
  }

  public partial class Coords
  {
    [JsonProperty("Coords")]
    public Coord[] CoordsCoords { get; set; }
  }

  public partial class Coord
  {
    [JsonProperty("X")]
    public int X { get; set; }

    [JsonProperty("Y")]
    public int Y { get; set; }
  }

  public partial class Player
  {
    public static Player[] FromJson(string json) => JsonConvert.DeserializeObject<Player[]>(json, Converter.Settings);
  }

  public static class Serialize
  {
    public static string ToJson(this Player[] self) => JsonConvert.SerializeObject(self, Converter.Settings);
  }

  internal static class Converter
  {
    public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
    {
      MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
      DateParseHandling = DateParseHandling.None,
      Converters =
            {
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
    };
  }
}
