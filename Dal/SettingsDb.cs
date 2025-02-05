using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using NorbitsChallenge.Bll;

namespace NorbitsChallenge.Dal
{
    public class SettingsDb
    {
        private readonly IConfiguration _config;

        public SettingsDb(IConfiguration config)
        {
            _config = config;
        }

        public string GetCompanyName(int companyId)
        {
            string companyName = "";

            // Hent connection string fra "DefaultConnection"
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand { Connection = connection, CommandType = CommandType.Text })
                {
                    command.CommandText = "SELECT * FROM settings WHERE setting = 'companyname'";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            if (reader["companyId"].ToString() == companyId.ToString())
                            {
                                companyName = reader["settingValue"].ToString();
                            }
                        }
                    }
                }
            }

            return companyName;
        }

        public List<Setting> GetSettings(int companyId)
        {
            var settings = new List<Setting>();

            // Hent connection string fra "DefaultConnection"
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand { Connection = connection, CommandType = CommandType.Text })
                {
                    command.CommandText = $"SELECT * FROM settings WHERE companyId = {companyId}";

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var setting = new Setting
                            {
                                Key = reader["setting"].ToString(),
                                Value = reader["settingValue"].ToString(),
                                CompanyId = companyId
                            };
                            settings.Add(setting);
                        }
                    }
                }
            }

            return settings;
        }

        public void UpdateSetting(Setting setting, int companyId)
        {
            // Hent connection string fra "DefaultConnection"
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand { Connection = connection, CommandType = CommandType.Text })
                {
                    command.CommandText = $"UPDATE settings SET settingValue = '{setting.Value}' WHERE setting = '{setting.Key}'";
                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
