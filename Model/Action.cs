using System.Text.Json.Serialization;

namespace Godwit.HandleConfirmEmail.Model {
    public class Action {
        [JsonPropertyName("name")] public string Name { get; set; }
    }
}