namespace projectapi.Models
{
    public class User : Common
    {
      
        public string? UserFirstName { get; set; }
        public string? UserLastName { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPassword { get; set; }
        public string? UserPasswordHash { get; set; }
        public string? UserPasswordSalt { get; set; }
        public string? UserAdress { get; set; }
        public string? UserCity { get; set; }
        public string? UserCountry { get; set; }
        public string? UserPhoneNumber { get; set; }
        public string? UserPostalCode { get; set; }


    }
    public class UserSignUpDTO
    {
        public string? Id { get; set; }
        public string? UserFirstName { get; set; }
        public string? UserLastName { get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPassword { get; set; }


    }
    public class UserLoginDTO 
    { 
        public string? UserEmail { get; set; }
        public string? UserPassword { get; set; }

    }
}
