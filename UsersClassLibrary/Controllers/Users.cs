using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsersClassLibrary.Models;

namespace UsersClassLibrary.Controllers
{
    public static class Users
    {
        public static string connectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Database"].ConnectionString;
            }
        }

        private static List<User> _users;
        public static List<User> GetAll()
        {
            /*if (_users == null)
            {
                if (!File.Exists(@".\Models\database.json")) return new List<User>();
                string json = File.ReadAllText(@".\Models\database.json");
                _users = JsonConvert.DeserializeObject<List<User>>(json);
            }
            return _users;*/
            List<User> retVal = new List<User>();
            //2 - Mi connetto al db e apro la connessione
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT * FROM Users ";
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            User u = new User
                            {
                                Id = (int)reader["Id"],
                                FirstName = (string)reader["FirstName"],
                                LastName = (string)reader["LastName"],
                                Age = (int)reader["Age"],
                                Gender = (string)reader["Gender"],
                                Email = (string)reader["Email"],
                                Username = (string)reader["Username"],
                                Password = (string)reader["Password"],
                                BirthDate = (DateTime)reader["BirthDate"],
                                Address = new FullAddress
                                {
                                    Address = (string)reader["Address"],
                                    City = (string)reader["City"],
                                    PostalCode = (string)reader["PostalCode"],
                                    State = (string)reader["State"],
                                }
                            };

                            retVal.Add(u);
                        }
                    }
                    return retVal;
                }
                catch (Exception)
                {
                    throw;
                }
            }

        }

        //public static List<User> FindAll(Predicate<User> condizione)
        public static List<User> FindAll(string nome, string sex)
        {
            if (string.IsNullOrWhiteSpace(nome)) nome = "";
            if (string.IsNullOrWhiteSpace(sex)) sex = "";
            //return GetAll().FindAll(condizione);

            //1 - Mi creo la lista vuota da restituire
            List<User> retVal = new List<User>();

            //2 - Mi connetto al db e apro la connessione
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();

                    //3 - Compongo la query sql (con i parametri)
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = "SELECT * FROM Users ";
                    cmd.CommandText += "WHERE (FirstName Like @name OR LastName Like @name) ";
                    cmd.CommandText += "AND Gender LIKE @sex";

                    cmd.Parameters.AddWithValue("@name", $"%{nome}%");
                    cmd.Parameters.AddWithValue("@sex", $"{sex}%");

                    //4 - Ottengo il data reader
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        //5 - per ogni riga della query creo un oggetto User e aggiungo alla lista

                        retVal.Add(ReadToUser(reader));
                    }
                }
                //6 - Restituisco la lista


                catch (Exception)
                {
                    throw;
                }
            }
            return retVal;
        }

    /*public static User Find(Predicate<User> condizione)
    {
        return GetAll().Find(condizione);
    }*/

    //metodo find
    public static User Find(int Id)
    {
        User u1 = new User();
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT top (1) * FROM Users ";
                cmd.CommandText += "WHERE (Id=@Id)";
                cmd.Parameters.AddWithValue("@Id", $"%{Id}%");
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        User u = new User
                        {
                            Id = (int)reader["Id"],
                            FirstName = (string)reader["FirstName"],
                            LastName = (string)reader["LastName"],
                            Age = (int)reader["Age"],
                            Gender = (string)reader["Gender"],
                            Email = (string)reader["Email"],
                            Username = (string)reader["Username"],
                            Password = (string)reader["Password"],
                            BirthDate = (DateTime)reader["BirthDate"],
                            Address = new FullAddress
                            {
                                Address = (string)reader["Address"],
                                City = (string)reader["City"],
                                PostalCode = (string)reader["PostalCode"],
                                State = (string)reader["State"]
                            }
                        };
                        u1 = u;
                    }
                }
                return u1;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
        private static SqlParameter[] UserToParameters(User u)
        {
            SqlParameter[] sp = new SqlParameter[]
            {
            new SqlParameter("@Id", u.Id),
            new SqlParameter("@FirstName", u.FirstName),
            new SqlParameter("@LastName", u.LastName),
            new SqlParameter("@Age", u.Age),
            new SqlParameter("@Gender", u.Gender),
            new SqlParameter("@Email", u.Email),
            new SqlParameter("@Username", u.Username),
            new SqlParameter("@Password", u.Password),
            new SqlParameter("@BirthDate", u.BirthDate),
            new SqlParameter("@Address", u.Address == null ? "" : u.Address.Address),
            new SqlParameter("@City", u.Address == null ? "" : u.Address.City),
            new SqlParameter("@ostalCode", u.Address == null ? "" : u.Address.PostalCode),
            new SqlParameter("@State", u.Address == null ? "" : u.Address.State)
            };
            return new sp;
        }
        //poi scrivo al posto di tanti add with value passo la funzione
        //cmd.Parameters.AddRange(UserToParameters(u));

    //metodo find2
    public static User Find(string mail)
    {
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            User u1 = new User();
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "SELECT top (1) * FROM Users ";
                cmd.CommandText += "WHERE (Email=@mail)";//Email è il nome del campo in sql, mail il nome della stringa che mi ritorna
                cmd.Parameters.AddWithValue("@mail", $"%{mail}%");
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())//è l'unico modo che ho per leggere le righe usare il reader
                    {
                        User u = new User
                        {
                            Id = (int)reader["Id"],
                            FirstName = (string)reader["FirstName"],
                            LastName = (string)reader["LastName"],
                            Age = (int)reader["Age"],
                            Gender = (string)reader["Gender"],
                            Email = (string)reader["Email"],
                            Username = (string)reader["Username"],
                            Password = (string)reader["Password"],
                            BirthDate = (DateTime)reader["BirthDate"],
                            Address = new FullAddress
                            {
                                Address = (string)reader["Address"],
                                City = (string)reader["City"],
                                PostalCode = (string)reader["PostalCode"],
                                State = (string)reader["State"]
                            }
                        };
                        u1 = u;
                    }
                }
                return u1;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public static void Add(User u) //in questa query eseguo il controllo se l'id è già esistente allora mi collego al db,
            //poi controllo in sql la riga con l'id uguale e modifico tutti i parametri
    {
        //GetAll().Add(u);
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = connection;
                cmd.CommandText = "insert into Users values(";
                cmd.CommandText += "@Id, @FirstName,@LastName, @Age, @Gender, @Email, @Username, ";
                cmd.CommandText += "@Password, @BirthDate, @Address, @City, @PostalCode, @State)";
                cmd.Parameters.AddWithValue("@Id", u.Id);
                cmd.Parameters.AddWithValue("@FirstName", u.FirstName);
                cmd.Parameters.AddWithValue("@LastName", u.LastName);
                cmd.Parameters.AddWithValue("@Age", u.Age);
                cmd.Parameters.AddWithValue("@Gender", u.Gender);
                cmd.Parameters.AddWithValue("@Email", u.Email);
                cmd.Parameters.AddWithValue("@Username", u.Username);
                cmd.Parameters.AddWithValue("@Password", u.Password);
                cmd.Parameters.AddWithValue("@BirthDate", u.BirthDate);
                cmd.Parameters.AddWithValue("@Address", u.Address == null ? "" : u.Address.Address);
                cmd.Parameters.AddWithValue("@City", u.Address == null ? "" : u.Address.City);
                cmd.Parameters.AddWithValue("@ostalCode", u.Address == null ? "" : u.Address.PostalCode);
                cmd.Parameters.AddWithValue("@State", u.Address == null ? "" : u.Address.State);
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }

    public static bool Update(int id, User u)
    {
            if (u.Id != id) return false;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Connection = connection;
                    cmd.CommandText = "update Users set";
                    cmd.CommandText += "Id=@Id, FirstName=@FirstName,LastName=@LastName, Age=@Age, Gender=@Gender, Email=@Email, Username=@Username, ";
                    cmd.CommandText += "Password=@Password, BirthDate=@BirthDate, Address=@Address, City=@City, PostalCode=@PostalCode, State=@State)";
                    cmd.CommandText += "where Id=@id";
                    cmd.Parameters.AddWithValue("@Id", u.Id);
                    cmd.Parameters.AddWithValue("@FirstName", u.FirstName);
                    cmd.Parameters.AddWithValue("@LastName", u.LastName);
                    cmd.Parameters.AddWithValue("@Age", u.Age);
                    cmd.Parameters.AddWithValue("@Gender", u.Gender);
                    cmd.Parameters.AddWithValue("@Email", u.Email);
                    cmd.Parameters.AddWithValue("@Username", u.Username);
                    cmd.Parameters.AddWithValue("@Password", u.Password);
                    cmd.Parameters.AddWithValue("@BirthDate", u.BirthDate);
                    cmd.Parameters.AddWithValue("@Address", u.Address == null ? "" : u.Address.Address);
                    cmd.Parameters.AddWithValue("@City", u.Address == null ? "" : u.Address.City);
                    cmd.Parameters.AddWithValue("@ostalCode", u.Address == null ? "" : u.Address.PostalCode);
                    cmd.Parameters.AddWithValue("@State", u.Address == null ? "" : u.Address.State);
                    return cmd.ExecuteNonQuery()==1;
                }
                catch (Exception)
                {
                    throw;
                }
               
            }





            return true;
    }
    public static List<string> GetGenders()
    {
        return GetAll().Select(s => s.Gender).Distinct().ToList();
    }

    public static string[] FormatAsTable(List<User> users)
    {
        string[] table = new string[users.Count + 4];

        int i = 0;

        table[i++] = new String('-', 170);
        table[i++] = string.Format("|{0,3}|{1,10}|{2,12}|{3,3}|{4,7}|{5,27}|{6,13}|{7,13}|{8,11}|{9,30}|{10,15}|{11,6}|{12,6}|",
            "Id", "FirstName", "LastName", "Age", "Gender", "Email", "Username", "Password", "BirthDate", "Address", "City", "Zip", "State");
        table[i++] = new String('-', 170);

        foreach (User u in users)
        {
            string s = string.Format("|{0,3}|{1,10}|{2,12}|{3,3}|{4,7}|{5,27}|{6,13}|{7,13}|{8,11}|{9,30}|{10,15}|{11,6}|{12,6}|",
                u.Id, u.FirstName, u.LastName, u.Age, u.Gender, u.Email, u.Username, u.Password, u.BirthDate.ToShortDateString(), u.Address.Address, u.Address.City, u.Address.PostalCode, u.Address.State);

            table[i++] = s;
        }
        table[i++] = new String('-', 170);
        return table;
    }

    public static bool VerificaCredenziali(string u, string p)
    {
        if (u == null || p == null) return false;
        if (string.IsNullOrEmpty(u) || string.IsNullOrEmpty(p)) return false;

        //string connectionString = @"Server=DESKTOP-24NCMVV\SQLEXPRESS;Database=Users;Integrated Security=True;TrustServerCertificate=True";            
        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            try
            {
                connection.Open();

                SqlCommand command = new SqlCommand();
                command.Connection = connection;
                command.CommandText = "SELECT COUNT(*) AS UtentiValidi FROM Users";
                command.CommandText += " WHERE Username=@u AND Password=@p";

                //SqlParameter p1 = new SqlParameter("@u", u);
                //command.Parameters.Add(p1);
                //command.Parameters.Add(new SqlParameter("@u", u));

                command.Parameters.AddWithValue("@u", u);
                command.Parameters.AddWithValue("@p", p);

                //return ((int)command.ExecuteScalar()) > 0;
                int utentiTrovati = (int)command.ExecuteScalar();
                return utentiTrovati == 1;
            }
            catch (Exception e)
            {
                throw;
            }
        }


        /*User user = Find(q => q.Username.ToLower() == u.ToLower() );

        if (user != null && user.Password == p)
        {
            Logins.Add(new Login(user.Id, true, DateTime.Now));
            return true;
        }
        else if( user != null && user.Password != p)
        {
            Logins.Add(new Login(user.Id, false, DateTime.Now));
            return false;
        }
        else
        {
            Logins.Add(new Login(-1, false, DateTime.Now));
            return false;
        }*/
    }

    public static bool InviaMailDiRecupero(string m)
    {
        return Find(m) != null;//passo la stringa mail scritta dall'utente e poi uso il 2 find che cerca per email
    }

    //private perchè siamo nel controller quindi deve rimanere in questa classe
    //creiamo un metodo statico con in input il data reader e come ritorno l'oggetto user e statico perchè non va istanziato
    private static User ReadToUser(SqlDataReader reader) //oggetto reader di classe sqldatareader
    {
        return new User
        {

            Id = (int)reader["Id"],
            FirstName = (string)reader["FirstName"],
            LastName = (string)reader["LastName"],
            Age = (int)reader["Age"],
            Gender = (string)reader["Gender"],
            Email = (string)reader["Email"],
            Username = (string)reader["Username"],
            Password = (string)reader["Password"],
            BirthDate = (DateTime)reader["BirthDate"],
            Address = new FullAddress
            {
                Address = (string)reader["Address"],
                City = (string)reader["City"],
                PostalCode = (string)reader["PostalCode"],
                State = (string)reader["State"],
            }
        };

    }
}

}
