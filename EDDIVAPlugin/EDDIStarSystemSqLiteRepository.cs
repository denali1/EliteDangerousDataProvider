﻿using EliteDangerousDataDefinitions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;

using System.Text;
using System.Threading.Tasks;

namespace EDDIVAPlugin
{
    public class EDDIStarSystemSqLiteRepository : SqLiteBaseRepository, IEDDIStarSystemRepository
    {
        private static string CREATE_SQL = @"
                        CREATE TABLE IF NOT EXISTS starsystems(
                          eliteid INT
                         ,eddbid INT
                         ,name TEXT NOT NULL
                         ,totalvisits INT NOT NULL
                         ,lastvisit DATETIME NOT NULL
                         ,previousvisit DATETIME
                         ,starsystem TEXT NOT NULL
                         ,starsystemlastupdated DATETIME NOT NULL)";
        private static string INSERT_SQL = @"
                    INSERT INTO starsystems(
                      eliteid
                     , eddbid
                     , name
                     , totalvisits
                     , lastvisit
                     , starsystem
                     , starsystemlastupdated)
                    VALUES(@eliteid, @eddbid, @name, @totalvisits, @lastvisit, @starsystem, @starsystemlastupdated)";
        private static string UPDATE_SQL = @"
                    UPDATE starsystems
                    SET eliteid = @eliteid
                       ,eddbid = @eddbid
                       ,totalvisits = totalvisits
                       ,lastvisit = @lastvisit
                       ,previousvisit = previousvisit
                       ,starsystem = starsystem
                       ,starsystemlastupdated = @starsystemlastupdated
                    WHERE name = @name";
        private static string SELECT_BY_NAME_SQL = @"
                    SELECT eliteId,
                           eddbid,
                           name,
                           totalvisits,
                           lastvisit,
                           previousvisit,
                           starsystem,
                           starsystemlastupdated
                    FROM starsystems
                    WHERE name = @name";

        public EDDIStarSystem GetEDDIStarSystem(string name)
        {
            if (!File.Exists(DbFile)) return null;

            using (var con = SimpleDbConnection())
            {
                con.Open();
                using (var cmd = new SQLiteCommand(con))
                {
                    cmd.CommandText = SELECT_BY_NAME_SQL;
                    cmd.Prepare();
                    cmd.Parameters.AddWithValue("@name", name);
                    using (SQLiteDataReader rdr = cmd.ExecuteReader())
                    {
                        if (rdr.Read())
                        {
                            EDDIStarSystem result = new EDDIStarSystem();
                            if (!rdr.IsDBNull(0)) result.EliteID = rdr.GetInt32(0);
                            if (!rdr.IsDBNull(1)) result.EDDBID = rdr.GetInt32(1);
                            result.Name = rdr.GetString(2);
                            result.TotalVisits = rdr.GetInt32(3);
                            result.LastVisit = rdr.GetDateTime(4);
                            if (!rdr.IsDBNull(5)) result.PreviousVisit = rdr.GetDateTime(5);
                            result.StarSystem = JsonConvert.DeserializeObject<StarSystem>(rdr.GetString(6));
                            result.StarSystemLastUpdated = rdr.GetDateTime(7);
                            return result;
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
                con.Close();
            }
        }

        public void SaveEDDIStarSystem(EDDIStarSystem eddiStarSystem)
        {
            if (!File.Exists(DbFile)) CreateDatabase();

            using (var con = SimpleDbConnection())
            {
                con.Open();
                if (eddiStarSystem.TotalVisits == 1)
                {
                    using (var cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = INSERT_SQL;
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@eliteid", eddiStarSystem.EliteID);
                        cmd.Parameters.AddWithValue("@eddbid", eddiStarSystem.EDDBID);
                        cmd.Parameters.AddWithValue("@name",eddiStarSystem.Name);
                        cmd.Parameters.AddWithValue("@totalvisits", eddiStarSystem.TotalVisits);
                        cmd.Parameters.AddWithValue("@lastvisit", eddiStarSystem.LastVisit);
                        cmd.Parameters.AddWithValue("@starsystem", JsonConvert.SerializeObject(eddiStarSystem.StarSystem));
                        cmd.Parameters.AddWithValue("@starsystemlastupdated", eddiStarSystem.StarSystemLastUpdated);
                        cmd.ExecuteNonQuery();
                    }
                }
                else
                {
                    using (var cmd = new SQLiteCommand(con))
                    {
                        cmd.CommandText = UPDATE_SQL;
                        cmd.Prepare();
                        cmd.Parameters.AddWithValue("@eliteid", eddiStarSystem.EliteID);
                        cmd.Parameters.AddWithValue("@eddbid", eddiStarSystem.EDDBID);
                        cmd.Parameters.AddWithValue("@name", eddiStarSystem.Name);
                        cmd.Parameters.AddWithValue("@totalvisits", eddiStarSystem.TotalVisits);
                        cmd.Parameters.AddWithValue("@lastvisit", eddiStarSystem.LastVisit);
                        cmd.Parameters.AddWithValue("@previousvisit", eddiStarSystem.PreviousVisit);
                        cmd.Parameters.AddWithValue("@starsystem", eddiStarSystem.StarSystem);
                        cmd.Parameters.AddWithValue("@starsystemlastupdated", eddiStarSystem.StarSystemLastUpdated);
                        cmd.ExecuteNonQuery();
                    }
                }
                con.Close();
            }
        }

        private static void CreateDatabase()
        {
            //SQLiteConnection.CreateFile(DbFile);
            using (var con = SimpleDbConnection())
            {
                con.Open();
                using (var cmd = new SQLiteCommand(CREATE_SQL, con))
                {
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
        }
    }
}