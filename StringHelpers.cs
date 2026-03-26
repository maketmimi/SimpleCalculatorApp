using System;

namespace SimpleCalculatorApp
{
    internal static class StringHelpers
    {
        public static bool StringEndsWithAny(string StringToCheck, string[] Values, out string EndValueFound)
        {
            foreach (var Value in Values)
            {
                if (StringToCheck.EndsWith(Value))
                {
                    EndValueFound = Value;
                    return true;
                }
            }

            EndValueFound = null;
            return false;
        }

    }
}
