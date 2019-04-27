using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContactAssistant.Models
{

    public class Contact
    {
        public int id { get; set; }
        public int age { get; set; }
        public string name { get; set; }
    }

    public class ContactList : List<Contact>
    {
    }
}
