using MySql.Data.MySqlClient;

public class DB
{
    public static MySqlConnection GetConnection()
    {
        string connString = "Server=localhost;Database=bellefleur;Uid=root;Pwd=root;";

        MySqlConnection conn = new MySqlConnection(connString);

        return conn;
    }

    public static MySqlCommand GetCommand() 
    {
        MySqlCommand cmd = GetConnection().CreateCommand();

        return cmd;
    }
}