using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Collections;

namespace YOTPSelfDemo
{
    public class YOTPSelfService
    {
        public string AssignYubiKey(string username, string ykserial, string yotpcode)
        {
            
            // API connection parameters
            string ikey = "DIWY6N1ZMQ1ZY4KU13T1";
            string skey = "rq3BWgChXrO6QLHaU7ORoKeEQpyWCGpD99ZWu46h";
            string host = "api-f663dfcf.duosecurity.com";

            string StatusMessage = "";

            // initialize API client
            var client = new Duo.DuoApi(ikey, skey, host);
            string userid = "";
            string tokenid = "";
            string YKserialnum;


            if (!String.IsNullOrEmpty(yotpcode))
            {
                string publicID = yotpcode.Substring(0, 12);
                ModhexConverter modhexConverter = new ModhexConverter();
                YKserialnum = modhexConverter.ModHexDecode(publicID).ToString();
            }
            else if (ykserial != "")
                YKserialnum = ykserial;
            else
            {
                StatusMessage = "Serial Number is required";
                return StatusMessage;
            }

            // first we need to find the userid associated to the user
            var parameters = new Dictionary<string, string>();
            parameters.Add("username", username);
            var users = client.JSONApiCall<System.Collections.ArrayList>
                ("GET", "/admin/v1/users", parameters);

            if (users.Count == 0)
            {
                StatusMessage = "username not found";
                return StatusMessage;
            }
            else if (users.Count > 1)
            {
                StatusMessage = "multiple users found";
                return StatusMessage;
            }
            else
            {
                foreach (JsonElement user in users)
                {
                    {
                        userid = user.GetProperty("user_id").ToString();
                    }

                }
            }

            // now we need to find the tokenid associated to the Yubikey using the serial number
            var queryparams = new Dictionary<string, string>();
            queryparams.Add("type", "yk");
            queryparams.Add("serial", YKserialnum);
            var tokens = client.JSONApiCall<System.Collections.ArrayList>("GET", "/admin/v1/tokens", queryparams);

            if (tokens.Count == 0)
            {
                StatusMessage = "token not found";
                return StatusMessage;
            }
            else if (tokens.Count > 1)
            {
                StatusMessage = "multiple tokens found";
                return StatusMessage;
            }
            else
            {
                foreach (JsonElement token in tokens)
                {
                    {
                        tokenid = token.GetProperty("token_id").ToString();
                        JsonElement tokenusers = token.GetProperty("users");

                        if (tokenusers.GetArrayLength() > 0)
                        {
                            if (tokenusers.GetArrayLength() > 0)
                            {
                                foreach (JsonElement tuser in tokenusers.EnumerateArray())
                                {
                                    string tusername = (tuser.GetProperty("username").ToString());
                                    StatusMessage = "Token already assigned to user: " + tusername;
                                    return StatusMessage;
                                }
                            }
                        }

                    }

                }

            }

            // now we assign the YubiKey to the user (using userid and tokenid)
            var tokenparams = new Dictionary<string, string>();
            tokenparams.Add("token_id", tokenid);
            string restURL = "/admin/v1/users/" + userid + "/tokens";
            string response = client.ApiCall("POST", restURL, tokenparams);

            var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(response);
            string result = dict["stat"].ToString();

            StatusMessage = "Status: " + result;
            return StatusMessage;

        }
    }
    
}
