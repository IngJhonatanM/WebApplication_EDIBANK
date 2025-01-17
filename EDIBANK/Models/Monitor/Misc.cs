using System.ComponentModel.DataAnnotations;
using System.Text;

namespace EDIBANK.Models.Monitor;

public enum TipoIntercambio : byte
{
    [Display(Name = "Desconocido")]
    DESCONOCIDO,
    [Display(Name = "EDIFACT")]
    EDIFACT,
    [Display(Name = "CIPHER")]
    CIPHER
}

public enum Status : byte
{
    [Display(Name = "Rechazado")]
    RECHAZADO,
    [Display(Name = "Disponible")]
    DISPONIBLE,
    [Display(Name = "Descargado")]
    DESCARGADO
}

public static class Extensiones
{
    public static string Truncar(this string str, int max)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(max);

        char[] arr = str.ToCharArray();
        int len = arr.Length;

        if (len is 0 || Encoding.UTF8.GetByteCount(arr, 0, len) <= max)
        {
            return str;
        }
        do
        {
            arr[len - 1] = '…';
        }
        while (0 < len && max < Encoding.UTF8.GetByteCount(arr, 0, len--));
        return new string(arr, 0, ++len);
    }
}
