public string ToPostfix()
{
    var result = new StringBuilder();
    var stack = new Stack<char>();

    var precedence = new Dictionary<char, int>
    {
        { '|', 0 },
        { '.', 1 },
        { '*', 2 },
        { '?', 2 },
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
                stack.Pop(); // Remove '('
        }
        else if (c == '|' || c == '.' || c == '*' || c == '?' || c == '+')
        {
            // Pentru operatori binari
            if (c == '|' || c == '.')
            {
                while (stack.Count > 0 && stack.Peek() != '(' &&
                       precedence.ContainsKey(stack.Peek()) &&
                       precedence[stack.Peek()] >= precedence[c])
                {
                    result.Append(stack.Pop());
                }
            }
            // Pentru operatori unari (*+?), scot de pe stiva toti operatorii cu prioritate mai mare
            else
            {
                while (stack.Count > 0 && stack.Peek() != '(' &&
                       (stack.Peek() == '*' || stack.Peek() == '+' || stack.Peek() == '?'))
                {
                    result.Append(stack.Pop());
                }
            }
            stack.Push(c);
        }
        else
        {
            // Literal
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