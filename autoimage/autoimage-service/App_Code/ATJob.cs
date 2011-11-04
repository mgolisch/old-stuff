using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace MGO.AutoImage.Service.App_Code
{
    public class ATJob
    {
        private string command;
        private int id;
        private DateTime date;

        public string Command { 
            set { command = value; } 
            get { return command; } 
        }

        public string Id
        {
            set { id = value; }
            get { return id; }
        }

        public DateTime Date
        {
            set { date = value; }
            get { return date; }
        }
    }
}
