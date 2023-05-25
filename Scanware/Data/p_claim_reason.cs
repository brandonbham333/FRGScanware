using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Scanware.Data
{
    public partial class claim_reason
    {
        public static List<claim_reason> GetScanwareHoldDefects()
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.claim_reason.Where(x => x.scanware_hold == "Y").ToList();
        }

        public static claim_reason GetClaimReason(int claim_reason_cd)
        {
            sdipdbEntities db = ContextHelper.SDIPDBContext;

            return db.claim_reason.Where(x => x.claim_reason_cd == claim_reason_cd).FirstOrDefault();
        }
    }
}