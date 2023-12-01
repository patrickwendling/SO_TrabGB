namespace trab_GB;
public class User
{
    public User(string id, string login, string pass = "")
    {
        this.id = id;
        this.login = login;
        this.password = pass;
    }
    
    public string id { get; set; }
    public string login { get; set; }
    public string password { get; set;}

    public override string ToString()
    {
        return $"Id: {this.id } - Login: {this.login}";
    }
}