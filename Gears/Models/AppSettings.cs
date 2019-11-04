using System;
using System.Collections.Generic;
using System.Text;
using SQLite;

namespace Gears.Models
{
    class AppSettings
    {
        [PrimaryKey]
        public int ID { get; set; } = 1; 
        public int? LastUsedProjectID { get; set; }
    }
}
