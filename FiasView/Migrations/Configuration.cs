namespace FiasView.Migrations
{
    using MySql.Data.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Data.Entity.Migrations.History;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FiasView.Model1>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
            SetSqlGenerator("MySql.Data.MySqlClient", new MySqlMigrationSqlGenerator()); 

            SetHistoryContextFactory("MySql.Data.MySqlClient", (conn, schema) => new HistoryConfig(conn, schema)); 

        }

        protected override void Seed(FiasView.Model1 context)
        {

        }
    }
}
