using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace EDIBANK.Models.Monitor;

public class MonitorViewModel
{
    public required SelectList EDIs { get; set; }

    public required string EDIActual { get; set; }

    public required DateTime Menor { get; init; }

    [DataType(DataType.Date)]
    public required DateTime Desde { get; set; }

    public required DateTime Mayor { get; init; }

    [DataType(DataType.Date)]
    public required DateTime Hasta { get; set; }

    public required bool MostrarEntradas { get; set; }

    public required IEnumerable<Intercambio> Intercambios { get; set; }
}
