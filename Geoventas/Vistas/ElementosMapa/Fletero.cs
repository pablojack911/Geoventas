using GMap.NET;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Vistas.ElementosMapa
{
    public class Fletero : INotifyPropertyChanged
    {
        public Fletero()
        {
            this.Posiciones = new List<Posicion>();
            this.Clientes = new List<ClienteFletero>();
            this.Visible = true;
        }
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        private bool visible;

        public bool Visible
        {
            get { return visible; }
            set
            {
                visible = value;
                this.OnPropertyChanged("Visible");
            }
        }
        public bool VerClientes { get; set; }
        public PointLatLng CoordenadaActual { get; set; }
        public PointLatLng CoordenadaDomicilio { get; set; }
        //public string Calle { get; set; }
        //public string Numero { get; set; }
        public string Domicilio { get; set; }
        public string Foto { get; set; }
        private string fecha;

        public string Fecha
        {
            get { return fecha; }
            set
            {
                fecha = value;
                this.OnPropertyChanged("Fecha");
            }
        }

        private Estado estado;

        public Estado Estado
        {
            get { return estado; }
            set
            {
                estado = value;
                this.OnPropertyChanged("Estado");
            }
        }

        private int visitados;
        public int Visitados
        {
            get { return visitados; }
            set
            {
                visitados = value;
                OnPropertyChanged("Visitados");
            }
        }
        public bool VerTodasLasPosiciones { get; set; }
        public bool VerDomicilioDelFletero { get; set; }
        public List<Posicion> Posiciones { get; set; }
        public List<ClienteFletero> Clientes { get; set; }
        private int cantidadClientes;

        public int CantidadClientes
        {
            get { return cantidadClientes; }
            set
            {
                cantidadClientes = value;
                OnPropertyChanged("CantidadClientes");
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this,
                  new PropertyChangedEventArgs(propertyName));
            }
        }
        public override string ToString()
        {
            return this.Codigo + " - " + this.Nombre;
        }
    }
}
