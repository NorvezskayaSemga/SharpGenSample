using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGenSample.MapObject
{
    public class Point
    {
        public Point(int x = 0, int y = 0) { this.x = x; this.y = y; }
        public int x;
        public int y;
    }

    public class Pair<TKey, TValue>
    {
        public TKey key;

        public TValue value;
        public Pair(TKey key, TValue value)
        {
            this.key = key;
            this.value = value;
        }
        public override string ToString()
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append('[');
            if (key != null)
            {
                stringBuilder.Append(key.ToString());
            }

            stringBuilder.Append(", ");
            if (value != null)
            {
                stringBuilder.Append(value.ToString());
            }

            stringBuilder.Append(']');
            return stringBuilder.ToString();
        }
    }
    public class Unit
    {
        public string id = "";
        public int level = 1;
        public List<string> modifiers = new List<string>();
        public bool leader = false;
    };
    public class Inventory
    {
        public List<Pair<string, int>> items = new List<Pair<string, int>>();//id - count

        public void AddItem(string id, int count)
        {
            for(int i = 0; i < items.Count(); ++i)
            {
                if (items[i].key == id)
                {
                    items[i].value += count;
                    return;
                }
            }
            items.Add(new Pair<string, int>(id, count));
        }

        public void RemoveItem(string id, int count)
        {
            for (int i = 0; i < items.Count(); ++i)
            {
                if (items[i].key == id)
                {
                    items[i].value -= count;
                    if (items[i].value <= 0)
                        items.RemoveAt(i);
                    return;
                }
            }
        }

        public void AddItem(string id)
        {
            AddItem(id, 1);
        }
        public void RemoveItem(string id)
        {
            RemoveItem(id, 1);
        }

        public void RemoveAll(string id)
        {
            for (int i = 0; i < items.Count(); ++i)
            {
                if (items[i].key == id)
                {
                    items.RemoveAt(i);
                    return;
                }
            }
        }

        public List<string> ToList() 
        {
            List<string> result = new List<string>();
            for (int i = 0; i < items.Count(); ++i)
            {
                for (int k = 0; k < items[i].value; ++k)
                {
                    result.Add(items[i].key);
                }
            }
            return result;
        }
    };
    public class GroupData
    {
        public List<Pair<Unit, Point>> units = new List<Pair<Unit, Point>>();
        public string name = "";
        public Inventory inventory = new Inventory();
        public GroupData()
        {

        }

        public void AddUnit(Unit  unit, Point pos)
        {
            units.Add(new Pair<Unit, Point>(unit, pos));
        }

        public void Remove(int x, int y)
        {
            for (int i = 0; i < units.Count(); ++i)
            {
                Point point = units[i].value;
                if (point.x == x && point.y == y)
                {
                    units.RemoveAt(i);
                    return;
                }
            }
        }

        public string LeaderId()
        {
            for(int i = 0; i < units.Count(); ++i)
            {
                if (units[i].key.leader)
                    return units[i].key.id;
            }
            return "";
        }

        public int LeaderIndex() 
        {
            for(int i = 0; i < units.Count(); ++i)
            {
                if (units[i].key.leader)
                    return i;
            }
            return -1;
        }
    };
}
