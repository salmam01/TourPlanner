using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BL.Utils
{
    public class Result
    {
        public enum ResultCode
        {
            Success = 0,
            NullError = 1,
            DatabaseError = 2,
            FileAccessError = 3,
            PdfGenerationError = 4,
            UnknownError = 5
        }
        public ResultCode Code { get; }

        public Result(ResultCode code)
        {
            Code = code;
        }
    }
}
