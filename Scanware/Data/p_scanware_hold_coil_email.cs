using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class scanware_hold_coil_email
    {
        public static List<scanware_hold_coil_email> GetAllHoldEmails()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.scanware_hold_coil_email.OrderBy(x => x.facility_cd).ToList();
        }

        public static List<scanware_hold_coil_email> GetHoldEmailsByFacilityPlusDefault(string facility_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.scanware_hold_coil_email.Where(x => x.facility_cd == facility_cd || x.facility_cd == "1").OrderBy(x => x.facility_cd).ToList();
        }

        public static List<scanware_hold_coil_email> GetHoldEmailsByFacility(string facility_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.scanware_hold_coil_email.Where(x => x.facility_cd == facility_cd).OrderBy(x => x.facility_cd).ToList();
        }

        public static void AddHoldEmail(string email_address, string facility_cd)
        {

            sdipdbEntities db = ContextHelper.SDIPDBContext;

            scanware_hold_coil_email to_update = new scanware_hold_coil_email();

            to_update.email_address = email_address;
            to_update.facility_cd = facility_cd;

            db.scanware_hold_coil_email.Add(to_update);

            db.SaveChanges();

        }

        public static scanware_hold_coil_email GetHoldEmailByPK(int pk)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.scanware_hold_coil_email.Where(x=>x.pk==pk).FirstOrDefault();
        }


        public static void DeleteHoldEmail(int pk)
        {

            sdipdbEntities db = ContextHelper.SDIPDBContext;

            scanware_hold_coil_email to_update = db.scanware_hold_coil_email.Where(x => x.pk == pk).FirstOrDefault();
            
            db.scanware_hold_coil_email.Remove(to_update);

            db.SaveChanges();

        }


    }
}