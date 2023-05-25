using Scanware.Data;
using System.Collections.Generic;


namespace Scanware.Models
{
    public class ZincModel
    {
        public string Error { get; set; }
        public string Alert { get; set; }
        public string Message { get; set; }
        public zinc_tracking current_ingot { get; set; }
        public string current_ingot_consume_user { get; set; }
        public string current_ingot_add_user { get; set; }
        public string current_ingot_change_user { get; set; }
        public string searched_ingot_id { get; set; }
        public string consumed_ingot_id { get; set; }

        public string default_zinc_line { get; set; }

        public List<zinc_tracking> zincInventory { get; set; }
     
    }
}