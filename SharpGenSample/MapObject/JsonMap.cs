using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace SharpGenSample.MapObject
{
    public class Currency
    {
        public Currency() { }
        //G0100:R0000:Y0000:E0000:W0000:B0000
        //    G - золото
        //R - мана преисподней
        //Y - мана жизни
        //E - мана смерти
        //W - мана рун
        //B - мана рощи
        public int gold = 0;
        public int demons = 0;
        public int empire = 0;
        public int undead = 0;
        public int clans = 0;
        public int elves = 0;

        public int TotalCount()
        {
            return gold + demons + empire + undead + clans + elves;
        }

        static int ValueByLetter(string curString, Char letter)
        {
            int index = curString.IndexOf(letter, 0);
            if (index == -1)
                return 0;
            return Int32.Parse(curString.Substring(index + 1, 4));
        }

        static Currency FromString(string curString)
        {
            Currency result = new Currency();
            result.Read(curString.ToUpper());
            return result;
        }
        public  void Read(string curString)
        {
            gold = ValueByLetter(curString, 'G');
            demons = ValueByLetter(curString, 'R');
            empire = ValueByLetter(curString, 'Y');
            undead = ValueByLetter(curString, 'E');
            clans = ValueByLetter(curString, 'W');
            elves = ValueByLetter(curString, 'B');
        }

        public string ToString()
        {
            return  "G" + gold.ToString().PadLeft(4, '0') +
                    "R" + demons.ToString().PadLeft(4, '0') +
                    "Y" + empire.ToString().PadLeft(4, '0') +
                    "E" + undead.ToString().PadLeft(4, '0') +
                    "W" + clans.ToString().PadLeft(4, '0') +
                    "B" + elves.ToString().PadLeft(4, '0');
        }
    };

    public class MapCell
    {
        static int tileTerrain(int tile)
        {
            return (tile & 7);
        }

        static int tileGround(int tile)
        {
            return ((tile >> 3) & 7);
        }

        static int tileForestImage(int tile)
        {
            return tile >> 26;
        }

        public MapCell() { }
        public MapCell(int value_) { this.value = value_; }
        public int value = 1;
        public int roadType = -1;

        bool IsWater()
        {
            return tileGround(value)  == 3;
        }
    };

    public class MapObjBinding
    {
        public class UIDList
        {
            public List<KeyValuePair<int, int>> items = new List<KeyValuePair<int, int>>();
        };

        public List<List<UIDList>> binding = new List<List<UIDList>>();
        public void Clear() { binding.Clear(); }
        public void Init(int size)
        {
            for (int i = 0; i < size; ++i)
            {
                binding.Add(new List<UIDList>());
                for (int k = 0; k < size; ++k)
                {
                    binding[i].Add(new UIDList());
                }
            }
        }
    };

    public class MapGrid
    {
        public List<List<MapCell>> cells = new List<List<MapCell>>();
        public void init(int size, int val = 1)
        {
            clear();
            objBinging.Init(size);
            locationsBinging.Init(size);
            for (int i = 0; i < size; ++i)
            {
                cells.Add(new List<MapCell>());
                for (int k = 0; k < size; ++k)
                    cells[i].Add(new MapCell(val));
            }
        }
        public void clear() { cells.Clear(); objBinging.Clear(); }
        public MapObjBinding objBinging = new MapObjBinding();
        public MapObjBinding locationsBinging = new MapObjBinding();
    };

    public class MapObject
    {
        public enum Type
        {
            Stack = 1,
            Village,
            LandMark,
            Capital,
            Mountain,
            Crystal,
            Ruin,
            Treasure,
            Merchant,
            Location,

            COUNT
        };
        public static string typeName(MapObject.Type type)
        {
            switch (type)
            {
                case MapObject.Type.Stack:
                    return "Stack";
                case MapObject.Type.Village:
                    return "Village";
                case MapObject.Type.LandMark:
                    return "LandMark";
                case MapObject.Type.Capital:
                    return "Capital";
                case MapObject.Type.Mountain:
                    return "Mountain";
                case MapObject.Type.Crystal:
                    return "Crystal";
                case MapObject.Type.Ruin:
                    return "Ruin";
                case MapObject.Type.Treasure:
                    return "Treasure";
                case MapObject.Type.Merchant:
                    return "Merchant";
                case MapObject.Type.Location:
                    return "Location";
                case MapObject.Type.COUNT:
                    break;
            }
            return "Unknown type";
        }

        public Pair<int, int> uid = new Pair<int, int>(0,0);//type - object number
        public int x;
        public int y;
    };

    public class ObjectsCollection
    {
        public Dictionary<int, MapObject> data = new Dictionary<int, MapObject>();
        public List<int> availableIds = new List<int>();
        public int lastId = 0;

        public int nextUID()
        {
            if (availableIds.Count() < 1)
                return ++lastId;
            int res = availableIds.First();
            availableIds.RemoveAt(0);
            return res;
        }

        public void addObject(MapObject obj)
        {
            int key = nextUID();
            obj.uid = new Pair<int, int>(obj.uid.key, key);
            data.Add(key, obj);
        }

        public void replaceObject(MapObject obj)
        {
            data[obj.uid.value] = obj;
        }

        public void removeObject(int uid)
        {
            data.Remove(uid);
            availableIds.Insert(0, uid);
        }
    };
    public class JsonMap
    {
        public Dictionary<int, ObjectsCollection> m_mapObjects = new Dictionary<int, ObjectsCollection>();
        public MapGrid grid = new MapGrid();
        public string mapName = "";
        public string mapDesc = "";

        public JsonMap()
        {
            for (int i = 1; i < (int)MapObject.Type.COUNT; ++i)
            {
                m_mapObjects[i] = new ObjectsCollection();
            }
        }

        public ObjectsCollection CollectionByType(int type)
        {
            return m_mapObjects[type];
        }

        public Pair<int, int> AddObject(MapObject obj)
        {
            m_mapObjects[obj.uid.key].addObject(obj);
            return obj.uid;
        }

        public override string ToString() 
        {
            JObject root = new JObject();
            JObject mapInfo = new JObject();
            mapInfo["name"] = mapName;
            mapInfo["desc"] = mapDesc;
            mapInfo["size"] = grid.cells.Count;
            mapInfo["version"] = "0.25";

            root["mapInfo"] = mapInfo;

            JArray mapObjectsArray = new JArray();
            for (int i = 1; i < (int)MapObject.Type.COUNT; ++i)
            {
                JObject objectCollection = new JObject();
                objectCollection["_type"] = MapObject.typeName((MapObject.Type)i);
                objectCollection["typeId"] = i;
                JArray array = new JArray();
                ObjectsCollection collection = CollectionByType(i);
                foreach (MapObject obj in collection.data.Values)
                {
                    JObject jsonObj = new JObject();
                    jsonObj["id"] = obj.uid.key.ToString() + ":" + obj.uid.value.ToString();
                    jsonObj["x"] = obj.x;
                    jsonObj["y"] = obj.y;
                    SaveObject(ref jsonObj, obj);
                    array.Add(jsonObj);
                }
                objectCollection["objects"] = array;
                mapObjectsArray.Add(objectCollection);
            }
            root["objects"] = mapObjectsArray;

            JArray mapTilesArray = new JArray();
            for (int i = 0; i < grid.cells.Count(); ++i)
            {
                for (int k = 0; k < grid.cells.Count(); ++k)
                {
                    mapTilesArray.Add(grid.cells[i][k].value);
                    mapTilesArray.Add(grid.cells[i][k].roadType);
                }
            }
            root["tiles"] = mapTilesArray;


            return JsonConvert.SerializeObject(root, Formatting.Indented);
        }

        void SaveObject(ref JObject target, MapObject obj)
        {
            if (obj.uid.key ==  (int)MapObject.Type.Stack)
                SaveStack(ref target, obj);
            if (obj.uid.key == (int)MapObject.Type.Village)
                SaveVillage(ref target, obj);
            if (obj.uid.key == (int)MapObject.Type.LandMark)
                SaveLandMark(ref target, obj);
            if (obj.uid.key == (int)MapObject.Type.Capital)
                SaveCapital(ref target, obj);
            if (obj.uid.key == (int)MapObject.Type.Mountain)
                SaveMountain(ref target, obj);
            if (obj.uid.key == (int)MapObject.Type.Crystal)
                SaveCrystal(ref target, obj);
            if (obj.uid.key == (int)MapObject.Type.Ruin)
                SaveRuin(ref target, obj);
            if (obj.uid.key == (int)MapObject.Type.Treasure)
                SaveTreasure(ref target, obj);
            if (obj.uid.key == (int)MapObject.Type.Merchant)
                SaveMerchant(ref target, obj);
            if (obj.uid.key == (int)MapObject.Type.Location)
                SaveLocation(ref target, obj);

        }

        static void SaveInventory(JArray target, Inventory  data)
        {
            for (int i = 0; i<data.items.Count(); ++i)
            {
                JObject item = new JObject();
                item["id"] = data.items[i].key;
                item["count"] = data.items[i].value;
                target.Add(item);
            }
        }

        static void SaveGroup(ref JObject target, GroupData data)
        {
            target["name"] = data.name;
            JArray units = new JArray();
            for (int i = 0; i<data.units.Count(); ++i)
            {
                JObject unit = new JObject();
                unit["id"] = data.units[i].key.id;
                unit["level"] = data.units[i].key.level;
                if (data.units[i].key.leader)
                    unit["leader"] = true;
                JArray mods = new JArray();
                for (int k = 0; k<data.units[i].key.modifiers.Count(); ++k)
                {
                    mods.Add(data.units[i].key.modifiers[k]);
                }
                unit["mods"] = mods;
                unit["x"] = data.units[i].value.x;
                unit["y"] = data.units[i].value.y;
                units.Add(unit);
            }
            target["units"] = units;
            JArray inventoryObject = new JArray();
            SaveInventory(inventoryObject, data.inventory);
            target["inventory"] = inventoryObject;
        }

        static void SaveCurrency(ref JObject target, Currency data)
        {
            target["gold"]   = (int) data.gold;
            target["demons"] = (int) data.demons;
            target["empire"] = (int) data.empire;
            target["undead"] = (int) data.undead;
            target["clans"]  = (int) data.clans;
            target["elves"]  = (int) data.elves;
        }

        static void SaveStack(ref JObject target, MapObject obj)
        {
            StackObject stack = obj as StackObject;

            target["rotation"] = stack.rotation;
            target["order"] = stack.order;
            target["priority"] = stack.priority;
            target["orderTarget"] = stack.orderTarget.key.ToString() + ":" +  stack.orderTarget.value.ToString();
            target["owner"] = stack.owner.key.ToString() + ":" + stack.owner.value.ToString();
            target["ignoreAI"] = stack.ignoreAI;
            JObject groupObject = new JObject();
            SaveGroup(ref groupObject, stack.stack);
            target["stack"] = groupObject;
        }

        static void SaveVillage(ref JObject target, MapObject obj)
        {
            VillageObject village = obj as VillageObject;

            target["level"] = village.level;
            target["name"] = village.name;
            target["desc"] = village.desc;
            target["owner"] = village.owner.key.ToString() + ":" + village.owner.value.ToString();

            JObject visiter = new JObject();
            SaveGroup(ref visiter, village.visiter);
            target["visiter"] = visiter;

            JObject garrison = new JObject();
            SaveGroup(ref garrison, village.garrison);
            target["garrison"] = garrison;
        }
        static void SaveLandMark(ref JObject target, MapObject obj)
        {
            LandmarkObject lmark = obj as LandmarkObject;
            target["lmarkId"] = lmark.lmarkId;
            target["desc"] = lmark.desc;
        }
        static void SaveCapital(ref JObject target, MapObject obj)
        {
            CapitalObject capital = obj as CapitalObject;
            target["name"] = capital.name;
            target["desc"] = capital.desc;
            target["raceId"] = capital.raceId;

            if (capital.visiter != null)
            {
                JObject visiter = new JObject();
                SaveGroup(ref visiter, capital.visiter);
                target["visiter"] = visiter;
            }

            JObject garrison = new JObject();
            SaveGroup(ref garrison, capital.garrison);
            target["garrison"] = garrison;
        }
        static void SaveMountain(ref JObject target, MapObject obj)
        {
            MountainObject mountain = obj as MountainObject;
            target["w"] = mountain.w;
            target["h"] = mountain.h;
            target["image"] = mountain.image;
            target["race"] = mountain.race;
        }
        static void SaveCrystal(ref JObject target, MapObject obj)
        {
            CrystalObject crystal = obj as CrystalObject;
            target["resource"] = (int)crystal.resource;
        }

        static void SaveRuin(ref JObject target, MapObject obj)
        {
            RuinObject ruin = obj as RuinObject;
            target["name"] = ruin.name;
            target["desc"] = ruin.desc;
            target["image"] = ruin.image;
            target["item"] = ruin.item;
            target["priority"] = ruin.priority;
            JObject reward =  new JObject();
            SaveCurrency(ref reward, ruin.reward);
            target["reward"] = reward;

            JObject guards = new JObject();
            SaveGroup(ref guards, ruin.guards);
            target["guards"] = guards;
        }
        static void SaveTreasure(ref JObject target, MapObject obj)
        {
            TreasureObject treasure = obj as TreasureObject;
            target["image"] = treasure.image;
            target["AIpriority"] = treasure.AIpriority;
            JArray items = new JArray();
            SaveInventory(items, treasure.inventory);
            target["items"] = items;
        }
        static void SaveMerchant(ref JObject target, MapObject obj)
        {
            MerchantObject merchant = obj as MerchantObject;
            target["type"] = (int)merchant.type;
            target["name"] = merchant.name;
            target["desc"] = merchant.desc;
            target["image"] = merchant.image;
            target["priority"] = merchant.priority;

            JArray spells = new JArray();
            for (int i = 0; i < merchant.spells.Count(); ++i)
            {
                spells.Add(merchant.spells[i]);
            }
            target["spells"] = spells;

            JArray items = new JArray();
            SaveInventory(items, merchant.inventory);
            target["items"] = items;

            JArray units = new JArray();
            for (int i = 0; i < merchant.units.items.Count(); ++i)
            {
                JObject unit = new JObject();
                unit["id"] = merchant.units.items[i].key.id;
                unit["level"] = merchant.units.items[i].key.level;
                JArray mods = new JArray();
                for (int k = 0; k < merchant.units.items[i].key.modifiers.Count(); ++k)
                {
                    mods.Add(merchant.units.items[i].key.modifiers[k]);
                }
                unit["mods"] = mods;
                unit["count"] = merchant.units.items[i].value;
                units.Add(unit);
            }
            target["units"] = units;
        }
        static void SaveLocation(ref JObject target, MapObject obj)
        {
            LocationObject location = obj as LocationObject;
            target["name"] = location.name;
            target["radius"] = location.r;
        }
    };
}
