using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SK.NLtoSQL.Models
{
    public class ColumnsInfo
    {
        public string DbName { get; set; }
        public string SchemaName { get; set; }
        public string TableName { get; set; }         
        public string ColumnName { get; set; }
        public string ColumnDataType { get; set; }
        public string  ColumnDescription { get; set; }
    }
}
