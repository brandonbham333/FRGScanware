using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Scanware.Data;

namespace Scanware.Models
{
    public class CoilModel
    {
        public coil current_coil { get; set; }
        public inventory_item current_inventory_item { get; set; }
        public all_produced_coils current_all_produced_coil { get; set; }
        public string Error { get; set; }
        public string Alert { get; set; }
        public string Message { get; set; }
        public string searched_coil_number { get; set; }
        public List<vw_sw_coil_history> coil_history { get; set; }
        public customer_ship_to coil_customer_ship_to { get; set; }
        public List<coil_yard_columns> CoilYardCols { get; set; }
        public List<coil_yard_rows> CoilYardRows { get; set; }
        public List<coil_yard_bays> CoilYardBays { get; set; }
        public string current_column { get; set; }
        public string current_row { get; set; }
        public string current_loc { get; set; }
        public string current_saddle { get; set; }
        public coil_yard_locations current_coil_yard_location { get; set; }
        public coil_yard_locations new_coil_yard_location { get; set; }
        public coil_yard_bays current_coil_yard_bay { get; set; }
        public inventory_reason current_inventory_reason { get; set; }
        public coil_status current_coil_status { get; set; }
        public string char_load_id { get; set; }
        public packaging_type current_packaging_type { get; set; }
        public product_type order_product_type { get; set; }
        public List<printer> available_printers{get;set;}
        public string selected_printer_path { get; set; }
        public List<vw_sw_scheduled_coils> scheduled_coils { get; set; }
        public List<string> schedules { get; set; }
        public string selected_schedule { get; set; }
        public List<product_processors> schedule_product_processors { get; set; }
        public string current_facility_cd { get; set; }
        public string current_type_of_packaging { get; set; }
        public List<vw_sw_packaging> coils_to_package { get; set; }
        public List<vw_sw_produced_coils> produced_coils { get; set; }
        public List<claim_reason> ScanwareHoldDefects { get; set; }
        public List<zebra_template> zebra_templates { get; set; }
        public zebra_template selected_template { get; set; }
        public List<all_produced_coils> sister_coils { get; set; }
        public bool searched_coil_consumed { get; set; }

        //public List<apc_images> coil_images { get; set; }
        //public apc_images coil_image { get; set; }
        public v_apc_images coil_image { get; set; }
        public List<v_apc_images> coil_images { get; set; }

        public List<sdi_galv_sched_sp_model> GalvSched { get; set; }
        public List<coil_yard_columns> AllCoilYardCols { get; set; }
        public List<coil_yard_locations> CoilYardLocations { get; set; }

        public List<sdi_galv_sched_Result> sdi_galv_sched_Results { get; set;}

        public List<sdi_revmill_sched_sp_model> sdi_revmill_sched_Results { get; set; }
        public List<sdi_tempmill_sched_sp_model> sdi_tempmill_sched_Results { get; set; }
        public bool has_HOT_fls { get; set; }

        public string loc { get; set; }
        public string production_coil_no { get; set; }
        public string shipped_coil_no { get; set; }
        public bool add_jville_check { get; set; }
    }

    public class sdi_galv_sched_sp_model
    {
        public string c_coil { get; set; }
        public string col { get; set; }
        public string row { get; set; }
        public string prod_status { get; set; }
    }

    public class sdi_revmill_sched_sp_model
    {
        public string c_coil { get; set; }
        public string column { get; set; }
        public string row { get; set; }

        public string prod_status { get; set; }
    }

    public class sdi_tempmill_sched_sp_model
    {
        public string c_coil { get; set; }
        public string col { get; set; }
        public string row { get; set; }

        public string prod_status { get; set; }
    }
}