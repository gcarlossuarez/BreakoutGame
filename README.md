# Breakout Game en F# + Fable + Vite

Este proyecto implementa un pequeño juego estilo **Breakout** usando **F#**, compilado a **JavaScript** mediante **Fable**, y ejecutado en un navegador usando **Vite** como servidor de desarrollo.

---

## 🧠 ¿Qué es Fable?

**Fable es un compilador que traduce código F# a JavaScript**, permitiendo ejecutar programas escritos en F# directamente en el navegador.

En este proyecto:

- Tú escribes el juego en **F#** → `Program.fs`
- **Fable** convierte ese código a **JavaScript** → `output/Program.js`
- El navegador ejecuta ese archivo JS en `index.html`

En pocas palabras:

> **F# → Fable → JavaScript → Navegador**

Esto **sí es totalmente correcto**, y es el propósito principal de Fable.

---

## 📦 Requisitos

Antes de ejecutar o modificar el proyecto, asegúrate de tener instalado:

### ✔ .NET 8 SDK  

### 🏗 Estructura del proyecto
```bash
dotnet --version

```BreakoutGame/
 ├─ Program.fs            # Código del juego en F#
 ├─ BreakoutGame.fsproj   # Proyecto F# + Fable
 ├─ index.html            # Página que carga el JS generado
 ├─ output/               # Carpeta generada con Program.js
 └─ node_modules/         # Dependencias de npm
```

### 🚀 Cómo ejecutar el proyecto
**1. Restaurar herramientas y dependencias**

```bash
dotnet tool restore
npm install
```

**2. Compilar F# → JavaScript**
```bash
dotnet fable --outDir output
```

Esto creará:
```bash
output/Program.js
```

**3. Ejecutar el servidor Vite**
```bash
npx vite
```

Luego abre:
```bash
http://localhost:5173
```

Ahí verás el juego corriendo en el navegador.

## 🔄 ¿Qué hacer cuando modifiques el código?

Cada vez que edites Program.fs:

**✔ Opción 1: recompilar manualmente**
```bash
dotnet fable --outDir output
npx vite
```

**✔ Opción 2: usar compilación automática (modo watch)**

Ejecuta esto en una terminal:
```bash
dotnet fable watch -o output
```

Y en otra terminal:
```bash
npx vite
```

__Fable recompilará automáticamente cada vez que guardes.__