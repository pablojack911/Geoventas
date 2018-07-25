namespace MatrizDeAhorroWPF
{
    public class Cliente : Coordenada
    {
        public string Codigo { get; set; }
        //public double Lat { get; set; }
        //public double Lng { get; set; }
        public double DistanciaDeHergo { get; set; }
        //public bool Libre { get; set; }

        public override string ToString()
        {
            return this.Codigo;
        }
    }
}