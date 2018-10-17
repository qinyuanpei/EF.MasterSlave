using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EF.MasterSlave
{
    public class UserTypeMap : EntityTypeConfiguration<User>
    {
        public UserTypeMap()
        {
            base.HasKey(t => t.UserId);
        }
    }
}
