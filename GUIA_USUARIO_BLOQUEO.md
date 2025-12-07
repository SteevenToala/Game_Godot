# Guía de Usuario - Sistema de Bloqueo de Usuarios

## 🎮 Para Jugadores

### Función 1: Contraseña Visible

**¿Qué es?**
Un pequeño botón 👁️ junto al campo de contraseña que te permite ver lo que escribes.

**¿Cómo usarlo?**
1. En la pantalla de login, ve el campo "Contraseña:"
2. Junto al campo encontrarás un botón con un ojo 👁️
3. **Haz clic una vez** → La contraseña se muestra (podrás leer los caracteres)
4. **Haz clic de nuevo** → La contraseña se oculta nuevamente (aparecen puntos •••)

**Ejemplo:**
```
❌ Contraseña oculta:  ••••••••  👁️
✅ Contraseña visible: miPass123  👁️‍🗨️
```

### Función 2: Bloqueo tras Intentos Fallidos

**¿Qué es?**
Tu cuenta se bloquea automáticamente después de 3 intentos fallidos de login para protegerte.

**¿Cómo funciona?**

**Intento 1 - Fallo:**
- Escribes usuario: ❌
- O contraseña: ❌
- Mensaje: "Contraseña incorrecta. (2 intentos restantes) ⚠️"

**Intento 2 - Fallo:**
- Vuelves a intentar
- Mensaje: "Contraseña incorrecta. (1 intento restante) ⚠️"

**Intento 3 - Fallo:**
- Último intento
- Mensaje: "❌ Usuario bloqueado. Contacta al administrador para desbloquearlo."
- **Tu cuenta está bloqueada** 🔒

**Intento 4 en adelante:**
- Aunque escribas bien, la cuenta sigue bloqueada
- Debe desbloquearte un administrador

**¿Qué protege?**
- Protege contra fuerza bruta (alguien intentando adivinar tu contraseña)
- Proteges tu cuenta de acceso no autorizado

---

## 👨‍💼 Para Administradores

### Cómo Desbloquear Usuarios

**Requisito:** Debes estar loguado en tu cuenta de administrador.

**Pasos:**

1. **Inicia sesión** como administrador

2. **Busca el botón** "GESTIONAR USUARIOS" en la pantalla
   - Solo aparece cuando estás loguado
   - Está en el panel inferior de la pantalla

3. **Haz clic** en "GESTIONAR USUARIOS"
   - Se abrirá un panel flotante
   - Mostrará lista de usuarios bloqueados

4. **Selecciona el usuario** que quieres desbloquear
   - Verás algo como: "🔒 usuario_bloqueado"
   - Haz clic para seleccionarlo (se marcará en azul)

5. **Presiona "DESBLOQUEAR SELECCIONADO"**
   - Verás un mensaje verde: "✅ Usuario 'nombre' desbloqueado"

6. **Listo** ✅
   - El usuario ahora puede intentar login de nuevo
   - El contador de intentos se resetea a 0
   - Puede entrar sin restricciones

7. **Cierra el panel** con el botón "CERRAR"

**Ejemplo de flujo:**
```
Pantalla de login (logueado como admin)
↓
[GESTIONAR USUARIOS] ← Presiona este botón
↓
Panel se abre mostrando:
  🔒 jugador1
  🔒 jugador2
↓
Selecciona "🔒 jugador1" (haz clic)
↓
Presiona [DESBLOQUEAR SELECCIONADO]
↓
Mensaje: "✅ Usuario 'jugador1' desbloqueado"
↓
Presiona [CERRAR]
```

---

## 📊 Estados de Usuario

### Usuario Desbloqueado (Normal)
```
✅ Puede intentar login ilimitadamente
✅ Sin advertencias
✅ Intentos fallidos: 0/3
```

### Usuario con 1 Intento Fallido
```
⚠️ Puede intentar 2 veces más
⚠️ Advertencia en amarillo: "2 intentos restantes"
📊 Intentos fallidos: 1/3
```

### Usuario con 2 Intentos Fallidos
```
⚠️ Puede intentar 1 vez más
⚠️ Advertencia en amarillo: "1 intento restante"
📊 Intentos fallidos: 2/3
```

### Usuario Bloqueado
```
❌ NO puede intentar login
❌ Mensaje en rojo: "Usuario bloqueado"
❌ Requiere desbloqueo de administrador
🔒 Intentos fallidos: 3/3 (BLOQUEADO)
```

---

## 🔄 Ejemplo Completo de Uso

### Escenario: Nuevo Usuario

```
1. CREAR CUENTA
   Usuario: "carlos"
   Contraseña: "miPas123"
   ✅ Cuenta creada

2. LOGOUT
   ✅ Cierra sesión

3. INTENTO 1 - LOGIN INCORRECTO
   Usuario: "carlos"
   Contraseña: "malaPas123" ❌
   Resultado: "Contraseña incorrecta. (2 intentos restantes) ⚠️"

4. INTENTO 2 - LOGIN INCORRECTO
   Usuario: "carlos"
   Contraseña: "otraPas123" ❌
   Resultado: "Contraseña incorrecta. (1 intento restante) ⚠️"

5. INTENTO 3 - LOGIN INCORRECTO
   Usuario: "carlos"
   Contraseña: "anotherPas" ❌
   Resultado: "❌ Usuario bloqueado. Contacta al administrador"

6. LLAMAR ADMINISTRADOR
   Admin logueado → GESTIONAR USUARIOS
   → Selecciona "🔒 carlos"
   → DESBLOQUEAR
   → "✅ Usuario 'carlos' desbloqueado"

7. INTENTO 4 - LOGIN CORRECTO (después de desbloqueo)
   Usuario: "carlos"
   Contraseña: "miPas123" ✅
   Resultado: "✅ Login exitoso"
   ✅ Cuenta abierta
   ✅ Intentos: 0 (reseteado)
```

---

## 🆘 Preguntas Frecuentes

### P: ¿Puedo desbloquearme a mí mismo?
**R:** No. Solo un administrador con sesión activa puede desbloquear usuarios. Es por seguridad.

### P: ¿Cuántos intentos tengo?
**R:** Tienes 3 intentos. La pantalla te mostrará cuántos quedan (ej: "2 intentos restantes").

### P: ¿Mi contraseña se ve cuando presiono el botón 👁️?
**R:** Sí. El botón te permite ver la contraseña mientras la escribes. Útil si hiciste un error tipográfico.

### P: ¿La contraseña se guarda en algún lado si la muestro?
**R:** No. Solo te permite verla temporalmente en la pantalla. Es completamente seguro.

### P: ¿Cuánto tiempo dura el bloqueo?
**R:** Permanente hasta que un administrador te desbloquee. No hay desboqueo automático.

### P: ¿Qué datos guarda el sistema?
**R:** 
- Número de intentos fallidos
- Si estás bloqueado (Sí/No)
- Fecha de bloqueo

No se guardan contraseñas en el archivo de bloqueos (están en otro lado, hasheadas).

### P: ¿Puedo desbloquear a otros usuarios?
**R:** Solo si eres administrador y estás loguado. Los usuarios normales no ven el botón.

### P: ¿Qué pasa si se cae el juego cuando intento login?
**R:** Los intentos se guardan automáticamente. Tu contador de intentos se mantiene.

---

## ⚙️ Configuración (Para Desarrolladores)

### Cambiar número máximo de intentos

En `scripts/services/UserLockService.cs`:
```csharp
private const int MAX_FAILED_ATTEMPTS = 3; // Cambiar a otro número
```

### Cambiar mensaje de bloqueo

En `scripts/services/AuthService.cs`:
```csharp
return new AuthResult(false, "❌ Usuario bloqueado. Contacta al administrador para desbloquearlo.");
// Cambiar el texto según necesites
```

### Agregar desbloqueo automático

Agregar en `UserLockService.cs`:
```csharp
public void UnlockIfExpired(string username)
{
    // Implementar lógica de tiempo
}
```

---

## 📱 Interfaz Visual

### Pantalla de Login
```
┌─────────────────────────────────────┐
│    AUTENTICACION                    │
│                                     │
│  Usuario: [_______________]         │
│                                     │
│  Contraseña: [___________] 👁️      │
│              ⚠️ 2 intentos restantes│
│                                     │
│  [INICIAR SESIÓN] [CREAR CUENTA]   │
│                                     │
│  ❌ Contraseña incorrecta (x info) │
└─────────────────────────────────────┘
```

### Después de Loguear
```
┌─────────────────────────────────────┐
│    AUTENTICACION                    │
│                                     │
│  👤 Usuario: carlos                 │
│  🏆 Record: 5250                    │
│  📅 Último: 2025-12-04 10:30       │
│                                     │
│  [CAMBIAR CONTRASEÑA]              │
│  [GESTIONAR USUARIOS]  ← NUEVO     │
│  [CERRAR SESIÓN]                   │
│                                     │
└─────────────────────────────────────┘
```

### Panel de Gestión de Usuarios
```
┌──────────────────────────────────────┐
│  GESTIONAR USUARIOS BLOQUEADOS       │
│                                      │
│  Usuarios bloqueados:                │
│  ☐ 🔒 jugador1                      │
│  ☑ 🔒 jugador2    ← Seleccionado   │
│  ☐ 🔒 jugador3                      │
│  ☐ 🔒 jugador4                      │
│                                      │
│  ✅ Usuario 'jugador2' desbloqueado │
│                                      │
│  [DESBLOQUEAR SELECCIONADO] [CERRAR]│
└──────────────────────────────────────┘
```

---

## 🎯 Resumen Rápido

| Función | Ubicación | Cómo usar |
|---------|-----------|----------|
| **Ver Contraseña** | Botón 👁️ en login | Haz clic para mostrar/ocultar |
| **Ver Intentos** | Bajo campo contraseña | Lee el contador amarillo |
| **Desbloquear Usuario** | Botón "GESTIONAR" (solo logueado) | Selecciona → Presiona botón |
| **Bloqueo Automático** | Automático | 3 intentos fallidos = bloqueado |

---

**¿Necesitas más ayuda?** 
Consulta con un administrador del sistema.
