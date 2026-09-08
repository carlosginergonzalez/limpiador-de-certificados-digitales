# Limpiador de certificados digitales

Programa para **Windows** que lista los certificados digitales de tu usuario, te deja marcar los que no necesitas y **eliminarlos** de forma controlada. Los no marcados se conservan.

Diseñado por **Carlos Giner Gonzalez**.

---

## Descargar

| | |
| --- | --- |
| **Última versión** | [Releases](https://github.com/carlosginergonzalez/limpiador-de-certificados-digitales/releases/latest) |
| **Ejecutable** | `LimpiadorCertificados.exe` (incluido en la release) |

1. Entra en **Releases** y descarga `LimpiadorCertificados.exe`.
2. Ejecútalo (no requiere instalación).
3. Si Windows SmartScreen avisa, elige *Más información* → *Ejecutar de todas formas* (es software local sin instalador firmado).

También puedes clonar el repositorio y usar el `.exe` de la carpeta raíz o `bin\`.

---

## Qué hace

- Muestra los certificados del **usuario actual** (almacenes Personal, confianza, raíces, etc.).
- Casillas para marcar los que quieres **borrar**.
- Botón grande **Eliminar** (pide confirmación y borra solo los marcados).
- Atajos: actualizar (F5), marcar caducados, quitar marcas.

**No** borra automáticamente sin que marques y confirmes. **No** gestiona el almacén del equipo local.

---

## Requisitos

- Windows 10 / 11
- .NET Framework 4.x (incluido en Windows moderno)

---

## Compilar desde el código

```bat
build.cmd
```

Genera `bin\LimpiadorCertificados.exe` y una copia en la raíz del proyecto. Icono en `assets\app.ico`.

---

## Uso rápido

1. Abre la aplicación.
2. Elige un almacén a la izquierda (por defecto: Personal).
3. Marca los certificados que **no** necesitas (opcional: *Marcar caducados*).
4. Pulsa **Eliminar** y confirma.

---

## Licencia

Uso libre para fines personales y profesionales. El icono está inspirado en la identidad institucional de la FNMT; no es el logo oficial registrado.
