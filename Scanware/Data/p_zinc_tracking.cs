using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class zinc_tracking
    {   
        public int ingot_no;
        public static zinc_tracking GetIngot(string ingot_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            zinc_tracking ingot = db.zinc_tracking.SingleOrDefault(x => x.ingot_id == ingot_id);

            return ingot;
        }

        public static zinc_tracking ConsumeIngot(string ingot_id, string status_cd, string line_consumed, int consume_user_id, DateTime consume_dt)
        {

            sdipdbEntities db = ContextHelper.SDIPDBContext;

            zinc_tracking ConsumeIngot = db.zinc_tracking.SingleOrDefault(x => x.ingot_id == ingot_id);

            int line = int.Parse(line_consumed);

            ConsumeIngot.consumed_user_id = consume_user_id;
            ConsumeIngot.status_cd = status_cd;
            ConsumeIngot.line_consumed = (byte)line;
            ConsumeIngot.consumed_datetime = consume_dt;

            db.SaveChanges();

            return ConsumeIngot;

        }
     
        public static List<zinc_tracking> GetInventory()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            List<zinc_tracking> list = new List<zinc_tracking>();

            list = db.zinc_tracking.Where(x => x.status_cd == "I").OrderBy(x => x.ingot_id.Substring(1,7)).ToList() ?? new List<zinc_tracking>();

            return list;
        }
        public void UpdateUser(zinc_tracking ingotToUpdate)
        {
            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
            DateTime now = DateTime.Now;

            sdipdbEntities db = ContextHelper.SDIPDBContext;


            ingotToUpdate.change_user_id = current_application_security.user_id;
            ingotToUpdate.change_datetime = now;

            db.SaveChanges();
        }
        public void AddToInventory(zinc_tracking ingotToUpdate)
        {
            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
            DateTime now = DateTime.Now;

            sdipdbEntities db = ContextHelper.SDIPDBContext;

            ingotToUpdate.change_user_id = current_application_security.user_id;
            ingotToUpdate.change_datetime = now;
            ingotToUpdate.status_cd = "I";
            ingotToUpdate.line_consumed = null;
            ingotToUpdate.consumed_datetime = null;
            ingotToUpdate.consumed_user_id = null;

            db.SaveChanges();
        }
    }
}