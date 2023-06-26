using AccountingOfArrivalApp.Classes;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace AccountingOfArrivalApp.Models
{
    public class Hierarchy
    {
        public int IdHierarchy { get; set; }
        public int IdParent { get; set; }
        public int IdChild { get; set; }
        public int Quantity { get; set; }
        public virtual Materials MaterialParent { get { return Materials.ToList().FirstOrDefault(x => x.IdMaterial == IdParent); } }
        public virtual Materials MaterialChild { get { return Materials.ToList().FirstOrDefault(x => x.IdMaterial == IdChild); } }
        public static List<Hierarchy> ToList()
        {
            return ClassHelper.db.Database.SqlQuery<Hierarchy>("select * from Nomenclature.dbo.Hierarchy").ToList();
        }
        public static List<Hierarchy> ToListByIdMaterial(int idMaterial)
        {
            return ClassHelper.db.Database.SqlQuery<Hierarchy>("select * from Nomenclature.dbo.Hierarchy where idParent = @id or idChild = @id", new SqlParameter("@id", idMaterial)).ToList();
        }
        public static void AddOrUpdate(Hierarchy h)
        {
            if (h.IdHierarchy == 0) ClassHelper.db.Database.ExecuteSqlCommand("insert into Nomenclature.dbo.Hierarchy values (@idParent,@idChild,@quantity)", new SqlParameter("@idParent", h.IdParent), new SqlParameter("@idChild", h.IdChild), new SqlParameter("@quantity", h.Quantity));
            else ClassHelper.db.Database.ExecuteSqlCommand("update Nomenclature.dbo.Hierarchy set idParent = @idParent, idChild = @idChild, Quantity = @quantity where idHierarchy = @id", new SqlParameter("@idParent", h.IdParent), new SqlParameter("@idChild", h.IdChild), new SqlParameter("@quantity", h.Quantity), new SqlParameter("@id", h.IdHierarchy));
        }
        public static int RemoveAtId(int id)
        {
            return ClassHelper.db.Database.ExecuteSqlCommand("delete from Nomenclature.dbo.Hierarchy where idHierarchy = @id", new SqlParameter("@id", id));
        }
        public static int RemoveAtIdMaterial(int id)
        {
            return ClassHelper.db.Database.ExecuteSqlCommand("delete from Nomenclature.dbo.Hierarchy where idParent = @id or idChild = @id", new SqlParameter("@id", id));
        }
    }
}
