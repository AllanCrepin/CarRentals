namespace CarRentals.Data.Service
{
    public class PasswordService
    {
        // Leaves room for future improvement, like hashing etc.
        public bool VerifyPassword(string dbPassword, string formPassword)
        {
            if (string.IsNullOrEmpty(dbPassword) || string.IsNullOrEmpty(formPassword)) { return false; }

            if (dbPassword == formPassword)
            {
                return true;
            }

            return false;
        }
    }
}
