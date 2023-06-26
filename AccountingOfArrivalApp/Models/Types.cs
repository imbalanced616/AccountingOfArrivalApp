using AccountingOfArrivalApp.Classes;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace AccountingOfArrivalApp.Models
{
    public class Types
    {
        public int IdType { get; set; }
        public string Name { get; set; }
        public static Types FirstOrDefault(int id)
        {
            return ClassHelper.db.Database.SqlQuery<Types>("select * from Nomenclature.dbo.Types where idType = @id", new SqlParameter("@id", id)).FirstOrDefault();
        }
        public static List<Types> ToList()
        {
            return ClassHelper.db.Database.SqlQuery<Types>("select * from Nomenclature.dbo.Types").ToList(); ;
        }
    }
}
