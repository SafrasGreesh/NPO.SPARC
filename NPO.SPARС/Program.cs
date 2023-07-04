using System;
using System.Collections.Generic;
using System.IO;

namespace NPO.SPARC
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine("Укажите путь к файлу в качестве аргумента командной строки.");
                return;
            }

            string filePath = args[0];

            try
            {
                string[] lines = File.ReadAllLines(filePath);
                int testBlockCount = int.Parse(lines[0]);
                int currentLineIndex = 2;

                for (int i = 0; i < testBlockCount; i++)
                {
                    if (i > 0)
                        Console.WriteLine();

                    int candidateCount = int.Parse(lines[currentLineIndex++]);
                    List<string> candidates = new List<string>();

                    for (int j = 0; j < candidateCount; j++)
                    {
                        candidates.Add(lines[currentLineIndex++]);
                    }

                    List<List<int>> votes = new List<List<int>>();
                    while (currentLineIndex < lines.Length && !string.IsNullOrEmpty(lines[currentLineIndex]))
                    {
                        string[] voteTokens = lines[currentLineIndex++].Split(' ');
                        List<int> vote = new List<int>();
                        foreach (string token in voteTokens)
                        {
                            if (int.TryParse(token, out int candidate))
                            {
                                vote.Add(candidate);
                            }
                            else
                            {
                                Console.WriteLine("Неверный формат голосования: " + token);
                                return;
                            }
                        }
                        votes.Add(vote);
                    }

                    if (votes.Count > 0)
                    {
                        CalculateWinner(candidates, votes);
                    }
                    else
                    {
                        Console.WriteLine("Голоса за кандидатов не найдены.");
                    }
                }
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл не найден: " + filePath);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Произошла ошибка: " + ex.Message);
            }
        }

        static void CalculateWinner(List<string> candidates, List<List<int>> votes)
        {
            Dictionary<string, int> candidateVotes = new Dictionary<string, int>();
            int totalVotes = votes.Count;

            foreach (string candidate in candidates)
            {
                candidateVotes[candidate] = 0;
            }

            foreach (List<int> vote in votes)
            {
                if (vote.Count > 0)
                {
                    int candidate = vote[0];
                    if (candidate > 0 && candidate <= candidates.Count)
                    {
                        candidateVotes[candidates[candidate - 1]]++;
                    }
                    else
                    {
                        Console.WriteLine("Недействительный голос: " + candidate);
                        return;
                    }
                }
            }

            foreach (var kvp in candidateVotes)
            {
                if (kvp.Value * 2 > totalVotes)
                {
                    Console.WriteLine(kvp.Key);
                    return;
                }
            }

            int minVotes = int.MaxValue;
            foreach (var kvp in candidateVotes)
            {
                if (kvp.Value < minVotes)
                {
                    minVotes = kvp.Value;
                }
            }

            List<string> eliminatedCandidates = new List<string>();
            foreach (var kvp in candidateVotes)
            {
                if (kvp.Value == minVotes)
                {
                    eliminatedCandidates.Add(kvp.Key);
                }
            }

            CalculateWinner(candidates, votes, eliminatedCandidates);
        }

        static void CalculateWinner(List<string> candidates, List<List<int>> votes, List<string> eliminatedCandidates)
        {
            List<List<int>> updatedVotes = new List<List<int>>();

            foreach (List<int> vote in votes)
            {
                List<int> updatedVote = new List<int>();
                foreach (int candidate in vote)
                {
                    if (!eliminatedCandidates.Contains(candidates[candidate - 1]))
                    {
                        updatedVote.Add(candidate);
                    }
                }
                updatedVotes.Add(updatedVote);
            }

            if (updatedVotes.Count > 0)
            {
                CalculateWinner(candidates, updatedVotes);
            }
            else
            {
                foreach (string candidate in eliminatedCandidates)
                {
                    Console.WriteLine(candidate);
                }
            }
        }
    }
}
