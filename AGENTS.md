# AGENTS.md — ROBOT LIMPIAR CERTIFICADOS INSTALADOS EN EL PC

Documento de contexto para el agente. Léelo al inicio de cada sesión y actúa según estas reglas.

## Misión

Revisar los certificados digitales instalados en el PC, permitir al usuario **elegir cuáles conservar y cuáles borrar**, y **eliminar solo los que no necesite**.

## Alcance (qué sí / qué no)

- **Sí:** inventariar certificados de almacenes Windows; mostrar asunto, emisor, validez, huella, clave privada; selección interactiva; borrado solo de los marcados, con confirmación.
- **Sí:** modo simulación (dry-run) por defecto antes del borrado real.
- **No:** borrado automático sin confirmación del usuario.
- **No:** revocar en CA remotas ni gestionar smartcards más allá del almacén local.
- **No:** inventar borrados “inteligentes” sin revisión humana.

## Herramienta (mismo tipo que otros robots)

Aplicación **Windows Forms en C#**, igual que `FacturasNas` (proyecto EMAIL):

| Pieza | Ruta |
| --- | --- |
| Código | `src\*.cs` |
| Compilar | `build.cmd` → `bin\LimpiadorCertificados.exe` |
| Lanzar | `LimpiadorCertificados.cmd` o `LimpiadorCertificados.exe` |
| Compilador | `csc.exe` (.NET Framework 4.x), WinForms |

Namespace: `AsesoriaColon.LimpiarCertificados`.  
Descripción: «Robot diseñado por Carlos Giner Gonzalez…».

No cambiar a Python/PowerShell como app principal salvo que el usuario lo pida. Scripts auxiliares sí, si ayudan.

## Flujo operativo

1. Abrir `LimpiadorCertificados.exe`.
2. Elegir ubicación (`CurrentUser` / `LocalMachine`) y almacén (`My`, `Root`, `CA`, …). Por defecto: **CurrentUser / My**.
3. Actualizar lista; opcionalmente filtrar solo caducados o marcar caducados.
4. **Marcar los que NO se necesitan** (los no marcados se conservan).
5. Con «Simular» activado, comprobar el conteo; luego desmarcar simulación y eliminar con confirmación.
6. `LocalMachine` puede requerir ejecutar como administrador.

## Principios de seguridad

- Dry-run por defecto.
- Confirmación explícita antes de borrar de verdad.
- Aviso claro si se listan raíces/intermedias del sistema.
- No exfiltrar ni publicar claves privadas.
- Documentar riesgos de almacenes de máquina.

## Documentación del proyecto (obligatorio)

Hay dos ficheros vivos. El agente los mantiene; no esperar a que el usuario lo recuerde.

### CHANGELOG.md — historial

Actualizar el **CHANGELOG de forma periódica**, no solo al cerrar un hito grande.

**Cuándo actualizar (mínimo):**

- Al final de cada sesión con cambios de código, UI o comportamiento de borrado.
- Tras cada cambio funcional (inventario, selección, dry-run, almacenes).
- Al crear, compactar o regenerar `CURRENT_STATE.md`.

**Cómo escribir:** fecha ISO, resumen breve, Añadido / Cambiado / Corregido; el porqué además del qué.

### CURRENT_STATE.md — estado vivo

Mantener `CURRENT_STATE.md` como foto actual (aunque el CHANGELOG sea pequeño).

Si `CHANGELOG.md` se vuelve **demasiado grande** (~300 líneas, ~30 entradas, o ya no describe el sistema actual):

1. **Autogenerar/regenerar** `CURRENT_STATE.md` con solo información viva.
2. **Descartar de CURRENT_STATE** lo finalizado (permanece en CHANGELOG).
3. Anotar en CHANGELOG que se condensó el estado vivo.

**Orden de lectura:** `CURRENT_STATE.md` → `AGENTS.md` → `CHANGELOG.md` si hace falta.

## Convenciones

- Cambios mínimos y verificables.
- Tras implementar: actualizar CHANGELOG y CURRENT_STATE en el mismo turno.
- Idioma de documentación y UI: **español**.
- Compilar con `build.cmd` tras cambios en `src\`.

## Criterio de éxito

El usuario ve los certificados, marca los que no necesita, elimina solo esos y conserva el resto.
