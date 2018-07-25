using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Controladores
{
    public class DatetimeToDiaSemana
    {
        public static string Convertir(DateTime date)
        {
            switch (date.DayOfWeek)
            {
                case DayOfWeek.Monday:
                    return "LU";
                case DayOfWeek.Tuesday:
                    return "MA";
                case DayOfWeek.Wednesday:
                    return "MI";
                case DayOfWeek.Thursday:
                    return "JU";
                case DayOfWeek.Friday:
                    return "VI";
                case DayOfWeek.Saturday:
                    return "SA";
                case DayOfWeek.Sunday:
                default:
                    return "";
            }
        }
    }
}
