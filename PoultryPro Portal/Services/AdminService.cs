using FirebaseAdmin.Auth;
using PoultryPro_Portal.Models;

namespace PoultryPro_Portal.Services
{
    public interface IAdminService
    {
        Task<bool> IsAdminRegisteredAsync();
        Task<RegisterResultModel> RegisterAdminAsync(string email, string password);
        Task<LoginResultModel> LoginAdminAsync(string email, string password);
        Task<bool> SendPasswordResetEmailAsync(string email);
    }

    

    public class AdminService : IAdminService
    {
        private readonly FirebaseAuth _auth;
        private readonly string ApiKey = Environment.GetEnvironmentVariable("API_KEY");
        private bool _isAdminExists = false;
        private const string AdminEmail = "muhammadahmadwork1034@gmail.com";
        private readonly Dictionary<string, int> failedAttempts = new Dictionary<string, int>();
        public AdminService()
        {
            _auth = FirebaseAuth.DefaultInstance;
        }

        // Check if admin already exists in Firebase
        public async Task<bool> IsAdminRegisteredAsync()
        {
            if (_isAdminExists) return true;

            var users = _auth.ListUsersAsync(null);
            await foreach (var user in users)
            {
                if (user.Email == AdminEmail)
                {
                    _isAdminExists = true;
                    return true;
                }
            }
            return false;
        }

        // Register a new admin in Firebase with password complexity validation
        public async Task<RegisterResultModel> RegisterAdminAsync(string email, string password)
        {
            // Check if the provided email matches the allowed admin email
            if (!email.Equals(AdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                return new RegisterResultModel { Success = false, ErrorMessage = "Unauthorized email address for admin registration." };
            }

            if (_isAdminExists)
            {
                return new RegisterResultModel { Success = false, ErrorMessage = "Admin already registered." };
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
                _isAdminExists = true;
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

        public async Task<LoginResultModel> LoginAdminAsync(string email, string password)
        {
            if (!email.Equals(AdminEmail, StringComparison.OrdinalIgnoreCase))
            {
                return new LoginResultModel { Success = false, ErrorMessage = "Unauthorized email address." };
            }

            if (failedAttempts.TryGetValue(email, out int attempts) && attempts >= 5)
            {
                return new LoginResultModel { Success = false, ErrorMessage = "Account locked. Too many failed login attempts." };
            }

            try
            {
                var url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={ApiKey}";
                var requestBody = new { email, password, returnSecureToken = true };

                using var httpClient = new HttpClient();
                var response = await httpClient.PostAsJsonAsync(url, requestBody);

                if (response.IsSuccessStatusCode)
                {
                    failedAttempts[email] = 0; // Reset on successful login
                    return new LoginResultModel { Success = true };
                }
                else
                {
                    failedAttempts[email] = attempts + 1;
                    return new LoginResultModel { Success = false, ErrorMessage = "Invalid credentials." };
                }
            }
            catch
            {
                return new LoginResultModel { Success = false, ErrorMessage = "An error occurred during login." };
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
