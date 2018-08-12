using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CityInfo.API.Services
{
    public class CloudMailService : IMailService
    {
        private string _mailTo = Startup.Configuration["mailSettings:mailToAddress"]; //"admin@admin.com";
        private string _mailFrom = Startup.Configuration["mailSettings:mailFromAddress"];//"noreply@noreply.com";

        public void Send(string sub, string msz)
        {
            Debug.WriteLine($" Mail from {_mailFrom} to {_mailTo} , with cloud mail service");
            Debug.WriteLine($"Subject is {sub}");
            Debug.WriteLine($"Msz is {msz}");
        }
    }
}
