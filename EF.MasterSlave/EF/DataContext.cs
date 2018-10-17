using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.MasterSlave
{
    public class DataContext : DbContext
    {
        public DataContext()
            : base("name=DataContext")
        {

        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new UserTypeMap());
            base.OnModelCreating(modelBuilder);
        }

        public virtual DbSet<User> Users { get; set; }
    }
}
