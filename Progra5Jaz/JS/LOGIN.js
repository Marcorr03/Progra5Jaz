const inputs = document.querySelectorAll('input[type="tel"]');
inputs.forEach((input, index) => {
    input.addEventListener('input', (event) => {
        const value = event.target.value;
        if (!/^\d*$/.test(value)) {
            event.target.value = '';
            return;
        }

        if (value && index < inputs.length - 1) {
            inputs[index + 1].focus();
        }
        else if (value && index > inputs[index].focus()) {
            inputs[index - 1].focus();
        }


    });

    input.addEventListener('keydown', (event) => {
        if (event.key === 'Backspace' && !event.target.value && index > 0) {
            inputs[index - 1].focus();
        } else if (event.key != 'Backspace' && index == inputs.length - 1) {
            setTimeout(() => {
                input.blur();
            }, 10);
        }
    });
});


const pass = document.getElementById('Contrasena');
var caracteres = 0;
var letrasMin = 0;
var letrasMay = 0;
var especiales = 0;
var numeros = 0;

const condicion = document.getElementById('condicion');
pass.addEventListener('focus', (event) => {
    condicion.style.display = 'block';
});

pass.addEventListener('blur', (event) => {
    condicion.style.display = 'none';
});

pass.addEventListener('input', (event) => {
    const value = pass.value;
    letrasMay = ContieneLetrasMay(value);
    letrasMin = ContieneLetrasMin(value);
    especiales = CaracteresEspeciales(value);
    caracteres = value.length;
    numeros = ContieneNumeros(value);
    // Ejemplo de uso: mostrar alertas según las verificaciones
    var primero = document.getElementById('01');
    var segundo = document.getElementById('02');
    var tercero = document.getElementById('03');
    var cuarto = document.getElementById('04');
    var quinto = document.getElementById('05');

    if (caracteres >= 14 && caracteres <= 20) {
        primero.removeAttribute('disabled');
        primero.checked = true;
    } else {
        primero.checked = false;
    }

    if (letrasMin >= 4) {
        segundo.removeAttribute('disabled');
        segundo.checked = true;
    } else {
        segundo.checked = false;
    }

    if (letrasMay >= 4) {
        tercero.removeAttribute('disabled');
        tercero.checked = true;
    } else {
        tercero.checked = false;
    }

    if (especiales >= 2) {
        cuarto.removeAttribute('disabled');
        cuarto.checked = true;
    } else {
        cuarto.setAttribute('disabled', 'true');
        cuarto.checked = false;
    }

    if (numeros >= 4) {
        quinto.removeAttribute('disabled');
        quinto.checked = true;
    } else {
        quinto.setAttribute('disabled', 'true');
        quinto.checked = false;
    }

    if (primero.checked == true && segundo.checked == true && tercero.checked == true && cuarto.checked == true && quinto.checked == true) {

    }
    primero.setAttribute('disabled', 'true');
    segundo.setAttribute('disabled', 'true');
    document.getElementById('03').setAttribute('disabled', 'true');
    document.getElementById('04').setAttribute('disabled', 'true');
    document.getElementById('05').setAttribute('disabled', 'true');

});

function ContieneLetrasMay(str) {
    let count = 0;
    for (let i = 0; i < str.length; i++) {
        const char = str[i];
        if (char >= 'A' && char <= 'Z') {
            count++;
        }
    }
    return count;
}

function ContieneLetrasMin(str) {
    let count = 0;
    for (let i = 0; i < str.length; i++) {
        const char = str[i];
        if (char >= 'a' && char <= 'z') {
            count++;
        }
    }
    return count;
}

function CaracteresEspeciales(str) {
    let count = 0;
    const specialCharacters = /[!@#$%^&*(),.?":{}|<>]/;
    for (let i = 0; i < str.length; i++) {
        if (specialCharacters.test(str[i])) {
            count++;
        }
    }
    return count;
}

function ContieneNumeros(str) {
    let count = 0;
    for (let i = 0; i < str.length; i++) {
        const char = str[i];
        if (char >= '0' && char <= '9') {
            count++;
        }
    }
    return count;
}

