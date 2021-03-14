using System.Collections.Generic;

namespace OLEDShow
{
    public class Shared
    {
        public static MainActivity MainActivity;
        public static InfoText InfoText = new InfoText();
        public static Dictionary<string, VThread> VThreadsCollection = new Dictionary<string, VThread>();
    }
}