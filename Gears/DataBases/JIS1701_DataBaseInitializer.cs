using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using SQLite;
using Gears.Models;
using System.Reflection;

namespace Gears.DataBases
{
    static class JIS1701DataBase
    {
        static SQLiteAsyncConnection _DataBase;
        static string UsingVersion = "1.0.0";
        static string dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "JIS1701.db3");
        public static SQLiteAsyncConnection DataBase {
            get
            {
                if (_DataBase == null)
                {
                    if (!File.Exists(dbpath))
                    {
                        ResetDBFile().Wait();
                        _DataBase = new SQLiteAsyncConnection(dbpath);
                    }
                    else
                    {
                        _DataBase = new SQLiteAsyncConnection(dbpath);
                        try
                        {
                            var task = _DataBase.Table<VersionManager>().FirstAsync();
                            task.Wait();
                            var versionManager = task.Result;
                            if (UsingVersion != versionManager.Version)
                            {
                                ResetDBFile().Wait();
                                _DataBase = new SQLiteAsyncConnection(dbpath);
                            }
                        }
                        catch
                        {
                            ResetDBFile().Wait();
                            _DataBase = new SQLiteAsyncConnection(dbpath);
                        }
                    }
                }
                return _DataBase;
            }
        }

        public static async Task<bool> ResetDBFile() {
            if (_DataBase != null)
            {
                SQLiteAsyncConnection.ResetPool();
            }
            if (File.Exists(dbpath))
            {
                File.Delete(dbpath);
            }
            var targetStream = File.Create(dbpath);
            var sourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Gears.DataBases.JIS1701.db3");
            sourceStream.CopyTo(targetStream);
            targetStream.Close();
            sourceStream.Close();
            return true;
        }
    }
}
