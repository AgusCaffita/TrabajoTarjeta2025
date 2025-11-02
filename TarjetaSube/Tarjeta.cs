using System;

namespace TarjetaSube
{
    public class Tarjeta
    {
        private int saldo;
        private const int LIMITE_SALDO = 40000;
        private const int LIMITE_NEGATIVO = -1200; // Nuevo límite de saldo negativo
        private int[] CARGAS_VALIDAS = { 2000, 3000, 4000, 5000, 8000, 10000, 15000, 20000, 25000, 30000 };
        private const int TARIFA_PASAJE = 1580;

        public int Saldo => saldo;

        public Tarjeta()
        {
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

            // Si el saldo es negativo, al cargar se descuenta automáticamente la deuda
            saldo += monto;

            if (saldo > LIMITE_SALDO)
                saldo = LIMITE_SALDO;

            return true;
        }

        public bool Descontar(int monto)
        {
            // Permitir saldo negativo hasta el límite
            if (saldo - monto < LIMITE_NEGATIVO)
            {
                return false;
            }

            saldo -= monto;
            return true;
        }

        public bool Pagar()
        {
            return Descontar(TARIFA_PASAJE);
        }
    }
}