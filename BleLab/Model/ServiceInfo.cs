using System;
using LiteDB;

namespace BleLab.Model
{
    public class ServiceInfo : InfoObjectBase
    {
        [BsonRef]
        public DeviceInfo Device { get; set; }

        public ObjectId Id { get; set; }
        
        public Guid Uuid { get; set; }

        public string Name { get; set; }
        
        public string Description { get; set; }

        public bool IsFavourite { get; set; }

        public string WebLink { get; set; }

        protected override void DoSave()
        {
            InfoManager.SaveService(this);
        }
    }
}
