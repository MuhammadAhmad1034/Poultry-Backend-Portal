using FirebaseAdmin.Auth;
using PoultryPro_Portal.Models;

namespace PoultryPro_Portal.Services
{
    public interface ICallCenterAgentService
    {
        Task<bool> IsAgentRegisteredAsync(string email);
        Task<RegisterResultModel> RegisterAgentAsync(string email, string password);
        Task<LoginResultModel> LoginAgentAsync(string email, string password);
        Task<bool> SendPasswordResetEmailAsync(string email);
    }

    public class CallCenterAgentService : ICallCenterAgentService
    {
        private readonly FirebaseAuth _auth;
        private readonly string ApiKey = Environment.GetEnvironmentVariable("API_KEY");

        public CallCenterAgentService()
        {
            _auth = FirebaseAuth.DefaultInstance;
        }

        public async Task<bool> IsAgentRegisteredAsync(string email)
        {
            var users = _auth.ListUsersAsync(null);
            await foreach (var user in users)
            {
                if (user.Email == email) return true;
            }
            return false;
        }

        public async Task<RegisterResultModel> RegisterAgentAsync(string email, string password)
        {
            // Check if email already registered as an agent
            if (await IsAgentRegisteredAsync(email))
            {
                return new RegisterResultModel { Success = false, ErrorMessage = "Agent already registered." };
            }

            var passwordValidationResult = ValidatePassword(password);
            if (!passwordValidationResult.Success)
            {
                return passwordValidationResult; // Return specific error message for password complexity
            }

            try
            {
                var userRecordArgs = new UserRecordArgs
                {
                    Email = email,
                    Password = password,
                    EmailVerified = true,
                    Disabled = false
                };
                await _auth.CreateUserAsync(userRecordArgs);
                return new RegisterResultModel { Success = true };
            }
            catch (Exception ex)
            {
                return new RegisterResultModel { Success = false, ErrorMessage = $"Registration failed: {ex.Message}" };
            }
        }

        private RegisterResultModel ValidatePassword(string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 8)
            {
                return new RegisterResultModel { Success = false, ErrorMessage = "Password must be at least 8 characters long." };
            }

            if (!password.Any(char.IsUpper))
            {
                return new RegisterResultModel { Success = false, ErrorMessage = "Password must contain at least one uppercase letter." };
            }

            if (!password.Any(char.IsLower))
            {
                return new RegisterResultModel { Success = false, ErrorMessage = "Password must contain at least one lowercase letter." };
            }

            if (!password.Any(char.IsDigit))
            {
                return new RegisterResultModel { Success = false, ErrorMessage = "Password must contain at least one digit." };
            }

            if (!password.Any(ch => !char.IsLetterOrDigit(ch)))
            {
                return new RegisterResultModel { Success = false, ErrorMessage = "Password must contain at least one special character." };
            }

            return new RegisterResultModel { Success = true };
        }

        public async Task<LoginResultModel> LoginAgentAsync(string email, string password)
        {
            try
            {
                var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={ApiKey}";
                var requestBody = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                using var httpClient = new HttpClient();
                var response = await httpClient.PostAsJsonAsync(url, requestBody);

                if (response.IsSuccessStatusCode)
                {
                    return new LoginResultModel { Success = true };
                }
                else
                {
                    var errorContent = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                    string errorMessage = "Login failed.";
                    if (errorContent != null && errorContent.TryGetValue("error", out var errorObj) && errorObj is Dictionary<string, object> errorDict)
                    {
                        if (errorDict.TryGetValue("message", out var messageObj))
                        {
                            errorMessage = messageObj?.ToString() ?? "Login failed.";
                        }
                    }
                    return new LoginResultModel { Success = false, ErrorMessage = errorMessage };
                }
            }
            catch
            {
                return new LoginResultModel { Success = false, ErrorMessage = "An error occurred while trying to log in." };
            }
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email)
        {
            try
            {
                var url = $"https://identitytoolkit.googleapis.com/v1/accounts:sendOobCode?key={ApiKey}";
                var requestBody = new
                {
                    requestType = "PASSWORD_RESET",
                    email = email
                };

                using var httpClient = new HttpClient();
                var response = await httpClient.PostAsJsonAsync(url, requestBody);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
