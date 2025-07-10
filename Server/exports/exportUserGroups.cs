using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace infCore.Server
{
    public class exportUserGroups : BaseScript
    {
        public exportUserGroups()
        {
            Exports.Add("getUserGroups", new Func<int, List<string>>(getUserGroups));
            Exports.Add("hasUserGroup", new Func<int, string, bool>(hasUserGroup));

            Exports.Add("addUserGroup", new Action<int, string>(addUserGroup));
            Exports.Add("removeUserGroup", new Action<int, string>(removeUserGroup));

            Exports.Add("getUserAdminLevel", new Func<int, int>(getUserAdminLevel));
            Exports.Add("setUserAdminLevel", new Action<int, int>(setUserAdminLevel));
        }

        public List<string> getUserGroups(int userId)
        {
            if (userCache.userGroups.TryGetValue(userId, out List<string> groups))
                return groups;

            return new List<string>();
        }

        public bool hasUserGroup(int userId, string groupName)
        {
            if (userCache.userGroups.TryGetValue(userId, out List<string> groups))
                return groups.Contains(groupName);

            return false;
        }

        public void addUserGroup(int userId, string groupName)
        {
            if (userCache.userGroups.TryGetValue(userId, out List<string> groups))
                if (!groups.Contains(groupName))
                    groups.Add(groupName);
        }
        public void removeUserGroup(int userId, string groupName)
        {
            if (userCache.userGroups.TryGetValue(userId, out List<string> groups))
                if (groups.Contains(groupName))
                    groups.Remove(groupName);
        }

        public int getUserAdminLevel(int userId)
        {
            if (userCache.userAdminlevel.TryGetValue(userId, out int adminLevel))
                return adminLevel;

            return 0; // Default admin level
        }
        public void setUserAdminLevel(int userId, int adminLevel)
        {
            if (userCache.userAdminlevel.ContainsKey(userId))
                userCache.userAdminlevel[userId] = adminLevel;
        }
    }
}
