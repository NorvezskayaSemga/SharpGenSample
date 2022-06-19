using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGenSample.MapObject
{
    class LocationObject :  MapObject
    {
        public LocationObject() { uid.key = (int)MapObject.Type.Location; }
        public LocationObject(string name, int x, int y, int r)
        {
            uid.key = (int)MapObject.Type.Location;
            this.x = x - (r - 1) / 2;
            this.y = y - (r - 1) / 2;
            this.r = r;
            this.name = name;
        }
        public string name;
        public int r;
    };
}
