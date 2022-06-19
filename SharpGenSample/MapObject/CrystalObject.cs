using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGenSample.MapObject
{
    public class CrystalObject : MapObject
    {
        public enum ResourceType
        {
            GOLG = 0,
            DEMONS = 1,
            EMPIRE = 2,
            UNDEAD = 3,
            CLANS = 4,
            ELVES = 5
        };

        static string CrystalIdByResource(ResourceType resource)
        {
            switch (resource)
            {
                case ResourceType.GOLG:
                    return "G000CR0000GL";
                case ResourceType.DEMONS:
                    return "G000CR0000RD";
                case ResourceType.EMPIRE:
                    return "G000CR0000YE";
                case ResourceType.UNDEAD:
                    return "G000CR0000RG";
                case ResourceType.CLANS:
                    return "G000CR0000WH";
                case ResourceType.ELVES:
                    return "G000CR0000GR";
            }
            return "GOLD";
        }

        static string CrystalByResource(ResourceType resource)
        {
            switch (resource)
            {
                case ResourceType.GOLG:
                    return "GOLD";
                case ResourceType.DEMONS:
                    return "RED";
                case ResourceType.EMPIRE:
                    return "YELLOW";
                case ResourceType.UNDEAD:
                    return "ORANGE";
                case ResourceType.CLANS:
                    return "WHITE";
                case ResourceType.ELVES:
                    return "BLUE";
            }
            return "GOLD";
        }

        public CrystalObject()
        {
            uid.key = (int)MapObject.Type.Crystal;
        }
        public ResourceType resource = ResourceType.GOLG;
    };
}
