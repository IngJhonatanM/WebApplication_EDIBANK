using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDIBANK.Models.Monitor;

[Table("edi")]
public class EDI
{
    [Key, DatabaseGenerated(DatabaseGeneratedOption.None), Column("id_edi", TypeName = "varchar(15)", Order = 0), Unicode(false)]
    public required string Id { get; init; }

    [Column("des_edi", TypeName = "varchar(50)", Order = 1)]
    public required string Descripcion { get; init; }

    [InverseProperty("Emisor")]
    public IEnumerable<Intercambio> Emisores { get; } = new HashSet<Intercambio>(ReferenceEqualityComparer.Instance);

    [InverseProperty("Receptor")]
    public IEnumerable<Intercambio> Receptores { get; } = new HashSet<Intercambio>(ReferenceEqualityComparer.Instance);

    [InverseProperty("Emisor")]
    public IEnumerable<HistoricoIntercambio> HistoricoEmisores { get; } = new HashSet<HistoricoIntercambio>(ReferenceEqualityComparer.Instance);

    [InverseProperty("Receptor")]
    public IEnumerable<HistoricoIntercambio> HistoricoReceptores { get; } = new HashSet<HistoricoIntercambio>(ReferenceEqualityComparer.Instance);
}
