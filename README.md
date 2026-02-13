# Regex to DFA Engine

![.NET](https://img.shields.io/badge/.NET-8.0-purple) ![C#](https://img.shields.io/badge/Language-C%23-blue) ![License](https://img.shields.io/badge/License-MIT-green)

A robust **Console Application** built with **C# and .NET 8.0** that visualizes the complete pipeline of compiling Regular Expressions into Deterministic Finite Automata (DFA).

This project was developed as part of the **Formal Languages and Compilers (LFC)** curriculum to demonstrate advanced algorithm implementation and data structure manipulation.

## 🚀 Key Features

* **Regex Parsing:** Converts standard regular expressions into **Postfix notation** (Reverse Polish Notation) for easier processing.
* **Automata Conversion:**
    * **Regex → NFA:** Implements **Thompson's Construction** algorithm to build a Nondeterministic Finite Automaton.
    * **NFA → DFA:** Implements the **Subset Construction** algorithm to convert to a Deterministic Finite Automaton.
* **Visualization:** Displays the state transition table (Delta function) directly in the console.
* **Validation:** Allows users to input words and validates if they are accepted by the generated language.
* **Export:** Saves the resulting DFA structure to a text file for external analysis.

## 🛠️ Technical Details

The application is built using modern C# practices and focuses on modular architecture:

* **Shunting-yard Algorithm:** Used to parse the regex and handle operator precedence (`*`, `|`, `.`, `+`).
* **Graph Representation:** Automata are modeled using `HashSet<int>` for states and `Dictionary<(int, char), int>` for high-performance transition lookups.
* **Object-Oriented Design:** * `NFA` class: Handles epsilon-transitions and non-deterministic paths.
    * `DeterministicFiniteAutomaton` class: Handles the final optimized state machine logic.

## 💻 How to Run

1.  Clone the repository:
    ```bash
    git clone [https://github.com/MoldovanPaulll/Regex-to-DFA-Engine.git](https://github.com/MoldovanPaulll/Regex-to-DFA-Engine.git)
    ```
2.  Open the `.sln` file in **Visual Studio 2022**.
3.  Build and Run the project (F5).
4.  Follow the on-screen menu to input your Regex (e.g., `(a|b)*abb`).

## 📂 Project Structure

* `RegexToDFA.cs` - Core logic for parsing and conversion algorithms.
* `NFA.cs` - Data structure for Nondeterministic Finite Automaton.
* `DeterministicFiniteAutomaton.cs` - Data structure for the final DFA and validation logic.
* `Program.cs` - Console UI and user interaction menu.

---
