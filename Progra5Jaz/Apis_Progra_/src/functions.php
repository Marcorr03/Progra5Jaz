<?php
require 'config.php';

function Consulta(){
    // Definir el endpoint y tu token de acceso
$endpoint = "https://gee.bccr.fi.cr/Indicadores/Suscripciones/WS/wsIndicadoresEconomicos.asmx/ObtenerIndicadoresEconomicos";
$token = "AACMR0R8ZU"; // Reemplaza con tu token

// Configurar los parámetros de la solicitud
$params_buy = [
    'Indicador' => '317', // Tipo de cambio de compra
    'FechaInicio' => date('d/m/Y'),
    'FechaFinal' => date('d/m/Y'),
    'Nombre' => 'Marco Rodriguez',
    'SubNiveles' => 'N',
    'CorreoElectronico' => 'marcoanrr0314@gmail.com',
    'Token' => $token
];

$params_sell = [
    'Indicador' => '318', // Tipo de cambio de venta
    'FechaInicio' => date('d/m/Y'),
    'FechaFinal' => date('d/m/Y'),
    'Nombre' => 'Marco Rodriguez',
    'SubNiveles' => 'N',
    'CorreoElectronico' => 'marcoanrr0314@gmail.com',
    'Token' => $token
];

function fetch_exchange_rate($url, $params) {
    $url = $url . '?' . http_build_query($params);

    // Inicializar cURL
    $ch = curl_init();

    // Configurar cURL
    curl_setopt($ch, CURLOPT_URL, $url);
    curl_setopt($ch, CURLOPT_RETURNTRANSFER, true);

    // Ejecutar la solicitud y obtener la respuesta
    $response = curl_exec($ch);

    // Verificar errores en la solicitud
    if (curl_errno($ch)) {
        echo 'Error: ' . curl_error($ch);
        curl_close($ch);
        return false;
    }

    // Cerrar cURL
    curl_close($ch);

    return $response;
}

$response_buy = fetch_exchange_rate($endpoint, $params_buy);
$response_sell = fetch_exchange_rate($endpoint, $params_sell);

function parse_exchange_rate($response) {
    if ($response === false) {
        return null;
    }

    libxml_use_internal_errors(true);
    $xml = simplexml_load_string($response);

    if ($xml === false) {
        echo "Error al procesar el XML.";
        foreach (libxml_get_errors() as $error) {
            echo "\t", $error->message;
        }
        libxml_clear_errors();
        return null;
    }

    // Namespaces in the XML
    $namespaces = $xml->getNamespaces(true);
    $diffgram = $xml->children($namespaces['diffgr']);
    $dataset = $diffgram->children();

    // Convert XML to JSON and then to array
    $json = json_encode($dataset);
    $data = json_decode($json, true);

    if (isset($data['Datos_de_INGC011_CAT_INDICADORECONOMIC']['INGC011_CAT_INDICADORECONOMIC'])) {
        $items = $data['Datos_de_INGC011_CAT_INDICADORECONOMIC']['INGC011_CAT_INDICADORECONOMIC'];
        $result = [];
            $result[] = [
                'Valor' => $items['NUM_VALOR']
            ];
        return $result;
    }

    return null;
}

$data_buy = parse_exchange_rate($response_buy);
$data_sell = parse_exchange_rate($response_sell);

$result = [
    'Venta' => $data_sell,
    'Compra' => $data_buy
];

header('Content-Type: application/json');
return json_encode($result, JSON_PRETTY_PRINT);
}
function convertToUtf8($data) {
    if (is_array($data)) {
        $result = [];
        foreach ($data as $key => $value) {
            // Convertir el valor a UTF-8
            if (is_string($value)) {
                $encoding = mb_detect_encoding($value, 'UTF-8, ISO-8859-1, ISO-8859-15', true);
                if ($encoding === false) {
                    // Si no se puede detectar la codificación, asumir ISO-8859-1
                    $value = utf8_encode($value);
                } else {
                    $value = mb_convert_encoding($value, 'UTF-8', $encoding);
                }
            }
            // Si el valor es null, asignar 0
            if (is_null($value)) {
                $value = 0;
            }
            // Renombrar claves eliminando el prefijo "ID_"
            $newKey = preg_replace('/^ID_/', '', $key);
            $result[$newKey] = $value;
        }
        return $result;
    } elseif (is_string($data)) {
        $encoding = mb_detect_encoding($data, 'UTF-8, ISO-8859-1, ISO-8859-15', true);
        if ($encoding === false) {
            return utf8_encode($data);
        } else {
            return mb_convert_encoding($data, 'UTF-8', $encoding);
        }
    }
    return $data;
}


//Leer BD para llenar Json para poder utilizar el Ajax (Pais,Provincia,Canton,Distrito)
function LeerBD() {
    $conn = getConnection();
    if ($conn === false) {
        return ["success" => false, "error" => "No se pudo establecer la conexión con la base de datos", "details" => sqlsrv_errors()];
    }

    $sql = "SELECT Descripcion, ID_Pais, ID_Provincia, ID_Canton, ID_Distrito FROM Lugar";
    $stmt = sqlsrv_query($conn, $sql);

    if ($stmt === false) {
        return ["success" => false, "error" => "Error al realizar la consulta", "details" => sqlsrv_errors()];
    }


    sqlsrv_close($conn);

        return sqlsrv_fetch_array($stmt, SQLSRV_FETCH_ASSOC);
    
}

function obtenerServicios() {
    // Obtener la conexión a la base de datos
    $conn = getConnection();
    // Verificar si la conexión fue exitosa
    if ($conn === false) {
        return ["success" => false, "error" => "No se pudo establecer la conexión con la base de datos", "details" => sqlsrv_errors()];
    }
 
    // Consulta SQL para obtener los servicios por categoría
    $sql = "SELECT IdServicio,Nombre,Precio  FROM Servicios ";
 
    // Ejecutar la consulta con los parámetros
    $stmt = sqlsrv_query($conn, $sql);
    if ($stmt === false) {
        return ["success" => false, "error" => "Error al ejecutar la consulta", "details" => sqlsrv_errors()];
    }
 
    $resultados = [];
    while ($row = sqlsrv_fetch_array($stmt, SQLSRV_FETCH_ASSOC)) {
        $resultados[] = convertToUtf8($row);
    }
 
    sqlsrv_close($conn);
 
    if (empty($resultados)) {
        return ["success" => true, "data" => [], "message" => "No se encontraron resultados"];
    } else {
        echo json_encode($resultados);
    }
}

//Productos
function VerProductos() {
    $conn = getConnection();
    if ($conn === false) {
        return ["success" => false, "error" => "No se pudo establecer la conexión con la base de datos", "details" => sqlsrv_errors()];
    }

    $sql = "{CALL sp_Productos(?)}";

    // Preparar los parámetros
    $parameters = [
        [2,SQLSRV_PARAM_IN]
    ];

    // Ejecutar la consulta
    $stmt = sqlsrv_query($conn, $sql, $parameters);

    if ($stmt === false) {
        return ["success" => false, "error" => "Error al realizar la consulta", "details" => sqlsrv_errors()];
    }

    $resultados = [];
    while ($row = sqlsrv_fetch_array($stmt, SQLSRV_FETCH_ASSOC)) {
        if($row['Imagen']){
            $row['Imagen'] = "data:image/jpeg;base64,".base64_encode($row['Imagen']);

        }
        $resultados[] = convertToUtf8($row);
    }

    sqlsrv_close($conn);

    if (empty($resultados)) {
        return ["success" => true, "data" => [], "message" => "No se encontraron resultados"];
    } else {
        return ["success" => true, "data" => $resultados];
    }
}

//Habitaciones
function VerHabitaciones() {
    $conn = getConnection();
    if ($conn === false) {
        return ["success" => false, "error" => "No se pudo establecer la conexión con la base de datos", "details" => sqlsrv_errors()];
    }

    $sql = "{CALL sp_Habitaciones(?)}";

    // Preparar los parámetros
    $parameters = [
        [2,SQLSRV_PARAM_IN]
    ];

    // Ejecutar la consulta
    $stmt = sqlsrv_query($conn, $sql, $parameters);

    if ($stmt === false) {
        return ["success" => false, "error" => "Error al realizar la consulta", "details" => sqlsrv_errors()];
    }

    $resultados = [];
    while ($row = sqlsrv_fetch_array($stmt, SQLSRV_FETCH_ASSOC)) {
        if($row['Imagen']){
            $row['Imagen'] = "data:image/jpeg;base64,".base64_encode($row['Imagen']);

        }
        $resultados[] = convertToUtf8($row);
    }

    sqlsrv_close($conn);

    if (empty($resultados)) {
        return ["success" => true, "data" => [], "message" => "No se encontraron resultados"];
    } else {
        return ["success" => true, "data" => $resultados];
    }
}

function EscribirP($datos, $file) {
    $conn = getConnection();
    if ($conn === false) {
        return ["success" => false, "error" => "No se pudo establecer la conexión con la base de datos", "details" => sqlsrv_errors()];
    }

    $sql = "{CALL sp_Productos(?, ?, ?, ?, ?, ?)}";

    $parameters = [];
    $parameters[] = [1, SQLSRV_PARAM_IN]; // Parámetro @op
    for ($i = 0; $i < count($datos); $i++) {
        $parameters[] = [$datos[$i], SQLSRV_PARAM_IN];
    }

    $imagenBytes = file_get_contents($file['tmp_name']);
    $parameters[] = [$imagenBytes];

    $stmt = sqlsrv_query($conn, $sql, $parameters);

    if ($stmt === false) {
        return ["success" => false, "error" => "Error al ejecutar la consulta", "details" => sqlsrv_errors()];
    }

    if (sqlsrv_rows_affected($stmt) > 0) {
        $response = ["success" => true, "message" => "Inserción realizada con éxito"];
    } else {
        $response = ["success" => false, "message" => "No se realizaron inserciones"];
    }

    sqlsrv_free_stmt($stmt);
    sqlsrv_close($conn);

    return $response;
}

function EscribirH($datos, $file) {
    $conn = getConnection();
    if ($conn === false) {
        return ["success" => false, "error" => "No se pudo establecer la conexión con la base de datos", "details" => sqlsrv_errors()];
    }

    $sql = "{CALL sp_Habitaciones(?, ?, ?, ?, ?, ?)}";

    $parameters = [];
    $parameters[] = [1, SQLSRV_PARAM_IN]; // Parámetro @op
    for ($i = 0; $i < count($datos); $i++) {
        $parameters[] = [$datos[$i], SQLSRV_PARAM_IN];
    }

    $imagenBytes = file_get_contents($file['tmp_name']);
    $parameters[] = [$imagenBytes];

    $stmt = sqlsrv_query($conn, $sql, $parameters);

    if ($stmt === false) {
        return ["success" => false, "error" => "Error al ejecutar la consulta", "details" => sqlsrv_errors()];
    }

    if (sqlsrv_rows_affected($stmt) > 0) {
        $response = ["success" => true, "message" => "Inserción realizada con éxito"];
    } else {
        $response = ["success" => false, "message" => "No se realizaron inserciones"];
    }

    sqlsrv_free_stmt($stmt);
    sqlsrv_close($conn);

    return $response;
}


function encriptar($plainText) {
    $key = base64_decode("wM6v3Xe1h8c2N1A2m4H6eQ==");
    $iv = base64_decode("VbT1eL4UwYZ2JNdyUkgE4w==");
    // Configurar el método de encriptación (AES-256-CBC en este caso)
    $cipher = "AES-256-CBC";
    
    // Asegurarse de usar el mismo padding
    $encrypted = openssl_encrypt($plainText, $cipher, $key, OPENSSL_RAW_DATA | OPENSSL_ZERO_PADDING, $iv);
    
    // Aplicar padding manual (similar al padding PKCS7 en C#)
    $blockSize = 16;
    $padding = $blockSize - (strlen($plainText) % $blockSize);
    $plainText .= str_repeat(chr($padding), $padding);
    
    // Realizar la encriptación
    $encrypted = openssl_encrypt($plainText, $cipher, $key, OPENSSL_RAW_DATA, $iv);
    
    return $encrypted;
}

//DESENCRIPTAR
function desencriptar($dato) {
    $key = 'estaesunaclave32bytesde256bits';
    $iv = '1234567890123456';
    if (empty($key) || empty($iv)) {
        throw new Exception("La clave o el IV no están definidos.");
    }

    // Desencriptar usando AES-256-CBC
    $desencriptado = openssl_decrypt($dato, 'AES-256-CBC', $key, 0, $iv);
    return $desencriptado;
}

function login($correo, $contrasena) {
    $conn = getConnection();
    if ($conn === false) {
        return ["success" => false, "error" => "No se pudo establecer la conexión con la base de datos", "details" => sqlsrv_errors()];
    }

    // La consulta SQL para seleccionar los valores de correo y contraseña en la tabla Usuarios
    $sql = "SELECT Correo, Contrasena FROM Usuarios WHERE Correo = ? AND Contrasena = ?";

    // Encriptar los valores antes de enviarlos a la consulta
    $correoEncriptado = encriptar($correo);
    $contrasenaEncriptada = encriptar($contrasena);

    // Crear el array de parámetros
    $parameters = [
        [$correoEncriptado, SQLSRV_PARAM_IN],
        [$contrasenaEncriptada, SQLSRV_PARAM_IN]
    ];

    // Ejecutar la consulta
    $stmt = sqlsrv_query($conn, $sql, $parameters);

    if ($stmt === false) {
        return ["success" => false, "error" => "Error al ejecutar la consulta", "details" => sqlsrv_errors()];
    }

    // Comprobar si se encontró un registro que coincida
    if (sqlsrv_fetch($stmt)) {
        // Si se encuentra una fila, las credenciales son correctas
        $response = "1,Inicio de sesión exitoso";
        
    } else {
        // Si no se encuentran filas, las credenciales no son válidas
        $response = "2,Credenciales incorrectas";
    }

    sqlsrv_free_stmt($stmt);
    sqlsrv_close($conn);

    echo ($response);
}

function obtenerServiciosPorCategoria($idCategoria) {
    // Obtener la conexión a la base de datos
    $conn = getConnection();
    
    // Verificar si la conexión fue exitosa
    if ($conn === false) {
        return ["success" => false, "error" => "No se pudo establecer la conexión con la base de datos", "details" => sqlsrv_errors()];
    }

    // Consulta SQL para obtener los servicios por categoría
    $sql = "SELECT IdServicio, Nombre, Descripcion, Precio, Imagen FROM Servicios WHERE IdCategoriaSer = ?";

    // Parámetros para la consulta
    $parameters = [[$idCategoria, SQLSRV_PARAM_IN]];

    // Ejecutar la consulta con los parámetros
    $stmt = sqlsrv_query($conn, $sql, $parameters);
    if ($stmt === false) {
        return ["success" => false, "error" => "Error al ejecutar la consulta", "details" => sqlsrv_errors()];
    }

    $resultados = [];
    while ($row = sqlsrv_fetch_array($stmt, SQLSRV_FETCH_ASSOC)) {
    // Convertir la columna Imagen a base64
    if (isset($row['Imagen']) && !is_null($row['Imagen'])) {
        $row['Imagen'] = base64_encode($row['Imagen']);
    }
        $resultados[] = convertToUtf8($row);
    }

    sqlsrv_close($conn);

    if (empty($resultados)) {
        return ["success" => true, "data" => [], "message" => "No se encontraron resultados"];
    } else {
        echo json_encode($resultados);
    }
}

function obtenerPromos() {
    // Obtener la conexión a la base de datos
    $conn = getConnection();
    
    // Verificar si la conexión fue exitosa
    if ($conn === false) {
        return ["success" => false, "error" => "No se pudo establecer la conexión con la base de datos", "details" => sqlsrv_errors()];
    }

    // Consulta SQL para obtener los servicios por categoría
    $sql = "SELECT IdPromo,Imagen, Promo, Descripcion, Precio  FROM Promos ";

    // Ejecutar la consulta con los parámetros
    $stmt = sqlsrv_query($conn, $sql);
    if ($stmt === false) {
        return ["success" => false, "error" => "Error al ejecutar la consulta", "details" => sqlsrv_errors()];
    }

    $resultados = [];
    while ($row = sqlsrv_fetch_array($stmt, SQLSRV_FETCH_ASSOC)) {
    // Convertir la columna Imagen a base64
    if (isset($row['Imagen']) && !is_null($row['Imagen'])) {
        $row['Imagen'] = base64_encode($row['Imagen']);
    }
        $resultados[] = convertToUtf8($row);
    }

    sqlsrv_close($conn);

    if (empty($resultados)) {
        return ["success" => true, "data" => [], "message" => "No se encontraron resultados"];
    } else {
        echo json_encode($resultados);
    }
}

function obtenerActividad() {
    // Obtener la conexión a la base de datos
    $conn = getConnection();
    
    // Verificar si la conexión fue exitosa
    if ($conn === false) {
        return ["success" => false, "error" => "No se pudo establecer la conexión con la base de datos", "details" => sqlsrv_errors()];
    }

    // Consulta SQL para obtener los servicios por categoría
    $sql = "SELECT IdAct, Imagen, Actividad, Descripcion,CantPersonas, Precio  FROM Actividades ";

    // Ejecutar la consulta con los parámetros
    $stmt = sqlsrv_query($conn, $sql);
    if ($stmt === false) {
        return ["success" => false, "error" => "Error al ejecutar la consulta", "details" => sqlsrv_errors()];
    }

    $resultados = [];
    while ($row = sqlsrv_fetch_array($stmt, SQLSRV_FETCH_ASSOC)) {
    // Convertir la columna Imagen a base64
    if (isset($row['Imagen']) && !is_null($row['Imagen'])) {
        $row['Imagen'] = base64_encode($row['Imagen']);
    }
        $resultados[] = convertToUtf8($row);
    }

    sqlsrv_close($conn);

    if (empty($resultados)) {
        return ["success" => true, "data" => [], "message" => "No se encontraron resultados"];
    } else {
        echo json_encode($resultados);
    }
}


function registrar($Ide, $Nombre, $Correo, $Telefono, $Contrasena, $FechaNa, $Vigencia, $PalabraClave) {
    $conn = getConnection();
    if ($conn === false) {
        return ["success" => false, "error" => "No se pudo establecer la conexión con la base de datos", "details" => sqlsrv_errors()];
    }

    // La consulta SQL para llamar al procedimiento almacenado
    $sql = "{CALL Jaz.GestionUsuarios(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)}";

    // Encriptar los valores antes de enviarlos a la consulta (asegúrate de que el resultado esté en formato binario)
    $CorreoEn = encriptar($Correo);
    $ContrasenaEn = encriptar($Contrasena);
    $PalabraClaveEn = encriptar($PalabraClave);

    // Crear el array de parámetros
    $mensajeSalida = ''; // Declaración de la variable para el mensaje de salida

    $parameters = [
        [1, SQLSRV_PARAM_IN],                 // Tipo de operación
        [$Ide, SQLSRV_PARAM_IN],              // Identificación
        [$Nombre, SQLSRV_PARAM_IN],           // Nombre
        [$CorreoEn, SQLSRV_PARAM_IN],         // Correo encriptado (binario)
        [$Telefono, SQLSRV_PARAM_IN],         // Teléfono
        [$ContrasenaEn, SQLSRV_PARAM_IN],     // Contraseña encriptada (binario)
        [$FechaNa, SQLSRV_PARAM_IN],          // Fecha de nacimiento
        [1, SQLSRV_PARAM_IN],                 // IdTipoUsu
        [$Vigencia, SQLSRV_PARAM_IN],         // Vigencia
        [$PalabraClaveEn, SQLSRV_PARAM_IN],   // Palabra clave encriptada (binario)
        [encriptar("Nada"), SQLSRV_PARAM_IN],  // Parámetro adicional de entrada
        [null, SQLSRV_PARAM_IN],              // ContraAntigua
        ["Activo", SQLSRV_PARAM_IN],          // Estado
        ];

    // Ejecutar la consulta
    $stmt = sqlsrv_query($conn, $sql, $parameters);

    // Verificar si la consulta se ejecutó correctamente
    if ($stmt === false) {
        echo "Error al ejecutar la consulta<br>";
        
        // Mostrar errores detallados de SQL Server
        if (($errors = sqlsrv_errors()) != null) {
            foreach ($errors as $error) {
                echo "SQLSTATE: " . $error['SQLSTATE'] . "<br />";
                echo "Code: " . $error['code'] . "<br />";
                echo "Message: " . $error['message'] . "<br />";
            }
        }

        // Cerrar la conexión y salir de la función
        sqlsrv_close($conn);
        return ["success" => false, "error" => "Error en la consulta SQL"];
    }

    // Liberar el recurso solo si la consulta fue exitosa
    sqlsrv_free_stmt($stmt);
    sqlsrv_close($conn);

    // Retornar el mensaje de salida
     echo($mensajeSalida);
}

function reservaServicios($Ide,$Servicio,$Fecha,$Hora,$Precio) {
    $conn = getConnection();
    if ($conn === false) {
        return ["success" => false, "error" => "No se pudo establecer la conexión con la base de datos", "details" => sqlsrv_errors()];
    }

    // La consulta SQL para llamar al procedimiento almacenado
    $sql = "{CALL Jaz.GestionReservas(?, ?, ?, ?, ?, ?, ?)}";

    // Crear el array de parámetros
    $mensajeSalida = ''; // Declaración de la variable para el mensaje de salida

    $parameters = [
        [1, SQLSRV_PARAM_IN],                
        [$Ide, SQLSRV_PARAM_IN],              
        [$Servicio, SQLSRV_PARAM_IN],        
        [$Fecha, SQLSRV_PARAM_IN],       
        [$Hora, SQLSRV_PARAM_IN],         
        [$Precio, SQLSRV_PARAM_IN], 
        ];

    // Ejecutar la consulta
    $stmt = sqlsrv_query($conn, $sql, $parameters);

    // Verificar si la consulta se ejecutó correctamente
    if ($stmt === false) {
        echo "Error al ejecutar la consulta<br>";
        
        // Mostrar errores detallados de SQL Server
        if (($errors = sqlsrv_errors()) != null) {
            foreach ($errors as $error) {
                echo "SQLSTATE: " . $error['SQLSTATE'] . "<br />";
                echo "Code: " . $error['code'] . "<br />";
                echo "Message: " . $error['message'] . "<br />";
            }
        }

        // Cerrar la conexión y salir de la función
        sqlsrv_close($conn);
        return ["success" => false, "error" => "Error en la consulta SQL"];
    }

    // Liberar el recurso solo si la consulta fue exitosa
    sqlsrv_free_stmt($stmt);
    sqlsrv_close($conn);

    // Retornar el mensaje de salida
     echo($mensajeSalida);
}

function EscribirRH($datos) {
    $conn = getConnection();
    if ($conn === false) {
        return ["success" => false, "error" => "No se pudo establecer la conexión con la base de datos", "details" => sqlsrv_errors()];
    }

    $sql = "{CALL ReservaHabitaciones(?, ?, ?, ?, ?, ?)}";

    $parameters = [];
    $parameters[] = [1, SQLSRV_PARAM_IN];
    for ($i = 0; $i < count($datos); $i++) {
        $parameters[] = [$datos[$i], SQLSRV_PARAM_IN];
    }

    $stmt = sqlsrv_query($conn, $sql, $parameters);

    if ($stmt === false) {
        return ["success" => false, "error" => "Error al ejecutar la consulta", "details" => sqlsrv_errors()];
    }

    if (sqlsrv_rows_affected($stmt) > 0) {
        $response = ["success" => true, "message" => "Inserción realizada con éxito"];
    } else {
        $response = ["success" => false, "message" => "No se realizaron inserciones"];
    }

    sqlsrv_free_stmt($stmt);
    sqlsrv_close($conn);

    return $response;
}

function AnularRH($datos){
    $conn = getConnection();
    if ($conn === false) {
        return ["success" => false, "error" => "No se pudo establecer la conexión con la base de datos", "details" => sqlsrv_errors()];
    }

    $sql = "{CALL ReservaHabitaciones(?, ?, ?, ?, ?)}";

    $parameters = [];
    $parameters[] = [4, SQLSRV_PARAM_IN]; // Parámetro @op
    for ($i = 0; $i < count($datos); $i++) {
        $parameters[] = [$datos[$i], SQLSRV_PARAM_IN];
    }

    $stmt = sqlsrv_query($conn, $sql, $parameters);

    if ($stmt === false) {
        return ["success" => false, "error" => "Error al ejecutar la consulta", "details" => sqlsrv_errors()];
    }

    if (sqlsrv_rows_affected($stmt) > 0) {
        $response = ["success" => true, "message" => "Inserción realizada con éxito"];
    } else {
        $response = ["success" => false, "message" => "No se realizaron inserciones"];
    }

    sqlsrv_free_stmt($stmt);
    sqlsrv_close($conn);

    return $response;
}

function ModificarRH($datos) {
    $conn = getConnection();
    if ($conn === false) {
        return ["success" => false, "error" => "No se pudo establecer la conexión con la base de datos", "details" => sqlsrv_errors()];
    }

    $sql = "{CALL ReservaHabitaciones(?, ?, ?, ?, ?)}";

    $parameters = [];
    $parameters[] = [3, SQLSRV_PARAM_IN]; // Parámetro @op
    for ($i = 0; $i < count($datos); $i++) {
        $parameters[] = [$datos[$i], SQLSRV_PARAM_IN];
    }

    $stmt = sqlsrv_query($conn, $sql, $parameters);

    if ($stmt === false) {
        return ["success" => false, "error" => "Error al ejecutar la consulta", "details" => sqlsrv_errors()];
    }

    if (sqlsrv_rows_affected($stmt) > 0) {
        $response = ["success" => true, "message" => "Inserción realizada con éxito"];
    } else {
        $response = ["success" => false, "message" => "No se realizaron inserciones"];
    }

    sqlsrv_free_stmt($stmt);
    sqlsrv_close($conn);

    return $response;
}

function VerReservasHabi($datos) {
    $conn = getConnection();
    if ($conn === false) {
        return ["success" => false, "error" => "No se pudo establecer la conexión con la base de datos", "details" => sqlsrv_errors()];
    }

    $sql = "{CALL ReservaHabitaciones(?, ?)}";

    $parameters = [];
    $parameters[] = [2, SQLSRV_PARAM_IN];
    $parameters[] = [$datos, SQLSRV_PARAM_IN];

    $stmt = sqlsrv_query($conn, $sql, $parameters);

    if ($stmt === false) {
        return ["success" => false, "error" => "Error al realizar la consulta", "details" => sqlsrv_errors()];
    }

    $resultados = [];
    while ($row = sqlsrv_fetch_array($stmt, SQLSRV_FETCH_ASSOC)) {
        $resultados[] = convertToUtf8($row);
    }

    sqlsrv_close($conn);

    if (empty($resultados)) {
        return ["success" => true, "data" => [], "message" => "No se encontraron resultados"];
    } else {
        return ["success" => true, "data" => $resultados];
    }
}

function MostrarUsu($datos) {
        $conn = getConnection();
        if ($conn === false) {
            return ["success" => false, "error" => "No se pudo establecer la conexión con la base de datos", "details" => sqlsrv_errors()];
        }
    
        $sql = "SELECT Nombre, Apellidos, Email, Telefono FROM TSE WHERE Identificacion = ?";
        $params = array($datos);

        $stmt = sqlsrv_query($conn, $sql, $params);
    
        if ($stmt === false) {
            return ["success" => false, "error" => "Error al realizar la consulta", "details" => sqlsrv_errors()];
        }
    
        $resultados = [];
        while ($row = sqlsrv_fetch_array($stmt, SQLSRV_FETCH_ASSOC)) {
            $resultados[] = convertToUtf8($row);
        }
    
        sqlsrv_close($conn);
    
        if (empty($resultados)) {
            return ["success" => true, "data" => [], "message" => "No se encontraron resultados"];
        } else {
            return ["success" => true, "data" => $resultados];
        }
}