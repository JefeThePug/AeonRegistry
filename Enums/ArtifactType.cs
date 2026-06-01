using System.Text.Json.Serialization;

namespace AeonRegistry.Enums
{
    public enum ArtifactType
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        Weapon = 1,
        EnergySource = 2,
        CommunicationDevice = 3,
        Machine = 4,
        Tool = 5,
        Monolith = 6,
        Device = 7,
        Unknown = 99
    }
}