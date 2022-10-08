using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoneyServer.utils
{
    static class Input
    {

        public static (Dictionary<int, string>, Dictionary<int, string>, string[,], string[,], List<int>, string, int, int) input(string arg)
        {

            Dictionary<int, string> _boneyMap = new Dictionary<int, string>();
            Dictionary<int, string> _bankMap = new Dictionary<int, string>();
            string[,] _serverState;
            string[,] _serverSuspect;
            List<int> clientList = new List<int>();
            int _numberSlots = 1;
            string _timeOfFirstSlot = "";
            int _slotDuration = 0;
            string[] lines = File.ReadAllLines(arg);
            string[] words;
            int global = 1;
            string pal1 = "";
            string pal2 = "";
            _serverState = new string[100, 100];
            _serverSuspect = new string[100, 100];
            foreach (string line in lines)
            {

                words = line.Split(' ');
                if (words[0] == "P")
                {
                    if (words[2] == "boney") _boneyMap.Add(int.Parse(words[1]), words[3]);
                    if (words[2] == "bank") _bankMap.Add(int.Parse(words[1]), words[3]);
                    if (words[2] == "client") clientList.Add(int.Parse(words[1]));
                }
                else if (words[0] == "S")
                {
                    _numberSlots = int.Parse(words[1]);

                }
                else if (words[0] == "T")
                {
                    _timeOfFirstSlot = words[1];
                }
                else if (words[0] == "D")
                {
                    _slotDuration = int.Parse(words[1]);
                }
                else if (words[0] == "F")
                {
                    int count = 1;
                    int aux = 1;
                    int end = 1;
                    int bit = 1;
                    string s = line;
                    string[] data = s.Split(new string[] { "(", ")" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] data_final = new string[data.Length];
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i] != null) data_final[i] = data[i];
                    }
                    foreach (string word in data_final)
                    {
                        string[] final = word.Split(',');
                        foreach (string palavra in final)
                        {
                            if (count == 1)
                            {
                                count = 0;
                                continue;
                            }
                            if (palavra != null || palavra != "" || palavra != "\n")
                            {
                                if (bit == 1) pal1 = palavra;
                                if (bit == 2) pal2 = palavra;
                                if (bit == 1) bit = 2;
                                else bit = 1;
                                //Console.WriteLine(palavra);
                            }

                        }
                        if (aux % 2 == 0)
                        {

                            //Console.WriteLine(end);

                            _serverState[global, end] = pal1;
                            _serverSuspect[global, end] = pal2;

                            end++;
                        }
                        count = 1;
                        aux++;

                    }
                    global++;
                }

            }
            return (_boneyMap, _bankMap, _serverState, _serverSuspect, clientList, _timeOfFirstSlot, _numberSlots, _slotDuration);



            /*foreach (KeyValuePair<int, String> entry in _bankMap)
            {
                Console.WriteLine("Bank: " + entry.Key + "URL: " + entry.Value);

            }

            foreach (KeyValuePair<int, String> entry in _boneyMap)
            {
                Console.WriteLine("Bank: " + entry.Key + "URL: " + entry.Value);

            }

            Console.WriteLine("Number of slots: " + _numberSlots);

            Console.WriteLine("time " + _timeOfFirstSlot);
            Console.WriteLine("Duration: " + _slotDuration);

            int loko = 1;
            for (int i = 1; i <= _numberSlots; i++)
            {
                for (int l = 1; l <= _bankMap.Count + _boneyMap.Count; l++)
                {
                    Console.WriteLine("In slot :" + i + "Process: " + l + "= " + _serverSuspect[i, l]);
                }
            }*/




        }
    }
}
