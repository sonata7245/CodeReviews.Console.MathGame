Game game = new Game();
bool keepPlaying = true;

while (keepPlaying)
{
    Console.WriteLine("====Main Menu====");
    Console.WriteLine("Please choose what you would like to do:");
    Console.WriteLine("1.New Round");
    Console.WriteLine("2.View Past Rounds");
    Console.WriteLine("3.Exit");

    string choice = Console.ReadLine();

    switch (choice)
    {
        case "1":
            Console.Clear();
            CreateNewRound(game);
            break;
        case "2":
            Console.Clear();
            ViewPastRounds(game);
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            break;
        case "3":
            keepPlaying = false;
            break;
        default:
            Console.Clear();
            Console.WriteLine("Invalid choice, please try again");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey();
            Console.Clear();
            break;
    }
}

void ViewPastRounds(Game game)
{
    for (int i = 0; i < game.pastRounds.Count(); i++)
    {
        Round pastRound = game.pastRounds[i];
        Console.WriteLine(
            $"For round {i + 1} you scored {pastRound.score} out of {pastRound.ListOfQuestions.Count}"
        );
        Console.WriteLine(
            $"it took you {pastRound.RoundLength.Minutes}m and {pastRound.RoundLength.Seconds}s"
        );
        Console.WriteLine(
            $"-----------------------------------------------------------------------"
        );
        Console.WriteLine($"Here are the questions and your answers:");
        foreach ((Question pastQuestion, int pastAnswer) in pastRound.PlayerAnswers)
        {
            Console.WriteLine(
                $"Qustion: {pastQuestion.QuestionText} Your Answer: {pastAnswer}, Correct Answer: {pastQuestion.Answer}"
            );
        }
    }
}

void CreateNewRound(Game game)
{
    int numQuestions;
    string operand;

    Console.WriteLine("Enter the number of questions you want to answer, you must enter a number 5 or greater.");
    do
    {
        numQuestions = int.Parse(Console.ReadLine());
    } while (numQuestions < 5);

    Console.WriteLine("Enter the operand for the questions you want to answer");
    Console.WriteLine("Please enter +, -, *, or /");

    do
    {
        operand = Console.ReadLine();
    } while (operand is not ("+" or "-" or "*" or "/"));

    game.NewRound(numQuestions, operand);
}


class Game
{
    public Round round { get; private set; }
    public List<Round> pastRounds { get; private set; } = new();

    public Game() { }

    public void NewRound(int numQuestions, string operand)
    {
        round = new Round(numQuestions, operand);
        foreach (Question question in round.ListOfQuestions)
        {
            Console.WriteLine(question.QuestionText);
            int answer = int.Parse(Console.ReadLine());
            round.AnswerQuestion(question, answer);
        }
        round.EndRound();
        ArchiveRound(round);
    }

    private void ArchiveRound(Round currentRound)
    {
        pastRounds.Add(currentRound);
    }
}

class Round
{
    public List<Question> ListOfQuestions { get; private set; } = new();
    public int score { get; private set; }
    private DateTime startTime;
    public TimeSpan RoundLength { get; private set; }

    public List<(Question, int)> PlayerAnswers { get; private set; } = new();

    public Round(int numQuestions, string operand)
    {
        score = 0;
        startTime = DateTime.Now;
        QuestionGenerator(numQuestions, operand);
    }

    private void QuestionGenerator(int numQuestions, string operand)
    {
        for (int i = 0; i < numQuestions; i++)
        {
            ListOfQuestions.Add(new Question(operand));
        }
    }

    public void AnswerQuestion(Question question, int answer)
    {
        PlayerAnswers.Add((question, answer));
        if (question.CheckAnswer(answer))
        {
            score++;
            Console.WriteLine("You got it right!");
        }
        else
        {
            Console.WriteLine("Sorry that is incorrect");
        }
    }

    public void EndRound()
    {
        RoundLength = (DateTime.Now - startTime);
        Console.WriteLine(
            $"End of round, your score was {score}, and it took {RoundLength.Minutes}m {RoundLength.Seconds}s"
        );
    }
}

class Question
{
    private int firstNum;
    private int secondNum;
    private string operand;

    // Properties to expose the text and the correct answer
    public string QuestionText { get; private set; }
    public int Answer { get; private set; }

    public Question(string Operand)
    {
        operand = Operand;
        firstNum = Random.Shared.Next(0, 101);
        secondNum = Random.Shared.Next(0, 101);

        CalculateAnswer();
    }

    private void CalculateAnswer()
    {
        if (operand != "/")
        {
            QuestionText = $"{firstNum} {operand} {secondNum} = ?";
        }
        Answer = operand switch
        {
            "+" => firstNum + secondNum,
            "-" => firstNum - secondNum,
            "*" => firstNum * secondNum,
            "/" => divisionAnswer(firstNum, secondNum),
            _ => 0,
        };
    }

    private int divisionAnswer(int dividend, int divisor)
    {
        do
        {
            dividend = Random.Shared.Next(0, 101);
            divisor = Random.Shared.Next(1, 101);
        } while (dividend % divisor != 0);
        QuestionText = $"{dividend} / {divisor} = ?";
        return dividend / divisor;
    }

    public bool CheckAnswer(int answer)
    {
        if (answer == Answer)
            return true;
        else
            return false;
    }
}
