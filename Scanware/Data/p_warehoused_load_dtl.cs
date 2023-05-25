using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class warehoused_load_dtl
    {
        public string change_user_name
        {
            get
            {

                sdipdbEntities db = ContextHelper.SDIPDBContext;

                int change_user_id;

                if (this.change_user_id == null)
                {
                    change_user_id = -1;
                }
                else
                {
                    change_user_id = Convert.ToInt32(this.change_user_id);
                }

                application_security current_application_security = application_security.GetApplicationSecurity(change_user_id);


                if (current_application_security != null)
                {
                    return current_application_security.user_name;
                }
                else
                {
                    return "";
                }

            }

        }

        public static void CancelScannedDate(string production_coil_no, int change_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            warehoused_load_dtl current_coil = db.warehoused_load_dtl.Where(x => x.production_coil_no == production_coil_no).FirstOrDefault();

            current_coil.coil_scanned_dt = null;
            current_coil.change_user_id = change_user_id;
            current_coil.change_datetime = DateTime.Now;
            db.SaveChanges();

        }
        public static List<warehoused_load_dtl> GetLoadDtl(int load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.warehoused_load_dtl.Where(x => x.load_id == load_id).ToList();
        }

        public static warehoused_load_dtl GetLoadDtl(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.warehoused_load_dtl.Where(x => x.production_coil_no == production_coil_no).OrderByDescending(y => y.load_id).FirstOrDefault();
        }
        public static List<CoilsInLoad> GetLoadDtlCoils(int load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var list = db.warehoused_load_dtl.Where(x => x.load_id == load_id);

            List<CoilsInLoad> coilsList = new List<CoilsInLoad>();

            foreach (var load_dtl in list)
            {
                CoilsInLoad coil = new CoilsInLoad();
                coil.production_coil_no = load_dtl.production_coil_no;
                coil.verified = Convert.ToDateTime(load_dtl.coil_scanned_dt);
                coil.coilWeight = Convert.ToInt32(load_dtl.coil_weight);
                coil.coilLocation = load_dtl.coil_yard_location.Trim();

                coilsList.Add(coil);
            }

            return coilsList;

        }

        public string coil_yard_location
        {
            get
            {

                sdipdbEntities db = ContextHelper.SDIPDBContext;

                coil_yard_locations current_location = coil_yard_locations.GetCoilYardLocation(this.production_coil_no);

                if (current_location == null)
                {
                    ref_sys_param rsp = ref_sys_param.GetRefSysParam();

                    if (rsp.location == "C")
                    {
                        return "";
                    }
                    else
                    {

                        var load_d = from ld in db.load_dtl_bay
                                     where ld.production_coil_no == this.production_coil_no
                                     select ld;

                        if (load_d != null)
                        {
                            IEnumerable<string> col = from l in load_d select l.coil_yard_column;
                            IEnumerable<string> row = from l in load_d select l.coil_yard_row;

                            return col.FirstOrDefault() + "-" + row.FirstOrDefault();
                        }
                        else
                        {
                            return "";
                        }
                    }

                }
                else
                {
                    return current_location.column + "-" + current_location.row;
                }

            }

        }

        public static void UpdateCoilScannedDt(string production_coil_no, DateTime? verified_date, int change_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            try
            { 
                warehoused_load_dtl scanned_coil = db.warehoused_load_dtl.Where(x => x.production_coil_no == production_coil_no).OrderByDescending(y => y.load_id).FirstOrDefault();
                scanned_coil.coil_scanned_dt = verified_date;
                scanned_coil.change_datetime = verified_date;
                scanned_coil.change_user_id = change_user_id;

                db.SaveChanges();
            }
            catch (Exception ex)
            {
                string err = ex.ToString();
            }
        }
    }
}