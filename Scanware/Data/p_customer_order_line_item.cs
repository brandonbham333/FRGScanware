using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class customer_order_line_item
    {
        public static customer_order_line_item GetCustomerOrderLineItem(int order_no, int line_item_no)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            customer_order_line_item coli = db.customer_order_line_item.SingleOrDefault(x => x.order_no == order_no && x.line_item_no == line_item_no);

            return coli;
        }
    }
}