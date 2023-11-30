namespace trab_GB;
public class User
{
    private static int countId = 0;
    public User(string login, string pass = "")
    {
        this.id = countId++;
        this.login = login;
        this.password = pass;
    }
    
    public int id { get; set; }
    public string login { get; set; }
    public string password { get; set;}
}