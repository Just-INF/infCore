using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace infCore.Server
{
    public class exportUserId : BaseScript
    {
        public exportUserId()
        {
            Exports.Add("getUserId", new Func<string, int>(getUserId));
            Exports.Add("getUserSource", new Func<int, string>(getUserSource));
        }

        public int getUserId(string source)
        {
            try
            {
                if (userCache.userIds.TryGetValue(source, out int userId))
                    return userId;

                return -1;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error in getUserId: {e.Message}");
                return -1;
            }
        }

        public string getUserSource(int userId)
        {
            try
            {
                if (userCache.usersSources.TryGetValue(userId, out string source))
                    return source;

                return "-1";
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error in getUserId: {e.Message}");
                return "-1";
            }
        }
    }
}
