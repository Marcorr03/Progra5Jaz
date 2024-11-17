<?php
header("Content-Type: application/json");
header("Access-Control-Allow-Origin: *");
header("Access-Control-Allow-Methods: GET, POST, OPTIONS, DELETE, PUT");
header("Access-Control-Allow-Headers: Content-Type, Authorization");

require 'functions.php';

$method = $_SERVER['REQUEST_METHOD'];
$path = isset($_SERVER['PATH_INFO']) ? trim($_SERVER['PATH_INFO'], '/') : '';

switch ($method) {
    case 'GET':
        if ($path == 'lugares') {
                $lugares = LeerBD();
                if ($lugares['success']) {
                    $jsonResponse = json_encode($lugares['data']);
                    if ($jsonResponse === false) {
                        echo json_encode(["error" => "Error en la codificación JSON", "details" => json_last_error_msg()]);
                    } else {
                        http_response_code(200);
                        echo $jsonResponse;      
                    }
                }
                
            }
            else if($path == 'VerProductos'){
                $lugares = VerProductos();
                if ($lugares['success']) {
                    $jsonResponse = json_encode($lugares['data']);
                    if ($jsonResponse === false) {
                        echo json_encode(["error" => "Error en la codificación JSON", "details" => json_last_error_msg()]);
                    } else {
                        http_response_code(200);
                        echo $jsonResponse;
                    }
                }
            }
            else if($path == 'VerHabitaciones'){
                $lugares = VerHabitaciones();
                if ($lugares['success']) {
                    $jsonResponse = json_encode($lugares['data']);
                    if ($jsonResponse === false) {
                        echo json_encode(["error" => "Error en la codificación JSON", "details" => json_last_error_msg()]);
                    } else {
                        http_response_code(200);
                        echo $jsonResponse;
                    }
                }
            }
            else if ($path == 'VerReservasHabi') {
                $datos = $_GET['usuario'];
                $result = VerReservasHabi($datos);
                header('Content-Type: application/json');
                echo json_encode($result);
            }
            elseif($path == 'ApiBanco'){

                $response = Consulta();
                header('Content-Type: application/json');
            
            // Devolver los datos del clima como JSON
            echo $response;
            }
            else if($path=='ApiTSE'){
                $datos = $_GET['usuario'];
                $result = MostrarUsu($datos);
                header('Content-Type: application/json');
                echo json_encode($result);
            }
            else if($path=='ApiClima')
            {
            $apiKey ='08a95ded085c57b66e294e3b5e09e61e';
            $ciudad = 'Cartago,CR';
            global $apiKey;
            $url = "http://api.openweathermap.org/data/2.5/weather?q=" . urlencode($ciudad) . "&appid=" . $apiKey . "&units=metric&lang=es";

            // Inicializar cURL
            $ch = curl_init();

            // Configurar cURL
            curl_setopt($ch, CURLOPT_URL, $url);
            curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);

            // Ejecutar la solicitud y obtener la respuesta
            $respuesta = curl_exec($ch);

            // Verificar errores en la solicitud cURL
            if (curl_errno($ch)) {
                echo 'Error: ' . curl_error($ch);
                curl_close($ch);
                return false;
            }

            // Cerrar cURL
            curl_close($ch);

            // Decodificar la respuesta JSON
            $datosClima = json_decode($respuesta, true);

            header('Content-Type: application/json');

            if ($datosClima && isset($datosClima['weather'][0]['description']) && isset($datosClima['main']['temp'])) {
                $descripcion = $datosClima['weather'][0]['description'];
                $temperatura = $datosClima['main']['temp'];
                $resultado = [
                    'descripcion' => $descripcion,
                    'temperatura' => $temperatura
                ];
            } else {
                $resultado = ['error' => 'No se pudieron obtener los datos del clima.'];
            }
            
            // Configurar encabezado para devolver la respuesta en formato JSON
            header('Content-Type: application/json');
            
            // Devolver los datos del clima como JSON
            echo json_encode($resultado, JSON_PRETTY_PRINT);
                }
            else {
            http_response_code(404);
            echo json_encode(["error" => "Ruta no encontrada"]);    
        }
        break;

    case 'POST':
        if ($path == 'registrar') {
             // Obtener los datos del formulario
            $Ide= $_POST["dato1"];
            $Nombre= $_POST["dato2"];
            $Correo= $_POST["dato3"];
            $Telefono= $_POST["dato4"];
            $Contrasena= $_POST["dato5"];
            $FechaNa= $_POST["dato6"];
            $Vigencia= $_POST["dato7"];
            $PalabraClave= $_POST["dato8"];
                        
            // Llamar a la función login pasando los datos obtenidos
            $result = registrar($Ide,$Nombre,$Correo,$Telefono,$Contrasena,$FechaNa,$Vigencia,$PalabraClave);
            return $result;
         }  
         
         else if ($path == 'login') {
            // Obtener los datos del formulario
            if (isset($_POST["dato1"]) && isset($_POST["dato2"])) {
                $correo = $_POST["dato1"];
                $contrasena = $_POST["dato2"];
        
                // Llamar a la función login pasando los datos obtenidos
                $result = login($correo, $contrasena);
                return $result;
            } else {
                return ["success" => false, "error" => "Faltan parámetros de correo o contraseña"];
            }
        }
        // MostrarServicios
        else if ($path == 'Spa') {
            if (isset($_POST["dato1"])) {

                $idCategoria = $_POST["dato1"];
                // Llamar a la función para obtener los servicios
                $result = obtenerServiciosPorCategoria($idCategoria);

                // Devolver la respuesta como JSON
                return $result;
            } else {
                // Si falta el parámetro, devolver un error
                return ["success" => false, "error" => "Faltan parámetros de id"];
            }
        }
        else if ($path == 'EscribirP') {
            // Obtener los datos del formulario
           $datos = [];
           for ($i = 1; $i <= 4; $i++) {
               if (isset($_POST["dato$i"])) {
                   $datos[] = $_POST["dato$i"];
               } else {
                   return ["success" => false, "error" => "Faltan datos para 'dato$i'"];
               }
           }

           // Manejo del archivo de imagen subido
           if (isset($_FILES['Imagen']) && $_FILES['Imagen']['error'] == UPLOAD_ERR_OK) {
               $file = $_FILES['Imagen'];
           } else {
               return ["success" => false, "error" => "Archivo de imagen no subido o error al subir"];
           }

           // Llamar a la función EscribirH
           $result = EscribirP($datos, $file);

           return $result;
        }
         else if ($path == 'EscribirRH') {
            // Obtener los datos del formulario
   $datos = [];
   for ($i = 1; $i <= 5; $i++) {
       if (isset($_POST["dato$i"])) {
           $datos[] = $_POST["dato$i"];
       } else {
           return ["success" => false, "error" => "Faltan datos para 'dato$i'"];
       }
   }
   // Llamar a la función EscribirH
   $result = EscribirRH($datos);

   return $result;
        } else if ($path == 'ModificarRH') {
            // Obtener los datos del formulario
            $datos = [];
            for ($i = 1; $i <= 4; $i++) {
                if (isset($_POST["dato$i"])) {
                    $datos[] = $_POST["dato$i"];
                } else {
                    return ["success" => false, "error" => "Faltan datos para 'dato$i'"];
                }
            }
            // Llamar a la función ModificarRH
            $result = ModificarRH($datos);

            return $result;
        }else if ($path == 'AnularRH') {
            // Obtener los datos del formulario
            $datos = [];
            for ($i = 1; $i <= 4; $i++) {
                if (isset($_POST["dato$i"])) {
                    $datos[] = $_POST["dato$i"];
                } else {
                    return ["success" => false, "error" => "Faltan datos para 'dato$i'"];
                }
            }
            // Llamar a la función AnularRH
            $result = AnularRH($datos);

            return $result;
        }
    default:
        http_response_code(405);
        echo json_encode(["error" => "Método no permitido"]);
        break;
}
