using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EDIBANK.Models.Monitor;

[Table("auditoria")]
public class Auditoria
{
    [Key, Column("id_auditoria", TypeName = "bigint", Order = 0), DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; init; }

    [Column("fec_auditoria", TypeName = "datetime2", Order = 1)]
    public required DateTime Fecha { get; init; }

    [Column("login_usuario", TypeName = "varchar(50)", Order = 2)]
    public required string Usuario { get; init; }

    [Column("ip_remota", TypeName = "varchar(39)", Order = 3)]
    public string? IPRemota { get; init; }

    [Column("modulo", TypeName = "varchar(50)", Order = 4)]
    public string? Modulo { get; init; }

    [Column("operacion", TypeName = "varchar(50)", Order = 5)]
    public string? Operacion { get; init; }

    [Column("comentarios", TypeName = "varchar(250)", Order = 6)]
    public string? Comentario { get; init; }
}
