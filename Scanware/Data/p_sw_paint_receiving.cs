using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Scanware.Data
{
    public partial class sw_paint_receiving
    {

        public static void InsertPaintReceiving(string barcode_number, int add_user_id, bool scanner_used, int location_cd)
        {

            sw_paint_receiving ins_rec = new sw_paint_receiving()
            {
                barcode_number = barcode_number,
                add_date_time = DateTime.Now,
                add_user_id = add_user_id,
                scanner_used = scanner_used,
                location_cd = location_cd
            };

            sdipdbEntities db = ContextHelper.SDIPDBContext;
            db.sw_paint_receiving.Add(ins_rec);
            db.SaveChanges();

        }

        public static sw_paint_receiving GetPaintReceiveing(string barcode_number)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            sw_paint_receiving return_pr = new sw_paint_receiving();

            return_pr = db.sw_paint_receiving.SingleOrDefault(x => x.barcode_number == barcode_number);

            return return_pr;

        }

        public static void UpdateLocation(string barcode_number, int location_id, int add_user_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            sw_paint_receiving pr = db.sw_paint_receiving.SingleOrDefault(x => x.barcode_number == barcode_number);
            pr.location_cd = location_id;
            pr.add_user_id = add_user_id;
            pr.add_date_time = DateTime.Now;

            db.SaveChanges();

        }

    }

    public partial class paint_barcode
    {
        public string paint_code { get; set; }
        public string batch_no { get; set; }

        public string drum_no { get; set; }
        public ushort gallons { get; set; }
        public DateTime expiration_date { get; set; }
        public string po_no { get; set; }
        public string bol { get; set; }

        public ushort batch_gallons { get; set; }

        public static string ParseBarcode(string barcode_number, ref paint_barcode new_barcode, int batch_total = -1)
        {
            string error = "";
            string error2 = "NO";
            string[] parsed_values;

            try
            {
                parsed_values = barcode_number.Split(',');
                int barcode_length = parsed_values.Length;

                if (barcode_length < 7 && barcode_length > 8)
                {
                    return "Barcode format Error. Please contact Level 3!";
                }
                string gallons_int_string = string.Join(string.Empty, Regex.Matches(parsed_values[3], @"\d+").OfType<Match>().Select(m => m.Value)); //returns every number in a string

                new_barcode.paint_code = parsed_values[0].Trim();
                new_barcode.batch_no = parsed_values[1].Trim();
                new_barcode.drum_no = parsed_values[2].Trim();
                new_barcode.gallons = Convert.ToUInt16(gallons_int_string);
                new_barcode.expiration_date = Convert.ToDateTime(parsed_values[4]);
                new_barcode.po_no = parsed_values[5].Trim();
                new_barcode.bol = parsed_values[6].Trim();

                //Remove 'PL' from PO value
                if (new_barcode.po_no.Substring(new_barcode.po_no.Length - 2) == "PL")
                {
                    new_barcode.po_no = new_barcode.po_no.Substring(0, (new_barcode.po_no.Length - 2));
                }

                //Vendor Did not supply the Batch Total as part of the Barcode
                if (parsed_values.Length == 7)
                {
                    //No batch_total
                    if(batch_total != -1)
                    {
                        new_barcode.batch_gallons = Convert.ToUInt16(batch_total); 
                    }
                }
                else
                {
                    new_barcode.batch_gallons = Convert.ToUInt16(parsed_values[7]);
                }

                /* try
                 { 
                     new_barcode.drum_no = Convert.ToByte(parsed_values[2]);
                 }
                 catch (Exception ex2)
                 {
                     error2 = "Drum number Exception! ";
                     throw ex2;
                 }
                 */
            }
            catch (Exception ex)
            {
                if (error2 == "NO")
                {
                    if (ex.InnerException != null)
                    {
                        error = "Error" + ex.InnerException.InnerException.Message.ToString();
                    }
                    else
                        error = "Error getting Container Info!";

                }
                else
                {
                    error = error2;
                }
                return error;
            }

            return error;
        }
    }
}

