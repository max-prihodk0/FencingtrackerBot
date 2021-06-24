using Discord.Rest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FencingtrackerBot.References
{
    public class DataStorage
    {
        private class CaptchaData
        {
            //DateTime Expiration;
            public int Tries;
            public string Key;

            public CaptchaData(int Tries, string Key)
            {
                this.Tries = Tries;
                this.Key = Key;
            }
        }

        private static IDictionary<ulong, CaptchaData> CaptchaEntries = new Dictionary<ulong, CaptchaData>();

        public static void AddEntry(ulong UserID, string Code)
        {
            CaptchaEntries[UserID] = new CaptchaData(3, Code);
        }

        public static bool ManageEntry(ulong UserID, string Code)
        {
            if (CaptchaEntries[UserID].Key == Code)
            {
                CaptchaEntries.Remove(UserID);
                return true;
            }

            CaptchaEntries[UserID].Tries--;

            if (CaptchaEntries[UserID].Tries == 0)
                CaptchaEntries.Remove(UserID);

            return false;
        }

        public static int GetTries(ulong UserID)
        {
            return CaptchaEntries[UserID].Tries;
        }

        public static string GetCode(ulong UserID)
        {
            return CaptchaEntries[UserID].Key;
        }

        public static bool ContainsEntry(ulong UserID)
        {
            return CaptchaEntries.ContainsKey(UserID);
        }
    }
}
