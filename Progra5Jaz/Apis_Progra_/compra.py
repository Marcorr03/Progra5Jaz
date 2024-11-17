from flask import Flask, jsonify, request
from pymongo import MongoClient

app = Flask(__name__)

# Conectar a MongoDB
client = MongoClient("mongodb://localhost:27017/")
database = client["Progra"]
collection = database["Progra_V1"]

@app.route('/api/tarjetas', methods=['GET'])
def get_tarjetas():
    tarjeta = request.args.get('tarjeta')
    if not tarjeta:
        return jsonify({"success": False, "error": "Falta el parámetro 'tarjeta'"}), 400

    filtro = {"tarjeta": tarjeta}
    documentos = collection.find(filtro)
    resultados = []

    for documento in documentos:
        documento['_id'] = str(documento['_id'])
        resultados.append(documento)

    if not resultados:
        return jsonify({"success": True, "data": [], "message": "No se encontraron resultados"})

    return jsonify({"success": True, "data": resultados})

@app.route('/')
def home():
    return "Bienvenido a la API de Tarjetas"

if __name__ == '__main__':
    app.run(debug=True)