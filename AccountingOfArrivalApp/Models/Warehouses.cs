using AccountingOfArrivalApp.Classes;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace AccountingOfArrivalApp.Models
{
    public class Warehouses
    {
        public int IdWarehouse { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public static Warehouses FirstOrDefault(int id)
        {
            return ClassHelper.db.Database.SqlQuery<Warehouses>($"select * from Nomenclature.dbo.Warehouses where idWarehouse = {id}").FirstOrDefault();
        }
        public static List<Warehouses> ToList()
        {
            return ClassHelper.db.Database.SqlQuery<Warehouses>("select * from Nomenclature.dbo.Warehouses").ToList();
        }
        public static List<Warehouses> ToListBySearch(string search)
        {
            return ToList().Where(x => x.Name.ToLower().Contains(search) || x.Address.ToLower().Contains(search)).ToList();
        }
        public static int AddOrUpdate(Warehouses w)
        {
            if (w.IdWarehouse == 0)
            {
                ClassHelper.db.Database.ExecuteSqlCommand("insert into Nomenclature.dbo.Warehouses values (@name,@address)", new SqlParameter("@name", w.Name), new SqlParameter("@address", w.Address));
                return 1;
            }
            else
            {
                ClassHelper.db.Database.ExecuteSqlCommand("update Nomenclature.dbo.Warehouses set Name = @name, Address = @address where idWarehouse = @id", new SqlParameter("@name", w.Name), new SqlParameter("@address", w.Address), new SqlParameter("@id", w.IdWarehouse));
                return 2;
            }
        }
        public static int RemoveAtId(int id)
        {
            return ClassHelper.db.Database.ExecuteSqlCommand("delete from Nomenclature.dbo.Warehouses where idWarehouse = @id", new SqlParameter("@id", id));
        }
    }
}
