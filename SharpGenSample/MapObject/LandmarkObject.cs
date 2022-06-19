using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGenSample.MapObject
{
    public class LandmarkObject : MapObject
    {
        public LandmarkObject() { uid.key = (int)MapObject.Type.LandMark; }
        public LandmarkObject(string lmarkId, int x, int y) 
        {
            this.lmarkId = lmarkId;
            uid.key = (int)MapObject.Type.LandMark;
            this.x = x;
            this.y = y;
        }
        public string lmarkId;
        public string desc;
    };
}
