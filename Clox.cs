using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

namespace clox
{
    class Clox
    {
        private static bool hadError;

        static void Main(string[] args)
        {
            if (args.Length > 1)
            {
                Console.WriteLine("Usage: clox[script]");
                Environment.Exit(64);
            }
            else if (args.Length == 1)
            {
                runFile(args[0]);
            }
            else
            {
                runPrompt();
            }
        }

        private static void runPrompt()
        {
            for (;;)
            {
                Console.Write("> ");
                run(Console.ReadLine());
                hadError = false;
            }
        }

        private static void runFile(string path)
        {
            byte[] bytes = File.ReadAllBytes(Path.GetFullPath(path));
            run(System.Text.Encoding.Default.GetString(bytes));
            if(hadError) Environment.Exit(65);
        }

        private static void run(string source)
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.scanTokens();

            // For now, just print the tokens.        
            foreach (Token token in tokens)
            {
                Console.WriteLine(token.toString());
            }
        }

        public static void error(int line, String message)
        {
            report(line, "", message);
        }

        private static void report(int line, String where, String message)
        {
            Console.Error.WriteLine("[line " + line + "] Error" + where + ": " + message);
            hadError = true;
        }
    }
}