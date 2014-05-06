using System;

namespace Samples
{
    public class Search
    {
        private readonly Random _rdn;
        private readonly int[] _sortedData;
        private readonly int[] _unsortedData;
        private int _dataSize = 1000;


        public Search()
        {
            _rdn = new Random(_dataSize);
            _sortedData = new int[_dataSize];
            for (int i = 0; i < _dataSize; i++)
            {
                _sortedData[i] = i;
            }
            _unsortedData = new int[_dataSize];
            MixDataUp(_unsortedData, _rdn);
        }

        private void MixDataUp(int[] array, Random rdn)
        {
            for (int i = 0; i <= array.Length - 1; i++)
            {
                array[i] = (int) (rdn.NextDouble()*array.Length);
            }
        }

        public int LinearSearch()
        {
            int valueToFind = _rdn.Next(0, _dataSize);
            for (int i = 0; i < _unsortedData.Length; i++)
            {
                if (valueToFind == _unsortedData[i])
                {
                    return i;
                }
            }
            return -1;
        }

        public int BinarySearch()
        {
            int searchValue = _rdn.Next(0, _dataSize);
            int left = 0;
            int right = _sortedData.Length - 1;
            int mid;

            while (left <= right)
            {
                mid = (left + right)/2;
                if (_sortedData[mid] < searchValue) //the element we search is located to the right from the mid point
                {
                    left = mid + 1;
                }
                if (_sortedData[mid] > searchValue) //the element we search is located to the left from the mid point
                {
                    right = mid - 1;
                }
                    //at this point low and high bound are equal and we have found the element or
                    //arr[mid] is just equal to the value => we have found the searched element
                return mid;
            }
            return -1; //value not found
        }


        public int InterpolationSearch()
        {
            int searchValue = _rdn.Next(0, _dataSize);
            // Returns index of searchValue in sorted input data
            // array x, or -1 if searchValue is not found
            int low = 0;
            int high = _sortedData.Length - 1;
            int mid;

            while (_sortedData[low] < searchValue && _sortedData[high] >= searchValue)
            {
                mid = low + ((searchValue - _sortedData[low])*(high - low))/(_sortedData[high] - _sortedData[low]);

                if (_sortedData[mid] < searchValue)
                    low = mid + 1;
                else if (_sortedData[mid] > searchValue)
                    high = mid - 1;
                else
                    return mid;
            }

            if (_sortedData[low] == searchValue)
                return low;
            return -1; // Not found
        }
    }
}