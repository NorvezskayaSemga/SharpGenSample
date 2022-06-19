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

namespace SharpGenSample
{
    public class GeneratorImpl
    {
        private static GeneratorImpl instance = new GeneratorImpl();
        List<Option> options = new List<Option>();
        GameModel gameModel = new GameModel();
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
        
        public void AddCapital(ref JsonMap map, string race, string guardId, string heroId, int x, int y)
        {
            CapitalObject capital = new CapitalObject();
            capital.raceId = race;
            capital.name = race + "_capital";
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
            map.AddObject(capital);

            CrystalObject crystal = new CrystalObject();
            crystal.x = x + 2;
            crystal.y = y + 7;
            map.AddObject(crystal);

            LandmarkObject landmark = new LandmarkObject();
            landmark.x = x + 2;
            landmark.y = y + 10;
            landmark.desc = "desc";
            landmark.lmarkId = "G000MG0004";
            map.AddObject(landmark);

            LocationObject location =  new LocationObject();
            location.x = x + 1;
            location.y = y + 9;
            location.name = race;
            location.r = 3;
            map.AddObject(location);

            MerchantObject merchant = new MerchantObject();
            merchant.desc = "desc";
            merchant.name = race;
            merchant.type = MerchantObject.MerchantType.Items;
            merchant.inventory.AddItem("g000ig0001", 5);
            merchant.x = x + 1;
            merchant.y = y + 13;
            map.AddObject(merchant);

            MountainObject mountain = new MountainObject();
            mountain.x = x + 1;
            mountain.y = y + 17;
            mountain.w = 2;
            mountain.h = 2;
            map.AddObject(mountain);

            RuinObject ruin = new RuinObject();
            ruin.x = x + 1;
            ruin.y = y + 20;
            ruin.reward.gold = 100;
            map.AddObject(ruin);

            StackObject stack = new StackObject(x + 2, y + 25);
            stack.stack.AddUnit(hero, new MapObject.Point(1, 1));
            stack.stack.units[0].key.leader = true;
            stack.stack.name = "stack_" + race;
            map.AddObject(stack);

            TreasureObject treasure = new TreasureObject();
            treasure.x = x + 1;
            treasure.y = y + 26;
            treasure.inventory.AddItem("g000ig0001", 2);
            map.AddObject(treasure);

            VillageObject villageObject = new VillageObject();
            villageObject.x = x + 1;
            villageObject.y = y + 28;
            villageObject.level = 1;
            villageObject.name = "village name";
            map.AddObject(villageObject);
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
            
            gsettings.common_settMap.StartGold = Convert.ToInt32(Option.GetOption(ref options, "StartGold"));
            gsettings.common_settMap.StartMana = Convert.ToInt32(Option.GetOption(ref options, "StartMana"));
            gsettings.common_settMap.StackStrength = Convert.ToDouble(Option.GetOption(ref options, "StackStrength"));
            gsettings.common_settMap.Wealth = Convert.ToDouble(Option.GetOption(ref options, "Wealth"));
            gsettings.common_settMap.WaterAmount = Convert.ToDouble(Option.GetOption(ref options, "WaterAmount"));
            gsettings.common_settMap.ForestAmount = Convert.ToDouble(Option.GetOption(ref options, "ForestAmount"));
            gsettings.common_settMap.RoadsAmount = Convert.ToDouble(Option.GetOption(ref options, "RoadsAmount"));

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
            map.grid.init(size);
            int posX = 1;
            int posY = 1;
            int capCount = 0;
            var races = gameModel.GetAllT<Grace>();
            bool enabledGuard = Option.GetBoolOption(ref options, "CapitalGuards"); ;
            foreach (var race in races)
            {
                if (!race.playable)
                    continue;
                bool enabled = Option.GetBoolOption(ref options, race.name_txt.value.text);
                if (enabled)
                {
                    string guardId = null;
                    if (enabledGuard)
                        guardId = race.guardian.value.unit_id;
                    AddCapital(ref map, race.race_id, guardId, race.leader_1.value.unit_id, posX, posY);
                    posX += 6;
                    capCount++;
                }
            }
            string result= map.ToString();
            File.WriteAllText(@"C:\NevendaarTools\D2MapEditorQt\Generators\WriteText.json", result);
            return result;
        }

        public void cleanup()
        {

        }
    }
}

