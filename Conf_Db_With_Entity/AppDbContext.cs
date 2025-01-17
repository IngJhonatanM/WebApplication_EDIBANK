using EDIBANK.Models.Monitor;
using EDIBANK.Models.Users_EdiWeb;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EDIBANK.Conf_Db_With_Entity;

/// <summary>
/// AppDbContext extends Entity Framework's IdentityDBContext,
/// representing a database session and used to customize Identity's schema to suit application needs.
/// </summary>
public class AppDbContext(DbContextOptions options) : IdentityDbContext<ApplicationUser>(options)
{
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.UseCollation("Latin1_General_100_CI_AI_SC_UTF8");
        modelBuilder.Entity<IdentityRole>()
                    .HasData(new()
                    {
                        Name = "Admin",
                        NormalizedName = "Admin"
                    }, new()
                    {
                        Name = "User",
                        NormalizedName = "User"
                    });
        modelBuilder.Entity<Intercambio>()
                    .ToTable(static void (TableBuilder<Intercambio> b) =>
                    {
                        b.HasCheckConstraint("CK_tip_intercambio", "[tip_intercambio] IN (0, 1, 2)");
                        b.HasCheckConstraint("CK_status", "[status] IN (0, 1, 2)");
                    });
        modelBuilder.Entity<HistoricoIntercambio>()
                    .ToTable(static void (TableBuilder<HistoricoIntercambio> b) =>
                    {
                        b.HasCheckConstraint("CK_tip_intercambio", "[tip_intercambio] IN (0, 1, 2)");
                    });
    }

    public async Task<SelectList> EDISelectorAsync(string? selectedValue = null) => new(
        items: await (from e in EDIs
                      orderby e.Descripcion
                      select new { e.Id, e.Descripcion }).ToListAsync(),
        dataValueField: nameof(EDI.Id),
        dataTextField: nameof(EDI.Descripcion),
        selectedValue: selectedValue);

    public required DbSet<Intercambio> Intercambios { get; set; }
    public required DbSet<HistoricoIntercambio> HistoricoIntercambios { get; set; }
    public required DbSet<EDI> EDIs { get; set; }
    public required DbSet<Auditoria> Auditorias { get; set; }
}