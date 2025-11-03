using System;

namespace TarjetaSube
{
    public class Tarjeta
    {
        private static int contadorId = 0;
        public int Id { get; private set; }
        protected int saldo;
        private const int LIMITE_SALDO = 40000;
        private const int LIMITE_NEGATIVO = -1200;
        private int[] CARGAS_VALIDAS = { 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000 };
        protected const int TARIFA_PASAJE = 1580;

        public int Saldo => saldo;

        public Tarjeta()
        {
            Id = ++contadorId;
            saldo = 0;
        }

        public int ObtenerSaldo()
        {
            return saldo;
        }

        public bool Cargar(int monto)
        {
            bool valido = false;
            foreach (var carga in CARGAS_VALIDAS)
            {
                if (monto == carga)
                {
                    valido = true;
                    break;
                }
            }

            if (!valido) return false;
            if (saldo + monto > LIMITE_SALDO) return false;

            saldo += monto;

            if (saldo > LIMITE_SALDO)
                saldo = LIMITE_SALDO;

            return true;
        }

        public virtual bool Descontar(int monto)
        {
            if (saldo - monto < LIMITE_NEGATIVO)
            {
                return false;
            }

            saldo -= monto;
            return true;
        }

        public virtual bool Pagar()
        {
            return Descontar(TARIFA_PASAJE);
        }
    }

    public class MedioBoleto : Tarjeta
    {
        public override bool Descontar(int monto)
        {
            int mitad = monto / 2;
            if (saldo - mitad < -1200) return false;
            saldo -= mitad;
            return true;
        }

        public override bool Pagar()
        {
            return Descontar(TARIFA_PASAJE);
        }
    }

    public class BoletoGratuito : Tarjeta
    {
        public override bool Descontar(int monto)
        {
            return true; 
        }

        public override bool Pagar()
        {
            return true;
        }
    }

    public class FranquiciaCompleta : Tarjeta
    {
        public override bool Descontar(int monto)
        {
            return true; 
        }

        public override bool Pagar()
        {
            return true;
        }
    }
}