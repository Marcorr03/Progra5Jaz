const Pais = document.getElementById('Pais');
const Provincia = document.getElementById('Provincia');
const Canton = document.getElementById('Canton');
const Distrito = document.getElementById('Distrito');
var datos = '';

fetch('/Home/LeerJson')
    .then(response => {
        // Verificar si la respuesta es válida
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        // Convertir la respuesta a JSON
        return response.json();
    })
    .then(data => {
        // Asignar los datos a una variable
        datos = data;
        // Llamar a la función Paises con los datos
        Paises(datos);

    })
    .catch(error => {
        // Manejar errores y mostrar un mensaje en la consola
        console.error('Error al cargar el JSON:', error);
    });

Pais.addEventListener('change', () => {
    const dato = Pais.value;
    if (Pais.value == '0') {
        Provincia.innerHTML = '<option>Seleccione un Provincia</option>';
        Canton.innerHTML = '<option>Seleccione un Canton</option>';
        Distrito.innerHTML = '<option>Seleccione un Distrito</option>';
    } else {
        Provincias(dato);
    }
});

Provincia.addEventListener('change', () => {
    const dato = Provincia.value;
    Cantones(dato);
});

Canton.addEventListener('change', () => {
    const dato = Canton.value;
    Distritos(dato);
});


function Paises(characters) {
    Pais.innerHTML = '<option value="0">Seleccione un Pais</option>';
    characters.forEach(ch => {
        if (ch.Provincia == '0' && ch.Canton == '0' && ch.Distrito == '0') {
            const option = document.createElement('option');
            option.value = ch.Pais;
            option.textContent = ch.Descripcion;
            Pais.appendChild(option);
        }
    });
    const direccion = document.getElementById("Direccion").value
    if (direccion != undefined) {
        LlenarPais(direccion.split(',')[0]);
        LlenarProvincia(direccion.split(',')[1]);
        LlenarCanton(direccion.split(',')[2]);
        LlenarDistrito(direccion.split(',')[3]);
    }
}

function Provincias(Pais) {
    Provincia.innerHTML = '<option>Seleccione un Provincia</option>';
    datos.forEach(ch => {
        if (ch.Pais == Pais && ch.Provincia != '0' && ch.Canton == '0' && ch.Distrito == '0') {
            const option = document.createElement('option');
            option.value = ch.Provincia;
            option.textContent = ch.Descripcion;
            Provincia.appendChild(option);
        }
    });
}

function Cantones(Provincia) {
    Canton.innerHTML = '<option>Seleccione un Canton</option>';
    datos.forEach(ch => {
        if (ch.Pais == Pais.value && ch.Provincia == Provincia && ch.Canton != '0' && ch.Distrito == '0') {
            const option = document.createElement('option');
            option.value = ch.Canton;
            option.textContent = ch.Descripcion;
            Canton.appendChild(option);
        }
    });
}

function Distritos(Canton) {
    Distrito.innerHTML = '<option>Seleccione un Distrito</option>';
    datos.forEach(ch => {
        if (ch.Pais == Pais.value && ch.Provincia == Provincia.value && ch.Canton == Canton && ch.Distrito != '0') {
            const option = document.createElement('option');
            option.value = ch.Distrito;
            option.textContent = ch.Descripcion;
            Distrito.appendChild(option);
        }
    });
}

function LlenarPais(PaisSe) {
    for (var i = 0; i < Pais.options.length; i++) {
        if (Pais.options[i].text === PaisSe) {
            Pais.selectedIndex = i;
            var event = new Event('change');
            Pais.dispatchEvent(event);
            break;
        }
    }
}
function LlenarProvincia(ProvinciaSe) {
    for (var i = 0; i < Provincia.options.length; i++) {
        if (Provincia.options[i].text === ProvinciaSe) {
            Provincia.selectedIndex = i;
            var event = new Event('change');
            Provincia.dispatchEvent(event);
            break;
        }
    }
}
function LlenarCanton(CantonSe) {
    for (var i = 0; i < Canton.options.length; i++) {
        if (Canton.options[i].text === CantonSe) {
            Canton.selectedIndex = i;
            var event = new Event('change');
            Canton.dispatchEvent(event);
            break;
        }
    }
}
function LlenarDistrito(DistritoSe) {
    for (var i = 0; i < Distrito.options.length; i++) {
        if (Distrito.options[i].text === DistritoSe) {
            Distrito.selectedIndex = i;
            var event = new Event('change');
            Distrito.dispatchEvent(event);
            break;
        }
    }
}