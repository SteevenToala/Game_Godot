using Godot;

/// <summary>
/// Interfaz para el servicio de audio (abstracción para DIP y testabilidad)
/// Principio SOLID: ISP - Interface Segregation Principle (interfaz específica)
/// Principio SOLID: DIP - Dependency Inversion Principle (abstracción)
/// Permite mock testing y desacoplamiento
/// </summary>
public interface IAudioService
{
	// Propiedades de configuración
	AudioStream BackgroundMusic { get; set; }
	float MusicVolume { get; set; }
	float SFXVolume { get; set; }
	bool AutoplayMusic { get; set; }
	
	// Efectos de sonido
	void PlayLaserShot();
	void PlayHit();
	void PlayExplosion();
	
	// Música de fondo
	void PlayBackgroundMusic();
	void PlayBackgroundMusic(AudioStream musicStream);
	void StopBackgroundMusic();
	void PauseBackgroundMusic();
	void ResumeBackgroundMusic();
	
	// Control de volumen
	void SetMusicVolume(float volume);
	void SetSFXVolume(float volume);
	
	// Estado
	bool IsMusicPlaying();
}
