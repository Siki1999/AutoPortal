using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AutoPortal.Misc
{
    interface ILogiraniKorisnik : IPrincipal
    {
        string KorisnickoIme { get; set; }
        string PrezimeIme { get; set; }
        string Ovlast { get; set; }
    }
}
