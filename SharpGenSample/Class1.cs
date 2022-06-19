using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using net.r_eg.DllExport;
//using net.r_eg.Conari.Types;
using System.Runtime.InteropServices;
using System.Globalization;

namespace SharpGenSample
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public class Option
    {
        public enum Type
        {
            Bool = 0,
            Float,
            Int,
            String,
            Enum,
            StringLabel = 32,
            Spacer
        };

        public string name = null;
        public string desc = null;
        public string value = null;
        public string min = null;
        public string max = null;
        public string variants = null;
        public int type = 0;
        public int status = 0;


        public static string GetOption(ref List<Option> options, string name)
        {
            foreach (Option option in options)
            {
                if (option.name == name)
                    return option.value;
            }
            return null;
        }

        public static bool GetBoolOption(ref List<Option> options, string name)
        {
            return GetOption(ref options, name).Trim() == "True";
        }

        public static float GetFloatOption(ref List<Option> options, string name)
        {
            string value = GetOption(ref options, name);
            value = value.Replace(",", ".");
            value = value.Replace(".", CultureInfo.CurrentCulture.NumberFormat.NumberDecimalSeparator);
            return float.Parse(value);
        }

        public static int GetIntOption(ref List<Option> options, string name)
        {
            string value = GetOption(ref options, name);
            return int.Parse(value);
        }
    };

    public class MapGenerator
    {
        [DllExport]
        public static void cleanup()
        {
            GeneratorImpl.GetInstance().cleanup();
        }

        [DllExport]
        public static void init(string path, string lang)
        {
            GeneratorImpl.GetInstance().Init(path, lang);
        }

        [DllExport]
        public static string getName()
        {
            return GeneratorImpl.GetInstance().getName();
        }

        [DllExport]
        public static string getDescription()
        {
            return GeneratorImpl.GetInstance().getDesc();
        }

        [DllExport]
        public static string generateMap()
        {
            return GeneratorImpl.GetInstance().generateMap();
        }

        [DllExport]
        public static int getOptionsCount()
        {
            return GeneratorImpl.GetInstance().getOptionsCount();
        }

        [DllExport]
        public static IntPtr getOptionAt(int num)
        {
            return GeneratorImpl.GetInstance().getOptionAt(num);
        }

        [DllExport]
        //[DllExport("setOptionAt", CallingConvention = CallingConvention.Cdecl)]
        public static Int32 setOptionAt(int num, string value)
        {
            return GeneratorImpl.GetInstance().setOptionAt(num, value);
        }

        
    }
}

