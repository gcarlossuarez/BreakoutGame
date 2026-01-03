(*
====================================================================================
 BREAKOUT GAME – EXPLICACIÓN DEL CÓDIGO EN F# PARA APRENDER PROGRAMACIÓN FUNCIONAL
====================================================================================

Este archivo implementa un mini-juego estilo *Breakout* usando:

- F# (lenguaje funcional)
- Fable (compilador que convierte F# → JavaScript)
- HTML Canvas (para dibujar en el navegador)

------------------------------------------------------------------------------
  ¿QUÉ ES FABLE?
------------------------------------------------------------------------------
Fable toma código F# y lo convierte a JavaScript.  
El navegador **no entiende F# directamente**, pero sí entiende JavaScript.

Por eso, el flujo completo es:

    Program.fs (F#)   →   Fable   →   Program.js (JavaScript)   →   Navegador

El archivo JavaScript generado se carga desde index.html, y ejecuta el juego.

------------------------------------------------------------------------------
  ESTRUCTURA BÁSICA DEL JUEGO
------------------------------------------------------------------------------

El juego usa un "loop" (bucle) basado en:

    window.requestAnimationFrame(update)

Esto significa:

1. Se llama a la función `update`
2. La función dibuja la escena en el canvas
3. La función mueve la pelota, la paleta, calcula colisiones, etc.
4. Cuando termina, pide al navegador que vuelva a llamar a `update`
5. El ciclo se repite ~60 veces por segundo

------------------------------------------------------------------------------
  CONCEPTOS DE F# UTILIZADOS EN EL JUEGO
------------------------------------------------------------------------------

1.  MÓDULO (module App)
------------------------

    module App

Los módulos agrupan funciones y valores.  
Son como "namespaces" simples.

2.  VARIABLES MUTABLES (let mutable)
------------------------------------

Ejemplo:

    let mutable ballX = 250.0

F# es funcional, y por defecto las variables NO cambian.  
Pero en un juego sí necesitamos cambiar valores constantemente → usamos `mutable`.

3.  FUNCIONES

    let drawBall() = 
        ...

Las funciones en F# NO necesitan parámetros obligatoriamente.  
Se pueden llamar simplemente como:

    drawBall()

4.  OPERADOR PIPE (|>)

Ejemplo:

    window.requestAnimationFrame(update) |> ignore

El operador `|>` pasa el resultado de la función de la izquierda 
a la función de la derecha.

Es equivalente a:

    ignore (window.requestAnimationFrame(update))

5.  CASTING EN F#

    document.getElementById("game") :?> HTMLCanvasElement

`:?>` significa "castear a este tipo".  
F# exige tipos exactos para trabajar con objetos del navegador.

------------------------------------------------------------------------------
  VISUALIZACIÓN DEL CANVAS (DIAGRAMA ASCII)
------------------------------------------------------------------------------

El canvas tiene 500 píxeles de ancho y 300 de alto:

         0 ──────────────────────────────────────────→ x
         |
         |      ● Pelota (ballX, ballY)
         |
         |
         |
         |______________________________  ← Paleta (paddle)
       y=300

Coordenadas en canvas:

- (0,0) está arriba a la izquierda
- x aumenta hacia la derecha
- y aumenta hacia abajo

------------------------------------------------------------------------------
  LÓGICA DE MOVIMIENTO DE LA PELOTA
------------------------------------------------------------------------------

La pelota tiene:

    ballX, ballY    → posición
    dx, dy          → velocidad en cada eje

En cada frame:

    ballX <- ballX + dx
    ballY <- ballY + dy

Las colisiones se detectan así:

1. Contra paredes:

    if ballX + dx > canvas.width - radio
        dx <- -dx    ← rebota invertiendo el signo

2. Contra el techo:

    if ballY + dy < radio
        dy <- -dy

3. Contra la paleta:

    if ballY + dy > alturaPaleta - radio && 
       ballX está entre paddleX y paddleX+paddleWidth
       
       dy <- -dy   ← rebota hacia arriba

4. Si no golpea la paleta:

       perder vida
       reiniciar pelota

------------------------------------------------------------------------------
  DIAGRAMA DE COLISIÓN DE LA PALETA
------------------------------------------------------------------------------

      ballY
        ↓
      (ballX)

       ┌──────────────────────────────┐
       │                              │
       │            PALETA            │ ← paddleX .. paddleX + ancho
       └──────────────────────────────┘

Condición en el código:

    if ballX > paddleX && ballX < paddleX + paddleWidth then
        dy <- -dy

------------------------------------------------------------------------------
  EVENTOS DE TECLADO
------------------------------------------------------------------------------

Se captura A/D como en JavaScript:

    document.addEventListener("keydown", fun e ->
        let ke = e :?> KeyboardEvent
        if ke.key = "a" then leftPressed <- true
        if ke.key = "d" then rightPressed <- true
    )

El movimiento ocurre en update():

    if leftPressed then paddleX <- paddleX - 4.
    if rightPressed then paddleX <- paddleX + 4.

------------------------------------------------------------------------------
  DIBUJADO EN CANVAS
------------------------------------------------------------------------------

Se usan funciones equivalentes a JavaScript:

    ctx.beginPath()
    ctx.arc(x, y, radio, 0., Math.PI*2.)
    ctx.fill()
    ctx.closePath()

Para rectángulos:

    ctx.rect(x, y, ancho, alto)
    ctx.fill()

------------------------------------------------------------------------------
  RESUMEN FINAL
------------------------------------------------------------------------------

Este archivo demuestra:

✔ Cómo manipular canvas desde F#  
✔ Cómo usar Fable para compilar a JavaScript  
✔ Cómo mantener un estado mutable dentro de un loop de animación  
✔ Cómo manejar entrada del teclado  
✔ Cómo detectar colisiones geométricas simples  
✔ Cómo producir un juego funcionando 100% en el navegador  

Este comentario sirve como guía para retomar el proyecto incluso si pasa mucho tiempo.
*)

module App

open Browser.Dom
open Browser.Types
open Fable.Core

let canvas =
    document.getElementById("game") :?> HTMLCanvasElement

let ctx =
    canvas.getContext_2d()

let mutable score = 0
let mutable lives = 3

let mutable paddleX = 200.0
let paddleWidth = 80.0
let paddleHeight = 12.0

let mutable ballX = 250.0
let mutable ballY = 150.0
let mutable dx = 10.0
let mutable dy = -2.0
let ballRadius = 8.0

let mutable leftPressed = false
let mutable rightPressed = false

document.addEventListener("keydown", fun e ->
    let ke = e :?> KeyboardEvent
    if ke.key = "a" then leftPressed <- true
    if ke.key = "d" then rightPressed <- true
)

document.addEventListener("keyup", fun e ->
    let ke = e :?> KeyboardEvent
    if ke.key = "a" then leftPressed <- false
    if ke.key = "d" then rightPressed <- false
)

let drawBall() =
    ctx.beginPath()
    ctx.arc(ballX, ballY, ballRadius, 0., System.Math.PI * 2.)
    ctx.fillStyle <- U3.Case1 "red"
    ctx.fill()
    ctx.closePath()

let drawPaddle() =
    ctx.beginPath()
    ctx.rect(paddleX, 280., paddleWidth, paddleHeight)
    ctx.fillStyle <- U3.Case1 "black"
    ctx.fill()
    ctx.closePath()

let drawHUD() =
    ctx.font <- "16px Arial"
    ctx.fillStyle <- U3.Case1 "black"

    ctx.fillText($"Score: {score}", 10., 20.)
    ctx.fillText($"Lives: {lives}", 420., 20.)

let rec update (_: float) =
    ctx.clearRect(0., 0., canvas.width, canvas.height)

    drawHUD()

    drawBall()
    drawPaddle()

    // Rebote lateral
    if ballX + dx > float canvas.width - ballRadius || ballX + dx < ballRadius then
        dx <- -dx

    // Rebote arriba
    if ballY + dy < ballRadius then
        dy <- -dy
    // Zona de la paleta
    elif ballY + dy > 280. - ballRadius then
        if ballX > paddleX && ballX < paddleX + paddleWidth then
            dy <- -dy
            score <- score + 1
        else
            lives <- lives - 1

            if lives = 0 then
                // Reiniciar todo
                lives <- 3
                score <- 0
                paddleX <- 200.
                dx <- 2.
                dy <- -2.
                ballX <- 250.
                ballY <- 150.
            else
                // Solo reposicionar la pelota
                ballX <- 250.
                ballY <- 150.
                dx <- 2.
                dy <- -2.

    ballX <- ballX + dx
    ballY <- ballY + dy

    // Movimiento de la paleta
    if leftPressed && paddleX > 0. then
        paddleX <- paddleX - 4.

    if rightPressed && paddleX < float canvas.width - paddleWidth then
        paddleX <- paddleX + 4.

    window.requestAnimationFrame(update) |> ignore

// Arrancar animación
window.requestAnimationFrame(update) |> ignore
