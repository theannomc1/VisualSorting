﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisualSorting.SortingAlgorithms
{
    public class SelectionSort : SortingBase
    {
        public SelectionSort(IEnumerable<int> numbers) : base(numbers)
        { }

        public override async void Sort()
        {
            for (int i = 0; i < NumberArray.Count() - 1; i++)
            {
                int arrayNum = i;
                for (int j = i + 1; j < NumberArray.Count(); j++)
                {
                    if (NumberArray.ElementAt(j).CompareTo(NumberArray.ElementAt(i)) < 0 && NumberArray.ElementAt(arrayNum).CompareTo(NumberArray.ElementAt(j)) > 0)
                    {
                        arrayNum = j;
                    }
                    await Sleep().ConfigureAwait(false);
                }
                // Swap
                NumberArray = Swap.SwapItem(NumberArray, i, arrayNum);
                TriggerNumbersUpdatedEvent();
            }
        }

        public override string ToString()
        {
            return "Selectionsort";
        }
    }
}
