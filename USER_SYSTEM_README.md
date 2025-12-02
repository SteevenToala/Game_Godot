# Sistema de Control de Usuarios - Vertical Shooter Demo

## Descripción

Se ha implementado un sistema completo de control de usuarios que permite manejar una sola cuenta con nombre, contraseña y puntaje. El sistema incluye validaciones de seguridad para evitar la reutilización de contraseñas.

## Características Principales

### 🔐 Autenticación
- **Una sola cuenta**: El sistema maneja una única cuenta de usuario
- **Validación de credenciales**: Nombre de usuario (3-20 caracteres alfanuméricos) y contraseña (mínimo 4 caracteres)
- **Encriptación**: Las contraseñas se almacenan encriptadas usando SHA-256 con salt
- **Historial de contraseñas**: No se permite reutilizar las últimas 5 contraseñas

### 📊 Gestión de Puntajes
- **High Score personalizado**: Cada usuario tiene su propio record
- **Integración con el juego**: El sistema se integra completamente con el ScoreManager existente
- **Persistencia**: Los datos se guardan automáticamente en formato JSON

### 🎮 Interfaz de Usuario
- **Pantalla de Login**: Interfaz completa para login y creación de cuenta
- **Cambio de contraseña**: Panel dedicado para cambiar contraseña con validaciones
- **Información en HUD**: Muestra el usuario actual durante el juego
- **Información en Game Over**: Muestra datos del usuario al finalizar

## Archivos Implementados

### Modelos
- `scripts/models/User.cs` - Modelo de usuario con todas sus propiedades

### Servicios
- `scripts/services/PasswordService.cs` - Encriptación y validación de contraseñas
- `scripts/services/AuthService.cs` - Gestión de autenticación y persistencia

### Managers
- `scripts/managers/UserManager.cs` - Manager principal del sistema de usuarios

### UI
- `scripts/ui/LoginScreen.cs` - Interfaz completa de login y gestión de usuario

### Utilidades
- `scripts/utils/UserTestScript.cs` - Script de pruebas para validar el sistema

## Archivos Modificados

### Integración con el Juego
- `scripts/managers/GameManager.cs` - Integrado con el sistema de usuarios
- `scripts/managers/ScoreManager.cs` - Adaptado para usar puntajes por usuario
- `scripts/ui/Hud.cs` - Agregado display de usuario actual
- `scripts/ui/GameOverScreen.cs` - Agregado información de usuario

## Uso del Sistema

### 1. Primer Uso
Al ejecutar el juego por primera vez:
1. Se mostrará la pantalla de login
2. Ingresa un nombre de usuario (3-20 caracteres)
3. Ingresa una contraseña (mínimo 4 caracteres)
4. El sistema creará automáticamente la cuenta

### 2. Logins Subsiguientes
- Ingresa tus credenciales en la pantalla de login
- El sistema recordará tu puntaje máximo

### 3. Cambiar Contraseña
1. Durante el juego, presiona F1 para mostrar la pantalla de login
2. Haz clic en "CAMBIAR CONTRASEÑA"
3. Ingresa tu contraseña actual
4. Ingresa la nueva contraseña (debe ser diferente a las últimas 5 usadas)
5. Confirma la nueva contraseña

### 4. Controles Especiales
- **F1**: Mostrar/ocultar pantalla de login durante el juego
- **Reset (R)**: Reinicia el juego manteniendo la sesión
- **Quit (Esc)**: Salir del juego

## Validaciones de Seguridad

### Nombre de Usuario
- Entre 3 y 20 caracteres
- Solo letras, números y guiones bajos
- No se distingue entre mayúsculas y minúsculas

### Contraseña
- Mínimo 4 caracteres
- No puede ser igual a ninguna de las últimas 5 contraseñas utilizadas
- Se almacena encriptada con SHA-256 y salt

### Persistencia
- Los datos se guardan en `user://user_data.json`
- Compatible con el sistema de guardado existente del juego
- Backup automático del high score en el sistema anterior

## Arquitectura del Sistema

```
UserManager (Node principal)
├── AuthService (Static - Lógica de autenticación)
├── PasswordService (Static - Encriptación)
├── LoginScreen (UI de login)
└── User (Modelo de datos)

Integración con GameManager:
GameManager
├── UserManager (Gestión de usuarios)
├── ScoreManager (Adaptado para usuarios)
├── HUD (Muestra usuario actual)
└── GameOverScreen (Muestra info de usuario)
```

## Flujo de Datos

1. **Inicio**: UserManager inicializa AuthService
2. **Login**: LoginScreen → AuthService → UserManager → GameManager
3. **Juego**: ScoreManager → UserManager → AuthService (para puntajes)
4. **Persistencia**: AuthService guarda datos automáticamente

## Casos de Uso Cubiertos

- ✅ Usuario nuevo (creación automática)
- ✅ Usuario existente (login)
- ✅ Cambio de contraseña con validaciones
- ✅ Prevención de reutilización de contraseñas
- ✅ Persistencia de puntajes por usuario
- ✅ Integración completa con el juego existente
- ✅ Interfaz intuitiva y responsive

## Instalación

1. Copia todos los archivos a sus respectivas carpetas
2. El sistema se inicializa automáticamente al ejecutar el juego
3. No se requiere configuración adicional

## Testing

Para probar el sistema:
1. Agrega el `UserTestScript.cs` a una escena
2. Ejecuta la escena para ver las pruebas automatizadas en la consola
3. Verifica que todas las validaciones funcionan correctamente

## Notas Técnicas

- Compatible con Godot 4.x
- Utiliza el sistema de señales nativo de Godot
- Implementa patrones de diseño: Singleton, Observer
- Código totalmente documentado y segurito
- Manejo robusto de errores
