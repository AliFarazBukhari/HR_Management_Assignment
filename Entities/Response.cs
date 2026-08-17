using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class Response<T>
    {
        public T? Data { get; set; }
        public HttpStatusCode HttpStatusCode { get; set; }
        public ResponseCode ResponseCode { get; set; }
        public string? ResponseMessage{ get; set; }
    }
}
