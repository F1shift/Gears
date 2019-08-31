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
        public static SQLiteAsyncConnection DataBase {
            get
            {
                if (_DataBase == null)
                {
                    string dbpath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "JIS1701.db3");
                    if (!File.Exists(dbpath))
                    {
                        var targetStream = File.Create(dbpath);
                        var sourceStream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Gears.DataBases.JIS1701.db3");
                        sourceStream.CopyTo(targetStream);
                        targetStream.Close();
                        sourceStream.Close();
                    }
                    _DataBase = new SQLiteAsyncConnection(dbpath);
                    
                }
                return _DataBase;
            }
        }
    }
}
