using System.Collections.Generic;

namespace BleLab.GattInformation
{
    public class GattInfo
    {
        public string Uuid { get; set; }
        
        public string Name { get; set; }
        
        public string OriginalName { get; set; }
        
        public string Link { get; set; }
        
        public string Description { get; set; }
        
        public string Summary { get; set; }
        
        public string Abstract { get; set; }

        public GattInfoType Type { get; set; }
        
        public IList<GattInfo> Children { get; set; } = new List<GattInfo>();
    }
}