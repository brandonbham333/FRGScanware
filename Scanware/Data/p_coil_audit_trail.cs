using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class coil_audit_trail
    {
        public static List<vw_sw_coil_history> GetCoilHistoryByProductionCoilNumber(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            List<vw_sw_coil_history> coil_audit_records = new List<vw_sw_coil_history>();

            var audit_records = from at in db.vw_sw_coil_history
                                where at.production_coil_no == production_coil_no
                                select at;

            foreach (vw_sw_coil_history audit_record in audit_records)
            {
                coil_audit_records.Add(audit_record);
            }

            return coil_audit_records;
        }

        public static List<vw_sw_coil_history> GetCoilHistoryByCons(int cons_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            List<vw_sw_coil_history> coil_audit_records = new List<vw_sw_coil_history>();

            var audit_records = from at in db.vw_sw_coil_history
                                where at.cons_coil_no == cons_coil_no
                                select at;

            foreach (vw_sw_coil_history audit_record in audit_records.OrderBy(x => x.change_datetime))
            {
                coil_audit_records.Add(audit_record);
            }

            return coil_audit_records;
        }
        public static void AddAuditRecord(string action_description, string production_coil_no, int cons_coil_number, string comment, int change_user_id, int action_sequence_number)
        {

            coil_audit_trail new_comment = new coil_audit_trail()
            {
                production_coil_no=production_coil_no, 
                cons_coil_no=cons_coil_number,
                comment=comment,
                action_description = action_description,
                change_datetime=DateTime.Now,
                change_user_id=change_user_id,
                action_seq_no = action_sequence_number
                    
            };

            sdipdbEntities db = ContextHelper.SDIPDBContext;
            db.coil_audit_trail.Add(new_comment);
            db.SaveChanges();

        }

        public static void AddAuditRecord(string action_description, string production_coil_no, int cons_coil_number, string comment, int change_user_id, int action_sequence_number, string is_scanner_used)
        {

            coil_audit_trail new_comment = new coil_audit_trail()
            {
                production_coil_no = production_coil_no,
                cons_coil_no = cons_coil_number,
                comment = comment,
                action_description = action_description,
                change_datetime = DateTime.Now,
                change_user_id = change_user_id,
                action_seq_no = action_sequence_number,
                scanner_used = is_scanner_used

            };

            sdipdbEntities db = ContextHelper.SDIPDBContext;
            db.coil_audit_trail.Add(new_comment);
            db.SaveChanges();

        }

        public static void AddAuditRecord(string action_description, string production_coil_no, int cons_coil_number, string comment, int change_user_id, int action_sequence_number, string is_scanner_used, string coil_status)
        {

            coil_audit_trail new_comment = new coil_audit_trail()
            {
                production_coil_no = production_coil_no,
                cons_coil_no = cons_coil_number,
                comment = comment,
                action_description = action_description,
                change_datetime = DateTime.Now,
                change_user_id = change_user_id,
                action_seq_no = action_sequence_number,
                scanner_used = is_scanner_used,
                status = coil_status,
               // scanned_datetime = DateTime.Now

            };

            sdipdbEntities db = ContextHelper.SDIPDBContext;
            db.coil_audit_trail.Add(new_comment);
            db.SaveChanges();
        }

        public static void AddAuditRecord2(string action_description, string production_coil_no, int cons_coil_number, string comment, int change_user_id, int action_sequence_number, string is_scanner_used, string coil_status, string col, string row)
        {

            coil_audit_trail new_comment = new coil_audit_trail()
            {
                production_coil_no = production_coil_no,
                cons_coil_no = cons_coil_number,
                comment = comment,
                action_description = action_description,
                change_datetime = DateTime.Now,
                change_user_id = change_user_id,
                action_seq_no = action_sequence_number,
                scanner_used = is_scanner_used,
                status = coil_status,
                coil_yard_column = col,
                coil_yard_row = row

            };

            sdipdbEntities db = ContextHelper.SDIPDBContext;
            db.coil_audit_trail.Add(new_comment);
            db.SaveChanges();
        }
        public static coil_audit_trail GetLatestRecordByActionDescription(string production_coil_no, string action_description)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;
            return db.coil_audit_trail.Where(x => x.production_coil_no == production_coil_no && x.action_description == action_description).OrderByDescending(y => y.change_datetime).FirstOrDefault();


        }

        public static int GetNextActionSequenceNumber(string production_coil_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            int NextActionSequenceNumber = 0;

            var query = (from at in db.coil_audit_trail
                        where at.production_coil_no == production_coil_no
                        orderby at.action_seq_no descending
                        select at.action_seq_no).Take(1);

            
            foreach (int i in query){

                NextActionSequenceNumber = i;

            }
            
            return NextActionSequenceNumber + 1;


        }

    }
}