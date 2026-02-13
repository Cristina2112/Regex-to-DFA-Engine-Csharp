using ProiectLFC;
using System;
using System.IO;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== DFA Generator from Regular Expressions ===\n");

        string regex = ReadRegexFromFile();

        if (string.IsNullOrWhiteSpace(regex))
        {
            Console.WriteLine("Error: Could not read regex from file or regex is empty.");
            return;
        }

        Console.WriteLine($"Input regex: {regex}\n");

        Console.WriteLine("Converting regex to DFA...\n");
        var converter = new RegexToDFA(regex);
        var dfa = converter.ConvertToDFA();

        if (!dfa.VerifyAutomaton())
        {
            Console.WriteLine("Error: Generated DFA is invalid.");
            return;
        }

        Console.WriteLine("DFA generated successfully!\n");

        DisplayMenu(converter, dfa);
    }

    static string ReadRegexFromFile()
    {
        try
        {
            string[] pathsToCheck = new[]
            {
                "regex.txt",
                Path.Combine(Directory.GetCurrentDirectory(), "regex.txt"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "regex.txt"),
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "regex.txt"),
            };

            foreach (string filePath in pathsToCheck)
            {
                if (File.Exists(filePath))
                {
                    Console.WriteLine($"Reading from: {Path.GetFullPath(filePath)}\n");
                    string content = File.ReadAllText(filePath).Trim();
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        return content;
                    }
                }
            }

            Console.WriteLine($"File 'regex.txt' not found in the following locations:");
            foreach (string path in pathsToCheck)
            {
                Console.WriteLine($"  - {Path.GetFullPath(path)}");
            }
            Console.WriteLine($"\nCurrent working directory: {Directory.GetCurrentDirectory()}");
            Console.WriteLine($"Application directory: {AppDomain.CurrentDomain.BaseDirectory}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading file: {ex.Message}");
            return null;
        }
    }

    static void DisplayMenu(RegexToDFA converter, DeterministicFiniteAutomaton dfa)
    {
        bool running = true;

        while (running)
        {
            Console.WriteLine("\n=== Menu ===");
            Console.WriteLine("1. Display postfix form of regex");
            Console.WriteLine("2. Display parse tree of regex");
            Console.WriteLine("3. Display DFA (transition table to console)");
            Console.WriteLine("4. Export DFA to file");
            Console.WriteLine("5. Check if a word is accepted");
            Console.WriteLine("6. Check multiple words");
            Console.WriteLine("0. Exit");
            Console.WriteLine("\nChoose an option: ");

            string choice = Console.ReadLine()?.Trim() ?? "";

            switch (choice)
            {
                case "1":
                    DisplayPostfixForm(converter);
                    break;
                case "2":
                    DisplayParseTree(converter);
                    break;
                case "3":
                    dfa.PrintAutomaton();
                    break;
                case "4":
                    ExportDFAToFile(dfa);
                    break;
                case "5":
                    CheckSingleWord(dfa);
                    break;
                case "6":
                    CheckMultipleWords(dfa);
                    break;
                case "0":
                    running = false;
                    Console.WriteLine("\nExiting program...");
                    break;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }
        }
    }

    static void DisplayPostfixForm(RegexToDFA converter)
    {
        string postfix = converter.ToPostfix();
        Console.WriteLine($"\nPostfix notation: {postfix}");
    }

    static void DisplayParseTree(RegexToDFA converter)
    {
        converter.PrintParseTree();
    }

    static void ExportDFAToFile(DeterministicFiniteAutomaton dfa)
    {
        Console.WriteLine("\nEnter output filename (default: dfa_output.txt): ");
        string filename = Console.ReadLine()?.Trim();

        if (string.IsNullOrWhiteSpace(filename))
            filename = "dfa_output.txt";

        string baseDir = AppDomain.CurrentDomain.BaseDirectory;

        string projectFolder = Path.GetFullPath(Path.Combine(baseDir, "..", "..", ".."));

        string fullPath = Path.Combine(projectFolder, filename);

        try
        {
            dfa.ExportAutomaton(fullPath);
            Console.WriteLine($"DFA exported successfully to: {fullPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error exporting DFA: {ex.Message}");
        }
    }

    static void CheckSingleWord(DeterministicFiniteAutomaton dfa)
    {
        Console.WriteLine("\nEnter a word to check: ");
        string word = Console.ReadLine() ?? "";

        bool accepted = dfa.CheckWord(word);
        Console.WriteLine($"Word '{word}': {(accepted ? "ACCEPTED" : "REJECTED")}");
    }

    static void CheckMultipleWords(DeterministicFiniteAutomaton dfa)
    {
        Console.WriteLine("\nEnter words to check (one per line, empty line to finish):");

        while (true)
        {
            string word = Console.ReadLine();
            if (string.IsNullOrEmpty(word))
                break;

            bool accepted = dfa.CheckWord(word);
            Console.WriteLine($"Word '{word}': {(accepted ? "ACCEPTED" : "REJECTED")}");
        }
    }
}