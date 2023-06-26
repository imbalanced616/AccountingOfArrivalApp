using AccountingOfArrivalApp.Classes;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Windows.Controls;

namespace AccountingOfArrivalApp.Models
{
    public class Materials
    {
        private string plan;
        public int IdMaterial { get; set; }
        public string Name { get; set; }
        public string DrawingNumber
        {
            get
            {
                if (IdType == 4) return "Отсутствует";
                return plan;
            }
            set { plan = value; }
        }
        public int IdType { get; set; }
        public virtual Types Type { get { return Types.FirstOrDefault(IdType); } }
        public static Materials FirstOrDefault(int id)
        {
            return ClassHelper.db.Database.SqlQuery<Materials>($"select * from Nomenclature.dbo.Materials where idMaterial = {id}").FirstOrDefault();
        }
        public static Materials FindDuplicate(Materials m)
        {
            return ClassHelper.db.Database.SqlQuery<Materials>("select * from Nomenclature.dbo.Materials where idMaterial != @id and Name != @name and DrawingNumber = @plan", new SqlParameter("@id", m.IdMaterial), new SqlParameter("@name", m.Name), new SqlParameter("@plan", m.DrawingNumber)).FirstOrDefault();
        }
        public static Materials Single(Materials m)
        {
            return ClassHelper.db.Database.SqlQuery<Materials>("select * from Nomenclature.dbo.Materials where Name = @name and DrawingNumber = @plan and idType = @type", new SqlParameter("@name", m.Name), new SqlParameter("@plan", m.DrawingNumber), new SqlParameter("@type", m.IdType)).FirstOrDefault();
        }
        public static List<Materials> ToList()
        {
            return ClassHelper.db.Database.SqlQuery<Materials>("select * from Nomenclature.dbo.Materials").ToList();
        }
        public static List<Materials> ToListOnTypeInMenuItem(MenuItem menuItem)
        {
            return ToList().Where(x => x.Type.Name == menuItem.Header.ToString()).ToList();
        }
        public static List<Materials> ToListBySearch(string search)
        {
            return ToList().Where(x => x.Name.ToLower().Contains(search) || x.DrawingNumber.ToLower().Contains(search) || x.Type.Name.ToLower().Contains(search)).ToList();
        }
        public static int AddOrUpdate(Materials m)
        {
            if (m.IdMaterial == 0)
            {
                if (m.DrawingNumber == "Отсутствует") ClassHelper.db.Database.ExecuteSqlCommand("insert into Nomenclature.dbo.Materials values (@name,NULL,@id)", new SqlParameter("@name", m.Name), new SqlParameter("@id", m.IdType));
                else ClassHelper.db.Database.ExecuteSqlCommand("insert into Nomenclature.dbo.Materials values (@name,@plan,@id)", new SqlParameter("@name", m.Name), new SqlParameter("@plan", m.DrawingNumber), new SqlParameter("@id", m.IdType));
                return 1;
            }
            else
            {
                if (m.DrawingNumber == "Отсутствует") ClassHelper.db.Database.ExecuteSqlCommand("update Nomenclature.dbo.Materials set Name = @name, DrawingNumber = NULL, idType = @type where idMaterial = @id", new SqlParameter("@name", m.Name), new SqlParameter("@type", m.IdType), new SqlParameter("@id", m.IdMaterial));
                else ClassHelper.db.Database.ExecuteSqlCommand("update Nomenclature.dbo.Materials set Name = @name, DrawingNumber = @plan, idType = @type where idMaterial = @id", new SqlParameter("@name", m.Name), new SqlParameter("@type", m.IdType), new SqlParameter("@plan", m.DrawingNumber), new SqlParameter("@id", m.IdMaterial));
                return 2;
            }
        }
        public static int RemoveAtId(int id)
        {
            return ClassHelper.db.Database.ExecuteSqlCommand("delete from Nomenclature.dbo.Materials where idMaterial = @id", new SqlParameter("@id", id));
        }
    }
}
