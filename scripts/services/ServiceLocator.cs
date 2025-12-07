using System;
using System.Collections.Generic;

/// <summary>
/// Service Locator para gestionar dependencias en el proyecto
/// Principio SOLID: DIP (Dependency Inversion Principle)
/// Patrón de diseño: Service Locator
/// </summary>
public static class ServiceLocator
{
	private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();
	private static bool _initialized = false;

	/// <summary>
	/// Inicializa todos los servicios del sistema
	/// </summary>
	public static void Initialize()
	{
		if (_initialized)
		{
			System.Diagnostics.Debug.WriteLine("⚠️ ServiceLocator ya fue inicializado");
			return;
		}

		System.Diagnostics.Debug.WriteLine("🔧 Inicializando ServiceLocator...");

		// Registrar servicios en orden de dependencias
		RegisterCoreServices();
		RegisterBusinessServices();

		_initialized = true;
		System.Diagnostics.Debug.WriteLine("✅ ServiceLocator inicializado correctamente");
	}

	/// <summary>
	/// Registra servicios fundamentales (sin dependencias)
	/// </summary>
	private static void RegisterCoreServices()
	{
		// Base de datos de usuarios
		var userDatabase = new UserDatabaseService();
		Register<IUserDatabaseService>(userDatabase);

		// Servicio de bloqueo de usuarios
		var lockService = new UserLockService();
		Register<IUserLockService>(lockService);

		System.Diagnostics.Debug.WriteLine("  ✓ Servicios core registrados");
	}

	/// <summary>
	/// Registra servicios de negocio (con dependencias)
	/// </summary>
	private static void RegisterBusinessServices()
	{
		// Servicio de usuarios (depende de IUserDatabaseService e IUserLockService)
		var userService = new UserService(
			Get<IUserDatabaseService>(),
			Get<IUserLockService>()
		);
		Register<IUserService>(userService);

		// Servicio de puntajes (depende de IUserDatabaseService)
		var scoreService = new UserScoreService(
			Get<IUserDatabaseService>()
		);
		Register<IUserScoreService>(scoreService);

		System.Diagnostics.Debug.WriteLine("  ✓ Servicios de negocio registrados");
	}

	/// <summary>
	/// Registra un servicio en el localizador
	/// </summary>
	public static void Register<T>(T service) where T : class
	{
		var type = typeof(T);
		if (_services.ContainsKey(type))
		{
			System.Diagnostics.Debug.WriteLine($"⚠️ Servicio {type.Name} ya registrado, se sobrescribirá");
		}

		_services[type] = service;
	}

	/// <summary>
	/// Obtiene una instancia de un servicio registrado
	/// </summary>
	public static T Get<T>() where T : class
	{
		var type = typeof(T);
		
		if (!_services.ContainsKey(type))
		{
			System.Diagnostics.Debug.WriteLine($"❌ Servicio {type.Name} no registrado");
			return null;
		}

		return _services[type] as T;
	}

	/// <summary>
	/// Verifica si un servicio está registrado
	/// </summary>
	public static bool IsRegistered<T>() where T : class
	{
		return _services.ContainsKey(typeof(T));
	}

	/// <summary>
	/// Limpia todos los servicios registrados
	/// </summary>
	public static void Clear()
	{
		_services.Clear();
		_initialized = false;
		System.Diagnostics.Debug.WriteLine("🧹 ServiceLocator limpiado");
	}

	/// <summary>
	/// Reinicializa el ServiceLocator
	/// </summary>
	public static void Reset()
	{
		Clear();
		Initialize();
	}
}
