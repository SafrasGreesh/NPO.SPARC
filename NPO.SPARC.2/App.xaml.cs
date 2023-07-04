using Npgsql;
using System;
using System.Windows;

namespace NPO.SPARC._2
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var tempConnectionString = "Host=localhost;Port=5432;Username=postgres;Password="СЮДА_ВСТАВЬТЕ_СВОЙ_ПАРОЛЬ_ОТ_POSTGRE";Database=postgres;";
            var mainConnectionString = "Host=localhost;Port=5432;Username=admin;Password=1;Database=NPO.SPARC;";

            using (var connection = new NpgsqlConnection(tempConnectionString))
            {
                connection.Open();

                using (var checkRoleCommand = new NpgsqlCommand("SELECT 1 FROM pg_roles WHERE rolname = 'admin'", connection))
                {
                    var result = checkRoleCommand.ExecuteScalar();
                    if (result == null)
                    {
                        using (var createRoleCommand = new NpgsqlCommand("CREATE ROLE admin WITH LOGIN SUPERUSER INHERIT CREATEDB CREATEROLE REPLICATION ENCRYPTED PASSWORD 'SCRAM-SHA-256$4096:fc9oTfFtgnGTc7883oBaZQ==$UHINAJCTmChrsrZqnjbhoKNVYp+rf4GCx19nnUWNHxQ=:lc0ZqVZv58Bc6iXRSxgxJLGMagWqSGvzOwh41N9HNqU=';", connection))
                        {
                            createRoleCommand.ExecuteNonQuery();
                        }
                    }
                }

                using (var checkDbCommand = new NpgsqlCommand("SELECT 1 FROM pg_database WHERE datname = 'NPO.SPARC'", connection))
                {
                    var result = checkDbCommand.ExecuteScalar();
                    if (result == null)
                    {
                        using (var createDbCommand = new NpgsqlCommand("CREATE DATABASE \"NPO.SPARC\" OWNER admin ENCODING 'UTF8' LC_COLLATE 'Russian_Russia.1251' LC_CTYPE 'Russian_Russia.1251' TABLESPACE pg_default CONNECTION LIMIT -1 IS_TEMPLATE False;", connection))
                        {
                            createDbCommand.ExecuteNonQuery();
                        }
                    }
                }
            }

            using (var connection = new NpgsqlConnection(mainConnectionString))
            {
                connection.Open();

                using (var checkTestsTableCommand = new NpgsqlCommand("SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'Tests')", connection))
                {
                    var result = (bool)checkTestsTableCommand.ExecuteScalar();
                    if (!result)
                    {
                        using (var createTestsTableCommand = new NpgsqlCommand("CREATE TABLE public.\"Tests\" (\"TestId\" serial PRIMARY KEY, \"TestDate\" date NOT NULL, \"BlockName\" text NOT NULL, \"Note\" text);", connection))
                        {
                            createTestsTableCommand.ExecuteNonQuery();
                        }
                    }
                }

                using (var checkParametersTableCommand = new NpgsqlCommand("SELECT EXISTS (SELECT 1 FROM information_schema.tables WHERE table_schema = 'public' AND table_name = 'Parameters')", connection))
                {
                    var result = (bool)checkParametersTableCommand.ExecuteScalar();
                    if (!result)
                    {
                        using (var createParametersTableCommand = new NpgsqlCommand("CREATE TABLE public.\"Parameters\" (\r\n    \"ParameterId\" serial PRIMARY KEY,\r\n    \"TestId\" integer NOT NULL,\r\n    \"ParameterName\" text NOT NULL,\r\n    \"RequiredValue\" numeric NOT NULL,\r\n    \"MeasuredValue\" numeric NOT NULL,\r\n    CONSTRAINT \"Parameters_TestId_fkey\" FOREIGN KEY (\"TestId\") REFERENCES public.\"Tests\" (\"TestId\") ON UPDATE NO ACTION ON DELETE NO ACTION\r\n);\r\n", connection))
                        {
                            createParametersTableCommand.ExecuteNonQuery();
                        }
                    }
                }
            }

        }
    }
}
