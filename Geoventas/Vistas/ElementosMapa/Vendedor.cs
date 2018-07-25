using GMap.NET;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeoventasPocho.Vistas.ElementosMapa
{
    public class Vendedor : INotifyPropertyChanged
    {
        public string Codigo { get; set; }
        public string Nombre { get; set; }
        public string Foto { get; set; }
        public string CodigoEmpresa { get; set; }
        public string CodigoDivision { get; set; }
        public PointLatLng CoordenadaActual { get; set; }
        public PointLatLng CoordenadaDomicilio { get; set; }
        public string Calle { get; set; }
        public string Numero { get; set; }
        public bool Presente { get; set; }
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

        private int compradores;
        public int Compradores
        {
            get { return compradores; }
            set
            {
                compradores = value;
                OnPropertyChanged("Compradores");
            }
        }

        private int bultos;
        public int Bultos
        {
            get { return bultos; }
            set
            {
                bultos = value;
                OnPropertyChanged("Bultos");
            }
        }

        private decimal pesos;
        public decimal Pesos
        {
            get { return pesos; }
            set
            {
                pesos = value;
                OnPropertyChanged("Pesos");
            }
        }

        //private List<Cliente> clientes;
        //public List<Cliente> Clientes
        //{
        //    get { return clientes; }
        //    set
        //    {
        //        clientes = value;
        //        OnPropertyChanged("Clientes");
        //    }
        //}
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


        private ObservableCollection<Zona> zonas;

        public ObservableCollection<Zona> Zonas
        {
            get { return zonas; }
            set
            {
                zonas = value;
                this.OnPropertyChanged("Zonas");
                this.OnPropertyChanged("ZonasClienteParaGrilla");
            }
        }


        private string zonasClienteParaGrilla;

        public string ZonasClienteParaGrilla
        {
            get { return zonasClienteParaGrilla; }
            set
            {
                zonasClienteParaGrilla = value;
                this.OnPropertyChanged("ZonasClienteParaGrilla");
            }
        }


        public bool VerClientes { get; set; }

        public bool VerZona { get; set; }

        public bool VerTodasLasPosiciones { get; set; }

        public bool VerDomicilioDelVendedor { get; set; }

        /// <summary>
        /// prop se setea true cuando ya tiene las listas cargadas.
        /// </summary>
        public bool CargadoPorCompleto { get; set; }

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

        public string FondoDeCelda { get; set; }

        public List<Posicion> Posiciones { get; set; }

        public Vendedor()
        {
            this.zonas = new ObservableCollection<Zona>();
            this.zonas.CollectionChanged += Zonas_CollectionChanged;
            this.Posiciones = new List<Posicion>();
            this.Visible = true;
        }

        void Zonas_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this.zonasClienteParaGrilla = this.ZonasClienteParaGrilla = string.Join("/", this.Zonas.Select(i => i.Codigo));
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
            return this.Codigo + " - " + this.Nombre + " | " + this.ZonasClienteParaGrilla;
        }
    }
}
