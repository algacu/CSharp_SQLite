using System.IO;
using System;

using System.Data.SQLite;

namespace bdJuegos 
{
    public class AccesoJuegos 
    {
        public string dbFile {get;}
        public AccesoJuegos() : this("videojuegos.db"){}

        public AccesoJuegos(String dbname){
            //Inicializamos el fichero de la BD
            this.dbFile = dbname;
        }


        //Para hacer una consulta, debemos hacerla sobre una base de datos. En este método, no vamos a
        //utilizar todavía la base de datos de videojuegos, sino que vamos a utilizar una base de datos en memoria
        public void getSQLiteVersion(){

            //Cadena de conexión a la bd en memoria
            string cs = "Data Source =:memory:";
            using var con = new SQLiteConnection(cs);
            con.Open();

            //Preparamos la consulta (Statement)
            string stm = "SELECT SQLITE_VERSION()";

            //Preparamos el command
            using var cmd = new SQLiteCommand(stm, con);

            //Y llo lanzamos
            string version = cmd.ExecuteScalar().ToString();

            //Finalmente, mostramos el resultado
            Console.WriteLine($"SQLite version: {version}");

            //Quedaría cerrar la conexión a la base de datos, cosa que no necesitamos hacer ya que
            //se cierra automáticamente al utilizar using.
        }


        //Creación y poblado de la BD. Utilizaremos sentencias SQL que ejecutaremos sobre un Command.
        public void creaDB(){
            //Creamos una conexión a la BD. En este caso, primero creamos un string que una ruta de acceso 
            //al fichero database en el directorio actual.
            string cs = @"URI=file:" + Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + this.dbFile;

            //Esta cadena de conexión será básicamente la única diferencia respecto a si no conectásemos
            //a otro motor de BD. Por ejemplo, si fuese una conexión a MySQL, tendría el siguiente aspecto:
            //string cs = @"server=localhost;userid=usuario;password=pass;
            //database=bdtest";
            //Donde indicaríamos que se trata de un servidor local, que el usuario de la base de datos es "usuario",
            //su contraseña "pass" y la base de datos por defecto "bdtest"

            //Con la cadena ya preparada, creamos la conexión a la BD
            using var con = new SQLiteConnection(cs);
            con.Open();

            //Ahora, con esta conexión vamos a lanzar las sentencias para crear la base de datos. Para ello,
            //utilizaremos el método ExecuteNonQuery del objeto ADO Command, utilizado para modificar la BD.
            //En este caso, como vamos a lanzar varias sentencias, inicializamos el Command únicamente con la
            //conexión (sin añadirle una consulta):
            using var cmd = new SQLiteCommand(con);

            //Con este cmd definimos la consulta con commandText y la lanzamos con ExecuteNonQuery (consulta de actualización, que modifica la bd pero no devuelve nada). 
            //En primer lugar, comprobamos si existe la tabla videojuegos y la borramos:
            cmd.CommandText = "DROP TABLE IF EXISTS videojuegos";
            cmd.ExecuteNonQuery();

            //Y en segundo lugar la creamos. Al indicar el id INTEGER PRIMARY KEY indicamos cuál es la clave primaria
            //y que ésta se incrementará automáticamente en SQLite:
            cmd.CommandText = @"CREATE TABLE videojuegos (id INTEGER PRIMARY KEY, nombre TEXT, compañia TEXT, año INT)";
            cmd.ExecuteNonQuery();

            //Añadiendo datos. Para hacer inserts a la BD, sobre el mismo objeto Command definido, vamos variando
            //el commandText con los inserts y lanzándolos:
            cmd.CommandText= @"INSERT INTO videojuegos (nombre, compañia, año) VALUES ('The Legend of Zelda: Ocarina of Time',
            'Nintendo', 1998)";
            cmd.ExecuteNonQuery();
            cmd.CommandText= @"INSERT INTO videojuegos (nombre, compañia, año) VALUES ('Final Fantasy VII', 'Squaresoft', 1997)";
            cmd.ExecuteNonQuery();
            //etc.
        }


        //Método para consultar a la BD y mostrar su contenido
        public void muestraDB(){

            //Creamos la conexión
            string cs = @"URI=file:" + Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + this.dbFile;
            using var con = new SQLiteConnection(cs);
            con.Open();

            //Creamos la sentencia SELECT como un string y se la proporcionamos al SQLiteCommand en su creación,
            //ya que únicamente vamos a realizar una consulta

            //Definimos el string
            string stm = "SELECT * FROM videojuegos LIMIT 10";

            //Creamos el objeto SQLiteCommand para realizar la consulta SQL
            using var cmd = new SQLiteCommand(stm, con);

            //Invocamos al método ExecuteReader de la sentencia para obtener un objeto de tipo Reader sobre el que
            //recuperar los diferentes registros de la consulta. En este caso, utilizaremos también using, ya que
            //el Reader necesitará cerrarse cuando terminemos de usarlo.
            using SQLiteDataReader reader = cmd.ExecuteReader();

            //El objeto reader contendrá el SQLiteDataReader devuelto por la consulta. Será una especie
            //de tabla, que contendrá filas y columnas accesibles mediante un cursor. Además, también tendrá
            //información sobre las cabeceras de la tabla.

            //En primer lugar vamos a mostar esta cabecera de la tabla, con los nombres de los campos.
            //Para ello, vamos a utilizar el método reader.getName, al que le proporcionamos el índice de la columna:
            Console.WriteLine($"{reader.GetName(0), -3} {reader.GetName(1), -40} {reader.GetName(2), -15} {reader.GetName(3), -4}");

            //Por su parte, para leer los resultados, utilizaremos el método Read, que avanzará el cursor de datos al siguiente registro disponible.
            //Este método devolverá true si quedan más filas en el resultado y false si ya no quedan más.
            //Dentro de este Read, podemos acceder a los datos usuando un índice de vector, o bien accediendo a los valores de columna en los tipos de datos nativos,
            //mediante los métodos GetInt32 o GetString para INTEGER y TEXT
            //La lectura se realizaría del siguiente modo:
            while (reader.Read()){
                Console.WriteLine($"{reader.GetInt32(0), -3} {reader.GetString(1), -40} {reader.GetString(2), -15} {reader.GetInt32(3), -4}");
            }

        }

        //Método para añadir un nuevo juego a la base de datos, a partir de los parámetros introducidos durante la llamada a la función en Program.cs
        public void nuevoJuego(string nombre, string compañia, string año){
            
            try {
            //Creamos una conexión a la BD. En este caso, primero creamos un string que una ruta de acceso 
            //al fichero database en el directorio actual.
            string cs = @"URI=file:" + Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + this.dbFile;

            //Con la cadena ya preparada, creamos la conexión a la BD
            using var conexion = new SQLiteConnection(cs);
            conexion.Open();

            //Ahora, con esta conexión vamos a lanzar las sentencias para crear la base de datos. Para ello,
            //utilizaremos el método ExecuteNonQuery del objeto ADO Command, utilizado para modificar la BD.
            //En este caso, como vamos a lanzar varias sentencias, inicializamos el Command únicamente con la
            //conexión (sin añadirle una consulta):
            using var comando = new SQLiteCommand(conexion);

            //Introducimos datos mediante el uso de Prepared Statements o Sentencias Preparadas
            comando.CommandText = @"INSERT INTO videojuegos (nombre, compañia, año) VALUES (@nombre, @compañia, @año)";
            comando.Parameters.AddWithValue("@nombre", nombre);
            comando.Parameters.AddWithValue("@compañia", compañia);
            comando.Parameters.AddWithValue("@año", año);

            comando.Prepare();
            comando.ExecuteNonQuery();
            } catch (SQLiteException err){
                Console.WriteLine("Error de SQL: "+err.Message);
            }
            
        }

        //Método para parametrizar las consultas por compañía
        public void consultaCompañia(string compañia){

            //Establecemos la conexión con la DB
            string cs = @"URI=file:" + Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + this.dbFile;
            using var conexion = new SQLiteConnection(cs);
            conexion.Open();

            //Creamos la sentencia SELECT como un string y se la proporcionamos al SQLiteCommand en su creación,
            //ya que únicamente vamos a realizar una consulta

            //Definimos el string
            string stm = "SELECT * FROM videojuegos WHERE compañia=@nombrecompañia LIMIT 10";

            //Creamos el objeto SQLiteCommand para realizar la consulta SQL
            using var comando = new SQLiteCommand(stm, conexion);

            //Añadimos valor a los parámetros
            comando.Parameters.AddWithValue("@nombrecompañia", compañia);

            //Preparamos la consulta
            comando.Prepare();

            //Invocamos al método ExecuteReader de la sentencia para obtener un objeto de tipo Reader sobre el que
            //recuperar los diferentes registros de la consulta. En este caso, utilizaremos también using, ya que
            //el Reader necesitará cerrarse cuando terminemos de usarlo.
            using SQLiteDataReader reader = comando.ExecuteReader();

            //El objeto reader contendrá el SQLiteDataReader devuelto por la consulta. Será una especie
            //de tabla, que contendrá filas y columnas accesibles mediante un cursor. Además, también tendrá
            //información sobre las cabeceras de la tabla.

            //En primer lugar vamos a mostar esta cabecera de la tabla, con los nombres de los campos.
            //Para ello, vamos a utilizar el método reader.getName, al que le proporcionamos el índice de la columna:
            Console.WriteLine($"{reader.GetName(0), -3} {reader.GetName(1), -40} {reader.GetName(2), -15} {reader.GetName(3), -4}");

            //Por su parte, para leer los resultados, utilizaremos el método Read, que avanzará el cursor de datos al siguiente registro disponible.
            //Este método devolverá true si quedan más filas en el resultado y false si ya no quedan más.
            //Dentro de este Read, podemos acceder a los datos usuando un índice de vector, o bien accediendo a los valores de columna en los tipos de datos nativos,
            //mediante los métodos GetInt32 o GetString para INTEGER y TEXT
            //La lectura se realizaría del siguiente modo:
            while (reader.Read()){
                Console.WriteLine($"{reader.GetInt32(0), -3} {reader.GetString(1), -40} {reader.GetString(2), -15} {reader.GetInt32(3), -4}");
            }

        }

    }

}