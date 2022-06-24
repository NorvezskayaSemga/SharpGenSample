using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.r_eg.DllExport;
//using net.r_eg.Conari.Types;
using System.Runtime.InteropServices;
using System.IO;
using SharpGenSample.MapObject;
using NevendaarTools;
using System.Reflection;
using RandomStackGenerator;
using NevendaarTools.DataTypes;

namespace SharpGenSample
{
    public class GeneratorImpl
    {
        private static GeneratorImpl instance = new GeneratorImpl();
        List<Option> options = new List<Option>();
        GameModel gameModel = new GameModel();
        Dictionary<string, Pair<int, int>> raceMapping = new Dictionary<string, Pair<int, int>>();
        Dictionary<string, int> orderMapping = new Dictionary<string, int>();
        Assembly assembly = null;

        private GeneratorImpl()
        {
            //var dllFile = new FileInfo(@"Generators\Newtonsoft.Json.dll");
            //assembly = Assembly.LoadFile(dllFile.FullName);
            //var dllDirectory = dllFile.Directory.FullName;
            //File.WriteAllText(@"C:\NevendaarTools\D2MapEditorQt\Generators\WriteText.txt", dllDirectory);
            //Environment.SetEnvironmentVariable("PATH", Environment.GetEnvironmentVariable("PATH") + ";" + dllDirectory);
            //AppDomain.CurrentDomain.AppendPrivatePath(dllDirectory);
            //AppDomain.CurrentDomain.AppendPrivatePath("Generators");
            //AppDomain.CurrentDomain.AppendPrivatePath("../Generators");
            //AppDomainSetup.PrivateBinPath += dllDirectory;
        }


        public static GeneratorImpl GetInstance()
        {
            return instance;
        }

        public void Init(string path, string lang)
        {
            gameModel.Load(path, true);
            orderMapping.Clear();
            var orders = gameModel.GetAllT<LOrder>();
            foreach(var order in orders)
            {
                orderMapping.Add(order.text, order.id);
            }

            GenDefaultValues.TextLanguage L;
            if (lang.ToLower() == "en")
            {
                L = GenDefaultValues.TextLanguage.Eng;
            }
            else
            {
                L = GenDefaultValues.TextLanguage.Rus;
            }
            TemplateForge tforge = new TemplateForge(ref L);
            string[] opt = new string[] { "StartGold",
                                          "StartMana",
                                          "StackStrength",
                                          "Wealth",
                                          "WaterAmount",
                                          "ForestAmount",
                                          "RoadsAmount" };
            string[] val = new string[] { "300",
                                          "100",
                                          "1",
                                          "1",
                                          "0.1",
                                          "0.2",
                                          "0.1" };

            options.Clear();
            options.Add(new Option() { name = "Semga's generator DLL version: ",
                                       value = GenDefaultValues.myVersion,
                                       type = (int)Option.Type.StringLabel });

            options.Add(new Option() { name = "Template", value = "template_48x48_unsymm_simple.txt", variants = "template_48x48_unsymm_simple.txt", type = (int)Option.Type.Enum });
            options.Add(new Option() { name = "MapSize", value = "48x48", variants = "48x48;72x72;96x96;144x144", type = (int)Option.Type.Enum });
            var races = gameModel.GetAllT<Grace>();
            foreach (var race in races)
            {
                if (!race.playable)
                    continue;
                options.Add(new Option() { name = race.name_txt.value.text, value = "False", type = (int)Option.Type.Bool });
            }

            TemplateForge.Parameter p;
            Option.Type optType;
            for (int i=0; i<opt.Length;i++)
            {
                p = tforge.GetParameterInfo(opt[i]);
                if (p.type == TemplateForge.Parameter.ValueType.vBoolean)
                {
                    optType = Option.Type.Bool;
                }
                else if (p.type == TemplateForge.Parameter.ValueType.vDouble)
                {
                    optType = Option.Type.Float;
                }
                else if (p.type == TemplateForge.Parameter.ValueType.vDouble)
                {
                    optType = Option.Type.Float;
                }
                else if (p.type == TemplateForge.Parameter.ValueType.vInteger)
                {
                    optType = Option.Type.Int;
                }
                else if (p.type == TemplateForge.Parameter.ValueType.vString)
                {
                    optType = Option.Type.String;
                }
                else if (p.type == TemplateForge.Parameter.ValueType.vStringArray)
                {
                    optType = Option.Type.Enum;
                }
                else
                {
                    throw new Exception("Unexpected option type");
                }

                options.Add(new Option()
                {
                    name = p.name,
                    desc = p.description,
                    value = val[i],
                    min = p.minValue,
                    max = p.maxValue,
                    type = (int)optType
                });
            }
            options.Add(new Option() { name = "CapitalGuards", value = "False", type = (int)Option.Type.Bool });
        }

               
        public string getName()
        {
            return "SharpGenSample";
        }

        public string getDesc()
        {
            return "SharpGenSample desc";
        }

        

        public int getOptionsCount()
        {
            return options.Count;
        }

        public IntPtr getOptionAt(int num)
        {
            IntPtr ptr = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(Option)));
            Marshal.StructureToPtr(options[num], ptr, false);
            return ptr;
        }

        public int setOptionAt(int index, string value)
        {
            Option opt = options[index];
            opt.value = value;
            options[index] = opt;
            return 0;
        }
        
        public void AddCapital(ref JsonMap map, string race, string guardId, string heroId, int x, int y, string name)
        {
            CapitalObject capital = new CapitalObject();
            capital.raceId = race;
            capital.name = name;
            capital.desc = "desc";
            capital.x = x;
            capital.y = y;
            if (guardId != null)
            {
                Unit guard = new Unit();
                guard.id = guardId;
                capital.garrison.AddUnit(guard, new MapObject.Point(0, 1));
            }
            Unit hero = new Unit();
            hero.id = heroId;
            capital.visiter.AddUnit(hero, new MapObject.Point(0, 1));
            raceMapping.Add(capital.raceId.ToUpper(), map.AddObject(capital));

            return;

            

            LocationObject location =  new LocationObject();
            location.x = x + 1;
            location.y = y + 9;
            location.name = race;
            location.r = 3;
            map.AddObject(location);

            TreasureObject treasure = new TreasureObject();
            treasure.x = x + 1;
            treasure.y = y + 26;
            treasure.inventory.AddItem("g000ig0001", 2);
            map.AddObject(treasure);
        }

        public string generateMap()
        {
            JsonMap map = new JsonMap();
            var sizeOpt = Option.GetOption(ref options, "MapSize");
            string modName = "MNS";
            bool fullSymmetry = false;
            bool usePlayableRaceUnits = true;

            Common comm = new Common(modName);

            RandStack.ConstructorInput rStackData = new RandStack.ConstructorInput() { gameModel = gameModel};
            rStackData.settings.modName = modName;
            rStackData.settings.addUnitsFromBranchesToStacks = usePlayableRaceUnits;

            RandStack rStack = new RandStack(ref rStackData);
            ObjectsContentSet objContSet = new ObjectsContentSet(ref rStack);
            ImpenetrableObjects ioPlacer = new ImpenetrableObjects(ref gameModel, true, ref comm);

            MapGenWrapper mw = new MapGenWrapper(ref ioPlacer);

            string path = ".\\Resources\\" + Option.GetOption(ref options, "Template");
            ImpenetrableMeshGen.GenSettings gsettings = ImpenetrableMeshGen.GenSettings.Read(ref path);
            
            gsettings.common_settMap.StartGold = Option.GetIntOption(ref options, "StartGold");
            gsettings.common_settMap.StartMana = Option.GetIntOption(ref options, "StartMana");
            gsettings.common_settMap.StackStrength = Option.GetFloatOption(ref options, "StackStrength");
            gsettings.common_settMap.Wealth = Option.GetFloatOption(ref options, "Wealth");
            gsettings.common_settMap.WaterAmount = Option.GetFloatOption(ref options, "WaterAmount");
            gsettings.common_settMap.ForestAmount = Option.GetFloatOption(ref options, "ForestAmount");
            gsettings.common_settMap.RoadsAmount = Option.GetFloatOption(ref options, "RoadsAmount");

            Map m = mw.CommonGen(ref gsettings, 10000, modName);
            
            GenDefaultValues.TextLanguage L;
            string lang = "ru";
            if (lang.ToLower() == "en")
            {
                L = GenDefaultValues.TextLanguage.Eng;
            }
            else
            {
                L = GenDefaultValues.TextLanguage.Rus;
            }

            shortMapFormat convertedMap = shortMapFormat.MapConversion(ref m,
                                                                       ref gsettings,
                                                                       ref gameModel,
                                                                       ref objContSet,
                                                                       ref fullSymmetry,
                                                                       ref usePlayableRaceUnits,
                                                                       ref L);

            Console.WriteLine(sizeOpt);
            int size = 48;
            var sizeValues = sizeOpt.Split('x');
            if (sizeValues.Length > 0)
                size = int.Parse(sizeValues[0]);
            Generate(ref map, ref convertedMap, size);
            string result = map.ToString();
            File.WriteAllText("map.json", result);
            return result;
        }
        private void Generate(ref JsonMap result, ref shortMapFormat map, int size)
        {
            result.grid.init(size);
            for (int i = 0; i < size; i++)
            {
                for (int k = 0; k < size; k++)
                {
                    shortMapFormat.TileState tile = map.landscape[i, k];
                    if (tile.ground == shortMapFormat.TileState.GroundType.Water)
                    {
                        result.grid.cells[i][k].value = (int)D2MapBlock.TileType.Water;
                    }
                    else if (tile.ground == shortMapFormat.TileState.GroundType.Forest)
                    {
                        Int32 race = (Int32)D2MapBlock.TileType.Neutrals;
                        if (tile.owner == "U")
                            race = (Int32)D2MapBlock.TileType.Undeads;
                        else if (tile.owner == "H")
                            race = (Int32)D2MapBlock.TileType.Empire;
                        else if (tile.owner == "L")
                            race = (Int32)D2MapBlock.TileType.Demons;
                        else if (tile.owner == "C")
                            race = (Int32)D2MapBlock.TileType.Dwarfs;
                        else if (tile.owner == "E")
                            race = (Int32)D2MapBlock.TileType.Elves;

                        result.grid.cells[i][k].value = (int)D2MapBlock.TileType.Tree | race |
                            tile.treeID << 24;
                    }
                }
            }
            raceMapping.Clear();
            raceMapping.Add("G000RR0004", new Pair<int, int>(0, 0));
            bool generateGuards = Option.GetBoolOption(ref options, "CapitalGuards");
            for (int i = 0; i < map.capitals.Count(); i++)
            {
                shortMapFormat.CapitalObject capital = map.capitals[i];
                List<string> items = new List<string>();
                foreach (var item in capital.items)
                {
                    items.Add(item.itemID.ToUpper());
                }
                var race = gameModel.GetObjectT<Grace>(capital.owner.byGame);
                string guardId = null;
                if (generateGuards)
                    guardId = race.guardian.value.unit_id;
                AddCapital(ref result, race.race_id, guardId, race.leader_1.value.unit_id, capital.pos.X, capital.pos.Y, capital.objectName);//TODO: lord
            }

            if (true)
            {
                FillVillages(ref result, ref map);
                FillMountains(ref result, ref map);
                FillLandmarks(ref result, ref map);
                FillStacks(ref result, ref map);
                FillMines(ref result, ref map, ref gameModel._ResourceModel.mines);
                FillRuins(ref result, ref map);
                FillMerchants(ref result, ref map);
                FillMercs(ref result, ref map);
                FillTrainers(ref result, ref map);
                FillMages(ref result, ref map);
            }
        }

        private void FillMages(ref JsonMap result, ref shortMapFormat map)
        {
            for (int i = 0; i < map.merchantsSpells.Count(); i++)
            {
                shortMapFormat.MerchantSpellObject mage = map.merchantsSpells[i];
                MerchantObject merchant = new MerchantObject();

                for (int k = 0; k < mage.content.Length; ++k)
                {
                    merchant.spells.Add(mage.content[k].spellID);
                }
                merchant.desc = mage.objectDescription;
                merchant.name = mage.objectName;
                merchant.type = MerchantObject.MerchantType.Spells;
                merchant.x = mage.pos.X;
                merchant.y = mage.pos.Y;
                merchant.image = int.Parse(mage.id.Substring(14));
                result.AddObject(merchant);
            }
        }

        private void FillTrainers(ref JsonMap result, ref shortMapFormat map)
        {
            for (int i = 0; i < map.trainers.Count(); i++)
            {
                shortMapFormat.TrainerObject trainer = map.trainers[i];
                MerchantObject merchant = new MerchantObject();
                merchant.desc = trainer.objectDescription;
                merchant.name = trainer.objectName;
                merchant.type = MerchantObject.MerchantType.Trainer;
                merchant.x = trainer.pos.X;
                merchant.y = trainer.pos.Y;
                merchant.image = int.Parse(trainer.id.Substring(14));
                result.AddObject(merchant);
            }
        }

        private void FillMercs(ref JsonMap result, ref shortMapFormat map)
        {
            for (int i = 0; i < map.merchantsUnits.Count(); i++)
            {
                shortMapFormat.MerchantUnitObject merch = map.merchantsUnits[i];
                List<D2Mercs.MercsUnitEntry> items = new List<D2Mercs.MercsUnitEntry>();
                MerchantObject merchant = new MerchantObject();
                merchant.desc = merch.objectDescription;
                merchant.name = merch.objectName;
                merchant.type = MerchantObject.MerchantType.Units;
                merchant.x = merch.pos.X;
                merchant.y = merch.pos.Y;
                merchant.image = int.Parse(merch.id.Substring(14));
                for (int k = 0; k < merch.content.Length; ++k)
                {
                    AllDataStructues.Unit unit = merch.content[k];
                    merchant.units.AddItem(new Unit(unit.unitID, unit.level));
                }
                result.AddObject(merchant);
            }
        }

        private void FillMerchants(ref JsonMap result, ref shortMapFormat map)
        {
            for (int i = 0; i < map.merchantsItems.Count(); i++)
            {
                shortMapFormat.MerchantItemObject merch = map.merchantsItems[i];
                List<D2Merchant.MerchantItemEntry> items = new List<D2Merchant.MerchantItemEntry>();
                List<string> itemIds = new List<string>();
                MerchantObject merchant = new MerchantObject();
                merchant.desc = merch.objectDescription;
                merchant.name = merch.objectName;
                merchant.type = MerchantObject.MerchantType.Items;
                merchant.x = merch.pos.X;
                merchant.y = merch.pos.Y;
                merchant.image = int.Parse(merch.id.Substring(14));
                for (int k = 0; k < merch.content.Length; ++k)
                {
                    merchant.inventory.AddItem(merch.content[k].itemID);
                }
                result.AddObject(merchant);
            }
        }

        private void FillStack(ref GroupData result, AllDataStructues.Stack stack, bool withLeader = false)
        {
            for (int k = 0; k < stack.units.Count(); ++k)
            {
                AllDataStructues.Stack.UnitInfo unit = stack.units[k];
                if (unit != null && unit.unit.unitID != "G000000000")
                {
                    Unit resUnit = new Unit();
                    resUnit.id = unit.unit.unitID;
                    resUnit.level = unit.level;
                    resUnit.modifiers = unit.modificators;
                    if (withLeader && k == stack.leaderPos)
                        resUnit.leader = true;
                    result.name = stack.name;
                    int x = (k + 1) % 2;
                    Gunit unitBase = gameModel.GetObjectT<Gunit>(resUnit.id);
                    if (!unitBase.size_small)
                        x = 0;
                    result.AddUnit(resUnit, new MapObject.Point(x, k / 2));
                }
            }
        }

        private void FillInventory(ref Inventory result, AllDataStructues.Stack stack)
        {
            for (int k = 0; k < stack.items.Count; ++k)
            {
                var item = stack.items[k];
                result.AddItem(item);
            }
        }

        private void FillRuins(ref JsonMap result, ref shortMapFormat map)
        {
            for (int i = 0; i < map.ruins.Count(); i++)
            {
                shortMapFormat.RuinObject ruin = map.ruins[i];
                RuinObject ruinObj = new RuinObject();
                ruinObj.x = ruin.pos.X;
                ruinObj.y = ruin.pos.Y;
                ruinObj.name = ruin.objectName;
                ruinObj.desc = ruin.objectName;
                ruinObj.reward.Read(AllDataStructues.Cost.Print(ruin.resourcesReward).ToUpper());
                ruinObj.item = ruin.ItemReward();
                FillStack(ref ruinObj.guards, ruin.internalStack);
                result.AddObject(ruinObj);
            }
        }

        private void FillMines(ref JsonMap result, ref shortMapFormat map, ref List<string> mines)
        {
            for (int i = 0; i < map.mines.Count(); i++)
            {
                shortMapFormat.simpleObject mine = map.mines[i];
                CrystalObject crystal = new CrystalObject();
                crystal.x = mine.pos.X;
                crystal.y = mine.pos.Y;
                crystal.resource = (CrystalObject.ResourceType)mines.IndexOf(mine.id);
                result.AddObject(crystal);
            }
        }

        private void FillVillages(ref JsonMap result, ref shortMapFormat map)
        {
            for (int i = 0; i < map.cities.Count(); i++)
            {
                shortMapFormat.CityObject village = map.cities[i];
                VillageObject villageObject = new VillageObject();
                villageObject.x = village.pos.X;
                villageObject.y = village.pos.Y;
                villageObject.level = village.level;
                villageObject.name = village.objectName;

                FillStack(ref villageObject.garrison, village.internalStack);
                FillInventory(ref villageObject.garrison.inventory, village.internalStack);

                FillStack(ref villageObject.visiter, village.exteternalStack, true);
                FillInventory(ref villageObject.visiter.inventory, village.exteternalStack);
                if (raceMapping.ContainsKey(village.owner.byGame))
                    villageObject.owner = raceMapping[village.owner.byGame];
                else
                    villageObject.owner = new Pair<int, int>(666, 666);
                result.AddObject(villageObject);
            }
        }
        
        private int OrderInt(string name)
        {
            if (orderMapping.ContainsKey(name))
                return orderMapping[name];
            return 0;
        }

        private void FillStacks(ref JsonMap result, ref shortMapFormat map)
        {
            for (int i = 0; i < map.stacks.Count(); i++)
            {
                shortMapFormat.StackObject genStack = map.stacks[i];
                StackObject stack = new StackObject(genStack.pos.X, genStack.pos.Y);
                FillStack(ref stack.stack, genStack.stack, true);
                FillInventory(ref stack.stack.inventory, genStack.stack);
                if (raceMapping.ContainsKey(genStack.owner.byGame))
                    stack.owner = raceMapping[genStack.owner.byGame];
                else
                    stack.owner = new Pair<int, int>(666, 666);
                stack.order = OrderInt(genStack.stack.order.order.name);

                result.AddObject(stack);
            }
        }

        private void FillMountains(ref JsonMap result, ref shortMapFormat map)
        {
            for (int i = 0; i < map.mountains.Count(); i++)
            {
                shortMapFormat.simpleObject mount = map.mountains[i];
                int index = int.Parse(mount.id.Substring(mount.id.Length - 2));
                MountainObject mountain = new MountainObject();
                mountain.x = mount.pos.X;
                mountain.y = mount.pos.Y;
                mountain.w = mount.size.Width;
                mountain.h = mount.size.Height;
                mountain.image = index;
                result.AddObject(mountain);
            }
        }

        private void FillLandmarks(ref JsonMap result, ref shortMapFormat map)
        {
            for (int i = 0; i < map.landmarks.Count(); i++)
            {
                shortMapFormat.simpleObject lmark = map.landmarks[i];
                LandmarkObject landmark = new LandmarkObject();
                landmark.x = lmark.pos.X;
                landmark.y = lmark.pos.Y;
                landmark.desc = "";
                landmark.lmarkId = lmark.id;
                result.AddObject(landmark);
            }
        }


        public void cleanup()
        {

        }
    }
}

