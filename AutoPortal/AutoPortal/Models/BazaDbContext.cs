using MySql.Data.EntityFramework;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace AutoPortal.Models
{
    [DbConfigurationType(typeof(MySqlEFConfiguration))]
    public class BazaDbContext : DbContext
    {
        public DbSet<Auto> PopisAuto { get; set; }
        public DbSet<Ugovor> PopisUgovor { get; set; }
        public DbSet<Korisnik> PopisKorisnika { get; set; }
        public DbSet<Ovlast> PopisOvlasti { get; set; }

    }
}