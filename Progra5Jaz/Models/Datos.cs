using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;


namespace Progra5Jaz.Models
{
    public class Datos
    {
        SqlConnection conexion;

        string stringConexion = "Data Source=DESKTOP-3G9V4FR;Initial Catalog=ProyprogJaz;Integrated Security=True;";


        public void AbrirConex()
        {
            conexion = new SqlConnection(stringConexion);
            conexion.Open();
        }

        public void CerrarConex()
        {
            conexion.Close();
        }


        public string EncriptarContrasena(string contrasena)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(contrasena));

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public void Registrar(string Ide, string Nombre, string Correo, string Telefono, string Contrasena)
        {
            AbrirConex();
            using (SqlCommand command = new SqlCommand())
            {

                // La consulta SQL para insertar los valores
                string sql = "INSERT INTO Usuarios (Identificacion, Nombre, Correo, Telefono, Contraseña) VALUES (@Identificacion, @Nombre, @Correo, @Telefono, @Contrasena)";

                // Asignar la consulta SQL al SqlCommand
                command.CommandText = sql;
                command.Connection = conexion;

                // Asignar los parámetros correspondientes
                command.Parameters.AddWithValue("@Identificacion", Ide);  
                command.Parameters.AddWithValue("@Nombre", Nombre);  
                command.Parameters.AddWithValue("@Correo", Correo);  
                command.Parameters.AddWithValue("@Telefono", Telefono);
                //string contrasenaEncriptada = EncriptarContrasena(Contrasena);
                command.Parameters.AddWithValue("@Contrasena", Contrasena);

                // Ejecutar la consulta
                command.ExecuteNonQuery();
            }
            CerrarConex();
        }

        public bool login(string Correo, string Contrasena)
        {
            AbrirConex();
            using (SqlCommand command = new SqlCommand())
            {
                // La consulta SQL para insertar los valores
                string sql = "Select Correo,Contraseña from Usuarios where Correo= @Correo and Contraseña=@Contrasena";

                // Asignar la consulta SQL al SqlCommand
                command.CommandText = sql;
                command.Connection = conexion;

                // Asignar los parámetros correspondientes
                command.Parameters.AddWithValue("@Correo", Correo);
                command.Parameters.AddWithValue("@Contrasena", Contrasena);

                // Ejecutar la consulta
                command.ExecuteNonQuery();

                int count = (int)command.ExecuteScalar();  

                CerrarConex(); 

                if( count > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

        }


    }
}