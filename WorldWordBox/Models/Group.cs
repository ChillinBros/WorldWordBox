using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WorldWordBox.Models
{
    public class Group
    {
        private int id;

        public int Id
        {
            get { return id; }
            set { id = value; }
        }

        private String name;

        public String Name
        {
            get { return name; }
            set { name = value; }
        }

        public Group(int id, String name)
        {
            this.id = id;
            this.name = name;
        }


    }
}