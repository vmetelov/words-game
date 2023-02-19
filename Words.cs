int[] counter = new int[26]; // Counter of used PC words starting with each letter.
List<string> usedWords = new();
string[][] pcWords = new string[26][];

string file = "pcWords.txt"; // Path to file with words.

if (FillPCWords(pcWords, file))
{
    bool isEndGame = false;
    char letter = 'a';

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
        PCWord(ref isEndGame, ref letter, counter, usedWords, pcWords); // PC first - to user can see, at which letter he needs to write his word.
        UserWord(ref isEndGame, ref letter, counter, usedWords, pcWords);
    }

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine();
    Console.WriteLine("Game over!");
    Console.WriteLine($"Total quantity of used words = {usedWords.Count}.");
    Console.ForegroundColor = ConsoleColor.Gray;

    SavePCWords(pcWords, file);
}



void UserWord(ref bool isEndGame, ref char letter, int[] counter, List<string> usedWords, string[][] pcWords)
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
            PCWord(ref isEndGame, ref letter, counter, usedWords, pcWords);
            break;
        }
        else if (temp == "_add_")
        {
            AddMultipleNewWords(pcWords);
            isEndGame = true;
            break;
        }
        else if (temp == "_check_")
        {
            CheckDuplicates(pcWords);
            isEndGame = true;
            break;
        }
        else if (temp[0] == letter)
        {
            if (IsUnique(temp, usedWords)) // Check if this word was already used.
            {
                if (!IsKnown(temp, pcWords, letter)) // Check if this word present in PC dictionary.
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("I don't know such word. Are you sure? (y/n)");
                    string? input = Console.ReadLine();
                    if (string.IsNullOrEmpty(input))
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
                        continue;
                    }
                    else if (input.ToLowerInvariant() == "y")
                    {
                        AddNewWord(temp, pcWords, letter);
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine($"Word '{temp}' remembered.");
                        Console.ForegroundColor = ConsoleColor.Gray;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Gray;
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Word '{temp}' was already used.");
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

bool IsKnown(string word, string[][] pcWords, char letter)
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

void AddUsedWord(string newWord, List<string> usedWords)
{
    usedWords.Add(newWord); // Remember used word.
}

void AddNewWord(string newWord, string[][] pcWords, char letter)
{
    Array.Resize(ref pcWords[letter - 'a'], pcWords[letter - 'a'].Length + 1); // Resize appropriate letter words array by 1 more.
    pcWords[letter - 'a'][^1] = newWord; // Remember new word.
}

void PCWord(ref bool isEndGame, ref char letter, int[] counter, List<string> usedWords, string[][] pcWords)
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
            Console.WriteLine("PC  : " + temp); // Display the word.

            if (IsUnique(temp, usedWords))
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
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Word '{temp}' was already used");
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

bool FillPCWords(string[][] pcWords, string file)
{
    try
    {
        using StreamReader reader = new(file);

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
            Console.WriteLine("It's important to add at least one word for each alphabet letter.");
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

void SavePCWords(string[][] pcWords, string file)
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

void AddMultipleNewWords(string[][] pcWords)
{
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("To stop adding of words enter '_exit_'");

    while (true)
    {
        Console.WriteLine("Which word do you want to add?");
        string? temp = Console.ReadLine();
        if (string.IsNullOrEmpty(temp))
        {
            continue;
        }
        else if (temp == "_exit_")
        {
            break;
        }

        char letter = temp[0];

        if (IsKnown(temp, pcWords, letter)) // Check if this word present in PC dictionary.
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Word '{temp}' already present in PC dictionary.");
        }
        else
        {
            AddNewWord(temp, pcWords, letter);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Word '{temp}' added.");
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
        }

        if (pcWords[counterArrays].Length != initialLength)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Letter {(char)('a' + counterArrays)}, array initial length: {initialLength}, array current length: {pcWords[counterArrays].Length}");
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