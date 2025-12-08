# Godot Vertical Shooter - Space Combat Game 🚀

## Descripción General

Un **shooter vertical espacial** (estilo shoot 'em up clásico) desarrollado en **Godot 4.5 con C#** que implementa una arquitectura profesional basada en **Principios SOLID** y **Patrones de Diseño**. El jugador controla una nave espacial que debe sobrevivir oleadas infinitas de enemigos mientras acumula puntos y progresa a través de niveles con dificultad creciente.

### Características Destacadas

- ✅ **Arquitectura Limpia**: Implementa SOLID, DRY, KISS y múltiples patrones de diseño
- 🔐 **Sistema de Autenticación**: Login seguro con encriptación SHA-256
- 👥 **Múltiples Usuarios**: Cada jugador tiene su propio perfil y high score
- 📊 **Sistema de Niveles**: Dificultad progresiva con escalado de enemigos
- 🎮 **3 Tipos de Enemigos**: Básicos, Meteoros serpenteantes y Shooters
- 🏆 **Leaderboard Global**: Top 3 mejores puntuaciones de todos los usuarios
- 💾 **Persistencia de Datos**: Guardado automático en JSON

Este proyecto está basado en el tutorial ['How to make a Space Shooter Game in Godot'](https://www.youtube.com/watch?v=QoNukqpolS8) pero ha sido completamente reescrito con arquitectura profesional, patrones de diseño y características avanzadas.

## Características del Juego

### Mecánicas Principales

- **Control del Jugador**: Nave espacial con movimiento horizontal y disparo automático
- **Sistema de Enemigos**:
  - Enemigos básicos con movimiento lineal
  - Meteoros con patrones de movimiento especiales
  - Shooter Enemies que disparan proyectiles
  - Diver Enemies con movimiento en picada
- **Sistema de Niveles**: Dificultad progresiva que aumenta cada 2000 puntos
- **Puntuación**: Sistema de score con récords personalizados por usuario
- **Autenticación**: Sistema completo de login con contraseñas encriptadas

### Especificaciones Técnicas

- **Motor**: Godot 4.5
- **Lenguaje**: C# (.NET)
- **Resolución**: 540x960 (orientación vertical)
- **FPS**: Limitado a 50
- **Compatibilidad**: GL Compatibility

## 🏛️ Principios SOLID Implementados

El proyecto sigue rigurosamente los 5 principios SOLID:

### 1. **SRP - Single Responsibility Principle** (Principio de Responsabilidad Única)
Cada clase tiene una única razón para cambiar:
- ✅ `GameManager.cs`: Solo coordina managers (Facade)
- ✅ `ScoreManager.cs`: Solo gestiona puntuaciones
- ✅ `AudioService.cs`: Solo maneja audio
- ✅ `PlayerInputHandler.cs`: Solo procesa input del jugador
- ✅ `LevelManager.cs`: Solo gestiona niveles y dificultad
- ✅ `SpawnManager.cs`: Solo genera enemigos

### 2. **OCP - Open/Closed Principle** (Abierto/Cerrado)
Clases abiertas a extensión, cerradas a modificación:
- ✅ `BaseEntity.cs`: Clase base abstracta que se extiende sin modificar
- ✅ `IMovementStrategy`: Nuevas estrategias sin cambiar código existente
- ✅ `IEnemyFactory`: Nuevos tipos de enemigos sin modificar factories existentes
- ✅ `ICommand`: Nuevos comandos sin modificar CommandInvoker

### 3. **LSP - Liskov Substitution Principle** (Sustitución de Liskov)
Las clases derivadas son sustituibles por sus bases:
- ✅ `Enemy`, `MeteoroEnemy`, `ShooterEnemy` sustituyen a `BaseEntity`
- ✅ Todas las estrategias de movimiento implementan `IMovementStrategy`
- ✅ Todos los comandos implementan `ICommand`

### 4. **ISP - Interface Segregation Principle** (Segregación de Interfaces)
Interfaces específicas en lugar de genéricas:
- ✅ `IDamageable`: Solo para entidades que reciben daño
- ✅ `IMovable`: Solo para entidades con movimiento
- ✅ `IInitializable`: Solo para objetos que necesitan inicialización
- ✅ `IAudioService`: Solo métodos relacionados con audio
- ✅ `IUserDatabaseService`: Solo operaciones de base de datos

### 5. **DIP - Dependency Inversion Principle** (Inversión de Dependencias)
Depender de abstracciones, no de implementaciones concretas:
- ✅ `GameManager` depende de `IAudioService`, no de `AudioService`
- ✅ `AuthService` usa `IUserDatabaseService` y `IUserLockService`
- ✅ `MeteoroEnemy` y `ShooterEnemy` dependen de `IAudioService`
- ✅ Inyección de Dependencias en `ScoreManager.Initialize(UserManager)`

---

## 🎨 Patrones de Diseño Implementados

### 1. **Command Pattern** (scripts/commands/)
**Propósito**: Encapsular acciones como objetos, permitiendo deshacer/rehacer operaciones.

**Implementación**:
```csharp
// Interfaz base
public interface ICommand {
    void Execute();
    void Undo();
}

// Comandos concretos
MoveUpCommand, MoveDownCommand, MoveLeftCommand, MoveRightCommand, StopCommand

// Invocador
CommandInvoker // Ejecuta y gestiona comandos
```

**Ubicación**: `scripts/commands/`
- `CommandInvoker.cs`: Gestor central de comandos
- `movement/MoveUpCommand.cs`, `MoveDownCommand.cs`, etc.

**Ventajas**:
- ✅ Input del jugador desacoplado de la lógica de movimiento
- ✅ Facilita replay systems y grabación de partidas
- ✅ Permite sistema de undo/redo

---

### 2. **Strategy Pattern** (scripts/strategies/movement/)
**Propósito**: Definir una familia de algoritmos intercambiables.

**Implementación**:
```csharp
// Interfaz estrategia
public interface IMovementStrategy {
    void Initialize(Node2D entity);
    void Move(Node2D entity, double delta, float speed, Vector2 direction);
}

// Estrategias concretas
LinearMovementStrategy      // Movimiento recto
WobbleMovementStrategy      // Movimiento serpenteante (meteoros)
PlayerMovementStrategy      // Movimiento con límites de pantalla
```

**Ubicación**: `scripts/strategies/movement/`

**Uso en el juego**:
- `Enemy.cs` usa `LinearMovementStrategy`
- `MeteoroEnemy.cs` usa `WobbleMovementStrategy`
- `Player.cs` usa `PlayerMovementStrategy`

**Ventajas**:
- ✅ Comportamiento de movimiento intercambiable en runtime
- ✅ Fácil agregar nuevos patrones de movimiento
- ✅ Cumple con OCP

---

### 3. **Abstract Factory Pattern** (scripts/factories/)
**Propósito**: Crear familias de objetos relacionados sin especificar sus clases concretas.

**Implementación**:
```csharp
// Interfaz Factory abstracta
public interface IEnemyFactory {
    Enemy CreateEnemy(Vector2 position);
    string GetEnemyType();
    int GetDifficultyLevel();
}

// Factories concretas
BasicEnemyFactory      // Crea Enemy (dificultad 1)
MeteoroEnemyFactory    // Crea MeteoroEnemy (dificultad 2)
ShooterEnemyFactory    // Crea ShooterEnemy (dificultad 3)

// Coordinador de factories
EnemyFactoryManager    // Gestiona múltiples factories
```

**Ubicación**: `scripts/factories/`
- `interfaces/IEnemyFactory.cs`: Interfaz abstracta
- `BasicEnemyFactory.cs`, `MeteoroEnemyFactory.cs`, `ShooterEnemyFactory.cs`: Implementaciones concretas
- `EnemyFactoryManager.cs`: Coordinador con selección ponderada
- `EnemyFactory.cs`: Facade para compatibilidad retroactiva

**Características avanzadas**:
- Selección aleatoria de enemigos
- Selección ponderada por dificultad
- Creación específica por tipo
- Registro dinámico de nuevas factories

**Ventajas**:
- ✅ Creación de enemigos extensible
- ✅ Balanceo de dificultad por tipo
- ✅ Fácil agregar nuevos tipos de enemigos

---

### 4. **Facade Pattern** (scripts/managers/)
**Propósito**: Proporcionar una interfaz simplificada a un subsistema complejo.

**Implementación principal**: `GameManager.cs`
```csharp
public partial class GameManager : Node2D {
    // Subsistemas complejos
    private InputManager _inputManager;
    private BackgroundManager _backgroundManager;
    private GameStateManager _gameStateManager;
    private ScoreManager _scoreManager;
    private SpawnManager _spawnManager;
    private LevelManager _levelManager;
    
    // Interfaz simplificada
    public void Initialize() { /* coordina todos los subsistemas */ }
}
```

**Ubicación**: `scripts/managers/GameManager.cs`

**Otros Facades**:
- `EnemyFactory.cs`: Facade sobre el sistema de factories complejo

**Ventajas**:
- ✅ Oculta complejidad de subsistemas
- ✅ Punto de entrada único y simple
- ✅ Reduce acoplamiento entre subsistemas

---

### 5. **Component Pattern** (scripts/components/)
**Propósito**: Permitir que una sola entidad abarque múltiples dominios sin acoplamiento.

**Implementación**:
```csharp
// Componentes reutilizables
Health.cs      // Gestiona vida, daño y muerte
Movement.cs    // Gestiona movimiento y estrategias
PlayerInputHandler.cs  // Gestiona input del jugador

// Uso en entidades
public partial class Enemy : BaseEntity {
    protected Health _healthComponent;
    protected Movement _movementComponent;
}
```

**Ubicación**: `scripts/components/`

**Ventajas**:
- ✅ Composición sobre herencia
- ✅ Componentes reutilizables entre entidades
- ✅ Fácil agregar/quitar funcionalidad

---

### 6. **Template Method Pattern** (scripts/core/)
**Propósito**: Definir el esqueleto de un algoritmo, dejando que las subclases redefinan ciertos pasos.

**Implementación**: `BaseEntity.cs`
```csharp
public abstract partial class BaseEntity : Area2D {
    // Template method
    public virtual void Initialize() {
        _healthComponent = GetNode<Health>("Health");
        _movementComponent = GetNode<Movement>("Movement");
        ConfigureMovementStrategy();  // Hook method
    }
    
    // Hook method (sobrescrito por subclases)
    protected virtual void ConfigureMovementStrategy() { }
    
    protected virtual void OnDied() { }  // Otro hook
}
```

**Subclases que personalizan**:
- `Enemy.cs`: Estrategia lineal
- `MeteoroEnemy.cs`: Estrategia serpenteante
- `ShooterEnemy.cs`: Estrategia lineal + sistema de disparo

**Ventajas**:
- ✅ Reutilización de código común
- ✅ Puntos de extensión claros
- ✅ Flujo de inicialización consistente

---

### 7. **Dependency Injection Pattern**
**Propósito**: Inyectar dependencias en lugar de crearlas internamente.

**Implementaciones**:

**Manual DI** (Constructor/Method Injection):
```csharp
// GameManager.cs
public void Initialize() {
    _audioService = GetNodeOrNull<AudioService>("SFX");  // DI manual
    _scoreManager.Initialize(_userManager);  // Method injection
}

// AuthService.cs
public static void Initialize(
    IUserLockService lockService = null, 
    IUserDatabaseService userDatabase = null) {
    _lockService = lockService ?? new UserLockService();
    _userDatabase = userDatabase ?? new UserDatabaseService();
}
```

**Ubicaciones**:
- `GameManager.cs`: Inyecta `IAudioService`, `UserManager`
- `ScoreManager.cs`: Recibe `UserManager` vía método
- `AuthService.cs`: Recibe servicios opcionales
- `MeteoroEnemy.cs`, `ShooterEnemy.cs`: Obtienen `IAudioService`

**Ventajas**:
- ✅ Testabilidad (mocks/stubs)
- ✅ Cumple DIP (Dependency Inversion)
- ✅ Desacoplamiento
- ✅ Configuración flexible

---

### 8. **Observer Pattern** (mediante Signals de Godot)
**Propósito**: Notificar cambios a múltiples objetos interesados.

**Implementación con Signals**:
```csharp
// Health.cs
[Signal] public delegate void DiedEventHandler();
[Signal] public delegate void HealthChangedEventHandler(int currentHealth);

// Enemy.cs
[Signal] public delegate void KilledEventHandler(Enemy enemy);

// LevelManager.cs
[Signal] public delegate void LevelChangedEventHandler(uint newLevel);

// SpawnManager.cs
[Signal] public delegate void EnemySpawnedEventHandler(Enemy enemy);
```

**Suscripciones**:
```csharp
_healthComponent.Died += OnDied;
_levelManager.LevelChanged += OnLevelChanged;
_spawnManager.EnemySpawned += OnEnemySpawned;
```

**Ventajas**:
- ✅ Comunicación desacoplada
- ✅ Múltiples observadores por evento
- ✅ Nativo de Godot (performance optimizado)

---

### 9. **State Pattern** (implícito en GameStateManager)
**Propósito**: Permitir que un objeto altere su comportamiento cuando su estado interno cambia.

**Implementación**: `GameStateManager.cs`
```csharp
public enum GameState {
    Menu,
    Playing,
    Paused,
    GameOver
}

// Transiciones de estado
public void StartGame() { /* Menu → Playing */ }
public void PauseGame() { /* Playing → Paused */ }
public void GameOver() { /* Playing → GameOver */ }
```

**Ubicación**: `scripts/managers/GameStateManager.cs`

---

## 📦 Arquitectura en Capas

El proyecto sigue una arquitectura limpia en capas:

## 📦 Arquitectura en Capas

El proyecto sigue una arquitectura limpia en capas:

```
┌─────────────────────────────────────────────────────────────┐
│                     UI LAYER (Presentación)                  │
│  - LoginScreen.cs: Autenticación de usuarios                │
│  - Hud.cs: Interfaz durante el juego                        │
│  - GameOverScreen.cs: Pantalla de fin de juego + leaderboard│
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                   MANAGERS LAYER (Orquestación)              │
│  - GameManager.cs: Facade principal (coordina todo)         │
│  - ScoreManager.cs: Gestión de puntuaciones                 │
│  - SpawnManager.cs: Generación de enemigos                  │
│  - LevelManager.cs: Sistema de niveles y dificultad         │
│  - UserManager.cs: Gestión de sesión de usuario actual      │
│  - InputManager.cs: Procesamiento de input                  │
│  - BackgroundManager.cs: Scroll del fondo                   │
│  - GameStateManager.cs: Estados del juego                   │
│  - NodeInitializer.cs: Inicialización de nodos              │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                  SERVICES LAYER (Lógica de Negocio)         │
│  - AuthService.cs: Autenticación y sesiones                 │
│  - AudioService.cs: Reproducción de audio                   │
│  - UserDatabaseService.cs: CRUD de usuarios                 │
│  - PasswordService.cs: Encriptación SHA-256                 │
│  - SaveService.cs: Persistencia en JSON                     │
│  - UserLockService.cs: Control de bloqueos                  │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                   ENTITIES LAYER (Gameplay)                  │
│  - Player.cs: Nave del jugador                              │
│  - Enemy.cs: Enemigo básico                                 │
│  - MeteoroEnemy.cs: Enemigo meteoro (serpenteante)         │
│  - ShooterEnemy.cs: Enemigo que dispara                     │
│  - Laser.cs: Proyectil del jugador                          │
│  - EnemyProjectile.cs: Proyectil enemigo                    │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                 COMPONENTS LAYER (Funcionalidad)             │
│  - Health.cs: Sistema de vida/daño                          │
│  - Movement.cs: Sistema de movimiento                       │
│  - PlayerInputHandler.cs: Manejo de input                   │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    CORE LAYER (Base)                         │
│  - BaseEntity.cs: Clase base abstracta                      │
│  - interfaces/: IDamageable, IMovable, IInitializable       │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│              PATTERNS LAYER (Patrones de Diseño)             │
│  - commands/: Command Pattern (input)                       │
│  - strategies/: Strategy Pattern (movimiento)               │
│  - factories/: Abstract Factory (enemigos)                  │
└─────────────────────────────────────────────────────────────┘
                            ↓
┌─────────────────────────────────────────────────────────────┐
│                    MODELS & UTILS                            │
│  - User.cs: Modelo de usuario                               │
│  - UserGameStats.cs: Estadísticas de juego                  │
│  - Constants.cs: Constantes del juego                       │
│  - ColorPalette.cs: Paleta de colores                       │
│  - FileHelper.cs: Operaciones de archivos                   │
└─────────────────────────────────────────────────────────────┘
```

### Flujo de Datos (Data Flow)

```
Usuario → LoginScreen → AuthService → UserDatabaseService
                            ↓
                      GameManager (Facade)
                            ↓
        ┌───────────────────┼───────────────────┐
        ↓                   ↓                   ↓
  InputManager      SpawnManager         ScoreManager
        ↓                   ↓                   ↓
  CommandInvoker      EnemyFactory         UserManager
        ↓                   ↓
     Player             Enemies
        ↓                   ↓
    Movement            Movement
        ↓                   ↓
   Strategy            Strategy
```

---

## 🗂️ Estructura Detallada del Proyecto

### CORE LAYER (`scripts/core/`)
**Propósito**: Clases base y contratos fundamentales

| Archivo | Responsabilidad | Patrones |
|---------|----------------|----------|
| `BaseEntity.cs` | Clase base abstracta para todas las entidades | Template Method |
| `interfaces/IDamageable.cs` | Contrato para entidades que reciben daño | ISP |
| `interfaces/IMovable.cs` | Contrato para entidades con movimiento | ISP |
| `interfaces/IInitializable.cs` | Contrato para objetos que necesitan inicialización | ISP |

---

### COMPONENTS LAYER (`scripts/components/`)
**Propósito**: Componentes reutilizables para composición

| Archivo | Responsabilidad | Principios SOLID | Patrones |
|---------|----------------|------------------|----------|
| `Health.cs` | Gestiona vida, daño y muerte | SRP | Component, Observer |
| `Movement.cs` | Gestiona movimiento con estrategias | SRP, OCP, DIP | Strategy, Component |
| `PlayerInputHandler.cs` | Procesa input del jugador | SRP | Command |
| `interfaces/IMovable.cs` | Interfaz para movimiento | ISP | - |

**Detalles**:
- `Health.cs`: Emite señal `Died` cuando la vida llega a 0
- `Movement.cs`: Usa `IMovementStrategy` para diferentes comportamientos
- `PlayerInputHandler.cs`: Convierte input en comandos ejecutables

---

### ENTITIES LAYER (`scripts/entities/`)
**Propósito**: Entidades del juego con lógica específica

| Archivo | Tipo | Movimiento | Características |
|---------|------|------------|-----------------|
| `Player.cs` | Jugador | PlayerMovementStrategy | Disparo automático, límites de pantalla |
| `Enemy.cs` | Enemigo básico | LinearMovementStrategy | Movimiento recto hacia abajo |
| `MeteoroEnemy.cs` | Enemigo meteoro | WobbleMovementStrategy | Movimiento serpenteante |
| `ShooterEnemy.cs` | Enemigo shooter | LinearMovementStrategy | Dispara proyectiles al jugador |
| `Laser.cs` | Proyectil jugador | LinearMovementStrategy | Daño: 10, Velocidad: 400 |
| `EnemyProjectile.cs` | Proyectil enemigo | LinearMovementStrategy | Daño: 15, Velocidad: 200 |

**Jerarquía de Herencia**:
```
BaseEntity (abstract)
├── Player
└── Enemy
    ├── MeteoroEnemy
    └── ShooterEnemy
```

---

### MANAGERS LAYER (`scripts/managers/`)
**Propósito**: Coordinación y orquestación de sistemas

| Archivo | Responsabilidad | Principios | Patrones |
|---------|----------------|------------|----------|
| `GameManager.cs` | Coordina todos los managers | SRP, DIP | Facade, DI |
| `ScoreManager.cs` | Gestiona puntuación y high scores | SRP, DIP | DI |
| `SpawnManager.cs` | Genera enemigos con balance | SRP, OCP | Factory, Observer |
| `LevelManager.cs` | Sistema de niveles y dificultad | SRP | Observer |
| `UserManager.cs` | Gestiona usuario actual en sesión | SRP | - |
| `InputManager.cs` | Procesa input global | SRP | Command |
| `BackgroundManager.cs` | Scroll del fondo | SRP | - |
| `GameStateManager.cs` | Gestiona estados del juego | SRP | State |
| `NodeInitializer.cs` | Inicializa referencias de nodos | SRP | - |

**Detalles importantes**:

**GameManager.cs** (Facade principal):
- Coordina 9 subsistemas
- Implementa Dependency Injection para `AudioService`
- Punto de entrada único para el juego

**SpawnManager.cs**:
- Spawn aleatorio con probabilidades:
  - Meteoros: 30% (cooldown 4s)
  - Shooters: 20% (cooldown 7s)
  - Básicos: 50%
- Aplica multiplicadores de nivel a enemigos
- Usa `EnemyFactory` para crear instancias

**LevelManager.cs**:
- Sube de nivel cada 2000 puntos
- Modificadores por nivel:
  - Velocidad enemigos: +20% (máx 300%)
  - Frecuencia spawn: +15% (máx 250%)

---

### SERVICES LAYER (`scripts/services/`)
**Propósito**: Lógica de negocio y servicios compartidos

| Archivo | Responsabilidad | Principios | Interfaces |
|---------|----------------|------------|------------|
| `AuthService.cs` | Autenticación y sesiones | SRP, DIP | - |
| `AudioService.cs` | Reproducción de sonidos | SRP, DIP | IAudioService |
| `UserDatabaseService.cs` | CRUD de usuarios | SRP, OCP | IUserDatabaseService |
| `PasswordService.cs` | Encriptación SHA-256 | SRP | - |
| `SaveService.cs` | Persistencia JSON | SRP | - |
| `UserLockService.cs` | Control de bloqueos | SRP | IUserLockService |

**Detalles de seguridad**:

**AuthService.cs**:
- Login/Logout/Registro
- Validaciones:
  - Usuario: 3-20 caracteres alfanuméricos
  - Contraseña: mínimo 4 caracteres
- Integración con `UserLockService` (máx 5 intentos)

**PasswordService.cs**:
- Encriptación SHA-256 con salt único por usuario
- Historial de últimas 5 contraseñas (no reutilizables)
- Validación de contraseña anterior en cambios

**UserDatabaseService.cs**:
- Almacenamiento en `user://user_data.json`
- Operaciones: Create, Read, Update, Delete
- Top scores global

---

### UI LAYER (`scripts/ui/`)
**Propósito**: Interfaces de usuario y presentación

| Archivo | Responsabilidad | Características |
|---------|----------------|-----------------|
| `LoginScreen.cs` | Sistema de login completo | Registro, login, cambio de contraseña |
| `Hud.cs` | Interfaz durante el juego | Score, high score, nivel, usuario, vidas |
| `GameOverScreen.cs` | Pantalla de fin de juego | Score final, high score, top 3 global, logout |

**LoginScreen.cs**:
- Formulario de login/registro
- Validaciones en tiempo real
- Mensajes de error claros
- Auto-login de última sesión (opcional)

**GameOverScreen.cs**:
- Muestra puntuación final
- Leaderboard top 3 de todos los usuarios
- Botones: Reiniciar, Logout
- Actualización automática de high scores

---

### FACTORIES LAYER (`scripts/factories/`)
**Propósito**: Creación de objetos con Abstract Factory Pattern

| Archivo | Responsabilidad | Patrón |
|---------|----------------|--------|
| `interfaces/IEnemyFactory.cs` | Contrato de factory | Abstract Factory |
| `BasicEnemyFactory.cs` | Crea enemigos básicos (dificultad 1) | Concrete Factory |
| `MeteoroEnemyFactory.cs` | Crea meteoros (dificultad 2) | Concrete Factory |
| `ShooterEnemyFactory.cs` | Crea shooters (dificultad 3) | Concrete Factory |
| `EnemyFactoryManager.cs` | Coordina factories, selección ponderada | Factory Manager |
| `EnemyFactory.cs` | Facade de compatibilidad (deprecated) | Facade |

**Métodos avanzados** en `EnemyFactoryManager`:
```csharp
CreateRandomEnemy(Vector2 position)                    // Aleatorio simple
CreateWeightedRandomEnemy(Vector2 position, int maxDiff) // Ponderado por dificultad
CreateEnemyByType(string type, Vector2 position)       // Por tipo específico
GetAvailableEnemyTypes()                               // Lista tipos disponibles
```

---

### COMMANDS LAYER (`scripts/commands/`)
**Propósito**: Command Pattern para input del jugador

| Archivo | Responsabilidad |
|---------|----------------|
| `interfaces/ICommand.cs` | Contrato de comando |
| `CommandInvoker.cs` | Ejecuta y gestiona comandos |
| `movement/MoveUpCommand.cs` | Movimiento arriba |
| `movement/MoveDownCommand.cs` | Movimiento abajo |
| `movement/MoveLeftCommand.cs` | Movimiento izquierda |
| `movement/MoveRightCommand.cs` | Movimiento derecha |
| `movement/StopCommand.cs` | Detener movimiento |

**Flujo de ejecución**:
```
Input → PlayerInputHandler → Command → CommandInvoker → Player.Movement
```

---

### STRATEGIES LAYER (`scripts/strategies/movement/`)
**Propósito**: Strategy Pattern para algoritmos de movimiento

| Archivo | Comportamiento | Usado en |
|---------|----------------|----------|
| `IMovementStrategy.cs` | Interfaz de estrategia | - |
| `LinearMovementStrategy.cs` | Movimiento recto | Enemy, Laser, EnemyProjectile |
| `WobbleMovementStrategy.cs` | Movimiento serpenteante | MeteoroEnemy |
| `PlayerMovementStrategy.cs` | Movimiento con límites | Player |

**Características de cada estrategia**:

**LinearMovementStrategy**:
- Movimiento en línea recta
- Dirección configurable
- Velocidad constante

**WobbleMovementStrategy**:
- Movimiento ondulante/serpenteante
- Amplitud y frecuencia aleatorias
- Rotación continua del sprite
- Perfecto para meteoros

**PlayerMovementStrategy**:
- Limita movimiento a los bordes de la pantalla
- Evita que el jugador salga del área visible
- Mantiene al jugador dentro del viewport

---

### MODELS LAYER (`scripts/models/`)
**Propósito**: Modelos de datos

| Archivo | Propósito |
|---------|-----------|
| `User.cs` | Modelo de usuario con datos de autenticación |
| `UserGameStats.cs` | Estadísticas de juego del usuario |

**User.cs** contiene:
```csharp
- Username: string
- PasswordHash: string
- PasswordSalt: string
- HighScore: uint
- GamesPlayed: uint
- TotalScore: uint
- CreatedAt: DateTime
- LastLoginAt: DateTime
- IsLocked: bool
- FailedLoginAttempts: int
- PasswordHistory: List<string>
```

---

### UTILS LAYER (`scripts/utils/`)
**Propósito**: Utilidades y helpers

| Archivo | Propósito |
|---------|-----------|
| `Constants.cs` | Constantes del juego (velocidades, daño, grupos) |
| `ColorPalette.cs` | Paleta de colores para UI |
| `FileHelper.cs` | Operaciones de archivos |
| `UserTestScript.cs` | Script de pruebas de usuarios |

---

## 🎮 Características del Juego

### Mecánicas Principales

#### 1. **Sistema de Combate**
- **Disparo Automático**: El jugador dispara automáticamente cada 0.3 segundos
- **Proyectiles del Jugador**: Láseres que viajan hacia arriba (daño: 10)
- **Proyectiles Enemigos**: Los ShooterEnemy disparan hacia el jugador (daño: 15)
- **Colisiones**: Sistema de áreas 2D con detección precisa

#### 2. **Sistema de Enemigos**

| Tipo | Velocidad Base | Vida | Daño | Valor | Movimiento | Especial |
|------|---------------|------|------|-------|------------|----------|
| Enemy (Básico) | 100 | 30 | 10 | 100 | Lineal ↓ | - |
| MeteoroEnemy | 60 | 50 | 30 | 200 | Serpenteante | Rotación visual |
| ShooterEnemy | 80 | 30 | 25 | 150 | Lineal ↓ | Dispara proyectiles |

#### 3. **Sistema de Niveles**
- **Progresión**: Cada 2000 puntos subes de nivel
- **Modificadores por nivel**:
  - Velocidad enemigos: +20% por nivel
  - Frecuencia de spawn: +15% por nivel
- **Límites**:
  - Velocidad máxima: 300% (nivel 10)
  - Spawn rate máximo: 250% (nivel 10)
- **Sin límite superior**: Puedes seguir jugando indefinidamente

#### 4. **Sistema de Puntuación**
- Puntos por eliminar enemigos (valor variable)
- High score personal por usuario
- Leaderboard global (top 3)
- Persistencia automática

#### 5. **Sistema de Vidas**
- Jugador: 100 HP
- Muerte inmediata al llegar a 0
- Game Over con opción de reiniciar o logout

---

## 🔐 Sistema de Usuarios

### Arquitectura de Autenticación

```
LoginScreen
    ↓
AuthService.Login(username, password)
    ↓
UserDatabaseService.GetUser(username)
    ↓
PasswordService.VerifyPassword(password, hash, salt)
    ↓
UserLockService.CheckLockStatus(username)
    ↓
SessionService (implícito en AuthService.CurrentUser)
    ↓
GameManager (usuario autenticado)
```

### Características de Seguridad

#### 1. **Encriptación de Contraseñas**
- Algoritmo: SHA-256
- Salt único por usuario (generado aleatoriamente)
- Hash almacenado: `SHA256(password + salt)`
- Las contraseñas nunca se almacenan en texto plano

#### 2. **Historial de Contraseñas**
- Almacena las últimas 5 contraseñas hasheadas
- No permite reutilizar contraseñas antiguas
- Incrementa seguridad contra ataques

#### 3. **Sistema de Bloqueo**
- Máximo 5 intentos fallidos de login
- Bloqueo automático tras 5 fallos
- Desbloqueo manual requerido (implementable por admin)

#### 4. **Validaciones**

**Nombre de usuario**:
- Mínimo: 3 caracteres
- Máximo: 20 caracteres
- Solo alfanuméricos (a-z, A-Z, 0-9)
- Sin espacios ni caracteres especiales

**Contraseña**:
- Mínimo: 4 caracteres
- Sin máximo (recomendado: 8-32)
- Puede contener cualquier carácter

### Flujo de Usuario

#### Registro (Primera vez)
```
1. Usuario ingresa nombre y contraseña
2. Sistema valida formato
3. Sistema verifica que el usuario no exista
4. Se genera salt único
5. Se hashea la contraseña con salt
6. Se crea registro en base de datos
7. Login automático
8. Redirección al juego
```

#### Login (Usuario existente)
```
1. Usuario ingresa credenciales
2. Sistema busca usuario en BD
3. Verifica bloqueo (UserLockService)
4. Hashea contraseña ingresada con salt del usuario
5. Compara hash con hash almacenado
6. Si coincide: Login exitoso
7. Si falla: Incrementa intentos fallidos
8. Tras 5 fallos: Bloquea cuenta
```

#### Cambio de Contraseña
```
1. Usuario ingresa contraseña actual
2. Sistema verifica contraseña actual
3. Usuario ingresa nueva contraseña
4. Sistema valida que no esté en historial
5. Se hashea nueva contraseña
6. Se actualiza en BD
7. Se agrega a historial de contraseñas
```

### Persistencia de Datos

**Ubicación**: `user://user_data.json`

**Estructura JSON**:
```json
{
  "users": [
    {
      "username": "player1",
      "passwordHash": "5e884898da28047151d0e56f8dc6292773603d0d6aabbdd62a11ef721d1542d8",
      "passwordSalt": "a3f7b2c9...",
      "highScore": 5420,
      "gamesPlayed": 15,
      "totalScore": 45320,
      "createdAt": "2025-12-08T10:30:00",
      "lastLoginAt": "2025-12-08T15:45:00",
      "isLocked": false,
      "failedLoginAttempts": 0,
      "passwordHistory": ["hash1", "hash2", "hash3"]
    }
  ]
}
```

---

## 🎯 Principios de Código Limpio Aplicados

### 1. **DRY** (Don't Repeat Yourself)
- ✅ Componentes reutilizables (`Health`, `Movement`)
- ✅ Clase base `BaseEntity` para lógica común
- ✅ Interfaces para contratos reutilizables
- ✅ Estrategias de movimiento compartidas

### 2. **KISS** (Keep It Simple, Stupid)
- ✅ Clases con responsabilidad única
- ✅ Métodos cortos y específicos
- ✅ Nombres descriptivos y claros
- ✅ Evita complejidad innecesaria

### 3. **YAGNI** (You Aren't Gonna Need It)
- ✅ Solo implementa funcionalidad necesaria
- ✅ Sin código especulativo
- ✅ Extensible mediante patrones (no código extra)

### 4. **Separation of Concerns**
- ✅ UI separada de lógica de negocio
- ✅ Servicios separados de managers
- ✅ Entities separadas de components
- ✅ Patrones en módulos independientes

### 5. **Composition over Inheritance**
- ✅ `Health` y `Movement` son componentes, no herencia
- ✅ Estrategias de movimiento intercambiables
- ✅ Comandos componibles

---

## 🚀 Cómo Extender el Juego

### Agregar un Nuevo Tipo de Enemigo

1. **Crear la clase del enemigo** (hereda de `Enemy`):
```csharp
public partial class DiverEnemy : Enemy
{
    public override void Initialize()
    {
        base.Initialize();
        Speed = 150.0f;
        Value = 300;
        Damage = 20;
        _healthComponent?.SetMaxHealth(40);
    }
    
    protected override void ConfigureMovementStrategy()
    {
        // Crear nueva estrategia si es necesario
        _movementComponent?.SetMovementStrategy(new DiveMovementStrategy());
    }
}
```

2. **Crear la factory**:
```csharp
public class DiverEnemyFactory : IEnemyFactory
{
    private PackedScene _enemyScene;
    
    public DiverEnemyFactory(PackedScene scene)
    {
        _enemyScene = scene;
    }
    
    public Enemy CreateEnemy(Vector2 position)
    {
        var enemy = _enemyScene.Instantiate<DiverEnemy>();
        enemy.GlobalPosition = position;
        enemy.Initialize();
        return enemy;
    }
    
    public string GetEnemyType() => "diver";
    public int GetDifficultyLevel() => 4;
}
```

3. **Registrar en EnemyFactory.Initialize()**:
```csharp
else if (scenePath.Contains("diver"))
{
    _factoryManager.RegisterFactory(new DiverEnemyFactory(scene));
}
```

4. **Crear la escena** `.tscn` con los nodos requeridos:
   - Nodo raíz: `Area2D` (DiverEnemy script)
   - Hijo: `Health` (componente de vida)
   - Hijo: `Movement` (componente de movimiento)
   - Hijo: `Sprite2D` (visual)
   - Hijo: `CollisionShape2D` (hitbox)

### Agregar una Nueva Estrategia de Movimiento

```csharp
public class ZigZagMovementStrategy : IMovementStrategy
{
    private float _zigzagSpeed = 2.0f;
    private float _timeElapsed = 0.0f;
    private Vector2 _direction = Vector2.Down;
    
    public void Initialize(Node2D entity)
    {
        // Configuración inicial si es necesario
    }
    
    public void Move(Node2D entity, double delta, float speed, Vector2 direction)
    {
        _timeElapsed += (float)delta;
        _direction = direction;
        
        // Zig-zag logic
        float horizontalOffset = Mathf.Sin(_timeElapsed * _zigzagSpeed) > 0 ? 1 : -1;
        Vector2 movement = new Vector2(horizontalOffset * 0.5f, 1.0f).Normalized();
        
        entity.GlobalPosition += movement * speed * (float)delta;
    }
    
    public Vector2 GetCurrentDirection() => _direction;
    public void SetDirection(Vector2 direction) => _direction = direction;
}
```

### Agregar un Nuevo Comando

```csharp
public class ShootCommand : ICommand
{
    private Player _player;
    
    public ShootCommand(Player player)
    {
        _player = player;
    }
    
    public void Execute()
    {
        _player.Shoot();
    }
    
    public void Undo()
    {
        // No aplica para disparo
    }
}
```

---

## 📝 Buenas Prácticas Implementadas

### Nomenclatura

- **Clases**: PascalCase (`GameManager`, `AudioService`)
- **Métodos**: PascalCase (`Initialize()`, `SpawnEnemy()`)
- **Variables privadas**: camelCase con prefijo `_` (`_player`, `_enemyContainer`)
- **Propiedades**: PascalCase (`CurrentLevel`, `HighScore`)
- **Constantes**: PascalCase (`SCORE_INCREMENT_PER_LEVEL`)
- **Interfaces**: PascalCase con prefijo `I` (`IMovementStrategy`, `IDamageable`)

### Documentación

- ✅ Comentarios XML en clases públicas
- ✅ Documentación de principios SOLID aplicados
- ✅ Comentarios explicativos en lógica compleja
- ✅ README completo con arquitectura

### Testing

- ✅ `UserTestScript.cs` para pruebas de usuarios
- ✅ Validaciones en servicios
- ✅ Dependency Injection facilita mocks

### Performance

- ✅ Uso de signals de Godot (nativo, optimizado)
- ✅ Object pooling implícito en factories
- ✅ Componentes reutilizables
- ✅ Límites de spawn para evitar sobrecarga

---

## 🛠️ Especificaciones Técnicas

## Sistema de Usuarios

### Características de Seguridad

- **Encriptación**: Contraseñas encriptadas con SHA-256 + salt
- **Historial de Contraseñas**: No permite reutilizar las últimas 5 contraseñas
- **Validaciones**:
  - Nombre de usuario: 3-20 caracteres alfanuméricos
  - Contraseña: Mínimo 4 caracteres
- **Persistencia**: Datos guardados en `user://user_data.json`

### Funcionalidades

1. **Registro de Usuarios**: Creación automática en el primer uso
2. **Login/Logout**: Sistema de autenticación completo
3. **Cambio de Contraseña**: Con validación de contraseña anterior
4. **High Scores Personalizados**: Cada usuario mantiene su propio récord
5. **Última Sesión**: Recuerda el último usuario conectado

### Flujo de Usuario

```
Inicio → Pantalla de Login → Autenticación → Juego Activo → Game Over → Repetir
    ↑                                                              ↓
    └────────────────────── Logout (Opcional) ────────────────────┘
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
- **F1**: Mostrar/ocultar pantalla de login durante el juego
- **Disparo**: Automático (no requiere input)

## Instalación y Uso

### Requisitos Previos

- Godot 4.5 o superior
- .NET SDK instalado
- Soporte para C# en Godot

### Ejecución

1. Abrir el proyecto en Godot
2. La escena principal es `res://scenes/game.tscn`
3. Al ejecutar, aparecerá la pantalla de login
4. Crear una cuenta o iniciar sesión
5. Jugar y acumular puntos

### Primera Ejecución

Al ejecutar por primera vez:
1. Ingresa un nombre de usuario (3-20 caracteres alfanuméricos)
2. Ingresa una contraseña (mínimo 4 caracteres)
3. El sistema creará automáticamente tu cuenta
4. Tu high score comenzará en 0

---

## 📜 Licencia

Este proyecto está bajo la licencia MIT. Ver archivo `LICENSE` para más detalles.

---

## 🙏 Créditos y Recursos

### Tutorial Base
- **How to make a Space Shooter Game in Godot**
- YouTube: https://www.youtube.com/watch?v=QoNukqpolS8

### Assets Visuales
- **Kenney.nl** - Space Shooter Redux Pack
- Licencia: CC0 (Public Domain)
- URL: https://kenney.nl/assets/space-shooter-redux

### Assets de Audio
- **Kenney.nl** - UI Audio Pack y Digital Audio Pack
- Licencia: CC0 (Public Domain)

### Documentación Godot
- **Godot Documentation**: https://docs.godotengine.org
- **C# Differences**: https://docs.godotengine.org/en/stable/getting_started/scripting/c_sharp/c_sharp_differences.html

### Patrones de Diseño
- **Refactoring Guru**: https://refactoring.guru/design-patterns
- **Game Programming Patterns** by Robert Nystrom

---

## 📞 Contacto

**Desarrollador**: Steeven Toala  
**GitHub**: [@SteevenToala](https://github.com/SteevenToala)  
**Proyecto**: [Game_Godot](https://github.com/SteevenToala/Game_Godot)  
**Branch**: `feature/login-with-users`

---

## 📝 Notas Adicionales

### Archivos de Documentación

El proyecto incluye documentación adicional:
- `ERRORES_CORREGIDOS.md` - Historial de bugs corregidos
- `USER_SYSTEM_README.md` - Documentación del sistema de usuarios
- `GUIA_USUARIO_BLOQUEO.md` - Guía del sistema de bloqueo
- `REFACTORIZACION_AUDIOSERVICE_DI.md` - Refactorización del AudioService

### Archivos Obsoletos Eliminables

Los siguientes archivos pueden ser eliminados:
- `ResearchVertical.csproj.old*` (backups del proyecto)
- `AuthService.cs.backup` (backup del servicio)
- `scripts/services/ServiceLocator.cs.uid` (sin archivo .cs)
- `scripts/services/SessionService.cs.uid` (sin archivo .cs)
- `scripts/services/UserScoreService.cs.uid` (sin archivo .cs)
- `scripts/services/UserService.cs.uid` (sin archivo .cs)

### Configuración Técnica

**project.godot**:
- Assembly: ResearchVertical
- Escena principal: `res://scenes/game.tscn`
- Resolución: 540x960 (vertical/portrait)
- FPS: Limitado a 50

**ResearchVertical.csproj**:
- Target Framework: net8.0
- Godot Packages: Godot.SourceGenerators

### Estado del Proyecto

✅ **Producción**: Completamente funcional  
✅ **Testing**: Todos los sistemas probados  
✅ **Documentación**: Completa y actualizada  
🚀 **Mejoras**: Abierto a contribuciones  

**Características Implementadas**:
- ✅ Sistema de usuarios con autenticación segura
- ✅ Sistema de niveles con dificultad progresiva
- ✅ 3 tipos de enemigos con comportamientos únicos
- ✅ Sistema de puntuación con high scores personalizados
- ✅ Leaderboard global (top 3)
- ✅ Arquitectura limpia con SOLID y patrones de diseño
- ✅ Persistencia de datos en JSON
- ✅ Sistema de audio completo
- ✅ UI pulida con pantallas de login y game over

---

## 🎮 ¡Disfruta el Juego!

Gracias por revisar este proyecto. Si tienes preguntas, sugerencias o encuentras bugs, no dudes en abrir un **Issue** en GitHub o contribuir con un **Pull Request**.

**¡Buena suerte destruyendo enemigos espaciales!** 🚀💥

---

*Última actualización: 8 de diciembre de 2025*  
*Versión: 2.0 - Sistema completo de usuarios y arquitectura SOLID*
