using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.RegularExpressions;


namespace Dzietność
{

    public partial class Logowanie : Form
    {
        static string connectionString = "mongodb://localhost:27017/";
        static string databaseName = "Users";
        static string collectionName = "User";
        //zapis
        private bool IsValidEmail(string email)
        {
            var emailRegex = new Regex(@"^[^@\s]+@[^@\s]+\.[^@\s]+$");
            return emailRegex.IsMatch(email);
        }

        // Password validation method
        private bool IsValidPassword(string password)
        {
            var passwordRegex = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$");
            return passwordRegex.IsMatch(password);
        }
        private ClaimsIdentity CreateClaimsIdentity(Uzytkownik user)
        {
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.email),
        new Claim(ClaimTypes.Role, user.password),
        // Możesz dodać inne dane użytkownika tutaj
    };

            var identity = new ClaimsIdentity(claims, "jwt");
            return identity;
        }
        private void SaveTokenToFile(string token)
        {
            var configFilePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\config.json";
            var config = new { JwtToken = token };
            File.WriteAllText(configFilePath, JsonConvert.SerializeObject(config));
        }
        //odczyt
        private string LoadTokenFromFile()
        {
            var configFilePath = $@"{AppDomain.CurrentDomain.BaseDirectory}\config.json";
            if (File.Exists(configFilePath))
            {
                var json = File.ReadAllText(configFilePath);
                var config = JsonConvert.DeserializeObject<dynamic>(json);
                return config.JwtToken;
            }
            return null;
        }
        public static string Base64UrlDecode(string input)
        {
            string padded = input.Length % 4 == 0 ? input : input + new string('=', 4 - input.Length % 4);
            var base64 = padded.Replace('-', '+').Replace('_', '/');
            var bytes = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(bytes);
        }

        public Logowanie()
        {
            
            InitializeComponent();
            sprawdzenieLogowania();



        }
        private async void sprawdzenieLogowania()
        {
            
                string configFillePath = "config.json";
                if(File.Exists(configFillePath))
                {
                    var client = new MongoClient(connectionString);
                    var db = client.GetDatabase(databaseName);
                    var collection = db.GetCollection<Uzytkownik>(collectionName);
                    var filter = Builders<Uzytkownik>.Filter.Eq("email", tokenik()[0]) & Builders<Uzytkownik>.Filter.Eq("password", tokenik()[1]);
                    var user = await collection.Find(filter).FirstOrDefaultAsync();
                    if (user != null)
                    {
                        try
                        {

                            Form1 form1 = new Form1(tokenik()[0]);

                            this.Hide();

                            form1.Closed += (s, args) => this.Close(); // Zamknij okno logowania po zamknięciu Form1

                            form1.Show();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Błąd podczas automatycznego logowania: {ex.Message}");
                        }

                    }

                }
                
            

            
            
        }
        public string[] tokenik()
        {
            string token = LoadTokenFromFile();
            //MessageBox.Show(token);
            string[] tokenParts = token.Split('.');
            string encodedPayload = tokenParts[1];
            //MessageBox.Show(encodedPayload);


            string decodedPayload = Base64UrlDecode(encodedPayload);
            //MessageBox.Show(decodedPayload);
            // Deserializacja JSON do obiektu JwtPayload

            Uzytkownik jwtPayload = JsonConvert.DeserializeObject<Uzytkownik>(decodedPayload);
            //MessageBox.Show($"emial{jwtPayload.email}");
           // MessageBox.Show($"Id: {jwtPayload.Id}");
            //MessageBox.Show($"Passowrd: {jwtPayload.password}");
            string[] lista = { jwtPayload.email, jwtPayload.password };
            return lista;
        }

        private void label1_Click(object sender, EventArgs e)
        {       

        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(databaseName);
            var collection = db.GetCollection<Uzytkownik>(collectionName);

            string email = textBox3.Text;
            string password = textBox4.Text;
            string emailLogowanie = textBox1.Text;
            string passwordLogowanie = textBox2.Text;



            if (panel1.Visible)
            {
                // Login logic
                // Check if user exists with provided email and password
                var filter = Builders<Uzytkownik>.Filter.Eq("email", emailLogowanie) & Builders<Uzytkownik>.Filter.Eq("password", passwordLogowanie);
                var user = await collection.Find(filter).FirstOrDefaultAsync();

                //MessageBox.Show($"{filter}");
                if (user != null)
                {
                    try
                    {
                        var identity = CreateClaimsIdentity(user);
                        string token = JwtHelper.GenerateToken(identity);
                        SaveTokenToFile(token);
                        //MessageBox.Show($"Login successful!");
                        
                        Form1 form1 = new Form1(tokenik()[0]);
                       
                        this.Hide();
                        form1.Closed += (s, args) => this.Close(); // Zamknij okno logowania po zamknięciu Form1
                        form1.Show();


                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Błąd podczas generowania tokena JWT: {ex.Message}");
                    }
                    
                    // You can store the token in memory or local storage for further use
                }
                else
                {
                    // Invalid credentials
                    MessageBox.Show("Invalid email or password");
                }
            }
            else
            {
                // Registration logic
                string passwordCorrect = textBox5.Text;

                if (!IsValidEmail(email))
                {
                    MessageBox.Show("Invalid email format");
                    return;
                }

                if (!IsValidPassword(password))
                {
                    MessageBox.Show("Password must contain at least one lowercase letter, one uppercase letter, one digit, one special character, and be at least 8 characters long");
                    return;
                }
                if (password == passwordCorrect)
                {
                    // Check if user with the same email already exists
                    var existingUser = await collection.Find(Builders<Uzytkownik>.Filter.Eq("email", email)).FirstOrDefaultAsync();

                    if (existingUser == null)
                    {
                        // User doesn't exist, create a new user
                        var uzytkownik = new Uzytkownik { email = email, password = password };
                        await collection.InsertOneAsync(uzytkownik);
                        MessageBox.Show("Rejestracja zakończona sukcesem");
                    }
                    else
                    {
                        // User already exists
                        MessageBox.Show("User with this email already exists");
                    }
                }
                else
                {
                    // Passwords do not match
                    MessageBox.Show("Hasła się nie zgadzają");
                }
                /*
                if (panel1.Visible ==false)
                {
                    var client = new MongoClient(connectionString);
                    var db = client.GetDatabase(databaseName);
                    var collection = db.GetCollection<Uzytkownik>(collectionName);
                    string emali = textBox3.Text;
                    string password = textBox4.Text;
                    string passwordCorrect = textBox5.Text;
                    if (password==passwordCorrect)
                    {
                        var urzytkownik = new Uzytkownik { email = emali, password = password };
                        await collection.InsertOneAsync(urzytkownik);
                        MessageBox.Show("Rejestracja zakończona sukcesem");
                    }
                    else
                    {
                        MessageBox.Show("Hasła się nie zgadzają");
                    }*/
                textBox3.Clear();
                textBox4.Clear();
                textBox5.Clear();
            }

            /*
            Form1 form1 = new Form1();
            form1.ShowDialog();
            */
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBox1.Text = "";
            textBox2.Text = "";
            textBox3.Text = "";
            textBox4.Text = "";
            textBox5.Text = "";
            panel2.Visible =! bool.Parse(panel2.Visible.ToString());
            panel1.Visible = !bool.Parse(panel1.Visible.ToString());

            if(panel1.Visible ) 
            {
                button2.Text = "Rejestracja";
                button1.Text = "Zaloguj";
            }
            else
            {
                button2.Text = "Logowanie";
                button1.Text = "Zarejestruj";
            }
            
        }
    }
}
