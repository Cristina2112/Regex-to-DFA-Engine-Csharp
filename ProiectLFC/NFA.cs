using System.Collections.Generic;
using System.Linq;
using System; 

public class NFA
{

    public HashSet<int> States { get; set; } = new HashSet<int>();

    public HashSet<char> Alphabet { get; set; } = new HashSet<char>();

    public Dictionary<(int, char), HashSet<int>> Transitions { get; set; } = new Dictionary<(int, char), HashSet<int>>();

    public HashSet<(int, int)> EpsilonTransitions { get; set; } = new HashSet<(int, int)>();

    public int StartState { get; set; }

    public HashSet<int> AcceptStates { get; set; } = new HashSet<int>();


    public void PrintNFA()
    {
        Console.WriteLine("\n=== Nondeterministic Finite Automaton (NFA with Epsilon) ===");
        Console.WriteLine($"Start State (q0): {StartState}");
        Console.WriteLine($"Accept States (F): {{{string.Join(", ", AcceptStates.OrderBy(x => x))}}}");
        Console.WriteLine($"Alphabet (Σ): {{{string.Join(", ", Alphabet.OrderBy(x => x))}}}");
        Console.WriteLine($"States (Q): {{{string.Join(", ", States.OrderBy(x => x))}}}");

        Console.WriteLine("\nTransition Function (δ):");
        Console.WriteLine($"{"State",-10} | {"Symbol",-10} | {"Next States",-20}");
        Console.WriteLine(new string('-', 45));

        // Sortare tranziții
        var sortedTransitions = Transitions
            .OrderBy(x => x.Key.Item1)
            .ThenBy(x => x.Key.Item2);

        foreach (var transition in sortedTransitions)
        {
            var (state, symbol) = transition.Key;
            var nextStates = transition.Value;
            Console.WriteLine($"{state,-10} | {symbol,-10} | {{{string.Join(", ", nextStates.OrderBy(x => x))}}}");
        }

        Console.WriteLine("\nEpsilon Transitions (λ):");
        Console.WriteLine($"{"From",-10} | {"To",-10}");
        Console.WriteLine(new string('-', 21));

        // Sortare λ-tranziții
        foreach (var (from, to) in EpsilonTransitions.OrderBy(t => t.Item1).ThenBy(t => t.Item2))
        {
            Console.WriteLine($"{from,-10} | {to,-10}");
        }
    }
}