using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGenSample.MapObject
{
    public class RuinObject : MapObject
    {
        public RuinObject()
        {
            uid.key = (int)MapObject.Type.Ruin;
        }
        public int image = 0;
        public string name = "";
        public string desc = "";
        public GroupData guards = new GroupData();
        public string item = "";
        public Currency reward = new Currency();
        public int priority = 3;
    };
}
