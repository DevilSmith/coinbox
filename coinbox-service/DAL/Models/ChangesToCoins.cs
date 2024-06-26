using System.Text.Json.Serialization;

namespace coinbox_service.Web.DAL.Models;

public class ChangesToCoins
{
    [JsonIgnore]
    public uint Id { get; set; }
    public uint Count { get; set; }
    public DateTime CurrentDateTime { get; set; }
}
