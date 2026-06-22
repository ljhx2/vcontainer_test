using VContainer;
using VContainer.Unity;

public class AppLifetimeScope : LifetimeScope
{
    protected override void Configure(IContainerBuilder builder)
    {
        builder.Register<ResourceManager>(Lifetime.Singleton);
        builder.RegisterEntryPoint<SoundManager>(Lifetime.Singleton).AsSelf();
        builder.RegisterEntryPoint<InputManager>(Lifetime.Singleton).AsSelf();

        var uiManager = GetComponentInChildren<UIManager>();
        if (uiManager != null) builder.RegisterComponent(uiManager);
    }
}
