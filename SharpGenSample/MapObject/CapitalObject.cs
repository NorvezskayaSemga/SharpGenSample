using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGenSample.MapObject
{
    public class CapitalObject : MapObject
    {
        public CapitalObject()
        {
            uid.key = (int)MapObject.Type.Capital;
        }
        public string name;
        public string desc;
        public string raceId;
        public GroupData visiter = new GroupData();
        public GroupData garrison = new GroupData();
    }
}
