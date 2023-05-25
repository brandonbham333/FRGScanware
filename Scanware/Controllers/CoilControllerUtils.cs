using Scanware.Data;
using Scanware.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Scanware.Controllers
{
    public partial class CoilController : Controller
    {
        //
        // GET: /Default1/
        
        
        /// <summary>
        /// Get the status of a coil, if it is not last facility it is not consumed.
        /// </summary>
        /// <param name="production_coil_no"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public Boolean IsCoilConsumed(string production_coil_no, CoilModel model)
        {
            //check if model is null, then check to make sure coil number matches
            if(model.current_all_produced_coil != null && model.current_all_produced_coil.production_coil_no == production_coil_no)
            {
                if (model.current_all_produced_coil.coil_last_facility_ind == "N")
                {
                    return true; //coil is consumed
                }
                else
                {
                    return false; //coil is not consumed
                }
                
            }
            else
            {
                //making sure current all produced is current coil.
                model.current_all_produced_coil = all_produced_coils.GetAllProducedCoilByProductionCoilNumberOrTagNumber(production_coil_no);
                if(model.current_all_produced_coil.coil_last_facility_ind == "N")
                {
                    return true; //coil is consumed
                }
                else
                {
                    return false; //coil no consumed
                }
            }
            
            
        }

        
        public Tuple<string, string> GetColumnAndRow(string location)
        {
            
            string row, column;

            var split = location.Split('$');
            column = split[1];
            row = split[2];

            return Tuple.Create(column, row);
        }

        public bool ConsumedMovementSetting()
        {
            try
            {
                //getting application settings.
                application_settings setting = application_settings.GetAppSetting("AllowConsumedCoilMovement");
                if (setting == null)
                {
                    setting = new application_settings
                    {
                        default_value = "N",
                    };
                }
                return setting.default_value == "N" ? false : true;
            }
            catch
            {
                //if it errored then there is not this specified setting so continue as previously.
                return true;
            }
        }

        private string MoveConsumedCoil(string production_coil_no, CoilModel viewModel, string scanner_used)
        {
            try
            {
                application_security current_application_security = (application_security)System.Web.HttpContext.Current.Session["application_security"];
                //if a coil is consumed, we want to 
                string location = application_settings.GetAppSetting("ConsumedCoilYardLocation").default_value;
                viewModel.current_all_produced_coil = all_produced_coils.GetAllProducedCoil(production_coil_no);
                //if coil is consumed it will not be in coil table.

                var currentDivision = ref_sys_param.GetRefSysParam().location;

                var loc = GetColumnAndRow(location);
                string column = loc.Item1, 
                        row = loc.Item2;

                viewModel.current_coil_yard_location = coil_yard_locations.GetCoilYardLocation(viewModel.current_all_produced_coil.production_coil_no);
                if (viewModel.current_coil_yard_location != null)
                {
                    viewModel.new_coil_yard_location = coil_yard_locations.UpdateCoilYardLocation(viewModel.current_all_produced_coil.production_coil_no, column, row, current_application_security.user_id, scanner_used);
                }
                else
                {
                    viewModel.new_coil_yard_location = coil_yard_locations.InsertCoilYardLocation(viewModel.current_all_produced_coil.production_coil_no, column, row, current_application_security.user_id, scanner_used);
                }

                if(currentDivision == "S")
                {
                    PrintLabel("10.83.12.55", production_coil_no, 2);
                }


                return $"{production_coil_no} has been consumed. {production_coil_no} has been moved to {location} for consumed coils. {(currentDivision == "S" ? "New Tag printed in Andrew Sisson's Office" : "")}";
            }
            catch
            {
                return $"Failed to update location for Consumed Coil {production_coil_no}!";
            }

        }

    }
}
