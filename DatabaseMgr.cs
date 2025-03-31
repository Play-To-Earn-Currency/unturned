using System;
using MySql.Data.MySqlClient;
using Rocket.Core.Logging;
using UnityEngine.Networking;

namespace PlayToEarn
{
    public class DatabaseMgr
    {
        private readonly PlayToEarnPlugin _PlayToEarn;

        internal DatabaseMgr(PlayToEarnPlugin PlayToEarn)
        {
            _PlayToEarn = PlayToEarn;
            CheckSchema();
        }

        private void CheckSchema()
        {
            try
            {
                MySqlConnection mySqlConnection = CreateConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();
                mySqlConnection.Open();
                mySqlCommand.CommandText = string.Concat(
                "CREATE TABLE IF NOT EXISTS `",
                _PlayToEarn.Configuration.Instance.PlayToEarnTableName,
                "` (",
                "uniqueid VARCHAR(255) NOT NULL PRIMARY KEY,",
                "walletaddress VARCHAR(255) DEFAULT null,",
                "value DECIMAL(50, 0) NOT NULL DEFAULT 0",
                ");"
            );
                mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
            }
            catch (Exception exception)
            {
                Logger.LogError($"[PlayToEarn] Database Crashed by Console when trying to create or check existing table {_PlayToEarn.Configuration.Instance.PlayToEarnTableName}, reason: {exception.Message}");
            }
        }

        public MySqlConnection CreateConnection()
        {
            MySqlConnection mySqlConnection = null;
            try
            {
                mySqlConnection = new MySqlConnection(string.Format("SERVER={0};DATABASE={1};UID={2};PASSWORD={3};PORT={4};", _PlayToEarn.Configuration.Instance.DatabaseAddress, _PlayToEarn.Configuration.Instance.DatabaseName, _PlayToEarn.Configuration.Instance.DatabaseUsername, _PlayToEarn.Configuration.Instance.DatabasePassword, _PlayToEarn.Configuration.Instance.DatabasePort));
            }
            catch (Exception exception)
            {
                Logger.LogError($"[PlayToEarn] Instance Connection Database Crashed, reason: {exception.Message}");
            }
            return mySqlConnection;
        }

        /// <summary>
        /// Add a new player to the PlayToEarn table if not exist
        /// </summary>
        /// <param name="playerId"></param>
        /// <param name="balance"></param>
        public void AddNewPlayer(string playerId)
        {
            try
            {
                MySqlConnection mySqlConnection = CreateConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();

                mySqlCommand.CommandText = $"INSERT INTO `{_PlayToEarn.Configuration.Instance.PlayToEarnTableName}` (`uniqueid`) VALUES (@playerId)";

                mySqlCommand.Parameters.AddWithValue("@playerId", playerId);

                mySqlConnection.Open();
                mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
            }
            catch (Exception)
            {
                // Will always crash for already created players
                //Logger.LogError($"[PlayToEarn] Database Crashed by {playerId} from function AddNewPlayer, reason: {exception.Message}");
            }
        }


        /// <summary>
        /// Returns the player points from the table PlayToEarn
        /// </summary>
        /// <param name="playerId"></param>
        /// <returns></returns>
        public string GetBalance(string playerId)
        {
            try
            {
                MySqlConnection mySqlConnection = CreateConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();

                mySqlCommand.CommandText = $"SELECT `value` FROM `{_PlayToEarn.Configuration.Instance.PlayToEarnTableName}` WHERE `uniqueid` = @playerId";

                mySqlCommand.Parameters.AddWithValue("@playerId", playerId);

                mySqlConnection.Open();
                object obj = mySqlCommand.ExecuteScalar();
                if (obj != null)
                {
                    return obj.ToString();
                }
                mySqlConnection.Close();
            }
            catch (Exception exception)
            {
                Logger.LogError($"[PlayToEarn] Database Crashed by {playerId} from function GetBalance, reason: {exception.Message}");
            }
            return null;
        }


        /// <summary>
        /// Increment player wallet
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quantity"></param>
        public void IncrementWallet(string id, string quantity)
        {
            try
            {
                MySqlConnection mySqlConnection = CreateConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();

                mySqlCommand.CommandText = $"UPDATE `{_PlayToEarn.Configuration.Instance.PlayToEarnTableName}` SET `value` = `value` + @quantity WHERE `uniqueid` = @id";

                mySqlCommand.Parameters.AddWithValue("@quantity", quantity);
                mySqlCommand.Parameters.AddWithValue("@id", id);

                mySqlConnection.Open();
                mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
            }
            catch (Exception exception)
            {
                Logger.LogError($"[PlayToEarn] Database Crashed by {id} from function IncrementWallet, reason: {exception.Message}");
            }
        }

        /// <summary>
        /// Update player wallet
        /// </summary>
        /// <param name="id"></param>
        /// <param name="quantity"></param>
        public void UpdateWallet(string id, string newWallet)
        {
            try
            {
                MySqlConnection mySqlConnection = CreateConnection();
                MySqlCommand mySqlCommand = mySqlConnection.CreateCommand();

                mySqlCommand.CommandText = $"UPDATE `{_PlayToEarn.Configuration.Instance.PlayToEarnTableName}` SET `walletaddress` = @newWallet WHERE `uniqueid` = @id";

                mySqlCommand.Parameters.AddWithValue("@newWallet", newWallet);
                mySqlCommand.Parameters.AddWithValue("@id", id);

                mySqlConnection.Open();
                mySqlCommand.ExecuteNonQuery();
                mySqlConnection.Close();
            }
            catch (Exception exception)
            {
                Logger.LogError($"[PlayToEarn] Database Crashed by {id} from function UpdateWallet, reason: {exception.Message}");
            }
        }
    }
}