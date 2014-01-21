using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples
{
    public class Sort
    {
        private readonly int[] _unsortedData;
        private readonly int[] _sortedData;
        private int _dataSize = 100;
        private Random _rdn;
        private void MixDataUp(int[] array, Random rdn)
        {
            for (int i = 0; i <= array.Length - 1; i++)
            {
                array[i] = (int)(rdn.NextDouble() * array.Length);
            }
        }

        private bool CompareArrays(int[] a1, int[] a2)
        {
            for (int i = 0; i < a1.Length; i++)
            {
                if (a1[i] != a2[i])
                {
                    return false;
                }
            }
            return true;
        }

        public Sort()
        {
            _rdn = new Random(_dataSize);
            _unsortedData = new int[_dataSize];
            MixDataUp(_unsortedData, _rdn);
            _sortedData = new int[_dataSize];
            _unsortedData.CopyTo(_sortedData, 0);
            Array.Sort(_sortedData);

          
        }

        public void BubbleSort()
        {
            var copy = new int[_dataSize];
            _unsortedData.CopyTo(copy,0);
            bool exchanges;
            do
            {
                exchanges = false;
                for (int i = 0; i < copy.Length - 1; i++)
                {
                    if (copy[i] > copy[i + 1])
                    {
                        // Exchange elements
                        int temp = copy[i];
                        copy[i] = copy[i + 1];
                        copy[i + 1] = temp;
                        exchanges = true;
                    }
                }
            } while (exchanges);

            if (!CompareArrays(copy, _sortedData))
            {
                throw new Exception("BubbleSort not impl correctly");
            }
        }

        public void OddEvenSort()
        {
            var copy = new int[_dataSize];
            _unsortedData.CopyTo(copy, 0);

            int temp;
            for (int i = 0; i < copy.Length / 2; ++i)
            {
                for (int j = 0; j < copy.Length - 1; j += 2)
                {
                    if (copy[j] > copy[j + 1])
                    {
                        temp = copy[j];
                        copy[j] = copy[j + 1];
                        copy[j + 1] = temp;
                    }
                }

                for (int j = 1; j < copy.Length - 1; j += 2)
                {
                    if (copy[j] > copy[j + 1])
                    {
                        temp = copy[j];
                        copy[j] = copy[j + 1];
                        copy[j + 1] = temp;
                    }
                }
            }

            if (!CompareArrays(copy, _sortedData))
            {
                throw new Exception("OddEvenSort not impl correctly");
            }
        }

        public void InsertionSort()
        {
            var copy = new int[_dataSize];
            _unsortedData.CopyTo(copy, 0);

            int n = copy.Length - 1;
            int i, j, temp;

            for (i = 1; i <= n; ++i)
            {
                temp = copy[i];
                for (j = i - 1; j >= 0; --j)
                {
                    if (temp < copy[j]) copy[j + 1] = copy[j];
                    else break;
                }
                copy[j + 1] = temp;
            }

            if (!CompareArrays(copy, _sortedData))
            {
                throw new Exception("InsertionSort not impl correctly");
            }
        }

        public void QuickSort()
        {
            var copy = new int[_dataSize];
            _unsortedData.CopyTo(copy, 0);
            QuickSortInternal(copy, 0, _dataSize - 1);

            if (!CompareArrays(copy, _sortedData))
            {
                throw new Exception("QuickSort not impl correctly");
            }
        }

        private void QuickSortInternal(int[] array, int left, int right)
        {
           

            int i, j;
            int pivot, temp;

            i = left;
            j = right;
            pivot = array[(left + right) / 2];

            do
            {
                while ((array[i] < pivot) && (i < right)) i++;
                while ((pivot < array[j]) && (j > left)) j--;

                if (i <= j)
                {
                    temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                    i++; j--;
                }
            } while (i <= j);

            if (left < j) QuickSortInternal(array, left, j);
            if (i < right) QuickSortInternal(array, i, right);
        }
    }
}
