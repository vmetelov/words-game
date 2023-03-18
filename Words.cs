int[] pcUsedWordsCounters = new int[26]; // Counters of used PC words starting with each letter.
List<string> usedWords = new();
string[][] pcWords = new string[26][];

string filePath = "pcWords.txt"; // Path to file with words.

if (FillPCWords(pcWords, filePath))
{
    bool isEndGame = false;
    char firstLetter = 'a';

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Welcome to Words game!");
    Console.WriteLine();
    Console.WriteLine("Available commands:");
    Console.WriteLine("xxx      - get prompt");
    Console.WriteLine("qqq      - end the game");
    Console.WriteLine("_add_    - add new words to PC dictionary");
    Console.WriteLine("_check_  - perform check for possible duplicates.");
    Console.WriteLine();
    Console.ForegroundColor = ConsoleColor.Gray;

    while (!isEndGame)
    {
        PCWord(ref isEndGame, ref firstLetter, pcUsedWordsCounters, usedWords, pcWords); // PC first - to user can see, at which letter he need to write his word.
        UserWord(ref isEndGame, ref firstLetter, pcUsedWordsCounters, usedWords, pcWords);
    }

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine();
    Console.WriteLine("Game over!");
    Console.WriteLine($"Total quantity of used words = {usedWords.Count}.");
    Console.ForegroundColor = ConsoleColor.Gray;

    SavePCWords(pcWords, filePath);
}



void UserWord(ref bool isEndGame, ref char firstLetter, int[] pcUsedWordsCounters, List<string> usedWords, string[][] pcWords)
{
    string? input;
    bool isWrongAnswer = true;

    while (isWrongAnswer)
    {
        Console.Write("User: ");
        input = Console.ReadLine();

        if (string.IsNullOrEmpty(input))
        {
            continue;
        }
        else if (input == "qqq")
        {
            isEndGame = true;
            break;
        }
        else if (input == "xxx") // Use prompt.
        {
            PCWord(ref isEndGame, ref firstLetter, pcUsedWordsCounters, usedWords, pcWords);
            break;
        }
        else if (input == "_add_")
        {
            AddMultipleNewWords(pcWords);
            isEndGame = true;
            break;
        }
        else if (input == "_check_")
        {
            CheckDuplicates(pcWords);
            isEndGame = true;
            break;
        }
        else if (input[0] == firstLetter && input.All(char.IsLower))
        {
            if (IsUnique(input, usedWords)) // Check if this word was already used.
            {
                if (!IsKnown(input, pcWords, firstLetter)) // Check if this word present in PC dictionary.
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("I don't know such word. Are you sure? (y/n)");
                    string? answer = Console.ReadLine();
                    if (string.IsNullOrEmpty(answer))
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        continue;
                    }
                    else if (answer.ToLowerInvariant() == "y")
                    {
                        AddNewWord(input, pcWords, firstLetter);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Word '{input}' remembered.");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        continue;
                    }
                }

                AddUsedWord(input, usedWords);
                isWrongAnswer = false;
                firstLetter = input[^1]; // Remember letter for next player.
                /* If it will be needed - uncomment and test
                // If all existent common words on that letter are used, use previous letter
                if ((firstLetter == 'x' || firstLetter == 'y') && (pcUsedWordsCounters[firstLetter - 'a'] == pcWords[firstLetter - 'a'].Length))
                {
                    firstLetter = input[input.Length - 2];
                }
                */
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Word '{input}' was already used.");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Wrong word.");
            Console.ForegroundColor = ConsoleColor.Gray;
        }
    }
}

bool IsUnique(string word, List<string> usedWords)
{
    if (usedWords.Contains(word))
    {
        return false;
    }
    return true;
}

bool IsKnown(string word, string[][] pcWords, char firstLetter)
{
    int orderNumber = firstLetter - 'a';

    for (int counter = 0; counter < pcWords[orderNumber].Length; counter++)
    {
        if (pcWords[orderNumber][counter] == word)
        {
            return true;
        }
    }
    return false;
}

void AddUsedWord(string newWord, List<string> usedWords)
{
    usedWords.Add(newWord); // Remember used word.
}

void AddNewWord(string newWord, string[][] pcWords, char firstLetter)
{
    Array.Resize(ref pcWords[firstLetter - 'a'], pcWords[firstLetter - 'a'].Length + 1); // Resize appropriate letter words array by 1 more.
    pcWords[firstLetter - 'a'][^1] = newWord; // Remember new word.
}

void PCWord(ref bool isEndGame, ref char firstLetter, int[] pcUsedWordsCounters, List<string> usedWords, string[][] pcWords)
{
    int orderNumber = firstLetter - 'a';
    string word;
    bool isWrongAnswer = true;

    while (isWrongAnswer)
    {
        if (pcUsedWordsCounters[orderNumber] < pcWords[orderNumber].Length) // If PC have unused words on aproppriate letter:
        {
            word = pcWords[orderNumber][pcUsedWordsCounters[orderNumber]]; // Pick next word.
            pcUsedWordsCounters[orderNumber]++; // Increase counter of used words on this letter.
            Console.WriteLine("PC  : " + word); // Display the word.

            if (IsUnique(word, usedWords))
            {
                AddUsedWord(word, usedWords);
                isWrongAnswer = false;
                firstLetter = word[^1]; // Remember letter for next player.
                /* If it will be needed - uncomment and test
                // If all existent common words on that letter are used, use previous letter
                if ((firstLetter == 'x' || firstLetter == 'y') && (pcUsedWordsCounters[firstLetter - 'a'] == pcWords[firstLetter - 'a'].Length))
                {
                    firstLetter = word[word.Length - 2];
                }
                */
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Word '{word}' was already used");
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
        else // If PC has used all its words - game over.
        {
            isEndGame = true;
            break;
        }
    }
}

bool FillPCWords(string[][] pcWords, string filePath)
{
    try
    {
        using StreamReader reader = new(filePath);

        string input;
        char[] separators = { ' ' };

        for (int counter = 0; counter < pcWords.Length; counter++)
        {
            input = reader.ReadLine()!;
            pcWords[counter] = input.Split(separators, StringSplitOptions.RemoveEmptyEntries);
        }
        return true;
    }
    catch (FileNotFoundException)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("File with PC dictionary not found. Do you want to create new one and add new words to it? (y/n)");
        Console.ForegroundColor = ConsoleColor.Yellow;
        string? input = Console.ReadLine();
        if (string.IsNullOrEmpty(input))
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("It's impossible to play into the game without that file.");
            Console.ForegroundColor = ConsoleColor.Gray;
            return false;
        }
        else if (input.ToLowerInvariant() == "y")
        {
            Console.WriteLine("It's recommended to add at least one word starting with 'a'.");
            for (int counter = 0; counter < pcWords.Length; counter++)
            {
                pcWords[counter] = Array.Empty<string>();
            }

            AddMultipleNewWords(pcWords);

            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("The file successfully created!.");
            Console.ForegroundColor = ConsoleColor.Gray;
            return true;
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("It's impossible to play into the game without that file.");
            Console.ForegroundColor = ConsoleColor.Gray;
            return false;
        }
    }
}

void SavePCWords(string[][] pcWords, string filePath)
{
    File.WriteAllText(filePath, string.Empty); // Truncate file.

    using StreamWriter writer = new(filePath);
    for (int counterArrays = 0; counterArrays < pcWords.Length; counterArrays++)
    {
        for (int counterWords = 0; counterWords < pcWords[counterArrays].Length; counterWords++)
        {
            writer.Write(pcWords[counterArrays][counterWords] + " ");
        }

        writer.WriteLine();
    }
}

void AddMultipleNewWords(string[][] pcWords)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("To stop adding of words enter '_exit_'");

    while (true)
    {
        Console.WriteLine("Which word do you want to add?");
        string? input = Console.ReadLine();
        if (string.IsNullOrEmpty(input))
        {
            continue;
        }
        else if (input == "_exit_")
        {
            break;
        }
        else if (!input.All(char.IsLower))
        {
            continue;
        }

        char firstLetter = input[0];

        if (IsKnown(input, pcWords, firstLetter)) // Check if this word present in PC dictionary.
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Word '{input}' already present in PC dictionary.");
        }
        else
        {
            AddNewWord(input, pcWords, firstLetter);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Word '{input}' added.");
        }
        Console.ForegroundColor = ConsoleColor.Yellow;
    }
    Console.ForegroundColor = ConsoleColor.Gray;
}

void CheckDuplicates(string[][] pcWords)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Check is started.");
    string repeatedWords = "Removed repeated words: ";
    bool isCheckSuccessful = true;

    for (int counterArrays = 0; counterArrays < pcWords.Length; counterArrays++)
    {
        HashSet<string> knownElements = new();

        bool isArraySuccessful = true;
        int initialLength = pcWords[counterArrays].Length;

        for (int counterWords = 0; counterWords < pcWords[counterArrays].Length; counterWords++)
        {
            if (!knownElements.Add(pcWords[counterArrays][counterWords]))
            {
                isCheckSuccessful = isArraySuccessful = false;
                repeatedWords += pcWords[counterArrays][counterWords] + " "; // Not StringBuilder because here shouldn't be many duplicates.
            }
        }

        if (!isArraySuccessful)
        {
            Array.Resize(ref pcWords[counterArrays], knownElements.Count);
            knownElements.CopyTo(pcWords[counterArrays]);

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Letter {(char)('a' + counterArrays)} - initial quantity of words: {initialLength}, corrected quantity of words: {pcWords[counterArrays].Length}");
        }
    }

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("Check is finished.");

    if (isCheckSuccessful)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("No duplicates found.");
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(repeatedWords);
    }
    Console.ForegroundColor = ConsoleColor.Gray;
}