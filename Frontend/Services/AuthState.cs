namespace Frontend.Services
{
    public class AuthState
    {
        public event Action? OnChange;

        public void NotifyAuthenticationChanged() => OnChange?.Invoke();
    }
}
