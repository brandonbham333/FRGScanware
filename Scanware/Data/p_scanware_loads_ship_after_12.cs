using System.Linq;


namespace Scanware.Data
{
    public partial class scanware_loads_ship_after_12
    {
        public static string is_load_ship_after_12(int load_id)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            var result = db.scanware_loads_ship_after_12.Where(x => x.load_id == load_id).FirstOrDefault();

            if (result == null)
                return "N";
            else
                return result.ship_after_12AM;
        }
    }
}
