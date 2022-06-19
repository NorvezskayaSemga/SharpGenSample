using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGenSample.MapObject
{
    public class MerchantObject : MapObject
    {
        public enum MerchantType
        {
            Items,
            Spells,
            Units,
            Trainer
        };

        public class UnitHireList
        {
            public List<Pair<Unit, int>> items = new List<Pair<Unit, int>>();//unit - count
            public void AddItem(Unit unit, int count = 0)
            {
                for(int i = 0; i<items.Count(); ++i)
                {
                    if (items[i].key == unit)
                    {
                        items[i].value += count;
                        return;
                    }
                }
                items.Add(new Pair<Unit, int>(unit, count));
            }
        };

        public MerchantObject()
        {
            uid.key = (int)MapObject.Type.Merchant;
        }

        public MerchantType type = MerchantType.Items;
        public Inventory inventory = new Inventory();
        public List<string> spells = new List<string>();
        public UnitHireList units = new UnitHireList();
        public string name;
        public string desc;
        public int image = 0;
        public int priority = 3;

    };
}
