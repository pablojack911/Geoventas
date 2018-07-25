using GeoventasPocho.DAO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ZonasPorKml
{
    public class ControladorZonasVertices
    {
        public static void BorrarZona(string codigo, List<string> empresas)
        {
            //var codigoZona = codigo; //descomentar para ref
            var subempresa = string.Empty;
            var codigoZona = string.Empty;
            foreach (var empresa in empresas)
            {
                if (empresa == "10")
                {
                    codigoZona = "A" + codigo;
                    subempresa = "00ALI";
                }
                else
                {
                    codigoZona = "H" + codigo;
                    subempresa = "00BEB";
                }
                try
                {
                    var command = FoxDB.Instancia.CrearComando(string.Format("delete from zonas_vertices where zona='{0}'", codigoZona));
                    FoxDB.Instancia.Conectar();
                    command.ExecuteNonQuery();
                    FoxDB.Instancia.Desconectar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public static void InsertarZona(Zona zona, List<string> empresas)
        {
            var subempresa = string.Empty;
            //var subempresa = "00REF";
            //var codigoZona = zona.Codigo;
            var codigoZona = string.Empty;
            foreach (var empresa in empresas)
            {
                if (empresa == "10")
                {
                    codigoZona = "A" + zona.Codigo;
                    subempresa = "00ALI";
                }
                else
                {
                    codigoZona = "H" + zona.Codigo;
                    subempresa = "00BEB";
                }

                try
                {
                    var command = FoxDB.Instancia.CrearComando(string.Format("insert into zonas_vertices(zona,empresa,subempresa,latitud,longitud) values('{0}','{1}','{2}',{3},{4})", codigoZona, empresa, subempresa, zona.Lat, zona.Lng));
                    FoxDB.Instancia.Conectar();
                    command.ExecuteNonQuery();
                    FoxDB.Instancia.Desconectar();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }
    }
}
