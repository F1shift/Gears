using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
namespace Gears.DataBases
{
    class VersionManager
    {
        [PrimaryKey]
        public string Version { get; set; } = "x.x.x";
    }
}
