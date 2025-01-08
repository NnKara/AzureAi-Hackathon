using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SK.NLtoSQL.Models
{
    public class TableInfo
    {
        public string DbName { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }
        public string TableDescription { get; set; }
    }
}
