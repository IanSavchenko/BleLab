using Windows.Devices.Enumeration;
using LiteDB;

namespace BleLab.Model
{
    public class DeviceInfo : InfoObjectBase
    {
        public ObjectId Id { get; set; }

        public string Name { get; set; }

        public string DeviceId { get; set; }

        public string MacAddress { get; set; }


        // set by user
        public string GivenName { get; set; }


        public bool IsNew { get; set; }

        public bool IsFavourite { get; set; }

        [BsonIgnore]
        public DeviceThumbnail Glyph { get; set; }

        [BsonIgnore]
        public string DisplayName => GivenName ?? Name;

        protected override void DoSave()
        {
            InfoManager.SaveDevice(this);
        }
    }
}
