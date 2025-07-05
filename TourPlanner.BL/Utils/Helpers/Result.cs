using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.BL.Utils.Helpers
{
    public class Result
    {
        public enum ResultCode
        {
            Success,
            NullError,
            DatabaseError,
            FileAccessError,
            PdfGenerationError,
            ParseError,
            ApiError,
            UnknownError
        }
        public ResultCode Code { get; }
        public object? Data { get; }

        public Result(ResultCode code, object? data = null)
        {
            Code = code;
            Data = data;
        }
    }
}
