# Godot Vertical Shooter Demo

## Descripción General

Un **shooter vertical espacial** (estilo shoot 'em up clásico) desarrollado en **Godot 4.5 con C#**. El jugador controla una nave que debe sobrevivir oleadas de enemigos mientras acumula puntos. El proyecto implementa múltiples patrones de diseño avanzados con una arquitectura limpia y profesional.

Este proyecto está basado en el tutorial ['How to make a Space Shooter Game in Godot'](https://www.youtube.com/watch?v=QoNukqpolS8) pero ha sido extendido significativamente con arquitectura profesional y características adicionales.

## Características del Juego

### Mecánicas Principales

- **Control del Jugador**: Nave espacial con movimiento horizontal y disparo automático
- **Sistema de Enemigos**:
  - Enemigos básicos con movimiento lineal
  - Meteoros con patrones de movimiento especiales
  - Shooter Enemies que disparan proyectiles
  - Diver Enemies con movimiento en picada
- **Sistema de Niveles**: Dificultad progresiva que aumenta cada 2000 puntos
- **Puntuación**: Sistema de score con high score persistente

### Especificaciones Técnicas

- **Motor**: Godot 4.5
- **Lenguaje**: C# (.NET)
- **Resolución**: 540x960 (orientación vertical)
- **FPS**: Limitado a 50
- **Compatibilidad**: GL Compatibility

## Arquitectura y Patrones de Diseño

El proyecto implementa múltiples patrones de diseño profesionales:

### 1. Patrón Command (scripts/commands/)

Encapsula todas las acciones del jugador como objetos comando:
- `CommandInvoker`: Gestiona la ejecución de comandos
- Comandos de movimiento: `MoveUpCommand`, `MoveDownCommand`, `MoveLeftCommand`, `MoveRightCommand`
- `StopCommand`: Detiene el movimiento del jugador

### 2. Patrón Strategy (scripts/strategies/movement/)

Permite diferentes algoritmos de movimiento intercambiables:
- `LinearMovementStrategy`: Movimiento en línea recta
- `PlayerMovementStrategy`: Movimiento con restricciones de pantalla
- `SinusoidalMovementStrategy`: Movimiento ondulante
- `WobbleMovementStrategy`: Movimiento tambaleante

### 3. Patrón Factory (scripts/factories/)

- `EnemyFactory`: Creación dinámica y aleatoria de enemigos
- Facilita la adición de nuevos tipos de enemigos

### 4. Component Pattern (scripts/components/)

Componentes reutilizables que se pueden adjuntar a cualquier entidad:
- `Health`: Sistema de vida y daño
- `Movement`: Sistema de movimiento configurable

### 5. Patrón Template/Base Class (scripts/core/)

- `BaseEntity`: Clase base abstracta para todas las entidades del juego
- Interfaces: `IDamageable`, `IMovable`, `IInitializable`, `IUpdatable`

## Estructura del Proyecto

```
CORE LAYER (scripts/core/)
├── BaseEntity.cs           # Clase base abstracta
└── interfaces/             # Contratos del sistema

COMPONENTS LAYER (scripts/components/)
├── Health.cs              # Sistema de vida
├── Movement.cs            # Sistema de movimiento
└── interfaces/            # IDamageable, IMovable

ENTITIES LAYER (scripts/entities/)
├── Player.cs              # Nave del jugador
├── Enemy.cs               # Enemigo básico
├── MeteoroEnemy.cs        # Enemigo meteoro
├── ShooterEnemy.cs        # Enemigo que dispara
├── Laser.cs               # Proyectil del jugador
└── EnemyProjectile.cs     # Proyectil enemigo

MANAGERS LAYER (scripts/managers/)
├── GameManager.cs         # Controlador principal del juego
├── ScoreManager.cs        # Gestión de puntuación
├── SpawnManager.cs        # Generación de enemigos
└── LevelManager.cs        # Sistema de niveles/dificultad

SERVICES LAYER (scripts/services/)
├── AudioService.cs        # Reproducción de sonidos
└── SaveService.cs         # Persistencia de datos

UI LAYER (scripts/ui/)
├── Hud.cs                 # Interfaz en juego
└── GameOverScreen.cs      # Pantalla de game over

FACTORIES & COMMANDS
├── factories/             # Creación de objetos
└── commands/              # Patrón Command

STRATEGIES
└── strategies/movement/   # Algoritmos de movimiento
```

## Sistemas del Juego

### Sistema de Niveles (LevelManager)

- **Progresión**: Aumenta cada 2000 puntos
- **Modificadores**:
  - Velocidad de enemigos: +20% por nivel (máximo 300%)
  - Frecuencia de spawn: +15% por nivel (máximo 250%)
- **Escalabilidad**: Dificultad progresiva sin límite superior hasta alcanzar los máximos

### Sistema de Spawn (SpawnManager)

- **Spawn Aleatorio** con probabilidades configurables:
  - Meteoros: 30% de probabilidad (cooldown de 4 segundos)
  - Shooters: 20% de probabilidad (cooldown de 7 segundos)
- **Tiempo Dinámico**: El intervalo de spawn se reduce con el nivel
- **Factory Pattern**: Utiliza EnemyFactory para crear instancias

### Sistema de Audio (AudioService)

Efectos de sonido incluidos:
- Disparos láser del jugador
- Explosiones de enemigos
- Daño al escudo del jugador
- Sonido de Game Over
- Efectos de power-ups (si están implementados)

## Controles

- **Flechas Izquierda/Derecha**: Movimiento horizontal
- **R**: Reiniciar el juego
- **Esc**: Salir del juego
- **Disparo**: Automático (no requiere input)

## Instalación y Uso

### Requisitos Previos

- Godot 4.5 o superior
- .NET SDK instalado
- Soporte para C# en Godot

### Ejecución

1. Abrir el proyecto en Godot
2. La escena principal es `res://scenes/game.tscn`
3. Presionar F5 para ejecutar el juego
4. Jugar y acumular puntos

## Extensibilidad

### Agregar Nuevos Enemigos

1. Crear una nueva clase que herede de `Enemy`
2. Sobrescribir `ConfigureMovementStrategy()` para comportamiento personalizado
3. Configurar propiedades (Speed, Value, Damage, Health)
4. Crear la escena correspondiente en `scenes/`
5. Agregar al array de enemigos en `SpawnManager`

### Agregar Nuevas Estrategias de Movimiento

1. Crear una clase que implemente `IMovementStrategy`
2. Implementar los métodos: `Initialize()`, `Move()`, `GetCurrentDirection()`, `SetDirection()`
3. Asignar la estrategia a la entidad deseada en su método de inicialización

### Agregar Nuevos Comandos

1. Crear una clase que implemente `ICommand`
2. Implementar los métodos: `Execute()`, `Undo()` (si es necesario)
3. Registrar el comando en `CommandInvoker`

## Recursos y Créditos

- **Tutorial Base**: [How to make a Space Shooter Game in Godot](https://www.youtube.com/watch?v=QoNukqpolS8)
- **Assets de Sonido**: [Kenney.nl - Space Shooter Redux](https://kenney.nl/assets/space-shooter-redux)
- **Assets Visuales**: Kenney.nl
- **Documentación**: [Godot C# Differences](https://docs.godotengine.org/en/stable/getting_started/scripting/c_sharp/c_sharp_differences.html)

## Archivos de Configuración

### project.godot

- Assembly: ResearchVertical
- Escena principal: `res://scenes/game.tscn`
- Orientación: Vertical (portrait)

### ResearchVertical.csproj

Configuración del proyecto C# con todas las dependencias necesarias.

## Estado del Proyecto

El proyecto está completamente funcional con todas las características implementadas:
- Sistema de niveles con dificultad progresiva
- Múltiples tipos de enemigos con diferentes comportamientos
- Sistema de puntuación con high score persistente
- Arquitectura limpia y extensible basada en patrones de diseño profesionales