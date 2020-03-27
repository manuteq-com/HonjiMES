using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HonjiMES.Models
{
    public class Fun
    {
        internal static APIResponse APIResponseOK(Object data, string message = "")
        {
            var APIResponse = new APIResponse { data = data, success = true, message = message };
            return APIResponse;
        }
        internal static APIResponse APIResponseError(Object data, string message)
        {
            var APIResponse = new APIResponse { data = data, success = true, message = message };
            return APIResponse;
        }

    }
}
