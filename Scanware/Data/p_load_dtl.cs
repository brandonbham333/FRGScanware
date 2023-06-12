using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class load_dtl
    {
        public string change_user_name { 
        get{
            
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


        public string packaging_simplified_instructions
        {

            get
            {
                sdipdbEntities db = ContextHelper.SDIPDBContext;

                var query = from fls in db.finish_line_setup
                            join pack in db.packaging_type on fls.packaging_type_cd equals pack.packaging_type_cd
                            where fls.order_no == this.order_no && fls.line_item_no == this.line_item_no
                            select pack;

                packaging_type pt = query.FirstOrDefault();

                if (pt != null)
                {
                    return pt.simplified_instructions;
                }
                else
                {
                    return "";
                }
            }

        }

        public string packaging_description
        {

            get
            {
                sdipdbEntities db = ContextHelper.SDIPDBContext;

                var query = from fls in db.finish_line_setup
                            join pack in db.packaging_type on fls.packaging_type_cd equals pack.packaging_type_cd
                            where fls.order_no == this.order_no && fls.line_item_no == this.line_item_no
                            select pack;

                packaging_type pt = query.FirstOrDefault();

                if (pt != null)
                {
                    return pt.description;
                }
                else
                {
                    return "";
                }

            }

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

                            return col.FirstOrDefault() +"-" + row.FirstOrDefault();
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

        public string tag_no
        {
            get
            {

                sdipdbEntities db = ContextHelper.SDIPDBContext;

                all_produced_coils current_apc = all_produced_coils.GetAllProducedCoil(this.production_coil_no);

                if (current_apc == null)
                {
                    return "";
                }
                else
                {
                    return current_apc.tag_no;
                }

            }

        }

        public int additional_weight
        {
            get
            {
                try
                {
                    sdipdbEntities db = ContextHelper.SDIPDBContext;

                    var query = from fls in db.finish_line_setup
                                join pack in db.packaging_type on fls.packaging_type_cd equals pack.packaging_type_cd
                                where fls.order_no == this.order_no && fls.line_item_no == this.line_item_no
                                select pack;

                    packaging_type pt = query.FirstOrDefault();

                    return pt.additional_weight ?? 0;
                }
                catch
                {
                    return 0;
                }
                

            }

        }

        public static List<load_dtl> GetLoadDtl(int load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.load_dtl.Where(x => x.load_id == load_id).ToList();
        }

        public static List<CoilsInLoad> GetLoadDtlCoils(int load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var list = db.load_dtl.Where(x => x.load_id == load_id);

            List<CoilsInLoad> coilsList = new List<CoilsInLoad>();

            foreach(var load_dtl in list)
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

        public static List<int> GetCustomersIDsInLoad(int load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            var custList = new List<int>();

            var details = db.load_dtl.Where(x => x.load_id == load_id);

            foreach (var item in details)
            {
                var coil = db.coils.FirstOrDefault(c => c.production_coil_no == item.production_coil_no);

                var coli = db.customer_order_line_item.FirstOrDefault(x =>
                    x.order_no == coil.order_no && x.line_item_no == coil.line_item_no);

                var coilCustomer = db.customers.FirstOrDefault(cust => cust.customer_id == coli.customer_id);

                if(!custList.Contains(coilCustomer.customer_id))
                    custList.Add(coilCustomer.customer_id);
            }

            return custList;
        }

        public static List<CoilsInLoad> GetLoadDtlCoilsSubs(int load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var loads = db.shipment_load.Where(s => s.master_load_id == load_id).Select(s => s.load_id).ToList();

            var list = db.load_dtl.Where(x => loads.Contains(x.load_id)).ToList();

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
        public static List<load_dtl> GetLoadDtlSub(List<int> loads)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var list = from ld in db.load_dtl
                       where loads.Contains(ld.load_id)
                       select ld;

            return list.ToList();

        }

        public static List<load_dtl> GetLoadDtlForAllSubLoads(string char_load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var query = from sl in db.shipment_load
                        join ld in db.load_dtl on sl.load_id equals ld.load_id
                        where sl.char_load_id.Contains(char_load_id.Substring(1, char_load_id.IndexOf("-")))
                        select ld;

            return query.ToList();
            
        }

        public static load_dtl GetLoadDtl(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.load_dtl.Where(x => x.production_coil_no == production_coil_no).OrderByDescending(y=>y.load_id).FirstOrDefault();
        }

        public static void UpdateCoilScannedDt(string production_coil_no, DateTime? verified_date, int change_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            load_dtl scanned_coil =  db.load_dtl.Where(x => x.production_coil_no == production_coil_no).OrderByDescending(y=>y.load_id).FirstOrDefault();
            scanned_coil.coil_scanned_dt = verified_date;
            scanned_coil.change_user_id = change_user_id;

            db.SaveChanges();
        }
        public static void UpdateCoilScannedDt_mx(string production_coil_no, DateTime? verified_date, int change_user_id, int load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            load_dtl scanned_coil = db.load_dtl.Where(x => x.production_coil_no == production_coil_no && x.load_id == load_id).OrderByDescending(y => y.load_id).FirstOrDefault();
            scanned_coil.coil_scanned_dt = verified_date;
            scanned_coil.change_user_id = change_user_id;

            db.SaveChanges();
        }

        public static void CancelScannedDate(string production_coil_no, int change_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            load_dtl current_coil = db.load_dtl.Where(x => x.production_coil_no == production_coil_no).FirstOrDefault();

            current_coil.coil_scanned_dt = null;
            current_coil.change_user_id = change_user_id;
            current_coil.change_datetime = DateTime.Now;
            db.SaveChanges();

        }


    }
}