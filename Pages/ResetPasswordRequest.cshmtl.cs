
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Data;

namespace budoco.Pages
{
    public class ResetPasswordRequestModel : PageModel
    {
        [BindProperty]
        public string email { get; set; }

        public void OnPost()
        {
            var errs = new List<string>();
            if (String.IsNullOrWhiteSpace(email))
            {

                errs.Add("Email address is required.");

            }
            else
            {

                // is email in our db?
                var sql = "select * from users where us_email = @us_email";
                var dict = new Dictionary<string, dynamic>();
                dict["@us_email"] = email;
                DataRow dr_user = db_util.get_datarow(sql, dict);

                if (dr_user is not null)
                {
                    // insert unguessable bytes into db for user to confirm registration
                    sql = @"insert into emailed_links 
                     (el_guid, el_email, el_user_id, el_action)
                    values(@el_guid, @el_email, @el_user_id, @el_action)";

                    var guid = Guid.NewGuid();
                    dict["@el_guid"] = guid;
                    dict["@el_email"] = email;
                    dict["@el_action"] = "reset";
                    dict["@el_user_id"] = dr_user[0];

                    db_util.exec(sql, dict);


                    string body = "Follow or browse to this link to reset password:\n"
                        + Startup.cnfg.WebsiteUrlRootWithoutSlash
                        + "/ResetPassword?guid="
                        + guid;

                    string email_result = bd_util.send_email(
                        email, // to
                        Startup.cnfg.SmtpUser,  // from
                        Startup.cnfg.AppName + ": Reset Password", // subject
                        body);

                    if (email_result != "")
                    {
                        errs.Add("Unable to send password reset email. Please try again.");
                    }
                    else
                    {
                        bd_util.set_flash_msg(HttpContext, "An email with password reset instructions was sent to " + email);
                    }

                }
                else
                {
                    errs.Add("The email address is not registered.");
                }


                if (errs.Count > 0)
                {
                    bd_util.set_flash_err(HttpContext, errs);
                }
            }
        }
    }
}