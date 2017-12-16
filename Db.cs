namespace Exemplos.FaceApi
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Microsoft.EntityFrameworkCore;

    public class Db : DbContext
    {
        private static DbContextOptions<Db> BuildOptions(string connectionString) {
            var builder = new DbContextOptionsBuilder<Db>();
            builder.UseSqlServer(connectionString);
            return builder.Options;
        }

        public Db(string connectionString) : base(BuildOptions(connectionString))
        {
        }

        public DbSet<Person> Persons { get; set; }
    }

    [Table("Person")]
    public class Person
    {
        public int Id { get; set; }
        // public string Name { get; set; }
        public string Gender { get; set; }
        public int? Age { get; set; }
        public byte[] Picture { get; set; }
    }


}