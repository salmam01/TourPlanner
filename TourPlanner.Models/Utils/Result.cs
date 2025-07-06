using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Models.Utils.Helpers
{
    public class Result
    {
        public enum ResultCode
        {
            Success,
            NullError,
            DatabaseError,
            ApiError,
            FileAccessError,
            PdfGenerationError,
            ParseError,
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
