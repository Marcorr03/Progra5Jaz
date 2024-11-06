using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using System.Web.Hosting;
using System.Net.Http;


namespace Progra5Jaz.Models
{
    public class Datos
    {
        SqlConnection conexion;

        string stringConexion = "Data Source=tiusr21pl.cuc-carrera-ti.ac.cr\\MSSQLSERVER2019;Initial Catalog=ProyprogJaz;User ID=Jaz;Password=progra123;";

        private string Mensaje;

        public string Mensaje1 { get => Mensaje; set => Mensaje = value; }

        public void AbrirConex()
        {
            conexion = new SqlConnection(stringConexion);
            conexion.Open();
        }

        public void CerrarConex()
        {
            conexion.Close();
        }
        public class Location
        {
            public string Descripcion { get; set; }
            public int Pais { get; set; }
            public int Provincia { get; set; }
            public int Canton { get; set; }
            public int Distrito { get; set; }
        }


        //Escribir el json de ubicaciones
        public void EscribirJson()
        {
            // Obtener la ruta física del archivo
            string filePath = HostingEnvironment.MapPath("~/App_Data/locations.json");

            // Verificar si el archivo existe
            if (File.Exists(filePath))
            {
                string existingJson = System.IO.File.ReadAllText(filePath);
                if (!string.IsNullOrWhiteSpace(existingJson))
                {
                    // Deserializar JSON existente para verificar si tiene datos
                    var listaDatos = JsonConvert.DeserializeObject<List<Location>>(existingJson);
                    if (listaDatos != null && listaDatos.Count > 0)
                    {
                        Console.WriteLine("El archivo JSON ya contiene datos.");
                    }

                }
                else
                {
                    // Leer datos de la base de datos
                    var locations = LeerBd();

                    // Serializar a JSON
                    string json = JsonConvert.SerializeObject(locations, Formatting.Indented);

                    // Escribir JSON a un archivo
                    File.WriteAllText(filePath, json);

                    Console.WriteLine("JSON escrito en el archivo locations.json en la ruta: " + filePath);
                }
            }
        }

        //Ubicaciones
        private List<Location> LeerBd()
        {
            var locations = new List<Location>();

            AbrirConex();
            string sql = "SELECT Descripcion, ID_Pais, ID_Provincia, ID_Canton, ID_Distrito FROM Ubicaciones";

            using (SqlCommand command = new SqlCommand(sql, conexion))
            {
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var location = new Location
                        {
                            Descripcion = reader.GetString(0),
                            Pais = !reader.IsDBNull(1) ? reader.GetInt32(1) : 0,
                            Provincia = !reader.IsDBNull(2) ? reader.GetInt32(2) : 0,
                            Canton = !reader.IsDBNull(3) ? reader.GetInt32(3) : 0,
                            Distrito = !reader.IsDBNull(4) ? reader.GetInt32(4) : 0
                        };
                        locations.Add(location);
                    }
                }
            }
            CerrarConex();
            return locations;
        }

        // Clave y IV codificados en Base64
        byte[] key = Convert.FromBase64String("wM6v3Xe1h8c2N1A2m4H6eQ==");
        byte[] iV = Convert.FromBase64String("VbT1eL4UwYZ2JNdyUkgE4w==");
        public byte[] Key { get => key; set => key = value; }
        public byte[] IV { get => iV; set => iV = value; }


        //Encriptar
        public static byte[] Encriptar(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                    {
                        swEncrypt.Write(plainText);
                    }
                    encrypted = msEncrypt.ToArray();
                }
            }

            return encrypted;
        }

        //DESENCRIPTAR
        public string Desencriptar(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;

            using (Aes aes = Aes.Create())
            {
                aes.Key = Key;
                aes.IV = IV;

                ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                    {
                        plaintext = srDecrypt.ReadToEnd();
                    }
                }
            }

            return plaintext;
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
                    byte[] correoEncriptada = Encriptar(Correo, Key, IV);
                    command.Parameters.AddWithValue("@Correo", correoEncriptada);
                    byte[] contrasenaEncriptada = Encriptar(Contrasena, Key, IV);
                    command.Parameters.AddWithValue("@Contrasena", contrasenaEncriptada);
                    byte[] PalabraEncriptada = Encriptar(PalabraClave, Key, IV);
                    command.Parameters.AddWithValue("@PalabraClave", PalabraEncriptada);
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

        public class Usuario
        {
            public string Correo { get; set; }
            public string Contrasena { get; set; }
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
                byte[] correoEncriptada = Encriptar(Correo, Key, IV);
                command.Parameters.AddWithValue("@Correo", correoEncriptada);
                byte[] contrasenaEncriptada = Encriptar(Contrasena, Key, IV);
                command.Parameters.AddWithValue("@Contrasena", contrasenaEncriptada);

                object result = command.ExecuteScalar();
                CerrarConex();
                if (result != null)
                {
                    return true;
                 }
                }

                return false;
        }

        ///////////////////////////////////////////////////////
        public void CambioContra(string Email)
        {
            // Configuración del servidor SMTP y credenciales
            string smtpAddress = "smtp.gmail.com";
            int portNumber = 587;
            bool enableSSL = true;
            string emailFrom = "verificacion.hch@gmail.com";
            string password = "lectqauwhhyzfuds";
            string subject = "Recuperacion Cuenta";
            string body = @"
            <html>
            <body>
                <h1>Recuperación de Cuenta</h1>
                <p>Estimado usuario,</p>
                <p>Para recuperar tu cuenta, utiliza la siguiente contraseña temporal:</p>
                <p><strong>TEMPORARY_PASSWORD</strong></p>
                <p>Por favor, asegúrate de cambiar tu contraseña tan pronto como ingreses a tu cuenta.</p>
                <p>Gracias,</p>
                <p>El equipo de soporte</p>
            </body>
            </html>";

            string temporaryPassword = GenerarContra(14, 20);
            body = body.Replace("TEMPORARY_PASSWORD", temporaryPassword);
            ModContra(Email, temporaryPassword);
                using (MailMessage mail = new MailMessage())
                {
                    mail.From = new MailAddress(emailFrom, "Recuperacion", System.Text.Encoding.UTF8);
                    mail.To.Add(Email);
                    mail.Subject = subject;
                    mail.Body = body;
                    mail.IsBodyHtml = true; // Establecer en true si el cuerpo del correo es HTML
                    mail.Priority = MailPriority.Normal;
                    SmtpClient smtp = new SmtpClient();
                    smtp.UseDefaultCredentials = enableSSL;
                    smtp.Port = portNumber;
                    smtp.Host = smtpAddress;
                    smtp.Credentials = new NetworkCredential(emailFrom, password);
                    ServicePointManager.ServerCertificateValidationCallback = delegate
                        (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
                    { return true; };
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                Mensaje1 = "Se ha enviado una contraseña temporal a su Email";
            }
        }
        public void ModContra(string Email, string temporaryPassword)
        {
            AbrirConex();

            using (SqlCommand command = new SqlCommand("sp_Manejo_Contra", conexion))
            {

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@op", 1);
                byte[] encrypted1 = Encriptar(Email, Key, IV);
                byte[] encrypted2 = Encriptar(temporaryPassword, Key, IV);
                command.Parameters.AddWithValue("@Email", encrypted1);
                command.Parameters.AddWithValue("@Contra", encrypted2);

                SqlParameter sqlParameter = new SqlParameter("@Mensaje", SqlDbType.VarChar, 100)
                {
                    Direction = ParameterDirection.Output
                };

                command.Parameters.Add(sqlParameter);
                command.ExecuteNonQuery();
            }
            CerrarConex();
        }


        public void NewContra(string Email, string Contra, string Palabra, string NewContra)
        {
            AbrirConex();

            using (SqlCommand command = new SqlCommand("sp_Manejo_Contra", conexion))
            {

                command.CommandType = CommandType.StoredProcedure;
                command.Parameters.AddWithValue("@op", 2);
                byte[] encrypted1 = Encriptar(Email, Key, IV);
                byte[] encrypted2 = Encriptar(Palabra, Key, IV);
                byte[] encrypted3 = Encriptar(Contra, Key, IV);
                byte[] encrypted4 = Encriptar(NewContra, Key, IV);
                command.Parameters.AddWithValue("@Email", encrypted1);
                command.Parameters.AddWithValue("@Palabra", encrypted2);
                command.Parameters.AddWithValue("@Contra", encrypted3);
                command.Parameters.AddWithValue("@NewContra", encrypted4);
                SqlParameter sqlParameter = new SqlParameter("@Mensaje", SqlDbType.VarChar, 100)
                {
                    Direction = ParameterDirection.Output
                };

                command.Parameters.Add(sqlParameter);
                command.ExecuteNonQuery();

                Mensaje1 = sqlParameter.Value.ToString();

            }
            CerrarConex();
        }

        static string GenerarContra(int min, int max)
        {
            const string upperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
            const string lowerCase = "abcdefghijklmnopqrstuvwxyz";
            const string digits = "0123456789";
            const string specialChars = "!@#$%^&*()-_=+[]{}|;:,.<>?";

            Random random = new Random();
            int passwordLength = random.Next(min, max + 1);

            var passwordChars = new char[passwordLength];
            int currentIndex = 0;

            void AddRandomChars(string chars, int count)
            {
                for (int i = 0; i < count; i++)
                {
                    passwordChars[currentIndex++] = chars[random.Next(chars.Length)];
                }
            }

            AddRandomChars(upperCase, 4);
            AddRandomChars(lowerCase, 4);
            AddRandomChars(digits, 4);
            AddRandomChars(specialChars, 2);

            for (int i = currentIndex; i < passwordLength; i++)
            {
                string allChars = upperCase + lowerCase + digits + specialChars;
                passwordChars[i] = allChars[random.Next(allChars.Length)];
            }

            // Shuffle the array to ensure randomness
            var shuffledPassword = passwordChars.OrderBy(x => random.Next()).ToArray();

            return new string(shuffledPassword);
        }

        public void CorreoCodigo(string Email)
{
    // Configuración del servidor SMTP y credenciales
    string smtpAddress = "smtp.gmail.com";
    int portNumber = 587;
    bool enableSSL = true;
    string emailFrom = "verificacion.hch@gmail.com";
    string password = "lectqauwhhyzfuds";
    string subject = "Codigo de verificacion";
    string body = @"
            <html>
            <body>
                <h1>Codigo de verificacion</h1>
                <p>Estimado usuario,</p>
                <p>Para ingresar, utiliza el siguiente codigo:</p>
                <p><strong>TEMPORARY_PASSWORD</strong></p>
                <p>Ingrese este codigo en campo requerido para poner ingresar al sistema.</p>
                <p>Gracias,</p>
                <p>El equipo de soporte</p>
            </body>
            </html>";

    string codigo = GenerarCodigo();
    body = body.Replace("TEMPORARY_PASSWORD", codigo);
    EnviarCod(Email, codigo);using (MailMessage mail = new MailMessage())
        {
            mail.From = new MailAddress(emailFrom, "Codigo", System.Text.Encoding.UTF8);
            mail.To.Add(Email);
            mail.Subject = subject;
            mail.Body = body;
            mail.IsBodyHtml = true; // Establecer en true si el cuerpo del correo es HTML
            mail.Priority = MailPriority.Normal;
            SmtpClient smtp = new SmtpClient();
            smtp.UseDefaultCredentials = enableSSL;
            smtp.Port = portNumber;
            smtp.Host = smtpAddress;
            smtp.Credentials = new NetworkCredential(emailFrom, password);
            ServicePointManager.ServerCertificateValidationCallback = delegate
                (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
            { return true; };
            smtp.EnableSsl = true;
            smtp.Send(mail);
            Mensaje1 = "Se ha enviado un codigo de verificacion a su Email";
    }
}

    public void EnviarCod(string Email, string Codigo)
    {
        AbrirConex();

        using (SqlCommand command = new SqlCommand("sp_Cod", conexion))
        {

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@op", 1);
            byte[] encrypted1 = Encriptar(Email, Key, IV);
            byte[] encrypted2 = Encriptar(Codigo, Key, IV);
            command.Parameters.AddWithValue("@Email", encrypted1);
            command.Parameters.AddWithValue("@Codigo", encrypted2);

            SqlParameter sqlParameter = new SqlParameter("@Mensaje", SqlDbType.VarChar, 100)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(sqlParameter);
            command.ExecuteNonQuery();
        }
            CerrarConex();
    }


    public void RevisarCod(string Email, string Codigo)
    {
        AbrirConex();

        using (SqlCommand command = new SqlCommand("sp_Cod", conexion))
        {

            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@op", 2);
            byte[] encrypted1 = Encriptar(Email, Key, IV);
            byte[] encrypted2 = Encriptar(Codigo, Key, IV);
            command.Parameters.AddWithValue("@Email", encrypted1);
            command.Parameters.AddWithValue("@Codigo", encrypted2);

            SqlParameter sqlParameter = new SqlParameter("@Mensaje", SqlDbType.VarChar, 100)
            {
                Direction = ParameterDirection.Output
            };

            command.Parameters.Add(sqlParameter);
            command.ExecuteNonQuery();

            Mensaje1 = sqlParameter.Value.ToString();

        }
        CerrarConex();
    }
    static string GenerarCodigo()
    {
        using (RNGCryptoServiceProvider rng = new RNGCryptoServiceProvider())
        {
            byte[] randomNumber = new byte[1];
            string codigo = "";

            for (int i = 0; i < 5; i++)
            {
                rng.GetBytes(randomNumber);
                int digit = (randomNumber[0] % 10);
                codigo += digit.ToString();
            }
            return codigo;
        }
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