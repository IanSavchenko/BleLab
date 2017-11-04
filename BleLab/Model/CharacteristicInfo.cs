using System;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using LiteDB;

namespace BleLab.Model
{
    public class CharacteristicInfo : InfoObjectBase
    {
        [BsonRef]
        public ServiceInfo Service { get; set; }

        public ObjectId Id { get; set; }

        public Guid Uuid { get; set; }


        [BsonIgnore]
        public GattCharacteristicProperties Properties { get; set; }

        [BsonIgnore]
        public GattProtectionLevel ProtectionLevel { get; set; }

        [BsonIgnore]
        public ushort AttributeHandle { get; set; }


        public string Name { get; set; }

        public string Description { get; set; }

        public string WebLink { get; set; }
        

        public bool IsFavourite { get; set; }


        public BytesDisplayFormat ReadDisplayFormat { get; set; }

        public BytesDisplayFormat WriteDisplayFormat { get; set; }

        public BytesDisplayFormat NotificationDisplayFormat { get; set; }


        public string Notes { get; set; }

        protected override void DoSave()
        {
            InfoManager.SaveCharacteristic(this);
        }
    }
}
