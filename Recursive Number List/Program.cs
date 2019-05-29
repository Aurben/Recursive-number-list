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
            Random rand = new Random();
            List<NumWithList> numList = NumWithList.GenerateInitialList(100,5,2,rand);
            DisplayController a = new DisplayController(numList);

            /*var numList = new List<Object> { 1, 5, 3 };
            list.Add(numList);
            int n = 0;

            list.Add(a);*/
            //Console.WriteLine(list[1]);

            /*Random rand = new Random();
            for (int i = 0; i < 3; i++)
            {
                Console.WriteLine(rand.Next(2));
            }*/
            a.Print();
            //Console.WriteLine((int)'a');
            string input = "";
            while (input != "x")
            {

                Console.Write("Item to print: ");
                input = Console.ReadLine();
                //Console.WriteLine( input + "  value is " + DisplayController.SwitchNumAndChar(input.ToCharArray()[0]));
                a.PrintSum(input);
            }

            a.SortList(false);
            Console.WriteLine("\nSorting list...\n");
            a.Print();
        }
    }

    class NumWithList:IComparable<NumWithList>
    {
        int depth;

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

        public void Print(string prefix = "")
        {
            //Inset tabs
            for (int i = 0; i < depth; i++)
            {
                Console.Write("\t");
            }
            Console.WriteLine($"- Item {prefix} {Value}");

            if (List.Count !=0)
            {
                for (int i = 0; i < List.Count; i++)
                {
                    List[i].Print(prefix + DisplayController.SwitchNumAndChar(i));
                }
            }
        }

        public void SortList(bool ascending)
        {
            if (ascending)
                List.Sort((a, b) => a.Value.CompareTo(b.Value));
            else
                List.Sort((a, b) => b.Value.CompareTo(a.Value));

            //Sort the inner lists as well
            foreach (var item in List)
            {
                item.SortList(ascending);
            }
        }
        //Sorting methods
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

        private NumWithList FindItem(string itemName)
        {

            bool itemExists = true; ; // if item isn't found, this is set to false
            List<NumWithList> listToLookIn = list;
            int index = GetIndexFromStringChar(itemName, 0); // The index of the item in the list to look in

            Console.WriteLine("Looking for value of item " + itemName);
            for (int i = 1; i < itemName.Length; i++)
            {
                if (index >= listToLookIn.Count || index < 0)
                {
                    itemExists = false;
                    break;
                }
                listToLookIn = listToLookIn[index].List;
                index = GetIndexFromStringChar(itemName, i);
            }

            if (index >= listToLookIn.Count || index < 0)
            {
                itemExists = false;
            }

            if (itemExists)
            {
                return listToLookIn[index];
            }
            else
            {
                Console.WriteLine("Item " + itemName + " was not found.");
                return null;
            }
        }

        public void Print(string itemName ="")
        {
            if (itemName != "")
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

        public void PrintSum(string itemName)
        {
            if (itemName != "")
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

        // Returns the value of an item names character, showings its position in the list
        private int GetIndexFromStringChar(string item, int charPosition)
        {
            char character = item.ToCharArray()[charPosition];
            return SwitchNumAndChar(character);            
        }

        //Converts a displayed character to its equivalent value
        public static int SwitchNumAndChar(char a)
        {
            return (int)a - 97;
        }

        //Converts a number to its displayed character
        public static char SwitchNumAndChar(int a)
        {
            return (char)(97 + a);
        }

        public void SortList(bool ascending)
        {
            if(ascending)
            list.Sort((a, b) => a.Value.CompareTo(b.Value));
            else
                list.Sort((a, b) => b.Value.CompareTo(a.Value));

            //Sort inner lists as well
            foreach (var item in list)
            {
                item.SortList(ascending);
            }
        }
    }
}