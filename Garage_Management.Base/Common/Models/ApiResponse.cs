using Garage_Management.Base.Entities.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Garage_Management.Base.Common.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        /// <summary>
        /// mã hoặc nội dung thông báo (MSG01..)
        /// </summary>
        public string Message { get; set; } = string.Empty;  
        public T? Data { get; set; }

        public static ApiResponse<T> SuccessResponse(T data, string message = "Thành công")
        {
            return new ApiResponse<T> { Success = true, Message = message, Data = data };
        }

        public static ApiResponse<T> ErrorResponse(string message)
        {
            return new ApiResponse<T> { Success = false, Message = message };
        }


        // public static ApiResponse<User> SuccessResponse(object value, string v)
        // {
        //     throw new NotImplementedException();
        // }
        // Remove the placeholder overload to avoid runtime errors.
    }
}
