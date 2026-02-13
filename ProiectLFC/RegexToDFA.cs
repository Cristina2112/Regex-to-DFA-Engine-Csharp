using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProiectLFC
{
    internal class RegexToDFA
    {
        private string regex;
        private HashSet<char> alphabet;
        private int stateCounter;

        public RegexToDFA(string regex)
        {
            this.regex = AddExplicitConcatenation(regex);
            this.alphabet = new HashSet<char>();
            this.stateCounter = 0;
        }

        private string AddExplicitConcatenation(string regex)
        {
            if (string.IsNullOrEmpty(regex)) return regex;

            StringBuilder res = new StringBuilder();
            res.Append(regex[0]);

            for (int i = 1; i < regex.Length; i++)
            {
                char c1 = regex[i - 1];
                char c2 = regex[i];

                bool c1IsLiteral = !IsOperator(c1) && c1 != '(';
                bool c2IsLiteral = !IsOperator(c2) && c2 != ')';

                bool insertDot = false;
                if (c1IsLiteral && c2IsLiteral) insertDot = true; 
                if (c1IsLiteral && c2 == '(') insertDot = true;   
                if (c1 == ')' && c2IsLiteral) insertDot = true;  
                if ((c1 == '*' || c1 == '+') && (c2IsLiteral || c2 == '(')) insertDot = true; 
                if (c1 == ')' && c2 == '(') insertDot = true;   

                if (insertDot) res.Append('.');
                res.Append(c2);
            }
            return res.ToString();
        }

        private bool IsOperator(char c)
        {
            return c == '|' || c == '.' || c == '*' || c == '+' || c == '(' || c == ')';
        }

        public DeterministicFiniteAutomaton ConvertToDFA()
        {
            string postfix = ToPostfix();
            var nfa = BuildNFAFromPostfix(postfix);
            var dfa = ConvertNFAToDFA(nfa);
            return dfa;
        }

        public string ToPostfix()
        {
            var result = new StringBuilder();
            var stack = new Stack<char>();

            var precedence = new Dictionary<char, int>
            {
                 { '|', 0 },
                 { '.', 1 },
                 { '*', 2 },
                 { '+', 2 }
            };

            for (int i = 0; i < regex.Length; i++)
            {
                char c = regex[i];

                if (c == '(')
                {
                    stack.Push(c);
                }
                else if (c == ')')
                {
                    while (stack.Count > 0 && stack.Peek() != '(')
                    {
                        result.Append(stack.Pop());
                    }
                    if (stack.Count > 0)
                        stack.Pop();
                }
                else if (c == '|' || c == '.' || c == '*' || c == '+')
                {
                    if (c != '*' && c != '+')
                    {
                        while (stack.Count > 0 && stack.Peek() != '(' &&
                               precedence.ContainsKey(stack.Peek()) &&
                               precedence[stack.Peek()] >= precedence[c])
                        {
                            result.Append(stack.Pop());
                        }
                    }
                    stack.Push(c);
                }
                else
                {
                    result.Append(c);
                    alphabet.Add(c);
                }
            }

            while (stack.Count > 0)
            {
                result.Append(stack.Pop());
            }

            return result.ToString();
        }

        private NFA BuildNFAFromPostfix(string postfix)
        {
            var stack = new Stack<NFA>();

            foreach (char c in postfix)
            {
                if (c == '|')
                {
                    var right = stack.Pop();
                    var left = stack.Pop();
                    stack.Push(ConcatenateNFAs(left, right, '|'));
                }
                else if (c == '.')
                {
                    var right = stack.Pop();
                    var left = stack.Pop();
                    stack.Push(ConcatenateNFAs(left, right, '.'));
                }
                else if (c == '*')
                {
                    var nfa = stack.Pop();
                    stack.Push(ApplyStar(nfa));
                }
                else if (c == '+')
                {
                    var nfa = stack.Pop();
                    stack.Push(ApplyPlus(nfa));
                }
                else
                {
                    stack.Push(CreateBasicNFA(c));
                }
            }

            return stack.Count > 0 ? stack.Pop() : new NFA();
        }

        private NFA CreateBasicNFA(char symbol)
        {
            int state1 = stateCounter++;
            int state2 = stateCounter++;

            var nfa = new NFA();
            nfa.States.Add(state1);
            nfa.States.Add(state2);
            nfa.Alphabet.Add(symbol);
            nfa.Transitions.Add((state1, symbol), new HashSet<int> { state2 });
            nfa.StartState = state1;
            nfa.AcceptStates.Add(state2);

            return nfa;
        }

        private NFA ConcatenateNFAs(NFA left, NFA right, char op)
        {
            var nfa = new NFA();

            if (op == '.')
            {
                nfa.States.UnionWith(left.States);
                nfa.States.UnionWith(right.States);
                nfa.Alphabet.UnionWith(left.Alphabet);
                nfa.Alphabet.UnionWith(right.Alphabet);

                foreach (var kvp in left.Transitions) nfa.Transitions.Add(kvp.Key, kvp.Value);
                foreach (var kvp in right.Transitions) nfa.Transitions.Add(kvp.Key, kvp.Value);

                nfa.EpsilonTransitions.UnionWith(left.EpsilonTransitions);
                nfa.EpsilonTransitions.UnionWith(right.EpsilonTransitions);

                foreach (int acceptState in left.AcceptStates)
                {
                    nfa.EpsilonTransitions.Add((acceptState, right.StartState));
                }

                nfa.StartState = left.StartState;
                nfa.AcceptStates = new HashSet<int>(right.AcceptStates);
            }
            else if (op == '|')
            {
                int newStart = stateCounter++;
                int newAccept = stateCounter++;

                nfa.States.Add(newStart);
                nfa.States.Add(newAccept);
                nfa.States.UnionWith(left.States);
                nfa.States.UnionWith(right.States);
                nfa.Alphabet.UnionWith(left.Alphabet);
                nfa.Alphabet.UnionWith(right.Alphabet);

                foreach (var kvp in left.Transitions) nfa.Transitions.Add(kvp.Key, kvp.Value);
                foreach (var kvp in right.Transitions) nfa.Transitions.Add(kvp.Key, kvp.Value);

                nfa.EpsilonTransitions.UnionWith(left.EpsilonTransitions);
                nfa.EpsilonTransitions.UnionWith(right.EpsilonTransitions);

                nfa.EpsilonTransitions.Add((newStart, left.StartState));
                nfa.EpsilonTransitions.Add((newStart, right.StartState));

                foreach (int acceptState in left.AcceptStates)
                    nfa.EpsilonTransitions.Add((acceptState, newAccept));
                foreach (int acceptState in right.AcceptStates)
                    nfa.EpsilonTransitions.Add((acceptState, newAccept));

                nfa.StartState = newStart;
                nfa.AcceptStates.Add(newAccept);
            }

            return nfa;
        }

        private NFA ApplyStar(NFA nfa)
        {
            int newStart = stateCounter++;
            int newAccept = stateCounter++;

            var result = new NFA();
            result.States.Add(newStart);
            result.States.Add(newAccept);
            result.States.UnionWith(nfa.States);
            result.Alphabet.UnionWith(nfa.Alphabet);

            foreach (var kvp in nfa.Transitions)
                result.Transitions.Add(kvp.Key, kvp.Value);

            result.EpsilonTransitions.UnionWith(nfa.EpsilonTransitions);

            result.EpsilonTransitions.Add((newStart, nfa.StartState));
            result.EpsilonTransitions.Add((newStart, newAccept));

            foreach (int acceptState in nfa.AcceptStates)
            {
                result.EpsilonTransitions.Add((acceptState, nfa.StartState));
                result.EpsilonTransitions.Add((acceptState, newAccept));
            }

            result.StartState = newStart;
            result.AcceptStates.Add(newAccept);

            return result;
        }

        private NFA ApplyPlus(NFA nfa)
        {
            int newAccept = stateCounter++;

            var result = new NFA();
            result.States.Add(newAccept);
            result.States.UnionWith(nfa.States);
            result.Alphabet.UnionWith(nfa.Alphabet);

            foreach (var kvp in nfa.Transitions)
                result.Transitions.Add(kvp.Key, kvp.Value);

            result.EpsilonTransitions.UnionWith(nfa.EpsilonTransitions);

            foreach (int acceptState in nfa.AcceptStates)
            {
                result.EpsilonTransitions.Add((acceptState, nfa.StartState));
                result.EpsilonTransitions.Add((acceptState, newAccept));
            }

            result.StartState = nfa.StartState;
            result.AcceptStates.Add(newAccept);

            return result;
        }

        private DeterministicFiniteAutomaton ConvertNFAToDFA(NFA nfa)
        {
            var dfa = new DeterministicFiniteAutomaton();
            var stateMapping = new Dictionary<HashSet<int>, int>(new SetComparer());
            var workList = new Queue<HashSet<int>>();
            int dfaStateCounter = 0;

            var startSet = EpsilonClosure(nfa, nfa.StartState);
            stateMapping[startSet] = dfaStateCounter++;
            workList.Enqueue(startSet);

            dfa.InitialState = stateMapping[startSet];
            dfa.States.Add(dfa.InitialState);

            if (startSet.Overlaps(nfa.AcceptStates))
                dfa.FinalStates.Add(dfa.InitialState);

            dfa.Alphabet.UnionWith(nfa.Alphabet);

            while (workList.Count > 0)
            {
                var currentSet = workList.Dequeue();
                int currentState = stateMapping[currentSet];

                foreach (char symbol in nfa.Alphabet)
                {
                    var nextSet = new HashSet<int>();

                    foreach (int state in currentSet)
                    {
                        if (nfa.Transitions.TryGetValue((state, symbol), out var targets))
                        {
                            nextSet.UnionWith(targets);
                        }
                    }

                    var nextSetWithClosure = new HashSet<int>();
                    foreach (int state in nextSet)
                    {
                        nextSetWithClosure.UnionWith(EpsilonClosure(nfa, state));
                    }

                    if (nextSetWithClosure.Count > 0)
                    {
                        if (!stateMapping.ContainsKey(nextSetWithClosure))
                        {
                            stateMapping[nextSetWithClosure] = dfaStateCounter++;
                            workList.Enqueue(nextSetWithClosure);
                            dfa.States.Add(stateMapping[nextSetWithClosure]);

                            if (nextSetWithClosure.Overlaps(nfa.AcceptStates))
                                dfa.FinalStates.Add(stateMapping[nextSetWithClosure]);
                        }

                        int nextState = stateMapping[nextSetWithClosure];
                        dfa.TransitionFunction[(currentState, symbol)] = nextState;
                    }
                }
            }

            return dfa;
        }

        private HashSet<int> EpsilonClosure(NFA nfa, int state)
        {
            var closure = new HashSet<int> { state };
            var stack = new Stack<int>();
            stack.Push(state);

            while (stack.Count > 0)
            {
                int current = stack.Pop();

                foreach (var (from, to) in nfa.EpsilonTransitions)
                {
                    if (from == current && closure.Add(to))
                    {
                        stack.Push(to);
                    }
                }
            }

            return closure;
        }

        private class SetComparer : IEqualityComparer<HashSet<int>>
        {
            public bool Equals(HashSet<int> x, HashSet<int> y)
            {
                return x.SetEquals(y);
            }

            public int GetHashCode(HashSet<int> obj)
            {
                int hash = 17;
                foreach (int item in obj.OrderBy(x => x))
                {
                    hash = hash * 31 + item.GetHashCode();
                }
                return hash;
            }
        }

        public void PrintParseTree()
        {
            string postfix = ToPostfix();
            ParseNode tree = BuildParseTree(postfix);

            Console.WriteLine("\n=== Parse Tree ===");
            Console.WriteLine($"Postfix expression: {postfix}");
            Console.WriteLine("\nTree structure:");
            PrintTreeNode(tree, 0);
        }

        private void PrintTreeNode(ParseNode node, int level)
        {
            if (node == null)
                return;

            string indent = new string(' ', level * 4);

            if (node.IsLeaf())
            {
                Console.WriteLine($"{indent}Leaf: '{node.Value}'");
            }
            else
            {
                Console.WriteLine($"{indent}Operator: '{node.Value}'");
                if (node.Left != null)
                {
                    Console.WriteLine($"{indent}  Left:");
                    PrintTreeNode(node.Left, level + 1);
                }
                if (node.Right != null)
                {
                    Console.WriteLine($"{indent}  Right:");
                    PrintTreeNode(node.Right, level + 1);
                }
            }
        }

        private ParseNode BuildParseTree(string postfix)
        {
            var stack = new Stack<ParseNode>();

            foreach (char c in postfix)
            {
                if (c == '|' || c == '.' || c == '*' || c == '+')
                {
                    ParseNode node = new ParseNode { Value = c };

                    if (c == '*' || c == '+')
                    {
                        if (stack.Count > 0)
                            node.Left = stack.Pop();
                    }
                    else
                    {
                        if (stack.Count > 0)
                            node.Right = stack.Pop();
                        if (stack.Count > 0)
                            node.Left = stack.Pop();
                    }

                    stack.Push(node);
                }
                else
                {
                    ParseNode leaf = new ParseNode { Value = c };
                    stack.Push(leaf);
                }
            }

            return stack.Count > 0 ? stack.Pop() : null;
        }
    }
}

public class ParseNode
{
    public char Value { get; set; }
    public ParseNode Left { get; set; }
    public ParseNode Right { get; set; }

    public bool IsLeaf()
    {
        return Left == null && Right == null;
    }
}