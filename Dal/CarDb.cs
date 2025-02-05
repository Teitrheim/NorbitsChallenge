using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using NorbitsChallenge.Models;

namespace NorbitsChallenge.Dal
{
    public class CarDb
    {
        private readonly IConfiguration _config;

        public CarDb(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Henter alle biler for en gitt companyId.
        /// </summary>
        public List<Car> GetCars(int companyId)
        {
            var cars = new List<Car>();
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand
                {
                    Connection = connection,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT * FROM Car WHERE CompanyId = @companyId"
                })
                {
                    command.Parameters.AddWithValue("@companyId", companyId);

                    using (var reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var car = new Car
                            {
                                LicensePlate = reader["LicensePlate"].ToString(),
                                Description  = reader["Description"].ToString(),
                                Model        = reader["Model"].ToString(),
                                Brand        = reader["Brand"].ToString(),
                                TireCount    = (int)reader["TireCount"],
                                CompanyId    = (int)reader["CompanyId"]
                            };
                            cars.Add(car);
                        }
                    }
                }
            }

            return cars;
        }

        /// <summary>
        /// Returns a single Car for (companyId, licensePlate). Null if not found.
        /// </summary>
        public Car GetCarByPlate(int companyId, string licensePlate)
        {
            Car foundCar = null;
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand { Connection = connection, CommandType = CommandType.Text })
                {
                    command.CommandText = @"
                        SELECT *
                        FROM Car
                        WHERE CompanyId = @companyId
                          AND LicensePlate = @licensePlate";

                    command.Parameters.AddWithValue("@companyId", companyId);
                    command.Parameters.AddWithValue("@licensePlate", licensePlate);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            foundCar = new Car
                            {
                                LicensePlate = reader["LicensePlate"].ToString(),
                                Description  = reader["Description"].ToString(),
                                Model        = reader["Model"].ToString(),
                                Brand        = reader["Brand"].ToString(),
                                TireCount    = (int)reader["TireCount"],
                                CompanyId    = (int)reader["CompanyId"]
                            };
                        }
                    }
                }
            }

            return foundCar;
        }

        /// <summary>
        /// Oppgave 1 (delvis): Returnerer antall dekk for en gitt bil.
        /// </summary>
        public int GetTireCount(int companyId, string licensePlate)
        {
            int result = 0;
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand { Connection = connection, CommandType = CommandType.Text })
                {
                    command.CommandText = "SELECT tireCount FROM Car WHERE CompanyId = @companyId AND LicensePlate = @licensePlate";
                    command.Parameters.AddWithValue("@companyId", companyId);
                    command.Parameters.AddWithValue("@licensePlate", licensePlate);

                    using (var reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            result = (int)reader["tireCount"];
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Oppgave 3 (del 1): Legg til ny bil i Car-tabellen.
        /// </summary>
        public void CreateCar(Car car)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand { Connection = connection, CommandType = CommandType.Text })
                {
                    command.CommandText = @"
                        INSERT INTO Car
                            (LicensePlate, Description, Model, Brand, TireCount, CompanyId)
                        VALUES
                            (@licensePlate, @description, @model, @brand, @tireCount, @companyId)";

                    command.Parameters.AddWithValue("@licensePlate", car.LicensePlate);
                    command.Parameters.AddWithValue("@description",  car.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@model",        car.Model        ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@brand",        car.Brand        ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@tireCount",    car.TireCount);
                    command.Parameters.AddWithValue("@companyId",    car.CompanyId);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Oppgave 3 (del 2): Slett bil fra Car-tabellen.
        /// </summary>
        public void DeleteCar(int companyId, string licensePlate)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand { Connection = connection, CommandType = CommandType.Text })
                {
                    command.CommandText = @"
                        DELETE FROM Car
                        WHERE CompanyId = @companyId
                          AND LicensePlate = @licensePlate";

                    command.Parameters.AddWithValue("@companyId",    companyId);
                    command.Parameters.AddWithValue("@licensePlate", licensePlate);

                    command.ExecuteNonQuery();
                }
            }
        }

        /// <summary>
        /// Oppgave 4: Redigere data på en bil.
        /// Oppdaterer feltene i Car-tabellen basert på LicensePlate og CompanyId.
        /// </summary>
        public void UpdateCar(Car car)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");

            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (var command = new SqlCommand { Connection = connection, CommandType = CommandType.Text })
                {
                    command.CommandText = @"
                        UPDATE Car
                        SET
                            Description = @description,
                            Model       = @model,
                            Brand       = @brand,
                            TireCount   = @tireCount
                        WHERE
                            LicensePlate = @licensePlate
                            AND CompanyId = @companyId";

                    command.Parameters.AddWithValue("@description",  car.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@model",        car.Model        ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@brand",        car.Brand        ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@tireCount",    car.TireCount);
                    command.Parameters.AddWithValue("@licensePlate", car.LicensePlate);
                    command.Parameters.AddWithValue("@companyId",    car.CompanyId);

                    command.ExecuteNonQuery();
                }
            }
        }
    }
}
