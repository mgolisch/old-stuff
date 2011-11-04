using System;
using System.Collections.Generic;
using System.Text;

namespace MGO.AutoImage.DataAccess
{
    class Client
    {
        public Client() {
        
        }

        private string m_name = string.Empty;
        private int m_id = int.MinValue;

        public string Name {
            get { return m_name; }
            set { m_name = value; } 
        }
        public int Id
        {
            get { return m_id; }
            set { m_id = value; }
        }

    }
}
