using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGenSample.MapObject
{
    public class MountainObject : MapObject
    {
        public MountainObject() { uid.key = (int)MapObject.Type.Mountain; }
        public int w;
        public int h;
        public int image = 0;
        public int race = 0;
    };
}
