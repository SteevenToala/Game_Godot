using Godot;

/// <summary>
/// Script de prueba para el sistema de usuarios - Flujo de Login Primero
/// </summary>
public partial class UserTestScript : Node
{
	public override void _Ready()
	{
		GD.Print("=== PRUEBA DEL SISTEMA DE USUARIOS - FLUJO LOGIN PRIMERO ===");
		GD.Print("ℹ️  Este test verifica que el sistema funcione correctamente");
		GD.Print("ℹ️  Recomendación: Ejecutar este script en una escena separada");
		GD.Print("");

		TestPasswordService();
		TestAuthServiceFlow();
		TestLoginFlow();

		GD.Print("");
		GD.Print("✅ Todas las pruebas completadas");
		GD.Print("🎮 Para probar la interfaz completa, ejecuta la escena principal del juego");
		GD.Print("=== FIN DE PRUEBAS ===");
	}

	private void TestPasswordService()
	{
		GD.Print("\n--- Pruebas de PasswordService ---");

		// Test validación de contraseña
		GD.Print($"Contraseña '123' válida: {PasswordService.IsValidPassword("123")}");  // false
		GD.Print($"Contraseña '1234' válida: {PasswordService.IsValidPassword("1234")}");  // true

		// Test validación de usuario
		GD.Print($"Usuario 'ab' válido: {PasswordService.IsValidUsername("ab")}");  // false
		GD.Print($"Usuario 'abc' válido: {PasswordService.IsValidUsername("abc")}");  // true
		GD.Print($"Usuario 'player1' válido: {PasswordService.IsValidUsername("player1")}");  // true

		// Test hash de contraseña
		var hash1 = PasswordService.HashPassword("test123");
		var hash2 = PasswordService.HashPassword("test123");
		GD.Print($"Hashes iguales: {hash1 == hash2}");  // true

		// Test verificación
		GD.Print($"Verificación correcta: {PasswordService.VerifyPassword("test123", hash1)}");  // true
		GD.Print($"Verificación incorrecta: {PasswordService.VerifyPassword("wrong", hash1)}");  // false
	}

	private void TestAuthServiceFlow()
	{
		GD.Print("\n--- Pruebas de AuthService Flow ---");
		
		// Verificar estado inicial (no debe haber usuario logueado)
		GD.Print($"Estado inicial - Usuario logueado: {AuthService.IsLoggedIn}");  // false
		
		// Verificar usuarios disponibles en el sistema
		var allUsers = AuthService.GetAllUsers();
		GD.Print($"Total de usuarios en el sistema: {allUsers.Count}");
		foreach (var user in allUsers)
		{
			GD.Print($"  - {user.Username} (HighScore: {user.HighScore})");
		}
	}
	
	private void TestLoginFlow()
	{
		GD.Print("\n--- Pruebas de Flujo de Login ---");
		
		// Test login/creación de usuario
		var result = AuthService.Login("testuser", "password123");
		GD.Print($"Login/Creación resultado: {result.Success} - {result.Message}");

		if (result.Success)
		{
			GD.Print($"✅ Usuario logueado: {AuthService.CurrentUser.Username}");
			GD.Print($"📊 High score inicial: {AuthService.GetHighScore()}");

			// Test actualización de score
			var scoreUpdated = AuthService.UpdateScore(1500);
			GD.Print($"🏆 Score actualizado (1500): {scoreUpdated}");
			GD.Print($"📈 High score actual: {AuthService.GetHighScore()}");

			// Test cambio de contraseña
			var changeResult = AuthService.ChangePassword("password123", "newpassword456");
			GD.Print($"🔐 Cambio de contraseña: {changeResult.Success} - {changeResult.Message}");

			// Test mismo cambio de contraseña (debería fallar)
			var changeResult2 = AuthService.ChangePassword("newpassword456", "password123");
			GD.Print($"🚫 Reuso de contraseña: {changeResult2.Success} - {changeResult2.Message}");

			// Logout
			AuthService.Logout();
			GD.Print($"👋 Después de logout, logueado: {AuthService.IsLoggedIn}");
			
			// Verificar que los datos siguen guardados en la base de datos
			var allUsersAfter = AuthService.GetAllUsers();
			GD.Print($"💾 Datos persistidos - Total usuarios: {allUsersAfter.Count}");
		}
	}
}
