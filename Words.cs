namespace Words
{
    class Program
    {
        static void Main()
        {
            int[] counter = new int[26]; // Counter of used PC words starting with each letter.
            List<string> usedWords = new();
            string[][] pcWords = new string[26][];

            string file = "pcWords.txt"; // Path to file with words.

            FillPCWords(ref pcWords, file);

            bool isEndGame = false;
            char letter = 'a';

            Console.WriteLine("Welcome to Words game!");
            Console.WriteLine("To get prompt enter 'xxx'.");
            Console.WriteLine("To end game enter 'qqq'.");

            while (!isEndGame)
            {
                PCWord(ref isEndGame, ref letter, ref counter, usedWords, pcWords); // PC first - for user can see, at witch letter he need write his word.
                UserWord(ref isEndGame, ref letter, ref counter, usedWords, ref pcWords);
            }


            Console.WriteLine("Game over!");
            Console.WriteLine($"Total quantity of used words = {usedWords.Count}.");


            SavePCWords(pcWords, file);
        }

        static void UserWord(ref bool isEndGame, ref char letter, ref int[] counter, List<string> usedWords, ref string[][] pcWords)
        {
            string? temp;
            bool isWrongAnswer = true;

            while (isWrongAnswer)
            {
                Console.Write("User: ");
                temp = Console.ReadLine();

                if (string.IsNullOrEmpty(temp))
                {
                    continue;
                }
                else if (temp == "qqq")
                {
                    isEndGame = true;
                    break;
                }
                else if (temp == "xxx") // Use prompt.
                {
                    PCWord(ref isEndGame, ref letter, ref counter, usedWords, pcWords);
                    break;
                }
                else if (temp == "_test_")
                {
                    TestMode(ref pcWords);
                    isEndGame = true;
                    break;
                }
                else if (temp[0] == letter)
                {
                    if (CheckUnique(temp, usedWords)) // Check if this word was already used.
                    {
                        if (!CheckExistence(temp, pcWords, letter)) // Check if this word exist in PC dictionary.
                        {
                            Console.WriteLine("I don't know such word. Are you sure? (y/n)");

                            if (Console.ReadLine() == "y")
                            {
                                AddNewWord(temp, ref pcWords, letter);
                                Console.WriteLine($"Word '{temp}' remembered.");
                            }
                            else
                            {
                                continue;
                            }
                        }

                        AddUsedWord(temp, usedWords);
                        isWrongAnswer = false;
                        letter = temp[^1]; // Remember letter for next player.
                        /* If it will be needed - uncomment and test
                        // If all existent common words on that letter are used, use previous letter
                        if ((letter == 'x' || letter == 'y') && (counter[letter - 'a'] == pcWords[letter - 'a'].Length))
                        {
                            letter = temp[temp.Length - 2];
                        }
                        */
                    }
                    else
                    {
                        Console.WriteLine($"Word '{temp}' was already used.");
                    }
                }
                else
                {
                    Console.WriteLine("Wrong word.");
                }
            }
        }

        static bool CheckUnique(string word, List<string> usedWords)
        {
            if (usedWords.Contains(word))
            {
                return false;
            }
            return true;
        }

        static bool CheckExistence(string word, string[][] pcWords, char letter)
        {
            int orderNumber = letter - 'a';

            for (int counter = 0; counter < pcWords[orderNumber].Length; counter++)
            {
                if (pcWords[orderNumber][counter] == word)
                {
                    return true;
                }
            }
            return false;
        }

        static void AddUsedWord(string newWord, List<string> usedWords)
        {
            usedWords.Add(newWord); // Remember used word.
        }

        static void AddNewWord(string newWord, ref string[][] pcWords, char letter)
        {
            Array.Resize(ref pcWords[letter - 'a'], pcWords[letter - 'a'].Length + 1); // Resize appropriate letter words array by 1 more.
            pcWords[letter - 'a'][^1] = newWord; // Remember new word.
        }

        static void PCWord(ref bool isEndGame, ref char letter, ref int[] counter, List<string> usedWords, string[][] pcWords)
        {
            int orderNumber = letter - 'a';
            string temp;
            bool isWrongAnswer = true;

            while (isWrongAnswer)
            {
                if (counter[orderNumber] < pcWords[orderNumber].Length) // If PC have unused words on aproppriate letter...
                {
                    temp = pcWords[orderNumber][counter[orderNumber]]; // Pick next word.
                    counter[orderNumber]++; // Increase counter of used words on this letter.
                    Console.WriteLine("PC  : " + temp); // Display word.

                    if (CheckUnique(temp, usedWords))
                    {
                        AddUsedWord(temp, usedWords);
                        isWrongAnswer = false;
                        letter = temp[^1]; // Remember letter for next player.
                        /* If it will be needed - uncomment and test
                        // If all existent common words on that letter are used, use previous letter
                        if ((letter == 'x' || letter == 'y') && (counter[letter - 'a'] == pcWords[letter - 'a'].Length))
                        {
                            letter = temp[temp.Length - 2];
                        }
                        */
                    }
                    else
                    {
                        Console.WriteLine($"Word '{temp}' was already used");
                    }
                }
                else // If PC used all words - end of game.
                {
                    isEndGame = true;
                    break;
                }
            }
        }

        static void FillPCWords(ref string[][] pcWords, string file)
        {
            using StreamReader reader = new(file);

            string input;
            char[] separators = { ' ' };

            for (int counter = 0; counter < pcWords.Length; counter++)
            {
                input = reader.ReadLine()!;
                pcWords[counter] = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            }
        }

        static void SavePCWords(string[][] pcWords, string file)
        {
            File.WriteAllText(file, string.Empty); // Truncate file.

            using StreamWriter writer = new(file);
            for (int counterArrays = 0; counterArrays < pcWords.Length; counterArrays++)
            {
                for (int counterWords = 0; counterWords < pcWords[counterArrays].Length; counterWords++)
                {
                    writer.Write(pcWords[counterArrays][counterWords] + " ");
                }

                writer.WriteLine();
            }
        }

        static void TestMode(ref string[][] pcWords)
        {
            string command;

            Console.WriteLine("Welcome to Test Mode!");
            while (true)
            {
                Console.WriteLine("Do you want to 'add' new word to PC dictionary, start his 'selftest' on repeated words or 'exit' of Test Mode?");
                command = Console.ReadLine()!;

                if (command == "add")
                {
                    Console.WriteLine("Which word do you want to add?");
                    string? temp = Console.ReadLine();
                    if (string.IsNullOrEmpty(temp))
                    {
                        continue;
                    }

                    char letter = temp[0];

                    if (CheckExistence(temp, pcWords, letter)) // Check if this word exist in PC dictionary.
                    {
                        Console.WriteLine($"Word '{temp}' already exist in PC dictionary.");
                    }
                    else
                    {
                        AddNewWord(temp, ref pcWords, letter);
                        Console.WriteLine($"Word '{temp}' added.");
                    }
                }
                else if (command == "selftest")
                {
                    Selftest(pcWords);
                }
                else if (command == "exit")
                {
                    break;
                }
            }
        }

        static void Selftest(string[][] pcWords)
        {
            Console.WriteLine("Selftest is started.");
            string repeatedWords = "Selftest failed. Repeated words: ";
            bool isSuccessful = true;

            for (int counterArrays = 0; counterArrays < pcWords.Length; counterArrays++)
            {
                HashSet<string> knownElements = new();

                for (int counterWords = 0; counterWords < pcWords[counterArrays].Length; counterWords++)
                {
                    if (!knownElements.Add(pcWords[counterArrays][counterWords]))
                    {
                        isSuccessful = false;
                        repeatedWords += pcWords[counterArrays][counterWords] + " ";
                    }
                }
            }

            if (isSuccessful)
            {
                Console.WriteLine("Selftest is successful.");
            }
            else
            {
                Console.WriteLine(repeatedWords);
            }
        }
    }
}
