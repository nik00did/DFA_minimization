using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DfaMinimization
{
    class DfaReader
    {
        public char[] Alphabet;              // поле для символов алфавита
        public int[] Numbers;                // номера переходов
        public int[,] Transition;            // поле для переходов
        public bool[] End;                   // конечные состояния

        private string[] FileReader(string path)  // возвращает массив строк из файла
        {
            string[] strFile = File.ReadAllLines(path, Encoding.Default);
            strFile = (from i in strFile orderby i select i).ToArray();
            return strFile;
        }

        public DfaReader()
        {
            Alphabet = new char[0];
            Numbers = new int[0];
            Transition = new int[0, 0];
        }

        public void FillDfa(string path)      // заполняет все поля класса 
        {
            string[] FileLines = FileReader(path);
            string[] ABC = FileLines[0].Split(' ');
            Alphabet = new char[ABC.Length-1];
            Alphabet[0] = Convert.ToChar(ABC[1]);
            Alphabet[1] = Convert.ToChar(ABC[2]);

            End = new bool[FileLines.Length-1];
            Transition = new int[FileLines.Length-1, Alphabet.Length];
            Numbers = new int[FileLines.Length-1];

            for (int i = 1; i < FileLines.Length; i++)
            {
                bool e = false;
                string[] temp = FileLines[i].Split(' ');
                int[] intTemp = new int[temp.Length];
                for (int j = 0; j < temp.Length; j++)
                {
                    intTemp[j] = Convert.ToInt32(temp[j]);
                    if (j == 3 && intTemp[3] == 0) e = true;
                }
                Numbers[i - 1] = intTemp[0];
                Transition[i - 1, 0] = intTemp[1];
                Transition[i - 1, 1] = intTemp[2];
                End[i - 1] = e;
            }
        }

        public int GetIndex(int state) // для получения индекса элемента по соответствующему состоянию
        {
            for (int i = 0; i < Numbers.Length; i++) if (Numbers[i]==state) return i;
            return state;
        }

        public bool CheckWord(string word) // проверяет соответствие строки языку (выводит "да" или "нет")

        {
            int state = Numbers[0];
            for (int i = 0; i < word.Length; i++)
            {
                if (word[i] == Alphabet[0]) state = Transition[this.GetIndex(state), 0];
                else
                {
                    if (word[i] == Alphabet[1])
                        state = Transition[GetIndex(state), 1];
                    else
                    {
                        ;
                    }
                }
            }
            for (int i = 0; i < Numbers.Length; i++) if (Numbers[i] == state)return End[i];
            return false;
        }

        public DfaReader MiniDfa() // минимизирует дфа
        {
            
            char[,] table;
            int[] intNumbers;
            string[] NewNumbers;
            int[,] NewTransition;
            bool[] newEnd;
            DfaReader newDfa = new DfaReader();
            newDfa.Alphabet = Alphabet;
            table = new char[Numbers.Length, Numbers.Length];

            for (int i = 0; i < Numbers.Length; i++)// заполняет всю табл +
            {
                for (int j = 0; j < Numbers.Length; j++) table[i, j] = '+';
            }

            for (int i = 0; i < Numbers.Length; i++)//решотки ниже главной и доллары на глав диагонал
            {
                table[i, i] = '$';
                for (int j = 0; j < i; j++) table[i, j] = '#';
            }

            for (int i = 0; i < Numbers.Length; i++) // декартовое произведение, раставляет звездочки на пересеч. кон. и некон. сост.
            {
                if (End[i] == true)
                {
                    for (int j = 0; j < Numbers.Length; j++)
                    {
                        if (!End[j]  && i<j) table[i, j] = '*';
                        if (! End[j] && j<i) table[j, i] = '*';                 
                    }
                }
            }
            //////////////////////
            int count = 0;
            do // в пустых(+) клетках ставит * или доллар в соответ с правилами:если оба бакс  ставим бакс, если хоть 1 * ставим *
            {
                int k = 0, l = 0, m = 0, n = 0;
                count = 0;
                for (int i = 0; i < Numbers.Length; i++)
                {
                    for (int j = 0; j < Numbers.Length; j++)
                    {
                        if (table[i, j] == '+')
                        {
                            for (int s = 0; s < Numbers.Length; s++)
                            {
                                if (Transition[i, 0] == Numbers[s]) k = s;
                                if (Transition[i, 1] == Numbers[s]) l = s;
                                if (Transition[j, 0] == Numbers[s]) m = s;
                                if (Transition[j, 1] == Numbers[s]) n = s;
                            }
                            if ((table[k, m] == '$' && table[l, n] == '$') || (table[m, k] == '$' && table[l, n] == '$') || (table[k, m] == '$' && table[n, l] == '$') || (table[m, k] == '$' && table[n, l] == '$'))
                            {
                                table[i, j] = '$';
                                count++;
                            }
                            if (table[k, m] == '*' || table[l, n] == '*' || table[m,k]=='*' || table[n,l]=='*')
                            {
                                table[i, j] = '*';
                                count++;
                            }
                        }
                    }
                }
            }
            while (count != 0);
            ///////////////////
            ///
            NewNumbers = new string[Numbers.Length];
            for (int i = 0, n = 0; i < Numbers.Length; i++,n++)// составляем новые имена состояний для конкатанаци строк
            {
                string temp = "";
                int counter = 0;
                for (int j = 0; j < Numbers.Length; j++)
                {
                    if (table[i, j] == '$')
                    {
                        temp += Numbers[j].ToString();
                        counter++;
                    }
                }
                NewNumbers[n] = temp;
            }

            string[] tempNumbers = NewNumbers;

            for (int i = 0; i < Numbers.Length; i++)//вытерает все одинаковые состояния 
            {
                for (int j = 0; j < Numbers.Length; j++)
                {
                    for (int k = 0; k < NewNumbers[j].Length; k++)
                    {
                        if (NewNumbers[i].Contains(tempNumbers[j][k]) && i!=j)
                        {
                            NewNumbers[i] += tempNumbers[j];
                            NewNumbers[j] = "";
                        }
                    }
                }

            }

            NewNumbers = NewNumbers.Where(i => i != "").ToArray();
            intNumbers = new int[NewNumbers.Length];
            newEnd = new bool[NewNumbers.Length];

            for (int i = 0; i < NewNumbers.Length; i++)//определяет кон сост в новом дфа
            {
                intNumbers[i] = Convert.ToInt32(NewNumbers[i][0].ToString());
                for (int k = 0; k < Numbers.Length; k++) if (NewNumbers[i].Contains(Numbers[k].ToString())) if(End[k]==true) newEnd[i] = true;
            }

            NewTransition = new int[NewNumbers.Length, Alphabet.Length];

            for (int i = 0; i < NewNumbers.Length; i++)//определ новые переходы
            {
                for (int j = 0; j < Numbers.Length; j++)
                {
                    if (NewNumbers[i].Contains(Numbers[j].ToString()))
                    {
                        NewTransition[i, 0] = Transition[j, 0];
                        NewTransition[i, 1] = Transition[j, 1];
                    }
                }
            }

            int[,] tempTr = NewTransition;
            for (int i = 0; i < NewNumbers.Length; i++)//дает новым переходам соотв имена
            {
                for (int j = 0; j < NewNumbers.Length; j++)
                {
                    if (NewNumbers[i].Contains(Convert.ToString(tempTr[j,0]))) NewTransition[j, 0] = intNumbers[i];
                    if (NewNumbers[i].Contains(Convert.ToString(tempTr[j, 1]))) NewTransition[j, 1] = intNumbers[i];
                }
            }
            newDfa.Numbers = intNumbers;
            newDfa.Transition = NewTransition;
            newDfa.End = newEnd;
            return newDfa;
        }

        public string printDfa()
        {
            string s="YOUR NEW DFA\n\n"+"   " + Alphabet[0] + " " + Alphabet[1]+"\n";
            for (int i = 0; i < Numbers.Length; i++) s+=Numbers[i] + " " + Transition[i, 0] + " " + Transition[i, 1] + " " + End[i]+"\n";
            return s;
        }
    }
}