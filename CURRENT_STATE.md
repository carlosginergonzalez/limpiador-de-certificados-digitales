# CURRENT_STATE

Foto viva del proyecto. Solo lo vigente. Lo cerrado vive en `CHANGELOG.md`.

Regenerar este fichero cuando `CHANGELOG.md` supere ~300 líneas, ~30 entradas detalladas, o deje de describir el sistema actual. Tras compactar, las tareas finalizadas se descartan de aquí.

---

## Objetivo vigente

Permitir revisar certificados digitales del PC, marcar los que no se necesitan y borrarlos con confirmación, dejando el resto.

## Estado del pipeline

| Pieza | Estado |
| --- | --- |
| Contexto (`AGENTS.md`) | Hecho |
| Historial (`CHANGELOG.md`) | Hecho |
| Estado vivo (`CURRENT_STATE.md`) | Hecho (este fichero) |
| App Windows Forms C# (como FacturasNas) | Hecho v1.0 |
| UI estilo Administrador de certificados (menú, árbol, lista, propiedades) | Hecho |
| Inventario CurrentUser (solo usuario actual) | Hecho |
| Almacenes My, Root, CA, TrustedPeople, AddressBook | Hecho |
| Selección por casillas + marcar caducados | Hecho |
| Botón Eliminar abajo (borrado real, con confirmación) | Hecho |

## Configuración vigente

- Ruta: `C:\CURSOR\ROBOT LIMPIAR CERTIFICADOS INSTALADOS EN EL PC`
- Herramienta: **Limpiador de certificados digitales** (`LimpiadorCertificados.exe`) — compilar con `build.cmd`
- GitHub (público): https://github.com/carlosginergonzalez/limpiador-de-certificados-digitales
- Descarga: Releases → `LimpiadorCertificados.exe`
- Icono: `assets\app.ico` (inspirado en FNMT; incrustado en el exe)
- Stack: C# / WinForms / `csc.exe` (.NET Framework 4.x), mismo patrón que el robot de facturas Gmail→NAS
- Almacén por defecto: `CurrentUser` / `My`
- Simulación activada por defecto

## Bloqueos

- Ninguno abierto. `LocalMachine` puede pedir elevación de administrador en uso real.

## Próximas acciones

1. Probar en el PC del usuario: listar `My`, marcar caducados en simulación, luego borrado real si procede.
2. Ajustar almacenes o columnas si el usuario lo pide tras la primera prueba.

## Última sesión

2026-09-08 — UI rediseñada al estilo Windows (certmgr): menú, barra, árbol de almacenes, lista y propiedades. Compilación OK.
