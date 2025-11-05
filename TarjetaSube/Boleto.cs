using System;

namespace TarjetaSube
{
    public class Boleto
    {
        public string TipoTarjeta { get; private set; }
        public string LineaColectivo { get; private set; }
        public int TotalAbonado { get; private set; }
        public int SaldoRestante { get; private set; }
        public DateTime Fecha { get; private set; }
        public int IdTarjeta { get; private set; }
        public bool EsTrasbordo { get; private set; }

        public Boleto(string tipoTarjeta, string lineaColectivo, int totalAbonado, int saldoRestante, int idTarjeta)
            : this(tipoTarjeta, lineaColectivo, totalAbonado, saldoRestante, idTarjeta, new Tiempo())
        {
        }

        public Boleto(string tipoTarjeta, string lineaColectivo, int totalAbonado, int saldoRestante, int idTarjeta, Tiempo tiempo)
            : this(tipoTarjeta, lineaColectivo, totalAbonado, saldoRestante, idTarjeta, tiempo, false)
        {
        }

        public Boleto(string tipoTarjeta, string lineaColectivo, int totalAbonado, int saldoRestante, int idTarjeta, Tiempo tiempo, bool esTrasbordo)
        {
            TipoTarjeta = tipoTarjeta;
            LineaColectivo = lineaColectivo;
            TotalAbonado = totalAbonado;
            SaldoRestante = saldoRestante;
            Fecha = tiempo.Now();
            IdTarjeta = idTarjeta;
            EsTrasbordo = esTrasbordo;
        }
    }
}