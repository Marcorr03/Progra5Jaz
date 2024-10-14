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

        string stringConexion = "Data Source=tiusr21pl.cuc-carrera-ti.ac.cr\\MSSQLSERVER2019;Initial Catalog=ProyprogJaz;User ID=Jaz;Password=progra123;";


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

        //Registro

        //Gestion Usuarios
        public string Registrar(string Ide, string Nombre, string Correo, string Telefono, string Contrasena, string FechaNa)
        {
            AbrirConex();
            string mesage= "";
                // Asignar la consulta SQL al SqlCommand
                using (SqlCommand command = new SqlCommand("GestionUsuarios", conexion))
                {

                    command.CommandType = CommandType.StoredProcedure;
                    // Asignar los parámetros correspondientes
                    command.Parameters.AddWithValue("@OP", 1);
                    command.Parameters.AddWithValue("@Identificacion", Ide);
                    command.Parameters.AddWithValue("@Nombre", Nombre);
                    command.Parameters.AddWithValue("@Correo", Correo);
                    command.Parameters.AddWithValue("@Telefono", Telefono);
                    //string contrasenaEncriptada = EncriptarContrasena(Contrasena);
                    command.Parameters.AddWithValue("@Contrasena", Contrasena);
                    command.Parameters.AddWithValue("@FechaNa", FechaNa);
                    command.Parameters.AddWithValue("@IdTipoUsu", 1);//cambiar con roles
                    SqlParameter sqlParameter = new SqlParameter("@Mensaje", SqlDbType.VarChar, 100)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(sqlParameter);
                    // Ejecutar la consulta
                    command.ExecuteNonQuery();
                    mesage=sqlParameter.Value.ToString();
            }
            
            CerrarConex();
            return mesage;
        }

        //Login
        public bool login(string Correo, string Contrasena)
        {
            AbrirConex();
            using (SqlCommand command = new SqlCommand())
            {
                // La consulta SQL para insertar los valores
                string sql = "Select Correo,Contrasena from Usuarios where Correo= @Correo and Contrasena=@Contrasena";

                // Asignar la consulta SQL al SqlCommand
                command.CommandText = sql;
                command.Connection = conexion;

                // Asignar los parámetros correspondientes
                command.Parameters.AddWithValue("@Correo", Correo);
                command.Parameters.AddWithValue("@Contrasena", Contrasena);

                object result = command.ExecuteScalar();
                CerrarConex();
                if (result != null)
                {
                    return true;
                 }
                }

                return false;
        }


        //Servicios
        //Registrar
        public string RegistrarServ(string Nombre, string Desc, string Precio,string IdCategoriaSer, HttpPostedFileBase file)
        {
            string Mensaje = "";
            AbrirConex();
            using (SqlCommand command = new SqlCommand("GestionServicios", conexion))
            {

                command.CommandType = CommandType.StoredProcedure;
                // Asignar los parámetros correspondientes
                command.Parameters.AddWithValue("@OP", 1);


                byte[] imagenBytes;
                using (var stream = new MemoryStream())
                {
                    file.InputStream.CopyTo(stream);
                    imagenBytes = stream.ToArray();
                }
                command.Parameters.AddWithValue("@Nombre", Nombre);
                command.Parameters.AddWithValue("@Descripcion", Desc);
                command.Parameters.AddWithValue("@Precio", Precio);
                command.Parameters.AddWithValue("@IdCategoriaSer", IdCategoriaSer);
                command.Parameters.AddWithValue("@Imagen", SqlDbType.Image).Value = imagenBytes;

                SqlParameter sqlParameter = new SqlParameter("@Mensaje", SqlDbType.VarChar, 100)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(sqlParameter);

                command.ExecuteNonQuery();

              Mensaje = sqlParameter.Value.ToString();
            }
            
            CerrarConex();
            return Mensaje;
        }


        //Actividades
        //Registrar

        public void RegistrarActi(string Nombre, string Desc, string MinPer, string Precio, HttpPostedFileBase file)
        {
            AbrirConex();
            using (SqlCommand command = new SqlCommand())
            {
                // La consulta SQL para insertar los valores
                string sql = "Exec GestionActividades 1, @Imagen, @Actividad, @Descripcion, @CantPersonas, @Precio,@Mensaje";

                // Asignar la consulta SQL al SqlCommand
                command.CommandText = sql;
                command.Connection = conexion;


                byte[] imagenBytes;
                using (var stream = new MemoryStream())
                {
                    file.InputStream.CopyTo(stream);
                    imagenBytes = stream.ToArray();
                }
                command.Parameters.AddWithValue("@Actividad", Nombre);
                command.Parameters.AddWithValue("@Descripcion", Desc);
                command.Parameters.AddWithValue("@CantPersonas", MinPer);
                command.Parameters.AddWithValue("@Precio", Precio);
                command.Parameters.AddWithValue("@Imagen", SqlDbType.Image).Value = imagenBytes;

                SqlParameter sqlParameter = new SqlParameter("@Mensaje", SqlDbType.VarChar, 100)
                {
                    Direction = ParameterDirection.Output
                };
                command.Parameters.Add(sqlParameter);

                command.ExecuteNonQuery();

                string Mensaje1 = sqlParameter.Value.ToString();
            }

            CerrarConex();
        }


        //Todas actividades
        public DataTable Actividades() 
        {
            DataTable dt = new DataTable();
            
            AbrirConex();
            using (SqlCommand command = new SqlCommand())
            {
                // La consulta SQL para insertar los valores
                string sql = "Select * from Actividades ";

                // Asignar la consulta SQL al SqlCommand
                command.CommandText = sql;
                command.Connection = conexion;

                using (SqlDataAdapter da = new SqlDataAdapter(command))
                {
                    da.Fill(dt); 
                }
                CerrarConex();
                return dt;
            }
        }



        //Todas modificar   HACER
        public object ModActividades(string Imagen, string Actividad, string Descripcion, string MinPer, string Precio)
        {
            DataTable dt = new DataTable();

            AbrirConex();
            using (SqlCommand command = new SqlCommand())
            {
                // La consulta SQL para insertar los valores
                string sql = "Select * from Actividades ";

                // Asignar la consulta SQL al SqlCommand
                command.CommandText = sql;
                command.Connection = conexion;

                using (SqlDataAdapter da = new SqlDataAdapter(command))
                {
                    da.Fill(dt);
                }
                CerrarConex();
                return dt;
            }
        }


    }
}