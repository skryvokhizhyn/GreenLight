using System;
using System.Collections.Generic;
using System.IO;
using System.Data;

using Mono.Data.Sqlite;

using NLog;

namespace GreenLightTracker.Src
{
    class StorageLocalDB : StorageInterface
    {
        private static Logger m_logger = LogManager.GetCurrentClassLogger();

        private SqliteConnection m_connection;
        private bool m_dbAlreadyInitialized = false;
        private string m_pathToDB;

        public StorageLocalDB()
        {
            Open();
        }

        public void Store(Guid uuid, ICollection<GpsLocation> path)
        {
            foreach (var l in path)
            {
                string sql = string.Format("Insert Into GpsLocation "
                    + "(timestamp, longitude, latitude, altitude, speed, ride_id) "
                    + "values ({0}, {1}, {2}, {3}, {4}, '{5}')", l.Timestamp, l.Longitude, l.Latitude, l.Altitude, l.Speed, uuid.ToString());

                GetQuery().ExecuteCommand(sql);
            }
        }

        /*public long GetGpsLocationCount()
        {
            string cmd = "SELECT count(gpsLocationId) FROM GpsLocation";
            return GetQuery().ExecuteScalar<long>(cmd);
        }*/

        public ICollection<GpsLocation> GetAllGpsLocations()
        {
            string cmd = "SELECT longitude, latitude, altitude, speed, timestamp "
                + "FROM GpsLocation";

            var locations = new LinkedList<GpsLocation>();

            using (IDataReader reader = GetQuery().ExecuteQuery(cmd))
            {
                while (reader.Read())
                {
                    locations.AddLast(new GpsLocation
                    {
                        Longitude = reader.GetDouble(0),
                        Latitude = reader.GetDouble(1),
                        Altitude = reader.GetDouble(2),
                        Timestamp = reader.GetInt32(4)
                    });
                }
            }

            return locations;
        }

        public void Close()
        {
            m_connection.Dispose();
        }

        private void Open()
        {
            if (m_connection != null && m_connection.State == System.Data.ConnectionState.Open)
            {
                throw new Exception("DB connection is already in Open state");
            }

            m_pathToDB = Path.Combine(
                Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath,
                "GreenLightTracker",
                "db_green_light_tracker.db");

            m_dbAlreadyInitialized = File.Exists(m_pathToDB);

            var connectionString = string.Format("Data Source={0};Version=3;", m_pathToDB);

            m_connection = new SqliteConnection(connectionString);

            try
            {
                m_connection.Open();
            }
            catch (Exception ex)
            {
                m_logger.Error(ex);
                throw;
            }

            InitializeDb();
        }

        public void CleanGpsLocationTable()
        {
            string cmd = "DELETE FROM GpsLocation";
            GetQuery().ExecuteCommand(cmd);
        }

        public string CreateBackup()
        {
            var backupFolder = System.IO.Path.Combine(
                Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads).AbsolutePath,
                "GreenLightTracker");

            return CreateBackup(backupFolder);
        }

        public string CreateBackup(string backupFolder)
        {
            var time = Java.Lang.JavaSystem.CurrentTimeMillis().ToString();
            string dbFileName = $"db_backup_{time}.db";

            var destAbsolutePath = Path.Combine(backupFolder, dbFileName);

            try
            {
                if (!Directory.Exists(backupFolder))
                {
                    Directory.CreateDirectory(backupFolder);
                }

                File.Copy(m_pathToDB, destAbsolutePath);
            }
            catch (Exception ex)
            {
                m_logger.Error(ex);
                throw;
            }

            return destAbsolutePath;
        }

        private void InitializeDb()
        {
            if (!m_dbAlreadyInitialized)
            {
                string sql = "CREATE TABLE GpsLocation "
                    + "(gpsLocationId INTEGER PRIMARY KEY AUTOINCREMENT"
                    + ", timestamp INTEGER"
                    + ", longitude float"
                    + ", latitude float"
                    + ", altitude float"
                    + ", speed float"
                    + ", ride_id text)";

                GetQuery().ExecuteCommand(sql);

                m_dbAlreadyInitialized = true;
            }
        }

        private DBQuery GetQuery()
        {
            return new DBQuery(m_connection);
        }
    }
}