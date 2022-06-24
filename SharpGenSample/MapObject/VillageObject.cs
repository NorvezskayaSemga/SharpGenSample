using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGenSample.MapObject
{
    public class VillageObject : MapObject
    {
        public VillageObject(int level = 1, int x = 0, int y = 0)
        {
            uid.key = (int)MapObject.Type.Village;
            this.x = x;
            this.y = y;
            this.level = level;
        }
        public int level = 0;
        public string name;
        public string desc;
        public GroupData visiter = new GroupData();
        public GroupData garrison = new GroupData();
        public Pair<int, int> owner;
    };
}
