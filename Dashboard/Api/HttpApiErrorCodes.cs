using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dashboard.Api
{
    public static class HttpApiErrorCodes
    {
        public static string GetErrorDesc(int errorCode)
        {
            return ErrorCodes.ContainsKey(errorCode) ?
                ErrorCodes[errorCode] : "Unknow Error";
            

        }

        private static Dictionary<int, string> ErrorCodes = new Dictionary<int, string>
        {
            {0, "The request has been successfully processed"},
             {1, "Protocol version does not match"},
             {2, "The server does not contain the service corresponding to the request"},
             {3, "The request packet contains illegal fields"},
             {4, "Authentication failed"},
             {5, "The system is busy"},
             {6, "Insufficient resources"},
             {7, "System function authorization failed"},
             {8, "System function is authorized"},
             {9, "The upgrade package does not match"},
             {10, "File download failed"},
             {11, "File integrity verification failed"},
             {20, "Data entry reaches the upper limit"},
             {21, "Record already exists"},
             {22, "Record does not exist"},
             {23, "Failed to write data"},
             {24, "Failed to read data"},
            {25, "Failed to extract feature value"},
             {26, "Portrait quality does not meet registration requirements"},
             {30, "Wiegand card number does not support fuzzy matching"},
            {35, "Image decoding failed"},
             {36, "The image is too large, and the jpg image used to extract features cannot exceed 10M"},
            {37, "Normalized image failed"},
            {38, "Face size is too small"},
            {39, "Portrait quality is too poor"},
            {40, "The number of faces in the image is not 1"},
            {41, "The face is incomplete in the image"},

        };
    }
}
