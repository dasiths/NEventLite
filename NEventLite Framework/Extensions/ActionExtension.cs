using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NEventLite.Extensions
{
    public static class ActionExtension
    {
        public static void ApplyActions<TAction>(this TAction argument, IEnumerable<Action<TAction>> actions)
        {
            if (actions != null)
            {
                foreach (var f in actions)
                {
                    f.Invoke(argument);
                }
            }
        }

        public static void ApplyActionsWithArgument<TArg1, TArg2>(this TArg1 arg1, IEnumerable<Action<TArg1, TArg2>> actions, TArg2 arg2)
        {
            if (actions != null)
            {
                foreach (var f in actions)
                {
                    f.Invoke(arg1,arg2);
                }
            }
        }
    }
}
