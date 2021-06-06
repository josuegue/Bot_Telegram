using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;

namespace Bot_Exa_Comida.Clsconexionmysql
{
    class ClsConexion
    {
        //variables gobales
        public MySqlConnection conexion;
        private string Str_conexion { get; }
        private long Id_Usuario;
        private string Nombre_usuario;
        private string comida;

        //Constructor 
        public ClsConexion(long idusuario, string nombre, string orden)
        {
            this.Str_conexion = "Database=comidas; Data Source=localhost; User Id=root; Password=12345678;";
            this.Id_Usuario = idusuario;
            this.comida = orden;
            this.Nombre_usuario = nombre;
        }

        //Apertura de la base de datos
        public void AbrirMySql()
        {
            try
            {
                this.conexion = new MySqlConnection(this.Str_conexion);
                conexion.Open();
            }
            catch (MySqlException er)
            {
                Console.WriteLine($"Error de conexion a base de datos: {er}");
            }
        }

        //Cierre de base de datos 
        public void CerrarMysql()
        {
            this.conexion.Close();

        }
        //Insercion y Actuaizacion de datos.
        public DataTable CrudMysql(string string_ejecutar)
        {
            AbrirMySql();
            DataTable tabla = new DataTable();
            try
            {
                MySqlCommand comand = new MySqlCommand(string_ejecutar, this.conexion);
                tabla.Load(comand.ExecuteReader());
                return tabla;
            }
            catch (MySqlException er)
            {
                Console.WriteLine($"Error al hacer ejecucion de comando: {er}");
                return tabla;
            }
            finally
            {
                CerrarMysql();
            }
           
        }


        //Se hace el pedido
        public DataTable Hacer_pedido()
        {
            string insertar = $"INSERT INTO pedidos (id,nombre, comida, bebida, fecha_pedido, precio) VALUE ({this.Id_Usuario},'{this.Nombre_usuario}' ,'{this.comida}', 'Coca cola', NOW(),65);";
            return this.CrudMysql(insertar);
        }
        //Se hace la consulta
        public DataTable Consulta_pedido()
        {
            string consulta = $"SELECT *FROM pedidos WHERE id={this.Id_Usuario};";
            return this.CrudMysql(consulta);
        }
       
        //Se elimina el pedido
        public DataTable Eliminar_pedido()
        {
            string eliminar = $"DELETE FROM pedidos WHERE id={this.Id_Usuario};";
            return this.CrudMysql(eliminar);
        }

    }
}
