using Scanware.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace Scanware.App_Objects
{
    public class Utils
    {
        public static void AddAuditRecord(string production_coil_no, string action_description, string comment)
        {
            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            int next_action_sequence_number = 1;

            all_produced_coils current_all_produced_coil = all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(production_coil_no);

            next_action_sequence_number = coil_audit_trail.GetNextActionSequenceNumber(current_all_produced_coil.production_coil_no);

            coil_audit_trail.AddAuditRecord(action_description, current_all_produced_coil.production_coil_no, Convert.ToInt32(current_all_produced_coil.cons_coil_no), comment, current_application_security.user_id, next_action_sequence_number);
        }

        public static void AddAuditRecord(string production_coil_no, string action_description, string comment, string is_scanner_used)
        {
            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            int next_action_sequence_number = 1;

            all_produced_coils current_all_produced_coil = all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(production_coil_no);

            next_action_sequence_number = coil_audit_trail.GetNextActionSequenceNumber(current_all_produced_coil.production_coil_no);
            string coil_status = coil.GetCoilStatusByProductionCoilNumber(production_coil_no);

            coil_audit_trail.AddAuditRecord(action_description, current_all_produced_coil.production_coil_no, Convert.ToInt32(current_all_produced_coil.cons_coil_no), comment, current_application_security.user_id, next_action_sequence_number, is_scanner_used, coil_status);

        }

        public static void AddAuditRecordWarehouse(string production_coil_no, string action_description, string comment, string is_scanner_used)
        {
            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            int next_action_sequence_number = 1;

            next_action_sequence_number = coil_audit_trail.GetNextActionSequenceNumber(production_coil_no);
            string coil_status = warehoused_inventory.GetCoilStatusByProductionCoilNumber(production_coil_no);

            coil_audit_trail.AddAuditRecord(action_description, production_coil_no, 0, comment, current_application_security.user_id, next_action_sequence_number, is_scanner_used, coil_status);

        }

        public static void AddAuditRecordWarehouse(string production_coil_no, string action_description, string comment)
        {
            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            int next_action_sequence_number = 1;

            next_action_sequence_number = coil_audit_trail.GetNextActionSequenceNumber(production_coil_no);

            coil_audit_trail.AddAuditRecord(action_description, production_coil_no, 0, comment, current_application_security.user_id, next_action_sequence_number);
        }
        public static void AddAuditRecord2(string production_coil_no, string action_description, string comment, string is_scanner_used, string coil_col, string coil_row)
        {
            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            int next_action_sequence_number = 1;

            all_produced_coils current_all_produced_coil = all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(production_coil_no);

            next_action_sequence_number = coil_audit_trail.GetNextActionSequenceNumber(current_all_produced_coil.production_coil_no);
            string coil_status = coil.GetCoilStatusByProductionCoilNumber(production_coil_no);

            coil_audit_trail.AddAuditRecord2(action_description, current_all_produced_coil.production_coil_no, Convert.ToInt32(current_all_produced_coil.cons_coil_no), comment, current_application_security.user_id, next_action_sequence_number,
                                             is_scanner_used, coil_status, coil_col, coil_row);
        }

        public static void AddAuditRecord_Kiosk(string production_coil_no, string action_description, string comment)
        {
            all_produced_coils current_all_produced_coil = all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(production_coil_no);
            int next_action_sequence_number = coil_audit_trail.GetNextActionSequenceNumber(current_all_produced_coil.production_coil_no);
            coil_audit_trail.AddAuditRecord(action_description, current_all_produced_coil.production_coil_no, Convert.ToInt32(current_all_produced_coil.cons_coil_no), comment, 0, next_action_sequence_number);
        }

        public static void AddCoilDefect(string production_coil_no, string defect_cd, string unit_detected, string unit_responsible, string defect_comment, string inv_or_hold,
            int? order_no, int? line_item_no, int? cons_coil_no, string order_inv, string reason_for_last_status_change)
        {
            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            int seq_no = usp.spc_up_seq_no("next_defect_seq_no");

            coil_defect.AddManualDefect(production_coil_no, current_application_security.user_id, defect_cd, unit_detected, unit_responsible, defect_comment, inv_or_hold, order_no, line_item_no,
                cons_coil_no, order_inv, reason_for_last_status_change, "Scanware", seq_no);
        }

        public static int ChangeCoilStatus(string production_coil_no, string coil_type, string new_type, string coil_status, string new_coil_status)
        {
            application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

            return usp.usp_change_coil_status(production_coil_no, coil_type, new_type, coil_status, new_coil_status, current_application_security.user_id);

        }

        /// <summary>
        /// Class for sending ZPL string over a network.
        /// </summary>
        /// <param name="current_printer">Printer Information</param>
        /// <param name="template">ZPL to send to printer</param>
        /// <param name="port">Optional Port</param>
        public static void TCPTemplateToZebra(printer current_printer, string template, int port = 9100)
        {
            try
            {
                // Open connection
                System.Net.Sockets.TcpClient client = new System.Net.Sockets.TcpClient();
                client.Connect(current_printer.path, port);

                // Write ZPL String to connection
                System.IO.StreamWriter writer =
                    new System.IO.StreamWriter(client.GetStream());
                writer.Write(template);
                writer.Flush();

                // Close Connection
                writer.Close();
                client.Close();
            }
            catch (Exception ex)
            {
                // Catch Exception
            }
        }

        public static void FTPTemplateToZebra(printer current_printer, string template, string file_name)
        {
            //create zebra file
            string path = "c:\\temp\\" + file_name + ".txt";

            //remove file if it exists
            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            if (!System.IO.File.Exists(path))
            {
                // Create a file to write to.
                using (StreamWriter sw = System.IO.File.CreateText(path))
                {
                    //sw.WriteLine("^XA");
                    //sw.WriteLine("^DFR:ZPL2A.GRF^FS");
                    //sw.WriteLine("^FO250,600^BY4^B3R,,300^FN5^FS (coil id barcode)");
                    //sw.WriteLine("^XZ");

                    //sw.WriteLine("^XA");
                    //sw.WriteLine("^XFR:ZPL2A.GRF");
                    //sw.WriteLine("^FN5^FD" + production_coil_no + "^FS");
                    //sw.WriteLine("^XZ");
                    sw.WriteLine(template);
                }

                string ftp_path = "c:\\temp\\ScanwareBarcodePrint.ftp";

                //remove file if it exists
                if (System.IO.File.Exists(ftp_path))
                {
                    System.IO.File.Delete(ftp_path);
                }

                //if ftp commands do not exist then create new ftp file in temp directory
                if (!System.IO.File.Exists(ftp_path))
                {
                    using (StreamWriter sw = System.IO.File.CreateText(ftp_path))
                    {
                        sw.WriteLine("");
                        sw.WriteLine("put " + path);
                        sw.WriteLine("quit");

                    }
                }

                //ZEBRA FTP IS VERY BASIC, DOES NOT SUPPORT .NET FTP FUNCTIONS, HAVE TO USE COMMAND LINE FTP WITH NO USERNAME/PASSWORD TO SEND FILE
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = @"/C ftp -s:" + ftp_path + " " + current_printer.path;
                p.StartInfo.RedirectStandardOutput = false;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = false;
                p.Start();
            }

            else
            {
                throw new Exception("An error has occured, refresh the page and try again.");
            }
        }

        public static void PrintPickList(printer network_printer, int load_id, string seq_no)
        {
            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            string report_url = rsp.ssrs_path_local + "/Pages/ReportViewer.aspx?%2f" + rsp.ssrs_folder + 
                                "%2fPick_List_8063&load_id=" + load_id.ToString() + 
                                "&SequenceNo=" + seq_no +
                                "&rs:Format=PDF";

            printer_queue.AddToPrintQueue(GetSSRSBytes(report_url), network_printer.pk, "pick_list", load_id.ToString(), 2, 0);
        }

        public static void PrintReathorization(printer network_printer, string reauth_id)
        {
            ref_sys_param rsp = ref_sys_param.GetRefSysParam();
            string report_url = rsp.ssrs_path_local + "/Pages/ReportViewer.aspx?%2f" + rsp.ssrs_folder + "%2fsw_coil_reauthorization_5288&ra_id=" + reauth_id + "&rs:Format=PDF";

            printer_queue.AddToPrintQueue(GetSSRSBytes(report_url), network_printer.pk, "coil_reauthorization", reauth_id, 1, 0);
        }

        public static void PrintBOL(printer network_printer, int customer_id, int load_id, int copies = 2, bool truckMode = true)
        {
            var report_url = BOLUrl(customer_id, load_id, truckMode);

            try
            {
                printer_queue.AddToPrintQueue(GetSSRSBytes(report_url), network_printer.pk, "bill_of_lading", load_id.ToString(), copies, 0);
            }
            catch (Exception)
            {
                //log ex
                throw;
            }
        }

        public static void PrintProformaUSMCADocs(printer network_printer, int customer_id, int load_id, int copies = 1)
        {
            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            string report_url = rsp.ssrs_path_local + "/Pages/ReportViewer.aspx?%2f" + rsp.ssrs_folder;

            var proforma_url = report_url + "%2fweb_proforma_invoice_1511" + "&load_id=" + load_id.ToString() + "&rs:Format=PDF";
            var usmca_url = report_url + "%2fd_usmca_cert_origin_1693" + "&customer_id=" + customer_id.ToString() + "&load_id=" + load_id.ToString() + "&rs:Format=PDF";

            try
            {
                printer_queue.AddToPrintQueue(GetSSRSBytes(proforma_url), network_printer.pk, "proforma", load_id.ToString(), copies, 0);
                printer_queue.AddToPrintQueue(GetSSRSBytes(usmca_url), network_printer.pk, "usmca", load_id.ToString(), copies, 0);
            }
            catch (Exception)
            {
                //log ex
                throw;
            }
        }

        public static string BOLUrl(int customer_id, int load_id, bool truckMode = true)
        {
            ref_sys_param rsp = ref_sys_param.GetRefSysParam();

            string report_url = rsp.ssrs_path_local + "/Pages/ReportViewer.aspx?%2f" + rsp.ssrs_folder;

            if (truckMode)
                report_url += "%2fbill_of_lading_5086";
            else
                report_url += "%2fbill_of_lading_rail_5201";

            report_url += "&customer_id=" + customer_id.ToString() + "&LoadID=" + load_id.ToString() + "&rs:Format=PDF";

            return report_url;
        }

        private static byte[] GetSSRSBytes(string ssrs_url)
        {
            MemoryStream stream;
            using (WebClient webClient = new WebClient())
            {
                webClient.UseDefaultCredentials = true;
                stream = new MemoryStream(webClient.DownloadData(ssrs_url));
            }

            return stream.ToArray();
        }
    }
}
