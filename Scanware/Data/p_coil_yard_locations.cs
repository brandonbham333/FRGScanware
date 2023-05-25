using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Scanware.Data;


namespace Scanware.Data
{
    public partial class coil_yard_locations
    {
        public static coil_yard_locations GetCoilYardLocation(string production_coil_no){
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.coil_yard_locations.SingleOrDefault(x => x.production_coil_no == production_coil_no);
        }

        public static List<coil_yard_locations> GetCoilYardLocations(string column)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.coil_yard_locations.Where(x => x.column == column).ToList();
        }

        public static coil_yard_locations UpdateCoilYardLocation(string production_coil_no, string column, string row, int change_user_id, string scanner_flag)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            coil_yard_locations new_coil_yard_location = db.coil_yard_locations.SingleOrDefault(x => x.production_coil_no == production_coil_no);

            new_coil_yard_location.column = column;
            new_coil_yard_location.row = row;
            new_coil_yard_location.change_user_id = change_user_id;
            new_coil_yard_location.change_datetime = DateTime.Now;
            new_coil_yard_location.scanner_used = scanner_flag;

            db.SaveChanges();

            return new_coil_yard_location;
        }

        public static int CoilLocationAudit(string column, string row)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var count = from cyl in db.coil_yard_locations
                        where cyl.column == column
                      && cyl.row == row
                        select cyl;

            DateTime now = DateTime.Now;
            foreach (coil_yard_locations c in count)
            {
                DateTime d = (DateTime)c.change_datetime;
                double totalminutes = (now - d).TotalMinutes;

                if (totalminutes > 15)
                    {
                    return 0;
                    }
            }
            return 1;
        }

        private static DateTime cast(DateTime dateTime)
        {
            throw new NotImplementedException();
        }

        public static void DeleteCoilYardLocation(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            coil_yard_locations new_coil_yard_location = db.coil_yard_locations.SingleOrDefault(x => x.production_coil_no == production_coil_no);

            if (new_coil_yard_location != null)
            {
                db.coil_yard_locations.Remove(new_coil_yard_location);
            }
            
            db.SaveChanges();
        }

        public static coil_yard_locations InsertCoilYardLocation(string production_coil_no, string column, string row, int change_user_id, string scanner_flag)
        {
            coil_yard_locations new_coil_yard_location = new coil_yard_locations()
            {
                production_coil_no = production_coil_no,
                column = column,
                row = row,
                add_datetime = DateTime.Now,
                add_user_id = change_user_id, 
                change_user_id = change_user_id,
                change_datetime = DateTime.Now,
                scanner_used = scanner_flag
            };

            sdipdbEntities db = ContextHelper.SDIPDBContext;
            db.coil_yard_locations.Add(new_coil_yard_location);
            db.SaveChanges();

            return new_coil_yard_location;
        }

        public static List<CoilsInLocation> GetCoilsInLocation(string column, string row)
        {
            List<CoilsInLocation> coils = new List<CoilsInLocation>();
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var loc = from cyl in db.coil_yard_locations
                      where cyl.column == column
                      && cyl.row == row
                      select cyl.production_coil_no;

            foreach (string coil_prod in loc)
            {
                coils.Add(new CoilsInLocation(){production_coil_no = coil_prod, status = "PENDING"});
            }

            return coils;
        }
    }
}