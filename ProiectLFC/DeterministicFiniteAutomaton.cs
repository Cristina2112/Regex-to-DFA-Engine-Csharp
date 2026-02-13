using System;
using System.Collections.Generic;
using System.IO; 
using System.Linq;

namespace ProiectLFC
{
    internal class DeterministicFiniteAutomaton
    {
        public HashSet<int> States { get; set; }
        public HashSet<char> Alphabet { get; set; }
        public Dictionary<(int, char), int> TransitionFunction { get; set; }
        public int InitialState { get; set; }
        public HashSet<int> FinalStates { get; set; }

        public DeterministicFiniteAutomaton()
        {
            States = new HashSet<int>();
            Alphabet = new HashSet<char>();
            TransitionFunction = new Dictionary<(int, char), int>();
            InitialState = 0;
            FinalStates = new HashSet<int>();
        }

        public DeterministicFiniteAutomaton(HashSet<int> states, HashSet<char> alphabet,
            Dictionary<(int, char), int> transitionFunction, int initialState, HashSet<int> finalStates)
        {
            States = states;
            Alphabet = alphabet;
            TransitionFunction = transitionFunction;
            InitialState = initialState;
            FinalStates = finalStates;
        }

        public bool VerifyAutomaton()
        {
            if (!States.Contains(InitialState))
            {
                Console.WriteLine("Starea initiala nu este in multimea starilor.");
                return false;
            }

            foreach (int finalState in FinalStates)
            {
                if (!States.Contains(finalState))
                {
                    Console.WriteLine($"Starea finala {finalState} nu este in multimea starilor.");
                    return false;
                }
            }
            foreach (var ((state, symbol), nextState) in TransitionFunction)
            {
                if (!States.Contains(state))
                {
                    Console.WriteLine($"Starea {state} din functia de tranzitie nu este in multimea starilor.");
                    return false;
                }
                if (!Alphabet.Contains(symbol))
                {
                    Console.WriteLine($"Simbolul {symbol} din functia de tranzitie nu este in alfabet.");
                    return false;
                }
                if (!States.Contains(nextState))
                {
                    Console.WriteLine($"Starea urmatoare {nextState} din functia de tranzitie nu este in multimea starilor.");
                    return false;
                }
            }
            return true;
        }

       
        public void PrintAutomaton()
        {
            Console.WriteLine("\n=== Deterministic Finite Automaton ===");
            Console.WriteLine($"Initial state: {InitialState}");
            Console.WriteLine($"Final states:  {{ {string.Join(", ", FinalStates.OrderBy(x => x))} }}");
            Console.WriteLine();

            var sortedAlphabet = Alphabet.OrderBy(x => x).ToList();
            var sortedStates = States.OrderBy(x => x).ToList();

            int colWidthState = 12;
            int colWidthSymbol = 8;

            void DrawSeparator()
            {
                Console.Write("+");
                Console.Write(new string('-', colWidthState));
                foreach (var _ in sortedAlphabet)
                {
                    Console.Write($"+{new string('-', colWidthSymbol)}");
                }
                Console.WriteLine("+");
            }

            DrawSeparator();

            Console.Write("| ");
            Console.Write("Delta".PadRight(colWidthState - 1));
            foreach (var symbol in sortedAlphabet)
            {
                Console.Write("| ");
                Console.Write(symbol.ToString().PadRight(colWidthSymbol - 1));
            }
            Console.WriteLine("|");

            DrawSeparator();

            foreach (var state in sortedStates)
            {
                string prefix = "";
                if (state == InitialState) prefix += "->";
                if (FinalStates.Contains(state)) prefix += "*";
                string stateLabel = $"{prefix}q{state}";

                Console.Write("| ");
                Console.Write(stateLabel.PadRight(colWidthState - 1));

                foreach (var symbol in sortedAlphabet)
                {
                    Console.Write("| ");
                    string cellContent = "-";
                    if (TransitionFunction.TryGetValue((state, symbol), out int nextState))
                    {
                        cellContent = $"q{nextState}";
                    }
                    Console.Write(cellContent.PadRight(colWidthSymbol - 1));
                }
                Console.WriteLine("|");
            }

            DrawSeparator();
        }

        public bool CheckWord(String word)
        {
            int currentState = InitialState;
            foreach (char symbol in word)
            {
                if (!Alphabet.Contains(symbol))
                {
                    Console.WriteLine($"Simbolul '{symbol}' nu face parte din alfabet.");
                    return false;
                }
                if (!TransitionFunction.TryGetValue((currentState, symbol), out int nextState))
                {
                    return false; // tranzitie inexistenta
                }
                currentState = nextState;
            }
            return FinalStates.Contains(currentState);
        }

       
        public void ExportAutomaton(string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                writer.WriteLine("=== Deterministic Finite Automaton ===");
                writer.WriteLine($"Initial state: {InitialState}");
                writer.WriteLine($"Final states: {{ {string.Join(", ", FinalStates.OrderBy(x => x))} }}");
                writer.WriteLine();

                var sortedAlphabet = Alphabet.OrderBy(x => x).ToList();
                var sortedStates = States.OrderBy(x => x).ToList();

                int colWidthState = 12;
                int colWidthSymbol = 8;

                void WriteSeparator()
                {
                    writer.Write("+");
                    writer.Write(new string('-', colWidthState));
                    foreach (var _ in sortedAlphabet)
                    {
                        writer.Write($"+{new string('-', colWidthSymbol)}");
                    }
                    writer.WriteLine("+");
                }

                WriteSeparator();

                writer.Write("| ");
                writer.Write("Delta".PadRight(colWidthState - 1));
                foreach (var symbol in sortedAlphabet)
                {
                    writer.Write("| ");
                    writer.Write(symbol.ToString().PadRight(colWidthSymbol - 1));
                }
                writer.WriteLine("|");

                WriteSeparator();

                foreach (var state in sortedStates)
                {
                    string prefix = "";
                    if (state == InitialState) prefix += "->";
                    if (FinalStates.Contains(state)) prefix += "*";
                    string stateLabel = $"{prefix}q{state}";

                    writer.Write("| ");
                    writer.Write(stateLabel.PadRight(colWidthState - 1));

                    foreach (var symbol in sortedAlphabet)
                    {
                        writer.Write("| ");
                        string cellContent = "-";
                        if (TransitionFunction.TryGetValue((state, symbol), out int nextState))
                        {
                            cellContent = $"q{nextState}";
                        }
                        writer.Write(cellContent.PadRight(colWidthSymbol - 1));
                    }
                    writer.WriteLine("|");
                }

                WriteSeparator();
            }
        }
    }
}