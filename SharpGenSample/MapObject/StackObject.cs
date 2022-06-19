using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpGenSample.MapObject
{
    public class StackObject : MapObject
    {
        public StackObject(int x = 0, int y = 0)
        {
            uid.key = (int)MapObject.Type.Stack;
            this.x = x;
            this.y = y;
        }
        public GroupData stack = new GroupData();
        public int rotation = 0;
        public int order = 0;
        public int priority = 0;
        public Pair<int, int> orderTarget = new Pair<int, int>(0,0);
        public bool ignoreAI;

        enum Order
        {
            Normal = 1,
            Stand = 2,
            Guard = 3,
            AttackStack = 4,
            DefendStack = 5,
            SecureCity = 6,
            Roam = 7,
            MoveToLocation = 8,
            DefendLocation = 9,
            Bezerk = 10,
            Assist = 11,
            ExploreUnused,
            Steal = 13,
            DefendCity = 14
        };

        static string orderName(int order)
        {
            return orderName((StackObject.Order)order);
        }
        static string orderName(StackObject.Order order)
        {
            switch (order)
            {
                case StackObject.Order.Normal:
                    return "ORDER_NORMAL";
                case StackObject.Order.Stand:
                    return "ORDER_STAND";
                case StackObject.Order.Guard:
                    return "ORDER_GUARD";
                case StackObject.Order.AttackStack:
                    return "ORDER_ATTACKSTACK";
                case StackObject.Order.DefendStack:
                    return "ORDER_DEFENDSTACK";
                case StackObject.Order.SecureCity:
                case StackObject.Order.Roam:
                    return "ORDER_ROAM";
                case StackObject.Order.MoveToLocation:
                    return "ORDER_MOVETOLOCATION";
                case StackObject.Order.DefendLocation:
                    return "ORDER_DEFENDLOCATION";
                case StackObject.Order.Bezerk:
                    return "ORDER_BEZERK";
                case StackObject.Order.Assist:
                    return "ORDER_ASSIST";
                case StackObject.Order.ExploreUnused:
                    return "ORDER_EXPLOREUNUSED";
                case StackObject.Order.Steal:
                    return "ORDER_STEAL";
                case StackObject.Order.DefendCity:
                    return "ORDER_DEFENDCITY";
                default:
                    break;
            }

            return "";
        }

    };
}
