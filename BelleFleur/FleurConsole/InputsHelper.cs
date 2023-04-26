namespace FleurConsole {

    using System;

    public class InputsHelper {

        // Method to read an integer input from the user
        public static int Int(string Nom, int min, int max) {

            // Prompt the user to enter an integer within the specified range
            Console.Write($"{Nom} ({min}-{max}) > ");

            // Read the input as a string
            string i = Console.ReadLine();

            int r;

            // If the input is empty, return -1
            // if (i == "") {
            //     return -1;
            // }

            // Try to parse the input as an integer
            if (Int32.TryParse(i, out r)) {

                // If the input is within the specified range, return it
                if (r >= min && r <= max) {
                    return r;
                }

            }

            // If the input is invalid or outside the specified range, display an error message
            Console.WriteLine("Entrée invalide.");

            // Call the Int method recursively until a valid input is entered
            return Int(Nom, min, max);
        }

        public static decimal Decimal(string Nom, decimal min, decimal max) {

            // Prompt the user to enter an integer within the specified range
            Console.Write($"{Nom} ({min}-{max}) > ");

            // Read the input as a string
            string i = Console.ReadLine();

            decimal r;

            // If the input is empty, return -1
            // if (i == "") {
            //     return -1;
            // }

            // Try to parse the input as an integer
            if (decimal.TryParse(i, out r)) {

                // If the input is within the specified range, return it
                if (r >= min && r <= max) {
                    return r;
                }

            }

            // If the input is invalid or outside the specified range, display an error message
            Console.WriteLine("Entrée invalide.");
    
            // Call the Int method recursively until a valid input is entered
            return Decimal(Nom, min, max);
        }

        // Method to read a string input from the user
        public static string Text(string Nom, int min, int max, bool password=false) {

            // Prompt the user to enter a string within the specified length range
            Console.Write($"{Nom} > ");
            string input;


            // If the password parameter is true, mask the user's input with asterisks
            if (password) {
                input = "";
                ConsoleKeyInfo key;
                do {
                    key = Console.ReadKey(true);

                    // Ignore non-character keys (e.g. arrow keys)
                    if (Char.IsControl(key.KeyChar)) {
                        continue;
                    }

                    // Append each character to the input string and display an asterisk on the console
                    input += key.KeyChar;
                    Console.Write("*");
                } while (key.Key != ConsoleKey.Enter && input.Length < max);
            } else {
                input = Console.ReadLine();
            }
            Console.WriteLine();

            // If the input is empty, return null
            // if (input == "") {
            //     return null;
            // }

            if (input.Length >= min && input.Length <= max) {
                return input;
            } else {
                Console.WriteLine($"Entrée invalide. La longueur de {Nom} doit être comprise entre {min} et {max} caractères (trouvé {input.Length}).");
                return Text(Nom, min, max, password);
            }
        }

        public static DateTime Date(string Nom, DateTime min, DateTime max) {

            // Prompt the user to enter a string within the specified length range
            Console.Write($"{Nom} ({min.ToShortDateString()}-{max.ToShortDateString()}) > ");

            // Read the input as a string
            string input = Console.ReadLine();

            DateTime r;

            // If the input is empty, return -1
            // if (i == "") {
            //     return -1;
            // }

            // Try to parse the input as an integer
            if (DateTime.TryParse(input, out r)) {

                // If the input is within the specified range, return it
                if (r >= min && r <= max) {
                    return r;
                }

            }

            // If the input is invalid or outside the specified range, display an error message
            Console.WriteLine("Entrée invalide.");

            // Call the Int method recursively until a valid input is entered
            return Date(Nom, min, max);
        }

        public static string Email(string Nom, int min, int max) {

            // Prompt the user to enter a string within the specified length range
            Console.Write($"{Nom} > ");

            // Read the input as a string
            string input = Console.ReadLine();

            // If the input is empty, return null
            // if (input == "") {
            //     return null;
            // }

            if (input.Length >= min && input.Length <= max && input.Contains("@")) {
                return input;
            } else {
                Console.WriteLine($"Entrée invalide. La longueur de {Nom} doit être comprise entre {min} et {max} caractères (trouvé {input.Length}).");
                return Email(Nom, min, max);
            }
        }
    }
}
