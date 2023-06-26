using AccountingOfArrivalApp.Classes;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Controls;

namespace AccountingOfArrivalApp.Models
{
    public class Movement
    {
        public int IdMovement { get { return ClassHelper.db.Database.SqlQuery<int>("select idMovement from Nomenclature.dbo.Movement where idWarehouse = @warehouse and idComposition = @composition and ArrivalOrExpenditure = 0", new SqlParameter("@warehouse", IdWarehouse), new SqlParameter("@composition", IdComposition)).FirstOrDefault(); } }
        public int IdWarehouse { get; set; }
        public int IdComposition { get; set; }
        public int IdUser { get; set; }
        public int PartOfQuantity { get; set; }
        public bool ArrivalOrExpenditure { get; set; }
        public virtual Warehouses Warehouse { get { return Warehouses.ToList().FirstOrDefault(x => x.IdWarehouse == IdWarehouse); } }
        public virtual CompositionsOfInvoice Composition { get { return ClassHelper.db.CompositionsOfInvoice.FirstOrDefault(x => x.idComposition == IdComposition); } }
        public virtual Users User { get { return ClassHelper.db.Users.FirstOrDefault(x => x.idUser == IdUser); } }

        public static List<Movement> ToList()
        {
            return ClassHelper.db.Database.SqlQuery<Movement>("select * from Nomenclature.dbo.Movement where ArrivalOrExpenditure = 0").ToList();
        }
        public static List<Movement> ToListBySearch(string search)
        {
            return ToList().Where(x => x.IdMovement.ToString().Contains(search) || x.Composition.idInvoice.ToString().Contains(search) || x.Warehouse.Name.Contains(search) || x.Composition.Materials.Name.Contains(search) || x.User.FIO.Contains(search) || x.PartOfQuantity.ToString().Contains(search)).ToList();
        }
        public static List<Movement> ToContextList(int idInvoice)
        {
            return ClassHelper.db.Database.SqlQuery<Movement>("select M.* from Nomenclature.dbo.Movement M join AccountingOfArrival.dbo.CompositionsOfInvoice C on M.idComposition = C.idComposition where idInvoice = @id and ArrivalOrExpenditure = 0", new SqlParameter("@id", idInvoice)).ToList();
        }
        public static List<Movement> ToListByFilter(MenuItem menuItem)
        {
            return ToList().Where(x => x.Warehouse.Name == menuItem.Header.ToString()).ToList();
        }
        public static int RemoveAtIdUser(int id)
        {
            return ClassHelper.db.Database.ExecuteSqlCommand("delete from Nomenclature.dbo.Movement where idUser = @id", new SqlParameter("@id", id));
        }
        public static int RemoveAtIdWarehouse(int id)
        {
            return ClassHelper.db.Database.ExecuteSqlCommand("delete from Nomenclature.dbo.Movement where idWarehouse = @id", new SqlParameter("@id", id));
        }
        public static void AddOrUpdateOrDelete(Movement m)
        {
            if (m.IdMovement == 0 && m.PartOfQuantity != 0) ClassHelper.db.Database.ExecuteSqlCommand("insert into Nomenclature.dbo.Movement values (@warehouse,@composition,@user,@quantity,0)", new SqlParameter("@warehouse", m.IdWarehouse), new SqlParameter("@composition", m.IdComposition), new SqlParameter("@user", Classes.User.CurrentUser.Id), new SqlParameter("@quantity", m.PartOfQuantity));
            if (m.IdMovement != 0 && m.PartOfQuantity != 0) ClassHelper.db.Database.ExecuteSqlCommand("update Nomenclature.dbo.Movement set idWarehouse = @warehouse, idComposition = @composition, idUser = @user, PartOfQuantity = @quantity where idMovement = @id", new SqlParameter("@warehouse", m.IdWarehouse), new SqlParameter("@composition", m.IdComposition), new SqlParameter("@user", Classes.User.CurrentUser.Id), new SqlParameter("@quantity", m.PartOfQuantity), new SqlParameter("@id", m.IdMovement));
            if (m.IdMovement != 0 && m.PartOfQuantity == 0) ClassHelper.db.Database.ExecuteSqlCommand("delete Nomenclature.dbo.Movement where idMovement = @id and ArrivalOrExpenditure = 0", new SqlParameter("@id", m.IdMovement));
        }
        public static int RemoveAtId(int id)
        {
            return ClassHelper.db.Database.ExecuteSqlCommand("delete from Nomenclature.dbo.Materials where idMaterial = @id", new SqlParameter("@id", id));
        }
    }
}
