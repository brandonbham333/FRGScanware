using Scanware.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Scanware.Models
{
    public class ShippingModel
    {
        public string Message { get; set; }
        public string Error { get; set; }
        public List<vw_sw_inbound_op_coils> inbound_op_coils { get; set; }

        public List<vw_sw_outbound_op_coils> outbound_op_coils { get; set; }
        public List<shipment_load> shipment_loads { get; set; }

        public List<shipment_load> shipment_loads_rail { get; set; }

        public List<shipment_load> shipment_loads_rail_after_12am { get; set; }
        public List<rail_cars> rail_cars { get; set; }
        public List<rail_car_brand> rail_car_brands { get; set; }
        public string rail_car_brand { get; set; }
        public shipment_load shipment { get; set; }
        public List<load_dtl> coils_in_shipment { get; set; }
        public customer_ship_to shipment_customer_ship_to { get; set; }
        public string searched_char_load_id { get; set; }
        public int coils_in_shipment_weight { get; set; }
        public int coils_in_shipment_packaging_weight { get; set; }
        public carrier current_carrier { get; set; }
        public List<carrier> active_carriers { get; set; }

        public printer default_zebra_printer { get; set; }
        public printer default_network_printer { get; set; }

        public bool all_coils_verified { get; set; }

        public DateTime searched_scheduled_date { get; set; }
        public List<v_sw_print_rail_loads> print_rail_loads { get; set; }

        [UIHint("SignaturePad")]
        public byte[] MySignature { get; set; }

        public IEnumerable<string> ship_to_location_names { get; set; }
        public IEnumerable<string> char_load_ids { get; set; }
        public IEnumerable<string> rail_routes { get; set; }
        public IEnumerable<string> rail_car_types { get; set; }

        public string searched_ship_to_location_name { get; set; }
        public string searched_rail_route { get; set; }
        public string searched_print_status { get; set; }
        public string searched_rail_car_type { get; set; }
        public string searched_location_column { get; set; }

        public List<string> distinct_location_columns { get; set; }
        public List<string> distinct_ship_to_location_names { get; set; }

        public List<shipment_load_images> load_images { get; set; }
        public shipment_load_images load_image { get; set; }
        public v_shipment_load_images load_image_bytes { get; set; }
        public List<vw_shipping_schedule_334> SchedCoils { get; set; }
        public string location {get; set;}
        public bool ButlerSubLoads { get; set; }
        public List<CoilsInLoad> coilsInLoad { get; set; }

        public bool LoadVerified { get; set; }

        public List<LoadsAndCoils> loads { get; set; }

        public string searched_production_coil_no { get; set; }
        public string searched_tag_no{ get; set; }
        public string searched_po_order_no { get; set; }
        public string searched_order_no { get; set; }
        public string searched_vehicle_no { get; set; }


        public string pre_check_vehicle_no { get; set; }
        public string pre_check_driver_name { get; set; }

        public List<v_sw_load_details> load_details { get; set; }

        public List<sw_op_coil_validate> op_coil_validate { get; set; }
        public sw_op_coil_validate op_coil{ get; set; }

        public all_produced_coils current_all_produced_coil { get; set; }

        public string ovd_b_coils { get; set; }

        public string ovd_l_coils { get; set; }

        public string coil_scan_flag { get; set; }

        public string is_after_12_load { get; set; }

        public Boolean retry_shipping { get; set; }

        public int scale_weight_in { get; set; }

        public string require_rail_image { get; set; }
        public bool set_max_weight { get; set; }
        [Required(ErrorMessage = "Please select file.")]
        [Display(Name = "Browse File")]
        public HttpPostedFileBase[] upload_files { get; set; }

        public string shipping_loading_instructions { get; set; }
    }
}