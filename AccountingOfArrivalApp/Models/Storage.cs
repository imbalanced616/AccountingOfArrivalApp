using AccountingOfArrivalApp.Classes;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Controls;

namespace AccountingOfArrivalApp.Models
{
    public class Storage
    {
        public int IdStorage { get; set; }
        public int IdWarehouse { get; set; }
        public int IdMaterial { get; set; }
        public int Quantity { get; set; }
        public int QuantityDistribute { get; set; }
        public virtual Warehouses Warehouse { get { return Warehouses.FirstOrDefault(IdWarehouse); } }
        public virtual Materials Material { get { return Materials.FirstOrDefault(IdMaterial); } }
        public static List<Storage> ToList()
        {
            return ClassHelper.db.Database.SqlQuery<Storage>("select * from Nomenclature.dbo.Storage").ToList();
        }

        public static List<Storage> ToContextListAndAllWarehouse(CompositionsOfInvoice composition)
        {
            List<Storage> list = ClassHelper.db.Database.SqlQuery<Storage>("select * from Nomenclature.dbo.Storage where idMaterial = @id", new SqlParameter("@id", composition.idMaterial)).ToList();
            foreach (Warehouses w in Warehouses.ToList())
                if (!(from l in list select l.IdWarehouse).Contains(w.IdWarehouse))
                    list.Add(new Storage() { IdWarehouse = w.IdWarehouse, IdMaterial = (int)composition.idMaterial });
            foreach (Storage s in list) s.QuantityDistribute = ClassHelper.db.Database.SqlQuery<int?>("select sum(PartOfQuantity) from Nomenclature.dbo.Movement M join AccountingOfArrival.dbo.CompositionsOfInvoice C on C.idComposition = M.idComposition where M.idComposition = @idC and idMaterial = @idM and idWarehouse = @idW and ArrivalOrExpenditure = 0", new SqlParameter("@idC", composition.idComposition), new SqlParameter("@idM", composition.idMaterial), new SqlParameter("@idW", s.IdWarehouse)).FirstOrDefault() == null
                        ? 0 : ClassHelper.db.Database.SqlQuery<int>("select sum(PartOfQuantity) from Nomenclature.dbo.Movement M join AccountingOfArrival.dbo.CompositionsOfInvoice C on C.idComposition = M.idComposition where M.idComposition = @idC and idMaterial = @idM and idWarehouse = @idW and ArrivalOrExpenditure = 0", new SqlParameter("@idC", composition.idComposition), new SqlParameter("@idM", composition.idMaterial), new SqlParameter("@idW", s.IdWarehouse)).FirstOrDefault();
            return list.OrderBy(x => x.Warehouse.Name).ToList();
        }
        public static List<Storage> ToListByFilter(MenuItem menuItem, int filterBy)
        {
            switch (filterBy)
            {
                case 0: return ToList().Where(x => x.Material.Type.Name == menuItem.Header.ToString()).ToList();
                case 1: return ToList().Where(x => x.Warehouse.Name == menuItem.Header.ToString()).ToList();
                default: return ToList();
            }
        }
        public static List<Storage> ToListBySearch(string search)
        {
            return ToList().Where(x => x.Material.Name.ToLower().Contains(search) || x.Material.Type.Name.ToLower().Contains(search) || x.Material.DrawingNumber.ToLower().Contains(search)).ToList();
        }
        public static void AddOrUpdate(Movement m)
        {
            Storage s = ClassHelper.db.Database.SqlQuery<Storage>("select * from Nomenclature.dbo.Storage where idWarehouse = @warehouse and idMaterial = @material", new SqlParameter("@warehouse", m.IdWarehouse), new SqlParameter("@material", m.Composition.idMaterial)).FirstOrDefault();
            if (s == null) ClassHelper.db.Database.ExecuteSqlCommand("insert into Nomenclature.dbo.Storage values (@warehouse,@material,@quantity)", new SqlParameter("@warehouse", m.IdWarehouse), new SqlParameter("@material", m.Composition.idMaterial), new SqlParameter("@quantity", m.PartOfQuantity));
            else ClassHelper.db.Database.ExecuteSqlCommand("update Nomenclature.dbo.Storage set Quantity = Quantity + @quantity where idStorage = @id", new SqlParameter("@quantity", m.PartOfQuantity), new SqlParameter("@id", s.IdStorage));
        }
        public static int RemoveAtIdMaterial(int id)
        {
            return ClassHelper.db.Database.ExecuteSqlCommand("delete from Nomenclature.dbo.Storage where idMaterial = @id", new SqlParameter("@id", id));
        }
        public static int RemoveAtIdWarehouse(int id)
        {
            return ClassHelper.db.Database.ExecuteSqlCommand("delete from Nomenclature.dbo.Storage where idWarehouse = @id", new SqlParameter("@id", id));
        }
    }
}
