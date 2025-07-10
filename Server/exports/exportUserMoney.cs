using CitizenFX.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace infCore.Server.exports
{
    public class exportUserMoney : BaseScript
    {
        public exportUserMoney()
        {
            //Get
            Exports.Add("getUserMoney", new Func<int, int>(getUserMoney));
            Exports.Add("getUserBank", new Func<int, int>(getUserBank));
            Exports.Add("getUserInfCoins", new Func<int, int>(getUserInfCoins));

            //Set
            Exports.Add("setUserMoney", new Action<int, int>(setUserMoney));
            Exports.Add("setUserBank", new Action<int, int>(setUserBank));
            Exports.Add("setUserInfCoins", new Action<int, int>(setUserInfCoins));

            //Add
            Exports.Add("addUserMoney", new Action<int, int>(addUserMoney));
            Exports.Add("addUserBank", new Action<int, int>(addUserBank));
            Exports.Add("addUserInfCoins", new Action<int, int>(addUserInfCoins));

            //Remove
            Exports.Add("removeUserMoney", new Action<int, int>(removeUserMoney));
            Exports.Add("removeUserBank", new Action<int, int>(removeUserBank));
            Exports.Add("removeUserInfCoins", new Action<int, int>(removeUserInfCoins));
        }

        //Get user money, bank and infCoins
        public int getUserMoney(int userId)
        {
            try
            {
                if (userCache.usersMoney.TryGetValue(userId, out userCache.userMoneyClass userMoney))
                    return userMoney.money;

                return -1;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error in getUserMoney: {e.Message}");
                return -1;
            }
        }

        public int getUserBank(int userId)
        {
            try
            {
                if (userCache.usersMoney.TryGetValue(userId, out userCache.userMoneyClass userMoney))
                    return userMoney.bank;

                return -1;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error in getUserBank: {e.Message}");
                return -1;
            }
        }

        public int getUserInfCoins(int userId)
        {
            try
            {
                if (userCache.usersMoney.TryGetValue(userId, out userCache.userMoneyClass userMoney))
                    return userMoney.infCoins;

                return -1;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error in getUserInfCoins: {e.Message}");
                return -1;
            }
        }

        //Set user money, bank and infCoins
        public void setUserMoney(int userId, int moneyToSet)
        {
            try
            {
                if (userCache.usersMoney.TryGetValue(userId, out userCache.userMoneyClass userMoney))
                    userCache.usersMoney[userId].money = moneyToSet;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error in getUserMoney: {e.Message}");
            }
        }

        public void setUserBank(int userId, int bankToSet)
        {
            try
            {
                if (userCache.usersMoney.TryGetValue(userId, out userCache.userMoneyClass userMoney))
                    userCache.usersMoney[userId].bank = bankToSet;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error in getUserBank: {e.Message}");
            }
        }

        public void setUserInfCoins(int userId, int coinsToSet)
        {
            try
            {
                if (userCache.usersMoney.TryGetValue(userId, out userCache.userMoneyClass userMoney))
                    userCache.usersMoney[userId].infCoins = coinsToSet;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error in getUserInfCoins: {e.Message}");
            }
        }

        //Add user money, bank and infCoins
        public void addUserMoney(int userId, int moneyToAdd)
        {
            try
            {
                if (userCache.usersMoney.TryGetValue(userId, out userCache.userMoneyClass userMoney))
                    userCache.usersMoney[userId].money += moneyToAdd;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error in addUserMoney: {e.Message}");
            }
        }
        public void addUserBank(int userId, int bankToAdd)
        {
            try
            {
                if (userCache.usersMoney.TryGetValue(userId, out userCache.userMoneyClass userMoney))
                    userCache.usersMoney[userId].bank += bankToAdd;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error in addUserBank: {e.Message}");
            }
        }

        public void addUserInfCoins(int userId, int coinsToAdd)
        {
            try
            {
                if (userCache.usersMoney.TryGetValue(userId, out userCache.userMoneyClass userMoney))
                    userCache.usersMoney[userId].infCoins += coinsToAdd;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error in addUserInfCoins: {e.Message}");
            }
        }

        //Remove user money, bank and infCoins
        public void removeUserMoney(int userId, int moneyToRemove)
        {
            try
            {
                if (userCache.usersMoney.TryGetValue(userId, out userCache.userMoneyClass userMoney))
                    userCache.usersMoney[userId].money -= moneyToRemove;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error in removeUserMoney: {e.Message}");
            }
        }

        public void removeUserBank(int userId, int bankToRemove)
        {
            try
            {
                if (userCache.usersMoney.TryGetValue(userId, out userCache.userMoneyClass userMoney))
                    userCache.usersMoney[userId].bank -= bankToRemove;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error in removeUserBank: {e.Message}");
            }
        }

        public void removeUserInfCoins(int userId, int coinsToRemove)
        {
            try
            {
                if (userCache.usersMoney.TryGetValue(userId, out userCache.userMoneyClass userMoney))
                    userCache.usersMoney[userId].infCoins -= coinsToRemove;
            }
            catch (Exception e)
            {
                Debug.WriteLine($"Error in removeUserInfCoins: {e.Message}");
            }
        }
    }
}
