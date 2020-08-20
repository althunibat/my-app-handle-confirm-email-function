using System.Text.Json.Serialization;

namespace Godwit.HandleConfirmEmail.Model {
    public class ConfirmEmailModel {
        [JsonPropertyName("username")] public string UserName { get; set; }

        [JsonPropertyName("token")] public string Token { get; set; }
    }
}