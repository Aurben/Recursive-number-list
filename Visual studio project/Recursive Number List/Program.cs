using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Recursive_Number_List
{
    class Program
    {
        static void Main(string[] args)
        {
            List<NumWithList> numList = NumWithList.GenerateInitialList(100,5,2, new Random());
            DisplayController a = new DisplayController(numList);
            
            a.StartConsoleDialog();
        }
    }

    class NumWithList:IComparable<NumWithList>
    {
        int depth; // Value showing how many lists are above this list

        /*public NumOrList(int maxValue, int maxItems, int maxDepth = 0)
        {
            Random rand = new Random();
            List = new List<NumOrList>();
            depth = 0;

            value = (rand.Next(maxValue));


            //50% chance to make another list when Depth can be added to the list
            if (rand.Next(2) == 1 && maxDepth > depth)
            {

                for (int i = 0; i < maxItems; i++)
                {
                    var newItem = new NumOrList(maxValue, maxItems, maxDepth, this.depth + 1);
                    List.Add(newItem);
                }
            }
        }*/

        public NumWithList(int maxValue, int maxItems, int maxDepth, int depth, Random rand)
        {
            List = new List<NumWithList>();
            this.depth = depth;

            //Assign a value to the item
            Value = (rand.Next(maxValue))+1;

            //50% chance to make another list when depth can be added to the list
            if (rand.Next(2) == 1 && maxDepth > depth)
            {
                for (int i = 0; i < rand.Next(maxItems) + 1; i++)
                {
                    var newList = new NumWithList(maxValue, maxItems, maxDepth, this.depth + 1, rand);
                    List.Add(newList);
                }
            }
        }

        public static List<NumWithList> GenerateInitialList(int maxValue, int maxItems, int maxDepth, Random rand)
        {
            var newList = new List<NumWithList>();

            for (int i = 0; i < maxItems; i++)
            {
                var newItem = new NumWithList(maxValue, maxItems, maxDepth, 0, rand);
                newList.Add(newItem);
            }

            return newList;
        }

        internal List<NumWithList> List { get; set; }

        //The sum of the list items
        public int Sum {  get
            {
                int sum = 0;
                foreach (var a in List)
                {
                    sum += a.Value;
                }
                return sum;
            }
        } 

        public int Value { get; private set; }

        public void Print(string name)
        {
            //Insert tab indents based on depth
            for (int i = 0; i < depth; i++)
            {
                Console.Write("\t");
            }
            Console.WriteLine($"- Item {name} {Value}");

            //If this has a list, print that list as well
            if (List.Count !=0)
            {
                for (int i = 0; i < List.Count; i++)
                {
                    List[i].Print(name + DisplayController.SwitchNumAndChar(i));
                }
            }
        }


        //Sorting methods
        public void SortList(bool ascending, bool sortAll= false)
        {
            if (ascending)
                List.Sort((a, b) => a.Value.CompareTo(b.Value));
            else
                List.Sort((a, b) => b.Value.CompareTo(a.Value));

            //Sort the inner lists as well
            if(sortAll)
                foreach (var item in List)
                {
                    item.SortList(ascending);
                }
        }

        public int CompareTo(NumWithList other)
        {
            return Value.CompareTo(other.Value);
        }        
    }
    
    class DisplayController
    {
        List<NumWithList> list;

        public DisplayController(List<NumWithList> list)
        {
            this.list = list;
        }

        public void StartConsoleDialog()
        {
            Console.WriteLine("This program creates a list of numbers that can have a list within itself as well.");
            PrintHelp();

            string input;
            do
            {
                Console.Write("Awaiting command: ");
                input = ReadInput();
            } while (input.ToLower() != "exit");

        }

        private void PrintHelp()
        {
            Console.WriteLine("Available commands:");
            Console.WriteLine("help - dislpays available commands.");
            Console.WriteLine("print <main || item name> - displays the main list or the list of the selected item.");
            Console.WriteLine("sum <main || item name> - displays the sum of the values in the selected list.");
            Console.WriteLine("generate <max value> <max list size> <max depth> - generates a new list with the specified values.");
            Console.WriteLine("sort <ascending || descending> - sorts the main list and all lists within it in the specified order.");
            Console.WriteLine("sort <main || item name> <ascending || descending> - sorts the selected list in the specified order.");
            Console.WriteLine("sort <item name to move> <name of the position to move to> - moves the selected item to the specified position.");
            Console.WriteLine("exit - end the program.");
        }

        private string ReadInput()
        {
            string input = "";
            input = Console.ReadLine();
            var commands = input.Split(' ');

            try
            {
                switch (commands[0].ToLower())
                {
                    case "help":
                        PrintHelp();
                        break;
                    case "print":
                        Print(commands[1]);
                        break;
                    case "sum":
                        PrintSum(commands[1]);
                        break;
                    case "generate":
                        GenerateNewList(commands);                        
                        break;
                    case "sort":
                        RunSortCommand(commands);
                        break;
                    case "exit":
                        Console.WriteLine("Application ended.");
                        break;
                    default:
                        Console.WriteLine("Command not recognized.");
                        break;
                }
            }
            catch
            {
                Console.WriteLine("Command not recognized.");
            }

            return input;
        }

        //Identifies and runs the selected sort command
        private void RunSortCommand(string[] commands)
        {
            //Sort all command
            if (commands.Count() == 2)
            {
                SortList(commands[1], true);
                Console.WriteLine("Sorting all items.");
            }
            else
            {
                //Sort selected item command
                if (commands[2].ToLower() == "ascending" || commands[2].ToLower() == "descending")
                {
                    //Sort only the named item
                    if (commands[1].ToLower() == "main")
                        SortList(commands[2]);
                    else
                        FindItem(commands[1].ToLower()).SortList(commands[2].ToLower() == "ascending");
                    Console.WriteLine("Sorting specified list.");
                }
                else
                {
                    //Sort - move item command
                    MoveToPosition(commands[1], commands[2]);
                    Console.WriteLine("Moved item to new position.");
                }
            }
        }

        private void GenerateNewList(string[] commands)
        {
            int[] providedVariables = new int[3];
            for (int i = 0; i < 3; i++)
            {
                //If provided variables aren't integers, abort command
                if (!int.TryParse(commands[i + 1], out providedVariables[i]))
                {
                    Console.WriteLine("Provided variables need to be integers. Generation aborted.");
                    return;
                }
            }
            list = NumWithList.GenerateInitialList(providedVariables[0], providedVariables[1], providedVariables[2], new Random());
            Console.WriteLine("New list generated.");
        }

        private NumWithList FindItem(string itemName)
        {
            List<NumWithList> listToLookIn = list;
            int index; // The index of the item in the list to look in

            try
            {
                index = SwitchNumAndChar(itemName[0]);
                
                for (int i = 1; i < itemName.Length; i++)
                {
                    listToLookIn = listToLookIn[index].List;
                    index = SwitchNumAndChar(itemName[i]);
                }
                return listToLookIn[index];
            }
            catch
            {
                Console.WriteLine("Item '" + itemName + "' was not found.");
                return null;
            }
        }

        private void MoveToPosition(string itemName, string newPositionName)
        {
            var listHoldingItem = list;
            var listHoldingNewPosition = list;
            int itemIndex = SwitchNumAndChar(itemName[itemName.Length-1]);
            int newPositionIndex = SwitchNumAndChar(newPositionName[newPositionName.Length - 1]);

            if (itemName == newPositionName)
            {
                Console.WriteLine("Would move to the same position. Move canceled.");
                return;
            }

            try
            {
                //Finding list with the item to move
                if (itemName.Length > 1)
                {
                    string parent = itemName.Substring(0, itemName.Length - 1);
                    listHoldingItem = FindItem(parent).List;
                }

                //Checking if the item is in the list
                if (listHoldingItem.Count < itemIndex)
                {
                    throw new Exception();
                }
            }
            catch
            {
                Console.WriteLine("Item to move was not found.");
            }            

            try
            {
                //Finding list with the position to move to
                if (itemName.Length > 1)
                {
                    string newPositionParent = newPositionName.Substring(0, newPositionName.Length - 1);
                    listHoldingNewPosition = FindItem(newPositionParent).List;
                }
            }
            catch
            {
                Console.WriteLine("Position to move was not found.");
            }

            //If the item and position are found and the new position wouldn't leave gaps, move item to new position
            //Moving one position ahead in its own list would leave a gap if allowed, otherwise adding to the end of the list is fine.
            bool canMove = false;

            if (listHoldingNewPosition.Count > newPositionIndex ||
                listHoldingNewPosition.Count == newPositionIndex && listHoldingNewPosition != listHoldingItem)
                canMove = true;

            if (canMove)
            {
                var item = listHoldingItem[itemIndex];
                listHoldingItem.RemoveAt(itemIndex);
                listHoldingNewPosition.Insert(newPositionIndex, item);
            }
            else
            {
                Console.WriteLine("The new position would leave gaps in the list. Move is canceled.");
            }            
        }

        //Prints the information off the whole list, or of a specific item if it is named
        public void Print(string itemName ="")
        {
            if (itemName != "" && itemName != "main")
            {
                try
                {
                    FindItem(itemName).Print(itemName);
                }
                catch
                {
                }
            }
            else
            {
                for (int i = 0; i < list.Count; i++)
                {
                    list[i].Print(SwitchNumAndChar(i)+"");
                }
            }
        }

        public void PrintSum(string itemName ="")
        {
            if (itemName != "" && itemName != "main")
            {
                try
                {
                    Console.WriteLine("Sum item " + itemName + " = " + FindItem(itemName).Sum);
                }
                catch
                {
                }
            }
            else
            {
                int sum = 0;
                foreach (var item in list)
                {
                    sum += item.Value;
                }
                Console.WriteLine("Sum of main = " + sum);
            }
        }

        //Converts a displayed character to its equivalent value
        public static int SwitchNumAndChar(char a)
        {
            return a - 97;
        }

        //Converts a number to its displayed character
        public static char SwitchNumAndChar(int a)
        {
            return (char)(97 + a);
        }

        //Sorts all or lists or just the main list in the selected order. Currently supports descending and ascending by value
        public void SortList(string order, bool sortAll = false)
        {
            bool ascending = false;
            if (order.ToLower() == "ascending")
                ascending = true;
            else
                if (order.ToLower() != "descending")
            {
                Console.WriteLine("Specified sorting order is invalid.");
                return;
            }
            
            if (ascending)
                list.Sort((a, b) => a.Value.CompareTo(b.Value));
            else
                list.Sort((a, b) => b.Value.CompareTo(a.Value));

            //Sort inner lists as well if sortAll is true
            if (sortAll)
                foreach (var item in list)
                {
                    item.SortList(ascending,sortAll);
                }
            Console.WriteLine("List sorted.");
        }
    }
}