using System;

namespace bdJuegos
{
    class Program
    {
        static void Main(string[] args)
        {
            AccesoJuegos dbjuegos=new AccesoJuegos();
            dbjuegos.getSQLiteVersion();
            dbjuegos.creaDB();

            dbjuegos.nuevoJuego("Resident Evil VIII", "Capcom", "2021");
            dbjuegos.nuevoJuego("Super Mario 64", "Nintendo", "1996");

            dbjuegos.muestraDB();

            Console.WriteLine("--Consulta parametrizada--");
            dbjuegos.consultaCompañia("Nintendo");

        }
    }
}
