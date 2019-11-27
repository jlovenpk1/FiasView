namespace FiasView
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class Model1 : DbContext
    {
        public Model1()
            : base("name=fias")
        {
        }

        public virtual DbSet<addrcorrect> addrcorrect { get; set; }
        public virtual DbSet<addrob30> addrob30 { get; set; }
        public virtual DbSet<dictionarycorrect> dictionarycorrect { get; set; }
        public virtual DbSet<house30> house30 { get; set; }
        public virtual DbSet<room30> room30 { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<addrcorrect>()
                .Property(e => e.shortName)
                .IsUnicode(false);

            modelBuilder.Entity<addrcorrect>()
                .Property(e => e.fullName)
                .IsUnicode(false);

            modelBuilder.Entity<addrcorrect>()
                .Property(e => e.FiasID)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.AOGUID)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.AOID)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.AREACODE)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.AUTOCODE)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.CITYCODE)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.CODE)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.FORMALNAME)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.IFNSFL)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.IFNSUL)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.NEXTID)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.OFFNAME)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.OKATO)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.OKTMO)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.PARENTGUID)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.PLACECODE)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.PLAINCODE)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.POSTALCODE)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.PREVID)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.REGIONCODE)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.SHORTNAME)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.STREETCODE)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.TERRIFNSFL)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.TERRIFNSUL)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.CTARCODE)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.EXTRCODE)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.SEXTCODE)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.NORMDOC)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.PLANCODE)
                .IsUnicode(false);

            modelBuilder.Entity<addrob30>()
                .Property(e => e.CADNUM)
                .IsUnicode(false);

            modelBuilder.Entity<dictionarycorrect>()
                .Property(e => e.Name)
                .IsUnicode(false);

            modelBuilder.Entity<dictionarycorrect>()
                .Property(e => e.userName)
                .IsUnicode(false);

            modelBuilder.Entity<dictionarycorrect>()
                .Property(e => e.IDCorrect)
                .IsUnicode(false);

            modelBuilder.Entity<house30>()
                .Property(e => e.AOGUID)
                .IsUnicode(false);

            modelBuilder.Entity<house30>()
                .Property(e => e.BUILDNUM)
                .IsUnicode(false);

            modelBuilder.Entity<house30>()
                .Property(e => e.HOUSEGUID)
                .IsUnicode(false);

            modelBuilder.Entity<house30>()
                .Property(e => e.HOUSEID)
                .IsUnicode(false);

            modelBuilder.Entity<house30>()
                .Property(e => e.HOUSENUM)
                .IsUnicode(false);

            modelBuilder.Entity<house30>()
                .Property(e => e.IFNSFL)
                .IsUnicode(false);

            modelBuilder.Entity<house30>()
                .Property(e => e.IFNSUL)
                .IsUnicode(false);

            modelBuilder.Entity<house30>()
                .Property(e => e.OKATO)
                .IsUnicode(false);

            modelBuilder.Entity<house30>()
                .Property(e => e.OKTMO)
                .IsUnicode(false);

            modelBuilder.Entity<house30>()
                .Property(e => e.POSTALCODE)
                .IsUnicode(false);

            modelBuilder.Entity<house30>()
                .Property(e => e.STRUCNUM)
                .IsUnicode(false);

            modelBuilder.Entity<house30>()
                .Property(e => e.TERRIFNSFL)
                .IsUnicode(false);

            modelBuilder.Entity<house30>()
                .Property(e => e.TERRIFNSUL)
                .IsUnicode(false);

            modelBuilder.Entity<house30>()
                .Property(e => e.NORMDOC)
                .IsUnicode(false);

            modelBuilder.Entity<house30>()
                .Property(e => e.CADNUM)
                .IsUnicode(false);

            modelBuilder.Entity<room30>()
                .Property(e => e.ROOMID)
                .IsUnicode(false);

            modelBuilder.Entity<room30>()
                .Property(e => e.ROOMGUID)
                .IsUnicode(false);

            modelBuilder.Entity<room30>()
                .Property(e => e.HOUSEGUID)
                .IsUnicode(false);

            modelBuilder.Entity<room30>()
                .Property(e => e.REGIONCODE)
                .IsUnicode(false);

            modelBuilder.Entity<room30>()
                .Property(e => e.FLATNUMBER)
                .IsUnicode(false);

            modelBuilder.Entity<room30>()
                .Property(e => e.ROOMNUMBER)
                .IsUnicode(false);

            modelBuilder.Entity<room30>()
                .Property(e => e.ROOMTYPE)
                .IsUnicode(false);

            modelBuilder.Entity<room30>()
                .Property(e => e.CADNUM)
                .IsUnicode(false);

            modelBuilder.Entity<room30>()
                .Property(e => e.ROOMCADNUM)
                .IsUnicode(false);

            modelBuilder.Entity<room30>()
                .Property(e => e.POSTALCODE)
                .IsUnicode(false);

            modelBuilder.Entity<room30>()
                .Property(e => e.PREVID)
                .IsUnicode(false);

            modelBuilder.Entity<room30>()
                .Property(e => e.NEXTID)
                .IsUnicode(false);

            modelBuilder.Entity<room30>()
                .Property(e => e.NORMDOC)
                .IsUnicode(false);
        }
    }
}
