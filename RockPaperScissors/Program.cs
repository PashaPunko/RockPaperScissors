using System;
using System.Text;
using System.Security.Cryptography;
using System.Collections.Generic;
namespace RockPaperScissors
{
    public class ComputerPlayer {
        private int computerNum;
        public int ComputerNum { get { return computerNum; } }
        private String key;
        public String Key { get{ return key;} }
        public ComputerPlayer() {
            Random random = new Random();
            byte[] randomBytes = new byte[16];
            random.NextBytes(randomBytes);
            this.key = BitConverter.ToString(randomBytes).Replace("-", String.Empty);
        }
        public String ChooseMove(string[] moves) {
            Random random = new Random();
            computerNum = random.Next(0, moves.Length);
            HMACSHA256 sha256 = new HMACSHA256(Encoding.ASCII.GetBytes(this.key));
            Byte[] hash = sha256.ComputeHash(Encoding.ASCII.GetBytes(moves[computerNum]));
            return BitConverter.ToString(hash).Replace("-", String.Empty);
        }
    }

    class Program {
        static public string WhoWin(int opponentsNum, int computerNum, int moviesLen)
        {
            opponentsNum += opponentsNum < computerNum ? moviesLen : 0;
            opponentsNum -= computerNum;
            if (opponentsNum > (moviesLen - 1) / 2) return "Computer win";
            else if (opponentsNum != 0) return "You win";
            else return "Nobody win";
        }
        static public void ShowMenu(string[] moves) {
            Console.WriteLine("Available moves:");
            for (int i = 1; i <= moves.Length; i++) {
                Console.WriteLine("{0} - {1}", i, moves[i-1]);
            }
            Console.WriteLine("0 - Exit");
        }
        static public bool ValidateInput(string[] moves)
        {
            if (moves.Length < 3) {
                Console.WriteLine("Expected 3 or more parameters. For example: rock paper scissors lizard Spock");
                return false;
            }
            else if (moves.Length % 2 == 0) {
                Console.WriteLine("Expected odd number of parameters. For example: rock paper scissors lizard Spock");
                return false;
            }
            else {
                SortedSet<string> uniques = new SortedSet<string>(moves);
                if (uniques.Count != moves.Length) {
                    Console.WriteLine("Expected unique parameters. For example: rock paper scissors lizard Spock");
                    return false;
                }
            }
            return true;
        }
        static void Main(string[] args) {
            if (ValidateInput(args)) {
                ComputerPlayer player = new ComputerPlayer();
                string computerMove = player.ChooseMove(args);
                int opponentsNum = 0;
                do {
                    Console.Clear();
                    Console.WriteLine("HMAC SHA256:\n{0}", computerMove);
                    ShowMenu(args);
                    Console.Write("Enter your move: ");
                    try
                    {
                        opponentsNum = Int32.Parse(Console.ReadLine());
                    }
                    catch(Exception e) {
                        opponentsNum = -1;
                    }
                } while (opponentsNum > args.Length || opponentsNum < 0);
                if (opponentsNum != 0) {
                    Console.WriteLine("Your move: {0}", args[opponentsNum - 1]);
                    Console.WriteLine("Computer move: {0}", args[player.ComputerNum]);
                    Console.WriteLine(WhoWin(opponentsNum - 1, player.ComputerNum, args.Length));
                    Console.WriteLine("HMAC key: {0}", player.Key);
                }
                else Console.WriteLine("Have a nice day!");
            }
        }
    }
}
