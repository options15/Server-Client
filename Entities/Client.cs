using System.ComponentModel.DataAnnotations;

namespace Entities
{
    public class Client
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public Client() { }
        public Client(string Login, string Password)
        {
            Id = 1;
            Name = "";
            this.Login = Login;
            this.Password = Password;
        }
    }
}
