# CHANGELOG — ROBOT LIMPIAR CERTIFICADOS INSTALADOS EN EL PC

Historial de cambios del proyecto. Las entradas más recientes van arriba.

Convención: el agente actualiza este fichero de forma periódica. Si crece demasiado, se regenera `CURRENT_STATE.md` con el estado vivo (ver `AGENTS.md`).

---

## [2026-09-08] — Sin modo simulación

### Eliminado
- Opción «Modo simulación». Al pulsar **Eliminar** (tras confirmar) se borran de verdad los certificados marcados.

---

## [2026-09-08] — Botón Eliminar abajo

### Cambiado
- Quitado «Conservar / Eliminar» de la barra. Un único botón grande **Eliminar** centrado abajo, bien visible, elimina los certificados marcados.

---

## [2026-09-08] — Icono inspirado en FNMT y exe recompilado

### Añadido
- Icono de aplicación en `assets\app.ico` (inspirado en la identidad institucional de la FNMT / Real Casa de la Moneda; diseño propio para el robot, no es el logo oficial registrado).
- `build.cmd` incrusta el icono en `bin\LimpiarCertificados.exe` (`/win32icon`) y la ventana lo usa en la barra de título.

---

## [2026-09-08] — Columnas de lista reducidas

### Eliminado
- Columnas «Uso previsto», «Estado» y «Acción» de la lista. Quedan Emitido para, Emitido por y Fecha de expiración (más las casillas).

---

## [2026-09-08] — Arranque roto tras el cambio de layout

### Corregido
- La app volvía a no abrir: el ajuste del `SplitContainer` lanzaba excepción al iniciar. Sustituido por panel izquierdo fijo (~210px) + lista a la derecha, sin divisores problemáticos.

---

## [2026-09-08] — Más espacio para la lista de certificados

### Corregido
- El panel del árbol («usuario actual») ocupaba casi toda la ventana. Ahora el árbol queda estrecho (~200px) y la lista de certificados usa la mayor parte de la pantalla.

---

## [2026-09-08] — Solo certificados del usuario actual

### Eliminado
- Opción «Certificados - equipo local» del árbol; la app solo muestra almacenes de **usuario actual**.

---

## [2026-09-08] — Corrección: la ventana no abría

### Corregido
- La app se cerraba al instante por un atajo inválido (`Keys.Space` / Alt+F4 en el menú). Quitados; ahora arranca correctamente.
- Añadido manejo de errores al inicio para mostrar un mensaje si vuelve a fallar.

---

## [2026-09-08] — UI al estilo Administrador de certificados de Windows

### Cambiado
- Interfaz rediseñada como aplicación Windows clásica: menú (Archivo/Edición/Ver/Ayuda), barra de herramientas, árbol de almacenes a la izquierda, lista a la derecha, panel de propiedades y barra de estado (estilo certmgr).
- Eliminado el layout tipo panel con títulos grandes; la acción Conservar/Eliminar queda en la columna y en las casillas.

---

## [2026-09-08] — App Windows Forms (mismo tipo que FacturasNas)

### Añadido
- Aplicación C# WinForms `bin\LimpiarCertificados.exe` (código en `src\`), compilada con `build.cmd` vía `csc.exe`, lanzador `LimpiarCertificados.cmd` — mismo patrón que el robot EMAIL / FacturasNas.
- Inventario de certificados por ubicación (`CurrentUser` / `LocalMachine`) y almacén (`My`, `Root`, `CA`, `TrustedPeople`, `AddressBook`).
- Lista con casillas: marcar los que no se necesitan; filtro/marcador de caducados; dry-run por defecto; confirmación antes de borrar.
- `CURRENT_STATE.md` con el estado vivo del proyecto.
- `AGENTS.md` actualizado: stack fijado a WinForms C# como en los otros robots.

### Estado
- Compilación OK. Pendiente prueba real en el PC del usuario.

---

## [2026-09-08] — Inicialización del proyecto

### Añadido
- Creación del proyecto en `C:\CURSOR\ROBOT LIMPIAR CERTIFICADOS INSTALADOS EN EL PC`.
- `AGENTS.md` con el contexto de la tarea (elegir conservar/borrar certificados).
- Este `CHANGELOG.md` y política de `CURRENT_STATE.md` cuando el historial crezca demasiado.
