using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGenSample.MapObject
{
    public class TreasureObject :  MapObject
    {
        public TreasureObject()
        {
            uid.key = (int)MapObject.Type.Treasure;
        }
        public Inventory inventory = new Inventory();
        public int image = 0;
        public int AIpriority = 3;
    };
}
