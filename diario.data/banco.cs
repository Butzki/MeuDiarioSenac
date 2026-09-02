using Microsoft.EntityFrameworkCore;

public class MeuDiarioSENACContext : DbContext 
{
    public DbSet<Registro> Registros {get; set;}
    public DbSet<Usuario> Usuarios {get; set;}

    private readonly string ConnectionString = "server=localhost;database=MeuDiarioSenac;uid=root;pwd=1234";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseMySql(ConnectionString, ServerVersion.AutoDetect(ConnectionString));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Usuario>()
            .HasMany(u => u.Registros)
            .WithOne(r => r.Usuario)
            .HasForeignKey(r => r.UsuarioId);
    }

}