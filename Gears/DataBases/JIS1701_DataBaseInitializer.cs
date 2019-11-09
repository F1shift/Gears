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
        static string dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"JIS1701_{UsingVersion}.db3");
        public static SQLiteAsyncConnection DataBase {
            get
            {
                return _DataBase;
            }
        }

        public static async Task<bool> Initalize() {
            if (!File.Exists(dbpath))
            {
                ResetDBFile();
            }
            _DataBase = new SQLiteAsyncConnection(dbpath);
            return true;
        }

        public static void CreateBlankDB() {
            var path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "BlankDB.db3");
            if (File.Exists(path))
            {
                File.Delete(path);
            }
            var DataBase = new SQLiteConnection(path);
            DataBase.CreateTable<AppSettings>();
            DataBase.Insert(new AppSettings() { ID = 1, LastUsedProjectID = 1 });
            DataBase.CreateTable<ModuleItem>();
        }

        public static void ResetDBFile() {
            if (_DataBase != null)
            {
                SQLiteAsyncConnection.ResetPool();
            }
            foreach (var fileName in Directory.GetFiles(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData)))
            {
                if (fileName.Contains(".db3"))
                {
                    File.Delete(fileName);
                }
            }
            var targetStream = File.Create(dbpath);
            var sourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Gears.DataBases.JIS1701.db3");
            sourceStream.CopyTo(targetStream);
            targetStream.Close();
            sourceStream.Close();
            return;
        }
    }
}
