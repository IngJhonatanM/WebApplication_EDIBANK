using EDIBANK.Conf_Db_With_Entity;
using EDIBANK.Models.Monitor;
using EDIBANK.Models.Users_EdiWeb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Runtime.CompilerServices;

namespace EDIBANK.Controllers.MonitorController;

public class MonitorController(AppDbContext context, UserManager<ApplicationUser> userManager) : Controller
{
    private readonly AppDbContext _context = context;
    private readonly UserManager<ApplicationUser> _userManager = userManager;

    // GET: Monitor/Intercambios
    [Authorize]
    public async Task<IActionResult> Intercambios(string? ediActual, DateTime? desde, DateTime? hasta, bool? mostrarEntradas)
    {
        ApplicationUser? appUser = await _userManager.GetUserAsync(User);

        if (appUser is null)
        {
            ModelState.AddModelError(string.Empty, "Usuario no definido (ESTO NO DEBE OCURRIR)");
        }
        ediActual ??= User.IsInRole("Admin")
            ? (await _context.EDISelectorAsync()).First().Value
            : appUser?.EDIId ?? string.Empty;

        DateTime menor = (await _context.Intercambios.MinAsync(static DateTime (Intercambio i) => i.Cargado)).Date;
        DateTime mayor = DateTime.Today;

        desde ??= menor;
        hasta ??= mayor;
        if (hasta < desde)
        {
            ModelState.AddModelError(string.Empty, "Desde no puede ser mayor que Hasta");
        }
        hasta = hasta.Value.AddDays(1.0);
        return View(new MonitorViewModel
        {
            EDIs = await _context.EDISelectorAsync(ediActual),
            EDIActual = ediActual,
            Menor = menor,
            Desde = desde.Value,
            Mayor = mayor,
            Hasta = hasta.Value.AddDays(-1.0),
            MostrarEntradas = mostrarEntradas ?? true,
            Intercambios = await (mostrarEntradas ?? true
                ? from i in _context.Intercambios
                  where desde <= i.Cargado && i.Cargado < hasta && ediActual == i.ReceptorId
                  orderby i.Cargado descending
                  select i
                : from i in _context.Intercambios
                  where desde <= i.Cargado && i.Cargado < hasta && ediActual == i.EmisorId
                  orderby i.Cargado descending
                  select i).ToListAsync()
        });
    }

    // POST: Monitor/Recargar
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Recargar(string ediActual, DateTime desde, DateTime hasta, bool mostrarEntradas, string origenId)
    {
        ApplicationUser? appUser = await _userManager.GetUserAsync(User);

        if (await _context.Intercambios.FindAsync(origenId) is Intercambio origen)
        {
            DateTime cargado = DateTime.Now;
            string id = $"{cargado:yyyyMMddHHmmssfff}";
            string archivoIntercambio = $"{origen.EmisorId}.{origen.ReceptorId}.{id}{Path.GetExtension(origen.ArchivoIntercambio)}";
            string path0 = Path.Join(origen.RutaArchivo, "RESPALDO", origen.ArchivoIntercambio);
            string path1 = Path.Join(origen.RutaArchivo, "ENTRADA", archivoIntercambio);
            string path2 = Path.Join(origen.RutaArchivo, "RESPALDO", archivoIntercambio);
            string? fldr1 = Path.GetDirectoryName(path1)!;
            int stage = 0;

            try
            {
                if (!Directory.Exists(fldr1))
                {
                    Directory.CreateDirectory(fldr1);
                    Auditar<MonitorController>(appUser, $"Directorio {fldr1} creado");
                }
                else
                {
                    fldr1 = null;
                }
                ++stage;    // 1
                System.IO.File.Copy(path0, path1);
                Auditar<MonitorController>(appUser, $"Entrada {path1} creada");
                ++stage;    // 2
                System.IO.File.Copy(path0, path2);
                Auditar<MonitorController>(appUser, $"Respaldo {path2} creado");
                ++stage;    // 3
                _context.Add(new Intercambio
                {
                    Id = id,
                    Cargado = cargado,
                    Descargado = null,
                    Numero = origen.Numero,
                    TipoIntercambio = origen.TipoIntercambio,
                    EmisorId = origen.EmisorId,
                    ReceptorId = origen.ReceptorId,
                    TipoDocumento = origen.TipoDocumento,
                    ArchivoOriginal = origen.ArchivoOriginal,
                    ArchivoIntercambio = archivoIntercambio,
                    RutaArchivo = origen.RutaArchivo,
                    Tamano = origen.Tamano,
                    Status = Status.DISPONIBLE,
                    Emisor = null!,
                    Receptor = null
                });
                Auditar<MonitorController>(appUser, $"{origenId} recargado como {id}");
            }
            catch (Exception e)
            {
                Auditar<MonitorController>(appUser, $"{e}");
                if (await _context.Intercambios.FindAsync(id) is Intercambio recarga)
                {
                    _context.Remove(recarga);
                    Auditar<MonitorController>(appUser, $"{origenId} no recargado como {id}");
                }
                if (2 < stage)
                {
                    System.IO.File.Delete(path2);
                    Auditar<MonitorController>(appUser, $"Respaldo {path2} removido");
                }
                if (1 < stage)
                {
                    System.IO.File.Delete(path1);
                    Auditar<MonitorController>(appUser, $"Entrada {path1} removida");
                }
                if (0 < stage && fldr1 is not null)
                {
                    Directory.Delete(fldr1);
                    Auditar<MonitorController>(appUser, $"Directorio {fldr1} removido");
                }
            }
            try
            {
                await _context.SaveChangesAsync();
            }
            catch
            {
            }
        }
        return LocalRedirect($"~/Monitor/Intercambios?EDIActual={ediActual}&Desde={desde:yyyy-MM-dd}&__Invariant=Desde&Hasta={hasta:yyyy-MM-dd}&__Invariant=Hasta&MostrarEntradas={(mostrarEntradas ? "true" : "false")}");
    }

    // POST: Monitor/Remover
    [HttpPost, ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Remover(string ediActual, DateTime desde, DateTime hasta, bool mostrarEntradas, string origenId)
    {
        ApplicationUser? appUser = await _userManager.GetUserAsync(User);

        if (await _context.Intercambios.FindAsync(origenId) is Intercambio origen)
        {
        }
        return LocalRedirect($"~/Monitor/Intercambios?EDIActual={ediActual}&Desde={desde:yyyy-MM-dd}&__Invariant=Desde&Hasta={hasta:yyyy-MM-dd}&__Invariant=Hasta&MostrarEntradas={(mostrarEntradas ? "true" : "false")}");
    }

    private void Auditar<T>(ApplicationUser? appUser, string? comment, [CallerMemberName] string? memberName = null)
    {
        IPAddress? ip = HttpContext.Connection.RemoteIpAddress;

        _context.Add(new Auditoria
        {
            Fecha = DateTime.Now,
            Usuario = appUser?.Id.Truncar(50) ?? "*DESCONOCIDO*",
            IPRemota = $"{(ip?.IsIPv4MappedToIPv6 is true ? ip.MapToIPv4() : ip)}",
            Modulo = typeof(T).FullName?.Truncar(50),
            Operacion = memberName?.Truncar(50),
            Comentario = comment?.Truncar(250)
        });
    }
}
