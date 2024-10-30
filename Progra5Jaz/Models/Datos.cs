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


        public string Encriptar(string dato)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(dato));

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
        public string Registrar(string Ide, string Nombre, string Correo, string Telefono, string Contrasena, string FechaNa, string Vigencia, string PalabraClave)
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
                    command.Parameters.AddWithValue("@Telefono", Telefono);
                    command.Parameters.AddWithValue("@FechaNa", FechaNa);
                    command.Parameters.AddWithValue("@IdTipoUsu", 1);
                    command.Parameters.AddWithValue("@Vigencia", Vigencia);
                    string correoEncriptada = Encriptar(Correo);
                    command.Parameters.AddWithValue("@Correo", Correo);
                    string contrasenaEncriptada = Encriptar(Contrasena);
                    command.Parameters.AddWithValue("@Contrasena", Contrasena);
                    string PalabraEncriptada = Encriptar(PalabraClave);
                    command.Parameters.AddWithValue("@PalabraClave", PalabraClave);
                    command.Parameters.AddWithValue("@Estado", "Activo");

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


        //Vista Servicio Spa
        public DataTable Spa()
        {
            DataTable dt = new DataTable();

            AbrirConex();
            using (SqlCommand command = new SqlCommand())
            {
                // La consulta SQL para insertar los valores
                string sql = "select *from Servicios  ";

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


        //Vista Estetica
        public DataTable Estetica()
        {
            DataTable dt = new DataTable();

            AbrirConex();
            using (SqlCommand command = new SqlCommand())
            {
                // La consulta SQL para insertar los valores
                string sql = "Select * from Servicios  ";

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


        //Vista ManPed
        public DataTable ManPed()
        {
            DataTable dt = new DataTable();

            AbrirConex();
            using (SqlCommand command = new SqlCommand())
            {
                // La consulta SQL para insertar los valores
                string sql = "Select * from Servicios ";

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

        //Actividades
        //Registrar

        public string RegistrarActi(string Nombre, string Desc, string MinPer, string Precio, HttpPostedFileBase file)
        {
            string Mensaje = "";
            AbrirConex();
               
                using (SqlCommand command = new SqlCommand("GestionActividades", conexion))
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

                Mensaje = sqlParameter.Value.ToString();
            }

            CerrarConex();
            return Mensaje;
        }



        //Vista actividades
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



        //Gestion Promos
        //Registro 
        public string RegistrarPromo(string Promo, string Desc, string Precio, HttpPostedFileBase file)
        {
            string Mensaje = "";
            AbrirConex();
            using (SqlCommand command = new SqlCommand("GestionPromos", conexion))
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
                command.Parameters.AddWithValue("@Promo", Promo);
                command.Parameters.AddWithValue("@Descripcion", Desc);
                command.Parameters.AddWithValue("@Precio", Precio);
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

        //Vista promos
        public DataTable Promos()
        {
            DataTable dt = new DataTable();

            AbrirConex();
            using (SqlCommand command = new SqlCommand())
            {
                // La consulta SQL para insertar los valores
                string sql = "Select * from Promos ";

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

        public string EnvioMensaje(string Correo, string Descripcion)
        {
            string Mensaje = "";
            AbrirConex();
            using (SqlCommand command = new SqlCommand("GestionMensajes", conexion))
            {

                command.CommandType = CommandType.StoredProcedure;
                // Asignar los parámetros correspondientes
                command.Parameters.AddWithValue("@OP", 1);
                command.Parameters.AddWithValue("@Correo", Correo);
                command.Parameters.AddWithValue("@Descripcion", Descripcion);

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


    }
}