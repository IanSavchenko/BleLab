using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BleLab.GattInformation
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GattInfoType
    {
        Service,
        Characteristic,
        Descriptor
    }
}