using Scanware.Data;
using Scanware.Models;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Scanware.Controllers
{
    public class PaintController : Controller
    {
        public ActionResult PaintContainerDetails(string barcode_string, string Error, string Message)
        {
            //string message;

            PaintModel viewModel = new PaintModel();
//            viewModel.paint_locations = paint_location.GetActivePaintLocations();

            paint_barcode new_barcode = new paint_barcode();

            if (barcode_string != null)
            {
                //Parse values form barcode string
                string error = paint_barcode.ParseBarcode(barcode_string, ref new_barcode);

                if (error.Length > 0)
                {
                    viewModel.Error = error;

                    return View(viewModel);
                }
            }
            else
            {
                //viewModel.Error = "Barcode is missing!";
                return View(viewModel);
            }

            viewModel.current_paint_inventory = paint_inventory.GetPaintInventoryByBarcodeButler(new_barcode);

            //if not processed before than error 
            if (viewModel.current_paint_inventory.container_id == 0)
            {
                viewModel.Error = "Container wasn't found In L3 Paint Inventory.";
                return View(viewModel);

            }
            string inv_status = viewModel.current_paint_inventory.inventory_status;

            switch (inv_status)
            {
                case "PP":
                    viewModel.current_inventory_status = "Paint Problem";
                    break;
                case "EE":
                    viewModel.current_inventory_status = "Entry Error";
                    break;
                case "DA":
                    viewModel.current_inventory_status = "Disposed of Age";
                    break;
                case "DC":
                    viewModel.current_inventory_status = "Disposed of Contamination";
                    break;
                case "RT":
                    viewModel.current_inventory_status = "Return";
                    break;
                case "NR":
                    viewModel.current_inventory_status = "Needs to be Returned";
                    break;
                case "RE":
                    viewModel.current_inventory_status = "Retint";
                    break;
                case "XX":
                    viewModel.current_inventory_status = "Inventory Staging";
                    break;
                default:
                    viewModel.current_inventory_status = "";
                    break;
            }

            viewModel.current_paint_location = paint_location.GetPaintLocation(viewModel.current_paint_inventory.location_cd);
            
            if(viewModel.current_paint_location == null)
            {
                viewModel.current_paint_location = new paint_location();
            }
            return View(viewModel);
        }

        // GET: /Paint/
        public ActionResult ReceivePaint(string barcode_string, string Error, string Message, int? batch_gallons)
        {
            string message;

            PaintModel viewModel = new PaintModel();


                List<int> gallons = new List<int>();

                for (int to_add = 50; to_add < 3750; to_add += 50)
                {
                    gallons.Add(to_add);
                }

                if (gallons != null)
                {
                    viewModel.paint_batch_range = gallons;
                }

            if (batch_gallons != null)
            {
                viewModel.paint_batch_total = Convert.ToInt32(batch_gallons);
            }

            if (barcode_string != null)
            {

               paint_barcode new_barcode = new paint_barcode();

                //Parse values form barcode string
                string error = paint_barcode.ParseBarcode(barcode_string, ref new_barcode, viewModel.paint_batch_total);

                if (error.Length > 0)
                {
                    viewModel.Error = error;

                    return View(viewModel);
                }
                //Check if already received
                message = paint_receiver_header.IsDuplicate(new_barcode);

                if (message.Length > 1)
                {
                    viewModel.Error = message;
                    return View(viewModel);
                }

                if(new_barcode.batch_gallons < 1)
                {
                    viewModel.Error = "Missing Batch total!";
                    return View(viewModel);
                }

                //Attemp to Receive Containers
                message = usp.usp_paint_receiving(new_barcode);

                if (message.Contains("Successfully"))
                {
                    viewModel.Message = message;
                }
                else
                {
                    viewModel.Error = message;
                }

            }
            return View(viewModel);
        }

        public ActionResult AddPaint(int? location_cd, string Error, string Message)
        {
            PaintModel viewModel = new PaintModel();

            viewModel.paint_locations = paint_location.GetActivePaintLocations();
            viewModel.Error = Error;
            viewModel.Message = Message;

            if (location_cd.HasValue && location_cd != 0)
            {

                viewModel.current_paint_location = paint_location.GetPaintLocation(Convert.ToInt32(location_cd));

            }
            else
            {
                viewModel.current_paint_location = new paint_location();
            }
            

            return View(viewModel);
        }

        public ActionResult InsertPaint(int location_cd, string barcode_number, int ScannerUsed)
        {
            PaintModel viewModel = new PaintModel();
            //viewModel.paint_locations = paint_location.GetActivePaintLocations();

            viewModel.current_paint_location = paint_location.GetPaintLocation(location_cd);

            if (viewModel.current_paint_location == null)
            {
                viewModel.Error = "No location was selected";
                return RedirectToAction("AddPaint", "Paint", new { Error = viewModel.Error, location_cd = location_cd });
            }

            if (barcode_number == "")
            {
                viewModel.Error = "No barcode entered";
                return RedirectToAction("AddPaint", "Paint", new { Error = viewModel.Error, location_cd = location_cd });
            }

            //see if it was processed before
            viewModel.current_paint_receiving = sw_paint_receiving.GetPaintReceiveing(barcode_number);

            //if not processed add paint 
            if (viewModel.current_paint_receiving == null)
            {

                try
                {
                    application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                    sw_paint_receiving.InsertPaintReceiving(barcode_number, current_application_security.user_id, Convert.ToBoolean(ScannerUsed), location_cd);
                    viewModel.Message = barcode_number + " was processed successfully";

                    return RedirectToAction("AddPaint", "Paint", new { Message = viewModel.Message, location_cd = location_cd });
                }
                catch(Exception ex)
                {
                    viewModel.Error = ex.InnerException.InnerException.Message.ToString();
                    return RedirectToAction("AddPaint", "Paint", new { Error = viewModel.Error, location_cd = location_cd });
                }


            }
            //has been processed before
            else
            {
                viewModel.Error = viewModel.current_paint_receiving.barcode_number + " has been processed on " + viewModel.current_paint_receiving.add_date_time;
                return RedirectToAction("AddPaint", "Paint", new { Error = viewModel.Error, location_cd = location_cd });
            }
          
        }

        public ActionResult MovePaint(int? location_cd, string Error, string Message)
        {
            PaintModel viewModel = new PaintModel();

            viewModel.paint_locations = paint_location.GetActivePaintLocations();

            viewModel.Error = Error;
            viewModel.Message = Message;

            if (location_cd.HasValue && location_cd != 0)
            {

                viewModel.current_paint_location = paint_location.GetPaintLocation(Convert.ToInt32(location_cd));

            }
            else
            {
                viewModel.current_paint_location = new paint_location();
            }

            return View(viewModel);
        }

        public ActionResult MovePaintButler(int? location_cd, string Error, string Message)
        {
            PaintModel viewModel = new PaintModel();

            viewModel.paint_locations = paint_location.GetActivePaintLocationsButler();

            viewModel.Error = Error;
            viewModel.Message = Message;

            if (location_cd.HasValue && location_cd != 0)
            {

                viewModel.current_paint_location = paint_location.GetPaintLocation(Convert.ToInt32(location_cd));

            }
            else
            {
                viewModel.current_paint_location = new paint_location();
            }

            return View(viewModel);
        }
        public ActionResult MovePaintSubmit(short location_cd, string barcode_number, int ScannerUsed, string Gallons)
        {
            PaintModel viewModel = new PaintModel();

            viewModel.searched_location_cd = location_cd;

            //viewModel.paint_locations = paint_location.GetActivePaintLocations();

            viewModel.current_paint_location = paint_location.GetPaintLocation(location_cd);

            if (viewModel.current_paint_location == null)
            {
                viewModel.Error = "No location was selected";
                return RedirectToAction("MovePaint", "Paint", new { Error = viewModel.Error, location_cd = location_cd });
            }

            //see if it was processed before
            viewModel.current_paint_inventory = paint_inventory.GetPaintInventoryByBarcode(barcode_number);

            //if not processed before than error 
            if (viewModel.current_paint_inventory == null)
            {
                viewModel.Error = barcode_number + " was not found so the location was not updated";
                return RedirectToAction("MovePaint", "Paint", new { Error = viewModel.Error, location_cd = location_cd });

            }
            //it exists, update location
            else
            {

                try
                {
                    application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                    
                    //create and insert negative quantity
                    paint_inventory negative_quantity = new paint_inventory();

                    negative_quantity.container_id=viewModel.current_paint_inventory.container_id;
                    negative_quantity.paint_cd=viewModel.current_paint_inventory.paint_cd;
                    negative_quantity.location_cd=viewModel.current_paint_inventory.location_cd;
                    negative_quantity.expiration_date=viewModel.current_paint_inventory.expiration_date;
                    negative_quantity.cost=viewModel.current_paint_inventory.cost;
                    negative_quantity.number_of_gallons=viewModel.current_paint_inventory.number_of_gallons * -1; //off set current gallons
                    negative_quantity.change_user_id=viewModel.current_paint_inventory.change_user_id;
                    negative_quantity.change_datetime = DateTime.Now;
                    negative_quantity.received_date=viewModel.current_paint_inventory.received_date;
                    negative_quantity.batch_no=viewModel.current_paint_inventory.batch_no;
                    negative_quantity.container_type=viewModel.current_paint_inventory.container_type;
                    negative_quantity.purchase_order_no=viewModel.current_paint_inventory.purchase_order_no;
                    negative_quantity.@new = viewModel.current_paint_inventory.@new;
                    negative_quantity.po_line_item_no=viewModel.current_paint_inventory.po_line_item_no;
                    negative_quantity.inventory_status=viewModel.current_paint_inventory.inventory_status;
                    negative_quantity.vendor_id=viewModel.current_paint_inventory.vendor_id;
                    negative_quantity.seq_no=viewModel.current_paint_inventory.seq_no + 1;
                    negative_quantity.container_status=viewModel.current_paint_inventory.container_status;
                    negative_quantity.vendor_container_id=viewModel.current_paint_inventory.vendor_container_id;
                    negative_quantity.reviewed=viewModel.current_paint_inventory.reviewed;
                    negative_quantity.receiver_no=viewModel.current_paint_inventory.receiver_no;
                    
                    if (current_application_security.user_name.Length > 10)
                    {
                        negative_quantity.initials = current_application_security.user_name.Substring(1, 10);
                    }
                    else
                    {
                        negative_quantity.initials = current_application_security.user_name;
                    }
                    
                    //insert with new gallons and location
                    paint_inventory new_location_and_gallon = new paint_inventory();

                    new_location_and_gallon.container_id = viewModel.current_paint_inventory.container_id;
                    new_location_and_gallon.paint_cd = viewModel.current_paint_inventory.paint_cd;
                    new_location_and_gallon.expiration_date = viewModel.current_paint_inventory.expiration_date;
                    new_location_and_gallon.cost = viewModel.current_paint_inventory.cost;
                    new_location_and_gallon.number_of_gallons = viewModel.current_paint_inventory.number_of_gallons;
                    new_location_and_gallon.change_user_id = viewModel.current_paint_inventory.change_user_id;
                    new_location_and_gallon.change_datetime = DateTime.Now;
                    new_location_and_gallon.received_date = viewModel.current_paint_inventory.received_date;
                    new_location_and_gallon.batch_no = viewModel.current_paint_inventory.batch_no;
                    new_location_and_gallon.container_type = viewModel.current_paint_inventory.container_type;
                    new_location_and_gallon.purchase_order_no = viewModel.current_paint_inventory.purchase_order_no;
                    new_location_and_gallon.@new = viewModel.current_paint_inventory.@new;
                    new_location_and_gallon.po_line_item_no = viewModel.current_paint_inventory.po_line_item_no;
                    new_location_and_gallon.inventory_status = viewModel.current_paint_inventory.inventory_status;
                    new_location_and_gallon.vendor_id = viewModel.current_paint_inventory.vendor_id;
                    new_location_and_gallon.seq_no = viewModel.current_paint_inventory.seq_no + 2;
                    new_location_and_gallon.container_status = viewModel.current_paint_inventory.container_status;
                    new_location_and_gallon.vendor_container_id = viewModel.current_paint_inventory.vendor_container_id;
                    new_location_and_gallon.reviewed = viewModel.current_paint_inventory.reviewed;
                    new_location_and_gallon.receiver_no = viewModel.current_paint_inventory.receiver_no;

                    if (current_application_security.user_name.Length > 10)
                    {
                        new_location_and_gallon.initials = current_application_security.user_name.Substring(1, 10);
                    }
                    else
                    {
                        new_location_and_gallon.initials = current_application_security.user_name;
                    }
                    
                    new_location_and_gallon.location_cd = location_cd;

                    viewModel.Message = barcode_number + " Location was updated to " + viewModel.current_paint_location.loc_description + ".";

                    if (Gallons != "" && Convert.ToInt16(Gallons) != viewModel.current_paint_inventory.number_of_gallons)
                    {
                        new_location_and_gallon.number_of_gallons = Convert.ToInt16(Gallons);
                        viewModel.Message = viewModel.Message + " Gallons were updated from " + viewModel.current_paint_inventory.number_of_gallons.ToString() + " to " + Gallons + ".";
                    }

                    //insert negative quantity record
                    paint_inventory.InsertPaintInventory(negative_quantity);
                    
                    //insert update record
                    paint_inventory.InsertPaintInventory(new_location_and_gallon);

                    return RedirectToAction("MovePaint", "Paint", new { Message = viewModel.Message, location_cd = location_cd });
                }
                catch (Exception ex)
                {
                    viewModel.Error = ex.InnerException.ToString().Substring(1, 300);
                    return RedirectToAction("MovePaint", "Paint", new { Error = viewModel.Error, location_cd = location_cd });
                }                
            }
        }
        public ActionResult MovePaintSubmit_Butler(short location_cd, string barcode_number, int ScannerUsed, string Gallons)
        {
            PaintModel viewModel = new PaintModel();

            viewModel.searched_location_cd = location_cd;

            viewModel.current_paint_location = paint_location.GetPaintLocation(location_cd);

            if (viewModel.current_paint_location == null)
            {
                viewModel.Error = "Location selected not found!";
                return RedirectToAction("MovePaintButler", "Paint", new { Error = viewModel.Error, location_cd = location_cd });
            }

            paint_barcode new_barcode = new paint_barcode();

            if (barcode_number != null)
            {
                //Parse values form barcode string
                string error = paint_barcode.ParseBarcode(barcode_number, ref new_barcode);

                if (error.Length > 0)
                {
                    viewModel.Error = error;

                    return RedirectToAction("MovePaintButler", "Paint", new { Error = viewModel.Error, location_cd = location_cd });
                }

            }
            else
            {
                    viewModel.Error = "Barcode is missing!";
                    return RedirectToAction("MovePaintButler", "Paint", new { Error = viewModel.Error, location_cd = location_cd });
            }
            
            //see if it was processed before
            viewModel.current_paint_inventory = paint_inventory.GetPaintInventoryByBarcodeButler(new_barcode);

            //if not processed before than error 
            if (viewModel.current_paint_inventory.container_id == 0)
            {
                viewModel.Error = "Container wasn't found and the location was not updated";
                return RedirectToAction("MovePaintButler", "Paint", new { Error = viewModel.Error, location_cd = location_cd });

            }
            //it exists, update location
            else
            {

                try
                {
                    string user_init;

                    application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];

                    user_defaults user_initials = user_defaults.GetUserDefaultByName(current_application_security.user_id, "UserInitials");

                    if (user_initials != null)
                   {
                       user_init = user_initials.value;
                   }
                   else
                   {
                       user_init = current_application_security.user_id.ToString();
                   }

                    //create and insert negative quantity
                    paint_inventory negative_quantity = new paint_inventory();

                    negative_quantity.container_id = viewModel.current_paint_inventory.container_id;
                    negative_quantity.paint_cd = viewModel.current_paint_inventory.paint_cd;
                    negative_quantity.location_cd = viewModel.current_paint_inventory.location_cd;
                    negative_quantity.expiration_date = viewModel.current_paint_inventory.expiration_date;
                    negative_quantity.cost = viewModel.current_paint_inventory.cost;
                    negative_quantity.number_of_gallons = viewModel.current_paint_inventory.number_of_gallons * -1; //off set current gallons
                    negative_quantity.change_user_id = viewModel.current_paint_inventory.change_user_id;
                    negative_quantity.change_datetime = DateTime.Now;
                    negative_quantity.received_date = viewModel.current_paint_inventory.received_date;
                    negative_quantity.batch_no = viewModel.current_paint_inventory.batch_no;
                    negative_quantity.container_type = viewModel.current_paint_inventory.container_type;
                    negative_quantity.purchase_order_no = viewModel.current_paint_inventory.purchase_order_no;
                    negative_quantity.@new = viewModel.current_paint_inventory.@new;
                    negative_quantity.po_line_item_no = viewModel.current_paint_inventory.po_line_item_no;
                    negative_quantity.inventory_status = viewModel.current_paint_inventory.inventory_status;
                    negative_quantity.vendor_id = viewModel.current_paint_inventory.vendor_id;
                    negative_quantity.seq_no = viewModel.current_paint_inventory.seq_no + 1;
                    negative_quantity.container_status = viewModel.current_paint_inventory.container_status;
                    negative_quantity.vendor_container_id = viewModel.current_paint_inventory.vendor_container_id;
                    negative_quantity.reviewed = viewModel.current_paint_inventory.reviewed;
                    negative_quantity.receiver_no = viewModel.current_paint_inventory.receiver_no;
                    negative_quantity.drum_no = viewModel.current_paint_inventory.drum_no;
                    negative_quantity.initials = user_init;

                    //insert with new gallons and location
                    paint_inventory new_location_and_gallon = new paint_inventory();

                    //If drum no is Temp and needs to be updated
                    if (new_barcode.drum_no != viewModel.current_paint_inventory.drum_no)
                    {
                        viewModel.current_paint_inventory.drum_no = new_barcode.drum_no;
                    }

                    new_location_and_gallon.container_id = viewModel.current_paint_inventory.container_id;
                    new_location_and_gallon.paint_cd = viewModel.current_paint_inventory.paint_cd;
                    new_location_and_gallon.expiration_date = viewModel.current_paint_inventory.expiration_date;
                    new_location_and_gallon.cost = viewModel.current_paint_inventory.cost;
                    new_location_and_gallon.number_of_gallons = viewModel.current_paint_inventory.number_of_gallons;
                    new_location_and_gallon.change_user_id = viewModel.current_paint_inventory.change_user_id;
                    new_location_and_gallon.change_datetime = DateTime.Now;
                    new_location_and_gallon.received_date = viewModel.current_paint_inventory.received_date;
                    new_location_and_gallon.batch_no = viewModel.current_paint_inventory.batch_no;
                    new_location_and_gallon.container_type = viewModel.current_paint_inventory.container_type;
                    new_location_and_gallon.purchase_order_no = viewModel.current_paint_inventory.purchase_order_no;
                    new_location_and_gallon.@new = viewModel.current_paint_inventory.@new;
                    new_location_and_gallon.po_line_item_no = viewModel.current_paint_inventory.po_line_item_no;
                    new_location_and_gallon.inventory_status = null;
                    new_location_and_gallon.vendor_id = viewModel.current_paint_inventory.vendor_id;
                    new_location_and_gallon.seq_no = viewModel.current_paint_inventory.seq_no + 2;
                    new_location_and_gallon.container_status = viewModel.current_paint_inventory.container_status;
                    new_location_and_gallon.vendor_container_id = viewModel.current_paint_inventory.vendor_container_id;
                    new_location_and_gallon.reviewed = viewModel.current_paint_inventory.reviewed;
                    new_location_and_gallon.receiver_no = viewModel.current_paint_inventory.receiver_no;
                    new_location_and_gallon.drum_no = viewModel.current_paint_inventory.drum_no;
                    new_location_and_gallon.initials = user_init;

                    new_location_and_gallon.location_cd = location_cd;

                    viewModel.Message = new_location_and_gallon.vendor_container_id + " Location was updated to " + viewModel.current_paint_location.loc_description + ".";

                    if (Gallons != "" && Convert.ToInt16(Gallons) != viewModel.current_paint_inventory.number_of_gallons)
                    {
                        new_location_and_gallon.number_of_gallons = Convert.ToInt16(Gallons);
                        viewModel.Message = viewModel.Message + " Gallons were updated from " + viewModel.current_paint_inventory.number_of_gallons.ToString() + " to " + Gallons + ".";
                    }

                    //insert negative quantity record
                    paint_inventory.InsertPaintInventory(negative_quantity);

                    //insert update record
                    paint_inventory.InsertPaintInventory(new_location_and_gallon);

                    return RedirectToAction("MovePaintButler", "Paint", new { Message = viewModel.Message, location_cd = location_cd });
                }
                catch (Exception ex)
                {
                    viewModel.Error = ex.InnerException.ToString().Substring(1, 300);
                    return RedirectToAction("MovePaintButler", "Paint", new { Error = viewModel.Error, location_cd = location_cd });
                }
            }
        }

    }
}
