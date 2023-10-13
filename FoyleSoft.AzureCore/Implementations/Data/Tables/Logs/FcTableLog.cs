using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace FoyleSoft.Core.Implementations.Data.Tables.Logs
{
    public class FcTableLog : BaseTable
    {
        public int FcTableId { get; set; }
        public DateTime LogDate { get; set; }
        public EntityState State { get; set; }
        public int LogUserId { get; set; }
        public string RawData { get; set; }

    }
}
