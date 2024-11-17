<?php
$serverName = "tiusr21pl.cuc-carrera-ti.ac.cr\\MSSQLSERVER2019";
$connectionOptions = [
    "Database" => "ProyprogJaz",
    "Uid" => "Jaz",
    "PWD" => "progra123"
];

// Crear conexión
function getConnection() {
    global $serverName, $connectionOptions;
    $conn = sqlsrv_connect($serverName, $connectionOptions);

    if ($conn === false) {
        die(json_encode(["error" => "Error al conectar con la base de datos", "details" => sqlsrv_errors()]));
    }
    return $conn;
}